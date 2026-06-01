using ApiPeliculas.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Genero>().Property(g => g.Nombre).HasMaxLength(50);
            /*
            modelBuilder.Entity<Genero>().HasData(
                new Genero { Id = 1, Nombre = "Acción" },
                new Genero { Id = 2, Nombre = "Comedia" },
                new Genero { Id = 3, Nombre = "Drama" },
                new Genero { Id = 4, Nombre = "Terror" },
                new Genero { Id = 5, Nombre = "Ciencia Ficción" }
            );
            */

            modelBuilder.Entity<Actor>().Property(a => a.Nombre).HasMaxLength(150);
            modelBuilder.Entity<Actor>().Property(a => a.Foto).IsUnicode();

            modelBuilder.Entity<Pelicula>().Property(p => p.Titulo).HasMaxLength(150);
            modelBuilder.Entity<Pelicula>().Property(p => p.Poster).IsUnicode();
            modelBuilder.Entity<GeneroPelicula>().HasKey(gp => new { gp.GeneroId, gp.PeliculaId });
            
            modelBuilder.Entity<ActorPelicula>().HasKey(ap => new { ap.ActorId, ap.PeliculaId });
        }
        public DbSet<Genero> Generos { get; set; }

        public DbSet<Actor> Actores { get; set; }

        public DbSet<Pelicula> Peliculas { get; set; }

        public DbSet<Comentario> Comentarios { get; set; }

        public DbSet<GeneroPelicula> GenerosPeliculas { get; set; }

        public DbSet<ActorPelicula> ActoresPeliculas { get; set; }

        public DbSet<Error> Errores { get; set; } 

    }
}
