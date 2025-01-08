using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace QLNhaHang.Models
{
    public partial class QLNhaHangContext : DbContext
    {
        public QLNhaHangContext()
        {
        }

        public QLNhaHangContext(DbContextOptions<QLNhaHangContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ban> Bans { get; set; } = null!;
        public virtual DbSet<Ca> Cas { get; set; } = null!;
        public virtual DbSet<DangKyCa> DangKyCas { get; set; } = null!;
        public virtual DbSet<DatBan> DatBans { get; set; } = null!;
        public virtual DbSet<HoaDon> HoaDons { get; set; } = null!;
        public virtual DbSet<HoaDonChiTiet> HoaDonChiTiets { get; set; } = null!;
        public virtual DbSet<LichSuChuyenBan> LichSuChuyenBans { get; set; } = null!;
        public virtual DbSet<LoaiMonAn> LoaiMonAns { get; set; } = null!;
        public virtual DbSet<MonAn> MonAns { get; set; } = null!;
        public virtual DbSet<NhanVien> NhanViens { get; set; } = null!;
        public virtual DbSet<SoLuongChiTietTrongCa> SoLuongChiTietTrongCas { get; set; } = null!;
        public virtual DbSet<SoLuongTrongCa> SoLuongTrongCas { get; set; } = null!;
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; } = null!;
        public virtual DbSet<ViTriCongViec> ViTriCongViecs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ban>(entity =>
            {
                entity.HasKey(e => e.MaBan)
                    .HasName("PK__Ban__3520ED6CF718F898");

                entity.ToTable("Ban");

                entity.Property(e => e.MaBan).HasMaxLength(30);

                entity.Property(e => e.ViTri).HasMaxLength(30);
            });

            modelBuilder.Entity<Ca>(entity =>
            {
                entity.HasKey(e => e.MaCa)
                    .HasName("PK__Ca__27258E7BA9C9ED66");

                entity.ToTable("Ca");

                entity.Property(e => e.MaCa).HasMaxLength(30);

                entity.Property(e => e.LoaiCa).HasMaxLength(50);
            });

            modelBuilder.Entity<DangKyCa>(entity =>
            {
                entity.HasKey(e => e.MaDangKy)
                    .HasName("PK__DangKyCa__BA90F02D894481CA");

                entity.ToTable("DangKyCa");

                entity.Property(e => e.MaDangKy).HasMaxLength(30);

                entity.Property(e => e.MaNv)
                    .HasMaxLength(30)
                    .HasColumnName("MaNV");

                entity.Property(e => e.MaQuanLy).HasMaxLength(30);

                entity.Property(e => e.Ngay).HasColumnType("date");

                entity.HasOne(d => d.MaNvNavigation)
                    .WithMany(p => p.DangKyCas)
                    .HasForeignKey(d => d.MaNv)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DangKyCa__MaNV__4BAC3F29");

                entity.HasOne(d => d.MaQuanLyNavigation)
                    .WithMany(p => p.DangKyCas)
                    .HasForeignKey(d => d.MaQuanLy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DangKyCa__MaQuan__4AB81AF0");
            });

            modelBuilder.Entity<DatBan>(entity =>
            {
                entity.HasKey(e => e.MaDatBan)
                    .HasName("PK__DatBan__703DFB75FA568ADB");

                entity.ToTable("DatBan");

                entity.Property(e => e.MaDatBan).HasMaxLength(30);

                entity.Property(e => e.MaBan).HasMaxLength(30);

                entity.Property(e => e.NgayDatBan).HasColumnType("datetime");

                entity.Property(e => e.Sdt)
                    .HasMaxLength(10)
                    .HasColumnName("SDT");

                entity.Property(e => e.TenKh)
                    .HasMaxLength(50)
                    .HasColumnName("TenKH");

                entity.HasOne(d => d.MaBanNavigation)
                    .WithMany(p => p.DatBans)
                    .HasForeignKey(d => d.MaBan)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DatBan__MaBan__5070F446");
            });

            modelBuilder.Entity<HoaDon>(entity =>
            {
                entity.HasKey(e => e.MaHoaDon)
                    .HasName("PK__HoaDon__835ED13B34605EE1");

                entity.ToTable("HoaDon");

                entity.Property(e => e.MaHoaDon).HasMaxLength(30);

                entity.Property(e => e.MaDatBan).HasMaxLength(30);

                entity.Property(e => e.NgayXuatHd)
                    .HasColumnType("datetime")
                    .HasColumnName("NgayXuatHD");

                entity.HasOne(d => d.MaDatBanNavigation)
                    .WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.MaDatBan)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HoaDon__MaDatBan__534D60F1");
            });

            modelBuilder.Entity<HoaDonChiTiet>(entity =>
            {
                entity.HasKey(e => new { e.MaHoaDon, e.MaMonAn })
                    .HasName("PK__HoaDonCh__C84FA059C0A34501");

                entity.ToTable("HoaDonChiTiet");

                entity.Property(e => e.MaHoaDon).HasMaxLength(30);

                entity.Property(e => e.MaMonAn).HasMaxLength(30);

                entity.HasOne(d => d.MaHoaDonNavigation)
                    .WithMany(p => p.HoaDonChiTiets)
                    .HasForeignKey(d => d.MaHoaDon)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HoaDonChi__MaHoa__5629CD9C");

                entity.HasOne(d => d.MaMonAnNavigation)
                    .WithMany(p => p.HoaDonChiTiets)
                    .HasForeignKey(d => d.MaMonAn)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HoaDonChi__MaMon__571DF1D5");
            });

            modelBuilder.Entity<LichSuChuyenBan>(entity =>
            {
                entity.HasKey(e => e.MaChuyenBan)
                    .HasName("PK__LichSuCh__9B5035546BB9598E");

                entity.ToTable("LichSuChuyenBan");

                entity.Property(e => e.MaChuyenBan).HasMaxLength(30);

                entity.Property(e => e.MaBanCu).HasMaxLength(30);

                entity.Property(e => e.MaBanMoi).HasMaxLength(30);

                entity.Property(e => e.MaDatBan).HasMaxLength(30);

                entity.Property(e => e.MaNv)
                    .HasMaxLength(30)
                    .HasColumnName("MaNV");

                entity.Property(e => e.ThoiGianChuyen).HasColumnType("datetime");

                entity.HasOne(d => d.MaBanCuNavigation)
                    .WithMany(p => p.LichSuChuyenBanMaBanCuNavigations)
                    .HasForeignKey(d => d.MaBanCu)
                    .HasConstraintName("FK__LichSuChu__MaBan__5AEE82B9");

                entity.HasOne(d => d.MaBanMoiNavigation)
                    .WithMany(p => p.LichSuChuyenBanMaBanMoiNavigations)
                    .HasForeignKey(d => d.MaBanMoi)
                    .HasConstraintName("FK__LichSuChu__MaBan__5BE2A6F2");

                entity.HasOne(d => d.MaDatBanNavigation)
                    .WithMany(p => p.LichSuChuyenBans)
                    .HasForeignKey(d => d.MaDatBan)
                    .HasConstraintName("FK__LichSuChu__MaDat__59FA5E80");

                entity.HasOne(d => d.MaNvNavigation)
                    .WithMany(p => p.LichSuChuyenBans)
                    .HasForeignKey(d => d.MaNv)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LichSuChuy__MaNV__5CD6CB2B");
            });

            modelBuilder.Entity<LoaiMonAn>(entity =>
            {
                entity.HasKey(e => e.MaLoaiMa)
                    .HasName("PK__LoaiMonA__12253B4564CEBB8E");

                entity.ToTable("LoaiMonAn");

                entity.Property(e => e.MaLoaiMa)
                    .HasMaxLength(30)
                    .HasColumnName("MaLoaiMA");

                entity.Property(e => e.TenLoaiMa)
                    .HasMaxLength(60)
                    .HasColumnName("TenLoaiMA");
            });

            modelBuilder.Entity<MonAn>(entity =>
            {
                entity.HasKey(e => e.MaMonAn)
                    .HasName("PK__MonAn__B1171625BC31F00A");

                entity.ToTable("MonAn");

                entity.Property(e => e.MaMonAn).HasMaxLength(30);

                entity.Property(e => e.HinhAnh).HasMaxLength(1024);

                entity.Property(e => e.LoaiMa)
                    .HasMaxLength(30)
                    .HasColumnName("LoaiMA");

                entity.Property(e => e.MoTa).HasMaxLength(60);

                entity.Property(e => e.TenMonAn).HasMaxLength(60);

                entity.HasOne(d => d.LoaiMaNavigation)
                    .WithMany(p => p.MonAns)
                    .HasForeignKey(d => d.LoaiMa)
                    .HasConstraintName("FK__MonAn__LoaiMA__398D8EEE");
            });

            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.HasKey(e => e.MaNv)
                    .HasName("PK__NhanVien__2725D70AD1CA8581");

                entity.ToTable("NhanVien");

                entity.Property(e => e.MaNv)
                    .HasMaxLength(30)
                    .HasColumnName("MaNV");

                entity.Property(e => e.Cccd)
                    .HasMaxLength(12)
                    .HasColumnName("CCCD");

                entity.Property(e => e.DiaChi).HasMaxLength(1000);

                entity.Property(e => e.Email).HasMaxLength(40);

                entity.Property(e => e.GioiTinh).HasMaxLength(3);

                entity.Property(e => e.HinhAnh).HasMaxLength(1024);

                entity.Property(e => e.MaBhyt)
                    .HasMaxLength(15)
                    .HasColumnName("MaBHYT");

                entity.Property(e => e.MaQuanLy).HasMaxLength(30);

                entity.Property(e => e.MaViTriCv)
                    .HasMaxLength(30)
                    .HasColumnName("MaViTriCV");

                entity.Property(e => e.NgayVaoLam).HasColumnType("date");

                entity.Property(e => e.Sdt)
                    .HasMaxLength(10)
                    .HasColumnName("SDT");

                entity.Property(e => e.TenNv)
                    .HasMaxLength(100)
                    .HasColumnName("TenNV");

                entity.Property(e => e.TrangThai).HasMaxLength(60);

                entity.HasOne(d => d.MaQuanLyNavigation)
                    .WithMany(p => p.InverseMaQuanLyNavigation)
                    .HasForeignKey(d => d.MaQuanLy)
                    .HasConstraintName("FK__NhanVien__MaQuan__3F466844");

                entity.HasOne(d => d.MaViTriCvNavigation)
                    .WithMany(p => p.NhanViens)
                    .HasForeignKey(d => d.MaViTriCv)
                    .HasConstraintName("FK__NhanVien__MaViTr__3E52440B");
            });

            modelBuilder.Entity<SoLuongChiTietTrongCa>(entity =>
            {
                entity.HasKey(e => e.MaQuanLyChiTiet)
                    .HasName("PK__SoLuongC__3AA1F0C96A5CC8B7");

                entity.ToTable("SoLuongChiTietTrongCa");

                entity.Property(e => e.MaQuanLyChiTiet).HasMaxLength(30);

                entity.Property(e => e.MaQuanLy).HasMaxLength(30);

                entity.Property(e => e.MaViTriCv)
                    .HasMaxLength(30)
                    .HasColumnName("MaViTriCV");

                entity.HasOne(d => d.MaQuanLyNavigation)
                    .WithMany(p => p.SoLuongChiTietTrongCas)
                    .HasForeignKey(d => d.MaQuanLy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SoLuongCh__MaQua__46E78A0C");

                entity.HasOne(d => d.MaViTriCvNavigation)
                    .WithMany(p => p.SoLuongChiTietTrongCas)
                    .HasForeignKey(d => d.MaViTriCv)
                    .HasConstraintName("FK__SoLuongCh__MaViT__47DBAE45");
            });

            modelBuilder.Entity<SoLuongTrongCa>(entity =>
            {
                entity.HasKey(e => e.MaQuanLy)
                    .HasName("PK__SoLuongT__2AB9EAF8640D436F");

                entity.ToTable("SoLuongTrongCa");

                entity.Property(e => e.MaQuanLy).HasMaxLength(30);

                entity.Property(e => e.MaCa).HasMaxLength(30);

                entity.Property(e => e.Ngay).HasColumnType("date");

                entity.HasOne(d => d.MaCaNavigation)
                    .WithMany(p => p.SoLuongTrongCas)
                    .HasForeignKey(d => d.MaCa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SoLuongTro__MaCa__440B1D61");
            });

            modelBuilder.Entity<TaiKhoan>(entity =>
            {
                entity.HasKey(e => e.TaiKhoan1)
                    .HasName("PK__TaiKhoan__D5B8C7F189825D87");

                entity.ToTable("TaiKhoan");

                entity.Property(e => e.TaiKhoan1)
                    .HasMaxLength(30)
                    .HasColumnName("TaiKhoan");

                entity.Property(e => e.MaNv)
                    .HasMaxLength(30)
                    .HasColumnName("MaNV");

                entity.Property(e => e.MatKhau).HasMaxLength(30);

                entity.HasOne(d => d.MaNvNavigation)
                    .WithMany(p => p.TaiKhoans)
                    .HasForeignKey(d => d.MaNv)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TaiKhoan__MaNV__5FB337D6");
            });

            modelBuilder.Entity<ViTriCongViec>(entity =>
            {
                entity.HasKey(e => e.MaViTriCv)
                    .HasName("PK__ViTriCon__F720CA709FC4D780");

                entity.ToTable("ViTriCongViec");

                entity.Property(e => e.MaViTriCv)
                    .HasMaxLength(30)
                    .HasColumnName("MaViTriCV");

                entity.Property(e => e.TenViTriCv)
                    .HasMaxLength(60)
                    .HasColumnName("TenViTriCV");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
