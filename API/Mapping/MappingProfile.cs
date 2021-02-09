using AsyncQueryProviderDemo.DAL;
using AsyncQueryProviderDemo.Models;
using AutoMapper;

namespace AsyncQueryProviderDemo.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserSetting, UserSettingModel>();

            CreateMap<UserSettingModel, UserSetting>();
        }
    }
}
