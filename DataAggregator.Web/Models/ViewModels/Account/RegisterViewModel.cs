using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Web.Models
{
    public sealed class RegisterViewModel
    {
        [Required]
        [Display(Name = "�����")]
        public string Name { get; set; }
        
        [Required]
        [Display(Name = "�������")]
        public string Surname { get; set; }

        [Display(Name = "���")]
        public string FirstName { get; set; }

        [Display(Name = "��������")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "������������ ����� ����������� �����")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "������")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "������������� ������")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "������ �� ���������.")]
        public string ConfirmPassword { get; set; }
    }
}