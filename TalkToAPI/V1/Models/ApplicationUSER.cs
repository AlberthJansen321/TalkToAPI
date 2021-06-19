using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Models
{
    public class ApplicationUSER:IdentityUser
    {
        public ApplicationUSER()
        {
            MensagensDe = new HashSet<Mensagem>();
            MensagensPara = new HashSet<Mensagem>();
            Tokens = new HashSet<Token>();
        }
        
        public string FullName { get; set; }
        public string Slogan { get; set; }

        [InverseProperty(nameof(Mensagem.De))]
        public  virtual ICollection<Mensagem> MensagensDe { get; set; }

        [InverseProperty(nameof(Mensagem.Para))]
        public virtual ICollection<Mensagem> MensagensPara { get; set; }

        [ForeignKey("UsuarioID")]
        public virtual ICollection<Token> Tokens { get; set; }
    }
}
