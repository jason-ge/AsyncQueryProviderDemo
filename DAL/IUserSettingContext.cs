using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;
using System.Threading.Tasks;
namespace AsyncQueryProviderDemo.DAL
{
    public interface IUserSettingContext
    {
        DbSet<UserSetting> UserSettings { get; set; }

        int SaveChanges();
        EntityEntry Update(object entity);
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken);
    }
}
