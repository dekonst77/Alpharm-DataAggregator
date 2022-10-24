using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.search
{
    [Table("SearchINNTask", Schema = "search")]
    public class SearchINNTask
    {
        public long Id { get; set; }
        //ИНН
        public string inn { get; set; }
        //Уникальный код
        public string GZId { get; set; }
        //Код ФЗ44
        public string Fz44Id { get; set; }
        //Код ФЗ223
        public string Fz223Id { get; set; }
        //Следующая страница для поиска
        public int NextPage { get; set; }
        //Поиск окончен
        public bool IsLoaded { get; set; }
        //Дата добавления записи
        public DateTime DateAdd { get; set; }
        //Последняя дата попытка загрузки
        public DateTime? LastTryLoad { get; set; }
        //Последняя дата перезапуска поиска
        public DateTime? LastReset { get; set; }
        //Последнее сообщение об ошибке
        public string ErrorMessage { get; set; }
    }
}
