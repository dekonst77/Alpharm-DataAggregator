using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Web.Models
{
    public sealed class ViewUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        public DateTime? LockDate { get; set; }

        [Display(Name = "Подразделение")]
        public string DepartmentName { get; set; }

        [Display(Name = "Почта")]
        public string Email { get; set; }

        [Display(Name = "Роли")]
        public IList<string> Roles { get; set; }
    }
}