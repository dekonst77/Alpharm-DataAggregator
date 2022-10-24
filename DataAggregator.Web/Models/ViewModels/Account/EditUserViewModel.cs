using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DataAggregator.Web.Models
{
    public class AspNetRolesSelected
    { 
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public bool Selected { get; set; }
    }
    public sealed class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "�������")]
        public string Surname { get; set; }

        [Display(Name = "���")]
        public string FirstName { get; set; }

        [Display(Name = "��������")]
        public string MiddleName { get; set; }

        [Display(Name = "������������ ������������")]
        public bool Locked { get; set; }

        [Display(Name = "���� ������ ����������")]
        public string LockDate { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "������")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "������������� ������")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "������ �� ���������.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "������������� ��������������")]
        public bool MultipleAuthentication { get; set; }

        public IEnumerable<AspNetRolesSelected> RolesList { get; set; }


        [Display(Name = "�������������")]
        public long? DepartmentId { get; set; }

        public IEnumerable<SelectListItem> Departments { get; set; }
    }
}