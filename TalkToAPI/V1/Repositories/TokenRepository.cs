using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Repositories.Interfaces;

namespace TalkToAPI.V1.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly TalkToDBcontext _mbanco;

        public TokenRepository(TalkToDBcontext banco)
        {
            _mbanco = banco;
        }
        public void Atualizar(Token token)
        {
            _mbanco.Token.Update(token);
            _mbanco.SaveChanges();
        }

        public void Cadastrar(Token token)
        {
            _mbanco.Token.Add(token);
            _mbanco.SaveChanges();
        }

        public Token Obter(string refreshtoken)
        {
            return _mbanco.Token.FirstOrDefault( t => t.RefreshToken == refreshtoken && t.Ultilizado == false);
            
        }
    }
}
