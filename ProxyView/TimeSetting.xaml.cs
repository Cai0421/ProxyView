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
using System.Windows.Shapes;

namespace ProxyView
{
    /// <summary>
    /// TimeSetting.xaml 的交互逻辑
    /// </summary>
    public partial class TimeSetting : Window
    {
        //声明委托
        public delegate void TransfDelegate(string dateTime);
        public event TransfDelegate TransfEvent;
        
        public TimeSetting()
        {
            InitializeComponent();
        }
        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
           string datetimeValue = timePicker.TimeSpan.ToString();
            TransfEvent(datetimeValue);
            this.Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
