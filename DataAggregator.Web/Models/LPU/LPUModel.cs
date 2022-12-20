using DataAggregator.Domain.Model.LPU;
using System;

namespace DataAggregator.Web.Models.LPU
{
    public class LPUModel
    {       
        public int LPUId { get; set; }
        public int? ParentId { get; set; }
        public int? ActualId { get; set; }
        public string Comment { get; set; }
        public DateTime Date_Create { get; set; }
        public int OrganizationId { get; set; }
        public string EntityINN { get; set; }
        public string EntityOGRN { get; set; }
        public string licencesNumber { get; set; }
        public string EntityName { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string ContactPersonFullname { get; set; }
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
        public int? IsDepartment { get; set; }
        public string Department { get; set; }
        public int? DepartmentId { get; set; }
        public int? DepartmentCnt { get; set; }
        public string Status { get; set; }
        public int? TypeId { get; set; }
        public int? KindId { get; set; }
        public string full_name { get; set; }
        public string name { get; set; }
        public string form { get; set; }


        public static LPUModel Create(LPUView model)
        {
            return ModelMapper.Mapper.Map<LPUModel>(model);
        }


       


    }
}