using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GS
{
    //[Table("licenses_adress_work", Schema = "dbo")]
    //public class licenses_adress_work
    //{
    //    [Key]
    //    public long Id { get; set; }
    //    public long licenses_adressId { get; set; }
    //    public virtual licenses_adress licenses_adress { get; set; }
    //    public string work { get; set; }
    //}
    [Table("licenses_file", Schema = "dbo")]
    public class licenses_file
    {
        [Key]
        public int Id { get; set; }
        public string filename {get;set;}
        public DateTime dt_load { get; set; }
        public DateTime period { get; set; }
        public virtual ICollection<licenses> licenses { get; set; }
    }

    [Table("licenses_adress", Schema = "dbo")]
    public class licenses_adress
    {
        [Key]
        public long Id { get; set; }
        public long licensesId { get; set; }
        public virtual licenses licenses { get; set; }
        public string address { get; set; }
        public string index { get; set; }
        public string region { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string works { get; set; }
        public DateTime date_add { get; set; }
        //public virtual ICollection<licenses_adress_work> licenses_adress_works { get; set; }
    }

    [Table("licenses", Schema = "dbo")]
    public class licenses
    {
        [Key]
        public long Id { get; set; }
        public string name { get; set; }
        public string activity_type { get; set; }
        public string full_name_licensee { get; set; }
        public string abbreviated_name_licensee { get; set; }
        public string brand_name_licensee { get; set; }
        public string form { get; set; }
        public string address { get; set; }
        public string ogrn { get; set; }
        public string inn { get; set; }
        public string number { get; set; }
        public DateTime date { get; set; }
        public string number_orders { get; set; }
        public DateTime date_order { get; set; }
        public DateTime date_register { get; set; }
        public string number_duplicate { get; set; }
        public DateTime date_duplicate { get; set; }
        public string termination { get; set; }
        public DateTime date_termination { get; set; }
        public string information { get; set; }
        public string information_regulations { get; set; }
        public string information_suspension_resumption { get; set; }
        public string information_cancellation { get; set; }
        public string information_reissuing { get; set; }
        public DateTime data_add { get; set; }
        public int licenses_fileId { get; set; }
        [JsonIgnore]
        public virtual licenses_file licenses_file { get; set; }
        [JsonIgnore]
        public virtual ICollection<licenses_adress> licenses_adress { get; set; }
    }

    [Table("licenses_ViewAll", Schema = "dbo")]
    public class licenses_ViewAll
    {
        [Key]
        public long Id { get; set; }
        public string name { get; set; }
        public string activity_type { get; set; }
        public string full_name_licensee { get; set; }
        public string abbreviated_name_licensee { get; set; }
        public string brand_name_licensee { get; set; }
        public string form { get; set; }
        public string address { get; set; }
        public string ogrn { get; set; }
        public string inn { get; set; }
        public string number { get; set; }
        public DateTime date { get; set; }
        public string number_orders { get; set; }
        public DateTime date_order { get; set; }
        public DateTime date_register { get; set; }
        public string number_duplicate { get; set; }
        public DateTime date_duplicate { get; set; }
        public string termination { get; set; }
        public DateTime date_termination { get; set; }
        public string information { get; set; }
        public string information_regulations { get; set; }
        public string information_suspension_resumption { get; set; }
        public string information_cancellation { get; set; }
        public string information_reissuing { get; set; }
        public DateTime data_add { get; set; }
        public int licenses_fileId { get; set; }
        public string address_point { get; set; }
        public long address_pointId { get; set; }
        public string index { get; set; }
        public string region { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string works { get; set; }
    }
}
