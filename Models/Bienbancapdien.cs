using System;
using System.Collections.Generic;

namespace Quản_lý_công_tơ_điện.Models;

public partial class Bienbancapdien
{
    public string MaBienBan { get; set; } = null!;

    public DateTime NgayLap { get; set; }

    public DateTime? ThoiGianBatDauCapDien { get; set; }

    public int? ChiSoBanDau { get; set; }

    public string? ViTriLapDat { get; set; }

    public string? MaCongTo { get; set; }

    public string? HangSanXuat { get; set; }

    public int? NamSanXuat { get; set; }

    public string MaPhieu { get; set; } = null!;

    public string MaLoaiCongTo { get; set; } = null!;

    public virtual ICollection<Chitietghidien> Chitietghidiens { get; set; } = new List<Chitietghidien>();

    public virtual Loaicongto MaLoaiCongToNavigation { get; set; } = null!;

    public virtual Phieucapdien MaPhieuNavigation { get; set; } = null!;
}
