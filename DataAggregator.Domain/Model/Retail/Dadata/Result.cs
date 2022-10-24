using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail.Dadata
{
    [Table("Result", Schema = "dadata")]
    public class Result
    {
        public long Id { get; set; }
        public string source { get; set; }
        public string result { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
        public string region_type { get; set; }
        public string region_type_full { get; set; }
        public string region { get; set; }
        public string area_type { get; set; }
        public string area_type_full { get; set; }
        public string area { get; set; }
        public string city_type { get; set; }
        public string city_type_full { get; set; }
        public string city { get; set; }
        public string settlement_type { get; set; }
        public string settlement_type_full { get; set; }
        public string settlement { get; set; }
        public string street_type { get; set; }
        public string street_type_full { get; set; }
        public string street { get; set; }
        public string house_type { get; set; }
        public string house_type_full { get; set; }
        public string house { get; set; }
        public string block_type { get; set; }
        public string block_type_full { get; set; }
        public string block { get; set; }
        public string flat_type { get; set; }
        public string flat { get; set; }
        public string flat_area { get; set; }
        public string square_meter_price { get; set; }
        public string flat_price { get; set; }
        public string postal_box { get; set; }
        public string fias_id { get; set; }
        public string kladr_id { get; set; }
        public string okato { get; set; }
        public string oktmo { get; set; }
        public string tax_office { get; set; }
        public string tax_office_legal { get; set; }
        public string timezone { get; set; }
        public string geo_lat { get; set; }
        public string geo_lon { get; set; }
        public string qc_geo { get; set; }
        public string qc_complete { get; set; }
        public string qc_house { get; set; }
        public string qc { get; set; }
        public string unparsed_parts { get; set; }
    }
}
