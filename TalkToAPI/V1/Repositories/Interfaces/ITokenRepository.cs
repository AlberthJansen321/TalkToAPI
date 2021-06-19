using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;

namespace TalkToAPI.V1.Repositories.Interfaces
{
     public interface ITokenRepository
    {
        void Cadastrar(Token token);
        Token Obter(string refreshtoken);
        void Atualizar(Token token);
        
    }
}
