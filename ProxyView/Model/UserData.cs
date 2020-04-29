using Prism.Commands;
using Prism.Mvvm;
using ProxyView.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProxyView.DataModel
{
    class UserData
    {
        public class User
        {
            public string UserName { get; set; }
            public BitmapImage Head { get; set; }
            public ObservableCollection<Url> urls {get; set;}
            public ObservableCollection<DateTimes> datetimes { get; set; }
        }
        public class Url
        {
            public string urlName {get; set;}
            public ObservableCollection<Log> Logs { get; set; }
            public ObservableCollection<NowDateTime> NowDateTimes { get; set; }
        }
        public class DateTimes
        {
            public string urlName { get; set; }
            public ObservableCollection<NowDateTime> dateTimes { get; set; }
        }
        public class Log
        {
            public string LogUrl { get; set; }
        }

        public class NowDateTime
        {
            public string nowDateTime {get; set;}
        }
        //初始化数据
        public UserData()
        {
            //用户姓名数据部分的读取
            string dirPathName = "D:\\Projects\\C#\\ProxyView\\TestData\\config\\";
            string dirLogPathName = "D:\\Projects\\C#\\ProxyView\\TestData\\log\\";
            //更改为log部分的读取
            //DirectoryInfo root = new DirectoryInfo(dirPathName);
            DirectoryInfo root = new DirectoryInfo(dirLogPathName);
            FileInfo[] files = root.GetFiles();
            int fileLength = files.Length;
            //用户获取
            users = new ObservableCollection<User>();
            for(int index = 0; index < fileLength; index ++)
            {
                FileInfo filePathName = files[index];
                string filePathString = filePathName.ToString();
                string userName = filePathString.Split('.')[0];
                User user = new User();
                user.UserName = userName;
                user.Head = new BitmapImage(new Uri("pack://application:,,,/icon/我的.png"));
                user.urls = new ObservableCollection<Url>();
                user.datetimes = new ObservableCollection<DateTimes>();
                //读取URL内容
                //string fileName = dirPathName + filePathString;
                string logFileName = dirLogPathName + filePathString;

                //URL XML Document部分
                XmlDocument doc = new XmlDocument();
                doc.Load(logFileName);
                XmlElement info = doc.DocumentElement;
                XmlElement urllist = (XmlElement)info.GetElementsByTagName("serverUrls")[0];
                XmlElement urlElement = (XmlElement)urllist.FirstChild;
                //Log XML Document部分
                XmlDocument logDoc = new XmlDocument();
                logDoc.Load(logFileName);
                XmlElement infoLog = logDoc.DocumentElement;
                XmlElement urlsLogList = (XmlElement)infoLog.GetElementsByTagName("serverUrls")[0];
                int urlLogIndex = 0;
                
                for(;urlElement!= null;urlElement=(XmlElement)urlElement.NextSibling)
                {
                    
                    string url = urlElement.GetAttribute("url");
                    Url temp_url = new Url();
                    DateTimes date_time = new DateTimes();

                    XmlElement urlLogList = (XmlElement)infoLog.GetElementsByTagName("serverUrl")[urlLogIndex];
                    XmlElement urlLogChild = (XmlElement)urlLogList.FirstChild;
                    temp_url.Logs = new ObservableCollection<Log>();
                    temp_url.NowDateTimes = new ObservableCollection<NowDateTime>();
                    temp_url.urlName = url;
                    date_time.dateTimes = new ObservableCollection<NowDateTime>();
                    date_time.urlName = url;
                    for (;urlLogChild!=null;urlLogChild=(XmlElement)urlLogChild.NextSibling)
                    {
                        Log tempLogUrl = new Log();
                        NowDateTime tempDateTime = new NowDateTime();
                        tempLogUrl.LogUrl = urlLogChild.GetAttribute("datetime") + " " + urlLogChild.GetAttribute("url");
                        tempDateTime.nowDateTime = urlLogChild.GetAttribute("datetime");
                        temp_url.Logs.Add(tempLogUrl);
                        temp_url.NowDateTimes.Add(tempDateTime);
                        date_time.dateTimes.Add(tempDateTime);
                    }
                    urlLogIndex++;
                
                    user.urls.Add(temp_url);
                    user.datetimes.Add(date_time);
                }
                users.Add(user);
            }
            //Url部分的读取部分
        }
        public ObservableCollection<User> GetUsers()
        {
            return users;
        }

        public ObservableCollection<Url> GetUrls(string Username)
        {
            for(int i=0; i< users.Count;i++)
            {
                if(users[i].UserName == Username)
                {
                    return users[i].urls;
                }
            }
            return null;
        }
        private ObservableCollection<User> users;
    }
}
