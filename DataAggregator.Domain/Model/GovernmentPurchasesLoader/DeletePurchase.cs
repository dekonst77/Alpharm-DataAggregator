using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    /// <summary>
    /// Список задач на удаление информации о закупке и инициализация перекачки закупки
    /// </summary>
    [Table("DeletePurchase", Schema = "purchase")]
    public class DeletePurchase
    {
        public long Id { get; set; }
        public Int16 StatusId { get; set; }
        public string PurchaseNumber { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? DateCompleted { get; set; }
    }
}