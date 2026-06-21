namespace Quản_lý_công_tơ_điện.UIModels
{
    public class ChiTietHoaDonRow
    {
        public int BacSo { get; set; }
        public string BacText => $"Bậc {BacSo}";
        public decimal DonGia { get; set; }
        public string DinhMucThucTeText { get; set; }
        public int? DinhMucDb { get; set; }
        public int DienNangTieuThu { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
