using System;
using System.Collections.Generic;

namespace Quản_lý_công_tơ_điện.Models;

public partial class Phieughidien
{
    public string MaPhieuGhi { get; set; } = null!;

    public string KyGhiChiSo { get; set; } = null!;

    public DateTime NgayGhi { get; set; }

    public string NhanVienGhi { get; set; } = null!;

    public virtual ICollection<Chitietghidien> Chitietghidiens { get; set; } = new List<Chitietghidien>();
}
