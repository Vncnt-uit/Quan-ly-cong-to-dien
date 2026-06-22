
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


-- DỮ LIỆU BẢNG PHIẾU GHI ĐIỆN (PHIEUGHIDIEN) TỪ 12/2020 ĐẾN 12/2025
INSERT INTO PHIEUGHIDIEN (MaPhieuGhi, KyGhiChiSo, NgayGhi, NhanVienGhi) VALUES
    -- Cột mốc cuối 2020 để T1/2021 có số sánh
    (N'PGD_20_12', '12/2020', '2020-12-05', N'Dương Văn A'),

    -- NĂM 2021 (Đầy đủ 12 tháng)
    (N'PGD_21_01', '01/2021', '2021-01-05', N'Dương Văn A'), (N'PGD_21_02', '02/2021', '2021-02-05', N'Trần Thị B'),
    (N'PGD_21_03', '03/2021', '2021-03-05', N'Trần Thị B'), (N'PGD_21_04', '04/2021', '2021-04-05', N'Dương Văn A'),
    (N'PGD_21_05', '05/2021', '2021-05-05', N'Dương Văn A'), (N'PGD_21_06', '06/2021', '2021-06-05', N'Trần Thị B'),
    (N'PGD_21_07', '07/2021', '2021-07-05', N'Trần Thị B'), (N'PGD_21_08', '08/2021', '2021-08-05', N'Dương Văn A'),
    (N'PGD_21_09', '09/2021', '2021-09-05', N'Dương Văn A'), (N'PGD_21_10', '10/2021', '2021-10-05', N'Trần Thị B'),
    (N'PGD_21_11', '11/2021', '2021-11-05', N'Trần Thị B'), (N'PGD_21_12', '12/2021', '2021-12-05', N'Dương Văn A'),

    -- NĂM 2022 (Đầy đủ 12 tháng)
    (N'PGD_22_01', '01/2022', '2022-01-05', N'Dương Văn A'), (N'PGD_22_02', '02/2022', '2022-02-05', N'Trần Thị B'),
    (N'PGD_22_03', '03/2022', '2022-03-05', N'Trần Thị B'), (N'PGD_22_04', '04/2022', '2022-04-05', N'Dương Văn A'),
    (N'PGD_22_05', '05/2022', '2022-05-05', N'Dương Văn A'), (N'PGD_22_06', '06/2022', '2022-06-05', N'Trần Thị B'),
    (N'PGD_22_07', '07/2022', '2022-07-05', N'Trần Thị B'), (N'PGD_22_08', '08/2022', '2022-08-05', N'Dương Văn A'),
    (N'PGD_22_09', '09/2022', '2022-09-05', N'Dương Văn A'), (N'PGD_22_10', '10/2022', '2022-10-05', N'Trần Thị B'),
    (N'PGD_22_11', '11/2022', '2022-11-05', N'Trần Thị B'), (N'PGD_22_12', '12/2022', '2022-12-05', N'Dương Văn A'),

    -- NĂM 2023 (Đầy đủ 12 tháng)
    (N'PGD_23_01', '01/2023', '2023-01-05', N'Dương Văn A'), (N'PGD_23_02', '02/2023', '2023-02-05', N'Trần Thị B'),
    (N'PGD_23_03', '03/2023', '2023-03-05', N'Trần Thị B'), (N'PGD_23_04', '04/2023', '2023-04-05', N'Dương Văn A'),
    (N'PGD_23_05', '05/2023', '2023-05-05', N'Dương Văn A'), (N'PGD_23_06', '06/2023', '2023-06-05', N'Trần Thị B'),
    (N'PGD_23_07', '07/2023', '2023-07-05', N'Trần Thị B'), (N'PGD_23_08', '08/2023', '2023-08-05', N'Dương Văn A'),
    (N'PGD_23_09', '09/2023', '2023-09-05', N'Dương Văn A'), (N'PGD_23_10', '10/2023', '2023-10-05', N'Trần Thị B'),
    (N'PGD_23_11', '11/2023', '2023-11-05', N'Trần Thị B'), (N'PGD_23_12', '12/2023', '2023-12-05', N'Dương Văn A'),

    -- NĂM 2024 (Đầy đủ 12 tháng)
    (N'PGD_24_01', '01/2024', '2024-01-05', N'Dương Văn A'), (N'PGD_24_02', '02/2024', '2024-02-05', N'Trần Thị B'),
    (N'PGD_24_03', '03/2024', '2024-03-05', N'Trần Thị B'), (N'PGD_24_04', '04/2024', '2024-04-05', N'Dương Văn A'),
    (N'PGD_24_05', '05/2024', '2024-05-05', N'Dương Văn A'), (N'PGD_24_06', '06/2024', '2024-06-05', N'Trần Thị B'),
    (N'PGD_24_07', '07/2024', '2024-07-05', N'Trần Thị B'), (N'PGD_24_08', '08/2024', '2024-08-05', N'Dương Văn A'),
    (N'PGD_24_09', '09/2024', '2024-09-05', N'Dương Văn A'), (N'PGD_24_10', '10/2024', '2024-10-05', N'Trần Thị B'),
    (N'PGD_24_11', '11/2024', '2024-11-05', N'Trần Thị B'), (N'PGD_24_12', '12/2024', '2024-12-05', N'Dương Văn A'),

    -- NĂM 2025 (Đầy đủ 12 tháng)
    (N'PGD_25_01', '01/2025', '2025-01-05', N'Dương Văn A'), (N'PGD_25_02', '02/2025', '2025-02-05', N'Trần Thị B'),
    (N'PGD_25_03', '03/2025', '2025-03-05', N'Trần Thị B'), (N'PGD_25_04', '04/2025', '2025-04-05', N'Dương Văn A'),
    (N'PGD_25_05', '05/2025', '2025-05-05', N'Dương Văn A'), (N'PGD_25_06', '06/2025', '2025-06-05', N'Trần Thị B'),
    (N'PGD_25_07', '07/2025', '2025-07-05', N'Trần Thị B'), (N'PGD_25_08', '08/2025', '2025-08-05', N'Dương Văn A'),
    (N'PGD_25_09', '09/2025', '2025-09-05', N'Dương Văn A'), (N'PGD_25_10', '10/2025', '2025-10-05', N'Trần Thị B'),
    (N'PGD_25_11', '11/2025', '2025-11-05', N'Trần Thị B'), (N'PGD_25_12', '12/2025', '2025-12-05', N'Dương Văn A');
