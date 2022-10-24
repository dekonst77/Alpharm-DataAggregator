using System.Collections.Generic;

namespace DataAggregator.Web.Models.Retail
{
    public class FolderStructureJson
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool IsFile { get; set; }

        public bool IsMissing { get; set; }

        public bool IsRecovery { get; set; }

        public bool IsError { get; set; }

        public virtual ICollection<FolderStructureJson> Childs { get; set; }


    }
}