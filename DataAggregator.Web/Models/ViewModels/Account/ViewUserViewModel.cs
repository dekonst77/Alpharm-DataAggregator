using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Web.Models
{
    public sealed class ViewUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "��� ������������")]
        public string UserName { get; set; }

        [Display(Name = "�������")]
        public string Surname { get; set; }

        [Display(Name = "���")]
        public string FirstName { get; set; }

        [Display(Name = "��������")]
        public string MiddleName { get; set; }

        public DateTime? LockDate { get; set; }

        [Display(Name = "�������������")]
        public string DepartmentName { get; set; }

        [Display(Name = "�����")]
        public string Email { get; set; }

        [Display(Name = "����")]
        public IList<string> Roles { get; set; }
    }
}