using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Web.Models
{
    public sealed class LoginViewModel
    {
        [Required]
        [Display(Name = "�����")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "������")]
        public string Password { get; set; }

        [Display(Name = "��������� ����?")]
        public bool RememberMe { get; set; }
    }
}