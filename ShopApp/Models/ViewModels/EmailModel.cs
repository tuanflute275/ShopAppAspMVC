namespace ShopApp.Models.ViewModels
{
    public class EmailModel
    {
        public EmailModel()
        {
            From = "tuanflute275@gmail.com";
            Password = "eyar ysoh lacl lpur";
        }
        public string To { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string Password { get; set; }
    }
}
