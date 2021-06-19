using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Models.DTO
{
    public class MensagemDTO:BaseDTO
    {
       
        public int Id { get; set; }

        public virtual ApplicationUSER De { get; set; }
     
        public string DeID { get; set; }

        public virtual ApplicationUSER Para { get; set; }
      
        public string ParaID { get; set; }

        public bool Excluido { get; set; }

        public string Texto { get; set; }

        public DateTime Criado { get; set; }

        public DateTime? Atualizado { get; set; }
    }
}
