using Prism.Commands;
using Prism.Mvvm;
using ProxyView.DataModel;
using ProxyView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyView.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProxyView.ViewMode
{ 
    
    class MainWindowViewModel : BindableBase
    {
        public DelegateCommand CloseCommand { get; set; }
        public DelegateCommand MiniCommand { get; set; }
        public DelegateCommand MaxCommand { get; set; }
        public DelegateCommand<object>UpdateUrl { get; set; }
        public DelegateCommand UpdateProxyCommand { get; set; }
        public DelegateCommand<object> SecondChanged { get; set; }
        public DelegateCommand<object> MinuteChanged { get; set; }
        public DelegateCommand<object> HourChanged { get; set; }
        //public DelegateCommand<object> SelectItemChangedCommand { get; set; }
        public DelegateCommand<object> SelectItemUrlChangedCommand { get; set; }
        public MainWindowViewModel()
        {
            mainData = new UserData();
            users = mainData.GetUsers();
            urlList = new ObservableCollection<UserData.Url>();
           
            logList = new ObservableCollection<UserData.Log>();
            datetimeList = new ObservableCollection<UserData.DateTimes>();
            //时间初始化部分，需要同页面前设置一致
            int Hour_ = 6;
            int Minute_ = 0;
            int Second_ = 0;
            //初始化UrlList
            UserData.User user_ = users[0];
            for (int i = 0; i < user_.urls.Count(); i++)
            {
                urlList.Add(user_.urls[i]);
                datetimeList.Add(user_.datetimes[i]);
            }
            nowdatetimeList = new ObservableCollection<UserData.NowDateTime>();
            CloseCommand = new DelegateCommand(() =>
            {
                
                Application.Current.Shutdown();
            });
            MiniCommand = new DelegateCommand(() =>
            {
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
            });
            MaxCommand = new DelegateCommand(() =>
            {
                if(this.MaxFlag == false)
                {
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
                    this.MaxFlag = true;
                }
                else
                {
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                    this.MaxFlag = false;
                }
            });
            //更新代理部分
            UpdateProxyCommand = new DelegateCommand(() =>
            {
                ProxyUpdate();
            });
            
            //数据更新函数
            void ProxyUpdate()
            {
                ProxyUpdate proxyUpdateCommand = new ProxyUpdate();
                proxyUpdateCommand.ProcessRequest();
                //数据部分的更新内容
                mainData = new UserData();
                users = mainData.GetUsers();
                urlList.Clear();
                logList.Clear();
                for (int i = 0; i < users[0].urls.Count(); i++)
                {
                    urlList.Add(users[0].urls[i]);
                    datetimeList.Add(users[0].datetimes[i]);
                }
            }
            //倒计时部分
            SecondChanged = new DelegateCommand<object>((p) =>
            {
                TextBox second_textbox = p as TextBox;

                string second_text = second_textbox.Text;
                Second_ = Convert.ToInt32(second_text);
                int Second_sum = Second_ + Minute_ * 60 + Hour_ * 3600;
                if(Second_sum == 0)
                {
                    ProxyUpdate();
                }
            });
            MinuteChanged = new DelegateCommand<object>((p) =>
            {
                TextBox Minute_TextBox = p as TextBox;
                string minute_text = Minute_TextBox.Text;
                Minute_ = Convert.ToInt32(minute_text);
            });
            HourChanged = new DelegateCommand<object>((p) =>
            {
                TextBox hour_textbox = p as TextBox;
                string hour_text = hour_textbox.Text;
                Hour_ = Convert.ToInt32(hour_text);
            });
            /*
            SelectItemChangedCommand = new DelegateCommand<object>((p) =>
            {
                ListView lv = p as ListView;
                UserData.User user_ = lv.SelectedItem as UserData.User;

                if (logList.Count != 0)
                {
                    nowdatetimeList.Clear();
                    logList.Clear();
                    change = false;
                }

                //change = true;
                urlList.Clear();

                for(int i=0; i< user_.urls.Count();i++)
                {
                    urlList.Add(user_.urls[i]);
                    datetimeList.Add(user_.datetimes[i]);
                }
            });
            */
            UpdateUrl = new DelegateCommand<object>((p) =>
            {
                int index_user = Convert.ToInt32(p);
                UserData.User new_user = users[index_user];
                string userName = new_user.UserName;
                WebProxy.Proxy proxy = new WebProxy.Proxy();
                proxy.userProxy(userName, 1);

            });

            SelectItemUrlChangedCommand = new DelegateCommand<object>((p) =>
            {
                if(change == false)
                {
                    change = true;
                    return;
                }
                ListView lv = p as ListView;
                UserData.DateTimes datetimes_ = lv.SelectedItem as UserData.DateTimes;
                UserData.Url url_ = lv.SelectedItem as UserData.Url;
                
                logList.Clear();
                nowdatetimeList.Clear();
                if (url_ == null)
                {
                    logList.Clear();
                    return;
                }
                for (int i = 0; i < url_.Logs.Count(); i++)
                {
                    logList.Add(url_.Logs[i]);
                    nowdatetimeList.Add(url_.NowDateTimes[i]);
                }
            });

        }


        private UserData mainData;
        private ObservableCollection<UserData.User> users;
        private ObservableCollection<UserData.Url> urlList;
        private ObservableCollection<UserData.Log> logList;
        private ObservableCollection<UserData.DateTimes> datetimeList;
        private ObservableCollection<UserData.NowDateTime> nowdatetimeList;
        private bool MaxFlag = false;
        private bool change = true;
        public ObservableCollection<UserData.User> Users
        {
            get { return users; }
            set { SetProperty(ref users, value); }
            
        }
        public ObservableCollection<UserData.Url> Urls
        {
            get { return urlList; }
            set { SetProperty(ref urlList, value); }
        }
        public ObservableCollection<UserData.Log> Logs
        {
            get { return logList; }
            set { SetProperty(ref logList, value); }
        }
        public ObservableCollection<UserData.DateTimes> Datetimes
        {
            get { return datetimeList; }
            set { SetProperty(ref datetimeList, value); }
        }
        public ObservableCollection<UserData.NowDateTime> nowDateTimes
        {
            get { return nowdatetimeList; }
            set { SetProperty(ref nowdatetimeList, value); }
        }
    }
}
