# Phân tích dự án SmartCafe

> Tài liệu được lập từ mã nguồn hiện có trong project `SmartCafe.API`. Tên class, entity, property, `DbSet`, bảng và khóa được giữ nguyên theo code.

## 1. Tổng quan

- **Solution:** `SmartCafe.slnx`
- **Project:** `SmartCafe.API/SmartCafe.API.csproj`
- **SDK:** `Microsoft.NET.Sdk.Web`
- **Target Framework:** `.NET 8` (`net8.0`)
- **Nullable:** bật
- **Implicit usings:** bật
- **Database provider runtime:** PostgreSQL qua `Npgsql.EntityFrameworkCore.PostgreSQL` và `UseNpgsql`.
- **API technology:** ASP.NET Core Controllers.
- **API documentation:** Swagger/OpenAPI qua `Swashbuckle.AspNetCore`, chỉ bật trong môi trường Development.

## 2. Cấu trúc thư mục và file

```text
SmartCafe.API/
├── Domain/
│   └── Entities/
│       ├── Category.cs
│       ├── Option.cs
│       ├── OptionGroup.cs
│       ├── Order.cs
│       ├── OrderDetail.cs
│       ├── OrderDetailOption.cs
│       ├── Product.cs
│       ├── ProductOptionGroup.cs
│       └── User.cs
├── Infrastructure/
│   └── Data/
│       ├── Configurations/
│       │   ├── CategoryConfiguration.cs
│       │   ├── OptionConfiguration.cs
│       │   ├── OptionGroupConfiguration.cs
│       │   ├── OrderConfiguration.cs
│       │   ├── OrderDetailConfiguration.cs
│       │   ├── OrderDetailOptionConfiguration.cs
│       │   ├── ProductConfiguration.cs
│       │   ├── ProductOptionGroupConfiguration.cs
│       │   └── UserConfiguration.cs
│       ├── DbInitializer.cs
│       └── SmartCafeDbContext.cs
├── Migrations/
│   ├── 20261018052319_InitialCreate.cs
│   ├── 20261018052319_InitialCreate.Designer.cs
│   └── SmartCafeDbContextModelSnapshot.cs
├── Properties/
│   └── launchSettings.json
├── Program.cs
├── SmartCafe.API.csproj
├── SmartCafe.API.csproj.Backup.tmp
├── SmartCafe.API.http
├── appsettings.json
└── appsettings.Development.json
```

Các thư mục được khai báo bằng `Folder Include` trong project nhưng hiện chưa có file nghiệp vụ tương ứng gồm:

- `Application/Services/`
- `Controllers/Auth/`
- `Domain/DTOs/`
- `Infrastructure/Migration/`

Danh sách trên phản ánh các file project hiện đang quản lý; không kết luận rằng workspace không thể chứa file ngoài danh sách project.

## 3. Các entity chính xác

### 3.1 `User`

Namespace: `SmartCafe.API.Domain.Entities`

| Property | Kiểu | Nullable | Giá trị mặc định trong entity | Ý nghĩa hiện tại |
|---|---|---:|---|---|
| `Id` | `Guid` | Không | — | Khóa chính |
| `Email` | `string` | Không theo CLR | `string.Empty` | Email người dùng; migration cho phép null |
| `PasswordHash` | `string` | Không | `string.Empty` | Mật khẩu đã băm |
| `FullName` | `string` | Không | `string.Empty` | Họ tên |
| `PhoneNumber` | `string` | Không theo CLR | `string.Empty` | Số điện thoại; migration cho phép null |
| `Role` | `string` | Không | `"Customer"` | Vai trò, được lưu dạng string |
| `IsActive` | `bool` | Không | `true` | Trạng thái hoạt động |
| `CreatedAt` | `DateTime` | Không | `DateTime.Now` | Thời điểm tạo |
| `Orders` | `ICollection<Order>` | Không | `new List<Order>()` | Navigation collection đến đơn hàng |

Cấu hình: khóa chính `Id`; `PasswordHash` tối đa 255 ký tự; `FullName` cuối cùng được cấu hình tối đa 100 ký tự; `Email` tối đa 150 và nullable trong migration; `PhoneNumber` tối đa 20 và nullable trong migration; `Role` tối đa 30; `IsActive` mặc định database `true`; `CreatedAt` mặc định database `CURRENT_TIMESTAMP`.

### 3.2 `Category`

