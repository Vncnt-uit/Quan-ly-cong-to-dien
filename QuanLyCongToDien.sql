
-- BM1: Phiếu đề nghị cấp điện

CREATE DATABASE QuanLyCapDien;
GO

USE QuanLyCapDien;
GO

-- 1. Bảng MUCDICH (Mục đích sử dụng)
-- Quy định 1: Có 2 loại mục đích sử dụng:
--   Sinh hoạt và Ngoài sinh hoạt
CREATE TABLE MUCDICH (
    MaMucDich   NVARCHAR(20)    NOT NULL,
    TenMucDich  NVARCHAR(100)   NOT NULL,

    CONSTRAINT PK_MUCDICH PRIMARY KEY (MaMucDich)
);
GO

-- 2. Bảng LOAIPHA (Loại số pha)
-- Quy định 1: Có 2 loại số pha: 1 pha và 3 pha

CREATE TABLE LOAIPHA (
    MaSoPha     NVARCHAR(20)    NOT NULL,
    TenSoPha    NVARCHAR(100)   NOT NULL,

    CONSTRAINT PK_LOAIPHA PRIMARY KEY (MaSoPha)
);
GO

-- 3. Bảng CAUHINHCAPDIEN (Cấu hình cấp điện)

-- Quy định 1: Nếu mục đích sử dụng là Sinh hoạt
--   thì số pha đăng ký phải là 1 pha

CREATE TABLE CAUHINHCAPDIEN (
    MaMucDich   NVARCHAR(20)    NOT NULL,
    MaSoPha     NVARCHAR(20)    NOT NULL,

    CONSTRAINT PK_CAUHINHCAPDIEN PRIMARY KEY (MaMucDich, MaSoPha),

    CONSTRAINT FK_CAUHINHCAPDIEN_MUCDICH
        FOREIGN KEY (MaMucDich)
        REFERENCES MUCDICH (MaMucDich),

    CONSTRAINT FK_CAUHINHCAPDIEN_LOAIPHA
        FOREIGN KEY (MaSoPha)
        REFERENCES LOAIPHA (MaSoPha)
);
GO

-- 4. Bảng PHIEUCAPDIEN (Phiếu đề nghị cấp điện)

CREATE TABLE PHIEUCAPDIEN (
    MaPhieu     NVARCHAR(20)    NOT NULL,
    ThoiGianGui DATETIME        NOT NULL DEFAULT GETDATE(),
    HoTen       NVARCHAR(100)   NOT NULL,
    CCCD        NVARCHAR(12)    NOT NULL,
    NamSinh     DATE            NULL,
    SoDienThoai NVARCHAR(15)    NULL,
    Email       NVARCHAR(100)   NULL,
    DiaChi      NVARCHAR(255)   NOT NULL,
    MaMucDich   NVARCHAR(20)    NOT NULL,
    MaSoPha     NVARCHAR(20)    NOT NULL,

    CONSTRAINT PK_PHIEUCAPDIEN PRIMARY KEY (MaPhieu),

    -- FK tham chiếu đến bảng CAUHINHCAPDIEN (composite key)
    CONSTRAINT FK_PHIEUCAPDIEN_CAUHINHCAPDIEN
        FOREIGN KEY (MaMucDich, MaSoPha)
        REFERENCES CAUHINHCAPDIEN (MaMucDich, MaSoPha)
);
GO

ALTER TABLE PHIEUCAPDIEN
ADD TrangThai NVARCHAR(100) NOT NULL DEFAULT N'Chưa xử lý';

ALTER TABLE PHIEUCAPDIEN
ADD CONSTRAINT UQ_PHIEUCAPDIEN_CCCD UNIQUE (CCCD);
GO

ALTER TABLE PHIEUCAPDIEN
ADD CONSTRAINT UQ_PHIEUCAPDIEN_Email UNIQUE (Email);
GO

ALTER TABLE PHIEUCAPDIEN ADD NamSinh_New INT NULL;
GO

UPDATE PHIEUCAPDIEN
SET NamSinh_New = YEAR(NamSinh)
WHERE NamSinh IS NOT NULL;
GO

ALTER TABLE PHIEUCAPDIEN DROP COLUMN NamSinh;
GO

EXEC sp_rename 'PHIEUCAPDIEN.NamSinh_New', 'NamSinh', 'COLUMN';
GO

-- NHẬP DỮ LIỆU MẪU


-- Dữ liệu bảng MUCDICH
INSERT INTO MUCDICH (MaMucDich, TenMucDich) VALUES
    (N'MD01', N'Sinh hoạt'),
    (N'MD02', N'Ngoài sinh hoạt');
GO

