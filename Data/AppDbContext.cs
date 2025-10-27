using DesafioBackend.Models;
using Microsoft.EntityFrameworkCore;


namespace DesafioBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Moto> Motos { get; set; }
        public DbSet<Entregador> Entregador { get; set; }
    }
}
