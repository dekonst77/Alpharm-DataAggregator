using DataAggregator.Domain.Model.LPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.LPU
{
    public class LPULicensesModel
    {
        public int Id { get; set; }

        public string EntityINN { get; set; }

        public string full_name_licensee { get; set; }

        public string Address { get; set; }

        public string EntityOGRN { get; set; }

        public string abbreviated_name_licensee { get; set; }

        public string form { get; set; }

        public string OrganizationId { get; set; }

        public string nameEnglish { get; set; }

        public int LPUPointId { get; set; }

        public string BricksId { get; set; }

        public string OKVED { get; set; }

        public string ContactPersonFullname { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public string worksId { get; set; }

        public string fias_code { get; set; }

        public string number { get; set; }

        public string EntityName { get; set; }

        public string EntityType { get; set; }

        public string NetworkName { get; set; }

        public string Brand { get; set; }

        public string TypeOf { get; set; }

        public string VidOf { get; set; }

        public string date_register { get; set; }

        public bool manualAdd { get; set; }

        public static LPULicensesModel Create(LPULicensesView model)
        {
            return ModelMapper.Mapper.Map<LPULicensesModel>(model);
        }

        public LPULicensesView ConvertToView()
        {
            return ModelMapper.Mapper.Map<LPULicensesView>(this);
        }
    }
}