| Property | Kiểu | Nullable | Giá trị mặc định trong entity | Ý nghĩa |
|---|---|---:|---|---|
| `Id` | `Guid` | Không | — | Khóa chính |
| `Name` | `string` | Không | `string.Empty` | Tên danh mục |
| `Description` | `string?` | Có | `null` | Mô tả |
| `DisplayOrder` | `int` | Không | — | Thứ tự hiển thị |
| `CreatedAt` | `DateTime` | Không | `DateTime.UtcNow` | Thời điểm tạo |
| `Products` | `ICollection<Product>` | Không | `new List<Product>()` | Các sản phẩm thuộc danh mục |

Cấu hình: `Name` bắt buộc, tối đa 100; `Description` tối đa 500 và nullable.

### 3.3 `Product`

| Property | Kiểu | Nullable | Giá trị mặc định trong entity | Ý nghĩa |
|---|---|---:|---|---|
| `Id` | `Guid` | Không | — | Khóa chính |
| `CategoryId` | `Guid` | Không | — | Foreign key đến `Category.Id` |
| `Name` | `string` | Không | `string.Empty` | Tên sản phẩm |
| `Description` | `string?` | Có | `null` | Mô tả |
| `Price` | `decimal` | Không | — | Giá sản phẩm |
| `ImageUrl` | `string?` | Có | `null` | URL hình ảnh |
| `StockQuantity` | `int` | Không | `0` | Số lượng tồn |
| `IsAvailable` | `bool` | Không | `true` | Có thể bán hay không |
| `CreatedAt` | `DateTime` | Không | `DateTime.UtcNow` | Thời điểm tạo |
| `Category` | `Category` | Không | `null!` | Navigation đến danh mục |
| `OrderDetails` | `ICollection<OrderDetail>` | Không | `new List<OrderDetail>()` | Các dòng đơn hàng |
| `ProductOptionGroups` | `ICollection<ProductOptionGroup>` | Không | `new List<ProductOptionGroup>()` | Liên kết đến nhóm tùy chọn |

Cấu hình: `Name` tối đa 150; `Description` và `ImageUrl` tối đa 500, nullable; `Price` `numeric(18,2)`; `StockQuantity` mặc định 0; `IsAvailable` mặc định true; `CreatedAt` mặc định `CURRENT_TIMESTAMP`; quan hệ với `Category` dùng `Restrict` khi xóa.

### 3.4 `OptionGroup`

| Property | Kiểu | Nullable | Giá trị mặc định trong entity | Ý nghĩa |
|---|---|---:|---|---|
| `Id` | `Guid` | Không | — | Khóa chính |
| `Name` | `string` | Không | `string.Empty` | Tên nhóm tùy chọn |
| `InputType` | `string` | Không | `"Single"` | Kiểu nhập/lựa chọn |
| `IsRequired` | `bool` | Không | `false` | Có bắt buộc chọn hay không |
| `AllowMultiple` | `bool` | Không | `false` | Cho phép chọn nhiều hay không |
| `Options` | `ICollection<Option>` | Không | `new List<Option>()` | Các tùy chọn trong nhóm |
| `ProductOptionGroups` | `ICollection<ProductOptionGroup>` | Không | `new List<ProductOptionGroup>()` | Liên kết với sản phẩm |

Cấu hình: `Name` tối đa 100; `IsRequired` và `AllowMultiple` có default database là false.

### 3.5 `Option`

| Property | Kiểu | Nullable | Giá trị mặc định trong entity | Ý nghĩa |
|---|---|---:|---|---|
| `Id` | `Guid` | Không | — | Khóa chính |
| `OptionGroupId` | `Guid` | Không | — | Foreign key đến `OptionGroup.Id` |
| `Name` | `string` | Không | `string.Empty` | Tên tùy chọn |
| `AdditionalPrice` | `decimal` | Không | `0` | Giá cộng thêm |
| `QuantityValue` | `decimal` | Không | `0` | Giá trị định lượng |
| `Unit` | `string` | Không | `"ml"` | Đơn vị định lượng, ví dụ ml/g |
| `LayerImageUrl` | `string?` | Có | `null` | URL ảnh lớp/tùy chọn |
| `DisplayOrder` | `int` | Không | `0` | Thứ tự hiển thị |
| `ExtraPrice` | `decimal` | Không | `0` | Giá cộng thêm bổ sung |
| `IsAvailable` | `bool` | Không | `true` | Có khả dụng hay không |
| `OptionGroup` | `OptionGroup` | Không | `null!` | Navigation đến nhóm |
| `OrderDetailOptions` | `ICollection<OrderDetailOption>` | Không | `new List<OrderDetailOption>()` | Các lựa chọn đã dùng trong dòng đơn |

