using System;
using System.Collections.Generic;

namespace Quản_lý_công_tơ_điện.Models;

public partial class Cauhinhcongto
{
    public string MaSoPha { get; set; } = null!;

    public string MaLoaiCongTo { get; set; } = null!;
    public virtual Loaicongto MaLoaiCongToNavigation { get; set; } = null!;
    public virtual Loaipha MaSoPhaNavigation { get; set; } = null!;
}
