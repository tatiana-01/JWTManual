using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dominio.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistencia;
public class JWTManualContext : DbContext
{
    public JWTManualContext(DbContextOptions<JWTManualContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
