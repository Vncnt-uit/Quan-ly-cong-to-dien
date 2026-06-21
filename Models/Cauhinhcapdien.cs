using System;
using System.Collections.Generic;

namespace Quản_lý_công_tơ_điện.Models;

public partial class Cauhinhcapdien
{
    public string MaMucDich { get; set; } = null!;

    public string MaSoPha { get; set; } = null!;

    public virtual Mucdich MaMucDichNavigation { get; set; } = null!;

    public virtual Loaipha MaSoPhaNavigation { get; set; } = null!;

    public virtual ICollection<Phieucapdien> Phieucapdiens { get; set; } = new List<Phieucapdien>();
}
