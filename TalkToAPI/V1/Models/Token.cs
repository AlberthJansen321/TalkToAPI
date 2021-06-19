using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models.DTO;

namespace TalkToAPI.V1.Models
{
    public class Token
    {
        [Key]
        public int Id { get; set; }
        public string RefreshToken { get; set; }
        [ForeignKey("Usuario")]
        public string UsuarioID { get; set; }
        public virtual ApplicationUSER Usuario { get; set; }
        public bool Ultilizado { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime ExpirationRefreshToken { get; set; }
        public DateTime Criado { get; set; }
        public DateTime? Atualizado { get; set; }
    }
}
