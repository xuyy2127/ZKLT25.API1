using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.Helper
{
    public class ApiSettings
    {
        [Required]
        [Url]
        public string BaseUrl { get; set; }

        [Url]
        public string AgvUrl { get; set; }

        [Range(1, 100)]
        public int TimeoutSeconds { get; set; } = 30;

        [Range(0, 5)]
        public int MaxRetries { get; set; } = 3;
    }

}
