using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.Helpers.Paginacao
{
    public class QueryPaginacao
    {
        public DateTime? Data { get; set; }
        public int? Pagnumero { get; set; }
        public int? Pagregistro { get; set; }
    }
}
