using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.LPU.Alphavision
{
    public class AuthenticationResponse
    {
        public string status { get; set; }
        public dynamic data { get; set; }
        public string message { get; set; }
    }
    public static class ResponseStatus
    {
        public const string Success = "Success";
        public const string Error = "Error";
    }

}
