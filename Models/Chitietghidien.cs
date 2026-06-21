using System;
using System.Collections.Generic;

namespace Quản_lý_công_tơ_điện.Models;

public partial class Chitietghidien
{
    public string MaPhieuGhi { get; set; } = null!;

    public string MaBienBan { get; set; } = null!;

    public int ChiSoCu { get; set; }

    public int ChiSoMoi { get; set; }

    public int SanLuongTieuThu { get; set; }

    public virtual ICollection<Hoadontiendien> Hoadontiendiens { get; set; } = new List<Hoadontiendien>();

    public virtual Bienbancapdien MaBienBanNavigation { get; set; } = null!;

    public virtual Phieughidien MaPhieuGhiNavigation { get; set; } = null!;
}
