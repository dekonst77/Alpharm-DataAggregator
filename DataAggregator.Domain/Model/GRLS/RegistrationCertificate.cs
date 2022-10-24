using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GRLS
{
    //[Table("RegistrationCertificate", Schema = "dbo")]
    public class RegistrationCertificate
    {
      public long Id {get; set; }
      public string Number {get; set; }
      public string RegistrationDate {get; set; }
      public string CirculationPeriod {get; set; }
      public string ExpDate {get; set; }
      public string CancellationDate {get; set; }
      public string ReissueDate {get; set; }
      public string Owner { get; set; }
      public string Country { get; set; }

    }
}