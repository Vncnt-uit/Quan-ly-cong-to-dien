
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

INSERT INTO PHIEUCAPDIEN (MaPhieu, ThoiGianGui, HoTen, CCCD, NamSinh, SoDienThoai, Email, DiaChi, MaMucDich, MaSoPha, TrangThai) VALUES
    (N'PCD001', '2026-04-10', N'Nguyễn Văn An',  N'001122334455', 1985, N'0901111111', N'an.nguyen@email.com', N'12 Lê Lợi, Q1, HCM', N'MD01', N'SP01', N'Đã xử lý'),
    (N'PCD002', '2026-04-11', N'Trần Thị Bích', N'001122334456', 1990, N'0902222222', N'bich.tran@email.com', N'34 Hai Bà Trưng, Q1, HCM', N'MD01', N'SP01', N'Đã xử lý'),
    (N'PCD003', '2026-04-12', N'Cửa Hàng Cafe X', N'001122334457', 1980, N'0903333333', N'cafex@email.com', N'56 Nguyễn Huệ, Q1, HCM', N'MD02', N'SP02', N'Đã xử lý'),
    (N'PCD004', '2026-04-13', N'Lê Hoàng Dũng',  N'001122334458', 1975, N'0904444444', N'dung.le@email.com', N'78 Pasteur, Q3, HCM', N'MD02', N'SP01', N'Đã xử lý'),
    (N'PCD005', '2026-04-14', N'Phạm Mỹ Hạnh',   N'001122334459', 1995, N'0905555555', N'hanh.pham@email.com', N'90 Điện Biên Phủ, Q3, HCM', N'MD01', N'SP01', N'Đã xử lý'),
    (N'PCD006', '2026-04-15', N'Xưởng Gỗ Minh',  N'001122334460', 1968, N'0906666666', N'xuongminh@email.com', N'12A KCN Tân Bình, HCM', N'MD02', N'SP02', N'Đã xử lý'),
    (N'PCD007', '2026-04-16', N'Trịnh Văn Giao', N'001122334461', 1982, N'0907777777', N'giao.trinh@email.com', N'45C Lạc Long Quân, Tân Bình, HCM', N'MD01', N'SP01', N'Đã xử lý'),
    (N'PCD008', '2026-04-17', N'Vũ Quỳnh Hương', N'001122334462', 1999, N'0908888888', N'huong.vu@email.com', N'89B Cách Mạng Tháng 8, Tân Bình, HCM', N'MD02', N'SP02', N'Đã xử lý'),
    (N'PCD009', '2026-04-18', N'Spa Hoa Mai',    N'001122334463', 1988, N'0909999999', N'spahoamai@email.com', N'100 Lê Văn Sỹ, Tân Bình, HCM', N'MD01', N'SP01', N'Đã xử lý'),
    (N'PCD010', '2026-04-19', N'Cơ Quan Thuế',   N'001122334464', 1970, N'0900000000', N'thue@email.com', N'200 Trường Chinh, Q3, HCM', N'MD02', N'SP01', N'Đã xử lý');
GO

