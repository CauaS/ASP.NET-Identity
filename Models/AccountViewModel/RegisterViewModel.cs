using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SemIdentity.Models.AccountViewModel
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="E-mail")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Esse campo estar em um formato  válido")]
        public string Email{ get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        [StringLength(100, ErrorMessage = "O campo {0} deve ter no mínimo {2} e no máximo {1} catacteres", MinimumLength = 8)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name ="Confirmar Senha")]
        [Compare("Password", ErrorMessage ="As senhas devem ser iguais.")] // compara com o campo Password acima.
        public string ConfirmPassword { get; set; }
    }
}
