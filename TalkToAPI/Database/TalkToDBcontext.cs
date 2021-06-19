using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;

namespace TalkToAPI
{
    public class TalkToDBcontext : IdentityDbContext<ApplicationUSER>
    {
        public TalkToDBcontext(DbContextOptions<TalkToDBcontext> options) : base(options)
        {

        }
        
        public DbSet<Mensagem> Mensagem { get; set; }
        public DbSet<Token> Token { get; set; }

    }
}