GO


-- CHI TIẾT GHI ĐIỆN
INSERT INTO CHITIETGHIDIEN (MaPhieuGhi, MaBienBan, ChiSoCu, ChiSoMoi, SanLuongTieuThu) VALUES
    -- Khởi tạo cuối năm 2020
    (N'PGD_20_12', N'BB001', 0, 100, 100), (N'PGD_20_12', N'BB002', 0, 300, 300),

    -- Dữ liệu 2021
    (N'PGD_21_01', N'BB001', 100, 250, 150), (N'PGD_21_01', N'BB002', 300, 650, 350),   -- Tăng
    (N'PGD_21_02', N'BB001', 250, 370, 120), (N'PGD_21_02', N'BB002', 650, 930, 280),   -- Giảm
    (N'PGD_21_03', N'BB001', 370, 500, 130), (N'PGD_21_03', N'BB002', 930, 1250, 320),  -- Tăng
    (N'PGD_21_04', N'BB001', 500, 680, 180), (N'PGD_21_04', N'BB002', 1250, 1650, 400), -- Tăng mạnh
    (N'PGD_21_05', N'BB001', 680, 880, 200), (N'PGD_21_05', N'BB002', 1650, 2150, 500), -- Tăng đỉnh điểm
    (N'PGD_21_06', N'BB001', 880, 1070, 190), (N'PGD_21_06', N'BB002', 2150, 2600, 450), -- Giảm nhẹ
    (N'PGD_21_07', N'BB001', 1070, 1260, 190), (N'PGD_21_07', N'BB002', 2600, 3050, 450), -- Đi ngang
    (N'PGD_21_08', N'BB001', 1260, 1420, 160), (N'PGD_21_08', N'BB002', 3050, 3400, 350), -- Giảm
    (N'PGD_21_09', N'BB001', 1420, 1560, 140), (N'PGD_21_09', N'BB002', 3400, 3700, 300), -- Giảm
    (N'PGD_21_10', N'BB001', 1560, 1680, 120), (N'PGD_21_10', N'BB002', 3700, 3950, 250), -- Giảm sâu
    (N'PGD_21_11', N'BB001', 1680, 1790, 110), (N'PGD_21_11', N'BB002', 3950, 4170, 220), -- Đáy
    (N'PGD_21_12', N'BB001', 1790, 1940, 150), (N'PGD_21_12', N'BB002', 4170, 4470, 300), -- Tăng trở lại

    -- Dữ liệu 2022
    (N'PGD_22_01', N'BB001', 1940, 2100, 160), (N'PGD_22_01', N'BB002', 4470, 4800, 330), 
    (N'PGD_22_02', N'BB002', 4800, 5080, 280), -- Tháng này cố tình thiếu BB001 để test
    (N'PGD_22_03', N'BB001', 2100, 2400, 300), (N'PGD_22_03', N'BB002', 5080, 5400, 320), -- BB1 vọt lên bù tháng trước
    (N'PGD_22_04', N'BB001', 2400, 2550, 150), (N'PGD_22_04', N'BB002', 5400, 5750, 350),
    (N'PGD_22_05', N'BB001', 2550, 2750, 200), (N'PGD_22_05', N'BB002', 5750, 6200, 450),
    (N'PGD_22_06', N'BB001', 2750, 2970, 220), (N'PGD_22_06', N'BB002', 6200, 6700, 500),
    (N'PGD_22_07', N'BB001', 2970, 3190, 220), (N'PGD_22_07', N'BB002', 6700, 7150, 450),
    (N'PGD_22_08', N'BB001', 3190, 3370, 180), (N'PGD_22_08', N'BB002', 7150, 7500, 350),
    (N'PGD_22_09', N'BB001', 3370, 3520, 150), (N'PGD_22_09', N'BB002', 7500, 7800, 300),
    (N'PGD_22_10', N'BB001', 3520, 3650, 130), (N'PGD_22_10', N'BB002', 7800, 8050, 250),
    (N'PGD_22_11', N'BB001', 3650, 3750, 100), (N'PGD_22_11', N'BB002', 8050, 8250, 200),
    (N'PGD_22_12', N'BB001', 3750, 3880, 130), (N'PGD_22_12', N'BB002', 8250, 8530, 280),

    -- Dữ liệu 2023
    (N'PGD_23_01', N'BB001', 3880, 4040, 160), (N'PGD_23_01', N'BB002', 8530, 8880, 350),
    (N'PGD_23_02', N'BB001', 4040, 4160, 120), (N'PGD_23_02', N'BB002', 8880, 9150, 270),
    (N'PGD_23_03', N'BB001', 4160, 4300, 140), (N'PGD_23_03', N'BB002', 9150, 9460, 310),
    (N'PGD_23_04', N'BB001', 4300, 4480, 180), (N'PGD_23_04', N'BB002', 9460, 9880, 420),
    (N'PGD_23_05', N'BB001', 4480, 4690, 210), (N'PGD_23_05', N'BB002', 9880, 10400, 520),
    (N'PGD_23_06', N'BB001', 4690, 4890, 200), (N'PGD_23_06', N'BB002', 10400, 10880, 480),
    (N'PGD_23_07', N'BB001', 4890, 5080, 190), (N'PGD_23_07', N'BB002', 10880, 11330, 450),
    (N'PGD_23_08', N'BB001', 5080, 5240, 160), (N'PGD_23_08', N'BB002', 11330, 11700, 370),
    (N'PGD_23_09', N'BB001', 5240, 5380, 140), (N'PGD_23_09', N'BB002', 11700, 12020, 320),
    (N'PGD_23_10', N'BB001', 5380, 5500, 120), (N'PGD_23_10', N'BB002', 12020, 12280, 260),
    (N'PGD_23_11', N'BB001', 5500, 5600, 100), (N'PGD_23_11', N'BB002', 12280, 12480, 200),
    (N'PGD_23_12', N'BB001', 5600, 5740, 140), (N'PGD_23_12', N'BB002', 12480, 12780, 300),

    -- Dữ liệu 2024
    (N'PGD_24_01', N'BB001', 5740, 5910, 170), (N'PGD_24_01', N'BB002', 12780, 13130, 350),
    (N'PGD_24_02', N'BB001', 5910, 6040, 130), (N'PGD_24_02', N'BB002', 13130, 13420, 290),
    (N'PGD_24_03', N'BB001', 6040, 6190, 150), (N'PGD_24_03', N'BB002', 13420, 13750, 330),
    (N'PGD_24_04', N'BB001', 6190, 6380, 190), (N'PGD_24_04', N'BB002', 13750, 14190, 440),
    (N'PGD_24_05', N'BB001', 6380, 6610, 230), (N'PGD_24_05', N'BB002', 14190, 14740, 550),
    (N'PGD_24_06', N'BB001', 6610, 6830, 220), (N'PGD_24_06', N'BB002', 14740, 15250, 510),
    (N'PGD_24_07', N'BB001', 6830, 7030, 200), (N'PGD_24_07', N'BB002', 15250, 15720, 470),
    (N'PGD_24_08', N'BB001', 7030, 7200, 170), (N'PGD_24_08', N'BB002', 15720, 16100, 380),
    (N'PGD_24_09', N'BB001', 7200, 7350, 150), (N'PGD_24_09', N'BB002', 16100, 16420, 320),
    (N'PGD_24_10', N'BB001', 7350, 7480, 130), (N'PGD_24_10', N'BB002', 16420, 16690, 270),
    (N'PGD_24_11', N'BB001', 7480, 7590, 110), (N'PGD_24_11', N'BB002', 16690, 16900, 210),
    (N'PGD_24_12', N'BB001', 7590, 7740, 150), (N'PGD_24_12', N'BB002', 16900, 17220, 320),

    -- Dữ liệu 2025
    (N'PGD_25_01', N'BB001', 7740, 7920, 180), (N'PGD_25_01', N'BB002', 17220, 17600, 380),
    (N'PGD_25_02', N'BB001', 7920, 8060, 140), (N'PGD_25_02', N'BB002', 17600, 17910, 310),
    (N'PGD_25_03', N'BB001', 8060, 8220, 160), (N'PGD_25_03', N'BB002', 17910, 18260, 350),
    (N'PGD_25_04', N'BB001', 8220, 8420, 200), (N'PGD_25_04', N'BB002', 18260, 18720, 460),
    (N'PGD_25_05', N'BB001', 8420, 8660, 240), (N'PGD_25_05', N'BB002', 18720, 19300, 580),
    (N'PGD_25_06', N'BB001', 8660, 8890, 230), (N'PGD_25_06', N'BB002', 19300, 19830, 530),
    (N'PGD_25_07', N'BB001', 8890, 9100, 210), (N'PGD_25_07', N'BB002', 19830, 20320, 490),
    (N'PGD_25_08', N'BB001', 9100, 9280, 180), (N'PGD_25_08', N'BB002', 20320, 20720, 400),
    (N'PGD_25_09', N'BB001', 9280, 9440, 160), (N'PGD_25_09', N'BB002', 20720, 21060, 340),
    (N'PGD_25_10', N'BB001', 9440, 9580, 140), (N'PGD_25_10', N'BB002', 21060, 21350, 290),
    (N'PGD_25_11', N'BB001', 9580, 9700, 120), (N'PGD_25_11', N'BB002', 21350, 21580, 230),
    (N'PGD_25_12', N'BB001', 9700, 9860, 160), (N'PGD_25_12', N'BB002', 21580, 21920, 340);
GO