-- Dữ liệu bảng LOAIPHA
INSERT INTO LOAIPHA (MaSoPha, TenSoPha) VALUES
    (N'SP01', N'1 pha'),
    (N'SP02', N'3 pha');
GO

-- Dữ liệu bảng CAUHINHCAPDIEN
-- Quy định 1: Sinh hoạt chỉ được dùng 1 pha
--             Ngoài sinh hoạt được dùng cả 1 pha và 3 pha
INSERT INTO CAUHINHCAPDIEN (MaMucDich, MaSoPha) VALUES
    (N'MD01', N'SP01'),     -- Sinh hoạt - 1 pha (hợp lệ)
    (N'MD02', N'SP01'),     -- Ngoài sinh hoạt - 1 pha (hợp lệ)
    (N'MD02', N'SP02');     -- Ngoài sinh hoạt - 3 pha (hợp lệ)
GO

-- Dữ liệu mẫu bảng PHIEUCAPDIEN
INSERT INTO PHIEUCAPDIEN (MaPhieu, ThoiGianGui, HoTen, CCCD, NamSinh, SoDienThoai, Email, DiaChi, MaMucDich, MaSoPha) VALUES
    (N'PCD001', '2026-05-07', N'Nguyễn Văn A', N'012345678901', '1990-01-15', N'0901234567', N'nguyenvana@email.com', N'123 Đường ABC, Quận 1, TP.HCM', N'MD01', N'SP01'),
    (N'PCD002', '2026-05-07', N'Trần Thị B', N'098765432109', '1985-06-20', N'0912345678', N'tranthib@email.com', N'456 Đường XYZ, Quận 3, TP.HCM', N'MD02', N'SP02');
GO

-- =============================================
-- KIỂM TRA DỮ LIỆU
-- =============================================

-- Xem tất cả phiếu cấp điện với thông tin chi tiết
SELECT 
    p.MaPhieu,
    p.ThoiGianGui,
    p.HoTen,
    p.CCCD,
    p.NamSinh,
    p.SoDienThoai,
    p.Email,
    p.DiaChi,
    p.TrangThai,
    m.TenMucDich   AS [Mục đích sử dụng],
    l.TenSoPha     AS [Số pha đăng ký]
    
FROM PHIEUCAPDIEN p
    INNER JOIN MUCDICH m  ON p.MaMucDich = m.MaMucDich
    INNER JOIN LOAIPHA l  ON p.MaSoPha   = l.MaSoPha;
GO

-- Xem cấu hình cấp điện hợp lệ
SELECT 
    m.TenMucDich   AS [Mục đích sử dụng],
    l.TenSoPha     AS [Số pha đăng ký]
FROM CAUHINHCAPDIEN c
    INNER JOIN MUCDICH m  ON c.MaMucDich = m.MaMucDich
    INNER JOIN LOAIPHA l  ON c.MaSoPha   = l.MaSoPha;
GO

-- BM2: Biên bản cấp điện

-- 5. Bảng LOAICONGTO (Loại công tơ)
CREATE TABLE LOAICONGTO (
    MaLoaiCongTo   NVARCHAR(20)    NOT NULL,
    TenLoaiCongTo  NVARCHAR(100)   NOT NULL,

    CONSTRAINT PK_LOAICONGTO PRIMARY KEY (MaLoaiCongTo)
);
GO

-- 6. Bảng CAUHINHCONGTO (Cấu hình công tơ)
CREATE TABLE CAUHINHCONGTO (
    MaSoPha        NVARCHAR(20)    NOT NULL,
    MaLoaiCongTo   NVARCHAR(20)    NOT NULL,

    CONSTRAINT PK_CAUHINHCONGTO PRIMARY KEY (MaSoPha, MaLoaiCongTo),

    CONSTRAINT FK_CAUHINHCONGTO_LOAIPHA
        FOREIGN KEY (MaSoPha)
        REFERENCES LOAIPHA (MaSoPha),

    CONSTRAINT FK_CAUHINHCONGTO_LOAICONGTO
        FOREIGN KEY (MaLoaiCongTo)
        REFERENCES LOAICONGTO (MaLoaiCongTo)
);
GO

