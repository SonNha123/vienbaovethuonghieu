-- ==========================================================
-- SCRIPT CHÈN DỮ LIỆU MẪU CHUYÊN NGHIỆP CHO CỔNG THÔNG TIN PR
-- Tên Database: KDTM_BaoVeThuongHieu_New
-- Nguồn dữ liệu: Phục dựng giao diện theo nguoitieudung.org.vn & baovethuonghieu.org
-- ==========================================================

USE KDTM_BaoVeThuongHieu_New;
GO

-- 1. XÓA BỚT DỮ LIỆU CŨ (NẾU CÓ) ĐỂ TRÁNH TRÙNG LẶP (Giữ lại Roles, Users và Categories gốc)
DELETE FROM dbo.Posts;
DELETE FROM dbo.Banners;
DELETE FROM dbo.Videos;
DELETE FROM dbo.PartnerLogos;
GO

-- 2. CHÈN DỮ LIỆU BANNERS QUẢNG CÁO (BANNER NỔI 2 BÊN VÀ BANNER DỌC)
-- Position có các giá trị: 'Header', 'Sidebar', 'Body'
-- Chúng ta chèn banner Trái, banner Phải, banner Top và các banner thân bài
INSERT INTO dbo.Banners (Title, ImageUrl, RedirectUrl, Position, StartDate, EndDate, IsActive, CreatedAt) VALUES
(N'Banner Dọc Bên Trái - Chống Hàng Giả Bảo Vệ Người Tiêu Dùng', 'https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?auto=format&fit=crop&w=400&q=80', 'https://baovethuonghieu.org', 'Sidebar', GETDATE(), DATEADD(year, 1, GETDATE()), 1, GETDATE()),
(N'Banner Dọc Bên Phải - Doanh Nghiệp Hàng Việt Nam Chất Lượng Cao', 'https://images.unsplash.com/photo-1607604276583-eef5d076aa5f?auto=format&fit=crop&w=400&q=80', 'https://baovethuonghieu.org', 'Sidebar', GETDATE(), DATEADD(year, 1, GETDATE()), 1, GETDATE()),
(N'Banner Ngang Khảo Sát Thương Hiệu Tiêu Biểu & Diễn Đàn Kinh Tế 2026', 'https://images.unsplash.com/photo-1618005198143-e5283b519a7f?auto=format&fit=crop&w=1200&q=80', 'https://baovethuonghieu.org', 'Header', GETDATE(), DATEADD(year, 1, GETDATE()), 1, GETDATE()),
(N'Banner Quảng Cáo Sự Kiện Trao Giải Thương Hiệu Vàng Đông Nam Á', 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?auto=format&fit=crop&w=1200&q=80', 'https://baovethuonghieu.org', 'Body', GETDATE(), DATEADD(year, 1, GETDATE()), 1, GETDATE()),
(N'Banner Quảng Cáo Công Bố Kết Quả Hàng Việt Tốt - Quyền Lợi Người Tiêu Dùng', 'https://images.unsplash.com/photo-1531403009284-440f080d1e12?auto=format&fit=crop&w=1200&q=80', 'https://baovethuonghieu.org', 'Body', GETDATE(), DATEADD(year, 1, GETDATE()), 1, GETDATE());
GO

-- 3. CHÈN DỮ LIỆU VIDEOS PR DOANH NGHIỆP & CẢNH BÁO TIÊU DÙNG
INSERT INTO dbo.Videos (Title, VideoUrl, ThumbnailUrl, IsFeatured, IsActive, CreatedAt) VALUES
(N'Lễ công bố và khảo sát Thương hiệu xuất sắc nhất năm 2026', 'https://www.youtube.com/embed/dQw4w9WgXcQ', 'https://images.unsplash.com/photo-1511578314322-379afb476865?auto=format&fit=crop&w=600&q=80', 1, 1, GETDATE()),
(N'Cảnh giác vấn nạn phân bón giả tàn phá mùa màng của nông dân miền Tây', 'https://www.youtube.com/embed/dQw4w9WgXcQ', 'https://images.unsplash.com/photo-1593113598332-cd288d649433?auto=format&fit=crop&w=600&q=80', 0, 1, GETDATE()),
(N'Giải pháp công nghệ chống hàng giả tối ưu bằng mã QR Code phủ cào', 'https://www.youtube.com/embed/dQw4w9WgXcQ', 'https://images.unsplash.com/photo-1526374965328-7f61d4dc18c5?auto=format&fit=crop&w=600&q=80', 0, 1, GETDATE());
GO

-- 4. CHÈN LOGO CÁC DOANH NGHIỆP THÀNH VIÊN Ở CHÂN TRANG (PARTNERS)
INSERT INTO dbo.PartnerLogos (CompanyName, LogoUrl, WebsiteUrl, DisplayOrder, IsActive, CreatedAt) VALUES
(N'iCheck Việt Nam', 'https://images.unsplash.com/photo-1614741118887-7a4ee193a5fa?auto=format&fit=crop&w=150&q=80', 'https://icheck.com.vn', 1, 1, GETDATE()),
(N'Viettel Telecom', 'https://images.unsplash.com/photo-1563986768609-322da13575f3?auto=format&fit=crop&w=150&q=80', 'https://viettel.vn', 2, 1, GETDATE()),
(N'Tập đoàn Điện lực Việt Nam EVN', 'https://images.unsplash.com/photo-1518770660439-4636190af475?auto=format&fit=crop&w=150&q=80', 'https://evn.com.vn', 3, 1, GETDATE()),
(N'Thương hiệu Vinamilk', 'https://images.unsplash.com/photo-1550583724-b2692b85b150?auto=format&fit=crop&w=150&q=80', 'https://vinamilk.com.vn', 4, 1, GETDATE()),
(N'Tập đoàn Petrolimex', 'https://images.unsplash.com/photo-1538333581680-95617159c98b?auto=format&fit=crop&w=150&q=80', 'https://petrolimex.com.vn', 5, 1, GETDATE()),
(N'Vietcombank', 'https://images.unsplash.com/photo-1559526324-4b87b5e36e44?auto=format&fit=crop&w=150&q=80', 'https://vietcombank.com.vn', 6, 1, GETDATE());
GO

-- 5. CHÈN DỮ LIỆU TIN BÀI (POSTS) CHO 7 CHUYÊN MỤC
-- AuthorId = 1 (Tài khoản admin mặc định)
-- Chuyên mục (CategoryId):
-- 1: Chương trình & Sự kiện (chuong-trinh-su-kien)
-- 2: Thương hiệu Doanh nghiệp (thuong-hieu-doanh-nghiep)
-- 3: Xây dựng Thương hiệu (xay-dung-thuong-hieu)
-- 4: Kinh nghiệm Tiêu dùng (kinh-nghiem-tieu-dung)
-- 5: Góc nhìn Thương hiệu (goc-nhin-thuong-hieu)
-- 6: Chống hàng giả (chong-hang-giai)
-- 7: Tiêu điểm (tieu-diem)

-- ==========================================================
-- CHUYÊN MỤC 7: TIÊU ĐIỂM (Được đưa vào Slider chính và nổi bật đầu trang)
-- ==========================================================
INSERT INTO dbo.Posts (Title, Slug, Summary, Content, ImageUrl, IsFeatured, IsApproved, CreatedAt, CategoryId, AuthorId) VALUES
(
    N'Khởi tố hình sự vụ án sản xuất phân bón giả quy mô cực lớn tại miền Tây',
    'khoi-to-hinh-su-vu-an-san-xuat-phan-bon-gia-quy-mo-lon-521',
    N'Lực lượng chức năng vừa triệt phá đường dây sản xuất phân bón giả quy mô lớn, thu giữ hơn 500 tấn sản phẩm kém chất lượng và tiến hành khởi tố vụ án hình sự để điều tra làm rõ.',
    N'<p><strong>Ngày 26/05</strong>, Tổng cục Quản lý thị trường phối hợp với lực lượng An ninh Kinh tế các tỉnh vùng đồng bằng sông Cửu Long đã tổ chức kiểm tra đột xuất và phát hiện một đường dây sản xuất, phân phối phân bón giả quy mô cực kỳ lớn.</p><p>Tại hiện trường, lực lượng chức năng đã thu giữ hơn 500 tấn phân bón các loại đang được đóng bao nhãn mác ngoại nhập giả mạo cùng hàng nghìn bao bì rỗng chưa sử dụng. Điều tra ban đầu cho thấy, các đối tượng đã mua nguyên liệu hóa chất giá rẻ sau đó pha trộn theo tỷ lệ không đạt chuẩn rồi đóng gói thành sản phẩm cao cấp bán ra thị trường với giá cao.</p><p>Hành vi này đã gây thiệt hại nghiêm trọng cho mùa màng và kinh tế của hàng ngàn hộ nông dân. Hiện Cơ quan Cảnh sát điều tra đã ký quyết định khởi tố hình sự vụ án để tiếp tục đấu tranh và làm rõ vai trò của các đối tượng liên quan.</p>',
    'https://images.unsplash.com/photo-1541872703-74c5e44368f9?auto=format&fit=crop&w=800&q=80',
    1, 1, GETDATE(), 7, 1
),
(
    N'Cảnh báo: Thực phẩm chức năng giả mạo thương hiệu lớn hoành hành mạnh trên MXH',
    'canh-bao-thuc-pham-chuc-nang-gia-hoanh-hanh-tren-mxh-882',
    N'Các loại thực phẩm chức năng, collagen, viên uống trắng da giả mạo nhãn hiệu nổi tiếng từ Mỹ, Nhật Bản, Hàn Quốc đang được rao bán tràn lan qua các phiên Livestream TikTok và Facebook.',
    N'<p>Vấn nạn thực phẩm chức năng giả mạo đang ở mức báo động đỏ khi các đối tượng làm giả vô cùng tinh vi từ bao bì, tem chống giả cho tới các mã vạch truy xuất nguồn gốc. Sản phẩm giả thường chứa các hoạt chất cấm hoặc hàm lượng thấp không có tác dụng y tế, trực tiếp đe dọa đến sức khỏe của người tiêu dùng.</p><p>Các bác sĩ khuyến cáo người tiêu dùng không nên mua các sản phẩm y tế, thực phẩm bảo vệ sức khỏe không rõ nguồn gốc hoặc có giá rẻ bất thường trên mạng xã hội. Nên tìm mua ở các nhà thuốc lớn hoặc showroom chính hãng có hóa đơn chứng từ đầy đủ.</p>',
    'https://images.unsplash.com/photo-1584017911766-d451b3d0e843?auto=format&fit=crop&w=800&q=80',
    1, 1, DATEADD(hour, -2, GETDATE()), 7, 1
),
(
    N'Khai mạc tuần lễ trưng bày nhận diện hàng thật - hàng giả tại thành phố Hà Nội',
    'khai-mac-tuan-le-trung-bay-nhan-dien-hang-that-hang-gia-112',
    N'Sự kiện nhằm nâng cao nhận thức của người tiêu dùng và hỗ trợ doanh nghiệp bảo vệ thương hiệu trước làn sóng xâm phạm sở hữu trí tuệ đang ngày càng diễn biến phức tạp.',
    N'<p>Hội chợ trưng bày nhận diện hàng thật - hàng giả do Cục Quản lý thị trường phối hợp tổ chức đã thu hút hàng ngàn lượt khách tham quan ngay trong ngày khai mạc đầu tiên. Tại đây, đại diện các thương hiệu lớn như Nike, Adidas, Casio, cùng nhiều thương hiệu Việt Nam đã trực tiếp hướng dẫn người dân cách thức phân biệt hàng thật và hàng giả bằng mắt thường và thiết bị hỗ trợ.</p><p>Ban tổ chức mong muốn thông qua sự kiện này, người dân sẽ có thêm kiến thức tự bảo vệ quyền lợi tiêu dùng của chính mình, đồng thời xây dựng văn hóa nói không với hàng lậu, hàng giả.</p>',
    'https://images.unsplash.com/photo-1540575467063-178a50c2df87?auto=format&fit=crop&w=800&q=80',
    1, 1, DATEADD(hour, -5, GETDATE()), 7, 1
);
GO

-- ==========================================================
-- CHUYÊN MỤC 5: GÓC NHÌN THƯƠNG HIỆU
-- ==========================================================
INSERT INTO dbo.Posts (Title, Slug, Summary, Content, ImageUrl, IsFeatured, IsApproved, CreatedAt, CategoryId, AuthorId) VALUES
(
    N'Bảo hộ sở hữu trí tuệ: Lá chắn thép giúp doanh nghiệp Việt vươn tầm quốc tế',
    'bao-ho-so-huu-tri-tue-la-chan-thep-doanh-nghiep-291',
    N'Đăng ký nhãn hiệu và bảo hộ quyền sở hữu trí tuệ là bước đi sống còn giúp doanh nghiệp tránh khỏi các vụ tranh chấp pháp lý phức tạp khi bước chân ra biển lớn.',
    N'<p>Nhiều thương hiệu nổi tiếng của Việt Nam thời gian qua đã bị các đối tượng nước ngoài đăng ký bảo hộ trước ở thị trường quốc tế, gây tổn thất hàng triệu USD để đòi lại quyền sở hữu. Điều này cho thấy tầm quan trọng của việc chủ động đăng ký bảo hộ sở hữu trí tuệ ngay từ giai đoạn đầu phát triển thương hiệu.</p><p>Các chuyên gia pháp lý nhấn mạnh bảo hộ nhãn hiệu không chỉ là công cụ pháp lý bảo vệ sản phẩm mà còn là tài sản vô hình có giá trị gia tăng cực lớn cho doanh nghiệp.</p>',
    'https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?auto=format&fit=crop&w=800&q=80',
    0, 1, DATEADD(day, -1, GETDATE()), 5, 1
),
(
    N'Chiến lược xây dựng thương hiệu số bền vững trong kỷ nguyên trí tuệ nhân tạo (AI)',
    'chien-luoc-xay-dung-thuong-hieu-so-ky-nguyen-ai-732',
    N'Ứng dụng AI trong phân tích hành vi khách hàng đang mở ra cơ hội đột phá để các doanh nghiệp định hình thương hiệu cá nhân hóa sâu sắc đến từng đối tượng tiêu dùng.',
    N'<p>Trong bối cảnh chuyển đổi số diễn ra mạnh mẽ, việc xây dựng thương hiệu không còn giới hạn ở các phương thức truyền thống. Doanh nghiệp cần xây dựng chiến lược đa kênh linh hoạt, tận dụng sức mạnh của dữ liệu lớn và AI để tối ưu trải nghiệm khách hàng, mang lại sự kết nối chân thực và nhất quán nhất.</p>',
    'https://images.unsplash.com/photo-1516321318423-f06f85e504b3?auto=format&fit=crop&w=800&q=80',
    0, 1, DATEADD(day, -2, GETDATE()), 5, 1
);
GO

-- ==========================================================
-- CHUYÊN MỤC 6: CHỐNG HÀNG GIẢ
-- ==========================================================
INSERT INTO dbo.Posts (Title, Slug, Summary, Content, ImageUrl, IsFeatured, IsApproved, CreatedAt, CategoryId, AuthorId) VALUES
(
    N'Triệt phá kho hàng giả mạo nhãn hiệu xa xỉ Hermes, Gucci trị giá hàng chục tỷ tại Hà Nội',
    'triet-pha-kho-hang-gia-nhan-hieu-xa-xi-hermes-gucci-381',
    N'Hàng nghìn sản phẩm túi xách, ví da, thắt lưng giả mạo các nhãn hiệu thời trang cao cấp đã bị lực lượng Quản lý thị trường bắt quả tang tại một kho hàng trung chuyển lớn.',
    N'<p>Các sản phẩm giả mạo tại đây được gia công tinh xảo, sử dụng da tổng hợp chất lượng khá tốt và đi kèm đầy đủ hộp, thẻ bảo hành giả nhằm đánh lừa người mua. Chủ kho hàng khai nhận đã phân phối số lượng lớn sản phẩm này thông qua các trang thương mại điện tử và tài khoản mạng xã hội cá nhân.</p><p>Toàn bộ lô hàng đã bị niêm phong để xử lý nghiêm theo quy định pháp luật.</p>',
    'https://images.unsplash.com/photo-1541872703-74c5e44368f9?auto=format&fit=crop&w=800&q=80',
    0, 1, DATEADD(day, -1, GETDATE()), 6, 1
),
(
    N'Giải pháp mã hóa QR Code chống giả: Lựa chọn hàng đầu bảo vệ hàng hóa nội địa',
    'giai-phap-ma-hoa-qr-code-chong-gia-bao-ve-hang-hoa-292',
    N'Ứng dụng công nghệ tem chống giả kỹ thuật số thông minh giúp người tiêu dùng truy xuất nguồn gốc sản phẩm nhanh chóng bằng điện thoại di động.',
    N'<p>Sự phát triển của công nghệ in ấn khiến các loại tem giấy chống giả thông thường dễ dàng bị làm giả. Công nghệ chống giả thế hệ mới tích hợp mã QR động giới hạn lượt quét và mã hóa dữ liệu trên hệ thống blockchain đang được đánh giá là giải pháp tối ưu giúp ngăn chặn hàng giả triệt để, đồng thời hỗ trợ doanh nghiệp quản lý kênh phân phối hiệu quả.</p>',
    'https://images.unsplash.com/photo-1526374965328-7f61d4dc18c5?auto=format&fit=crop&w=800&q=80',
    0, 1, DATEADD(day, -3, GETDATE()), 6, 1
);
GO

-- ==========================================================
-- CHUYÊN MỤC 4: KINH NGHIỆM TIÊU DÙNG
-- ==========================================================
INSERT INTO dbo.Posts (Title, Slug, Summary, Content, ImageUrl, IsFeatured, IsApproved, CreatedAt, CategoryId, AuthorId) VALUES
(
    N'Bí quyết nhận diện mỹ phẩm chính hãng và hàng fake chuẩn xác chỉ trong 1 phút',
    'bi-quyet-nhan-dien-my-pham-chinh-hang-va-fake-883',
    N'Dưới đây là những mẹo nhỏ cực kỳ hữu ích giúp các chị em dễ dàng nhận biết son môi, kem dưỡng da thật giả dựa trên thiết kế bao bì và hương thơm.',
    N'<p>Mỹ phẩm giả là tác nhân hàng đầu gây dị ứng da, hỏng da mặt nghiêm trọng cho người dùng. Hãy chú ý đến chi tiết in ấn trên vỏ hộp: chữ in trên hàng thật luôn sắc nét, không bị nhòe, vỏ hộp chắc chắn. Ngoài ra, hương thơm của hàng thật luôn dịu nhẹ, tự nhiên trong khi hàng giả thường nồng mùi hóa chất tổng hợp.</p>',
    'https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?auto=format&fit=crop&w=800&q=80',
    0, 1, DATEADD(day, -2, GETDATE()), 4, 1
),
(
    N'Mẹo mua sắm trực tuyến an toàn: Làm gì khi nhận phải hàng giả, hàng nhái từ shipper?',
    'meo-mua-sam-truc-tuyen-an-toan-lam-gi-khi-nhan-hang-gia-129',
    N'Hướng dẫn quy trình khiếu nại, trả hàng hoàn tiền trên các sàn TMĐT lớn như Shopee, Lazada khi phát hiện sản phẩm nhận được là hàng giả.',
    N'<p>Khi mua hàng online, bạn luôn được khuyến khích quay lại video đồng kiểm hoặc video khui hộp sản phẩm. Đây là bằng chứng pháp lý quan trọng nhất để các sàn TMĐT tiến hành xử lý yêu cầu trả hàng hoàn tiền. Tuyệt đối không bấm nút "Đã nhận được hàng" nếu bạn chưa kiểm tra kỹ chất lượng sản phẩm.</p>',
    'https://images.unsplash.com/photo-1556742044-3c52d6e88c62?auto=format&fit=crop&w=800&q=80',
    0, 1, DATEADD(day, -4, GETDATE()), 4, 1
);
GO

-- ==========================================================
-- CHUYÊN MỤC 2: THƯƠNG HIỆU DOANH NGHIỆP
-- ==========================================================
INSERT INTO dbo.Posts (Title, Slug, Summary, Content, ImageUrl, IsFeatured, IsApproved, CreatedAt, CategoryId, AuthorId) VALUES
(
    N'Vinamilk dẫn đầu danh sách thương hiệu sữa được người tiêu dùng Việt tin dùng nhất',
    'vinamilk-dan-dau-danh-sach-thuong-hieu-sua-tin-dung-302',
    N'Nhờ không ngừng cải tiến công nghệ sản xuất và giữ vững chất lượng nguồn sữa tươi sạch đạt chuẩn quốc tế, Vinamilk tiếp tục duy trì vị thế thống trị.',
    N'<p>Với hơn 45 năm hình thành và phát triển, Vinamilk đã xây dựng hệ thống trang trại bò sữa đạt chuẩn GlobalGAP lớn nhất Đông Nam Á, mang đến cho người tiêu dùng những dòng sản phẩm sữa chất lượng cao, an toàn tuyệt đối. Thương hiệu cũng đã xuất khẩu sản phẩm thành công tới hơn 50 quốc gia trên thế giới.</p>',
    'https://images.unsplash.com/photo-1550583724-b2692b85b150?auto=format&fit=crop&w=800&q=80',
    0, 1, DATEADD(day, -1, GETDATE()), 2, 1
),
(
    N'Tập đoàn Điện lực Việt Nam EVN: Đi đầu trong chuyển đổi số và nâng cao dịch vụ khách hàng',
    'tap-doan-dien-luc-viet-nam-evn-di-dau-trong-chuyen-doi-so-928',
    N'EVN đã triển khai 100% dịch vụ điện trực tuyến cấp độ 4, mang đến sự tiện lợi tối đa và minh bạch hóa thông tin sử dụng điện cho mọi người dân.',
    N'<p>Thông qua ứng dụng chăm sóc khách hàng trên điện thoại di động và cổng dịch vụ công quốc gia, khách hàng sử dụng điện của EVN giờ đây có thể thực hiện mọi thủ tục từ ký hợp đồng mua bán điện, thanh toán hóa đơn cho đến tra cứu chỉ số điện năng tiêu thụ hàng ngày một cách vô cùng nhanh chóng và tiện lợi.</p>',
    'https://images.unsplash.com/photo-1518770660439-4636190af475?auto=format&fit=crop&w=800&q=80',
    0, 1, DATEADD(day, -3, GETDATE()), 2, 1
);
GO

-- ==========================================================
-- CHUYÊN MỤC 1: CHƯƠNG TRÌNH & SỰ KIỆN
-- ==========================================================
INSERT INTO dbo.Posts (Title, Slug, Summary, Content, ImageUrl, IsFeatured, IsApproved, CreatedAt, CategoryId, AuthorId) VALUES
(
    N'Công bố kết quả Khảo sát Thương hiệu Vàng Việt Nam tiêu biểu năm 2026',
    'cong-bo-ket-qua-khao-sat-thuong-hieu-vang-viet-nam-2026-881',
    N'Chương trình nhằm tôn vinh các doanh nghiệp Việt Nam có thành tích xuất sắc trong việc xây dựng hình ảnh thương hiệu uy tín, chất lượng sản phẩm dịch vụ cao.',
    N'<p>Ban Tổ chức chương trình khảo sát Thương hiệu Vàng Việt Nam đã chính thức công bố danh sách 100 thương hiệu tiêu biểu nhất năm. Các doanh nghiệp được vinh danh đều đáp ứng đầy đủ các tiêu chí khắt khe về năng lực tài chính, đóng góp ngân sách nhà nước, các hoạt động an sinh xã hội và đặc biệt là nhận được sự tín nhiệm lớn từ cộng đồng người tiêu dùng toàn quốc.</p>',
    'https://images.unsplash.com/photo-1540575467063-178a50c2df87?auto=format&fit=crop&w=800&q=80',
    0, 1, DATEADD(day, -2, GETDATE()), 1, 1
),
(
    N'Hội thảo quốc tế: Tìm kiếm giải pháp ngăn ngừa hàng giả trong chuỗi cung ứng logistics toàn cầu',
    'hoi-thao-quoc-te-tim-kiem-giai-phap-ngan-ngua-hang-gia-921',
    N'Hội thảo đã quy tụ nhiều diễn giả, nhà quản lý và chuyên gia công nghệ trong và ngoài nước cùng thảo luận về các giải pháp bảo vệ luồng hàng hóa.',
    N'<p>Vấn nạn hàng giả xuyên biên giới đang đặt ra nhiều thách thức lớn cho các doanh nghiệp xuất nhập khẩu. Tại hội thảo, nhiều công nghệ mới như mã vạch thông minh bảo mật cao, định danh sản phẩm bằng chip RFID cùng các giải pháp liên kết hải quan đa quốc gia đã được giới thiệu nhằm tối ưu hóa khả năng giám sát hàng hóa từ nhà máy tới tay người tiêu dùng cuối cùng.</p>',
    'https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?auto=format&fit=crop&w=800&q=80',
    0, 1, DATEADD(day, -4, GETDATE()), 1, 1
);
GO

-- ==========================================================
-- CHUYÊN MỤC 3: XÂY DỰNG THƯƠNG HIỆU
-- ==========================================================
INSERT INTO dbo.Posts (Title, Slug, Summary, Content, ImageUrl, IsFeatured, IsApproved, CreatedAt, CategoryId, AuthorId) VALUES
(
    N'Làm thế nào để một thương hiệu startup Việt khẳng định được chỗ đứng trên thị trường?',
    'lam-the-nao-de-startup-viet-khang-dinh-cho-dung-thuong-hieu-771',
    N'Bài viết chia sẻ những kinh nghiệm thực chiến đắt giá từ các nhà sáng lập startup thành công trong việc định vị sản phẩm và tối ưu ngân sách truyền thông.',
    N'<p>Đối với một startup có nguồn lực hạn chế, việc cạnh tranh trực tiếp với các ông lớn bằng ngân sách quảng cáo khổng lồ là điều không thể. Thay vào đó, startup cần tập trung vào việc giải quyết triệt để một nỗi đau cụ thể của khách hàng, tạo ra sản phẩm chất lượng thực sự khác biệt và kể một câu chuyện thương hiệu truyền cảm hứng chạm đến cảm xúc người tiêu dùng.</p>',
    'https://images.unsplash.com/photo-1522071820081-009f0129c71c?auto=format&fit=crop&w=800&q=80',
    0, 1, DATEADD(day, -3, GETDATE()), 3, 1
),
(
    N'Tầm quan trọng của thiết kế bộ nhận diện thương hiệu chuyên nghiệp cho doanh nghiệp vừa và nhỏ',
    'tam-quan-trong-cua-thiet-ke-bo-nhan-dien-thuong-hieu-sme-221',
    N'Bộ nhận diện thương hiệu ấn tượng là cầu nối quan trọng đầu tiên giúp doanh nghiệp thu hút sự chú ý của đối tác và khách hàng tiềm năng.',
    N'<p>Một logo chuyên nghiệp, phối màu hài hòa và hệ thống ấn phẩm văn phòng đồng bộ sẽ tạo nên ấn tượng sâu sắc về sự chỉn chu và uy tín của doanh nghiệp trong mắt khách hàng. Các doanh nghiệp SME nên đầu tư ngân sách hợp lý để thiết kế bộ nhận diện thương hiệu chuẩn hóa ngay từ đầu để xây dựng hình ảnh lâu dài.</p>',
    'https://images.unsplash.com/photo-1486406146926-c627a92ad1ab?auto=format&fit=crop&w=800&q=80',
    0, 1, DATEADD(day, -5, GETDATE()), 3, 1
);
GO

PRINT '=== CHÈN DỮ LIỆU MẪU MỚI THÀNH CÔNG ===';
