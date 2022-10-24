using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace DataAggregator.Domain.Model.Project
{
    [Table("History", Schema = "prj")]
    public class History
    {
        [Key]
        public long Id { get; set; }
        public DateTime DT { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public int StepId { get; set; }
        public int ProjectId { get; set; }
    }
    [Table("Project", Schema = "prj")]
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public int PeriodYM { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string ProjectManagerId { get; set; }
    }
    [Table("ProjectType", Schema = "prj")]
    public class ProjectType
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
    }
    [Table("StepStatus", Schema = "prj")]
    public class StepStatus
    {
        [Key]
        public byte Id { get; set; }
        public string Value { get; set; }
        public string NextStep { get; set; }
        public string PNG { get; set; }
    }
    [Table("StepTemplate", Schema = "prj")]
    public class StepTemplate
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string CMD_End { get; set; }
    }
    [Table("Steps", Schema = "prj")]
    public class Steps
    {
        [Key]
        public int Id { get; set; }
        public byte orderby { get; set; }
        public int ProjectId { get; set; }
        public int StepTemplateId { get; set; }
        public DateTime DateBeginPlan { get; set; }
        public DateTime DateEndPlan { get; set; }
        public DateTime? DateBeginReal { get; set; }
        public DateTime? DateEndReal { get; set; }
        public byte DateDay { get; set; }
        public string StepManagerId { get; set; }
        public byte StepStatusId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int? DateBeginPlanHH { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int? DateEndPlanHH { get; set; }

        public void HHGet()
        {
            DateBeginPlanHH = 0;
            DateEndPlanHH = 0;

            if (DateBeginPlan != null)
                DateBeginPlanHH = DateBeginPlan.Hour;

            if (DateEndPlan != null)
                DateEndPlanHH = DateEndPlan.Hour;
        }
        public void HHSet()
        {
            if (DateBeginPlanHH == null)
                DateBeginPlanHH = 0;
            if (DateEndPlanHH == null)
                DateEndPlanHH = 0;



            if (DateBeginPlan != null)
                DateBeginPlan = new DateTime(DateBeginPlan.Year, DateBeginPlan.Month, DateBeginPlan.Day, (int)DateBeginPlanHH, 0, 0, 0);

            if (DateEndPlan != null)
                DateEndPlan = new DateTime(DateEndPlan.Year, DateEndPlan.Month, DateEndPlan.Day, (int)DateEndPlanHH, 0, 0, 0);
        }
    }

 /*   public class StepsEx : Steps
    {

        public int? DateBeginPlanHH { get; set; }
        public int? DateEndPlanHH { get; set; }
        public void HHGet()
        {
            DateBeginPlanHH = 0;
            DateEndPlanHH = 0;

            if (DateBeginPlan != null)
                DateBeginPlanHH = DateBeginPlan.Hour;

            if (DateEndPlan != null)
                DateEndPlanHH = DateEndPlan.Hour;
        }
        public void HHSet()
        {
            if (DateBeginPlanHH == null)
                DateBeginPlanHH = 0;
            if (DateEndPlanHH == null)
                DateEndPlanHH = 0;



            if (DateBeginPlan != null)
                DateBeginPlan = new DateTime(DateBeginPlan.Year, DateBeginPlan.Month, DateBeginPlan.Day, (int)DateBeginPlanHH, 0, 0, 0);

            if (DateEndPlan != null)
                DateEndPlan = new DateTime(DateEndPlan.Year, DateEndPlan.Month, DateEndPlan.Day, (int)DateEndPlanHH, 0, 0, 0);
        }
        
    }*/
}