INSERT INTO BIENBANCAPDIEN (MaBienBan, NgayLap, ThoiGianBatDauCapDien, ChiSoBanDau, ViTriLapDat, MaCongTo, HangSanXuat, NamSanXuat, MaPhieu, MaLoaiCongTo) VALUES
    (N'BB001', '2026-04-15', '2026-04-15 08:00:00', 0, N'Cột điện trước nhà', N'CT000001', N'GELEX', 2025, N'PCD001', N'LCT01'),
    (N'BB002', '2026-04-16', '2026-04-16 09:30:00', 0, N'Tường rào', N'CT000002', N'EMIC',  2026, N'PCD002', N'LCT01'),
    (N'BB003', '2026-04-17', '2026-04-17 10:00:00', 0, N'Trạm biến áp nhỏ', N'CT000003', N'GELEX', 2025, N'PCD003', N'LCT02'),
    (N'BB004', '2026-04-18', '2026-04-18 13:00:00', 0, N'Cột điện trước nhà', N'CT000004', N'EMIC',  2024, N'PCD004', N'LCT01'),
    (N'BB005', '2026-04-19', '2026-04-19 14:15:00', 0, N'Hành lang', N'CT000005', N'GELEX', 2026, N'PCD005', N'LCT01'),
    (N'BB006', '2026-04-20', '2026-04-20 08:45:00', 0, N'Trạm biến áp xưởng', N'CT000006', N'EMIC',  2025, N'PCD006', N'LCT02'),
    (N'BB007', '2026-04-21', '2026-04-21 09:00:00', 0, N'Tường trước nhà', N'CT000007', N'GELEX', 2024, N'PCD007', N'LCT01'),
    (N'BB008', '2026-04-22', '2026-04-22 10:30:00', 0, N'Cột điện ngõ', N'CT000008', N'EMIC',  2026, N'PCD008', N'LCT02'),
    (N'BB009', '2026-04-23', '2026-04-23 15:00:00', 0, N'Hộp kỹ thuật tầng 1', N'CT000009', N'GELEX', 2025, N'PCD009', N'LCT01'),
    (N'BB010', '2026-04-24', '2026-04-24 16:00:00', 0, N'Trạm điện tòa nhà', N'CT000010', N'EMIC',  2026, N'PCD010', N'LCT01');
GO

INSERT INTO PHIEUGHIDIEN (MaPhieuGhi, KyGhiChiSo, NgayGhi, NhanVienGhi) VALUES
    (N'PGD_26_05', '05/2026', '2026-05-30', N'Trần Ghi Điện');
GO

INSERT INTO CHITIETGHIDIEN (MaPhieuGhi, MaBienBan, ChiSoCu, ChiSoMoi, SanLuongTieuThu) VALUES
    (N'PGD_26_05', N'BB001', 0, 50, 50),
    (N'PGD_26_05', N'BB002', 0, 120, 120),
    (N'PGD_26_05', N'BB003', 0, 250, 250),
    (N'PGD_26_05', N'BB004', 0, 80, 80),
    (N'PGD_26_05', N'BB005', 0, 190, 190),
    (N'PGD_26_05', N'BB006', 0, 300, 300),
    (N'PGD_26_05', N'BB007', 0, 45, 45),
    (N'PGD_26_05', N'BB008', 0, 100, 100),
    (N'PGD_26_05', N'BB009', 0, 350, 350),
    (N'PGD_26_05', N'BB010', 0, 150, 150);
GO


INSERT INTO HOADONTIENDIEN (MaHoaDon, TongTien, MaPhieuGhi, MaBienBan) VALUES
    (N'HD001', 50000.00, N'PGD_26_05', N'BB001'),   -- 50 kWh
    (N'HD002', 130000.00, N'PGD_26_05', N'BB002'),  -- 120 kWh
    (N'HD003', 350000.00, N'PGD_26_05', N'BB003'),  -- 250 kWh
    (N'HD004', 80000.00, N'PGD_26_05', N'BB004'),   -- 80 kWh
    (N'HD005', 235000.00, N'PGD_26_05', N'BB005'),  -- 190 kWh
    (N'HD006', 450000.00, N'PGD_26_05', N'BB006'),  -- 300 kWh
    (N'HD007', 45000.00, N'PGD_26_05', N'BB007'),   -- 45 kWh
    (N'HD008', 100000.00, N'PGD_26_05', N'BB008'),  -- 100 kWh
    (N'HD009', 550000.00, N'PGD_26_05', N'BB009'),  -- 350 kWh
    (N'HD010', 175000.00, N'PGD_26_05', N'BB010');  -- 150 kWh
GO

