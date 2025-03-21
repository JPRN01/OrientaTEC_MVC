﻿using Microsoft.EntityFrameworkCore;
using OrientaTEC_MVC.Models;

namespace OrientaTEC_MVC
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SomeEntity> SomeEntities { get; set; }
        public DbSet<EquipoGuia> EquipoGuia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Definir explícitamente el nombre de la tabla para SomeEntity
            modelBuilder.Entity<SomeEntity>().ToTable("SomeEntity");
        }
    }

    public class SomeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
