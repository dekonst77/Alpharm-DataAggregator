using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.Alphavision.User
{
    public class RegisterUserModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email обязателен")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Пароль обязателен")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Организация является обязательным")]
        public int SupplierId { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }

        public DateTime Birthday { get; set; } = DateTime.Now;

        public int PostId { get; set; }

        public List<string> Roles { get; set; }

        public bool ApiEnabled { get; set; }
    }
}
