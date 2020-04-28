using Prism.Commands;
using Prism.Mvvm;
using ProxyView.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProxyView.ViewMode
{
    
    class MainWindowViewModel : BindableBase
    {
        
        public DelegateCommand CloseCommand { get; set; }
        public DelegateCommand MiniCommand { get; set; }
        public DelegateCommand MaxCommand { get; set; }
        public DelegateCommand<object>UpdateUrl { get; set; }
        public DelegateCommand<object> SelectItemChangedCommand { get; set; }
        public DelegateCommand<object> SelectItemUrlChangedCommand { get; set; }
        public MainWindowViewModel()
        {
            mainData = new UserData();
            users = mainData.GetUsers();
            urlList = new ObservableCollection<UserData.Url>();
            logList = new ObservableCollection<UserData.Log>();
            datetimeList = new ObservableCollection<UserData.DateTimes>();
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
