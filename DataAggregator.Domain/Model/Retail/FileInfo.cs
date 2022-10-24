using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace DataAggregator.Domain.Model.Retail
{
    public class FileInfo
    {
        public long Id { get; set; }

        public string FileName { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateInsert { get; set; }

        public string Path { get; set; }

        public int YearStart { get; set; }

        public int MonthStart { get; set; }

        public int YearEnd { get; set; }

        public int MonthEnd { get; set; }

        //Последняя дата изменения файла
        public DateTime? LastWriteTime { get; set; }


        public long SourceId { get; set; }

        public FileInfo()
        {
            FileStatusId = 1;
        }
        
        public long FileStatusId { get; set; }
      
        public virtual FileStatus FileStatus { get; set; }

        public virtual Source Source { get; set; }

        public virtual IList<FileData> FileData { get; set; }

        public virtual IList<FileInfoLog> FileInfoLog { get; set; }
    }
}
