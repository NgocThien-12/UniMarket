UniMarket – Website mua bán đồ cũ dành cho sinh viên
UniMarket là dự án website mua bán đồ đã qua sử dụng hướng đến sinh viên. Website được định hướng để người dùng tìm kiếm sản phẩm, xem thông tin chi tiết, đăng bán đồ không còn nhu cầu sử dụng và quản lý hoạt động mua bán.
Lưu ý: Các chức năng, đường dẫn và tài khoản mẫu bên dưới cần được đối chiếu với mã nguồn thực tế của phiên bản đang chạy. Chỉ giữ lại những nội dung đã được triển khai và kiểm thử.

1. Công nghệ sử dụng

- Ngôn ngữ và nền tảng: C#, .NET 9, ASP.NET Core MVC.
- Truy cập dữ liệu: Entity Framework Core.
- Cơ sở dữ liệu: Microsoft SQL Server.
- Đăng nhập và phân quyền: ASP.NET Core Identity; các vai trò như Admin và User nếu đã được cấu hình.
- Giao diện: Razor Views (.cshtml), HTML, CSS, JavaScript và Bootstrap.
- Kiểm thử: xUnit và EF Core InMemory nếu đã được thiết lập trong dự án.

2. Chức năng chính
   2.1. Người mua

- Xem danh sách sản phẩm và thông tin chi tiết.
- Tìm kiếm, lọc sản phẩm theo các tiêu chí website hỗ trợ.
- Thêm sản phẩm vào giỏ hàng và cập nhật số lượng.
- Nhập thông tin nhận hàng và đặt hàng nếu luồng đặt hàng đã được cấu hình.
- Xem lịch sử đơn hàng và trạng thái xử lý nếu chức năng đã được triển khai.
- Lưu sản phẩm yêu thích nếu chức năng này đã được bật.
  2.2. Người bán
- Đăng tin bán sản phẩm đã qua sử dụng.
- Quản lý tin đăng của mình theo các chức năng đã triển khai.
- Cung cấp thông tin sản phẩm như tên, mô tả, giá, tình trạng và hình ảnh.
  2.3. Quản trị viên
  Nếu phân hệ Admin và phân quyền đã được cấu hình, quản trị viên có thể quản lý sản phẩm, tài khoản, danh mục và đơn hàng. Quyền truy cập cần được kiểm tra ở phía máy chủ, không chỉ ẩn nút trên giao diện.

3. Cấu trúc thư mục tham khảo
   Tên thư mục và tệp thực tế có thể khác tùy phiên bản mã nguồn.
   UniMarket_1/
   ├── Areas/
   │ └── Admin/ # Phân hệ quản trị, nếu có
   ├── Controllers/ # Điều khiển các yêu cầu của website
   ├── Data/ # DbContext và khởi tạo dữ liệu, nếu có
   ├── Models/ # Các lớp mô hình dữ liệu
   ├── Services/ # Xử lý nghiệp vụ, nếu có
   ├── ViewModels/ # Dữ liệu dùng cho giao diện, nếu có
   ├── Views/ # Giao diện Razor
   ├── wwwroot/ # CSS, JavaScript, hình ảnh và tài nguyên tĩnh
   ├── UniMarket.Tests/ # Dự án kiểm thử, nếu có
   ├── appsettings.json # Cấu hình ứng dụng
   ├── Program.cs # Đăng ký dịch vụ và cấu hình ứng dụng
   └── UniMarket.csproj # Tệp dự án .NET
4. Yêu cầu môi trường

- .NET SDK tương thích với phiên bản khai báo trong UniMarket.csproj (README gốc mô tả .NET 9).
- Microsoft SQL Server, SQL Server Express hoặc LocalDB.
- Visual Studio 2022 có workload ASP.NET and web development, hoặc Visual Studio Code cùng công cụ .NET cần thiết.
- Công cụ dòng lệnh dotnet.

5. Cài đặt và chạy dự án
   Bước 1: Lấy mã nguồn
   Nếu dự án đã được đẩy lên GitHub, thay <URL-repository> bằng URL repository thực tế:
   git clone <URL-repository>
   cd UniMarket_1
   Nếu đã có mã nguồn trên máy, mở Terminal tại thư mục chứa UniMarket.csproj.
   Bước 2: Cấu hình chuỗi kết nối
   Mở appsettings.json và kiểm tra phần ConnectionStrings. Ví dụ cho SQL Server Express:

