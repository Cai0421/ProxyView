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
using System.Windows.Threading;
using ProxyView.DataModel;
using ProxyView.Model;
using ProxyView;
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
        private DispatcherTimer timer = new DispatcherTimer();
        private ProcessCount processCount;
        //倒计时全局参数
        string Hour_ = "06";
        string Minute_ = "00";
        string Second_ = "00";
        //.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.Button_MouseLeftButtonDown), true);
        
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
             
            icon();
            wsl = WindowState;
            this.Loaded += new RoutedEventHandler(MainWin_Loaded);
            this.Update_Button.AddHandler(System.Windows.Controls.Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.TimeReset_MouseLeftButtonDown), true);
            this.Mini_Button.AddHandler(System.Windows.Controls.Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.Mini_MouseLeftButtonDown), true);
            this.Modify_Button.AddHandler(System.Windows.Controls.Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.TimeModify_MouseLeftButtonDown), true);
        }

        //icon设置
        private void icon()
        {
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.Text = "代理监视器";
            //this.notifyIcon.Icon = new System.Drawing.Icon("C:/Users/ADMIN/Desktop/ProxyView/ProxyView/Icon/title.ico");
            this.notifyIcon.Icon = new System.Drawing.Icon("C:/System/wudaTest/proxyTest/title.ico");
            this.notifyIcon.Visible = true;
            notifyIcon.MouseDoubleClick += onNotifyIconDoubleClick;
            //this.notifyIcon.ShowBalloonTip(1000);
        }
        private void onNotifyIconDoubleClick(object sender, EventArgs e)
        {
            this.Show();
            WindowState = wsl;
        }
        //窗口加载部分
        private void MainWin_Loaded(object sender, RoutedEventArgs e)
        {
            //设置定时器

            
            timer.Interval = new TimeSpan(10000000);   //时间间隔为一秒
            timer.Tick += new EventHandler(timer_Tick);

            //转换成秒数
            Int32 hour = Convert.ToInt32(Hour_TextBox.Text);
            Int32 minute = Convert.ToInt32(Miniute_TextBox.Text);
            Int32 second = Convert.ToInt32(Second_TextBox.Text);

            //处理倒计时的类
            processCount = new ProcessCount(hour * 3600 + minute * 60 + second);
            CountDown += new CountDownHandler(processCount.ProcessCountDown);

            //开启定时器
            timer.Start();
        }
        //定时器重置部分
        private void TimeReset_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.timer_reset();
        }
        private void timer_reset()
        {
            timer.Stop();

            //timer.Interval = new TimeSpan(10000000);   //时间间隔为一秒
            //timer.Tick += new EventHandler(timer_Tick);
            this.Hour_TextBox.Text = Hour_;
            this.Miniute_TextBox.Text = Minute_;
            this.Second_TextBox.Text = Second_;
            //转换成秒数
            Int32 hour = Convert.ToInt32(Hour_TextBox.Text);
            Int32 minute = Convert.ToInt32(Miniute_TextBox.Text);
            Int32 second = Convert.ToInt32(Second_TextBox.Text);

            //处理倒计时的类
            processCount = new ProcessCount(hour * 3600 + minute * 60 + second);
            CountDown += new CountDownHandler(processCount.ProcessCountDown);

            //开启定时器
            timer.Start();
        }

        //定时器修改部分
        private void TimeModify_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
            TimeSetting timeSettingWindow = new TimeSetting();
            timeSettingWindow.TransfEvent += DateTime_TransfEvent;
            timeSettingWindow.Show();

        }
        //窗口时间注册
        private void DateTime_TransfEvent(string dateTimeValue)
        {
            
            string[] dateTimeArray = dateTimeValue.Split(':');
            Hour_ = dateTimeArray[0];

            Minute_ = dateTimeArray[1];

            Second_ = dateTimeArray[2];

            
            timer.Start();
            timer_reset();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (OnCountDown())
            {
                Hour_TextBox.Text = processCount.GetHour();
                Miniute_TextBox.Text = processCount.GetMinute();
                Second_TextBox.Text = processCount.GetSecond();
                if (Hour_TextBox.Text == "00" && Miniute_TextBox.Text == "00" && Second_TextBox.Text == "00")
                {
                    this.timer_reset();
                    //MainWindowViewModel dataModel = new MainWindowViewModel();
                }
            }
            else
                timer.Stop();
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        public event CountDownHandler CountDown;
        public bool OnCountDown()
        {
            if (CountDown != null)
                return CountDown();

            return false;
        }
        public delegate bool CountDownHandler();

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
