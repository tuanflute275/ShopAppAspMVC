CREATE DATABASE ShopAppAspWeb;
GO

USE ShopAppAspWeb;
GO

CREATE TABLE Accounts (
  userId int primary key identity,
  userName nvarchar(255) NOT NULL,
  userPassword varchar(255) DEFAULT NULL,
  userFullName nvarchar(255) DEFAULT NULL,
  userPhone nvarchar(255) DEFAULT NULL,
  userAddress nvarchar(255) DEFAULT NULL,
  userAvatar nvarchar(255) DEFAULT NULL,
  userEmail nvarchar(255) DEFAULT NULL,
  userGender int DEFAULT 1,-- 1 Nam, 0 Nữ
  userActive int DEFAULT 1, -- 1 Hoạt động, 0 Tạm khóa
  userRole int DEFAULT 0 -- 1 Admin, 0 User
  
)
GO

CREATE TABLE Categories (
  categoryId int primary key identity,
  categoryName nvarchar(255) DEFAULT NULL,
  categorySlug nvarchar(255) DEFAULT NULL,
  categoryStatus int DEFAULT NULL
) 
GO

CREATE TABLE Products (
  productId int primary key identity,
  productCategoryId int,
  productDescription ntext DEFAULT NULL,
  productImage nvarchar(255) DEFAULT NULL,
  productName nvarchar(255) DEFAULT NULL,
  productPrice float DEFAULT NULL,
  productSalePrice float DEFAULT NULL,
  productStatus int DEFAULT NULL,
  FOREIGN KEY (productCategoryId) REFERENCES Categories(categoryId)
) 
GO

CREATE TABLE Blogs (
  blogId int primary key identity,
  blogDescription ntext DEFAULT NULL,
  blogImage nvarchar(255) DEFAULT NULL,
  blogName nvarchar(255) DEFAULT NULL
)
GO

CREATE TABLE Carts (
  cartId int primary key identity,
  quantity int DEFAULT NULL,
  totalAmount int DEFAULT NULL,
  productId int,
  userId int,
  FOREIGN KEY (productId) REFERENCES Products(productId),
  FOREIGN KEY (userId) REFERENCES Accounts(userId)
) 
GO

CREATE TABLE Orders (
  orderId int primary key identity,
  orderAddress nvarchar(255) DEFAULT NULL,
  orderAmount float DEFAULT NULL,
  orderDate DATETIME DEFAULT GETDATE(),
  orderEmail nvarchar(255) DEFAULT NULL,
  orderFullName nvarchar(255) DEFAULT NULL,
  orderNote ntext DEFAULT NULL,
  orderPaymentMethods nvarchar(255) DEFAULT NULL,
  orderPhone nvarchar(255) DEFAULT NULL,
  orderStatus int DEFAULT NULL,
  orderStatusPayment varchar(255) DEFAULT NULL,
  userId int,
  FOREIGN KEY (userId) REFERENCES Accounts(userId)
)
GO

CREATE TABLE OrderDetail (
  orderDetailId int primary key identity,
  orderId int,
  productId int,
  price float DEFAULT 0,
  quantity int DEFAULT 0,
  totalMoney float DEFAULT 0
)
GO

CREATE TABLE Logs (
  logId int primary key identity,
  userName nvarchar(255) DEFAULT NULL,
  workTation nvarchar(255) DEFAULT NULL,
  request nvarchar(255) DEFAULT NULL,
  response ntext DEFAULT NULL,
  ipAdress nvarchar(255) DEFAULT NULL,
  timeLogin DATETIME DEFAULT NULL,
  timeLogout DATETIME DEFAULT NULL,
  timeActionRequest DATETIME DEFAULT NULL,
)
GO


-- INSERT DATA TO TABLE ACCOUNTS
INSERT INTO Accounts (userName, userActive, userAddress, userAvatar, userEmail, userFullName, userGender, userPassword, userPhone) VALUES
('admin', 0, 'Ba Vì - Hà Nội', 'http://res.cloudinary.com/dxo2y5smg/image/upload/v1715758983/avatar/w7h3fidb4hrihitihcd0.jpg', 'admin@gmail.com', 'admin', 1, '123456', '0982467073'),
('tuanflute', 0, 'Ha Noi', 'http://res.cloudinary.com/dxo2y5smg/image/upload/v1715759179/avatar/dlog2z2uulqsqaeikjhd.jpg', 'tuanflute27052004@gmail.com', 'tuanflute275', 1, '123456', '0982467073'),
('user', 0, 'Ba Vì - Hà Nội', NULL, 'user@gmail.com', 'user', 0, '123456', '0386564543');

