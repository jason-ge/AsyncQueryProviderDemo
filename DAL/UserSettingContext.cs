using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncQueryProviderDemo.DAL
{
    public class UserSettingContext : DbContext, IUserSettingContext
    {
        public virtual DbSet<UserSetting> UserSettings { get; set; }

        public UserSettingContext(DbContextOptions<UserSettingContext> options) : base(options)
        {
        }

        public UserSettingContext()
        {
        }

        public override EntityEntry Update(object entity)
        {
            if(entity.GetType() == typeof(UserSetting))
            {
                var userSettingEntity = (UserSetting)entity;
                var entry = ChangeTracker.Entries<UserSetting>().First(x => x.Entity.UserSettingId == userSettingEntity.UserSettingId);

                entry.State = EntityState.Modified;
                return base.Update(entity);
            }
            else
            {
                throw new ArgumentException($"Unknow entity type ${entity.GetType()}");
            }
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.DisplayName());
            }
        }
    }
}
