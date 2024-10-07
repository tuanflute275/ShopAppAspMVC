using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToolProduct
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string url = "http://159.89.205.32:5000/Admin";
            webView21.Source = new Uri(url);
        }

        private async void webView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            // login to home admin
            var username = "admin";
            var password = "111111";
            await webView21.ExecuteScriptAsync("javascript:  " +
                                    "var consumerNo= document.getElementById('username'); " +
                                    "consumerNo.value = '" + username + "'; " +
                                    "consumerNo.dispatchEvent(new Event('input')) ");
            await webView21.ExecuteScriptAsync("javascript:  " +
                                    "var consumerNo= document.getElementById('password'); " +
                                    "consumerNo.value = '" + password + "'; " +
                                    "consumerNo.dispatchEvent(new Event('input')) ");
            await Task.Delay(1000);
            await webView21.ExecuteScriptAsync(@"
                const btnSubmitLogin = document.getElementById('btnLogin');
                if (btnSubmitLogin) {
                    btnSubmitLogin.click();
                } else {
                    console.error('Button not found'); 
                }
            ");

            // chuyển hướng đến trang sản phẩm
            string url2 = "http://159.89.205.32:5000/Admin/Product";
            webView21.Source = new Uri(url2);

            await Task.Delay(4000);
        }
       
    }
}