INSERT INTO CHITIETHOADON (MaHoaDon, Bac, DonGia, DinhMuc, DienNangTieuThu, ThanhTien) VALUES
    -- HD001 (50 kWh)
    (N'HD001', 1, 1000, 100, 50, 50000),
    
    -- HD002 (120 kWh)
    (N'HD002', 1, 1000, 100, 100, 100000), (N'HD002', 2, 1500, 100, 20, 30000), 
    
    -- HD003 (250 kWh)
    (N'HD003', 1, 1000, 100, 100, 100000), (N'HD003', 2, 1500, 100, 100, 150000), 
    (N'HD003', 3, 2000, NULL, 50, 100000),
    
    -- HD004 (80 kWh)
    (N'HD004', 1, 1000, 100, 80, 80000),
    
    -- HD005 (190 kWh)
    (N'HD005', 1, 1000, 100, 100, 100000), (N'HD005', 2, 1500, 100, 90, 135000), 
    
    -- HD006 (300 kWh)
    (N'HD006', 1, 1000, 100, 100, 100000), (N'HD006', 2, 1500, 100, 100, 150000), 
    (N'HD006', 3, 2000, NULL, 100, 200000),
    
    -- HD007 (45 kWh)
    (N'HD007', 1, 1000, 100, 45, 45000),
    
    -- HD008 (100 kWh)
    (N'HD008', 1, 1000, 100, 100, 100000), 
    
    -- HD009 (350 kWh)
    (N'HD009', 1, 1000, 100, 100, 100000), (N'HD009', 2, 1500, 100, 100, 150000), 
    (N'HD009', 3, 2000, NULL, 150, 300000),
    
    -- HD010 (150 kWh)
    (N'HD010', 1, 1000, 100, 100, 100000), (N'HD010', 2, 1500, 100, 50, 75000); 
GO

INSERT INTO PHIEUGHIDIEN (MaPhieuGhi, KyGhiChiSo, NgayGhi, NhanVienGhi) VALUES
-- NĂM 2021
(N'PGD_21_01', '01/2021', '2021-01-28', N'Trần Ghi Điện'), (N'PGD_21_02', '02/2021', '2021-02-28', N'Trần Ghi Điện'), 
(N'PGD_21_03', '03/2021', '2021-03-28', N'Trần Ghi Điện'), (N'PGD_21_04', '04/2021', '2021-04-28', N'Trần Ghi Điện'),
(N'PGD_21_05', '05/2021', '2021-05-28', N'Trần Ghi Điện'), (N'PGD_21_06', '06/2021', '2021-06-28', N'Trần Ghi Điện'), 
(N'PGD_21_07', '07/2021', '2021-07-28', N'Trần Ghi Điện'), (N'PGD_21_08', '08/2021', '2021-08-28', N'Trần Ghi Điện'),
(N'PGD_21_09', '09/2021', '2021-09-28', N'Trần Ghi Điện'), (N'PGD_21_10', '10/2021', '2021-10-28', N'Trần Ghi Điện'), 
(N'PGD_21_11', '11/2021', '2021-11-28', N'Trần Ghi Điện'), (N'PGD_21_12', '12/2021', '2021-12-28', N'Trần Ghi Điện'),

-- NĂM 2022
(N'PGD_22_01', '01/2022', '2022-01-28', N'Trần Ghi Điện'), (N'PGD_22_02', '02/2022', '2022-02-28', N'Trần Ghi Điện'), 
(N'PGD_22_03', '03/2022', '2022-03-28', N'Trần Ghi Điện'), (N'PGD_22_04', '04/2022', '2022-04-28', N'Trần Ghi Điện'),
(N'PGD_22_05', '05/2022', '2022-05-28', N'Trần Ghi Điện'), (N'PGD_22_06', '06/2022', '2022-06-28', N'Trần Ghi Điện'), 
(N'PGD_22_07', '07/2022', '2022-07-28', N'Trần Ghi Điện'), (N'PGD_22_08', '08/2022', '2022-08-28', N'Trần Ghi Điện'),
(N'PGD_22_09', '09/2022', '2022-09-28', N'Trần Ghi Điện'), (N'PGD_22_10', '10/2022', '2022-10-28', N'Trần Ghi Điện'), 
(N'PGD_22_11', '11/2022', '2022-11-28', N'Trần Ghi Điện'), (N'PGD_22_12', '12/2022', '2022-12-28', N'Trần Ghi Điện'),

