using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.rts
{
    [Table("Stage", Schema = "rts")]
    public class RTSStage
    {
        public Byte Id { get; set; }
        public string Value { get; set; }
        //Идентификатор из боевой базы
        public Byte StageId { get; set; }
    }
}
