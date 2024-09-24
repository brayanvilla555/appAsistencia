using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using proyectoAsistencia.Models;

namespace proyectoAsistencia.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Administrador> Administrador { get; set; } = default!;
        public DbSet<Ponente> Ponente { get; set; } = default!;
        public DbSet<Estudiante> Estudiante { get; set; } = default!;
        public DbSet<Evento> Evento { get; set; } = default!;
        public DbSet<EventoPonente> EventoPonente { get; set; } = default!;
        public DbSet<Docente> Docente { get; set; } = default!;
        public DbSet<Asistencia> Asistencia { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asistencia>()
                .HasOne(a => a.Evento)
                .WithMany(e => e.Asistencias)
                .HasForeignKey(a => a.EventoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Asistencia>()
                .HasOne(a => a.Estudiante)
                .WithMany(e => e.Asistencias)
                .HasForeignKey(a => a.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Asistencia>()
                .HasOne(a => a.Administrador)
                .WithMany(ad => ad.Asistencias)
                .HasForeignKey(a => a.AdministradorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de la entidad pivote EventoPonente con clave compuesta
            modelBuilder.Entity<EventoPonente>()
                .HasKey(ep => new { ep.EventoId, ep.PonenteId });

            modelBuilder.Entity<EventoPonente>()
                .HasOne(ep => ep.Evento)
                .WithMany(e => e.EventoPonentes)
                .HasForeignKey(ep => ep.EventoId);

            modelBuilder.Entity<EventoPonente>()
                .HasOne(ep => ep.Ponente)
                .WithMany(p => p.EventoPonentes)
                .HasForeignKey(ep => ep.PonenteId);



            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Curso> Curso { get; set; } = default!;
    }
}