-- NĂM 2023
(N'PGD_23_01', '01/2023', '2023-01-28', N'Trần Ghi Điện'), (N'PGD_23_02', '02/2023', '2023-02-28', N'Trần Ghi Điện'), 
(N'PGD_23_03', '03/2023', '2023-03-28', N'Trần Ghi Điện'), (N'PGD_23_04', '04/2023', '2023-04-28', N'Trần Ghi Điện'),
(N'PGD_23_05', '05/2023', '2023-05-28', N'Trần Ghi Điện'), (N'PGD_23_06', '06/2023', '2023-06-28', N'Trần Ghi Điện'), 
(N'PGD_23_07', '07/2023', '2023-07-28', N'Trần Ghi Điện'), (N'PGD_23_08', '08/2023', '2023-08-28', N'Trần Ghi Điện'),
(N'PGD_23_09', '09/2023', '2023-09-28', N'Trần Ghi Điện'), (N'PGD_23_10', '10/2023', '2023-10-28', N'Trần Ghi Điện'), 
(N'PGD_23_11', '11/2023', '2023-11-28', N'Trần Ghi Điện'), (N'PGD_23_12', '12/2023', '2023-12-28', N'Trần Ghi Điện'),

-- NĂM 2024
(N'PGD_24_01', '01/2024', '2024-01-28', N'Trần Ghi Điện'), (N'PGD_24_02', '02/2024', '2024-02-28', N'Trần Ghi Điện'), 
(N'PGD_24_03', '03/2024', '2024-03-28', N'Trần Ghi Điện'), (N'PGD_24_04', '04/2024', '2024-04-28', N'Trần Ghi Điện'),
(N'PGD_24_05', '05/2024', '2024-05-28', N'Trần Ghi Điện'), (N'PGD_24_06', '06/2024', '2024-06-28', N'Trần Ghi Điện'), 
(N'PGD_24_07', '07/2024', '2024-07-28', N'Trần Ghi Điện'), (N'PGD_24_08', '08/2024', '2024-08-28', N'Trần Ghi Điện'),
(N'PGD_24_09', '09/2024', '2024-09-28', N'Trần Ghi Điện'), (N'PGD_24_10', '10/2024', '2024-10-28', N'Trần Ghi Điện'), 
(N'PGD_24_11', '11/2024', '2024-11-28', N'Trần Ghi Điện'), (N'PGD_24_12', '12/2024', '2024-12-28', N'Trần Ghi Điện'),

-- NĂM 2025
(N'PGD_25_01', '01/2025', '2025-01-28', N'Trần Ghi Điện'), (N'PGD_25_02', '02/2025', '2025-02-28', N'Trần Ghi Điện'), 
(N'PGD_25_03', '03/2025', '2025-03-28', N'Trần Ghi Điện'), (N'PGD_25_04', '04/2025', '2025-04-28', N'Trần Ghi Điện'),
(N'PGD_25_05', '05/2025', '2025-05-28', N'Trần Ghi Điện'), (N'PGD_25_06', '06/2025', '2025-06-28', N'Trần Ghi Điện'), 
(N'PGD_25_07', '07/2025', '2025-07-28', N'Trần Ghi Điện'), (N'PGD_25_08', '08/2025', '2025-08-28', N'Trần Ghi Điện'),
(N'PGD_25_09', '09/2025', '2025-09-28', N'Trần Ghi Điện'), (N'PGD_25_10', '10/2025', '2025-10-28', N'Trần Ghi Điện'), 
(N'PGD_25_11', '11/2025', '2025-11-28', N'Trần Ghi Điện'), (N'PGD_25_12', '12/2025', '2025-12-28', N'Trần Ghi Điện'),

