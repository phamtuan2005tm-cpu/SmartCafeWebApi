# 03. Database Design Specification

**Project Name:** Smart Cafe & POS Ordering System  
**Database Engine:** PostgreSQL (Compatible with Entity Framework Core 8/9)  
**Document Version:** 1.2 (Finalized Schema with Customizer & Recipe Support)  

---

## 1. Entity-Relationship Overview

Hệ thống cơ sở dữ liệu được thiết kế theo mô hình quan hệ chuẩn hóa (Relational Database), hỗ trợ tối ưu cho việc phân quyền, quản lý thực đơn tĩnh/động, cơ chế tùy chỉnh món (Customizer) trực quan và xử lý đơn hàng thời gian thực.

```text
[Users] (Auth & Roles)
  │
  └──< [Orders] (Dine-in / Takeaway)
        │
        └──< [OrderDetails] (Items in Order)
              │
              └──< [OrderDetailOptions] (Selected Customizer Options: Size, Ice, Sugar, Topping)

[Categories]
  │
  └──< [Products] (Base Menu Items)
        │
        └──< [ProductOptionGroups] (Junction Table) >── [OptionGroups] (Group: Ice, Sugar, Topping)
                                                              │
                                                              └──< [Options] (Details: 50% Ice, Extra Cheese)
```

---

## 2. Detailed Table Schemas & EF Core Entities

### 2.1 Bảng `Users` (Quản lý tài khoản & Phân quyền)
Lưu trữ thông tin định danh của người dùng (`Customer`, `Staff`, `Admin`).

* `Id` (`Guid`, Primary Key)
* `Email` (`varchar(255)`, Unique, Not Null)
* `PasswordHash` (`varchar(255)`, Not Null - BCrypt hashed)
* `FullName` (`varchar(100)`, Not Null)
* `Role` (`varchar(50)`, Not Null - `Customer`, `Staff`, `Admin`)
* `IsActive` (`boolean`, Default: `true`)
* `CreatedAt` (`timestamp with time zone`, Default: `CURRENT_TIMESTAMP`)

---

### 2.2 Bảng `Categories` (Danh mục sản phẩm)
Phân loại các nhóm sản phẩm (Coffee, Tea, Freeze, Bakery...).

* `Id` (`Guid`, Primary Key)
* `Name` (`varchar(100)`, Not Null)
* `Description` (`text`, Nullable)
* `DisplayOrder` (`int`, Default: `0`)
* `CreatedAt` (`timestamp with time zone`, Default: `CURRENT_TIMESTAMP`)

---

### 2.3 Bảng `Products` (Sản phẩm / Món ăn gốc)
Lưu thông tin cốt lõi của món nước hoặc bánh.

* `Id` (`Guid`, Primary Key)
* `CategoryId` (`Guid`, Foreign Key -> `Categories.Id`, Not Null)
* `Name` (`varchar(150)`, Not Null)
* `Description` (`text`, Nullable - Mô tả thành phần cho AI Context)
* `Price` (`numeric(18,2)`, Not Null - Giá chuẩn gốc)
* `ImageUrl` (`varchar(500)`, Nullable - Ảnh đại diện ly nước gốc)
* `StockQuantity` (`int`, Default: `0` - Số lượng tồn kho trong ngày)
* `IsAvailable` (`boolean`, Default: `true` - Trạng thái kinh doanh)
* `CreatedAt` (`timestamp with time zone`, Default: `CURRENT_TIMESTAMP`)

---

### 2.4 Các bảng Quản lý Tùy chỉnh Công thức (Customizer & Recipe)

#### Bảng `OptionGroups` (Nhóm tùy chọn)
Định nghĩa các nhóm tiêu chí tùy chỉnh (Ví dụ: "Mức Đường", "Mức Đá", "Topping", "Độ Chua của Tắc").
* `Id` (`Guid`, Primary Key)
* `Name` (`varchar(100)`, Not Null)
* `InputType` (`varchar(50)`, Not Null - `Single` chọn 1 hoặc `Multiple` chọn nhiều)

