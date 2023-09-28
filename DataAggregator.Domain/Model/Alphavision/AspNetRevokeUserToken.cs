using System;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.Alphavision
{
    public class AspNetRevokeUserToken
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string HashToken { get; set; }

        public DateTime Expiration { get; set; }
    }
}
