using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsyncQueryProviderDemo.DAL;
using AsyncQueryProviderDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace AsyncQueryProviderDemo.Services
{
    public class UserSettingService : IUserSettingService
    {
        private readonly IUserSettingContext _context;

        public UserSettingService(IUserSettingContext context)
        {
            _context = context;
        }

        public async Task<UserSettingModel> AddUserSettingAsync(UserSettingModel userSetting)
        {
            UserSetting setting = await _context.UserSettings.FirstOrDefaultAsync(s => s.UserId == userSetting.UserId && s.SettingKey == userSetting.SettingKey).ConfigureAwait(false);
            
            if (setting == null)
            {
                setting = new UserSetting()
                {
                    UserId = userSetting.UserId,
                    SettingKey = userSetting.SettingKey,
                    SettingValue = userSetting.SettingValue,
                };
                await _context.UserSettings.AddAsync(setting).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                userSetting.UserSettingId = setting.UserSettingId;
                return userSetting;
            }
            else
            {
                throw new ArgumentException("The user setting with same UserId and SettingKey already exists");
            } 
        }

        public async Task UpdateUserSettingAsync(UserSettingModel setting)
        {
            var entity = await _context.UserSettings.FirstOrDefaultAsync(c => c.UserSettingId == setting.UserSettingId).ConfigureAwait(false);
            if (entity != null)
            {
                entity.UserId = setting.UserId;
                entity.SettingKey = setting.SettingKey;
                entity.SettingValue = setting.SettingValue;

                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
                throw new ArgumentException($"Cannot find the user setting with id {setting.UserSettingId}");
            }
        }

        public async Task DeleteUserSettingAsync(int id)
        {
            var entity = await _context.UserSettings.FirstOrDefaultAsync(c => c.UserSettingId == id).ConfigureAwait(false);
            if (entity != null)
            {
                _context.UserSettings.Remove(entity);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
                throw new ArgumentException($"Cannot find the user setting with id {id}");
            }
        }

        public async Task<UserSettingModel> GetUserSettingsByIdAsync(int id)
        {
            var setting = await _context.UserSettings.FindAsync(id).ConfigureAwait(false);
            if (setting != null)
            {
                return new UserSettingModel()
                {
                    UserId = setting.UserId,
                    SettingKey = setting.SettingKey,
                    SettingValue = setting.SettingValue,
                    UserSettingId = setting.UserSettingId,
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<UserSettingModel>> GetUserSettingsByUserIdAsync(string userId)
        {
            return await _context.UserSettings.Where(x => x.UserId == userId).Select(s => new UserSettingModel()
            {
                UserId = userId,
                SettingKey = s.SettingKey,
                SettingValue = s.SettingValue,
                UserSettingId = s.UserSettingId,
            }).ToListAsync().ConfigureAwait(false);
        }
    }
}
