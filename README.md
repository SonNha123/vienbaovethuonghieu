# Cổng thông tin Truyền thông & PR Doanh nghiệp - Viện Phát triển Thương hiệu

Dự án này là cổng thông tin điện tử cao cấp, được thiết kế theo cấu trúc trang tin `baovethuonghieu.org` cũ, sao chép 100% giao diện gốc và được phân tách thành **hai ứng dụng ASP.NET Core độc lập** để tối đa hóa bảo mật và hiệu năng.

---

## 🌟 Kiến trúc Tách biệt 2 Port (Multi-Port Architecture)
Hệ thống được chia làm hai phân hệ chạy trên hai cổng (Port) riêng biệt, kết nối chung tới cơ sở dữ liệu **Microsoft SQL Server 2022**:

1. **`PortalApp` (Trang tin tức công cộng - Chạy Port 5000)**:
   - **Giao diện gốc 100%**: Sao chép hoàn hảo giao diện tờ báo truyền thống gốc (Menu xanh đậm `#133A7C`, Chân trang xanh trời `#29BDF4`, 2 bên lề có banner quảng cáo đứng chạy dọc màn hình khóa khung content ở giữa).
   - **Tối đa hóa bảo mật**: Không chứa bất kỳ nút đăng nhập, form đăng nhập hay URL quản trị nào. Trang web chỉ đọc dữ liệu từ SQL Server, ngăn chặn hoàn toàn nguy cơ tấn công chiếm quyền admin.
2. **`AdminApp` (CMS quản trị bảo mật - Chạy Port 5001)**:
   - **Phân khu riêng biệt**: Chỉ chạy trên cổng 5001 bảo mật (hoặc trỏ subdomain riêng `admin.baovethuonghieu.org`).
   - **Tính năng đầy đủ**: Chứa màn hình đăng nhập bảo mật, tính năng phân quyền (SuperAdmin, Editor, Partner) và đầy đủ trang CRUD để quản lý Banner quảng cáo, Video PR, Logo doanh nghiệp, kiểm duyệt bài viết nháp.
   - **Đồng bộ hóa ảnh tức thì**: Khi Admin upload ảnh ở Port 5001, hệ thống tự động copy tệp tin sang thư mục `wwwroot/uploads` của `PortalApp` (Port 5000) giúp hiển thị ngay lập tức.

---

## 🛠 Hướng dẫn chạy thử nghiệm cục bộ (Local Run)

Hệ thống đã được thiết kế cơ chế **Tự động tạo Cơ sở dữ liệu mới (EnsureCreated)** khi khởi động, giúp bạn chạy thử nghiệm dễ dàng mà không cần thao tác thủ công:

### Bước 1: Cấu hình kết nối SQL Server
Mở file `appsettings.json` trong **cả hai thư mục** `PortalApp/` và `AdminApp/` để điền thông tin SQL Server mới của bạn:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_IP;Database=KDTM_BaoVeThuongHieu_New;User ID=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
}
```
 
### Bước 2: Khởi chạy dự án
Mở hai cửa sổ Terminal/PowerShell riêng biệt để chạy đồng thời cả hai ứng dụng:
 
1. **Khởi chạy Trang tin Portal (Port 5000)**:
   ```bash
   cd PortalApp
   dotnet run
   ```
2. **Khởi chạy Trang quản trị CMS (Port 5001)**:
   ```bash
   cd AdminApp
   dotnet run
   ```
 
*Khi ứng dụng khởi động, database mang tên `KDTM_BaoVeThuongHieu_New` sẽ tự động được tạo trên SQL Server của bạn cùng 7 danh mục tin gốc và tài khoản admin mặc định.*
 
---
 
## 🔐 Thông tin tài khoản mặc định
- **Đường dẫn đăng nhập CMS**: Truy cập `http://localhost:5001/Auth/Login`
- **Tên đăng nhập**: `admin`
- **Mật khẩu**: `YOUR_ADMIN_PASSWORD`
 
---
 
## 🚀 Hướng dẫn Triển khai lên VPS Windows & IIS
 
Để đưa ứng dụng lên máy chủ chạy chính thức, hãy thực hiện các bước sau:
 
### Bước 1: Chuẩn bị trên VPS Windows
1. Truy cập Remote Desktop (RDP) vào VPS Windows của bạn bằng tài khoản quản trị.
2. Đảm bảo VPS đã cài đặt:
   - **IIS (Internet Information Services)**
   - **.NET 8.0 Hosting Bundle**

### Bước 2: Xuất bản dự án (Publish)
Tại máy tính cục bộ của bạn, thực hiện biên dịch và xuất bản cả hai dự án:
```bash
# Xuất bản PortalApp
cd PortalApp
dotnet publish -c Release -o ../publish/portal

# Xuất bản AdminApp
cd ../AdminApp
dotnet publish -c Release -o ../publish/admin
```

### Bước 3: Đưa tệp lên VPS & Cấu hình IIS
1. Sao chép thư mục `publish/portal` lên VPS (Ví dụ: `C:\inetpub\wwwroot\portal`).
2. Sao chép thư mục `publish/admin` lên VPS (Ví dụ: `C:\inetpub\wwwroot\admin`).
3. Mở **IIS Manager** trên VPS:
   - **Tạo Site 1 (Portal)**: Nhấp chuột phải vào *Sites* -> *Add Website*. Tên Site: `BaoVeThuongHieu_Portal`, Physical path: `C:\inetpub\wwwroot\portal`, Binding Port: **80** (Tên miền `baovethuonghieu.org`).
   - **Tạo Site 2 (Admin)**: Nhấp chuột phải vào *Sites* -> *Add Website*. Tên Site: `BaoVeThuongHieu_Admin`, Physical path: `C:\inetpub\wwwroot\admin`, Binding Port: **8080** hoặc **5001** (Hoặc dùng tên miền phụ `admin.baovethuonghieu.org` trên Port 80).
4. Cấu hình **Application Pool**:
   - Chọn *Application Pools* bên cột trái.
   - Nhấp đúp vào Pool `BaoVeThuongHieu_Portal` và `BaoVeThuongHieu_Admin`.
   - Chọn **.NET CLR Version** thành **No Managed Code** (Bắt buộc đối với ứng dụng .NET Core).
5. Phân quyền thư mục:
   - Đảm bảo thư mục `wwwroot/uploads` trong cả hai thư mục `portal` và `admin` được cấp quyền **Ghi (Write)** cho user `IIS_IUSRS` để tính năng upload ảnh banner, video, logo hoạt động trơn tru.
