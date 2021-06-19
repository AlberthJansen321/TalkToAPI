using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;

namespace TalkToAPI.V1.Repositories.Interfaces
{
    public interface IMensagemRepository
    {
        void Cadastrar(Mensagem mensagem);
        List<Mensagem> ObterMensagens(string DeID,string ParaID);
        Mensagem Obter(int id);
        void Atualizar(Mensagem mensagem);
    }
}
