-- ==========================================================
-- SCRIPT KHỞI TẠO CƠ SỞ DỮ LIỆU CỔNG THÔNG TIN PR DOANH NGHIỆP
-- Hệ quản trị: Microsoft SQL Server 2022
-- Tên Database: KDTM_BaoVeThuongHieu_New
-- ==========================================================

USE master;
GO

-- 1. Tạo Database mới nếu chưa tồn tại
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'KDTM_BaoVeThuongHieu_New')
BEGIN
    CREATE DATABASE KDTM_BaoVeThuongHieu_New;
END
GO

USE KDTM_BaoVeThuongHieu_New;
GO

-- Xóa các khóa ngoại nếu đã tồn tại để tránh xung đột khi chạy lại
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_Roles_RoleId')
    ALTER TABLE dbo.Users DROP CONSTRAINT FK_Users_Roles_RoleId;
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Posts_Categories_CategoryId')
    ALTER TABLE dbo.Posts DROP CONSTRAINT FK_Posts_Categories_CategoryId;
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Posts_Users_AuthorId')
    ALTER TABLE dbo.Posts DROP CONSTRAINT FK_Posts_Users_AuthorId;
GO

-- Xóa các bảng nếu đã tồn tại
IF OBJECT_ID('dbo.PartnerLogos', 'U') IS NOT NULL DROP TABLE dbo.PartnerLogos;
IF OBJECT_ID('dbo.Videos', 'U') IS NOT NULL DROP TABLE dbo.Videos;
IF OBJECT_ID('dbo.Banners', 'U') IS NOT NULL DROP TABLE dbo.Banners;
IF OBJECT_ID('dbo.Posts', 'U') IS NOT NULL DROP TABLE dbo.Posts;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;
GO

-- 2. Tạo Bảng Roles (Phân quyền)
CREATE TABLE dbo.Roles (
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200) NULL,
    CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (Id ASC)
);
GO

-- 3. Tạo Bảng Users (Tài khoản)
CREATE TABLE dbo.Users (
    Id INT IDENTITY(1,1) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NULL,
    CompanyName NVARCHAR(150) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
    CreatedAt DATETIME NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT GETDATE(),
    RoleId INT NOT NULL,
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id ASC),
    CONSTRAINT FK_Users_Roles_RoleId FOREIGN KEY (RoleId) REFERENCES dbo.Roles (Id) ON DELETE NO ACTION
);
GO

-- 4. Tạo Bảng Categories (Chuyên mục)
CREATE TABLE dbo.Categories (
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Slug NVARCHAR(150) NOT NULL,
    DisplayOrder INT NOT NULL CONSTRAINT DF_Categories_DisplayOrder DEFAULT 0,
    CONSTRAINT PK_Categories PRIMARY KEY CLUSTERED (Id ASC)
);
GO

