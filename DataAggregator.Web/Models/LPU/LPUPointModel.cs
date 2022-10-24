using DataAggregator.Domain.Model.LPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.LPU
{
    public class LPUPointModel
    {  
       
        public int PointId { get; set; }
        public string Address { get; set; }
        public string Address_index { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address_street { get; set; }
        public string Address_comment { get; set; }
        public string Address_float { get; set; }
        public string Address_room { get; set; }
        public string Address_room_area { get; set; }
        public string Address_koor { get; set; }
        public string BricksId { get; set; }
        public string Bricks_FederalDistrict { get; set; }
        public string Bricks_City { get; set; }
        public string Bricks_CityType { get; set; }
        public int? LPUcnt { get; set; }
        public int? DoubleMinPoint { get; set; }
        public static LPUPointModel Create(LPUPointView model)
        {
            return ModelMapper.Mapper.Map<LPUPointModel>(model);
        }
    }
}