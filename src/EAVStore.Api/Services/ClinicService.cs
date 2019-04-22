using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EAVStore.Api.ViewModels;
using EAVStore.DataAccess;
using EAVStore.DataAccess.Entities;
using EAVStore.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace EAVStore.Api.Services
{
    public class ClinicService : IClinicService
    {
        private readonly EavStoreDbContext _dbContext;

        public ClinicService(
            EavStoreDbContext dbContext
        ) {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task AddPatientAsync(CreatePatientVm patientDto) {
            var patient = new EavEntity {
                Id = Guid.NewGuid(),
                EntityType = EntityType.Patient,
                AttributeValues = new List<AttributeValueEntity> {
                    new AttributeValueEntity {AttributeType = AttributeType.PatientName, Value = patientDto.Name},
                    new AttributeValueEntity
                        {AttributeType = AttributeType.PatientAge, Value = patientDto.Age.ToString()},
                }
            };

            _dbContext.Entities.Add(patient);

            return _dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<PatientVm>> GetAllPatientsAsync(CancellationToken cancellationToken) {
            var entities = await _dbContext.Entities
                .AsNoTracking()
                .WithAttributes()
                .Where(x=>x.EntityType == EntityType.Patient)
                .ToArrayAsync(cancellationToken);

            var patientVms = entities
                .Select(EavToPatientVm)
                .ToArray();

            //May be changed to Parallel.ForEach collecting results into ConcurrentBug.
            //No need until amount of patients is not too big.
            foreach (var patientVm in patientVms) {
                patientVm.Operations = await GetOperationsAsync(patientVm.PatientId);
            }

            return patientVms.ToImmutableArray();
        }

        public async Task<PatientVm> GetPatientAsync(Guid patientId, CancellationToken cancellationToken) {
            //Will throw InvalidOperationException which will lead to 500.
            //TODO Should get FirstOrDefault, return null if not found and handled in controller as Entity Not Found (404)
            var patientVm = EavToPatientVm(
                await _dbContext.Entities
                    .WithAttributes()
                    .AsNoTracking()
                    .SingleAsync(x => x.Id == patientId && x.EntityType == EntityType.Patient, cancellationToken)
            );

            patientVm.Operations = await GetOperationsAsync(patientVm.PatientId);

            return patientVm;
        }

        public async Task AddOperationAsync(CreateOperationVm operationDto) {
            var entity = new EavEntity {
                Id = Guid.NewGuid(),
                EntityType = EntityType.Operation,
                AttributeValues = new List<AttributeValueEntity> {
                    new AttributeValueEntity
                        {AttributeType = AttributeType.PatientId, Value = operationDto.PatientId.ToString()},
                    new AttributeValueEntity
                        {AttributeType = AttributeType.OperationName, Value = operationDto.OperationName}
                }
            };

            _dbContext.Entities.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<IReadOnlyCollection<OperationVm>> GetOperationsAsync(Guid patientId) {
            var entities = await _dbContext.Entities
                .WithAttributes()
                .Where(
                    operation =>
                        operation.EntityType == EntityType.Operation &&
                        operation.AttributeValues.Single(av => av.AttributeType == AttributeType.PatientId)
                            .Value == patientId.ToString()
                )
                .AsNoTracking()
                .ToArrayAsync();

            return entities
                .Select(EavToOperationVm)
                .ToImmutableArray();
        }

        private static PatientVm EavToPatientVm(EavEntity entity) =>
            new PatientVm {
                PatientId = entity.Id,
                Name = entity.AttributeValues.Single(av => av.AttributeType == AttributeType.PatientName).Value,
                Age = ushort.Parse(
                    entity.AttributeValues.Single(av => av.AttributeType == AttributeType.PatientAge).Value
                )
            };

        private static OperationVm EavToOperationVm(EavEntity entity) =>
            new OperationVm {
                OperationId = entity.Id,
                OperationName = entity.AttributeValues.Single(av => av.AttributeType == AttributeType.OperationName)
                    .Value,
            };
    }
}