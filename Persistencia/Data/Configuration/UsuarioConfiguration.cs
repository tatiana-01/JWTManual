using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistencia.Data.Configuration;
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");
            
            builder.HasIndex(p => new
            {
                p.Email
            }).HasDatabaseName("IX_MiIndice")
            .IsUnique();

            builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(200);
        }
    }
