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

namespace ProxyView
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ProxyView"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ProxyView;assembly=ProxyView"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:TimeSpanPicker/>
    ///
    /// </summary>
    public class TimeSpanPicker : Control
    {
        static TimeSpanPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSpanPicker), new FrameworkPropertyMetadata(typeof(TimeSpanPicker)));
        }


        /// <summary>
        /// 时间控制方法，自动计算时间
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);


            if (e.Property == HourProperty)
            {
                int hour = (int)e.NewValue;
                if (hour == 24)
                {
                    SetValue(HourProperty, 23);
                }
                else if (hour == -1)
                {
                    SetValue(HourProperty, 0);
                }
                //else
                SetValue(TimeSpanProperty, new TimeSpan(Hour, TimeSpan.Minutes, TimeSpan.Seconds));
            }
            else if (e.Property == MinuteProperty)
            {
                int minute = (int)e.NewValue;
                if (minute == -1)
                {
                    if (Hour == 0)
                    {
                        SetValue(MinuteProperty, 0);
                    }
                    else
                    {
                        SetValue(MinuteProperty, 59);
                        SetValue(HourProperty, Hour - 1);
                    }
                }
                else if (minute == 60)
                {
                    if (Hour == 24)
                    {
                        SetValue(MinuteProperty, 59);
                    }
                    else
                    {
                        SetValue(MinuteProperty, 0);
                        SetValue(HourProperty, Hour + 1);
                    }
                }
                //else
                SetValue(TimeSpanProperty, new TimeSpan(TimeSpan.Hours, Minute, TimeSpan.Seconds));
            }
            else if (e.Property == SecondProperty)
            {
                int second = (int)e.NewValue;
                if (second == -1)
                {
                    if (Minute > 0 || Hour > 0)
                    {
                        SetValue(SecondProperty, 59);
                        SetValue(MinuteProperty, Minute - 1);
                    }
                    else
                    {
                        SetValue(SecondProperty, 0);
                    }
                }
                else if (second == 60)
                {

                    SetValue(SecondProperty, 0);
                    SetValue(MinuteProperty, Minute + 1);

                }
                //设置时间
                SetValue(TimeSpanProperty, new TimeSpan(TimeSpan.Hours, TimeSpan.Minutes, Second));
            }
            else if (e.Property == TimeSpanProperty)
            {
                TimeSpan ts = (TimeSpan)e.NewValue;

                SetValue(HourProperty, ts.Hours);
                SetValue(MinuteProperty, ts.Minutes);
                SetValue(SecondProperty, ts.Seconds);
            }
        }



        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TimeSpanPicker), new PropertyMetadata(false));



        public TimeSpan TimeSpan
        {
            get { return (TimeSpan)GetValue(TimeSpanProperty); }
            set { SetValue(TimeSpanProperty, value); }
        }


        public static readonly DependencyProperty TimeSpanProperty =
            DependencyProperty.Register("TimeSpan", typeof(TimeSpan), typeof(TimeSpanPicker), new PropertyMetadata(TimeSpan.Zero));



        public int Hour
        {
            get { return (int)GetValue(HourProperty); }
            set { SetValue(HourProperty, value); }
        }


        public static readonly DependencyProperty HourProperty =
            DependencyProperty.Register("Hour", typeof(int), typeof(TimeSpanPicker), new PropertyMetadata(0));



        public int Minute
        {
            get { return (int)GetValue(MinuteProperty); }
            set { SetValue(MinuteProperty, value); }
        }


        public static readonly DependencyProperty MinuteProperty =
            DependencyProperty.Register("Minute", typeof(int), typeof(TimeSpanPicker), new PropertyMetadata(0));




        public int Second
        {
            get { return (int)GetValue(SecondProperty); }
            set { SetValue(SecondProperty, value); }
        }


        public static readonly DependencyProperty SecondProperty =
            DependencyProperty.Register("Second", typeof(int), typeof(TimeSpanPicker), new PropertyMetadata(0));
    }

}