-- INSERT DATA TO TABLE CATEGORIES
INSERT INTO Categories (categoryName,categorySlug, categoryStatus) VALUES
(N'ÁO KHOÁC', 'ao-khoac', 1),
(N'ÁO THUN','ao-thun', 1),
(N'ÁO POLO','ao-polo', 1),
(N'ÁO SƠ MI','ao-so-mi', 1),
(N'ÁO CHUI ĐẦU/ HOODIE','ao-chui-dau-hoodie', 1),
(N'QUẦN JOGGER','quan-jogger', 1),
(N'QUẦN SHORTS','quan-shorts', 1),
(N'QUẦN DÀI NỮ', 'quan-dai-nu',1),
(N'VÁY','vay', 1),
(N'GIÀY DÉP','giay-dep', 1),
(N'TÚI VÍ','tui-vi',1),
(N'MŨ NÓN','mu-non', 1);
GO

-- INSERT DATA TO TABLE PRODUCTS
INSERT INTO Products(productCategoryId, productDescription, productImage, productName, productPrice, productSalePrice, productStatus) VALUES
(12, N'Lấy cảm hứng từ hiệp hội bóng chày MLB, chiếc nón bucket Fur sẽ mang đến cho bạn thiết kế hiện đại, trẻ trung với logo thêu sắc nét nổi bật trên nền gam màu tương phản. Có điểm nhấn là phần viền lông mềm mịn, mang đến sự sang trọng, thời thượng, item này ', 'https://product.hstatic.net/200000642007/product/50bks_3ahtf0936_1_c9cf06502bce48e7a8fe0acb0537c0d8_3e0c7c47ce1a4e178dbd374f3d41f3ae_master.jpg', 'Nón bucket unisex thời trang Fur', 2090000, 0, 1),
(12, N'Lấy cảm hứng từ hiệp hội bóng chày MLB, chiếc nón bucket Fur sẽ mang đến cho bạn thiết kế hiện đại, trẻ trung với logo thêu sắc nét nổi bật trên nền gam màu tương phản. Có điểm nhấn là phần viền lông mềm mịn, mang đến sự sang trọng, thời thượng, item này ', 'https://product.hstatic.net/200000642007/product/43bgl_3ahtf0936_1_f68178ff70a741d4a1aeb4f1e3fffa92_b700f151391b4689988dda1700538e44_master.jpg', 'Nón bucket unisex thời trang Fur', 2090000, 1990000, 1),
(12, N'Bứt phá khỏi những thiết kế đơn điệu, chiếc nón bucket Classic Monogram sẽ mang đến làn gió mới cho phong cách thời trang của bạn. Là sự kết hợp độc đáo giữa họa tiết monogram cùng màu sắc tương phản tạo nên điểm nhấn ấn tượng, chiếc nón sẽ là must-have-i', 'https://product.hstatic.net/200000642007/product/50sbd_3ahtm124n_1_68feacb7877347779c486c655b5f6577_85f1fef4198c421293101fd8e5115298_master.jpg', 'Nón bucket unisex thời trang Classic Monogram', 2190000, 2000000, 1),
(12, N'Chiếc nón bucket Big Classic Monogram sẽ không bao giờ làm các tín đồ MLB thất vọng với thiết kế độc đáo đậm chất retro. Những dòng chữ thêu độc quyền ở chính giữa nón kết hợp cùng gam màu trung tính đã tạo nên một phong cách độc đáo mang tính biểu tượng,', 'https://product.hstatic.net/200000642007/product/50sal_3ahtv013n_1_6c7c9ec0fe3246b1a3b63caae1081573_d4c9bec9fe554487a1fc797d6170aa86_master.jpg', 'Nón bucket unisex Big Classic Monogram', 1590000, 1290000, 1),
(12, N'Một chiếc nón bucket sẽ là điểm nhấn hoàn hảo cho outfit những ngày hè sắp tới của bạn. Với hoạ tiết monogram đặc trưng cùng chi tiết logo nổi bật ở phía trước, ánh nắng cũng sẽ không thể làm lu mờ bạn. Sẵn sàng khuấy động mùa hè thôi! ', 'https://product.hstatic.net/200000642007/product/07gnl_3ahtm0933_1_930734d6b35d4989ac057a393d3b5a50_a62b44e8f64b4662bb8f4e2862faf0d4_master.jpg', 'Nón bucket unisex Gradient Monogram', 1890000, 1290000, 1);
GO

