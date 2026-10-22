# 02. Architecture & Tech Stack Specification

**Project Name:** Smart Cafe & POS Ordering System  
**Document Version:** 1.1 (Updated with Full-stack Ecosystem, Git, Testing & Deployment)  
**Architecture Pattern:** Clean Architecture (Monolithic Solution with 4 Logical Layers)  

---

## 1. Complete Technology Stack Matrix

Hệ thống ưu tiên sử dụng các công nghệ hiện đại, có hiệu năng cao và chuẩn hóa cho mô hình Full-stack Application:

| Tầng / Thành phần | Công nghệ lựa chọn | Lý do lựa chọn (Trade-off Analysis) |
| :--- | :--- | :--- |
| **Backend Framework** | .NET 8 Web API | LTS release, hiệu năng xử lý I/O cao, hỗ trợ Dependency Injection sẵn có. |
| **Database** | PostgreSQL | Relational Database mã nguồn mở, mạnh mẽ, xử lý dữ liệu giao dịch (ACID) an toàn. |
| **ORM / Data Access** | Entity Framework Core 8 | Tích hợp sâu với .NET, hỗ trợ Fluent API mạnh mẽ để cấu hình schema chính xác. |
| **Authentication** | JWT (JSON Web Token) | Stateless authentication, phù hợp cho RESTful API, dễ dàng mở rộng Client. |
| **Frontend Client** | React.js / SPA (hoặc Next.js) | Component-based, render mượt mà cho trải nghiệm POS & Order real-time. |
| **API Documentation** | Swagger / OpenAPI | Tự động sinh tài liệu thử nghiệm API tương tác trực tiếp. |
| **Version Control** | Git & GitHub | Quản lý mã nguồn theo luồng Gitflow chuẩn công nghiệp. |
| **Container & Cloud** | Docker & Render / Railway | Đóng gói môi trường nhất quán, hỗ trợ CI/CD deployment tự động. |

---

## 2. Solution Structure & Project Layout

Dự án được cấu trúc dưới dạng **Monorepo** chứa cả tài liệu (`docs/`), mã nguồn Backend (`src/`) và ứng dụng Frontend (`web/`):

```text
smart-cafe-pos/
├── docs/                                    <-- Tài liệu đặc tả hệ thống (Documentation-as-Code)
│   ├── 01-overview-and-requirements.md
│   ├── 02-architecture-and-techstack.md
│   └── 03-database-design.md
│
├── src/                                     <-- BACKEND (.NET 8 Clean Architecture)
│   ├── SmartCafe.sln                        <-- Solution File chính
│   │
│   ├── Core/
│   │   ├── SmartCafe.Domain/                <-- Class Library (Entities, Enums)
│   │   └── SmartCafe.Application/             <-- Class Library (DTOs, Interfaces, Services)
│   │
│   ├── Infrastructure/
│   │   └── SmartCafe.Infrastructure/       <-- Class Library (AppDbContext, EF Core, Repositories)
│   │
│   └── Presentation/
│       └── SmartCafe.Api/                   <-- Web API Project (Controllers, Program.cs)
│
└── web/                                     <-- FRONTEND (React Client Application)
    ├── public/
    ├── src/
    │   ├── assets/
    │   ├── components/                      <-- Reusable UI Components
    │   ├── pages/                           <-- Menu, Cart, KDS, Admin Dashboard
    │   ├── services/                        <-- Axios / Fetch API Callers
    │   └── App.jsx
    └── package.json
```

---

## 3. Clean Architecture 4-Layer Details (Backend)

### 3.1 Domain Layer (`SmartCafe.Domain`)
* **Loại Project:** Class Library
* **Trách nhiệm:** Chứa toàn bộ mô hình nghiệp vụ cốt lõi (**Core Business Entities**), các hằng số, định nghĩa Enums (ví dụ: `OrderStatus`).
* **Quy tắc phụ thuộc:** Tầng trung tâm, **tuyệt đối không tham chiếu** đến bất kỳ dự án hay thư viện bên ngoài nào.

### 3.2 Application Layer (`SmartCafe.Application`)
* **Loại Project:** Class Library
* **Trách nhiệm:** Định nghĩa các kịch bản sử dụng (**Use Cases**), các giao diện kết nối (**Interfaces** như `IAppDbContext`, `IJwtTokenGenerator`), các lớp truyền tải dữ liệu (**DTOs**).
* **Quy tắc phụ thuộc:** Chỉ tham chiếu duy nhất đến **`SmartCafe.Domain`**.

