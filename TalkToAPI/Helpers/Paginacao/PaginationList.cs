using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models.DTO;

namespace TalkToAPI.Helpers.Paginacao
{
    public class PaginationList<T>
    {
        public List<T> Results { get; set; } = new List<T>();
        public Paginacao Paginacao { get; set; }
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
    }
}
