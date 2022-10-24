using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.OFD
{
    public class Classifier_ExternalView
    {
        [Key]
        public long ClassifierId { get; set; }
        public long DrugId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }
        public string DrugDescription { get; set; }
        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
        public string TradeName { get; set; }
        public string Brand { get; set; }

        //INNGroupId
        //INNGroup
        //DosageGroupId
        //DosageGroup
        //FormProductId
        //FormProduct
        //ConsumerPackingCount
        //IsBad
        //FTGId
        //FTG   
        //PackerId
        //Packer
        //DrugDescription
        //BrandId
        //Brand     
        //TotalVolumeCount
        //TotalVolumeId
        //TotalVolume    
        //Used

    }
}