#### Bảng `Options` (Giá trị tùy chọn chi tiết)
Chứa các lựa chọn cụ thể nằm bên trong nhóm, kèm theo giá điều chỉnh và ảnh lớp phủ (Visual Layer).
* `Id` (`Guid`, Primary Key)
* `OptionGroupId` (`Guid`, Foreign Key -> `OptionGroups.Id`, Not Null)
* `Name` (`varchar(100)`, Not Null - Ví dụ: "50% Đường", "Thêm Kem Cheese")
* `AdditionalPrice` (`numeric(18,2)`, Default: `0.00` - Giá cộng thêm hoặc trừ bớt)
* `LayerImageUrl` (`varchar(500)`, Nullable - Đường dẫn file PNG trong suốt dùng cho Frontend Visual Toggle)
* `DisplayOrder` (`int`, Default: `0`)

#### Bảng trung gian `ProductOptionGroups` (Liên kết Món ăn với Nhóm tùy chọn)
Cho phép một sản phẩm có thể gắn nhiều nhóm tùy chọn linh hoạt (N-N).
* `ProductId` (`Guid`, Composite PK, Foreign Key -> `Products.Id`)
* `OptionGroupId` (`Guid`, Composite PK, Foreign Key -> `OptionGroups.Id`)

---

### 2.5 Các bảng Quản lý Đơn hàng (Orders & POS)

#### Bảng `Orders` (Đơn hàng)
* `Id` (`Guid`, Primary Key)
* `CustomerId` (`Guid`, Foreign Key -> `Users.Id`, Nullable - hỗ trợ khách vãng lai nếu cần)
* `OrderType` (`varchar(50)`, Not Null - `DineIn` hoặc `Takeaway`)
* `TableNumber` (`int`, Nullable - Bắt buộc nếu là DineIn)
* `TotalAmount` (`numeric(18,2)`, Not Null)
* `Status` (`varchar(50)`, Not Null - `Pending`, `Preparing`, `Completed`, `Cancelled`)
* `CreatedAt` (`timestamp with time zone`, Default: `CURRENT_TIMESTAMP`)

#### Bảng `OrderDetails` (Chi tiết món trong đơn hàng)
* `Id` (`Guid`, Primary Key)
* `OrderId` (`Guid`, Foreign Key -> `Orders.Id`, Not Null)
* `ProductId` (`Guid`, Foreign Key -> `Products.Id`, Not Null)
* `Quantity` (`int`, Not Null)
* `UnitPrice` (`numeric(18,2)`, Not Null - Giá tại thời điểm đặt)

#### Bảng `OrderDetailOptions` (Lưu vết cấu hình tùy chỉnh của từng ly nước)
Lưu giữ chính xác những gì khách đã chọn (Size, Đường, Đá, Topping) để nhân viên pha chế ở màn hình KDS đọc được.
* `Id` (`Guid`, Primary Key)
* `OrderDetailId` (`Guid`, Foreign Key -> `OrderDetails.Id`, Not Null)
* `OptionId` (`Guid`, Foreign Key -> `Options.Id`, Not Null)
* `OptionNameSnapshot` (`varchar(100)`, Not Null - Lưu tên tùy chọn tại thời điểm đặt phòng trường hợp sau này Admin sửa tên Option gốc)
* `AdditionalPriceSnapshot` (`numeric(18,2)`, Not Null)

---

## 3. PostgreSQL & EF Core Configuration Notes

1. **Precision & Scale:** Tất cả các trường tiền tệ (`Price`, `AdditionalPrice`, `TotalAmount`) bắt buộc sử dụng kiểu `numeric(18,2)` để tránh sai số dấu phẩy động.
2. **Cascade Deleting Policy:** 
   * Xóa `Category` $\rightarrow$ Set Null hoặc Restrict `Products` để tránh mất dữ liệu thực đơn vô ý.
   * Xóa `Order` $\rightarrow$ Cascade xóa `OrderDetails` và `OrderDetailOptions`.
3. **Data Seeding:** Tài khoản `Admin` mặc định và các `Categories` / `OptionGroups` cơ bản (Đường, Đá) sẽ được chèn tự động bằng phương thức `HasData()` trong `DbContext` khi khởi chạy migration lần đầu.