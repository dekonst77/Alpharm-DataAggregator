using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using DataAggregator.Console.ServiceReference1;
using DataAggregator.Core.Classifier;
using DataAggregator.Domain.DAL;

namespace DataAggregator.Console
{
    class Program
    {

        //public static void Test()
        //{




        //    var text = "Хлорпротиксен табл.п/о15мгх30";
        //    var replace = "Таблетки покрытые оболочкой ";
        //    var source = "табл.п.о.";

        //    //var result = DrugClassifierFunctions.ReplaceSubstringByTemplate(text, source, replace);
          
        //}




        static void Main(string[] args)
        {
            //
            //Test();


            //using (DatabaseManagerServiceClient client = new DatabaseManagerServiceClient())
            //{
            //    client.CreateExternalShipment();
            //}

            //return;
            


            //var bezrukovGuid = new Guid("d9aba36d-1002-460e-8578-6039c84dbf0f");

            //using (var context = new DrugClassifierContext())
            //{
            //    var replaces = context.Database.SqlQuery<ReplaceModel>("SELECT FromId,ToId FROM temp.ReplacementTable WHERE Done is null").ToList();

            //    foreach (var replaceModel in replaces)
            //    {
            //        ClassifierEditor editor = new ClassifierEditor(context, bezrukovGuid);
            //        editor.ReplaceDrug(replaceModel.FromId, replaceModel.ToId);
            //    }


            //}
        }


        public class ReplaceModel
        {
            [Key, Column(Order = 1)]
            public long FromId { get; set; }
            [Key, Column(Order = 2)]
            public long ToId { get; set; }
        }
    }
}
