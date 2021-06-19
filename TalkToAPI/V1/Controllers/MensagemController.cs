using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Models.DTO;
using TalkToAPI.V1.Repositories.Interfaces;

namespace TalkToAPI.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class MensagemController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMensagemRepository _mensagemRepository;

        public MensagemController(IMapper mapper, IMensagemRepository mensagemRepository)
        {
            _mapper = mapper;
            _mensagemRepository = mensagemRepository;
        }
        [Authorize]
        [HttpGet("{DeID}/{ParaID}",Name ="ObterMensagem")]
        public ActionResult Obter(string DeID,string ParaID)
        {
            if (DeID == ParaID)
            {
                throw new Exception("Mensagem não ser enviada para o mesmo usuário");
            }

           var mensagens = _mensagemRepository.ObterMensagens(DeID, ParaID);
           var listMensagem = _mapper.Map<List<Mensagem>, List<MensagemDTO>>(mensagens);

           var lista = new ListaDTO<MensagemDTO>() { Lista = listMensagem  };
           lista.Links.Add(new LinkDTO("_obtermensagem", Url.Link("ObterMensagem", new { DeID = DeID,ParaID = ParaID }),"GET"));

           return Ok(lista);
           
         
        }
        [Authorize]
        [HttpPost("",Name ="CadastrarMensagem")]
        public ActionResult Cadastrar([FromBody]Mensagem mensagem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _mensagemRepository.Cadastrar(mensagem);

                    var mensagensDB = _mapper.Map<Mensagem, MensagemDTO>(mensagem);

                    mensagensDB.Links.Add(new LinkDTO("self",Url.Link("CadastrarMensagem", null),"POST"));
                    mensagensDB.Links.Add(new LinkDTO("_AtualizacaoParcial", Url.Link("AtualizacaoParcial", new { id = mensagem.Id }), "PATCH"));
                    
                    return Ok(mensagensDB);
                }
                catch(Exception e)
                {
                    return UnprocessableEntity(e);
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }
        [Authorize]
        [HttpPatch("{id}",Name = "AtualizacaoParcial")]
        public ActionResult AtualizacaoParcial(int id,[FromBody]JsonPatchDocument<Mensagem> jsonPatch)
        {
            if(jsonPatch == null)
            {
                return BadRequest();
            }

            var mensagem = _mensagemRepository.Obter(id);

            jsonPatch.ApplyTo(mensagem);
            mensagem.Atualizado = DateTime.Now;

            _mensagemRepository.Atualizar(mensagem);

            var mensagensDB = _mapper.Map<Mensagem, MensagemDTO>(mensagem);
            mensagensDB.Links.Add(new LinkDTO("SELF", Url.Link("AtualizacaoParcial", new { id = mensagem.Id }), "PATCH"));


            return Ok(mensagem);
        }
    }
}
