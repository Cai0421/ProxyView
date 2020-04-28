using ProxyView.ViewMode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using ProxyView.DataModel;
namespace ProxyView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        WindowState ws;
        WindowState wsl;
        NotifyIcon notifyIcon;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            icon();
            wsl = WindowState;
        }

        //icon设置
        private void icon()
        {
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.Text = "代理监视器";
            this.notifyIcon.Icon = new System.Drawing.Icon("D:/Projects/C#/ProxyView/ProxyView/Icon/github.ico");
            this.notifyIcon.Visible = true;
            notifyIcon.MouseDoubleClick += onNotifyIconDoubleClick;
            //this.notifyIcon.ShowBalloonTip(1000);
        }
        private void onNotifyIconDoubleClick(object sender, EventArgs e)
        {
            this.Show();
            WindowState = wsl;
        }
        
        private void NavBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Mini_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ws = WindowState;
            if(ws == WindowState.Normal)
            {
                this.Hide();
            }
        }
    }

}
