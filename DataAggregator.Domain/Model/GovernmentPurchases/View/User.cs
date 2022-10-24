using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases.View
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string FullNameWithoutPatronymic { get; set; }
        public string Surname { get; set; }
    }
}