-- INSERT DATA TO TABLE BLOGS
INSERT INTO Blogs (blogDescription, blogImage, blogName) VALUES
(N'Chắc rằng không ít chàng trai cảm thấy việc chụp hình là vô cùng khó khăn. Đừng lo, Coolmate sẽ hướng dẫn một số cách tạo dáng chụp ảnh nam đẹp và cool ngầu.nChắc rằng không ít chàng trai cảm thấy việc chụp hình là vô cùng khó khăn. Tâm lý ngại ống kính m', 'https://media.coolmate.me/cdn-cgi/image/quality=80,format=auto/uploads/November2023/tao-dang-chup-anh-nam-dep.jpg', N'Bí kíp tạo dáng chụp ảnh nam đẹp ngầu như mẫu nam Hàn Quốc'),
(N'Tìm hiểu bí kíp phối đồ tập gym nam cực chất để mang lại sự thoải mái và phong cách cho các chàng trai. Xem các gợi ý và nguyên tắc phối đồ tốt nhất để tạo nên một phong cách tập luyện ấn tượng.nTrang phục tập gym nam là một tiêu chí quan trọng được nhiều', 'https://media.coolmate.me/cdn-cgi/image/quality=80,format=auto/uploads/May2023/bi-kip-phoi-do-tap-gym-nam-cuc-chat-lai-thoai-mai-cho-chang-1823_747_10.jpg', N'Bí kíp phối đồ tập gym nam cực chất lại thoải mái cho chàng'),
(N'Nước Việt Nam ta đã trải qua hơn 4000 năm với hàng ngàn di tích lịch sử nổi tiếng như một chứng tích hào hùng cho tinh thần bất khuất, kiên trung. Hãy cùng Coolmate “điểm danh” top 19+ di tích lịch sử Việt Nam đẹp và nổi tiếng nhất cả nước nhé.nViệt Nam đ', 'https://media.coolmate.me/cdn-cgi/image/quality=80,format=auto/uploads/January2022/di-tich-lich-su-viet-nam-11_63.png', N'Tổng hợp 19+ di tích lịch sử Việt Nam đẹp nổi tiếng qua năm tháng'),
(N'Nếu bạn muốn tìm kiếm những trang phục "bao thoải mái" trong thời tiết nóng bức? Tham khảo ngay top 10 sản phẩm Ex-dry Coolmate trong bài viết nhé!nMùa hè đến, những sản phẩm hướng đến sự thoải mái ngày càng được quan tâm nhiều hơn. Nếu bạn cũng là một ng', 'https://media.coolmate.me/cdn-cgi/image/width=550,height=623,quality=80,format=auto/uploads/April2024/top-san-pham-ex-dry-coolmate.jpg', N'Top 10 dòng sản phẩm Ex-Dry bán chạy nhất Coolmate hiện nay'),
(N'Bật mí 14 cách giặt quần lót bị dính máu mà không phải ai cũng biếtn1. Giặt quần lót dính máu với nước lạnhnNước lạnh là "vũ khí" đầu tiên và hiệu quả nhất để đánh bay vết máu trên quần lót.  Nước lạnh giúp làm đông cứng máu và ngăn chặn quá trình thấm sâ', 'https://media.coolmate.me/cdn-cgi/image/width=550,height=623,quality=80,format=auto/uploads/April2024/cach-gap-ao-khoac-gon-gang_(47).jpg', N'14 cách giặt quần lót dính máu sạch nhanh chóng ngay tại nhà'),
(N'Màu nude là màu gì?nMàu nude là màu gì? Màu nude là một gam màu trung tính, mang sắc độ dịu nhẹ, gần với màu trắng nhưng có chút ngà. Trong tiếng Anh, "nude" được sử dụng để chỉ tình trạng không mặc quần áo. Từ này cũng có thể hiểu là màu sắc gần giống vớ', 'https://media.coolmate.me/cdn-cgi/image/width=550,height=623,quality=80,format=auto/uploads/April2024/1.png', N'Màu nude là màu gì? Ứng dụng và nguyên tắc phối màu trong cuộc sống'),
(N'1. Vải hemp là gì? Tìm hiểu về nguồn gốc của vải HempnKhông chỉ “xâm chiếm" ngành công nghiệp thời trang thế giới, vải hemp còn đang được đón nhận khá mạnh mẽ ở thị trường Việt Nam. Vậy chính xác vải hemp là gì và tại sao loại vải này lại được yêu thích đ', 'https://media.coolmate.me/cdn-cgi/image/width=550,height=623,quality=80,format=auto/uploads/April2024/vai-hemp-10.jpg', N'Vải hemp là gì? Ưu nhược điểm và ứng dụng trong cuộc sống'),
(N'Bật mí 8 cách phối đồ màu đen thời trang, nam tínhnMàu đen tỏa ra sức hút mãnh liệt bởi sự huyền bí, nam tính. Đây cũng chính là tone màu được nam giới ưa chuộng trong thời trang vì tính linh hoạt, sang trọng và lịch lãm. Chính vì vậy, màu đen phối được r', 'https://media.coolmate.me/cdn-cgi/image/width=550,height=623,quality=80,format=auto/uploads/April2024/13.jpg', N'Màu đen phối với màu gì nam? Top 8 cách phối đồ sành điệu, phong cách'),
(N'Vải chống thấm nước là gì?nCác loại vải chống thấm có khả năng ngăn chặn nước thẩm thấu qua bề mặt. Đồng thời bảo vệ đồ vật bên trong luôn được khô ráo, sạch sẽ và tránh ẩm mốc. Ngày nay chất liệu này sử dụng rất phổ biến để may quần áo, túi xách và những', 'https://media.coolmate.me/cdn-cgi/image/width=550,height=623,quality=80,format=auto/uploads/April2024/cac-loai-vai-chong-tham-nuoc-3.jpg', N'6 loại vải chống thấm nước phổ biến nhất hiện nay'),
(N'Vải tole là gì? Cách phân biệt vải tole và vải lanhnVải tole, hay còn gọi là vải tôn, vải toile,... là chất liệu vải mỏng được tạo nên từ sợi cây lanh pha cùng một số chất liệu khác. Bề mặt vải tole có rất nhiều lỗ thoáng khí nhỏ với kích thước không đều,', 'https://media.coolmate.me/cdn-cgi/image/width=550,height=623,quality=80,format=auto/uploads/April2024/vai-tole-la-gi-9.jpg', N'Vải tole là gì? Phân biệt vải tole và vải lanh bạn nên biết');
GO

