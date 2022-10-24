using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Web.Models.Classifier
{
    public class NFCModel
    {
        public long? Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }

        public System.Int16 RouteAdministrationId { get; set; }
    }


    }