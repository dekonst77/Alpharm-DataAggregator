using System;
using System.Collections.Generic;

namespace DataAggregator.Web.Managers
{
    public class AggregatedErrorsException : Exception
    {
        public ICollection<string> Errors { get; private set; }

        public AggregatedErrorsException(params string[] errors) : base(string.Join(Environment.NewLine, errors))
        {
            Errors = errors;
        }
    }
}