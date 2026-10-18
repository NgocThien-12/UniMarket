Contributing to UniMarket
Cảm ơn bạn đã quan tâm và muốn đóng góp cho UniMarket — website mua bán đồ cũ dành cho sinh viên. Tài liệu này hướng dẫn cách chuẩn bị môi trường, tạo nhánh, kiểm tra thay đổi và gửi Pull Request.

1. Trước khi bắt đầu

- Đọc README để hiểu mục tiêu dự án và cách chạy ứng dụng.
- Kiểm tra các issue hiện có để tránh làm trùng công việc.
- Với thay đổi lớn, hãy mở issue hoặc trao đổi với người bảo trì trước khi triển khai.
- Chỉ gửi những thay đổi phù hợp với phạm vi dự án và có thể giải thích rõ ràng.

2. Chuẩn bị môi trường
   Cài đặt các công cụ cần thiết:

- .NET SDK tương thích với phiên bản được khai báo trong UniMarket.csproj.
- SQL Server, SQL Server Express hoặc LocalDB nếu tính năng cần truy cập cơ sở dữ liệu.
- Git.
- Visual Studio hoặc Visual Studio Code.
  Clone repository và chuyển vào thư mục dự án:
  git clone <URL-repository>
  cd UniMarket_1
  Thay <URL-repository> bằng URL GitHub thực tế của repository.

3. Tạo nhánh làm việc
   Không nên thực hiện thay đổi trực tiếp trên nhánh main. Hãy tạo nhánh riêng:
   git checkout main
   git pull
   git checkout -b feature/ten-chuc-nang
   Quy ước tên nhánh gợi ý:

- feature/ten-chuc-nang — thêm chức năng.
- fix/mo-ta-loi — sửa lỗi.
- docs/noi-dung — cập nhật tài liệu.
- refactor/noi-dung — cải thiện cấu trúc code mà không thay đổi hành vi mong muốn.
  Dùng tên ngắn gọn, mô tả đúng nội dung thay đổi.

4. Quy tắc viết mã

- Ưu tiên đặt tên class, method và biến rõ nghĩa, nhất quán với quy ước hiện có của dự án.
- Giữ cấu trúc ASP.NET Core MVC hiện tại; đặt controller, model, view và service đúng vị trí phù hợp.
- Kiểm tra dữ liệu đầu vào ở phía máy chủ; không chỉ dựa vào kiểm tra ở giao diện.
- Với chức năng cần đăng nhập, hãy kiểm tra xác thực, phân quyền và quyền sở hữu dữ liệu.
- Không đưa mật khẩu, khóa bí mật, dữ liệu cá nhân thật hoặc chuỗi kết nối chứa thông tin xác thực vào mã nguồn.
- Không thay đổi cấu trúc database hoặc migration nếu không cần thiết; nếu có thay đổi, hãy mô tả rõ trong Pull Request.
- Giữ giao diện tiếng Việt nhất quán với website hiện tại.

5. Kiểm tra trước khi gửi
   Tại thư mục chứa file dự án, chạy:
   dotnet restore
   dotnet build
   dotnet test
   Nếu repository chưa có dự án kiểm thử, lệnh dotnet test có thể không chạy được hoặc không tìm thấy bài kiểm thử. Trong trường hợp đó, hãy ghi rõ các bước kiểm tra thủ công đã thực hiện.
   Nếu thay đổi liên quan đến database, hãy kiểm tra trên môi trường phát triển và xác nhận ứng dụng hoạt động với cấu trúc dữ liệu mới.
6. Commit thay đổi
   Hãy tạo các commit nhỏ, tập trung vào một mục đích và viết thông điệp dễ hiểu. Ví dụ:
   git add .
   git commit -m "feat: add product filtering"
   Tiền tố gợi ý:

- feat: thêm chức năng.
- fix: sửa lỗi.
- docs: cập nhật tài liệu.
- refactor: tái cấu trúc code.
- test: thêm hoặc sửa kiểm thử.
- chore: thay đổi cấu hình hoặc công việc bảo trì.

7. Push nhánh và tạo Pull Request
   git push -u origin feature/ten-chuc-nang
   Sau đó, mở repository trên GitHub và tạo Pull Request từ nhánh vừa push vào main.
   Pull Request nên có:

- Mục đích: vấn đề cần giải quyết.
- Thay đổi chính: các file hoặc chức năng đã cập nhật.
- Cách kiểm tra: lệnh đã chạy hoặc các bước kiểm thử thủ công.
- Ảnh chụp màn hình: đính kèm nếu có thay đổi giao diện.
- Lưu ý database/cấu hình: nêu rõ nếu cần cập nhật migration, script SQL hoặc appsettings.json.
  Không đưa mật khẩu, token, thông tin cá nhân hoặc dữ liệu nhạy cảm vào commit hay Pull Request.

8. Báo lỗi hoặc đề xuất chức năng
   Khi tạo issue, hãy cung cấp thông tin phù hợp:

- Mô tả ngắn gọn vấn đề hoặc đề xuất.
- Các bước tái hiện lỗi.
- Kết quả mong muốn và kết quả thực tế.
- Thông báo lỗi hoặc ảnh chụp màn hình nếu có.
- Môi trường chạy và phiên bản liên quan nếu cần.

9. Giấy phép
   Khi đóng góp, bạn đồng ý rằng phần đóng góp của mình được phát hành theo giấy phép của repository, nếu điều đó phù hợp với quyền sở hữu và các điều khoản áp dụng. Hãy đọc file LICENSE trong repository trước khi đóng góp.
   Cảm ơn bạn đã giúp UniMarket tốt hơn!