-- 7. Bảng BIENBANCAPDIEN (Biên bản cấp điện)
CREATE TABLE BIENBANCAPDIEN (
    MaBienBan               NVARCHAR(20)    NOT NULL,
    NgayLap                 DATETIME        NOT NULL DEFAULT GETDATE(),
    ThoiGianBatDauCapDien   DATETIME        NULL,
    ChiSoBanDau             INT             NULL,
    ViTriLapDat             NVARCHAR(255)   NULL,
    MaCongTo                NVARCHAR(50)    NULL,
    HangSanXuat             NVARCHAR(100)   NULL,
    NamSanXuat              INT             NULL,
    
    MaPhieu                 NVARCHAR(20)    NOT NULL,
    MaLoaiCongTo            NVARCHAR(20)    NOT NULL,

    CONSTRAINT PK_BIENBANCAPDIEN PRIMARY KEY (MaBienBan),

    CONSTRAINT FK_BIENBANCAPDIEN_PHIEUCAPDIEN
        FOREIGN KEY (MaPhieu)
        REFERENCES PHIEUCAPDIEN (MaPhieu),

    CONSTRAINT FK_BIENBANCAPDIEN_LOAICONGTO
        FOREIGN KEY (MaLoaiCongTo)
        REFERENCES LOAICONGTO (MaLoaiCongTo)
);
GO

ALTER TABLE BIENBANCAPDIEN
ADD CONSTRAINT UQ_BIENBANCAPDIEN_MaPhieu UNIQUE (MaPhieu);
GO

ALTER TABLE BIENBANCAPDIEN
ADD CONSTRAINT UQ_BIENBANCAPDIEN_MaCongTo UNIQUE (MaCongTo);

INSERT INTO LOAICONGTO (MaLoaiCongTo, TenLoaiCongTo) VALUES
    (N'LCT01', N'Công tơ 1 pha'),
    (N'LCT02', N'Công tơ 3 pha');
GO

INSERT INTO CAUHINHCONGTO (MaSoPha, MaLoaiCongTo) VALUES
    (N'SP01', N'LCT01'),
    (N'SP02', N'LCT02');
GO

INSERT INTO BIENBANCAPDIEN (MaBienBan, NgayLap, ThoiGianBatDauCapDien, ChiSoBanDau, ViTriLapDat, MaCongTo, HangSanXuat, NamSanXuat, MaPhieu, MaLoaiCongTo) VALUES
    (
        N'BB001', 
        '2026-05-10', 
        '2026-05-10', 
        0, 
        N'Sân sau nhà', 
        N'CT123456', 
        N'GELEX', 
        2025, 
        N'PCD001', 
        N'LCT01'
    ),
    (
        N'BB002', 
        '2026-05-12 14:00:00', 
        '2026-05-12 15:30:00', 
        0, 
        N'Trạm biến áp nội bộ công ty', 
        N'CT987654', 
        N'EMIC', 
        2026, 
        N'PCD002', 
        N'LCT02'
    );
GO

SELECT 
    bb.MaBienBan,
    bb.NgayLap,
    p.HoTen AS [Tên Khách Hàng],
    p.DiaChi AS [Địa Chỉ],
    lct.TenLoaiCongTo AS [Loại Công Tơ Lắp Đặt],
    bb.MaCongTo AS [Số Seri Công Tơ],
    bb.ChiSoBanDau
FROM BIENBANCAPDIEN bb
    INNER JOIN PHIEUCAPDIEN p ON bb.MaPhieu = p.MaPhieu
    INNER JOIN LOAICONGTO lct ON bb.MaLoaiCongTo = lct.MaLoaiCongTo;
GO

-- 7. Bảng PHIEUGHIDIEN (Phiếu ghi điện)
CREATE TABLE PHIEUGHIDIEN (
    MaPhieuGhi NVARCHAR(20) PRIMARY KEY,
    KyGhiChiSo VARCHAR(20)                       NOT NULL,
    NgayGhi DATETIME                             NOT NULL,
    NhanVienGhi NVARCHAR(100)                    NOT NULL
);

-- 8. Bảng CHITIETGHIDIEN (Chi tiết ghi điện)
CREATE TABLE CHITIETGHIDIEN (
    MaPhieuGhi NVARCHAR(20) NOT NULL,
    MaBienBan NVARCHAR(20) NOT NULL,
    ChiSoCu INT NOT NULL,
    ChiSoMoi INT NOT NULL,
    SanLuongTieuThu INT NOT NULL,

    PRIMARY KEY (MaPhieuGhi, MaBienBan),
    
    -- Khai báo Khóa ngoại
    FOREIGN KEY (MaPhieuGhi) REFERENCES PHIEUGHIDIEN(MaPhieuGhi),
    FOREIGN KEY (MaBienBan) REFERENCES BIENBANCAPDIEN(MaBienBan)
);

INSERT INTO PHIEUGHIDIEN (MaPhieuGhi, KyGhiChiSo, NgayGhi, NhanVienGhi) VALUES
    (N'PGD001', '06/2026', '2026-06-05', N'Dương Văn A'),
    (N'PGD002', '07/2026', '2026-07-05', N'Dương Văn A');
GO

