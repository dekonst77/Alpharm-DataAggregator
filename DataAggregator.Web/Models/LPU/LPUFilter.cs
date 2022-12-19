using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.LPU
{
    public class LPUFilter
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string BrickId { get; set; }

        public int? LPUId { get; set; }

        public int? PointId { get; set; }
        public Boolean? IsBrick { get; set; }
        public Boolean? IsGPS { get; set; }
        public Boolean? IsAddress { get; set; }
        public Boolean? IsLPU { get; set; }
        public Boolean? IsDepartment { get; set; }
        public Boolean? IsActual { get; set; }
        public Boolean? Double { get; set; }
        public string SF { get; set; }
        public string City { get; set; }
        public Boolean? IsNullType { get; set; }
        public Boolean? IsNullKind { get; set; }
        public int? TypeId { get; set; }
        public int? KindId { get; set; }

    }
}