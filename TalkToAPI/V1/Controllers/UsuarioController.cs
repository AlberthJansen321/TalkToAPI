using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Models.DTO;
using TalkToAPI.V1.Repositories.Interfaces;

namespace TalkToAPI.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsuarioController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly UserManager<ApplicationUSER> _userManager;
        private readonly IMapper _mapper;

        public UsuarioController(UserManager<ApplicationUSER> userManager, IUsuarioRepository usuarioRepository, ITokenRepository tokenRepository,
            IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            _usuarioRepository = usuarioRepository;
            _tokenRepository = tokenRepository;
            _configuration = configuration;
            _mapper = mapper;
        }
        [HttpPost("login")]
        public ActionResult Login([FromBody] UsuarioDTO usuarioDTO)
        {
            ModelState.Remove("Confirmacaosenha");
            ModelState.Remove("Nome");

            if (ModelState.IsValid)
            {
                ApplicationUSER usuario = _usuarioRepository.Obter(usuarioDTO.Email, usuarioDTO.Senha);

                if (usuario != null)
                {
                    //retorna TokenJWT
                    return GerarToken(usuario);
                }
                else
                {
                    return NotFound("Usuario não localizado");
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }
        [Authorize]
        [HttpGet("", Name = "ObterUsuarios")]
        public ActionResult ObterTodos()
        {
            var UsuariosAppUSER = _userManager.Users.ToList();
            
            var ListUsuariosDTO = _mapper.Map<List<ApplicationUSER>, List<UsuarioDTO>>(UsuariosAppUSER);

            foreach (var usuarios in ListUsuariosDTO)
            {
                usuarios.Links.Add(new LinkDTO("self", Url.Link("ObterUsuarios", new { id = usuarios.Id }), "GET"));
            }

            var lista = new ListaDTO<UsuarioDTO>() { Lista = ListUsuariosDTO };
            lista.Links.Add(new LinkDTO("self", Url.Link("ObterUsuarios", null), "GET"));

           

            return Ok(lista);
        }
        [Authorize]
        [HttpGet("{id}", Name = "ObterUsuario")]
        public ActionResult ObterUsuario(string id)
        {
            var usuario = _userManager.FindByIdAsync(id).Result;

            if (usuario == null)
                return NotFound();

            var UsuarioDTODb = _mapper.Map<ApplicationUSER, UsuarioDTO>(usuario);
            UsuarioDTODb.Links.Add(new LinkDTO("self", Url.Link("ObterUsuario", new { id = UsuarioDTODb.Id }), "GET"));
            UsuarioDTODb.Links.Add(new LinkDTO("_atualizarusuario", Url.Link("AtualizarUsuario", new { id = UsuarioDTODb.Id }), "PUT"));

            return Ok(UsuarioDTODb);

        }
        [HttpPost("", Name = "CadastrarUsuario")]
        public ActionResult Cadastrar([FromBody] UsuarioDTO usuarioDTO)
        {
            if (ModelState.IsValid)
            {
                ApplicationUSER usuario = _mapper.Map<UsuarioDTO, ApplicationUSER>(usuarioDTO);
            
                var resultado = _userManager.CreateAsync(usuario, usuarioDTO.Senha).Result;

                if (!resultado.Succeeded)
                {
                    List<string> Erros = new List<string>();
                    foreach (var erro in resultado.Errors)
                    {
                        Erros.Add(erro.Description);
                    }

                    return UnprocessableEntity(Erros);
                }
                else
                {
                    var UsuarioDTODb = _mapper.Map<ApplicationUSER, UsuarioDTO>(usuario);
                    UsuarioDTODb.Links.Add(new LinkDTO("self", Url.Link("CadastrarUsuario", new { id = UsuarioDTODb.Id }), "POST"));
                    UsuarioDTODb.Links.Add(new LinkDTO("_obterusuario", Url.Link("ObterUsuario", new { id = UsuarioDTODb.Id }), "GET"));
                    UsuarioDTODb.Links.Add(new LinkDTO("_atualizarusuario", Url.Link("AtualizarUsuario", new { id = UsuarioDTODb.Id }), "PUT"));

                    return Ok(UsuarioDTODb);
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }
        [Authorize]
        [HttpPut("{id}", Name = "AtualizarUsuario")]
        public ActionResult Atualizar(string id, [FromBody] UsuarioDTO UsuarioDTO)
        {

            ApplicationUSER usuario = _userManager.GetUserAsync(HttpContext.User).Result;
            if (usuario.Id != id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                usuario.FullName = UsuarioDTO.Nome;
                usuario.UserName = UsuarioDTO.Email;
                usuario.Email = UsuarioDTO.Email;
                usuario.Slogan = UsuarioDTO.Slogan;

                var resultado = _userManager.UpdateAsync(usuario).Result;
                _userManager.RemovePasswordAsync(usuario);
                _userManager.AddPasswordAsync(usuario, UsuarioDTO.Senha);

                if (!resultado.Succeeded)
                {
                    List<string> Erros = new List<string>();
                    foreach (var erro in resultado.Errors)
                    {
                        Erros.Add(erro.Description);
                    }

                    return UnprocessableEntity(Erros);
                }
                else
                {

                    var UsuarioDTODb = _mapper.Map<ApplicationUSER, UsuarioDTO>(usuario);
                    UsuarioDTODb.Links.Add(new LinkDTO("self", Url.Link("atualizarusuario", new { id = UsuarioDTODb.Id }), "PUT"));
                    UsuarioDTODb.Links.Add(new LinkDTO("obter", Url.Link("obterusuario", new { id = UsuarioDTODb.Id }), "GET"));

                    return Ok(UsuarioDTODb);
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }
        [HttpPost("renovar")]
        public ActionResult Renovar(TokenDTO tokenDTO)
        {
            var resultado = _tokenRepository.Obter(tokenDTO.RefreshToken);

            if (resultado == null)
                return NotFound();
            //Atualizar a base de dados pois o token será usado
            resultado.Ultilizado = true;
            resultado.Atualizado = DateTime.Now;
            _tokenRepository.Atualizar(resultado);

            //Gerrar ou Renovar o token para o usuario
            var usuario = _usuarioRepository.Obter(resultado.UsuarioID);

            return GerarToken(usuario);

        }
        private ActionResult GerarToken(ApplicationUSER usuario)
        {
            var token = BuildToken(usuario);

            var TokenModel = new Token()
            {
                RefreshToken = token.RefreshToken,
                Expiration = token.Expiration,
                ExpirationRefreshToken = token.ExpirationRefreshToken,
                Usuario = usuario,
                Ultilizado = false
            };

            _tokenRepository.Cadastrar(TokenModel);

            return Ok(token);

        }
        private TokenDTO BuildToken(ApplicationUSER usuario)
        {
            var claims = new[]
           {
                new Claim(JwtRegisteredClaimNames.Email,usuario.Email),
                new Claim(JwtRegisteredClaimNames.Sub,usuario.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Chavetoken"]));
            var singn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(

                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: exp,
                    signingCredentials: singn
                );

            //String Token
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            //Data de Expiração do Refresh Token
            var expRefreshToken = DateTime.UtcNow.AddHours(2);

            //Refresh Token
            var refreshtoken = Guid.NewGuid().ToString();

            var TokenDTO = new TokenDTO { Token = tokenString, Expiration = exp, RefreshToken = refreshtoken, ExpirationRefreshToken = expRefreshToken };

            return TokenDTO;
        }
    }
}
