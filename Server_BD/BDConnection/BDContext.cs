using Microsoft.EntityFrameworkCore;
using Server_BD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_BD.BDConnection
{
    public class BDContext:DbContext
    {
        public DbSet<MyNote> Notes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-MV43C0T;Database=MyNote;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
