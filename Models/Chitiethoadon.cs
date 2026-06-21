using System;
using System.Collections.Generic;

namespace Quản_lý_công_tơ_điện.Models;

public partial class Chitiethoadon
{
    public string MaHoaDon { get; set; } = null!;

    public int Bac { get; set; }

    public decimal DonGia { get; set; }

    public int? DinhMuc { get; set; }

    public int DienNangTieuThu { get; set; }

    public decimal ThanhTien { get; set; }

    public virtual Hoadontiendien MaHoaDonNavigation { get; set; } = null!;
}
