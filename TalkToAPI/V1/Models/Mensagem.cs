using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Models
{
    public class Mensagem
    {
        [Key]
        public int Id { get; set; }
        public bool Excluido { get; set; }
        [Required]
        public string Texto { get; set; }
        public DateTime Criado { get; set; }
        public DateTime? Atualizado { get; set; }

        [ForeignKey("De")]
        [Required]
        public string DeID { get; set; }
        public virtual ApplicationUSER De { get; set; }

        [ForeignKey("Para")]
        [Required]
        public string ParaID { get; set; }
        public virtual ApplicationUSER Para{ get; set; }
    }
}