{
"ConnectionStrings": {
"DefaultConnection": "Server=YOUR_SQL_SERVER;Database=UniMarketDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
}

Thay Server bằng tên SQL Server/instance thực tế trên máy. Nếu dùng LocalDB, hãy dùng chuỗi kết nối tương ứng với (localdb)\\MSSQLLocalDB.
Không đưa mật khẩu thật, thông tin kết nối riêng tư hoặc khóa bí mật vào repository công khai. Với thông tin nhạy cảm, hãy dùng User Secrets hoặc biến môi trường.

Bước 3: Tạo hoặc cập nhật cơ sở dữ liệu
Cách tạo database phụ thuộc vào dự án đang dùng EF Core Migrations, script SQL hay cơ chế khởi tạo riêng.

- Nếu dự án dùng migrations, kiểm tra các migration hiện có rồi chạy lệnh phù hợp, ví dụ:
  dotnet ef database update
- Nếu dự án có file .sql, mở file bằng SQL Server Management Studio (SSMS) hoặc công cụ SQL phù hợp; kiểm tra tên database và bảng trước khi thực thi.
- Nếu dự án có lớp khởi tạo dữ liệu, kiểm tra Program.cs và lớp đó để biết ứng dụng có tự tạo dữ liệu hay không.
  Không chạy nhiều cách khởi tạo trên một database hiện có khi chưa kiểm tra, vì có thể gây lỗi trùng bảng hoặc sai cấu trúc.
  Bước 4: Khôi phục package, build và chạy
  Tại thư mục chứa UniMarket.csproj, chạy:
  dotnet restore
  dotnet build
  dotnet run
  Mở URL được hiển thị trong Terminal. Địa chỉ có thể khác nhau tùy cấu hình chạy.

6. Tài khoản thử nghiệm
   Chỉ sử dụng tài khoản mẫu nếu chúng đã được tạo trong database hoặc được khởi tạo bởi mã nguồn hiện tại. Không nên công bố mật khẩu mặc định trong repository công khai nếu tài khoản đó có quyền quản trị.
   Nếu chưa có tài khoản mẫu, hãy đăng ký qua website (nếu chức năng đăng ký hoạt động) hoặc kiểm tra lớp khởi tạo dữ liệu và cấu hình Identity để tạo tài khoản quản trị an toàn.
7. Chạy kiểm thử
   Nếu repository có dự án kiểm thử UniMarket.Tests, có thể chạy:
   dotnet test
   Kết quả thực tế sẽ hiển thị trong Terminal. Không nên ghi số lượng bài kiểm thử thành công cố định nếu chưa chạy lại trên phiên bản mã nguồn hiện tại.
8. Bảo mật và lưu ý phát triển

- Kiểm tra dữ liệu đầu vào ở phía máy chủ bằng validation phù hợp.
- Bảo vệ chức năng quản trị bằng xác thực và phân quyền phía máy chủ.
- Dùng cơ chế chống CSRF cho các biểu mẫu thay đổi dữ liệu khi phù hợp.
- Không lưu mật khẩu dạng văn bản thuần; sử dụng ASP.NET Core Identity hoặc cơ chế băm mật khẩu an toàn.
- Không đưa thông tin bí mật, dữ liệu cá nhân thật hoặc chuỗi kết nối chứa thông tin xác thực lên GitHub.
- Kiểm tra quyền sở hữu trước khi cho phép người dùng sửa/xóa tin đăng, giỏ hàng hoặc đơn hàng.

9. Đóng góp
   Tạo branch riêng cho thay đổi, kiểm tra bằng dotnet build và dotnet test (nếu có), sau đó mở Pull Request. Nếu repository có hướng dẫn riêng, hãy xem thêm CONTRIBUTING.md.
10. Giấy phép
    Dự án sử dụng MIT License nếu repository có tệp LICENSE chứa đầy đủ nội dung giấy phép MIT. Xem LICENSE để biết điều khoản cụ thể.
    Chỉ áp dụng giấy phép cho phần mã nguồn mà bạn có quyền cấp phép. Thư viện,
