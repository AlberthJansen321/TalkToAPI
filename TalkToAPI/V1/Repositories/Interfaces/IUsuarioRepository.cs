using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;

namespace TalkToAPI.V1.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        void Cadastrar(ApplicationUSER usuario,string senha);
        ApplicationUSER Obter(string email,string senha);
        ApplicationUSER Obter(string id);
    }
}
