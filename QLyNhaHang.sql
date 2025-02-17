CREATE DATABASE [QLNhaHang]
GO
USE [QLNhaHang]
GO
CREATE TABLE [dbo].[LoaiMonAn]
(
	 [MaLoaiMA] nvarchar(30) not null primary key
	,[TenLoaiMA] nvarchar(60) not null
)
--select * from DangKyCa
--delete from DangKyCa where MaDangKy='DK20250210001'
CREATE TABLE [dbo].[MonAn]
(
	 [LoaiMA] nvarchar(30) references [dbo].[LoaiMonAn]([MaLoaiMA])
	,[MaMonAn] nvarchar(30) not null primary key
	,[TenMonAn] nvarchar(60) not null
	,[HinhAnh] nvarchar(1024)
	,[Gia] float
	,[MoTa] nvarchar(60)
	,[TrangThai] tinyint
)

CREATE TABLE [dbo].[ViTriCongViec]
(
	 [MaViTriCV] nvarchar(30) not null primary key
	,[TenViTriCV] nvarchar(60) not null
)

CREATE TABLE [dbo].[NhanVien]
(
	 [MaNV] nvarchar(30) not null primary key
	,[TenNV] nvarchar(100) not null
	,[HinhAnh] nvarchar(1024)
	,[CCCD] nvarchar(12) not null 
	,[GioiTinh] nvarchar(3)
	,[SDT] nvarchar(10) 
	,[Email] nvarchar(40)
	,[DiaChi] nvarchar(1000)
	,[MaViTriCV] nvarchar(30) references [dbo].[ViTriCongViec]([MaViTriCV])
	,[TrangThai] nvarchar(60) ----Đang làm việc/ Đã nghỉ việc
	,[MaQuanLy] nvarchar(30) references [dbo].[NhanVien]([MaNV])
	,[ThuViec] bit ----0: thử viếc/ 1: làm chính thức
	,[NgayVaoLam] date
	,[MaBHYT] nvarchar(15) 
)

CREATE TABLE [dbo].[Ca]
(
	 [MaCa] nvarchar(30) not null primary key
	,[ThoiGianBatDau] time
	,[ThoiGianKetThuc] time
	,[LoaiCa] nvarchar(50) not null
)

CREATE TABLE [dbo].[SoLuongTrongCa]
(
	 [MaQuanLy] nvarchar(30) not null primary key
	,[MaCa] nvarchar(30) not null references [dbo].[Ca]([MaCa])
	,[Ngay] date not null
	,[SoLuongToiDa] int
)

CREATE TABLE [dbo].[SoLuongChiTietTrongCa]
(
	 [MaQuanLy] nvarchar(30) not null references [dbo].[SoLuongTrongCa]([MaQuanLy])
	,[MaQuanLyChiTiet] nvarchar(30) not null primary key
	,[MaViTriCV] nvarchar(30) references [dbo].[ViTriCongViec]([MaViTriCV])
	,[SoLuong] int
)

CREATE TABLE [dbo].[DangKyCa]
(	
	 [MaQuanLy] nvarchar(30) not null references [dbo].[SoLuongTrongCa]([MaQuanLy])
	,[MaDangKy] nvarchar(30) primary key
	,[MaNV] nvarchar(30) not null references [dbo].[NhanVien]([MaNV])
	,[Ngay] date
)

CREATE TABLE [dbo].[Ban]
(
	 [MaBan] nvarchar(30) not null primary key
	,[SoLuongNguoi] int
	,[ViTri] nvarchar(30) not null
	,[TrangThai] bit ----0: chưa ai ngồi/ 1: ngồi rồi
)

CREATE TABLE [dbo].[DatBan]
(
	 [MaDatBan] nvarchar(30) not null primary key
	,[MaBan] nvarchar(30) not null references [dbo].[Ban]([MaBan])
	,[SoNguoiDi] int
	,[TenKH] nvarchar(50) 
	,[SDT] nvarchar(10)
	,[NgayDatBan] datetime
)

CREATE TABLE [dbo].[HoaDon]
(
	 [MaHoaDon] nvarchar(30) not null primary key
	,[MaDatBan] nvarchar(30) not null references [dbo].[DatBan]([MaDatBan])
	,[NgayXuatHD] datetime
	,[TongTien] float
	,[TrangThai] nvarchar(30)
)

CREATE TABLE [dbo].[HoaDonChiTiet]
( 
	 [MaHoaDon] nvarchar(30) not null references [dbo].[HoaDon]([MaHoaDon])
	,[MaMonAn] nvarchar(30) not null references [dbo].[MonAn]([MaMonAn])
	,[SoLuong] int
	,[Gia] float
	,primary key([MaHoaDon],[MaMonAn])
)

CREATE TABLE [dbo].[LichSuChuyenBan] (
     [MaChuyenBan] nvarchar(30) primary key
    ,[MaDatBan] NVARCHAR(30) references [dbo].[DatBan]([MaDatBan])
    ,[MaBanCu] NVARCHAR(30) references [dbo].[Ban]([MaBan])
    ,[MaBanMoi] NVARCHAR(30) references [dbo].[Ban]([MaBan])
    ,[ThoiGianChuyen] DATETIME
    ,[MaNV] nvarchar(30) not null references [dbo].[NhanVien]([MaNV])
	,[LyDoChuyen] nvarchar(max)
)

CREATE TABLE [dbo].[TaiKhoan] (
     [TaiKhoan] nvarchar(30) primary key
	,[MatKhau] nvarchar(60)
    ,[MaNV] nvarchar(30) not null references [dbo].[NhanVien]([MaNV])
)


CREATE TABLE MaXacNhan (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MaXacNhan NVARCHAR(50) NOT NULL, -- Mã xác nhận
    Email NVARCHAR(100) NOT NULL, -- Email khách hàng
    NgayDatBan DATETIME, -- Ngày đặt bàn
	TenKH NVARCHAR(80),
    TrangThai NVARCHAR(20), -- Trạng thái (chờ xác nhận, đã xác nhận, hủy)
	SoNguoiDi INT,
	SDT nvarchar(10),
    NgayTao DATETIME DEFAULT GETDATE() -- Ngày tạo mã xác nhận
);


--- Bàn 
INSERT INTO [dbo].[Ban] ([MaBan],[SoLuongNguoi],[ViTri],[TrangThai])
VALUES	
	------Chưa ai ngồi
	 ('B001',4,N'Lầu 1',0)
	,('B002',4,N'Lầu 1',0)
	,('B003',4,N'Lầu 1',0)
	,('B004',2,N'Lầu 1',0)
	,('B005',6,N'Lầu 1',0)
	,('B006',4,N'Lầu 1',0)
	,('B007',2,N'Lầu 1',0)
	,('B008',4,N'Lầu 1',0)
	,('B009',2,N'Lầu 1',0)
	,('B010',6,N'Lầu 1',0)
	,('B011',2,N'Lầu 1',0)
	,('B012',4,N'Lầu 1',0)


	,('B013',4,N'Lầu 2',0)
	,('B014',4,N'Lầu 2',0)
	,('B015',6,N'Lầu 2',0)
	,('B016',2,N'Lầu 2',0)
	,('B017',6,N'Lầu 2',0)
	,('B018',4,N'Lầu 2',0)
	,('B019',2,N'Lầu 2',0)

	,('B020',4,N'Lầu 3',0)
	,('B021',4,N'Lầu 3',0)
	,('B022',2,N'Lầu 3',0)
	,('B023',2,N'Lầu 3',0)
	,('B024',4,N'Lầu 3',0)
	,('B025',4,N'Lầu 3',0)
	----------------Đặt bàn
	,('B026',4,N'Lầu 1',1)
	,('B027',4,N'Lầu 1',1)
	,('B028',4,N'Lầu 1',1)
	,('B029',2,N'Lầu 1',1)
	,('B030',6,N'Lầu 1',1)
	,('B031',4,N'Lầu 1',1)
	,('B032',2,N'Lầu 1',1)
	,('B033',4,N'Lầu 1',1)
	,('B034',2,N'Lầu 1',1)
	,('B035',6,N'Lầu 1',1)
	,('B036',2,N'Lầu 1',1)
	,('B037',4,N'Lầu 1',1)
	,('B038',4,N'Lầu 1',1)

	,('B039',4,N'Lầu 2',1)
	,('B040',4,N'Lầu 2',1)
	,('B041',6,N'Lầu 2',1)
	,('B042',2,N'Lầu 2',1)
	,('B043',6,N'Lầu 2',1)
	,('B044',4,N'Lầu 2',1)
	,('B045',2,N'Lầu 2',1)


	,('B046',4,N'Lầu 3',1)
	,('B047',4,N'Lầu 3',1)
	,('B048',2,N'Lầu 3',1)
	,('B049',2,N'Lầu 3',1)
	,('B050',4,N'Lầu 3',1)
---Ca Trực 

INSERT INTO [dbo].[Ca]([MaCa], [ThoiGianBatDau], [ThoiGianKetThuc], [LoaiCa])
VALUES
	 ('C001', N'7:00 AM', N'3:00 PM', N'Full-time') -- Ca sáng (8 tiếng)
    ,('C002', N'2:30 PM', N'10:30 PM', N'Full-time') -- Ca chiều (8 tiếng)
    ,('C003', N'7:00 AM', N'11:00 AM', N'Part-time') -- Ca sáng ngắn (4 tiếng)
    ,('C004', N'11:00 AM',N'3:00 PM', N'Part-time') -- Ca trưa ngắn (4 tiếng)
    ,('C005', N'2:30 PM', N'6:30 PM', N'Part-time')  -- Ca chiều ngắn (4 tiếng)
    ,('C006', N'6:30 PM',N'10:30 PM', N'Part-time') -- Ca tối ngắn (4 tiếng)

---Loại Món Ăn
INSERT INTO [dbo].[LoaiMonAn]([MaLoaiMA], [TenLoaiMA])
VALUES
	 ('LMA001', N'Khai Vị')
    ,('LMA002', N'Món Chính')
    ,('LMA003', N'Tráng Miệng')
    ,('LMA004', N'Đồ Uống')