-- INSERT DATA TO TABLE CARTS
INSERT INTO Carts(quantity, totalamount, productid, userId) VALUES
(2, 2000000, 1, 2),
(3, 3590000, 2, 3);
GO

-- INSERT DATA TO TABLE ORDERS
INSERT INTO Orders (orderAddress, orderAmount, orderDate, orderEmail, orderFullName, orderNote, orderPaymentMethods, orderPhone, orderStatus, orderStatusPayment, userId) VALUES
('Hà Nội', 1057000, NULL, 'tuanflute27052004@gmail.com', 'tuanflute', 'thank you', 'Thanh toán khi giao hàng', '0386564543', 5, 'Đã thanh toán',2),
('Hà Nội', 2380000, NULL, 'nguyenvana@gmail.com', 'nguyễn văn a', ':))', 'Thanh toán khi giao hàng', '123456789', 1, 'Chưa thanh toán', 3);
GO
/*
* 1. cho xac nhan
* 2. xac nhan don hang
* 3. chuan bi don hang
* 4. dang giao hang
* 5.da giao hang
* 6. huy don hang
*
*/


-- INSERT DATA TO TABLE ORDER_DETAIL
INSERT INTO OrderDetail(orderId,productId, price, quantity, totalMoney) VALUES
(1,1, 990000, 114, 990000),
(1,2, 2290000, 55, 2290000),
(1,3, 7290000, 4, 7290000),
(2,4, 1190000, 115, 2380000);
GO