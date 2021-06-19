using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Repositories.Interfaces;

namespace TalkToAPI.V1.Repositories
{
    public class MensagemRepository : IMensagemRepository
    {
        private readonly TalkToDBcontext _mbanco;

        public MensagemRepository(TalkToDBcontext mbanco)
        {
            _mbanco = mbanco;
        }

        public void Atualizar(Mensagem mensagem)
        {
            _mbanco.Mensagem.Update(mensagem);
            _mbanco.SaveChanges();
        }

        public void Cadastrar(Mensagem mensagem)
        {
            _mbanco.Mensagem.Add(mensagem);
            _mbanco.SaveChanges();
        }

        public Mensagem Obter(int id)
        {
            return _mbanco.Mensagem.Find(id);
        }

        public List<Mensagem> ObterMensagens(string deid, string paraid)
        {
            return _mbanco.Mensagem.Where(e => (e.DeID == deid || e.DeID == paraid) && (e.ParaID == deid || e.ParaID == paraid)).ToList();
        }
    }
}