### 3.3 Infrastructure Layer (`SmartCafe.Infrastructure`)
* **Loại Project:** Class Library
* **Trách nhiệm:** Hiện thực hóa các giao diện đã định nghĩa ở tầng Application. Quản lý `AppDbContext`, cấu hình bảng bằng **Fluent API**, tạo dữ liệu mồi (**Data Seeding**), và xử lý tương tác với PostgreSQL.
* **Quy tắc phụ thuộc:** Tham chiếu đến **`SmartCafe.Application`** và **`SmartCafe.Domain`**.

### 3.4 Presentation Layer (`SmartCafe.Api`)
* **Loại Project:** Web API Project
* **Trách nhiệm:** Điểm tiếp nhận request từ bên ngoài (**HTTP Endpoints**), định nghĩa **Controllers**, đăng ký DI (**Dependency Injection**) trong `Program.cs`, cấu hình **Middlewares** xử lý lỗi và xác thực JWT Bearer.
* **Quy tắc phụ thuộc:** Tham chiếu đến tất cả các tầng còn lại để kết nối toàn bộ hệ thống tại thời điểm khởi chạy (**Runtime**).

---

## 4. Frontend & Backend Integration Flow

### 4.1 Quy tắc phụ thuộc Backend (Dependency Vector)
Mối quan hệ phụ thuộc mã nguồn (**Compile-time Dependency**) tuân thủ nghiêm ngặt chiều từ ngoài vào trong:

$$\text{SmartCafe.Api} \longrightarrow \text{SmartCafe.Infrastructure} \longrightarrow \text{SmartCafe.Application} \longrightarrow \text{SmartCafe.Domain}$$

### 4.2 Luồng tích hợp giữa Frontend & Backend (Client-Server Request Lifecycle)
1. **Frontend Request:** Người dùng thao tác trên màn hình POS/Web (`web/`), Frontend gửi HTTP Request (kèm `Authorization: Bearer <token>`) đến `.NET Web API` (`SmartCafe.Api`).
2. **CORS & Authentication Middleware:** API kiểm tra cấu hình CORS và xác thực JWT Token hợp lệ.
3. **Controller Execution:** `OrdersController` tiếp nhận DTO và chuyển xuống `OrderService` (`SmartCafe.Application`).
4. **Business & Data Persistence:** Service áp dụng logic nghiệp vụ, cập nhật trạng thái Entity (`SmartCafe.Domain`) và lưu qua `AppDbContext` (`SmartCafe.Infrastructure`) vào PostgreSQL.
5. **Response Delivery:** Dữ liệu chuẩn hóa dạng JSON DTO được trả về để Frontend cập nhật trạng thái UI.

---

## 5. Development Quality Assurance: Testing Strategy

Hệ thống thiết lập quy trình kiểm thử phân tầng để đảm bảo tính ổn định của mã nguồn:

* **Unit Testing (xUnit / NUnit):** Tập trung kiểm thử logic nghiệp vụ tại tầng `Application` và `Domain` (ví dụ: test thuật toán tính tổng tiền đơn hàng, chuyển đổi trạng thái `OrderStatus`).
* **Integration Testing:** Kiểm thử khả năng tương tác giữa EF Core với cơ sở dữ liệu PostgreSQL (sử dụng InMemory Database hoặc Testcontainers).
* **API Testing:** Kiểm thử các HTTP Endpoints thông qua Swagger UI / Postman Collection.

---

## 6. Git Workflow & Deployment Strategy

### 6.1 Git Branching Model (Gitflow Standard)
Dự án áp dụng mô hình phân nhánh Gitflow chuẩn:
* **`main`**: Nhánh chứa mã nguồn chính thức đã kiểm thử và sẵn sàng cho Production.
* **`develop`**: Nhánh tích hợp mã nguồn chính cho quá trình phát triển.
* **`feature/<feature-name>`**: Các nhánh ngắn hạn dùng để phát triển từng tính năng riêng biệt (ví dụ: `feature/jwt-auth`, `feature/order-kds`).

### 6.2 Deployment Pipeline (DevOps)
* **Containerization:** Đóng gói ứng dụng .NET 8 Web API và Frontend thành các **Docker Container Images** để đảm bảo ứng dụng chạy đồng nhất trên mọi môi trường.
* **Database Migration:** Tự động áp dụng `dotnet ef database update` trong quy trình khởi chạy container để đồng bộ schema PostgreSQL.
* **Hosting Platforms:**
  * **Database:** PostgreSQL Managed Service (Supabase / Render / NeonDB).
  * **Backend API:** Docker Container deployed trên Render / Railway / Azure App Service.
  * **Frontend Client:** Vercel / Netlify.