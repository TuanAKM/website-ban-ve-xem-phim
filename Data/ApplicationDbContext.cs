using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniCinema.Models;

namespace MiniCinema.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Phim> Phims { get; set; }
        public DbSet<PhongChieu> PhongChieus { get; set; }
        public DbSet<Ghe> Ghes { get; set; }
        public DbSet<SuatChieu> SuatChieus { get; set; }
        public DbSet<GiaoDich> GiaoDichs { get; set; }
        public DbSet<Ve> Ves { get; set; }
        public DbSet<BaoCaoThongKe> BaoCaoThongKes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Ve>()
                .HasOne(v => v.GiaoDich)
                .WithMany(g => g.Ves)
                .HasForeignKey(v => v.MaGiaoDich)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ve>()
                .HasOne(v => v.Ghe)
                .WithMany()
                .HasForeignKey(v => v.GheId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SuatChieu>()
                .HasOne(s => s.PhongChieu)
                .WithMany(p => p.SuatChieus)
                .HasForeignKey(s => s.PhongChieuId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