Các property decimal được chuẩn hóa thành `numeric(18,2)` trong `SmartCafeDbContext`; quan hệ với `OptionGroup` là 1-N, xóa nhóm sẽ cascade sang options.

### 3.6 `ProductOptionGroup`

| Property | Kiểu | Nullable | Ý nghĩa |
|---|---|---:|---|
| `ProductId` | `Guid` | Không | Khóa/FK đến `Product.Id` |
| `OptionGroupId` | `Guid` | Không | Khóa/FK đến `OptionGroup.Id` |
| `Product` | `Product` | Không | Navigation đến sản phẩm |
| `OptionGroup` | `OptionGroup` | Không | Navigation đến nhóm tùy chọn |

Đây là entity bảng trung gian cho quan hệ nhiều-nhiều giữa `Product` và `OptionGroup`. Khóa chính hợp phần là `{ ProductId, OptionGroupId }`. Hai quan hệ đều `Cascade` khi xóa.

### 3.7 `Order`

| Property | Kiểu | Nullable | Giá trị mặc định trong entity | Ý nghĩa |
|---|---|---:|---|---|
| `Id` | `Guid` | Không | — | Khóa chính |
| `CustomerId` | `Guid?` | Có | `null` | Dự kiến là FK đến `User.Id` |
| `OrderType` | `string` | Không | `"DineIn"` | Loại đơn hàng |
| `TableNumber` | `int?` | Có | `null` | Số bàn, nếu có |
| `TotalAmount` | `decimal` | Không | — | Tổng tiền |
| `Status` | `string` | Không | `"Pending"` | Trạng thái: comment trong entity nêu `Pending`, `Preparing`, `Completed`, `Cancelled` |
| `CreatedAt` | `DateTime` | Không | `DateTime.UtcNow` | Thời điểm tạo |
| `Customer` | `User?` | Có | `null` | Navigation đến khách hàng |
| `OrderDetails` | `ICollection<OrderDetail>` | Không | `new List<OrderDetail>()` | Các dòng trong đơn |

Cấu hình: `TotalAmount` `numeric(18,2)`; `Status` lưu dạng string, tối đa 50; `CreatedAt` mặc định `CURRENT_TIMESTAMP`; quan hệ xóa dùng `Restrict`.

### 3.8 `OrderDetail`

| Property | Kiểu | Nullable | Ý nghĩa |
|---|---|---:|---|
| `Id` | `Guid` | Không | Khóa chính |
| `OrderId` | `Guid` | Không | Foreign key đến `Order.Id` |
| `ProductId` | `Guid` | Không | Foreign key đến `Product.Id` |
| `Quantity` | `int` | Không | Số lượng sản phẩm |
| `UnitPrice` | `decimal` | Không | Đơn giá tại thời điểm đặt |
| `Order` | `Order` | Không | Navigation đến đơn hàng |
| `Product` | `Product` | Không | Navigation đến sản phẩm |
| `OrderDetailOptions` | `ICollection<OrderDetailOption>` | Không | Các option của dòng đơn |

Quan hệ với `Order` là 1-N, `Cascade`; quan hệ với `Product` là 1-N, `Restrict`; `UnitPrice` là `numeric(18,2)`.

### 3.9 `OrderDetailOption`

| Property | Kiểu | Nullable | Giá trị mặc định trong entity | Ý nghĩa |
|---|---|---:|---|---|
| `Id` | `Guid` | Không | — | Khóa chính |
| `OrderDetailId` | `Guid` | Không | — | Foreign key đến `OrderDetail.Id` |
| `OptionId` | `Guid` | Không | — | Foreign key đến `Option.Id` |
| `OptionNameSnapshot` | `string` | Không | `string.Empty` | Tên option được chụp tại thời điểm đặt |
| `AdditionalPriceSnapshot` | `decimal` | Không | `0` | Giá option được chụp tại thời điểm đặt |
| `OrderDetail` | `OrderDetail` | Không | `null!` | Navigation đến dòng đơn |
| `Option` | `Option` | Không | `null!` | Navigation đến option gốc |

Quan hệ với `OrderDetail` là 1-N, `Cascade`; quan hệ với `Option` là 1-N, `Restrict`; các trường snapshot được dùng để bảo toàn thông tin lịch sử.

## 4. DbContext và mapping

`SmartCafeDbContext` nằm tại `Infrastructure/Data/SmartCafeDbContext.cs` và khai báo các `DbSet` sau:

