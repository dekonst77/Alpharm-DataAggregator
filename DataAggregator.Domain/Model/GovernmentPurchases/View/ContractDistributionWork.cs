using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Domain.Utils;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases.View
{
    public class ContractDistributionWork
    {
        /// <summary>
        /// Идентификатор закупки
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Номер закупки
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Наименование объекта закупки
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Дата объявления
        /// </summary>
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime DateBegin { get; set; }

        /// <summary>
        /// Дата аукциона
        /// </summary>
         [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Идентификатор класса закупки
        /// </summary>
        public Byte? PurchaseClassId { get; set; }

        /// <summary>
        /// Класс закупки (Нет/ЛС/на проверку/на удаление)
        /// </summary>
        public string PurchaseClass { get; set; }

        /// <summary>
        /// Идентификатор пользователя, которому назначена закупка
        /// </summary>
        public Guid? ContractAssignedToUserId { get; set; }

        /// <summary>
        /// Пользователь, которому назначена закупка
        /// </summary>
        public string ContractAssignedToUser { get; set; }

        /// <summary>
        /// Идентификатор пользователя, установившего класс закупки
        /// </summary>
        public Guid? PurchaseClassUserId { get; set; }

        /// <summary>
        /// Пользователь, установивший класс закупки
        /// </summary>
        public string PurchaseClassUser { get; set; }

        /// <summary>
        /// URL закупки на zakupki.gov.ru
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// Сумма всех лотов закупки
        /// </summary>
        public decimal? Sum { get; set; }

        /// <summary>
        /// Статус закупки
        /// </summary>
        public Int16 StatusId { get; set; }

        /// <summary>
        /// Пользователь который делает закупку
        /// </summary>
        public string UserInWork { get; set; }

        /// <summary>
        /// Кол-во контрактов, пригодных для обработки
        /// </summary>
        public int ContractCount { get; set; }

        /// <summary>
        /// Кол-во KK контрактов, из тех что в ContractCount
        /// </summary>
        public int ContractKKCount { get; set; }
        public int ContractPDFCount { get; set; }

        /// <summary>
        /// Характер закупки
        /// </summary>
        public string NatureName { get; set; }
        public string Supplier_Winner { get; set; }
    }
}


