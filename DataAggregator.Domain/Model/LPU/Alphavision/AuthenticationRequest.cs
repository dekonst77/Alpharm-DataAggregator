using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.LPU.Alphavision
{
    public class AuthenticationRequest
    {
        [Required]
        public string Login { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }

}
