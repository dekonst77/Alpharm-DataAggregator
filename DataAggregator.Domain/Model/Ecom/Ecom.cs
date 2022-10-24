using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data.SqlTypes;

namespace DataAggregator.Domain.Model.Ecom
{
    [Table("CoefficientsCount", Schema = "EcomNew")]
    public class CoefficientsCount
    {
        [Key]
        [Column(Order = 1)]
        public DateTime Period { get; set; }
        [Key]
        [Column(Order = 2)]
        public string RegionCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string CountCategory { get; set; }

        public decimal CountMin { get; set; }
        public decimal CountMax { get; set; }
    }

    [Table("CoefficientsPrice", Schema = "EcomNew")]
    public class CoefficientsPrice
    {
        [Key]
        [Column(Order = 1)]
        public DateTime Period { get; set; }
        [Key]
        [Column(Order = 2)]
        public string RegionCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PriceCategory { get; set; }

        public decimal PriceMin { get; set; }
        public decimal PriceMax { get; set; }
    }

    [Table("RegionalCoefficients", Schema = "EcomNew")]
    public class RegionalCoefficients
    {
        [Key]
        [Column(Order = 1)]
        public DateTime Period { get; set; }
        [Key]
        [Column(Order = 2)]
        public string RegionCode { get; set; }

        public string Region { get; set; }
        public double RegCoeff { get; set; }
    }


    [Table("Coefficients", Schema = "EcomNew")]
    public class Coefficients
    {
        [Key]
        [Column(Order = 1)]
        public DateTime Period { get; set; }
        [Key]
        [Column(Order = 2)]
        public string RegionCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PriceCategory { get; set; }
        [Key]
        [Column(Order = 4)]
        public string CountCategory { get; set; }

        public double Coefficient { get; set; }
    }

    [Table("Coefficients_PivotView", Schema = "EcomNew")]
    public class Coefficients_PivotView
    {
        [Key]
        [Column(Order = 1)]
        public DateTime Period { get; set; }
        [Key]
        [Column(Order = 2)]
        public string RegionCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PriceCategory { get; set; }

        public double CoefficientColsA { get; set; }
        public double CoefficientColsB { get; set; }
        public double CoefficientColsC { get; set; }
        public double CoefficientColsD { get; set; }
    }
}