- `Users` → `User`
- `Categories` → `Category`
- `Products` → `Product`
- `Orders` → `Order`
- `OrderDetails` → `OrderDetail`
- `OptionGroups` → `OptionGroup`
- `Options` → `Option`
- `ProductOptionGroups` → `ProductOptionGroup`
- `OrderDetailOptions` → `OrderDetailOption`

Trong `OnModelCreating`:

1. Gọi `ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())` để tự động áp dụng toàn bộ `IEntityTypeConfiguration<T>`.
2. Chuẩn hóa toàn bộ `decimal`/`decimal?` thành `numeric(18,2)`.
3. Đăng ký khóa hợp phần cho `ProductOptionGroup`.
4. Gọi `DbInitializer.SeedData(modelBuilder)` để seed dữ liệu.

## 5. Sơ đồ quan hệ dữ liệu hiện tại

```text
User 1 ──── N Order
Category 1 ──── N Product
OptionGroup 1 ──── N Option
Product N ──── N OptionGroup
  thông qua ProductOptionGroup
Order 1 ──── N OrderDetail
Product 1 ──── N OrderDetail
OrderDetail 1 ──── N OrderDetailOption
Option 1 ──── N OrderDetailOption
```

### Khóa và hành vi xóa

| Quan hệ | Foreign key | Delete behavior |
|---|---|---|
| `Product` → `Category` | `Product.CategoryId` | `Restrict` |
| `Option` → `OptionGroup` | `Option.OptionGroupId` | `Cascade` |
| `ProductOptionGroup` → `Product` | `ProductOptionGroup.ProductId` | `Cascade` |
| `ProductOptionGroup` → `OptionGroup` | `ProductOptionGroup.OptionGroupId` | `Cascade` |
| `OrderDetail` → `Order` | `OrderDetail.OrderId` | `Cascade` |
| `OrderDetail` → `Product` | `OrderDetail.ProductId` | `Restrict` |
| `OrderDetailOption` → `OrderDetail` | `OrderDetailOption.OrderDetailId` | `Cascade` |
| `OrderDetailOption` → `Option` | `OrderDetailOption.OptionId` | `Restrict` |
| `Order` → `User` theo cấu hình hiện tại | **`Order.Id`** | `Restrict` |

## 6. Migration/schema đối chiếu

Migration hiện có là `20261018052319_InitialCreate` và provider/schema được sinh là PostgreSQL, thể hiện qua các kiểu `uuid`, `character varying`, `timestamp with time zone`, `numeric(18,2)`.

Các bảng được tạo:

- `Categories`
- `OptionGroups`
- `Users`
- `Products`
- `Options`
- `Orders`
- `ProductOptionGroups`
- `OrderDetails`
- `OrderDetailOptions`

Migration xác nhận khóa chính hợp phần của `ProductOptionGroups` là `{ ProductId, OptionGroupId }`, đồng thời xác nhận snapshot fields trong `OrderDetailOptions`.

## 7. Khởi tạo dữ liệu mẫu

`DbInitializer.SeedData` seed dữ liệu bằng `HasData`:

- 3 `User`: role `Admin`, `Staff`, `Customer`.
- 2 `Category`: `Cà Phê`, `Trà & Trái Cây`.
- 2 `Product`: `Bạc Sỉu Sữa Dừa`, `Trà Đào Cam Sả`.
- 2 `OptionGroup`: `Mức Đường`, `Mức Đá`.
- 4 `Option`: các mức đường/đá.
- 4 dòng liên kết `ProductOptionGroup`.

Mật khẩu mẫu được băm bằng SHA-256 rồi chuyển sang Base64 trong method private `HashPassword`. Đây là dữ liệu seed môi trường phát triển; không nên dùng mật khẩu mẫu trong môi trường thật.

## 8. Bootstrap và cấu hình runtime

### `Program.cs`

- Đăng ký Controllers và Swagger.
- Đăng ký `SmartCafeDbContext` với:
  `UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))`.
- Bật Swagger/Swagger UI khi `app.Environment.IsDevelopment()`.
- Dùng HTTPS redirection, authorization middleware và `MapControllers()`.

Hiện tại `AddControllers()` được gọi hai lần trong `Program.cs`; lần gọi thứ hai không cần thiết.

### Connection string

`appsettings.json` và `appsettings.Development.json` đều có `ConnectionStrings:DefaultConnection` trỏ đến PostgreSQL localhost, database `SmartCafeDb`.