-- NĂM 2026 (TỚI THÁNG 4)
(N'PGD_26_01', '01/2026', '2026-01-28', N'Trần Ghi Điện'), (N'PGD_26_02', '02/2026', '2026-02-28', N'Trần Ghi Điện'),
(N'PGD_26_03', '03/2026', '2026-03-28', N'Trần Ghi Điện'), (N'PGD_26_04', '04/2026', '2026-04-28', N'Trần Ghi Điện');
GO

INSERT INTO CHITIETGHIDIEN (MaPhieuGhi, MaBienBan, ChiSoCu, ChiSoMoi, SanLuongTieuThu) VALUES
    -- Cuối năm 2023
    (N'PGD_23_12', N'BB001', 1000, 1150, 150),
    (N'PGD_23_12', N'BB002', 2000, 2250, 250),
    
    -- Cuối năm 2024
    (N'PGD_24_12', N'BB001', 2950, 3100, 150),
    (N'PGD_24_12', N'BB002', 5250, 5500, 250),
    
    -- Cuối năm 2025
    (N'PGD_25_12', N'BB001', 4900, 5050, 150),
    (N'PGD_25_12', N'BB002', 8500, 8750, 250),
    
    -- Tháng 01/2026
    (N'PGD_26_01', N'BB001', 5050, 5200, 150),
    (N'PGD_26_01', N'BB002', 8750, 9000, 250),
    
    -- Tháng 02/2026 (Tháng Tết, dùng ít)
    (N'PGD_26_02', N'BB001', 5200, 5330, 130),
    (N'PGD_26_02', N'BB002', 9000, 9220, 220),
    
    -- Tháng 03/2026 (Bắt đầu nóng lên)
    (N'PGD_26_03', N'BB001', 5330, 5490, 160),
    (N'PGD_26_03', N'BB002', 9220, 9500, 280),
    
    -- Tháng 04/2026 (Cao điểm mùa khô)
    (N'PGD_26_04', N'BB001', 5490, 5670, 180),
    (N'PGD_26_04', N'BB002', 9500, 9800, 300);
GO

INSERT INTO HOADONTIENDIEN (MaHoaDon, TongTien, MaPhieuGhi, MaBienBan) VALUES
    (N'HD_2312_BB01', 175000.00, N'PGD_23_12', N'BB001'), (N'HD_2312_BB02', 350000.00, N'PGD_23_12', N'BB002'),
    (N'HD_2412_BB01', 175000.00, N'PGD_24_12', N'BB001'), (N'HD_2412_BB02', 350000.00, N'PGD_24_12', N'BB002'),
    (N'HD_2512_BB01', 175000.00, N'PGD_25_12', N'BB001'), (N'HD_2512_BB02', 350000.00, N'PGD_25_12', N'BB002'),
    
    (N'HD_2601_BB01', 175000.00, N'PGD_26_01', N'BB001'), (N'HD_2601_BB02', 350000.00, N'PGD_26_01', N'BB002'),
    (N'HD_2602_BB01', 145000.00, N'PGD_26_02', N'BB001'), (N'HD_2602_BB02', 290000.00, N'PGD_26_02', N'BB002'),
    (N'HD_2603_BB01', 190000.00, N'PGD_26_03', N'BB001'), (N'HD_2603_BB02', 410000.00, N'PGD_26_03', N'BB002'),
    (N'HD_2604_BB01', 220000.00, N'PGD_26_04', N'BB001'), (N'HD_2604_BB02', 450000.00, N'PGD_26_04', N'BB002');
GO