-- Kỳ ghi điện Tháng 06/2026 (Mã phiếu: PGD001)
INSERT INTO CHITIETGHIDIEN (MaPhieuGhi, MaBienBan, ChiSoCu, ChiSoMoi, SanLuongTieuThu) VALUES
    (N'PGD001', N'BB001', 0, 150, 150),
    (N'PGD001', N'BB002', 0, 320, 320);
GO

-- Kỳ ghi điện Tháng 07/2026 (Mã phiếu: PGD002)
INSERT INTO CHITIETGHIDIEN (MaPhieuGhi, MaBienBan, ChiSoCu, ChiSoMoi, SanLuongTieuThu) VALUES
    (N'PGD002', N'BB001', 150, 310, 160),
    (N'PGD002', N'BB002', 320, 700, 380);
GO


SELECT 
    p.KyGhiChiSo AS [Kỳ Ghi],
    p.NgayGhi AS [Ngày Ghi],
    ct.MaBienBan AS [Mã Biên Bản],
    ct.ChiSoCu AS [Chỉ Số Cũ],
    ct.ChiSoMoi AS [Chỉ Số Mới],
    ct.SanLuongTieuThu AS [Sản Lượng Tiêu Thụ (kWh)],
    p.NhanVienGhi AS [Nhân Viên Ghi]
FROM CHITIETGHIDIEN ct
    INNER JOIN PHIEUGHIDIEN p ON ct.MaPhieuGhi = p.MaPhieuGhi
WHERE p.KyGhiChiSo = '06/2026';

-- 10. Bảng HOADONTIENDIEN (Hóa đơn tiền điện)
CREATE TABLE HOADONTIENDIEN (
    MaHoaDon NVARCHAR(20) NOT NULL,
    TongTien DECIMAL(18,2) NOT NULL,
    
    MaPhieuGhi NVARCHAR(20) NOT NULL,
    MaBienBan NVARCHAR(20) NOT NULL, 

    CONSTRAINT PK_HOADONTIENDIEN PRIMARY KEY (MaHoaDon),

    CONSTRAINT FK_HOADONTIENDIEN_CHITIETGHIDIEN
        FOREIGN KEY (MaPhieuGhi, MaBienBan)
        REFERENCES CHITIETGHIDIEN (MaPhieuGhi, MaBienBan)
);
GO

-- 11. Bảng CHITIETHOADON (Chi tiết tính tiền theo bậc)
CREATE TABLE CHITIETHOADON (
    MaHoaDon NVARCHAR(20) NOT NULL,
    Bac INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    DinhMuc INT NOT NULL,
    DienNangTieuThu INT NOT NULL,
    ThanhTien DECIMAL(18,2) NOT NULL,

    CONSTRAINT PK_CHITIETHOADON PRIMARY KEY (MaHoaDon, Bac),

    CONSTRAINT FK_CHITIETHOADON_HOADONTIENDIEN
        FOREIGN KEY (MaHoaDon)
        REFERENCES HOADONTIENDIEN (MaHoaDon)
);
GO
ALTER TABLE CHITIETHOADON
ALTER COLUMN DinhMuc INT NULL;
GO

INSERT INTO HOADONTIENDIEN (MaHoaDon, TongTien, MaPhieuGhi, MaBienBan)
VALUES (N'HD_062026_BB001', 175000.00, N'PGD001', N'BB001');

INSERT INTO CHITIETHOADON (MaHoaDon, Bac, DonGia, DinhMuc, DienNangTieuThu, ThanhTien)
VALUES
    (N'HD_062026_BB001', 1, 1000, 100, 100, 100000),
    (N'HD_062026_BB001', 2, 1500, 100, 50, 75000);

SELECT 
    -- Parent Data (The Header)
    p.MaPhieuGhi,
    p.KyGhiChiSo,
    p.NgayGhi,
    p.NhanVienGhi,
    
    -- Child Data (The DataGrid Rows)
    c.MaBienBan,
    c.ChiSoCu,
    c.ChiSoMoi,
    c.SanLuongTieuThu
FROM 
    PHIEUGHIDIEN p
LEFT JOIN 
    CHITIETGHIDIEN c ON p.MaPhieuGhi = c.MaPhieuGhi
ORDER BY 
    p.NgayGhi DESC, 
    c.MaBienBan;

-- Bang quy dinh gia dien (De co the thuc hien dieu chinh gia dien)
CREATE TABLE QUYDINHGIADIEN (
Bac INT NOT NULL,
DonGia DECIMAL(18,2) NOT NULL,
DinhMuc INT NULL,

    CONSTRAINT PK_QUYDINHGIADIEN PRIMARY KEY (Bac)
);
GO

INSERT INTO QUYDINHGIADIEN (Bac, DonGia, DinhMuc)
VALUES 
    (1, 1000, 100),
    (2, 1500, 100),
    (3, 2000, NULL);
GO