> Connection string hiện chứa mật khẩu dạng plain text trong file cấu hình. Nên chuyển sang User Secrets, biến môi trường hoặc secret store khi chạy ngoài máy cá nhân.

### `launchSettings.json`

- HTTP: `http://localhost:5241`, mở `/swagger`.
- HTTPS: `https://localhost:7128` và `http://localhost:5241`, mở `/swagger`.
- IIS Express: profile riêng, có HTTPS port `44325`.
- Các profile đặt `ASPNETCORE_ENVIRONMENT=Development`.

## 9. Các điểm cần kiểm tra/cần ưu tiên xử lý

### 9.1 Sai khóa ngoại có khả năng làm sai quan hệ `Order`–`User`

Trong entity, `Order` có property `CustomerId` và navigation `Customer`. Tuy nhiên `OrderConfiguration` hiện ghi:

```csharp
.HasForeignKey(o => o.Id)
```

Migration cũng tạo FK `FK_Orders_Users_Id` từ `Orders.Id` sang `Users.Id`, thay vì từ `Orders.CustomerId` sang `Users.Id`. Điều này không khớp với tên property và thiết kế thông thường của `CustomerId`, đồng thời khiến `Order.Id` bị ràng buộc là phải tồn tại trong `Users.Id`.

Nếu mục tiêu là liên kết đơn hàng với khách hàng theo `CustomerId`, cần sửa mapping thành `HasForeignKey(o => o.CustomerId)`, sau đó tạo migration cập nhật schema. Không nên chỉ sửa migration đã áp dụng trong môi trường có dữ liệu; cần quy trình migration phù hợp.

### 9.2 `UserConfiguration` cấu hình `FullName` hai lần

`FullName` được cấu hình tối đa 50 ký tự ở đoạn đầu và tối đa 100 ký tự ở đoạn sau. Migration hiện phản ánh `maxLength: 100`, nhưng nên xóa cấu hình trùng để tránh nhầm lẫn.

### 9.3 Nullable CLR và nullable database chưa hoàn toàn nhất quán

`User.Email` và `User.PhoneNumber` là `string` không nullable trong C# nhưng configuration lại dùng `IsRequired(false)`, migration cho phép null. Nếu database cho phép null, nên cân nhắc khai báo `string?` trong entity; nếu bắt buộc, đổi mapping và migration tương ứng.

### 9.4 Giá trị thời gian mặc định không đồng nhất

Một số entity khởi tạo bằng `DateTime.UtcNow`, còn `User.CreatedAt` dùng `DateTime.Now`. Database default cho nhiều bảng là `CURRENT_TIMESTAMP`. Nên thống nhất UTC ở entity và database để tránh sai lệch múi giờ.

### 9.5 Provider package dư thừa hoặc cần xác minh

Project tham chiếu cả `Microsoft.EntityFrameworkCore.SqlServer` và PostgreSQL, nhưng `Program.cs` chỉ dùng PostgreSQL. Nếu không có kế hoạch hỗ trợ SQL Server, có thể xem xét loại bỏ package SQL Server để giảm phụ thuộc.

### 9.6 Bảo mật seed data

`DbInitializer` chứa email và mật khẩu mẫu rõ ràng trước khi băm. Không nên dùng các thông tin này ở production; nên thay bằng secret/seed flow an toàn.

### 9.7 API surface hiện còn tối thiểu

Project file hiện không liệt kê Controller, DTO, Service hoặc repository nghiệp vụ ngoài các thư mục placeholder. `SmartCafe.API.http` vẫn gọi endpoint mẫu `/weatherforecast/`, trong khi `Program.cs` không cho thấy endpoint WeatherForecast tương ứng.

## 10. Kết luận

Mô hình hiện tại là một ASP.NET Core Web API .NET 8 sử dụng EF Core 8 và PostgreSQL, với miền dữ liệu xoay quanh người dùng, danh mục, sản phẩm, nhóm tùy chọn, đơn hàng và các dòng tùy chọn của đơn hàng. Mô hình entity gồm chính xác 9 class: `User`, `Category`, `Product`, `OptionGroup`, `Option`, `ProductOptionGroup`, `Order`, `OrderDetail`, `OrderDetailOption`.

Điểm cần ưu tiên xác minh trước khi phát triển tiếp là mapping `Order.Customer` vì code hiện tại dùng `Order.Id` làm foreign key thay cho `Order.CustomerId`. Các điểm tiếp theo là cấu hình trùng `FullName`, tính nhất quán nullable/thời gian và việc bảo vệ connection string/seed credentials.
