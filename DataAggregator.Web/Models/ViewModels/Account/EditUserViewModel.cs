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
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [Display(Name = "Пользователь заблокирован")]
        public bool Locked { get; set; }

        [Display(Name = "Дата начала блокировки")]
        public string LockDate { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Множественная аутентификация")]
        public bool MultipleAuthentication { get; set; }

        public IEnumerable<AspNetRolesSelected> RolesList { get; set; }


        [Display(Name = "Подразделение")]
        public long? DepartmentId { get; set; }

        public IEnumerable<SelectListItem> Departments { get; set; }
    }
}