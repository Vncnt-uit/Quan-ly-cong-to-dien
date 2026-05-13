using System;
using System.Collections.Generic;

namespace Quản_lý_công_tơ_điện.Models;

public partial class Mucdich
{
    public string MaMucDich { get; set; } = null!;

    public string TenMucDich { get; set; } = null!;

    public virtual ICollection<Cauhinhcapdien> Cauhinhcapdiens { get; set; } = new List<Cauhinhcapdien>();
}
