using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<Bienbancapdien> Bienbancapdiens { get; set; }

    public virtual DbSet<Cauhinhcapdien> Cauhinhcapdiens { get; set; }

    public virtual DbSet<Chitietghidien> Chitietghidiens { get; set; }

    public virtual DbSet<Chitiethoadon> Chitiethoadons { get; set; }

    public virtual DbSet<Hoadontiendien> Hoadontiendiens { get; set; }

    public virtual DbSet<Loaicongto> Loaicongtos { get; set; }

    public virtual DbSet<Loaipha> Loaiphas { get; set; }

    public virtual DbSet<Mucdich> Mucdiches { get; set; }

    public virtual DbSet<Phieucapdien> Phieucapdiens { get; set; }

    public virtual DbSet<Phieughidien> Phieughidiens { get; set; }

    public virtual DbSet<Cauhinhcongto> Cauhinhcongtos { get; set; }
    
    public virtual DbSet<QuyDinhGiaDien> QuyDinhGiaDiens { get; set; } = null!;

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
        modelBuilder.Entity<Bienbancapdien>(entity =>
        {
            entity.HasKey(e => e.MaBienBan);

            entity.ToTable("BIENBANCAPDIEN");

            entity.HasIndex(e => e.MaCongTo, "UQ_BIENBANCAPDIEN_MaCongTo").IsUnique();

            entity.HasIndex(e => e.MaPhieu, "UQ_BIENBANCAPDIEN_MaPhieu").IsUnique();

            entity.Property(e => e.MaBienBan).HasMaxLength(20);
            entity.Property(e => e.HangSanXuat).HasMaxLength(100);
            entity.Property(e => e.MaCongTo).HasMaxLength(50);
            entity.Property(e => e.MaLoaiCongTo).HasMaxLength(20);
            entity.Property(e => e.MaPhieu).HasMaxLength(20);
            entity.Property(e => e.NgayLap)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ThoiGianBatDauCapDien).HasColumnType("datetime");
            entity.Property(e => e.ViTriLapDat).HasMaxLength(255);

            entity.HasOne(d => d.MaLoaiCongToNavigation).WithMany(p => p.Bienbancapdiens)
                .HasForeignKey(d => d.MaLoaiCongTo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BIENBANCAPDIEN_LOAICONGTO");

            entity.HasOne(d => d.MaPhieuNavigation).WithOne(p => p.Bienbancapdien)
                .HasForeignKey<Bienbancapdien>(d => d.MaPhieu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BIENBANCAPDIEN_PHIEUCAPDIEN");
        });

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

        modelBuilder.Entity<Cauhinhcongto>(entity =>
        {
            entity.HasKey(e => new { e.MaSoPha, e.MaLoaiCongTo });

            entity.ToTable("CAUHINHCONGTO");

            entity.HasOne(d => d.MaLoaiCongToNavigation)
                  .WithMany(p => p.Cauhinhcongtos)
                  .HasForeignKey(d => d.MaLoaiCongTo)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_CAUHINHCONGTO_LOAICONGTO");

            entity.HasOne(d => d.MaSoPhaNavigation)
                  .WithMany(p => p.Cauhinhcongtos)
                  .HasForeignKey(d => d.MaSoPha)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_CAUHINHCONGTO_LOAIPHA");
        });

        modelBuilder.Entity<Chitietghidien>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieuGhi, e.MaBienBan }).HasName("PK__CHITIETG__3CAD6E3B04A35ED7");

            entity.ToTable("CHITIETGHIDIEN");

            entity.Property(e => e.MaPhieuGhi).HasMaxLength(20);
            entity.Property(e => e.MaBienBan).HasMaxLength(20);

            entity.HasOne(d => d.MaBienBanNavigation).WithMany(p => p.Chitietghidiens)
                .HasForeignKey(d => d.MaBienBan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHITIETGH__MaBie__0F624AF8");

            entity.HasOne(d => d.MaPhieuGhiNavigation).WithMany(p => p.Chitietghidiens)
                .HasForeignKey(d => d.MaPhieuGhi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHITIETGH__MaPhi__0E6E26BF");
        });

        modelBuilder.Entity<Chitiethoadon>(entity =>
        {
            entity.HasKey(e => new { e.MaHoaDon, e.Bac });

            entity.ToTable("CHITIETHOADON");

            entity.Property(e => e.MaHoaDon).HasMaxLength(20);
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.Chitiethoadons)
                .HasForeignKey(d => d.MaHoaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CHITIETHOADON_HOADONTIENDIEN");
        });

        modelBuilder.Entity<Hoadontiendien>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon);

            entity.ToTable("HOADONTIENDIEN");

            entity.Property(e => e.MaHoaDon).HasMaxLength(20);
            entity.Property(e => e.MaBienBan).HasMaxLength(20);
            entity.Property(e => e.MaPhieuGhi).HasMaxLength(20);
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Chitietghidien).WithMany(p => p.Hoadontiendiens)
                .HasForeignKey(d => new { d.MaPhieuGhi, d.MaBienBan })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HOADONTIENDIEN_CHITIETGHIDIEN");
        });

        modelBuilder.Entity<Loaicongto>(entity =>
        {
            entity.HasKey(e => e.MaLoaiCongTo);

            entity.ToTable("LOAICONGTO");

            entity.Property(e => e.MaLoaiCongTo).HasMaxLength(20);
            entity.Property(e => e.TenLoaiCongTo).HasMaxLength(100);
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

            entity.HasIndex(e => e.Cccd).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

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
            entity.Property(e => e.NamSinh).HasColumnName("NamSinh");
            entity.Property(e => e.ThoiGianGui)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(100)
                .HasDefaultValue("Chưa xử lý");

            entity.HasOne(d => d.Cauhinhcapdien).WithMany(p => p.Phieucapdiens)
                .HasForeignKey(d => new { d.MaMucDich, d.MaSoPha })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PHIEUCAPDIEN_CAUHINHCAPDIEN");
        });

        modelBuilder.Entity<Phieughidien>(entity =>
        {
            entity.HasKey(e => e.MaPhieuGhi).HasName("PK__PHIEUGHI__01AC4D1B9283B6EB");

            entity.ToTable("PHIEUGHIDIEN");

            entity.Property(e => e.MaPhieuGhi).HasMaxLength(20);
            entity.Property(e => e.KyGhiChiSo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayGhi).HasColumnType("datetime");
            entity.Property(e => e.NhanVienGhi).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<QuyDinhGiaDien>(entity =>
        {
            entity.HasKey(e => e.Bac);

            entity.ToTable("QUYDINHGIADIEN");

            entity.Property(e => e.Bac).ValueGeneratedNever();

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
