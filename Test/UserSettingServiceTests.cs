using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AsyncQueryProviderDemo.DAL;
using AsyncQueryProviderDemo.Mapping;
using AsyncQueryProviderDemo.Models;
using AsyncQueryProviderDemo.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Xunit;

namespace AsyncQueryProviderDemo.UnitTests
{
    public class UserSettingServiceTests
    {
        private UserSetting setting1;
        private UserSetting setting2;
        private UserSetting setting3;
        private UserSetting setting4;
        private IMapper _mapper;

        public UserSettingServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
            
            setting1 = new UserSetting
            {
                SettingKey = "SettingKey1",
                SettingValue = "SettingValue1",
                UserId = "testuser1",
                UserSettingId = 1
            };
            setting2 = new UserSetting
            {
                SettingKey = "SettingKey2",
                SettingValue = "SettingValue2",
                UserId = "testuser1",
                UserSettingId = 2
            };
            setting3 = new UserSetting
            {
                SettingKey = "SettingKey3",
                SettingValue = "SettingValue3",
                UserId = "testuser2",
                UserSettingId = 3
            };

            setting4 = new UserSetting
            {
                SettingKey = "SettingKey4",
                SettingValue = "SettingValue4",
                UserId = "testuser2",
                UserSettingId = 4
            };
        }

        [Fact]
        public async Task AddUserSettingAsync()
        {
            var settings = new List<UserSetting>();
            var mockSettings = CreateDbSetMock(settings.AsQueryable());
            mockSettings.Setup(m => m.AddAsync(It.IsAny<UserSetting>(), default)).Callback<UserSetting, CancellationToken>((s, token) =>
            {
                settings.Add(s);
            });

            var mockContext = new Mock<UserSettingContext>();
            mockContext.Setup(m => m.UserSettings).Returns(mockSettings.Object);

            var service = new UserSettingService(mockContext.Object);

            var result = await service.AddUserSettingAsync(_mapper.Map<UserSettingModel>(this.setting1));

            Assert.NotNull(result);
            Assert.Equal(this.setting1.SettingKey, result.SettingKey);

            await Assert.ThrowsAsync<ArgumentException>(() => service.AddUserSettingAsync(_mapper.Map<UserSettingModel>(this.setting1)));
        }

        [Fact]
        public async Task DeleteUserSettingAsync()
        {
            var settings = new List<UserSetting>() { setting1, setting2, setting3 };
            var mockSettings = CreateDbSetMock(settings.AsQueryable());
            mockSettings.Setup(m => m.Remove(It.IsAny<UserSetting>())).Callback<UserSetting>(s =>
            {
                settings.Remove(settings.Find(t => t.UserSettingId == s.UserSettingId));
            });


            var mockContext = new Mock<UserSettingContext>();
            mockContext.Setup(m => m.UserSettings).Returns(mockSettings.Object);

            var service = new UserSettingService(mockContext.Object);

            await service.DeleteUserSettingAsync(setting1.UserSettingId);

            await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteUserSettingAsync(setting1.UserSettingId));
        }

        [Fact]
        public async Task UpdateUserSettingAsync()
        {
            var settings = new List<UserSetting>() { setting1, setting2, setting3 };
            var mockSettings = CreateDbSetMock(settings.AsQueryable());

            var mockContext = new Mock<UserSettingContext>();
            mockContext.Setup(m => m.UserSettings).Returns(mockSettings.Object);

            var service = new UserSettingService(mockContext.Object);

            await service.UpdateUserSettingAsync(new UserSettingModel
            {
                SettingKey = "SettingKey1",
                SettingValue = "UpdatedValue",
                UserId = "testuser1",
                UserSettingId = 1
            });
            var results = await service.GetUserSettingsByUserIdAsync(this.setting1.UserId);

            Assert.NotNull(results);
            Assert.Equal(2, results.Count());

            Assert.NotNull(results.FirstOrDefault(s => s.SettingValue == "UpdatedValue"));

            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateUserSettingAsync(_mapper.Map<UserSettingModel>(this.setting4)));
        }

        [Fact]
        public async Task GetUserSettingsByUserId()
        {
            var settings = new List<UserSetting>() { setting1, setting2, setting3 };
            var mockSettings = CreateDbSetMock(settings.AsQueryable());

            var mockContext = new Mock<UserSettingContext>();
            mockContext.Setup(m => m.UserSettings).Returns(mockSettings.Object);

            var service = new UserSettingService(mockContext.Object);

            var result = await service.GetUserSettingsByUserIdAsync(this.setting1.UserId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            result = await service.GetUserSettingsByUserIdAsync("non-existing-user");

            Assert.Empty(result);

        }

        [Fact]
        public async Task GetUserSettingsById()
        {
            var settings = new List<UserSetting>() { setting1, setting2, setting3 };
            var mockSettings = CreateDbSetMock(settings.AsQueryable());
            mockSettings.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns((object[] r) =>
            {
                return new ValueTask<UserSetting>(mockSettings.Object.FirstOrDefaultAsync(b => b.UserSettingId == (int)r[0]));
            });

            var mockContext = new Mock<UserSettingContext>();
            mockContext.Setup(m => m.UserSettings).Returns(mockSettings.Object);

            var service = new UserSettingService(mockContext.Object);

            var result = await service.GetUserSettingsByIdAsync(this.setting1.UserSettingId);

            Assert.NotNull(result);
            Assert.Equal(this.setting1.UserSettingId, result.UserSettingId);

            result = await service.GetUserSettingsByIdAsync(1000);

            Assert.Null(result);
        }

        private static Mock<DbSet<T>> CreateDbSetMock<T>(IQueryable<T> items) where T : class
        {
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IAsyncEnumerable<T>>()
                .Setup(x => x.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<T>(items.GetEnumerator()));
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(items.Provider));
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.Expression).Returns(items.Expression);
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.ElementType).Returns(items.ElementType);
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.GetEnumerator()).Returns(items.GetEnumerator());

            return dbSetMock;
        }
    }
}
