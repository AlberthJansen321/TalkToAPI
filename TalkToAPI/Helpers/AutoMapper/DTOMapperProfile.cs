using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Models.DTO;

namespace TalkToAPI.Helpers.AutoMapper
{
    public class DTOMapperProfile:Profile
    {
        public DTOMapperProfile()
        {
            CreateMap<ApplicationUSER, UsuarioDTO>()
                .ForMember(dest => dest.Nome,origem => origem.MapFrom(o => o.FullName));

            CreateMap<Mensagem, MensagemDTO>();
        }
    }
}
