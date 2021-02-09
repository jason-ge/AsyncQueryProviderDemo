using System;
using System.ComponentModel.DataAnnotations;

namespace AsyncQueryProviderDemo.DAL
{
    public class UserSetting
    {
        [Key]
        public int UserSettingId { get; set; }

        [Required]
        public string SettingKey { get; set; }

        [Required]
        public string SettingValue { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
