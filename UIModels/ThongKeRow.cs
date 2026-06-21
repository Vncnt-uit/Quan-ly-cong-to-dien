    public class ThongKeRowViewModel
    {
        public int STT { get; set; }
        public string ThangText => $"Tháng {Thang}";
        public int Thang { get; set; }
        public int SanLuong { get; set; }
        public string SoVoiThangTruoc { get; set; }
    }
