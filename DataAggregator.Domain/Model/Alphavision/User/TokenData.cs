using System;
using System.Runtime.CompilerServices;

namespace DataAggregator.Domain.Model.Alphavision.User
{
    public class TokenData
    {
        public string access_token { get; set; }
        public DateTime? expires_at { get; set; }

    }
}
