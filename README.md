📚 Library Management System – Hệ thống Quản lý Thư viện

Đồ án môn học Nhập môn Công nghệ Phần mềm – SE104.P21
Trường Đại học Công nghệ Thông tin – Đại học Quốc gia TP. HCM

---

🚀 Giới thiệu

Hệ thống Quản lý Thư viện được xây dựng nhằm hỗ trợ các thư viện trong việc quản lý sách, độc giả và hoạt động mượn trả một cách hiệu quả.
Phần mềm giúp giảm thiểu công việc thủ công, tối ưu quy trình quản lý và mang lại trải nghiệm hiện đại, tiện lợi cho cả thủ thư và độc giả.

---

Mục tiêu chính:

Giảm tải công việc thủ công cho thủ thư.

Hỗ trợ người dùng mượn – trả – tra cứu sách nhanh chóng.

Đảm bảo tính bảo mật – tiện dụng – hiệu quả – tiến hóa.

Cung cấp báo cáo và thống kê chi tiết.

---

✨ Tính năng nổi bật
👩‍💼 Đối với Admin

Quản lý người dùng, phân quyền theo vai trò.

Quản lý quyền truy cập và chỉnh sửa chính sách, quy định.

📖 Đối với Thủ thư / Quản lý

Quản lý sách, tác giả, độc giả.

Tiếp nhận sách mới.

Xử lý mượn – trả – phạt.

Lập các loại báo cáo (mượn theo thể loại, trả trễ).

Thống kê trực quan bằng biểu đồ.

Trò chuyện với độc giả.

👨‍🎓 Đối với Độc giả

Đăng ký/đăng nhập/đổi mật khẩu bằng OTP & Google OAuth.

Tra cứu sách nhanh chóng.

Quản lý danh sách mượn, trả, phạt, sách yêu thích.

Đánh giá và bình luận sách.

Trò chuyện trực tiếp với thủ thư.

---

🏗️ Kiến trúc hệ thống

Ứng dụng phát triển theo mô hình Client – Server với kiến trúc WebApi (RESTful API).

Frontend: Giao diện người dùng (Web).

Backend: Xử lý logic nghiệp vụ, quản lý dữ liệu, xác thực JWT.

Database: Thiết kế chuẩn hóa với nhiều bảng (Readers, Books, Authors, LoanSlip, Penalty, Reports, Roles, Permissions, OTP, …).

---

🛠️ Công nghệ sử dụng

Ngôn ngữ: C#, .NET, Java (Backend), ReactJS (Frontend)

Cơ sở dữ liệu: PostgreSQL

Bảo mật: JWT Authentication, OAuth2 Google Login

Triển khai: RESTful API, mô hình phân quyền Role-Permission

---

📸 Hình ảnh giao diện

🔑 Màn hình Đăng nhập/Đăng ký/Quên mật khẩu

📊 Dashboard thống kê & báo cáo

📚 Danh sách sách, tác giả, độc giả

📝 Quản lý mượn – trả – phạt

💬 Trò chuyện giữa độc giả và thủ thư

---

🎯 Kết luận

Dự án Quản lý Thư viện đã đáp ứng được các yêu cầu cơ bản của một hệ thống quản lý thư viện hiện đại: trực quan, dễ sử dụng, bảo mật, và dễ dàng mở rộng.
Trong tương lai, hệ thống có thể phát triển thêm:

Ứng dụng mobile native.

Tích hợp AI gợi ý sách cho độc giả.

Kết nối liên thông nhiều thư viện khác nhau.
