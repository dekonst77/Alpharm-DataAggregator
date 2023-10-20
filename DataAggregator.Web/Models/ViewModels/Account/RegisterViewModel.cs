using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Web.Models
{
    public sealed class RegisterViewModel
    {
        [Display(Name = "�����")]
        [Required(ErrorMessage = "������� �����")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "������� �������")]
        [Display(Name = "�������")]
        public string Surname { get; set; }

        [Display(Name = "���")]
        public string FirstName { get; set; }

        [Display(Name = "��������")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "������� email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "������������ ����� ����������� �����")]
        public string Email { get; set; }

        [Required(ErrorMessage = "������� ������")]
        [DataType(DataType.Password)]
        [Display(Name = "������")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "������������� ������")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "������ �� ���������.")]
        public string ConfirmPassword { get; set; }
    }
}