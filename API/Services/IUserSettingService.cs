using System.Collections.Generic;
using System.Threading.Tasks;
using AsyncQueryProviderDemo.Models;

namespace AsyncQueryProviderDemo.Services
{
    public interface IUserSettingService
    {
        Task<UserSettingModel> AddUserSettingAsync(UserSettingModel userSetting);

        Task UpdateUserSettingAsync(UserSettingModel userSetting);

        Task DeleteUserSettingAsync(int id);

        Task<IEnumerable<UserSettingModel>> GetUserSettingsByUserIdAsync(string userId);
    }
}
