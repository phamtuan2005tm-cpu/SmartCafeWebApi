# 01. Overview & Requirements Specification

**Project Name:** Smart Cafe & POS Ordering System  
**Document Version:** 1.2 (Finalized Requirements with Interactive Customizer & AI Assistant)  
**Methodology:** Documentation-as-Code  

---

## 1. Executive Summary & Project Scope

**Smart Cafe & POS Ordering System** là hệ thống quản lý và đặt món thông minh dành cho quán cà phê hiện đại. Hệ thống kết hợp giữa trải nghiệm gọi món trực quan cho Khách hàng (`Customer`), công cụ vận hành hiệu quả cho Nhân viên (`Staff`), và trung tâm quản trị toàn diện cho Quản lý (`Admin`).

### Các điểm nhấn công nghệ cốt lõi:
* **Interactive Visual Customizer:** Giao diện tùy chỉnh món trực quan theo cơ chế chọn tầng (Visual Layering Toggle), áp dụng khung công thức chuẩn hóa của quán.
* **AI-Powered Cafe Assistant:** Trợ lý AI tích hợp có khả năng truy vấn dữ liệu thực đơn/tồn kho thời gian thực qua Backend API để tư vấn và giải đáp cho khách hàng.
* **Kitchen Display System (KDS):** Màn hình theo dõi và cập nhật trạng thái đơn hàng thời gian thực cho bộ phận pha chế.

---

## 2. Stakeholders & Roles (Actors)

Hệ thống phân định rõ 3 vai trò người dùng (Role-Based Access Control - RBAC) cùng cơ chế khởi tạo tài khoản riêng biệt:

| Actor | Mô tả | Cơ chế khởi tạo tài khoản |
| :--- | :--- | :--- |
| **Customer** | Khách hàng sử dụng dịch vụ xem menu, tùy chỉnh món, đặt hàng và trò chuyện với AI. | **Public Registration** (Tự đăng ký công khai). |
| **Staff** | Nhân viên phục vụ / thu ngân / pha chế theo dõi đơn hàng và cập nhật KDS. | **Admin-Created** (Do Admin tạo trong trang quản trị). |
| **Admin** | Quản trị viên hệ thống có toàn quyền quản lý thực đơn, nhân viên và báo cáo. | **Data Seeded** (Khởi tạo sẵn trong C# Seed Data). |

---

## 3. Functional Requirements (FRs) by Business Modules

### 3.1 Module 1: Authentication & Authorization (AUTH)
* **FR-AUTH-01 (Customer Registration):** Cho phép người dùng công khai đăng ký tài khoản `Customer` bằng Email và Mật khẩu.
* **FR-AUTH-02 (User Login):** Cho phép người dùng đăng nhập bằng Email và Mật khẩu. Hệ thống xác thực và trả về JWT Bearer Token chứa Role tương ứng.
* **FR-AUTH-03 (Role-Based Authorization):** Bảo vệ các API Endpoints dựa trên Role (`Customer`, `Staff`, `Admin`).
* **FR-AUTH-04 (Staff Account Management):** Cho phép `Admin` tạo, khóa (`IsActive`) hoặc cấp lại mật khẩu cho tài khoản `Staff`.

---

### 3.2 Module 2: Catalog & Menu Management (CAT)
* **FR-CAT-01 (Category CRUD):** Cho phép `Admin` Thêm, Sửa, Xóa và sắp xếp thứ tự hiển thị (`DisplayOrder`) của các danh mục sản phẩm (Coffee, Tea, Freeze, Bakery...).
* **FR-CAT-02 (Product CRUD & Pricing):** Cho phép `Admin` quản lý thông tin sản phẩm (Tên, Mô tả chi tiết thành phần, Giá chuẩn `numeric`, Hình ảnh đại diện, Danh mục).
* **FR-CAT-03 (Option Group & Recipe Config):** Cho phép `Admin` thiết lập các Nhóm Tùy Chọn (Option Groups) tái sử dụng (như *Mức đường, Mức đá, Size, Topping*) và gán khung công thức cho từng sản phẩm.
* **FR-CAT-04 (Inventory & Availability Status):** Cho phép `Admin` / `Staff` bật/tắt trạng thái kinh doanh (`IsAvailable`) hoặc cập nhật số lượng tồn kho trong ngày (`StockQuantity`).
* **FR-CAT-05 (Public Menu Browsing):** Cho phép mọi Actor (kể cả khách vãng lai) xem danh mục, danh sách sản phẩm, tìm kiếm và lọc món.

---

### 3.3 Module 3: Order, Cart & Customizer POS (ORD)
* **FR-ORD-01 (Cart Management):** Cho phép `Customer` thêm món vào giỏ hàng, tăng/giảm số lượng, xóa món và tự động tính tổng tiền tạm tính.
* **FR-ORD-02 (Interactive Product Customizer):** Cho phép `Customer` tùy chỉnh món trực quan trên giao diện (chọn Size, mức Đường, mức Đá, Topping). Giao diện hiển thị các lớp ảnh (Visual Layers) phản hồi theo đúng lựa chọn công thức.
* **FR-ORD-03 (Order Checkout & Context):** Cho phép chọn hình thức đơn hàng **Dine-in** (Uống tại quán - chọn số bàn `TableNumber`) hoặc **Takeaway** (Mang đi) và gửi đơn hàng về hệ thống.
* **FR-ORD-04 (Kitchen Display System - KDS):** Cho phép `Staff` xem danh sách đơn hàng theo thời gian thực và chuyển đổi vòng đời trạng thái đơn: `Pending` $\rightarrow$ `Preparing` $\rightarrow$ `Completed` / `Cancelled`.
* **FR-ORD-05 (Auto Stock Deduction):** Tự động trừ số lượng tồn kho (`StockQuantity`) của sản phẩm khi đơn hàng được ghi nhận/hoàn thành.

---

### 3.4 Module 4: AI Cafe Assistant & Smart Analytics (AI)
* **FR-AI-01 (Smart Menu & Stock Query):** Trợ lý AI có khả năng truy vấn dữ liệu thực đơn và số lượng tồn kho từ Backend API để trả lời chính xác tình trạng món cho khách hàng.
* **FR-AI-02 (Recipe & Taste Recommendation):** AI phân tích mô tả sản phẩm và khẩu vị khách hàng để đưa ra lời khuyên chọn món phù hợp.
* **FR-AI-03 (General Assistant Chit-chat):** AI xử lý các câu hỏi giao tiếp thông thường nhưng duy trì định hướng thương hiệu của Smart Cafe nhờ System Prompt Guardrails.

---

## 4. Non-Functional Requirements (NFRs)

* **NFR-PERF-01 (Performance):** Thời gian phản hồi API (Response Time) $< 200\text{ms}$ cho các tác vụ thường và $< 2\text{s}$ cho truy vấn AI.
* **NFR-SEC-01 (Security):** Mật khẩu người dùng được mã hóa bằng thuật toán BCrypt / Argon2. JWT Token có thời hạn hết hạn (Expiration Time) rõ ràng.
* **NFR-DATA-01 (Data Integrity):** Giá tiền sử dụng kiểu dữ liệu `numeric(18,2)` để tránh lỗi làm tròn số thập phân.
* **NFR-USAB-01 (Usability):** Giao diện Responsive hoạt động mượt mà trên cả thiết bị di động (Mobile) và máy tính bảng POS.