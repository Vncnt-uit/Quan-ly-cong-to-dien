Execute file sql để tạo Database trước.
Vào file App.config, ở dòng connectionString="Server=.;Database=QuanLyCapDien;Trusted_Connection=True;TrustServerCertificate=True;" />
Sửa "Server=." thành tên Server SQL của mọi người xong mới chạy và test được.

## Cập nhật quan trọng (Cần đọc kỹ)
SQL có vài thay đổi với giá trị trong bảng lên kiểm tra từng bảng, nếu có dòng alter table nào chưa chạy thì chạy với lại có vài bảng có thể mình làm khác nhau nên là xóa CSDL chạy lại toàn bộ cho chắc ăn.

Chạy 2 lệnh này để hard reset lại local của ông theo cái repo tại tui phải refactor hết để nó ra MVVM
```bash
- git fetch origin
- git reset --hard origin/main
