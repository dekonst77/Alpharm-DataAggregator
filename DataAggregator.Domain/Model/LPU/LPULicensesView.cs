using DataAggregator.Domain.Model.GS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.LPU
{
    [Table("LPULicensesView", Schema = "lpu")]
    public class LPULicensesView
    {
        public int Id { get; set; }

        public string EntityINN { get; set; }
        public string full_name_licensee { get; set; }
        public string Address { get; set; }
        public string EntityOGRN { get; set; }

        public string abbreviated_name_licensee { get; set; }

        public string form { get; set; }

        [Column("ID ОГРН+ИНН")]
        public int? OrganizationId { get; set; }

        [Column("Наименование на англ.яз. (краткое)")]
        public string nameEnglish { get; set; }

        [Column("LPU_PointId")]
        public int LPUPointId { get; set; }

        public string BricksId { get; set; }

        public string OKVED { get; set; }

        public string ContactPersonFullname { get; set; }

        public string Phone { get; set; }

        [Column("E-mail")]
        public string Email { get; set; }

        public string Website { get; set; }

        public string worksId { get; set; }

        public string fias_code { get; set; }

        public string number { get; set; }

        public string EntityName { get; set; }

        public string EntityType { get; set; }

        public string NetworkName { get; set; }

        public string Brand { get; set; }

        [Column("Тип ЛПУ")]
        public string TypeOf { get; set; }

        [Column("Вид ЛПУ")]
        public string VidOf { get; set; }

        public string date_register { get; set; }

        public bool manualAdd { get; set; }

        public virtual LPUPoint LPUPoint { get; set; }

        public virtual Organization Organization { get; set; }

        

    }
}
