using System;
using System.Collections.Generic;

namespace Quản_lý_công_tơ_điện.Models;

public partial class Loaipha
{
    public string MaSoPha { get; set; } = null!;

    public string TenSoPha { get; set; } = null!;

    public virtual ICollection<Cauhinhcapdien> Cauhinhcapdiens { get; set; } = new List<Cauhinhcapdien>();

    public virtual ICollection<Cauhinhcongto> Cauhinhcongtos { get; set; } = new List<Cauhinhcongto>();
}
