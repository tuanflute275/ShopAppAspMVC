namespace ShopApp.Enums
{
    public class OrderStatus
    {
        public const int CHOXACNHAN = 1;
        public const int XACNHANDONHANG = 2;
        public const int CHUANBIDONHANG = 3;
        public const int DANGGIAOHANG = 4;
        public const int DAGIAOHANG = 5;
        public const int HUYDONHANG = 6;

        public const string CHOXACNHANTXT = "Chờ xác nhận";
        public const string XACNHANDONHANGTXT = "Xác nhận đơn hàng";
        public const string CHUANBIDONHANGTXT = "Chuẩn bị đơn hàng";
        public const string DANGGIAOHANGTXT = "Đang giao hàng";
        public const string DAGIAOHANGTXT = "Đã giao hàng";
        public const string HUYDONHANGTXT = "Hủy đơn hàng";
    }
}
