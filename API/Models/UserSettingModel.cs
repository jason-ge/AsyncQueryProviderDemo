using System.ComponentModel.DataAnnotations;

namespace AsyncQueryProviderDemo.Models
{
    public class UserSettingModel
    {
        public int UserSettingId { get; set; }

        [Required]
        public string SettingKey { get; set; }

        [Required]
        public string SettingValue { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
