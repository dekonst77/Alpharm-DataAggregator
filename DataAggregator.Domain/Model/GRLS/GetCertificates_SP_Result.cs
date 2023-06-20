using System;

namespace DataAggregator.Domain.Model.GRLS
{
    public class GetCertificates_SP_Result
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Number_ID { get; set; }
        public string type { get; set; }
        public string html { get; set; }
        public bool Exchangeable { get; set; }
        public bool Reference { get; set; }
        public Nullable<System.DateTime> data_end { get; set; }
        public Nullable<System.DateTime> data_Annul { get; set; }
        public string StorageLife { get; set; }
        public Nullable<System.DateTime> date_registration { get; set; }
        public string Owner_Name { get; set; }
        public string Owner_Country { get; set; }
        public string TN { get; set; }
        public string INN { get; set; }
        public string FV_raw { get; set; }
        public string ManfWay_raw { get; set; }
        public string FTG { get; set; }
        public string SubstRaw { get; set; }
        public string ATC_WHO { get; set; }
        public bool ved { get; set; }
        public System.DateTime last_update { get; set; }
        public System.DateTime last_control { get; set; }
        public string status { get; set; }
        public string prescription { get; set; }
    }
}