-- 5. Tạo Bảng Posts (Tin bài)
CREATE TABLE dbo.Posts (
    Id INT IDENTITY(1,1) NOT NULL,
    Title NVARCHAR(250) NOT NULL,
    Slug NVARCHAR(300) NOT NULL,
    Summary NVARCHAR(500) NULL,
    Content NVARCHAR(MAX) NOT NULL,
    ImageUrl NVARCHAR(500) NULL,
    IsFeatured BIT NOT NULL CONSTRAINT DF_Posts_IsFeatured DEFAULT 0,
    IsApproved BIT NOT NULL CONSTRAINT DF_Posts_IsApproved DEFAULT 0,
    CreatedAt DATETIME NOT NULL CONSTRAINT DF_Posts_CreatedAt DEFAULT GETDATE(),
    CategoryId INT NOT NULL,
    AuthorId INT NOT NULL,
    CONSTRAINT PK_Posts PRIMARY KEY CLUSTERED (Id ASC),
    CONSTRAINT FK_Posts_Categories_CategoryId FOREIGN KEY (CategoryId) REFERENCES dbo.Categories (Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Posts_Users_AuthorId FOREIGN KEY (AuthorId) REFERENCES dbo.Users (Id) ON DELETE NO ACTION
);
GO

-- 6. Tạo Bảng Banners (Quảng cáo)
CREATE TABLE dbo.Banners (
    Id INT IDENTITY(1,1) NOT NULL,
    Title NVARCHAR(150) NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    RedirectUrl NVARCHAR(500) NULL,
    Position NVARCHAR(50) NOT NULL CONSTRAINT DF_Banners_Position DEFAULT 'Header',
    StartDate DATETIME NOT NULL CONSTRAINT DF_Banners_StartDate DEFAULT GETDATE(),
    EndDate DATETIME NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Banners_IsActive DEFAULT 1,
    CreatedAt DATETIME NOT NULL CONSTRAINT DF_Banners_CreatedAt DEFAULT GETDATE(),
    CONSTRAINT PK_Banners PRIMARY KEY CLUSTERED (Id ASC)
);
GO

-- 7. Tạo Bảng Videos (Thư viện Video PR)
CREATE TABLE dbo.Videos (
    Id INT IDENTITY(1,1) NOT NULL,
    Title NVARCHAR(150) NOT NULL,
    VideoUrl NVARCHAR(500) NOT NULL,
    ThumbnailUrl NVARCHAR(500) NULL,
    IsFeatured BIT NOT NULL CONSTRAINT DF_Videos_IsFeatured DEFAULT 0,
    IsActive BIT NOT NULL CONSTRAINT DF_Videos_IsActive DEFAULT 1,
    CreatedAt DATETIME NOT NULL CONSTRAINT DF_Videos_CreatedAt DEFAULT GETDATE(),
    CONSTRAINT PK_Videos PRIMARY KEY CLUSTERED (Id ASC)
);
GO

-- 8. Tạo Bảng PartnerLogos (Logo Doanh nghiệp thành viên)
CREATE TABLE dbo.PartnerLogos (
    Id INT IDENTITY(1,1) NOT NULL,
    CompanyName NVARCHAR(150) NOT NULL,
    LogoUrl NVARCHAR(500) NOT NULL,
    WebsiteUrl NVARCHAR(500) NULL,
    DisplayOrder INT NOT NULL CONSTRAINT DF_PartnerLogos_DisplayOrder DEFAULT 0,
    IsActive BIT NOT NULL CONSTRAINT DF_PartnerLogos_IsActive DEFAULT 1,
    CreatedAt DATETIME NOT NULL CONSTRAINT DF_PartnerLogos_CreatedAt DEFAULT GETDATE(),
    CONSTRAINT PK_PartnerLogos PRIMARY KEY CLUSTERED (Id ASC)
);
GO

-- ==========================================================
-- CHÈN DỮ LIỆU MẪU CƠ BẢN (SEED DATA)
-- ==========================================================

-- Chèn dữ liệu Roles
SET IDENTITY_INSERT dbo.Roles ON;
INSERT INTO dbo.Roles (Id, Name, Description) VALUES
(1, N'SuperAdmin', N'Quản trị viên tối cao hệ thống'),
(2, N'Editor', N'Biên tập viên / Phóng viên'),
(3, N'Partner', N'Tài khoản Doanh nghiệp / Đối tác');
SET IDENTITY_INSERT dbo.Roles OFF;
GO

-- Chèn dữ liệu Categories (7 danh mục truyền thống của baovethuonghieu.org)
SET IDENTITY_INSERT dbo.Categories ON;
INSERT INTO dbo.Categories (Id, Name, Slug, DisplayOrder) VALUES
(1, N'Chương trình & Sự kiện', N'chuong-trinh-su-kien', 1),
(2, N'Thương hiệu Doanh nghiệp', N'thuong-hieu-doanh-nghiep', 2),
(3, N'Xây dựng Thương hiệu', N'xay-dung-thuong-hieu', 3),
(4, N'Kinh nghiệm Tiêu dùng', N'kinh-nghiem-tieu-dung', 4),
(5, N'Góc nhìn Thương hiệu', N'goc-nhin-thuong-hieu', 5),
(6, N'Chống hàng giả', N'chong-hang-giai', 6),
(7, N'Tiêu điểm', N'tieu-diem', 7);
SET IDENTITY_INSERT dbo.Categories OFF;
GO

-- Chèn tài khoản Admin mặc định (Username: admin, Password: Thuong@123)
SET IDENTITY_INSERT dbo.Users ON;
INSERT INTO dbo.Users (Id, Username, PasswordHash, FullName, Email, CompanyName, IsActive, CreatedAt, RoleId) VALUES
(1, N'admin', N'd37e29b473701d83f8e9f00c37d22d387cce56d37671609ee5a6b643a8a8609a', N'Super Admin', N'admin@baovethuonghieu.org', N'Viện Phát triển Thương hiệu', 1, '2026-05-26 00:00:00', 1);
SET IDENTITY_INSERT dbo.Users OFF;
GO

PRINT '=== KHOI TAO DATABASE VA SEED DATA THANH CONG ===';
GO