--------Món Ăn
---Khai Vị
select * from MonAn
INSERT INTO [dbo].[MonAn] ([LoaiMA], [MaMonAn], [TenMonAn], [HinhAnh], [Gia], [MoTa],[TrangThai])
VALUES

    ('LMA001', 'MA001', N'Salad Caesar',N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186569/QLNhaHang/salad-caesar-20250118144926.jpg', 90000, N'Salad xà lách với sốt đặc trưng',0),
    ('LMA001', 'MA002', N'Súp Hành Tây Kiểu Pháp', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737188959/QLNhaHang/súp-hành-tây-kiểu-pháp-20250118152916.jpg', 85000, N'Súp hành tây đậm đà kiểu Pháp',0),
    ('LMA001', 'MA003', N'Bánh Mì Bruschetta', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186642/QLNhaHang/bánh-mì-bruschetta-20250118145040.jpg', 60000, N'Bánh mì nướng phủ cà chua và húng quế',0),
    ('LMA001', 'MA004', N'Phô Mai Que', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186659/QLNhaHang/phô-mai-que-20250118145057.jpg', 75000, N'Phô mai chiên giòn tan',0),
    ('LMA001', 'MA005', N'Đĩa Khai Vị Antipasto', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186676/QLNhaHang/đĩa-khai-vị-antipasto-20250118145114.jpg', 150000, N'Đĩa khai vị với thịt nguội và phô mai',0),
    ('LMA001', 'MA006', N'Salad Caprese', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186691/QLNhaHang/salad-caprese-20250118145130.jpg', 90000, N'Salad cà chua và phô mai Mozzarella',0),
    ('LMA001', 'MA007', N'Pâté Gan Ăn Kèm Bánh Mì', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186707/QLNhaHang/pâté-gan-ăn-kèm-bánh-mì-20250118145144.jpg', 80000, N'Pâté gan béo ngậy ăn cùng bánh mì nướng',0),
    ('LMA001', 'MA008', N'Tôm Cocktail', 'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186723/QLNhaHang/tôm-cocktail-20250118145201.jpg', 110000, N'Tôm tươi chấm sốt cocktail',0),
    ('LMA001', 'MA009', N'Đĩa Phô Mai Tổng Hợp', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186738/QLNhaHang/đĩa-phô-mai-tổng-hợp-20250118145216.jpg', 160000, N'Nhiều loại phô mai cao cấp',0),
    ('LMA001', 'MA010', N'Thịt Bò Carpaccio', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186760/QLNhaHang/thịt-bò-carpaccio-20250118145238.jpg', 140000, N'Thịt bò sống thái lát mỏng',0),
    ('LMA001', 'MA011', N'Bánh Mì Bơ Tỏi', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186787/QLNhaHang/bánh-mì-bơ-tỏi-20250118145304', 60000, N'Bánh mì giòn thơm vị bơ tỏi',0),
    ('LMA001', 'MA012', N'Trứng Nhồi Gia Vị', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186804/QLNhaHang/trứng-nhồi-gia-vị-20250118145323.jpg', 70000, N'Trứng nhồi nhân đậm vị',0),
    ('LMA001', 'MA013', N'Bánh Cua Kiểu Mỹ', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186821/QLNhaHang/bánh-cua-kiểu-mỹ-20250118145339.png', 120000, N'Bánh cua chiên giòn',0),
    ('LMA001', 'MA014', N'Dưa Lưới Cuộn Thịt Nguội', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186841/QLNhaHang/dưa-lưới-cuộn-thịt-nguội-20250118145359.jpg', 130000, N'Dưa lưới tươi cuộn thịt nguội',0),
    ('LMA001', 'MA015', N'Cá Hồi Tartare', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186869/QLNhaHang/cá-hồi-tartare-20250118145426.jpg', 150000, N'Cá hồi sống trộn gia vị đặc biệt',2);


---Món Chính
INSERT INTO [dbo].[MonAn] ([LoaiMA], [MaMonAn], [TenMonAn], [HinhAnh], [Gia], [MoTa],[TrangThai])
VALUES
    ('LMA002', 'MA016', N'Bít Tết Ribeye', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186887/QLNhaHang/bít-tết-ribeye-20250118145445.jpg', 450000, N'Bò bít tết phần ribeye thơm mềm',0),
    ('LMA002', 'MA017', N'Cá Hồi Nướng Wellington', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186903/QLNhaHang/cá-hồi-nướng-wellington-20250118145501.jpg', 350000, N'Cá hồi bọc bột nướng giòn tan',0),
    ('LMA002', 'MA018', N'Mì Ý Sốt Kem Carbonara', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186924/QLNhaHang/mì-ý-sốt-kem-carbonara-20250118145521.jpg', 200000, N'Mì Ý sốt kem với thịt xông khói',0),
    ('LMA002', 'MA019', N'Gà Cuộn Phô Mai Cordon Bleu',N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186939/QLNhaHang/gà-cuộn-phô-mai-cordon-bleu-20250118145537.jpg', 220000, N'Gà chiên giòn cuộn phô mai',0),
    ('LMA002', 'MA020', N'Thịt Bê Milanese', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186954/QLNhaHang/thịt-bê-milanese-20250118145553.jpg', 300000, N'Thịt bê chiên xù kiểu Ý',0),
    ('LMA002', 'MA021', N'Sườn Cừu Nướng', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186973/QLNhaHang/sườn-cừu-nướng-20250118145611.jpg', 420000, N'Sườn cừu nướng sốt thảo mộc',0),
    ('LMA002', 'MA022', N'Cơm Hải Sản Tây Ban Nha', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737186990/QLNhaHang/cơm-hải-sản-tây-ban-nha-20250118145628.jpg', 320000, N'Cơm nấu hải sản thơm ngon',0),
    ('LMA002', 'MA023', N'Gà Nấu Rượu Vang',N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187006/QLNhaHang/gà-nấu-rượu-vang-20250118145644.jpg', 280000, N'Gà hầm rượu vang đỏ kiểu Pháp',0),
    ('LMA002', 'MA024', N'Thịt Heo Chiên Xù Schnitzel', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187023/QLNhaHang/thịt-heo-chiên-xù-schnitzel-20250118145700.jpg', 210000, N'Thịt heo chiên giòn kiểu Đức',0),
    ('LMA002', 'MA025', N'Tôm Hấp Sốt Rượu Vang Trắng', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187041/QLNhaHang/tôm-hấp-sốt-rượu-vang-trắng-20250118145719.jpg', 250000, N'Tôm hấp thơm lừng sốt vang trắng',0),
    ('LMA002', 'MA026', N'Đùi Vịt Hầm Kiểu Pháp', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187057/QLNhaHang/đùi-vịt-hầm-kiểu-pháp-20250118145735.jpg', 320000, N'Vịt hầm mềm với gia vị',0),
    ('LMA002', 'MA027', N'Bánh Lasagna Ý', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187079/QLNhaHang/bánh-lasagna-ý-20250118145757.jpg', 280000, N'Bánh lasagna nhiều lớp thịt và phô mai',0),
    ('LMA002', 'MA028', N'Bò Hầm Rượu Vang Kiểu Pháp', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187093/QLNhaHang/bò-hầm-rượu-vang-kiểu-pháp-20250118145811.jpg', 350000, N'Bò hầm rượu vang đỏ',0),
    ('LMA002', 'MA029', N'Sò Điệp Sốt Bơ', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187109/QLNhaHang/sò-điệp-sốt-bơ-20250118145827.jpg', 300000, N'Sò điệp nấu sốt bơ thơm lừng',0),
    ('LMA002', 'MA030', N'Pizza Margherita Truyền Thống', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187124/QLNhaHang/pizza-margherita-truyền-thống-20250118145843.jpg', 180000, N'Pizza Ý truyền thống với cà chua và phô mai',0);

---Tráng Miệng
INSERT INTO [dbo].[MonAn] ([LoaiMA], [MaMonAn], [TenMonAn], [HinhAnh], [Gia], [MoTa], [TrangThai])
VALUES
    ('LMA003', 'MA031', N'Bánh Phô Mai', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187145/QLNhaHang/bánh-phô-mai-20250118145901.jpg', 150000, N'Bánh phô mai mềm mịn, ngọt ngào',0),
    ('LMA003', 'MA032', N'Kem Sữa Ý', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187164/QLNhaHang/kem-sữa-ý-20250118145922.jpg', 120000, N'Kem sữa thơm mát, mềm mịn kiểu Ý',0),
    ('LMA003', 'MA033', N'Bánh Su Kem', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187181/QLNhaHang/bánh-su-kem-20250118145939.jpg', 100000, N'Bánh su kem nhân socola, giòn và béo',0),
    ('LMA003', 'MA034', N'Bánh Socola Tan Chảy', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187199/QLNhaHang/bánh-socola-tan-chảy-20250118145957.jpg', 130000, N'Bánh socola nóng hổi, nhân socola tan chảy',0),
    ('LMA003', 'MA035', N'Kem Caramel Nướng', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187218/QLNhaHang/kem-caramel-nướng-20250118150016.png', 110000, N'Kem caramel nướng giòn lớp mặt',0),
    ('LMA003', 'MA036', N'Bánh Chuối Caramel', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187239/QLNhaHang/bánh-chuối-caramel-20250118150037.jpg', 140000, N'Bánh chuối với caramel ngọt ngào',0),
    ('LMA003', 'MA037', N'Kem Mâm Xôi', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187255/QLNhaHang/kem-mâm-xôi-20250118150054.jpg', 90000, N'Kem mâm xôi mát lạnh, thanh mát',0),
    ('LMA003', 'MA038', N'Bánh Opera Nhiều Lớp', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187272/QLNhaHang/bánh-opera-nhiều-lớp-20250118150110.jpg', 150000, N'Bánh nhiều lớp socola và cà phê đậm đà',0),
    ('LMA003', 'MA039', N'Socola Truffle', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187297/QLNhaHang/socola-truffle-20250118150135.jpg', 120000, N'Socola truffle ngọt ngào, mềm tan trong miệng',0),
    ('LMA003', 'MA040', N'Bánh Pavlova Trái Cây', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187321/QLNhaHang/bánh-pavlova-trái-cây-20250118150159.jpg', 130000, N'Bánh meringue giòn với trái cây tươi',0),
    ('LMA003', 'MA041', N'Bánh Chanh Dây', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187337/QLNhaHang/bánh-chanh-dây-20250118150215.jpg', 110000, N'Bánh chanh dây tươi, ngọt dịu và thơm',0),
    ('LMA003', 'MA042', N'Bánh Quẩy Churros', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187354/QLNhaHang/bánh-quẩy-churros-20250118150231.jpg', 80000, N'Bánh quẩy chiên giòn, ăn kèm với sốt socola',0),
    ('LMA003', 'MA043', N'Bánh Tart Hoa Quả', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187373/QLNhaHang/bánh-tart-hoa-quả-20250118150251.jpg', 120000, N'Bánh tart giòn với nhân hoa quả tươi ngon',0),
    ('LMA003', 'MA044', N'Kem Gelato Ý', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187389/QLNhaHang/kem-gelato-ý-20250118150307.jpg', 100000, N'Kem gelato mềm mịn kiểu Ý',0),
    ('LMA003', 'MA045', N'Bánh Brownie Socola', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187404/QLNhaHang/bánh-brownie-socola-20250118150322.jpg', 110000, N'Bánh brownie socola đậm đà, mềm mịn',0);

---Đồ Uống
INSERT INTO [dbo].[MonAn] ([LoaiMA], [MaMonAn], [TenMonAn], [HinhAnh], [Gia], [MoTa], [TrangThai])
VALUES
    ('LMA004', 'MA046', N'Cà Phê Espresso', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187423/QLNhaHang/cà-phê-espresso-20250118150342.jpg', 50000, N'Cà phê đậm đà, nguyên chất',0),
    ('LMA004', 'MA047', N'Cà Phê Sữa Latte', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187444/QLNhaHang/cà-phê-sữa-latte-20250118150402.jpg', 60000, N'Cà phê sữa thơm béo',0),
    ('LMA004', 'MA048', N'Cà Phê Cappuccino', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187464/QLNhaHang/cà-phê-cappuccino-20250118150422.jpg', 70000, N'Cà phê sữa đánh bọt mịn',0),
    ('LMA004', 'MA049', N'Cà Phê Sữa Flat White', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187498/QLNhaHang/cà-phê-sữa-flat-white-20250118150456.jpg', 75000, N'Cà phê sữa ít bọt, nhẹ nhàng',0),
    ('LMA004', 'MA050', N'Cà Phê Rượu Whisky', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187514/QLNhaHang/cà-phê-rượu-whisky-20250118150513.jpg', 100000, N'Cà phê kết hợp với rượu whisky đặc biệt',0),
    ('LMA004', 'MA051', N'Trà Xanh Matcha Latte', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187541/QLNhaHang/trà-xanh-matcha-latte-20250118150539.jpg', 75000, N'Trà xanh matcha kết hợp với sữa mịn',0),
    ('LMA004', 'MA052', N'Sinh Tố Dâu Tây', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187558/QLNhaHang/sinh-tố-dâu-tây-20250118150556.jpg', 90000, N'Sinh tố dâu tươi mát lạnh',0),
    ('LMA004', 'MA053', N'Cocktail Mojito Bạc Hà', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187574/QLNhaHang/cocktail-mojito-bạc-hà-20250118150612.jpg', 120000, N'Cocktail mojito mát lạnh với bạc hà tươi',0),
    ('LMA004', 'MA054', N'Cocktail Sangria Trái Cây', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187594/QLNhaHang/cocktail-sangria-trái-cây-20250118150631.jpg', 150000, N'Cocktail rượu vang trái cây ngọt ngào',0),
    ('LMA004', 'MA055', N'Socola Nóng', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187612/QLNhaHang/socola-nóng-20250118150650.jpg', 70000, N'Socola nóng đậm đà',0),
    ('LMA004', 'MA056', N'Cocktail Mai Tai', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187630/QLNhaHang/cocktail-mai-tai-20250118150709.jpg', 60000, N'Cà phê Mỹ nhẹ nhàng, nguyên chất',0),
    ('LMA004', 'MA057', N'Cocktail Bellini', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187646/QLNhaHang/cocktail-bellini-20250118150724.jpg', 130000, N'Cocktail Bellini với rượu vang và đào tươi',0),
    ('LMA004', 'MA058', N'Cocktail Dứa Piña Colada', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187660/QLNhaHang/cocktail-dứa-piña-colada-20250118150738.jpg', 140000, N'Cocktail dứa mát lạnh với rượu rum',0),
    ('LMA004', 'MA059', N'Trà Earl Grey', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187674/QLNhaHang/trà-earl-grey-20250118150752.jpg', 70000, N'Trà Earl Grey thơm nhẹ, thanh mát',0),
    ('LMA004', 'MA060', N'Trà Chanh Đá', N'https://res.cloudinary.com/dz4xwoz19/image/upload/v1737187689/QLNhaHang/trà-chanh-đá-20250118150807.jpg', 50000, N'Trà chanh tươi mát lạnh',0);

INSERT INTO [dbo].[ViTriCongViec] ([MaViTriCV],[TenViTriCV])
VALUES
	----Chỉnh sau
	 ('VT001', N'Quản lý nhà hàng')
    ,('VT002', N'Đầu bếp trưởng')
    ,('VT003', N'Đầu bếp phụ')
    ,('VT004', N'Phụ bếp')
    ,('VT005', N'Nhân viên phục vụ')
    ,('VT006', N'Nhân viên pha chế')
    ,('VT007', N'Nhân viên lễ tân')
    ,('VT008', N'Nhân viên vệ sinh')
    ,('VT009', N'Nhân viên kho')
    ,('VT010', N'Quản lý ca')
-----TS: là đc hỗ trợ 100% ko cần đóng BHYT
--2: 100% nhỏ này nhà nc bảo kê
--4: 80% nhỏ này làm là phải đóng 1.5% lương nha
select * from ViTriCongViec
INSERT INTO [dbo].[NhanVien] ([MaNV],[TenNV],[HinhAnh],[CCCD],[GioiTinh],[SDT],[Email],[DiaChi],[MaViTriCV],[MaQuanLy],[ThuViec],[NgayVaoLam],[MaBHYT],[TrangThai])
VALUES
	----Quản lý nhà hàng
	 ('NV001',N'Bùi Ngọc Uyên Chi',null,'362792807872',N'Nữ','0352716278','buingocuyenchi@gmail.com',N'F11/27E2 đường Phạm Thị Nghĩ, ấp 6, Xã Vĩnh Lộc A, Huyện Bình Chánh, TP Hồ Chí Minh','VT001',null,null,null,'DN4266261528123',1)
	 -----Đầu bếp trưởng
	,('NV002',N'Bùi Tiến Sĩ',null,'426198092876',N'Nam','0261928071','buitiensi@gmail.com',N'2/33 đường 147, KP5, Phường Tăng Nhơn Phú B, Thành phố Thủ Đức, TP Hồ Chí Minh','VT002','NV001',1,'1999/03/04','DN4217829172345',1)
	,('NV003',N'Nguyễn Thế Dũng',null,'636271836457',N'Nam','0372261728','nguyenthedung@gmail.com',N' 223 Hoàng Văn Thụ, Phường 08, Quận Phú Nhuận, TP Hồ Chí Minh','VT002','NV001',1,'1997/12/03','DN4231435213324',1)
	,('NV004',N'Đào Huy Hoàng',null,'654276527625',N'Nam','0526765167','daohuyhoang@gmail.com',N'Số 103, đường số 5, Phường Linh Xuân, Thành phố Thủ Đức, TP Hồ Chí Minh','VT002','NV001',1,'2002/03/05','TS2365278162716',1)
	,('NV005',N'Hoàng Hương Diễm',null,'979065467865',N'Nữ','0982765178','hoanghuongdiem@gmail.com',N'Tầng 16, tòa nhà E, Town Central, số 11 Đoàn Văn Bơ, phường 13, Quận 4, TP Hồ Chí Minh','VT002','NV001',1,'2000/03/13','DN4625187265298',1)
	----Phó bếp
	,('NV006',N'Nguyễn Quỳnh Chi',null,'362517826362',N'Nữ','0352673526','nguyenquynhchi@gmail.com',N'Số 473 Đỗ Xuân Hợp, Phường Phước Long B, Thành phố Thủ Đức, TP Hồ Chí Minh','VT003',null,1,'2002/11/13','TS2625762516276',1)
	,('NV007',N'Nguyễn Thanh Trúc',null,'453627635626',N'Nữ','0852676536','nguyenthanhtruc@gmail.com',N'120 Vũ Tông Phan , Khu Phố 5, Phường An Phú, Thành phố Thủ Đức, TP Hồ Chí Minh','VT003',null,1,'1999/06/17','DN4526765254625',1)
	,('NV008',N'Nguyễn Đắc Tú',null,'983768987637',N'Nam','0243565342','nguyendactu@gmail.com',N'72 Bình Giã, Phường 13, Quận Tân Bình, TP Hồ Chí Minh','VT003',null,1,'2000/09/18','DN4765765765654',1)
	,('NV009',N'Nguyễn Thị Nga',null,'872657638762',N'Nữ','0827617261','nguyenthinga@gmail.com',N'C10 Rio Vista, 72 Dương Đình Hội, Phường Phước Long B, Thành phố Thủ Đức, TP Hồ Chí Minh','VT003',null,1,'2000/04/12','DN4342652761652',1)
	,('NV010',N'Trần Thị Kim Chi',null,'214652415425',N'Nữ','0872657891','tranthikimchi@gmail.com',N'Số 125, Đường số 5, Khu đô thị Lakeview city, Phường An Phú, Thành phố Thủ Đức, TP Hồ Chí Minh','VT003',null,1,'2001/05/21','DN4879878789878',1)
	,('NV011',N'Hoàng Đức Hưng',null,'657854323432',N'Nam','0309805654','hoangduchung@gmail.com',N'197 Nguyễn Văn Thủ, Phường Đa Kao, Quận 1, TP Hồ Chí Minh','VT003',null,1,'1999/11/16','DN4345678456756',1)
	,('NV012',N'Nguyễn Thị Ngọc Hà',null,'892761762876',N'Nữ','0236576876','nguyenthingocha@gmail.com',N'1001/2 /5 Đường Nguyễn Thị Định, Khu Phố 3, Phường Cát Lái, Thành phố Thủ Đức, TP Hồ Chí Minh','VT003',null,1,'1999/03/19','DN4765765765482',1)
	,('NV013',N'Mai Thị Hiên',null,'562768628176',N'Nữ','0456567543','maithihien@gmail.com',N'33/9A Đường số 08, Khu phố 01, Phường Linh Xuân, Thành phố Thủ Đức, TP Hồ Chí Minh','VT003',null,1,'2001/12/20','DN4526576528761',1)
INSERT INTO [dbo].[NhanVien] ([MaNV],[TenNV],[HinhAnh],[CCCD],[GioiTinh],[SDT],[Email],[DiaChi],[MaViTriCV],[MaQuanLy],[ThuViec],[NgayVaoLam],[MaBHYT],[TrangThai])
VALUES
	-----Phụ bếp
	---------Làm chính thức
	 ('NV014',N'Phạm Thị Thanh',null,'376276352765',N'Nữ','0365278653','phamthithanh@gmail.com',N'45 Nguyễn Đôn Tiết, Phường Cát Lái, Thành phố Thủ Đức, TP Hồ Chí Minh','VT004',null,1,'2003/02/21','TS2754139028632',1)
	,('NV015',N'Hoàng Thị Huyền',null,'362528736276',N'Nữ','0854657657','hoangthihuyen@gmail.com',N'Số 58 đường 53, Phường Tân Phong, Quận 7, TP Hồ Chí Minh','VT004',null,1,'2010/06/19','DN4638491207556',1)
	,('NV016',N'Phạm Văn Khang',null,'465378928765',N'Nam','0564378543','phamvankhang@gmail.com',N'Số 12, Đường số 2, Phường Phú Hữu, Thành phố Thủ Đức, TP Hồ Chí Minh','VT004',null,1,'2020/03/25','TS2290384657143',1)
	,('NV017',N'Vũ Mạnh Hùng',null,'542657897267',N'Nam','0652786567','vumanhhung@gmail.com',N'Số 1B Đường số 30, Khu phố 2, Phường An Khánh, Thành phố Thủ Đức, TP Hồ Chí Minh','VT004',null,1,'2000/04/12','DN4728649051389',1)
	,('NV018',N'Nguyễn Xuân Thành',null,'653726537625',N'Nam','0154678087','nguyenxuanthanh@gmail.com',N'C10 Rio Vista, 72 Dương Đình Hội, Phường Phước Long B, Thành phố Thủ Đức, TP Hồ Chí Minh','VT004',null,1,'2022/01/11','DN4150893267423',1)	
	,('NV019',N'Nguyễn Thị Hà',null,'241325615634',N'Nữ','0254652430','nguyenthiha@gmail.com',N'8/7 Đường 49B, Khu phố 4, Phường Thảo Điền, Thành phố Thủ Đức, TP Hồ Chí Minh','VT004',null,1,'2018/03/26','DN4391762540854',1)
	,('NV020',N'Dương Văn Long',null,'540240154256',N'Nam','0147502510','duongvanlong@gmail.com',N'462 Nguyễn Thị Định, Phường Thạnh Mỹ Lợi, Thành phố Thủ Đức, TP Hồ Chí Minh','VT004',null,1,'2004/05/26','DN4483290167509',1)
	,('NV021',N'Trần Thị Vân Hải',null,'635760583702',N'Nữ','0906737632','tranthivanhai@gmail.com',N'Số 37, Đường Tôn, Phường Bến Nghé, Quận 1, TP Hồ Chí Minh','VT004',null,1,'2011/09/01','DN4597631284034',1)
	----------Thử việc
INSERT INTO [dbo].[NhanVien] ([MaNV],[TenNV],[HinhAnh],[CCCD],[GioiTinh],[SDT],[Email],[DiaChi],[MaViTriCV],[MaQuanLy],[ThuViec],[NgayVaoLam],[MaBHYT],[TrangThai])
VALUES
	 ('NV022',N'Nguyễn Tuấn Linh',null,'365025901652',N'Nam','0251065987','nguyentanlinh@gmail.com',N'52 Đại Lộ 3, Tổ 1, Khu phố 4, Phường Phước Bình, Thành phố Thủ Đức, TP Hồ Chí Minh','VT004',null,0,'2024/10/22',null,1)
	,('NV023',N'Đinh Thị Hiền Lê',null,'526108926165',N'Nữ','0972510254','dinhthihienle@gmail.com',N'12A Đường 109, Khu phố 5, Phường Phước Long B, Thành phố Thủ Đức, TP Hồ Chí Minh','VT004',null,0,'2024/07/23',null,1)
	,('NV024',N'Vũ Hoàng Anh',null,'862809726187',N'Nữ','0267987657','vuhoanganh@gmail.com',N'Số 68/8/17D, Đường Trần Thị Cờ, phường Thới An, Quận 12, TP Hồ Chí Minh','VT004',null,0,'2024/11/23',null,1)
	,('NV025',N'Khương Hải Yến',null,'524890261508',N'Nữ','0165786506','khuonghaiyen@gmail.com',N'132 Cao Đức Lân, Phường An Phú, Thành phố Thủ Đức, TP Hồ Chí Minh','VT004',null,0,'2024/12/22',null,1)
	-----------------------Nhân viên phục vụ
	-------Làm chính thức
	,('NV026',N'Nguyễn Thị Kim Huệ',null,'243876543876',N'Nữ','0987872531','nguyenthikimhue@gmail.com',N'365/21/2 Nguyễn Thị Kiểu, phường Tân Thới Hiệp, Quận 12, TP Hồ Chí Minh','VT005',null,1,'2003/04/16','TS2062574839134',1)
	,('NV027',N'Nguyễn Thị Hải Yến',null,'231465302514',N'Nữ','0263980732','nguyenthihaiyen@gmail.com',N'36 Mai Chí Thọ, Phường An Phú, Thành phố Thủ Đức, TP Hồ Chí Minh','VT005',null,1,'2019/02/10','DN4507148926328',1)	
	,('NV028',N'Hà Quốc Hùng',null,'128765306532',N'Nam','0890263625','haquochung@gmail.com',N'Số 35 Đường số 3, KDC Him Lam, Phường Trường Thọ, Thành phố Thủ Đức, TP Hồ Chí Minh','VT005',null,1,'2021/07/21','DN4983645207123',1)
	,('NV029',N'Doãn Thanh Tuấn',null,'679853891656',N'Nam','0697891054','doanhthanhtuan@gmail.com',N'179 QL1A, Phường Bình Chiểu, Thành phố Thủ Đức, TP Hồ Chí Minh','VT005',null,1,'2022/06/18','DN4467320158957',1)
	,('NV030',N'Quang Ánh Nguyệt',null,'535287610265',N'Nữ','0653875078','quanganhnguyet@gmail.com',N'185/57/27 Ngô Chí Quốc, Khu phố 2, Phường Bình Chiểu, Thành phố Thủ Đức, TP Hồ Chí Minh','VT005',null,1,'1999/05/28','DN4891650327438',1)	
	,('NV031',N'Lý Anh Thư',null,'652650929202',N'Nữ','0820154256','lyanhthu@gmail.com',N'12/14/18 Đường số 8, Phường Hiệp Bình Phước, Thành phố Thủ Đức, TP Hồ Chí Minh','VT005',null,1,'2005/08/12','DN4213487596054',1)
	,('NV032',N'Bùi Hồng Ngọc',null,'257090817652',N'Nữ','0765256176','buihongngoc@gmail.com',N'798 Hồng Bàng, Phường 11, Quận 11, TP Hồ Chí Minh','VT005',null,1,'2013/04/28','DN4036258149734',1)
	,('NV033',N'Lương Thị Hải Vân',null,'865326176209',N'Nữ','0526026517','luongthihaivan@gmail.com',N'17B Đường 41, Phường Linh Đông, Thành phố Thủ Đức, TP Hồ Chí Minh','VT005',null,1,'2016/07/07','DN4746295031832',1)
	,('NV034',N'Lâm Trần Lê Phát',null,'241676289176',N'Nam','0860265543','lamtranlephatg@gmail.com',N'19G/13 Đường số 9, Phường Long Bình, Thành phố Thủ Đức, TP Hồ Chí Minh','VT005',null,1,'2000/03/10','DN4480362751967',1)
	,('NV035',N'Đoàn Quốc Hưng',null,'786506424562',N'Nam','0765436435','doanquochung@gmail.com',N'122/1 Đường Lê Văn Thịnh, Phường Bình Trưng Tây, Thành phố Thủ Đức, TP Hồ Chí Minh','VT005',null,1,'2012/06/27','DN4859671204335',1)	
	,('NV036',N'Nguyễn Anh Quân',null,'453427807697',N'Nam','0475430465','nguyenanhquan@gmail.com',N'115/19 Hồ Văn Tư, Phường Trường Thọ, Thành phố Thủ Đức, TP Hồ Chí Minh','VT005',null,1,'2020/07/23','DN4104825379632',1)
	,('NV037',N'Hoàng Quốc Việt',null,'265434284692',N'Nam','0372643074','hoangquocviet@gmail.com',N'243/39 Tôn Đản, phường 15, Quận 4, TP Hồ Chí Minh','VT005',null,1,'2010/06/19','DN4562984731032',1)
	,('NV038',N'Trần Thị Tâm',null,'376251726218',N'Nữ','0637262876','tranthitam@gmail.com',N'4367/10 đường Nguyễn Cửu Phú, Khu phố 4, Phường Tân Tạo A, Quận Bình Tân, TP Hồ Chí Minh','VT005',null,1,'2000/12/11','DN4798354162035',1)
	,('NV039',N'Nguyễn Trọng Anh',null,'454325414251',N'Nam','0434738272','nguyentronganh@gmail.com',N'Số 1A Nguyễn Văn Đậu, Phường 05, Quận Phú Nhuận, TP Hồ Chí Minh','VT005',null,1,'2000/04/12','DN4642571830967',1)
	,('NV040',N'Lê Xuân Thiệp',null,'765786467954',N'Nam','0247865873','lexuanthiep@gmail.com',N'41 Đường số 2, Vạn Phúc 1, Phường Hiệp Bình Phước, Thành phố Thủ Đức, TP Hồ Chí Minh','VT005',null,1,'2022/01/11','DN4215680493756',1)	
	----------Thử việc
	,('NV041',N'Phan Lệ Kim Chi',null,'435276852412',N'Nữ','0435424376','phanlekimchi@gmail.com',N'36/38 Quốc Lộ 1A, Khu Phố 3, phường An Phú Đông, Quận 12, TP Hồ Chí Minh','VT005',null,0,'2024/11/26',null,1)
	,('NV042',N'Bùi Thị Kim Oanh',null,'652435265465',N'Nữ','0567687265','buithikimoanh@gmail.com',N'13 đường số 22, Phường Bình Trị Đông B, Quận Bình Tân, TP Hồ Chí Minh','VT005',null,0,'2024/12/22',null,1)
	,('NV043',N'Nguyễn Viết Trung',null,'765287625367',N'Nam','0463527876','nguyenviettrung@gmail.com',N'Số 952/11, Tỉnh lộ 43, khu phố 1, Phường Bình Chiểu, Thành phố Thủ Đức, TP Hồ Chí Minh','VT005',null,0,'2024/10/28',null,1)
	,('NV044',N'Nguyễn Thị Thùy Linh',null,'652761897065',N'Nữ','0321625096','nguyenthithuylinh@gmail.com',N'806 Quốc Lộ 1A khu phố 5, Phường Linh Trung, Thành phố Thủ Đức, TP Hồ Chí Minh','VT005',null,0,'2024/11/01',null,1)
	,('NV045',N'Hoàng Thị Huyền Trang',null,'156726065896',N'Nữ','0425142507','hoangthihuyentrang@gmail.com',N'Số 03 Nguyễn Bỉnh Khiêm, Phường Bình Thọ, Thành phố Thủ Đức, TP Hồ Chí Minh','VT005',null,0,'2024/11/10',null,1)	
	--------------------------Nhân viên pha chế
	----------Làm chính thức
	,('NV046',N'Trần Thị Kim Thảo',null,'452617628654',N'Nữ','0435678654','tranthikimthao@gmail.com',N'Số 149 Lê Văn Việt, Phường Hiệp Phú, Thành phố Thủ Đức, TP Hồ Chí Minh','VT006',null,1,'2003/04/16','TS2308127459667',1)
	,('NV047',N'Trần Hữu Đạt',null,'562512780265',N'Nam','0876253643','tranhuudat@gmail.com',N'371 Nguyễn Kiệm, Phường 3, Quận Gò Vấp, TP Hồ Chí Minh','VT006',null,1,'2019/02/10','DN4579846132003',1)	
	,('NV048',N'Nguyễn Thị Tố Anh',null,'645443245065',N'Nữ','0342651861','nguyenthitoanh@gmail.com',N'123 Lý Chính Thắng, Phường Võ Thị Sáu, Quận 3, TP Hồ Chí Minh','VT006',null,1,'2003/04/03','DN4631927850434',1)
	,('NV049',N'Trần Duy Thanh',null,'435678405432',N'Nam','0325678543','tranduythanh@gmail.com',N'280E14 Lương Định Của, Phường An Phú, Thành phố Thủ Đức, TP Hồ Chí Minh','VT006',null,1,'2023/12/24','DN4483950671205',1)
	,('NV050',N'Dương Văn Nghĩa',null,'454309426165',N'Nam','0435628716','duongvannghia@gmail.com',N'256/30 Phan Huy ích, Phường 12, Quận Gò Vấp, TP Hồ Chí Minh','VT006',null,1,'1999/11/13','DN4290754861345',1)	
	,('NV051',N'Tông Văn Giáp',null,'747654728716',N'Nam','0421231789','tongvangiap@gmail.com',N'156/8 Đường Số 2, Phường Tăng Nhơn Phú B, Thành phố Thủ Đức, TP Hồ Chí Minh','VT006',null,1,'2018/10/04','DN4836247105911',1)
	,('NV052',N'Thiều Thị Trà My',null,'654357654097',N'Nữ','0627187398','thieuthitramy@gmail.com',N'Số 8 Nguyễn Hậu, Phường Tân Thành, Quận Tân phú, TP Hồ Chí Minh','VT006',null,1,'2014/05/28','DN4740135862912',1)
	,('NV053',N'Đặng Mai Quỳnh',null,'988023665267',N'Nữ','0432675465','dangmaiquynh@gmail.com',N'Số 1014 Phạm Văn Đồng, Phường Hiệp Bình Chánh, Thành phố Thủ Đức, TP Hồ Chí Minh','VT006',null,1,'2002/12/22','DN4351804762934',1)
	----------Thử việc
	,('NV054',N'Vũ Văn Thanh',null,'653278402365',N'Nam','0215276176','vuvanthanh@gmail.com',N'80/59/104A Đường Dương Quảng Hàm, Phường 5, Quận Gò Vấp, TP Hồ Chí Minh','VT006',null,0,'2024/11/03',null,1)
	,('NV055',N'Phùng Thị Lê Phương',null,'652418765276',N'Nữ','0254142142','phungthilethanh@gmail.com',N'135a Trần Bá Giao, Phường 5, Quận Gò Vấp, TP Hồ Chí Minh','VT006',null,0,'2024/11/12',null,1)	
	------------------------Nhân viên lễ tân
	-----------Làm chính thức
	,('NV056',N'Nguyễn Duy Quang',null,'265172865427',N'Nam','0716209802','nguyenduyquang@gmail.com',N'56/12/5 đường Linh Đông, Phường Linh Đông, Thành phố Thủ Đức, TP Hồ Chí Minh','VT007',null,1,'2000/12/30','DN4064389215763',1)
	,('NV057',N'Bạch Thị Hà Thư',null,'241354657076',N'Nữ','0564345643','bachthihathu@gmail.com',N'1/47 đường 53, Khu phố 8, Phường Hiệp Bình Chánh, Thành phố Thủ Đức, TP Hồ Chí Minh','VT007',null,1,'2004/08/18','DN4519863742023',1)	

	,('NV058',N'Phạm Thị Kim Yến',null,'543786506545',N'Nữ','0767805475','phamthikimyen@gmail.com',N'12/2 Đường 11, Khu phố 4, Phường An Phú, Thành phố Thủ Đức, TP Hồ Chí Minh','VT007',null,1,'2010/09/20','DN4756408213954',1)
	,('NV059',N'Trần Mạnh Hùng',null,'653427602651',N'Nam','0826352065','tranmanhhung@gmail.com',N'23/12 đường 27, Khu phố 9, Phường Hiệp Bình Chánh, Thành phố Thủ Đức, TP Hồ Chí Minh','VT007',null,1,'2022/01/23','DN4186359720434',1)

	,('NV060',N'Dương Thanh Long',null,'321762892652',N'Nam','0897532421','duongthanhlong@gmail.com',N'191/4B Tây Hòa, Phường Phước Long A, Thành phố Thủ Đức, TP Hồ Chí Minh','VT007',null,1,'1999/11/13','DN4802473561978',1)	
	,('NV061',N'Nguyễn Thị Thắm',null,'653256476521',N'Nữ','0926351026','nguyenthitham@gmail.com',N'700/26/14 Quốc Lộ 13, Phường Hiệp Bình Phước, Thành phố Thủ Đức, TP Hồ Chí Minh','VT007',null,1,'2000/07/14','DN4539627084145',1)
	----------Thử việc
	,('NV062',N'Nguyễn Thanh Quân',null,'543246608654',N'Nam','0326753789','nguyenthanhquan@gmail.com',N'33 Đường Nam Hòa, Phường Phước Long A, Thành phố Thủ Đức, TP Hồ Chí Minh','VT007',null,0,'2024/11/12',null,1)
	,('NV063',N'Đỗ Tất Cường',null,'876430902565',N'Nam','0226432897','dotatcuong@gmail.com',N'45 Đinh Thị Thi, Phường Hiệp Bình Phước, Thành phố Thủ Đức, TP Hồ Chí Minh','VT007',null,0,'2024/12/20',null,1)	
	-----------------------Nhân viên vệ sinh
	-----------Làm chính thức
	,('NV064',N'Phan Quỳnh Lan',null,'736539227625',N'Nam','0273829016','phanquynhlan@gmail.com',N'Số 29 Nguyễn Đình Thi, KDC Gia Hòa, Phường Phước Long B, Thành phố Thủ Đức, TP Hồ Chí Minh','VT008',null,1,'2001/03/24','DN4631284759043',1)
	,('NV065',N'Trần Hữu Tuấn',null,'243603628719',N'Nam','0678043567','tranhuutuan@gmail.com',N'Số 22 Đường số 7, Khu đô thị Vạn Phúc, Phường Hiệp Bình Phước, Thành phố Thủ Đức, TP Hồ Chí Minh','VT008',null,1,'2005/08/03','DN4490561832763',1)	

	,('NV066',N'Nguyễn Đắc Nghĩa',null,'128372639026',N'Nam','0724376387','nguyendacnghia@gmail.com',N'23/4 Đường 27, Khu phố 9, Phường Hiệp Bình Chánh, Thành phố Thủ Đức, TP Hồ Chí Minh','VT008',null,1,'2011/05/18','DN4218904375632',1)
	,('NV067',N'Tô Minh Hương',null,'451987165065',N'Nữ','0421820765','tominhhuong@gmail.com',N'643 Phạm Văn Đồng, Phường Hiệp Bình Chánh, Thành phố Thủ Đức, TP Hồ Chí Minh','VT008',null,1,'2008/09/19','DN4157398240639',1)

	,('NV068',N'Nguyễn Đình Tời',null,'824160260276',N'Nam','0261528965','nguyendinhtoi@gmail.com',N'Số 10 đường 12, khu phố 5, Phường Hiệp Bình Chánh, Thành phố Thủ Đức, TP Hồ Chí Minh','VT008',null,1,'2010/09/20','DN4846130297527',1)
	,('NV069',N'Ngô Văn Đoan',null,'261029875027',N'Nam','0165087265','ngovandoan@gmail.com',N'39/12/1 Đường số 10, khu phố 3, Phường Linh Xuân, Thành phố Thủ Đức, TP Hồ Chí Minh','VT008',null,1,'2018/10/04','DN4362579018428',1)
	----------Thử việc
	,('NV070',N'Khổng Trọng Thắng',null,'287068152875',N'Nam','0976261518','khongtrongthang@gmail.com',N'38B, Đường 54, Phường Thảo Điền, Thành phố Thủ Đức, TP Hồ Chí Minh','VT008',null,0,'2024/11/28',null,1)
	,('NV071',N'Bùi Minh Đức',null,'628016287624',N'Nam','0720182764','buiminhduc@gmail.com',N'87/126 Đường Số 3, Phường Bình Hưng Hòa, Quận Bình Tân, TP Hồ Chí Minh','VT008',null,0,'2024/12/03',null,1)	
	-----------------------Nhân viên kho
	--------------Làm chính thức
	,('NV072',N'Vũ Tuấn Việt',null,'925160261872',N'Nam','0165027618','vutuanviet@gmail.com',N'135/17/59 Nguyễn Hữu Cảnh, Phường 22, Quận Bình Thạnh, TP Hồ Chí Minh','VT009',null,1,'2002/10/22','DN4745208631963',1)
	,('NV073',N'Phạm Ngọc Hoa',null,'872601627542',N'Nam','0825162065','phamngochoa@gmail.com',N'276 Phan Xích Long, Phường 07, Quận Phú Nhuận, TP Hồ Chí Minh','VT009',null,1,'2011/11/16','DN4904376215863',1)	

	,('NV074',N'Bùi Phương Thảo',null,'231762970265',N'Nữ','0241768643','buiphuongthao@gmail.com',N'Số 48 Nguyễn Văn Nguyễn, Phường Tân Định, Quận 1, TP Hồ Chí Minh','VT009',null,1,'2019/06/06','DN4513826740953',1)
	,('NV075',N'Phạm Thị Thu Hương',null,'324176829765',N'Nữ','0170268926','phamthithuhuong@gmail.com',N'150 Nơ Trang Long, Phường 14, Quận Bình Thạnh, TP Hồ Chí Minh','VT009',null,1,'2008/09/19','DN4067315948234',1)
	------------Thử việc
	,('NV076',N'Bùi Chí Nam',null,'652917208652',N'Nam','0985260162','buichinam@gmail.com',N'389/5/10/4 Quốc Lộ 13, Phường Hiệp Bình Phước, Thành phố Thủ Đức, TP Hồ Chí Minh','VT009',null,0,'2024/10/27',null,1)
	,('NV077',N'Nguyễn Quốc Tuấn',null,'762598276526',N'Nam','0652861082','nguyenquoctuan@gmail.com',N'06 Giải Phóng, Phường 4, Quận Tân Bình, TP Hồ Chí Minh','VT009',null,0,'2024/12/11',null,1)	
	------------------------Quản lý ca
	,('NV078',N'Lê Thị Phương Lan',null,'726570254819',N'Nữ','0725148729','lethiphuonglan@gmail.com',N'163/67 Thành Thái, Phường 14, Quận 10, TP Hồ Chí Minh','VT010','NV001',1,'2005/03/11','DN4498215637063',1)
	,('NV079',N'Nguyễn Thị Minh Phương',null,'987253410265',N'Nữ','0652716276','nguyenthiminhphuong@gmail.com',N'439/22/10 Đường Nguyễn Văn Khối, Phường 8, Quận Gò Vấp, TP Hồ Chí Minh','VT010','NV001',1,'2013/05/21','DN4867930124532',1)	

	,('NV080',N'Trần Thị Phương Thúy',null,'656706871675',N'Nữ','0752027654','tranthiphuongthuy@gmail.com',N'262/54 A Tôn Thất Thuyết, phường 03, Quận 4, TP Hồ Chí Minh','VT010','NV001',1,'2022/07/23','DN4431760582937',1)
	,('NV081',N'Phan Quang Đoàn',null,'652401627526',N'Nam','0765475065','phanquangdoan@gmail.com',N'595 Lê Văn Thọ, Phường 14, Quận Gò Vấp, TP Hồ Chí Minh','VT010','NV001',1,'1999/12/13','DN4580139624756',1)

	,('NV082',N'Nguyễn Văn Quyết',null,'635202802763',N'Nam','0782465272','nguyenvanquyet@gmail.com',N'600/12/25 Kinh Dương Vương, Phường An Lạc, Quận Bình Tân, TP Hồ Chí Minh','VT010','NV001',1,'2010/05/20','DN4296047581345',1)
	,('NV083',N'Dương Thế Vinh',null,'250268392768',N'Nam','0578664296','duongthevinh@gmail.com',N'833 Lê Hồng Phong, Phường 12, Quận 10, TP Hồ Chí Minh','VT010','NV001',1,'2022/07/26','DN4362850479142',1)	

	,('NV084',N'Nguyễn Thị Kim Mai',null,'542760180265',N'Nữ','0926878697','nguyenthikimmai@gmail.com',N'536/15/14 Lê Văn Sỹ, Phường 11, Quận Phú Nhuận, TP Hồ Chí Minh','VT010','NV001',1,'2023/07/19','DN4519743206883',1)
	,('NV085',N'Chu Quốc Trường',null,'251986527627',N'Nam','0826152769','chuquoctruong@gmail.com',N'84 Đường số 3, Phường Trường Thọ, Thành phố Thủ Đức, TP Hồ Chí Minh','VT010','NV001',1,'2018/06/28','DN4841967325035',1)

INSERT INTO [dbo].[SoLuongTrongCa] ([MaQuanLy],[MaCa],[Ngay],[SoLuongToiDa])
VALUES
--('QL20250209001','C001','2025/02/09',20)
	 ('QL20241222001','C001','2024/12/22',20)
	,('QL20241222002','C002','2024/12/22',23)

	,('QL20241222003','C003','2024/12/22',2)
	,('QL20241222004','C004','2024/12/22',3)
	,('QL20241222005','C005','2024/12/22',3)
	,('QL20241222006','C006','2024/12/2',3)
	----------------------------------------
	,('QL20241223001','C001','2024/12/23',19)
	,('QL20241223002','C002','2024/12/23',19)
	,('QL20241223003','C003','2024/12/23',2)
	,('QL20241223004','C004','2024/12/23',2)
	,('QL20241223005','C005','2024/12/23',2)
	,('QL20241223006','C006','2024/12/23',2)

INSERT INTO [dbo].[SoLuongChiTietTrongCa] ([MaQuanLy],[MaQuanLyChiTiet],[MaViTriCV],[SoLuong])
VALUES	
--('QL20250209001','QL20250209001.1','VT003',1)
	 ('QL20241222001','QL20241222001.1','VT002',1)
	,('QL20241222001','QL20241222001.2','VT003',2)
	,('QL20241222001','QL20241222001.3','VT004',4)
	,('QL20241222001','QL20241222001.4','VT005',5)
	,('QL20241222001','QL20241222001.5','VT006',2)
	,('QL20241222001','QL20241222001.6','VT007',2)
	,('QL20241222001','QL20241222001.7','VT008',2)
	,('QL20241222001','QL20241222001.8','VT009',1)
	,('QL20241222001','QL20241222001.9','VT010',1)

	,('QL20241222002','QL20241222002.1','VT002',2)
	,('QL20241222002','QL20241222002.2','VT003',3)
	,('QL20241222002','QL20241222002.3','VT004',5)
	,('QL20241222002','QL20241222002.4','VT005',6)
	,('QL20241222002','QL20241222002.5','VT006',2)
	,('QL20241222002','QL20241222002.6','VT007',2)
	,('QL20241222002','QL20241222002.7','VT008',1)
	,('QL20241222002','QL20241222002.8','VT009',1)
	,('QL20241222002','QL20241222002.9','VT010',1)

	
	,('QL20241222003','QL20241222003.1','VT002',0)
	,('QL20241222003','QL20241222003.2','VT003',0)
	,('QL20241222003','QL20241222003.3','VT004',1)
	,('QL20241222003','QL20241222003.4','VT005',1)
	,('QL20241222003','QL20241222003.5','VT006',0)
	,('QL20241222003','QL20241222003.6','VT007',0)
	,('QL20241222003','QL20241222003.7','VT008',0)
	,('QL20241222003','QL20241222003.8','VT009',0)
	,('QL20241222003','QL20241222003.9','VT010',0)

	,('QL20241222004','QL20241222004.1','VT002',0)
	,('QL20241222004','QL20241222004.2','VT003',0)
	,('QL20241222004','QL20241222004.3','VT004',1)
	,('QL20241222004','QL20241222004.4','VT005',2)
	,('QL20241222004','QL20241222004.5','VT006',0)
	,('QL20241222004','QL20241222004.6','VT007',0)
	,('QL20241222004','QL20241222004.7','VT008',0)
	,('QL20241222004','QL20241222004.8','VT009',0)
	,('QL20241222004','QL20241222004.9','VT010',0)

	,('QL20241222005','QL20241222005.1','VT002',0)
	,('QL20241222005','QL20241222005.2','VT003',0)
	,('QL20241222005','QL20241222005.3','VT004',1)
	,('QL20241222005','QL20241222005.4','VT005',2)
	,('QL20241222005','QL20241222005.5','VT006',0)
	,('QL20241222005','QL20241222005.6','VT007',1)
	,('QL20241222005','QL20241222005.7','VT008',0)
	,('QL20241222005','QL20241222005.8','VT009',0)
	,('QL20241222005','QL20241222005.9','VT010',0)

	,('QL20241222006','QL20241222006.1','VT002',0)
	,('QL20241222006','QL20241222006.2','VT003',0)
	,('QL20241222006','QL20241222006.3','VT004',1)
	,('QL20241222006','QL20241222006.4','VT005',2)
	,('QL20241222006','QL20241222006.5','VT006',0)
	,('QL20241222006','QL20241222006.6','VT007',1)
	,('QL20241222006','QL20241222006.7','VT008',0)
	,('QL20241222006','QL20241222006.8','VT009',0)
	,('QL20241222006','QL20241222006.9','VT010',0)

	------2024/12/13
	,('QL20241223001','QL20241223001.1','VT002',1)
	,('QL20241223001','QL20241223001.2','VT003',2)
	,('QL20241223001','QL20241223001.3','VT004',3)
	,('QL20241223001','QL20241223001.4','VT005',4)
	,('QL20241223001','QL20241223001.5','VT006',2)
	,('QL20241223001','QL20241223001.6','VT007',2)
	,('QL20241223001','QL20241223001.7','VT008',2)
	,('QL20241223001','QL20241223001.8','VT009',1)
	,('QL20241223001','QL20241223001.9','VT010',1)

	,('QL20241223002','QL20241223002.1','VT002',2)
	,('QL20241223002','QL20241223002.2','VT003',2)
	,('QL20241223002','QL20241223002.3','VT004',4)
	,('QL20241223002','QL20241223002.4','VT005',4)
	,('QL20241223002','QL20241223002.5','VT006',2)
	,('QL20241223002','QL20241223002.6','VT007',2)
	,('QL20241223002','QL20241223002.7','VT008',1)
	,('QL20241223002','QL20241223002.8','VT009',1)
	,('QL20241223002','QL20241223002.9','VT010',1)
	
	,('QL20241223003','QL20241223003.1','VT002',0)
	,('QL20241223003','QL20241223003.2','VT003',0)
	,('QL20241223003','QL20241223003.3','VT004',1)
	,('QL20241223003','QL20241223003.4','VT005',1)
	,('QL20241223003','QL20241223003.5','VT006',0)
	,('QL20241223003','QL20241223003.6','VT007',0)
	,('QL20241223003','QL20241223003.7','VT008',0)
	,('QL20241223003','QL20241223003.8','VT009',0)
	,('QL20241223003','QL20241223003.9','VT010',0)

	,('QL20241223004','QL20241223004.1','VT002',0)
	,('QL20241223004','QL20241223004.2','VT003',0)
	,('QL20241223004','QL20241223004.3','VT004',1)
	,('QL20241223004','QL20241223004.4','VT005',1)
	,('QL20241223004','QL20241223004.5','VT006',0)
	,('QL20241223004','QL20241223004.6','VT007',0)
	,('QL20241223004','QL20241223004.7','VT008',0)
	,('QL20241223004','QL20241223004.8','VT009',0)
	,('QL20241223004','QL20241223004.9','VT010',0)
	,('QL20241223005','QL20241223005.1','VT002',0)
	,('QL20241223005','QL20241223005.2','VT003',0)
	,('QL20241223005','QL20241223005.3','VT004',1)
	,('QL20241223005','QL20241223005.4','VT005',1)
	,('QL20241223005','QL20241223005.5','VT006',0)
	,('QL20241223005','QL20241223005.6','VT007',1)
	,('QL20241223005','QL20241223005.7','VT008',0)
	,('QL20241223005','QL20241223005.8','VT009',0)
	,('QL20241223005','QL20241223005.9','VT010',0)
	,('QL20241223006','QL20241223006.1','VT002',0)
	,('QL20241223006','QL20241223006.2','VT003',0)
	,('QL20241223006','QL20241223006.3','VT004',1)
	,('QL20241223006','QL20241223006.4','VT005',1)
	,('QL20241223006','QL20241223006.5','VT006',0)
	,('QL20241223006','QL20241223006.6','VT007',1)
	,('QL20241223006','QL20241223006.7','VT008',0)
	,('QL20241223006','QL20241223006.8','VT009',0)
	,('QL20241223006','QL20241223006.9','VT010',0)

---8 tiêng là HD001 VÀ 002
INSERT INTO [dbo].[DangKyCa] ([MaQuanLy],[MaDangKy],[MaNV],[Ngay])
VALUES	
--('QL20250209001','DK20250209001','NV007','2024/02/02')
	 ('QL20241222001','DK20241222001','NV002','2024/12/12')
	,('QL20241222001','DK20241222002','NV006','2024/12/12')
	,('QL20241222001','DK20241222003','NV008','2024/12/18')
	,('QL20241222001','DK20241222004','NV019','2024/12/17')
	,('QL20241222001','DK20241222005','NV020','2024/12/16')
	,('QL20241222001','DK20241222006','NV018','2024/12/15')
	,('QL20241222001','DK20241222007','NV022','2024/12/12')
	,('QL20241222001','DK20241222008','NV027','2024/12/13')
	,('QL20241222001','DK20241222009','NV030','2024/12/19')
	,('QL20241222001','DK20241222010','NV033','2024/12/19')
	,('QL20241222001','DK20241222011','NV036','2024/12/20')
	,('QL20241222001','DK20241222012','NV041','2024/12/13')
	,('QL20241222001','DK20241222013','NV048','2024/12/14')
	,('QL20241222001','DK20241222014','NV054','2024/12/12')
	,('QL20241222001','DK20241222015','NV057','2024/12/15')
	,('QL20241222001','DK20241222016','NV062','2024/12/16')
	,('QL20241222001','DK20241222017','NV065','2024/12/17')
	,('QL20241222001','DK20241222018','NV068','2024/12/18')
	,('QL20241222001','DK20241222019','NV072','2024/12/12')
	,('QL20241222001','DK20241222020','NV078','2024/12/14')

	,('QL20241222002','DK20241222021','NV003','2024/12/12')
	,('QL20241222002','DK20241222022','NV005','2024/12/12')
	,('QL20241222002','DK20241222023','NV007','2024/12/18')
	,('QL20241222002','DK20241222024','NV008','2024/12/17')
	,('QL20241222002','DK20241222025','NV013','2024/12/16')
	,('QL20241222002','DK20241222026','NV019','2024/12/15')
	,('QL20241222002','DK20241222027','NV020','2024/12/12')
	,('QL20241222002','DK20241222028','NV024','2024/12/13')
	,('QL20241222002','DK20241222029','NV021','2024/12/19')
	,('QL20241222002','DK20241222030','NV025','2024/12/19')
	,('QL20241222002','DK20241222031','NV027','2024/12/20')
	,('QL20241222002','DK20241222032','NV029','2024/12/13')
	,('QL20241222002','DK20241222033','NV039','2024/12/14')
	,('QL20241222002','DK20241222034','NV032','2024/12/12')
	,('QL20241222002','DK20241222035','NV035','2024/12/15')
	,('QL20241222002','DK20241222036','NV045','2024/12/16')
	,('QL20241222002','DK20241222037','NV048','2024/12/17')
	,('QL20241222002','DK20241222038','NV055','2024/12/18')
	,('QL20241222002','DK20241222039','NV072','2024/12/12')
	,('QL20241222002','DK20241222040','NV063','2024/12/14')
	,('QL20241222002','DK20241222041','NV065','2024/12/15')
	,('QL20241222002','DK20241222042','NV075','2024/12/19')
	,('QL20241222002','DK20241222043','NV079','2024/12/17')

	,('QL20241222003','DK20241222044','NV024','2024/12/16')
	,('QL20241222003','DK20241222045','NV031','2024/12/13')

	,('QL20241222004','DK20241222046','NV017','2024/12/17')
	,('QL20241222004','DK20241222047','NV028','2024/12/18')
	,('QL20241222004','DK20241222048','NV034','2024/12/12')

	,('QL20241222005','DK20241222049','NV015','2024/12/13')
	,('QL20241222005','DK20241222050','NV044','2024/12/15')
	,('QL20241222005','DK20241222051','NV037','2024/12/18')
	,('QL20241222005','DK20241222052','NV061','2024/12/14')

	,('QL20241222006','DK20241222053','NV010','2024/12/13')
	,('QL20241222006','DK20241222054','NV028','2024/12/15')
	,('QL20241222006','DK20241222055','NV042','2024/12/18')
	,('QL20241222006','DK20241222056','NV058','2024/12/14')

INSERT INTO [dbo].[DangKyCa] ([MaQuanLy],[MaDangKy],[MaNV],[Ngay])
VALUES		
	 ('QL20241223001','DK20241223001','NV002','2024/12/12')
	,('QL20241223001','DK20241223002','NV006','2024/12/12')
	,('QL20241223001','DK20241223003','NV008','2024/12/18')
	,('QL20241223001','DK20241223004','NV019','2024/12/17')
	,('QL20241223001','DK20241223005','NV020','2024/12/16')
	,('QL20241223001','DK20241223006','NV018','2024/12/15')
	,('QL20241223001','DK20241223007','NV022','2024/12/12')
	,('QL20241223001','DK20241223008','NV027','2024/12/13')
	,('QL20241223001','DK20241223009','NV030','2024/12/19')
	,('QL20241223001','DK20241223010','NV033','2024/12/19')
	,('QL20241223001','DK20241223011','NV048','2024/12/14')
	,('QL20241223001','DK20241223012','NV054','2024/12/12')
	,('QL20241223001','DK20241223013','NV057','2024/12/15')
	,('QL20241223001','DK20241223014','NV062','2024/12/16')
	,('QL20241223001','DK20241223015','NV065','2024/12/17')
	,('QL20241223001','DK20241223016','NV068','2024/12/18')
	,('QL20241223001','DK20241223017','NV072','2024/12/12')
	,('QL20241223001','DK20241223018','NV078','2024/12/14')
	,('QL20241223002','DK20241223019','NV003','2024/12/12')
	,('QL20241223002','DK20241223020','NV005','2024/12/12')
	,('QL20241223002','DK20241223021','NV007','2024/12/18')
	,('QL20241223002','DK20241223022','NV008','2024/12/17')
	,('QL20241223002','DK20241223023','NV013','2024/12/16')
	,('QL20241223002','DK20241223024','NV019','2024/12/15')
	,('QL20241223002','DK20241223025','NV020','2024/12/12')
	,('QL20241223002','DK20241223026','NV024','2024/12/13')
	,('QL20241223002','DK20241223027','NV027','2024/12/20')
	,('QL20241223002','DK20241223028','NV029','2024/12/13')
	,('QL20241223002','DK20241223029','NV039','2024/12/14')
	,('QL20241223002','DK20241223030','NV032','2024/12/12')
	,('QL20241223002','DK20241223031','NV048','2024/12/17')
	,('QL20241223002','DK20241223032','NV055','2024/12/18')	
	,('QL20241223002','DK20241223033','NV063','2024/12/14')
	,('QL20241223002','DK20241223034','NV060','2024/12/16')
	,('QL20241223002','DK20241223035','NV065','2024/12/15')
	,('QL20241223002','DK20241223036','NV075','2024/12/19')
	,('QL20241223002','DK20241223037','NV079','2024/12/17')
	,('QL20241223003','DK20241223038','NV024','2024/12/16')
	,('QL20241223003','DK20241223039','NV031','2024/12/13')

	,('QL20241223004','DK20241223040','NV017','2024/12/17')
	,('QL20241223004','DK20241223041','NV028','2024/12/18')

	,('QL20241223005','DK20241223042','NV015','2024/12/13')
	,('QL20241223005','DK20241223043','NV044','2024/12/15')
	,('QL20241223005','DK20241223044','NV061','2024/12/14')

	,('QL20241223006','DK20241223045','NV012','2024/12/13')
	,('QL20241223006','DK20241223046','NV028','2024/12/15')
	,('QL20241223006','DK20241223047','NV061','2024/12/14')

INSERT INTO [dbo].[DatBan] ([MaDatBan],[MaBan],[SoNguoiDi],[TenKH],[SDT],[NgayDatBan])
VALUES
	 ('DB20241222001','B001',3,N'Nguyễn Quốc Tuấn','0563287185','2024/12/22 19:00')
	,('DB20241222002','B002',4,null,null,'2024/12/22 10:15')
	,('DB20241222003','B003',3,N'Lê Thị Phương Lan','0873261726','2024/12/22 20:30')
	,('DB20241222004','B004',1,null,null,'2024/12/22 12:07')
	,('DB20241222005','B005',5,N'Phan Quang Đoàn','0241865267','2024/12/22 08:40')
	,('DB20241222006','B006',5,null,null,'2024/12/22 11:30')
	,('DB20241222007','B007',2,N'Nguyễn Thị Kim Mai','0765975487','2024/12/22 15:18')
	,('DB20241222008','B008',4,null,null,'2024/12/22 13:20')
	,('DB20241222009','B009',1,N'Dương Thế Vinh','0241629817','2024/12/22 21:30')
	,('DB20241222010','B010',6,null,null,'2024/12/22 19:15')

	,('DB20241223001','B011',2,N'Lại Đỗ Quyên','0251871826','2024/12/23 19:00')
	,('DB20241223002','B012',3,null,null,'2024/12/23 10:15')
	,('DB20241223003','B013',4,N'Lê Minh Việt','0964206417','2024/12/23 20:30')
	,('DB20241223004','B014',3,null,null,'2024/12/23 12:07')
	,('DB20241223005','B015',5,N'Trần Thị Thu Hiền','0926152716','2024/12/23 08:40')
	,('DB20241223006','B016',2,null,null,'2024/12/23 11:30')
	,('DB20241223007','B017',6,N'An Thành Công','0726151872','2024/12/23 15:18')
	,('DB20241223008','B018',4,null,null,'2024/12/23 13:20')
	,('DB20241223009','B019',1,N'Đoàn Mai Phương','0265163615','2024/12/23 21:30')
	,('DB20241223010','B020',4,null,null,'2024/12/23 19:15')
--Tài khoản
INSERT INTO [dbo].[TaiKhoan] ([TaiKhoan],[MatKhau],[MaNV])
VALUES	
	 ('NV001','$2a$11$yAj7Me2.nzooAo/LiNoKg.Kp4AkYn2GEJN3LphYFa.XOP4eyWzsGC','NV001')
	,('NV002','$2a$11$yVEYc.OPhJZGY.S.BPZ.2.SO0XtXYbPd8OuH3fEp8nbjlvdDyMzty','NV002')
	,('NV003','$2a$11$cC.paQWQeH0aMPbEXwzsFO7L.b5NvXYz0iorZpn4.3PmPGQl7QfVm','NV003')
	,('NV004','$2a$11$B6eMKMYfqw4EJCBIVaORp.W/8V6uENfvnsGx4KgYtJ.nAOiQJotUS','NV004')
	,('NV005','$2a$11$MVcNXtoOAEtc9IyMSIGP0ee5MiskT/LN5bgmPPcRh5BSGmFskoR2C','NV005')
	,('NV006','$2a$11$itgv.3N0fhq/prBtDg1qOepR6NwyF6KkzfIcbl/U8HjbuCSCkXpDq','NV006')
	,('NV007','$2a$11$aJ3zK4J6GZBt6dVmWtfDMuHZ0Ys7bHRBomI1zmFA3cTZQvH..Sk8e','NV007')
	,('NV008','$2a$11$vH75h4SD2yD17drq/2H5Ju6xcxrlZWHSBCyiwKs7qJgpUiq6ulIJO','NV008')
	,('NV009','$2a$11$cT7ev9K68n3toxRxkvss6OFgEDAqQktz02JBb5/aDKQPCKSOnntf6','NV009')
	,('NV010','$2a$11$uKzgjzKStUfFRW2uCu73beRj8.pInCZBvaFyQWLqOUSV0WWFSVWeG','NV010')
	,('NV011','$2a$11$ELZ/GtofzdVnnsTTVcLAuOq8m6rtJk/Mt.J9NlN7Np8BxT8KKetmq','NV011')
	,('NV012','$2a$11$bkOMSMLb1PbJErg4BEYawue3190nOlDL0nSPXEi1xCH0lyqptiQSi','NV012')
	,('NV013','$2a$11$POJm.vf/.O6IjqOZuSwah.GJutBuNeSttpEgksDPZma1evsUcEWDC','NV013')
	,('NV014','$2a$11$e15DImMgx2ZmX/wtHb90N.V29wzmOJI1s2dM62d9a4eX67e8KPiRS','NV014')
	,('NV015','$2a$11$UKvI4VVfibIP2295.x.Lfe9jCjhToD39/6vWxHws2W64pCEcb4KTy','NV015')
	,('NV016','$2a$11$veKA708XVYDD22qNYyTHNOLvEGMuaEGBbX.6aLAct1VzN7fXZ/MKS','NV016')
	,('NV017','$2a$11$74bkNdIfEw/3hLX5p7DEX.oNmyL4seTCtjG6rcZrEeC8Z40l7Hy.i','NV017')
	,('NV018','$2a$11$TgktLxMGHDWl0./ljOwow.4rXMawrrbOmv84Ft463gA8tYsm0TFKS','NV018')
	,('NV019','$2a$11$PoAazE6.zvaNgf8gb7XRK.cfa9Kc4GtnprneGIU/vuicx.cW5Y9jC','NV019')
	,('NV020','$2a$11$So858f.ViLVJLUiGbyJS/.mrpARWPoSQG1D115j9x4iOqsNTyfVVy','NV020')
	,('NV021','$2a$11$Bc9ReddT4XpCsvujfNYl6eEUZg/pzax5jIQ2DjhY3XDNqenp087Pm','NV021')
	,('NV022','$2a$11$PD7SvmA8fICqVU1b044hCOGBZml.M0GLNDK0dJFPiAe512Re8yjJ.','NV022')
	,('NV023','$2a$11$xh6pvJ.thyU./p2omdLDJOKi/XeW3lM3isyVCPpYZMcGNn2AkVUpO','NV023')
	,('NV024','$2a$11$Ir9p5rCyzAmx1GFDb7om4ud0Tus6l/tw2ChrFTaOv6FkOlYOMVEn.','NV024')
	,('NV025','$2a$11$snHks4mhwPkui9lrEDrx.e/b7j3WPWm47TggrlIAhDv1EtSxzbcfW','NV025')
	,('NV026','$2a$11$YwQ.STro71MTJJuhueel..f7RNcnUYre9.ugFUJiBaW/WL.vqN4KO','NV026')
	,('NV027','$2a$11$VfRH..kfEFZI4jBRxIm9oeR.y5RK64kbUAGemSJNNBL.jU7m.Tb.a','NV027')
	,('NV028','$2a$11$XgfDhZkMF7iryq0XjRKJfun3HqoHDRJrz.3nsVSz8UnF4U6yJm0Om','NV028')
	,('NV029','$2a$11$jvti8fiAjUe7Z3J1z/AxiOhRae/.7/X3RzBtGSX.U73CKeDn1ed.W','NV029')
	,('NV030','$2a$11$zW7KzXn8/KHF/EUQlYILP.XcODup9NOmaIUc0TclZ5QyJn5pKF5gS','NV030')
	,('NV031','$2a$11$.qFDcW2CaDWpfaXa02vJ0eICI/4W.Xzrqlqs3N5/jysrdbXC2V5ZO','NV031')
	,('NV032','$2a$11$mC5Q5YQAOe8TfpvACE.uGO9EpFPw7qoclesZ09W4PkswlzcfUjqEe','NV032')
	,('NV033','$2a$11$gXduqTtWTCMkISO.hXbAR.i2I8W6fn87ePQKHUeE6sI0J46Zd.rKq','NV033')
	,('NV034','$2a$11$n9t.SCGa6BW4dfi7J5w7H.HK58rmx.yQ1lETGrXgQevHlzcz.aXd6','NV034')
	,('NV035','$2a$11$A.b999.vHfJNJ84RKFp95e4RKsKfbspgxmw/kQ3iB9kP.EQwxRHLO','NV035')
	,('NV036','$2a$11$i5xipT5UIp2L0I3WPEstguvZ4.eSPlJOVhTTClXyqJTSORtAU6YCC','NV036')
	,('NV037','$2a$11$Qu.r4sle3YJa7EyxZcgqMOi.T4HKgAx.gTQORkEl6cEWM4gA7L1fm','NV037')
	,('NV038','$2a$11$Xd/Ne9qluIumSLHFe.BxgOLeMqKww3zIDxOIsUyo4YpAyKb5s.bdy','NV038')
	,('NV039','$2a$11$rnz7M4Yx8HbB0mlv66yzMOwKtd7U/Tfn5zNxvV2v6Wyc/Yibhz60e','NV039')
	,('NV040','$2a$11$D/RRneozOAgUQQtnuF6jXODHT7G1k5FsOdfmUqFowWaFNQlWVxly.','NV040')
	,('NV041','$2a$11$3kMuSeqhJ/t6euMqsa9Cy.H7Ty.kgzTSpJcZtYXsdvlqW0z7MWHs6','NV041')
	,('NV042','$2a$11$0cxTrg77c.Fw64f.2o91yOoZmCHjciVldeF0sQ/Slc2ZSQTVrtMYK','NV042')
	,('NV043','$2a$11$yM93QnHBVLPkZd0tVY5FEelIW9eoKtFvvbqUUje11.CZfHKcoDYO2','NV043')
	,('NV044','$2a$11$kzk.voVF5KvK47DgWqC3cukZG1ZpZ04fDMvYB3JsxvhlLlA7nxxl.','NV044')
	,('NV045','$2a$11$Jp2PCn6FbaKF/m1OB1uTy.xleavyBLJRO8iYENkVjsQpQTHdd.mTW','NV045')
	,('NV046','$2a$11$zZ/zRF9SVSADalqI1/0K/OrGMV5Jj36ggv3Lzx0mIb0sHnYRTnDtm','NV046')
	,('NV047','$2a$11$I2H1T3BvY1G02wQU0QhwiuRMce/BUUWc2yEy5vb2l9cPbDRUkYLmu','NV047')
	,('NV048','$2a$11$XGMetRpxNhSZqzXrBOpU1.QQnXk2XvvHBkq8r3FSPDLSINZMZpBeS','NV048')
	,('NV049','$2a$11$xhsmaCIDxRG1uSAFOCkfQONnkWqfh125NBXsXEh6xS03EPxm7iBbG','NV049')
	,('NV050','$2a$11$aqL2qWOqKSwyVNASqyuvWOhgRnYDMLXKeVBscENj8pIiYWUAcerDG','NV050')
	,('NV051','$2a$11$QRu2WHQWiD3YMeaGlH7/beGDiwTrlgG7dVuZGzVJ5hyLebuhTMN4i','NV051')
	,('NV052','$2a$11$zgeDHmK3vzeW733Ob2DFoOUCvJfVK58V5RnpOeKcl/UDn.WSKcI8q','NV052')
	,('NV053','$2a$11$koWwKqfcjk./MCQ48VGZvumNIZY.Wwsm.20sEKnx94sXy6gp4Fhdu','NV053')
	,('NV054','$2a$11$CQ8mQnneVEX.u6FTmRCNIee9j3n0zEwx4cJxWLBWr5E.P1ekovJYy','NV054')
	,('NV055','$2a$11$VMi26n4Zw.t48v5D03ue4OP.T/ZRZrr/WsSsqi6it2oKoT6yFeJ2O','NV055')
	,('NV056','$2a$11$mXvRJdiCX1qZnlSZsU3vCeT.y8Spzjm2p6Mdrh5yVuNmFMb8XAz96','NV056')
	,('NV057','$2a$11$cgNTOoKV/zQfvj2tYrD6duR0oFiKSFVjkAKNGoVRVffTyeakX3xae','NV057')
	,('NV058','$2a$11$1rNVdZMBU5QRArUqGF3vj.1HU2HyKpvy/G2MiBugKR/ty8f3OSBwG','NV058')
	,('NV059','$2a$11$bb937LF2dHo9oR6ZTsgOL.Y3zqswwtkeHTO6SYlehIXkrMY1J5L8a','NV059')
	,('NV060','$2a$11$nO6fcG/wQN6HGzo2q0LC9uGvr9fEr1BdgVJ/hvVE623TJsg87kvHO','NV060')
	,('NV061','$2a$11$h1LPEbS8JG6AiKrngCxYJO/vadzlw20RAISlEUBzqMVJGcm23YPpS','NV061')
	,('NV062','$2a$11$mPqPFnNVMYy0W0/ZgEBbCedbQTnLi8il7oJv6JW0QH7ufVyxZTUY.','NV062')
	,('NV063','$2a$11$Ru4UHPzZ90Jum1SOPN.MPe8YMo35IYLX17SjtZHKfqp5HWGbPPKfq','NV063')
	,('NV064','$2a$11$aOPf9crBnLJYdMM/IHnogej5pvwIgSKtR4RZuE3tXa4ofnLhntk9u','NV064')
	,('NV065','$2a$11$KtDmRAzyyw6aDS1ZBTcZAuXSk99PUdoV4AifMCsAm00oX4MZB60Je','NV065')
	,('NV066','$2a$11$CegtI9SLZKRI5O.i8.iHH.WgFURE8QFOjiXPKoHtlGa71YY92O3Ia','NV066')
	,('NV067','$2a$11$CtPiPZo.cmLKuVs3N9XDI.jtiRC5n2jr3zZCFH0pS/2aHDNIpCBv6','NV067')
	,('NV068','$2a$11$zRLfqrcbjSoTUWwkySvFDe0LPTLPXWfJVvqdeH.u22WmXGZW42k2a','NV068')
	,('NV069','$2a$11$ZW2bu4H0o/R0iwpKDjQQY.VG8ahFHgvoCNIiZL2lPYhuTgERH7fI.','NV069')
	,('NV070','$2a$11$vxwYuWxSXKsFPMK.XfoHXubgSaDQBEcYvYJe28kwA9YjJm3qIFbVu','NV070')
	,('NV071','$2a$11$GYa2NXVTcMIxS6LKgQNTOeg2YvFQbb2vbj4xuaNcxCT8wRY7hLTtS','NV071')
	,('NV072','$2a$11$sk0uLtF2WCq67A5bIF/8XuLfDHRx2NK6N9X6CW48uSANLzs4XgM..','NV072')
	,('NV073','$2a$11$Oj7LjMOtuzC6sigQBVkjHuP7qkhv029JrpW5/zMSWygA1lVBzUiyG','NV073')
	,('NV074','$2a$11$2bXdcP5y7tumPyqBiZuGuuZ5Mv.Eb2/R.sWgWEB5CKbyW3VL1X9U.','NV074')
	,('NV075','$2a$11$C4bhtWqPAfiH67OH0EH1reNELIVjyLVgorXgmUCrblx2AK36pi8NG','NV075')
	,('NV076','$2a$11$Yovrwr/DYqAk8xo7MI1PyO1oc1FguGgBUOEtFdhfm2.SPtNJZlktm','NV076')
	,('NV077','$2a$11$5zkhRmp84d1HvV7P31nRzOmu57Rx6ZC/6VrPZygnna5kTTtpjrCUa','NV077')
	,('NV078','$2a$11$F8eTiqq9JEe.dls/xJyb3u2pxeX2msTBamTKglQ/bfamdFxG1QjGa','NV078')
	,('NV079','$2a$11$IaGUJsoWZHEbeN6tn2m4bOesjHAReU6ZrgqAb8rkSCbhQC2LSsw6S','NV079')
	,('NV080','$2a$11$mCk9XY8Ygdhvg7or0w7YdO1qMyAFvBnsPT8OtkL5avDdfq/SDnhUG','NV080')
	,('NV081','$2a$11$Ew78oEV/nqjP1Zr9UkYu5Os3aRPGHUP7PgXR403dGoxjBN02znZQO','NV081')
	,('NV082','$2a$11$1I9aSQVZQ4hksv.sPvDhlOs26Xgu3La9e/7pvr/r3F3mt6Vc8TPxa','NV082')
	,('NV083','$2a$11$9CiFbgifsisBcQWfIxt.Mu0TfdNOXUVBgJrhvDfHLgwrB.ptl3WDm','NV083')
	,('NV084','$2a$11$/LRdW4q/q4R0ebSLGAPLRecwQcMgC8vJapnndObimKbEtPglnt.v6','NV084')
	,('NV085','$2a$11$fRjX4tT.SI5dU3ACgpJhtu9L5ELFe.eoI0eyViqpkbxpmkoaE.8EO','NV085')

INSERT INTO [dbo].[LichSuChuyenBan] ([MaChuyenBan],[MaDatBan],[MaBanCu],[MaBanMoi],[ThoiGianChuyen],[MaNV],[LyDoChuyen])
VALUES
	 ('CB20241222001','DB20241222001','B021','B001','2024/12/22 19:15','NV035',N'Bàn bị hỏng')
	,('CB20241222002','DB20241222004','B022','B004','2024/12/22 12:15','NV027',N'Bàn ở nơi có ánh sáng quá mạnh')
	,('CB20241222003','DB20241222008','B029','B008','2024/12/22 13:25','NV035',N'Số lượng người thay đổi')
	,('CB20241232001','DB20241223002','B025','B012','2024/12/22 10:20','NV033',N'Tiện di chuyển do có người già')
	,('CB20241232002','DB20241223004','B029','B014','2024/12/22 12:15','NV028',N'Số lượng người thay đổi')
	,('CB20241232003','DB20241223007','B043','B017','2024/12/22 15:25','NV044',N'Bàn bị hỏng')

INSERT INTO [dbo].[HoaDon]([MaHoaDon],[MaDatBan],[NgayXuatHD],[TongTien],[TrangThai])
VALUES
	 ('HD20241222001','DB20241222001','2024/12/22 19:45',1155000,N'Đã thanh toán')
	,('HD20241222002','DB20241222002','2024/12/22 10:45',2480000,N'Đã thanh toán')
	,('HD20241222003','DB20241222003','2024/12/22 21:15',2010000,N'Đã thanh toán')
	,('HD20241222004','DB20241222004','2024/12/22 12:35',380000,N'Đã thanh toán')
	,('HD20241222005','DB20241222005','2024/12/22 09:15',3885000,N'Đã thanh toán')
	,('HD20241222006','DB20241222006','2024/12/22 12:00',3885000,N'Đã thanh toán')
	,('HD20241222007','DB20241222007','2024/12/22 15:50',1130000,N'Đã thanh toán')
	,('HD20241222008','DB20241222008','2024/12/22 14:00',1130000,N'Đã thanh toán')
	,('HD20241222009','DB20241222009','2024/12/22 21:45',440000,N'Đã thanh toán')
	,('HD20241222010','DB20241222010','2024/12/22 19:45',5480000,N'Đã thanh toán')


	,('HD20241223001','DB20241223001','2024/12/23 19:45',600000,N'Đã thanh toán')
	,('HD20241223002','DB20241223002','2024/12/23 10:45',1860000,N'Đã thanh toán')
	,('HD20241223003','DB20241223003','2024/12/23 21:15',1480000,N'Đã thanh toán')
	,('HD20241223004','DB20241223004','2024/12/23 12:35',1620000,N'Đã thanh toán')
	,('HD20241223005','DB20241223005','2024/12/23 09:15',3885000,N'Đã thanh toán')
	,('HD20241223006','DB20241223006','2024/12/23 12:00',1280000,N'Đã thanh toán')
	,('HD20241223007','DB20241223007','2024/12/23 15:50',4220000,N'Đã thanh toán')
	,('HD20241223008','DB20241223008','2024/12/23 14:00',1130000,N'Đã thanh toán')
	,('HD20241223009','DB20241223009','2024/12/23 21:45',440000,N'Đã thanh toán')
	,('HD20241223010','DB20241223010','2024/12/23 19:45',2520000,N'Đã thanh toán')



INSERT INTO [dbo].[HoaDonChiTiet] ([MaHoaDon],[MaMonAn],[SoLuong],[Gia])
VALUES	
	  ('HD20241222001','MA001',3,90000)
	 ,('HD20241222001','MA002',3,85000)
	 ,('HD20241222001','MA011',3,60000)
	 ,('HD20241222001','MA015',3,150000)

	 ,('HD20241222002','MA008',4,110000)
	 ,('HD20241222002','MA012',4,70000)
	 ,('HD20241222002','MA022',4,320000)
	 ,('HD20241222002','MA032',4,120000)

	 ,('HD20241222003','MA003',3,60000)
	 ,('HD20241222003','MA007',3,80000)
	 ,('HD20241222003','MA009',3,160000)
	 ,('HD20241222003','MA059',3,70000)
	 ,('HD20241222003','MA029',3,300000)

	 ,('HD20241222004','MA028',1,350000)
	 ,('HD20241222004','MA036',3,50000)

	 ,('HD20241222005','MA044',5,100000)
	 ,('HD20241222005','MA030',1,180000)
	 ,('HD20241222005','MA031',5,150000)
	 ,('HD20241222005','MA025',3,250000)
	 ,('HD20241222005','MA020',2,300000)
	 ,('HD20241222005','MA001',2,90000)
	 ,('HD20241222005','MA004',5,75000)
	 ,('HD20241222005','MA008',5,110000)

	 ,('HD20241222006','MA044',5,100000)
	 ,('HD20241222006','MA030',1,180000)
	 ,('HD20241222006','MA031',5,150000)
	 ,('HD20241222006','MA025',3,250000)
	 ,('HD20241222006','MA020',2,300000)
	 ,('HD20241222006','MA001',2,90000)
	 ,('HD20241222006','MA004',5,75000)
	 ,('HD20241222006','MA008',5,110000)

	 ,('HD20241222007','MA002',2,85000)
	 ,('HD20241222007','MA005',2,150000)
	 ,('HD20241222007','MA018',2,200000)
	 ,('HD20241222007','MA014',2,130000)

	 ,('HD20241222008','MA002',2,85000)
	 ,('HD20241222008','MA005',2,150000)
	 ,('HD20241222008','MA018',2,200000)
	 ,('HD20241222008','MA014',2,130000)

	 ,('HD20241222009','MA043',1,120000)
	 ,('HD20241222009','MA022',1,320000)

	 ,('HD20241222010','MA005',6,150000)
	 ,('HD20241222010','MA009',3,160000)
	 ,('HD20241222010','MA022',3,320000)
	 ,('HD20241222010','MA023',6,280000)
	 ,('HD20241222010','MA033',2,100000)
	 ,('HD20241222010','MA034',2,130000)
	 ,('HD20241222010','MA035',2,110000)
	 ,('HD20241222010','MA037',2,90000)
	 ,('HD20241222010','MA050',6,100000)
	 -----2024/12/23
	 ,('HD20241223001','MA059',2,50000)
	 ,('HD20241223001','MA030',1,180000)
	 ,('HD20241223001','MA022',1,320000)

	 ,('HD20241223002','MA008',3,110000)
	 ,('HD20241223002','MA012',3,70000)
	 ,('HD20241223002','MA022',3,320000)
	 ,('HD20241223002','MA032',3,120000)

	 ,('HD20241223003','MA003',4,60000)
	 ,('HD20241223003','MA007',4,80000)
	 ,('HD20241223003','MA009',4,160000)
	 ,('HD20241223003','MA059',4,70000)


	 ,('HD20241223004','MA006',3,90000)
	 ,('HD20241223004','MA008',3,110000)
	 ,('HD20241223004','MA018',3,200000)
	 ,('HD20241223004','MA058',3,140000)

	 ,('HD20241223005','MA044',5,100000)
	 ,('HD20241223005','MA030',1,180000)
	 ,('HD20241223005','MA031',5,150000)
	 ,('HD20241223005','MA025',3,250000)
	 ,('HD20241223005','MA020',2,300000)
	 ,('HD20241223005','MA001',2,90000)
	 ,('HD20241223005','MA004',5,75000)
	 ,('HD20241223005','MA008',5,110000)

	 ,('HD20241223006','MA016',2,450000)
	 ,('HD20241223006','MA040',2,130000)
	 ,('HD20241223006','MA047',2,60000)


	 ,('HD20241223007','MA002',6,85000)
	 ,('HD20241223007','MA005',6,150000)
	 ,('HD20241223007','MA018',6,200000)
	 ,('HD20241223007','MA019',2,220000)
	 ,('HD20241223007','MA059',3,70000)
	 ,('HD20241223007','MA053',3,120000)
	 ,('HD20241223007','MA042',3,80000)
	 ,('HD20241223007','MA043',3,120000)

	 ,('HD20241223008','MA002',2,85000)
	 ,('HD20241223008','MA005',2,150000)
	 ,('HD20241223008','MA018',2,200000)
	 ,('HD20241223008','MA014',2,130000)

	 ,('HD20241223009','MA043',1,120000)
	 ,('HD20241223009','MA022',1,320000)

	 ,('HD20241223010','MA005',4,150000)
	 ,('HD20241223010','MA009',2,160000)
	 ,('HD20241223010','MA022',2,320000)
	 ,('HD20241223010','MA023',2,280000)
	 ,('HD20241223010','MA033',4,100000)

