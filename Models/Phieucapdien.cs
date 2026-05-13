using System;
using System.Collections.Generic;

namespace Quản_lý_công_tơ_điện.Models;

public partial class Phieucapdien
{
    public string MaPhieu { get; set; } = null!;

    public DateTime ThoiGianGui { get; set; }

    public string HoTen { get; set; } = null!;

    public string Cccd { get; set; } = null!;

    public DateOnly? NamSinh { get; set; }

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public string DiaChi { get; set; } = null!;

    public string MaMucDich { get; set; } = null!;

    public string MaSoPha { get; set; } = null!;

    public virtual Cauhinhcapdien Cauhinhcapdien { get; set; } = null!;
}
