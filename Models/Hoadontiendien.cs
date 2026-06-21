using System;
using System.Collections.Generic;

namespace Quản_lý_công_tơ_điện.Models;

public partial class Hoadontiendien
{
    public string MaHoaDon { get; set; } = null!;

    public decimal TongTien { get; set; }

    public string MaPhieuGhi { get; set; } = null!;

    public string MaBienBan { get; set; } = null!;

    public virtual Chitietghidien Chitietghidien { get; set; } = null!;

    public virtual ICollection<Chitiethoadon> Chitiethoadons { get; set; } = new List<Chitiethoadon>();
}
