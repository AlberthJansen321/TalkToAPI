using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Models.DTO
{
    public class UsuarioDTO:BaseDTO
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "O campo nome é óbrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O campo email é óbrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "O campo senha é óbrigatório")]
        public string Senha { get; set; }
        [Required(ErrorMessage = "O campo Confirmacao de senha é óbrigatório")]
        [Compare("Senha")]
        public string Confirmacaosenha { get; set; }
        public string Slogan { get; set; }
    }
}
