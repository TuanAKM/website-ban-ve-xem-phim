# Website bán vé xem phim

Hệ thống quản lý bán vé xem phim dành cho những rạp có quy mô vừa và nhỏ

## Công nghệ sử dụng

* **Backend:** ASP.NET Core 9.0 MVC
* **Database:** SQL Server
* **Bảo mật:** ASP.NET Core Identity
* **Realtime:** SignalR
* **Frontend:** Bootstrap 5, JavaScript

## Hướng dẫn chạy

1. **Clone repo về máy**:
    ```bash
    git clone https://github.com/TuanAKM/website-ban-ve-xem-phim.git
    ```

2. **Cấu hình Database**:
    * Sửa chuỗi kết nối `DefaultConnection` trong file `appsettings.json`

3. **Chạy Migration để khởi tạo Database**:
    ```bash
    dotnet ef database update
    ```

4. **Chạy server**:
    ```bash
    dotnet run
    ```

## Tài khoản

### Quản trị viên(Admin)
* **Tài khoản:** admin
* **Mật khẩu:** Admin@123

### Nhân viên
* **Tài khoản:** staff
* **Tài khoản:** Staff@123
