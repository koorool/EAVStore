using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EAVStore.Api.Services;
using EAVStore.Api.ViewModels;
using EAVStore.DataAccess;
using EAVStore.DataAccess.Entities;
using EAVStore.DataAccess.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EAVStore.Api.Tests
{
    public class ClinicServiceTests
    {
        private static EavStoreDbContext GetDbContext(string name) {
            return new EavStoreDbContext(
                new DbContextOptionsBuilder<EavStoreDbContext>()
                    .UseInMemoryDatabase(name)
                    .Options
            );
        }

        [Fact]
        public async Task CanAddPatient() {
            //Arrange
            var createPatientVm = new CreatePatientVm {
                Name = "PatientX",
                Age = 13
            };
            var dbContext = GetDbContext(nameof(CanAddPatient));
            var sut = new ClinicService(dbContext);

            //Action
            await sut.AddPatientAsync(createPatientVm);

            //Assert
            var eavEntity = await dbContext.Entities
                .AsNoTracking()
                .WithAttributes()
                .SingleAsync();

            eavEntity.Should().NotBeNull();
            eavEntity.EntityType.Should().Be(EntityType.Patient);

            eavEntity.AttributeValues.Should()
                .NotBeNullOrEmpty()
                .And.HaveCount(2);

            var name = eavEntity.AttributeValues.Single(x => x.AttributeType == AttributeType.PatientName).Value;
            name.Should().Be(createPatientVm.Name);

            var age = eavEntity.AttributeValues.Single(x => x.AttributeType == AttributeType.PatientAge).Value;
            age.Should().Be(createPatientVm.Age.ToString());
        }

        [Fact]
        public async Task CanGetPatient() {
            //Arrange
            var dbContext = GetDbContext(nameof(CanAddPatient));
            var patientId = Guid.NewGuid();
            const string patientName = "Jon Snow";
            const string patientAge = "24";
            var patientEntity = new EavEntity {
                Id = patientId,
                EntityType = EntityType.Patient,
                AttributeValues = new List<AttributeValueEntity> {
                    new AttributeValueEntity {
                        Id = Guid.NewGuid(),
                        AttributeType = AttributeType.PatientName,
                        Value = patientName
                    },
                    new AttributeValueEntity {
                        Id = Guid.NewGuid(),
                        AttributeType = AttributeType.PatientAge,
                        Value = patientAge
                    }
                }
            };

            var operationId = Guid.NewGuid();
            const string operationName = "Lymphadenectomy";
            var operationEntity = new EavEntity {
                Id = operationId,
                EntityType = EntityType.Operation,
                AttributeValues = new List<AttributeValueEntity> {
                    new AttributeValueEntity {
                        Id = Guid.NewGuid(),
                        AttributeType = AttributeType.OperationName,
                        Value = operationName
                    },
                    new AttributeValueEntity {
                        Id = Guid.NewGuid(),
                        AttributeType = AttributeType.PatientId,
                        Value = patientId.ToString()
                    }
                }
            };

            dbContext.Entities.AddRange(patientEntity, operationEntity);
            await dbContext.SaveChangesAsync();

            var sut = new ClinicService(dbContext);

            //Action
            var patient = await sut.GetPatientAsync(patientId, default);

            //Assert
            patient.Should().NotBeNull();
            patient.Name.Should().Be(patientName);
            patient.Age.Should().Be(ushort.Parse(patientAge));

            patient.Operations.Should().NotBeNullOrEmpty();
            var operation = patient.Operations.Single();
            operation.Should().NotBeNull();
            operation.OperationId.Should().Be(operationId);
            operation.OperationName.Should().Be(operationName);
        }

        [Fact]
        public async Task CanGetAllPatients() {
            //Arrange
            var dbContext = GetDbContext(nameof(CanAddPatient));
            var patientId = Guid.NewGuid();
            const string patientName = "Jon Snow";
            const string patientAge = "24";
            var patientEntities = new[] {
                new EavEntity {
                    Id = patientId,
                    EntityType = EntityType.Patient,
                    AttributeValues = new List<AttributeValueEntity> {
                        new AttributeValueEntity {
                            Id = Guid.NewGuid(),
                            AttributeType = AttributeType.PatientName,
                            Value = patientName
                        },
                        new AttributeValueEntity {
                            Id = Guid.NewGuid(),
                            AttributeType = AttributeType.PatientAge,
                            Value = patientAge
                        }
                    }
                },
                new EavEntity {
                    Id = Guid.NewGuid(),
                    EntityType = EntityType.Patient,
                    AttributeValues = new List<AttributeValueEntity> {
                        new AttributeValueEntity {
                            Id = Guid.NewGuid(),
                            AttributeType = AttributeType.PatientName,
                            Value = "123123"
                        },
                        new AttributeValueEntity {
                            Id = Guid.NewGuid(),
                            AttributeType = AttributeType.PatientAge,
                            Value = "12"
                        }
                    }
                }
            };

            var operationId = Guid.NewGuid();
            const string operationName = "Lymphadenectomy";
            var operationEntity = new EavEntity {
                Id = operationId,
                EntityType = EntityType.Operation,
                AttributeValues = new List<AttributeValueEntity> {
                    new AttributeValueEntity {
                        Id = Guid.NewGuid(),
                        AttributeType = AttributeType.OperationName,
                        Value = operationName
                    },
                    new AttributeValueEntity {
                        Id = Guid.NewGuid(),
                        AttributeType = AttributeType.PatientId,
                        Value = patientId.ToString()
                    }
                }
            };

            dbContext.Entities.AddRange(patientEntities);
            dbContext.Entities.Add(operationEntity);
            await dbContext.SaveChangesAsync();

            var sut = new ClinicService(dbContext);

            //Action
            var patients = await sut.GetAllPatientsAsync(default);

            //Assert
            patients.Should()
                .NotBeNullOrEmpty()
                .And.HaveCount(2);

            var patientWithOperation = patients.Single(x => x.PatientId == patientId);

            patientWithOperation.Should().NotBeNull();
            patientWithOperation.Name.Should().Be(patientName);
            patientWithOperation.Age.Should().Be(ushort.Parse(patientAge));

            patientWithOperation.Operations.Should().NotBeNullOrEmpty();
            var operation = patientWithOperation.Operations.Single();
            operation.Should().NotBeNull();
            operation.OperationId.Should().Be(operationId);
            operation.OperationName.Should().Be(operationName);
        }


        [Fact]
        public async Task CanAddOperation() {
            //Arrange
            var dbContext = GetDbContext(nameof(CanAddPatient));
            var patientId = Guid.NewGuid();
            var patientEntity = new EavEntity {
                Id = patientId,
                EntityType = EntityType.Patient,
                AttributeValues = new List<AttributeValueEntity> {
                    new AttributeValueEntity {
                        Id = Guid.NewGuid(),
                        AttributeType = AttributeType.PatientName,
                        Value = "Jon Snow"
                    },
                    new AttributeValueEntity {
                        Id = Guid.NewGuid(),
                        AttributeType = AttributeType.PatientAge,
                        Value = "24"
                    }
                }
            };

            const string operationName = "Lymphadenectomy";
            var createOperationVm = new CreateOperationVm {
                OperationName = operationName,
                PatientId = patientId
            };

            dbContext.Entities.Add(patientEntity);
            await dbContext.SaveChangesAsync();

            var sut = new ClinicService(dbContext);

            //Action
            await sut.AddOperationAsync(createOperationVm);

            //Assert
            var eavEntity = await dbContext.Entities
                .AsNoTracking()
                .WithAttributes()
                .SingleAsync(x=>x.EntityType == EntityType.Operation);

            eavEntity.Should().NotBeNull();

            eavEntity.AttributeValues.Should()
                .NotBeNull();

            var name = eavEntity.AttributeValues.Single(x => x.AttributeType == AttributeType.OperationName).Value;
            name.Should().Be(createOperationVm.OperationName);
        }
    }
}