INSERT INTO CHITIETHOADON (MaHoaDon, Bac, DonGia, DinhMuc, DienNangTieuThu, ThanhTien) VALUES
    -- CÁC HÓA ĐƠN 150 kWh (Tới bậc 2)
    (N'HD_2312_BB01', 1, 1000, 100, 100, 100000), (N'HD_2312_BB01', 2, 1500, 100, 50, 75000),
    (N'HD_2412_BB01', 1, 1000, 100, 100, 100000), (N'HD_2412_BB01', 2, 1500, 100, 50, 75000),
    (N'HD_2512_BB01', 1, 1000, 100, 100, 100000), (N'HD_2512_BB01', 2, 1500, 100, 50, 75000),
    (N'HD_2601_BB01', 1, 1000, 100, 100, 100000), (N'HD_2601_BB01', 2, 1500, 100, 50, 75000),

    -- CÁC HÓA ĐƠN 250 kWh (Tới bậc 3)
    (N'HD_2312_BB02', 1, 1000, 100, 100, 100000), (N'HD_2312_BB02', 2, 1500, 100, 100, 150000), (N'HD_2312_BB02', 3, 2000, NULL, 50, 100000),
    (N'HD_2412_BB02', 1, 1000, 100, 100, 100000), (N'HD_2412_BB02', 2, 1500, 100, 100, 150000), (N'HD_2412_BB02', 3, 2000, NULL, 50, 100000),
    (N'HD_2512_BB02', 1, 1000, 100, 100, 100000), (N'HD_2512_BB02', 2, 1500, 100, 100, 150000), (N'HD_2512_BB02', 3, 2000, NULL, 50, 100000),
    (N'HD_2601_BB02', 1, 1000, 100, 100, 100000), (N'HD_2601_BB02', 2, 1500, 100, 100, 150000), (N'HD_2601_BB02', 3, 2000, NULL, 50, 100000),

    -- HD_2602_BB01: 130 kWh
    (N'HD_2602_BB01', 1, 1000, 100, 100, 100000), (N'HD_2602_BB01', 2, 1500, 100, 30, 45000),
    -- HD_2602_BB02: 220 kWh
    (N'HD_2602_BB02', 1, 1000, 100, 100, 100000), (N'HD_2602_BB02', 2, 1500, 100, 100, 150000), (N'HD_2602_BB02', 3, 2000, NULL, 20, 40000),

    -- HD_2603_BB01: 160 kWh
    (N'HD_2603_BB01', 1, 1000, 100, 100, 100000), (N'HD_2603_BB01', 2, 1500, 100, 60, 90000),
    -- HD_2603_BB02: 280 kWh
    (N'HD_2603_BB02', 1, 1000, 100, 100, 100000), (N'HD_2603_BB02', 2, 1500, 100, 100, 150000), (N'HD_2603_BB02', 3, 2000, NULL, 80, 160000),

    -- HD_2604_BB01: 180 kWh
    (N'HD_2604_BB01', 1, 1000, 100, 100, 100000), (N'HD_2604_BB01', 2, 1500, 100, 80, 120000),
    -- HD_2604_BB02: 300 kWh
    (N'HD_2604_BB02', 1, 1000, 100, 100, 100000), (N'HD_2604_BB02', 2, 1500, 100, 100, 150000), (N'HD_2604_BB02', 3, 2000, NULL, 100, 200000);
GO

INSERT INTO CHITIETGHIDIEN (MaPhieuGhi, MaBienBan, ChiSoCu, ChiSoMoi, SanLuongTieuThu) VALUES
    (N'PGD_23_01', N'BB001', 0, 70, 70),       (N'PGD_23_01', N'BB002', 0, 140, 140),
    (N'PGD_23_02', N'BB001', 70, 130, 60),     (N'PGD_23_02', N'BB002', 140, 260, 120),
    (N'PGD_23_03', N'BB001', 130, 210, 80),    (N'PGD_23_03', N'BB002', 260, 420, 160),
    (N'PGD_23_04', N'BB001', 210, 300, 90),    (N'PGD_23_04', N'BB002', 420, 600, 180),
    (N'PGD_23_05', N'BB001', 300, 410, 110),   (N'PGD_23_05', N'BB002', 600, 820, 220),
    (N'PGD_23_06', N'BB001', 410, 530, 120),   (N'PGD_23_06', N'BB002', 820, 1060, 240),
    (N'PGD_23_07', N'BB001', 530, 650, 120),   (N'PGD_23_07', N'BB002', 1060, 1300, 240),
    (N'PGD_23_08', N'BB001', 650, 750, 100),   (N'PGD_23_08', N'BB002', 1300, 1500, 200),
    (N'PGD_23_09', N'BB001', 750, 840, 90),    (N'PGD_23_09', N'BB002', 1500, 1680, 180),
    (N'PGD_23_10', N'BB001', 840, 920, 80),    (N'PGD_23_10', N'BB002', 1680, 1840, 160),
    (N'PGD_23_11', N'BB001', 920, 1000, 80),   (N'PGD_23_11', N'BB002', 1840, 2000, 160);
