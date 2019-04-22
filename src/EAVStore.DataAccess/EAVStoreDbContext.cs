using EAVStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EAVStore.DataAccess
{
    public class EavStoreDbContext: DbContext
    {
        public DbSet<EavEntity> Entities { get; set; }
        public DbSet<AttributeValueEntity> AttributeValues { get; set; }

        public EavStoreDbContext(DbContextOptions<EavStoreDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

        }
    }
}