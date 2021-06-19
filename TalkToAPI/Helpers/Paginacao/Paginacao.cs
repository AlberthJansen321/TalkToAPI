using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.Helpers.Paginacao
{
    public class Paginacao
    {
        public int NumeroPagina { get; set; }
        public int Registroporpagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
    }
}