GO

INSERT INTO CHITIETGHIDIEN (MaPhieuGhi, MaBienBan, ChiSoCu, ChiSoMoi, SanLuongTieuThu) VALUES
    (N'PGD_24_01', N'BB001', 1150, 1290, 140), (N'PGD_24_01', N'BB002', 2250, 2490, 240),
    (N'PGD_24_02', N'BB001', 1290, 1420, 130), (N'PGD_24_02', N'BB002', 2490, 2710, 220),
    (N'PGD_24_03', N'BB001', 1420, 1570, 150), (N'PGD_24_03', N'BB002', 2710, 2960, 250),
    (N'PGD_24_04', N'BB001', 1570, 1740, 170), (N'PGD_24_04', N'BB002', 2960, 3240, 280),
    (N'PGD_24_05', N'BB001', 1740, 1930, 190), (N'PGD_24_05', N'BB002', 3240, 3550, 310),
    (N'PGD_24_06', N'BB001', 1930, 2140, 210), (N'PGD_24_06', N'BB002', 3550, 3890, 340),
    (N'PGD_24_07', N'BB001', 2140, 2340, 200), (N'PGD_24_07', N'BB002', 3890, 4210, 320),
    (N'PGD_24_08', N'BB001', 2340, 2520, 180), (N'PGD_24_08', N'BB002', 4210, 4500, 290),
    (N'PGD_24_09', N'BB001', 2520, 2680, 160), (N'PGD_24_09', N'BB002', 4500, 4770, 270),
    (N'PGD_24_10', N'BB001', 2680, 2820, 140), (N'PGD_24_10', N'BB002', 4770, 5020, 250),
    (N'PGD_24_11', N'BB001', 2820, 2950, 130), (N'PGD_24_11', N'BB002', 5020, 5250, 230);
GO

INSERT INTO CHITIETGHIDIEN (MaPhieuGhi, MaBienBan, ChiSoCu, ChiSoMoi, SanLuongTieuThu) VALUES
    (N'PGD_25_01', N'BB001', 3100, 3240, 140), (N'PGD_25_01', N'BB002', 5500, 5740, 240),
    (N'PGD_25_02', N'BB001', 3240, 3370, 130), (N'PGD_25_02', N'BB002', 5740, 5960, 220),
    (N'PGD_25_03', N'BB001', 3370, 3520, 150), (N'PGD_25_03', N'BB002', 5960, 6210, 250),
    (N'PGD_25_04', N'BB001', 3520, 3690, 170), (N'PGD_25_04', N'BB002', 6210, 6490, 280),
    (N'PGD_25_05', N'BB001', 3690, 3880, 190), (N'PGD_25_05', N'BB002', 6490, 6800, 310),
    (N'PGD_25_06', N'BB001', 3880, 4090, 210), (N'PGD_25_06', N'BB002', 6800, 7140, 340),
    (N'PGD_25_07', N'BB001', 4090, 4290, 200), (N'PGD_25_07', N'BB002', 7140, 7460, 320),
    (N'PGD_25_08', N'BB001', 4290, 4470, 180), (N'PGD_25_08', N'BB002', 7460, 7750, 290),
    (N'PGD_25_09', N'BB001', 4470, 4630, 160), (N'PGD_25_09', N'BB002', 7750, 8020, 270),
    (N'PGD_25_10', N'BB001', 4630, 4770, 140), (N'PGD_25_10', N'BB002', 8020, 8270, 250),
    (N'PGD_25_11', N'BB001', 4770, 4900, 130), (N'PGD_25_11', N'BB002', 8270, 8500, 230);
GO