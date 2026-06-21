using System;
using System.Collections.Generic;

namespace Quản_lý_công_tơ_điện.Models;

public partial class Loaicongto
{
    public string MaLoaiCongTo { get; set; } = null!;

    public string TenLoaiCongTo { get; set; } = null!;

    public virtual ICollection<Bienbancapdien> Bienbancapdiens { get; set; } = new List<Bienbancapdien>();

    public virtual ICollection<Cauhinhcongto> Cauhinhcongtos { get; set; } = new List<Cauhinhcongto>();
}
