using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace Quản_lý_công_tơ_điện.Models;

public partial class QuanLyCapDienContext : DbContext
{
    public QuanLyCapDienContext()
    {
    }

    public QuanLyCapDienContext(DbContextOptions<QuanLyCapDienContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cauhinhcapdien> Cauhinhcapdiens { get; set; }

    public virtual DbSet<Loaipha> Loaiphas { get; set; }

    public virtual DbSet<Mucdich> Mucdiches { get; set; }

    public virtual DbSet<Phieucapdien> Phieucapdiens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cauhinhcapdien>(entity =>
        {
            entity.HasKey(e => new { e.MaMucDich, e.MaSoPha });

            entity.ToTable("CAUHINHCAPDIEN");

            entity.Property(e => e.MaMucDich).HasMaxLength(20);
            entity.Property(e => e.MaSoPha).HasMaxLength(20);

            entity.HasOne(d => d.MaMucDichNavigation).WithMany(p => p.Cauhinhcapdiens)
                .HasForeignKey(d => d.MaMucDich)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CAUHINHCAPDIEN_MUCDICH");

            entity.HasOne(d => d.MaSoPhaNavigation).WithMany(p => p.Cauhinhcapdiens)
                .HasForeignKey(d => d.MaSoPha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CAUHINHCAPDIEN_LOAIPHA");
        });

        modelBuilder.Entity<Loaipha>(entity =>
        {
            entity.HasKey(e => e.MaSoPha);

            entity.ToTable("LOAIPHA");

            entity.Property(e => e.MaSoPha).HasMaxLength(20);
            entity.Property(e => e.TenSoPha).HasMaxLength(100);
        });

        modelBuilder.Entity<Mucdich>(entity =>
        {
            entity.HasKey(e => e.MaMucDich);

            entity.ToTable("MUCDICH");

            entity.Property(e => e.MaMucDich).HasMaxLength(20);
            entity.Property(e => e.TenMucDich).HasMaxLength(100);
        });

        modelBuilder.Entity<Phieucapdien>(entity =>
        {
            entity.HasKey(e => e.MaPhieu);

            entity.ToTable("PHIEUCAPDIEN");

            entity.Property(e => e.MaPhieu).HasMaxLength(20);
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .HasColumnName("CCCD");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MaMucDich).HasMaxLength(20);
            entity.Property(e => e.MaSoPha).HasMaxLength(20);
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.ThoiGianGui)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Cauhinhcapdien).WithMany(p => p.Phieucapdiens)
                .HasForeignKey(d => new { d.MaMucDich, d.MaSoPha })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PHIEUCAPDIEN_CAUHINHCAPDIEN");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
