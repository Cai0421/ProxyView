using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProxyView
{
    public class NumbiricTextBox : TextBox
    {
        private static Regex regex = new Regex("[0-9]+");
        public NumbiricTextBox()
        {
            SetValue(InputMethod.IsInputMethodEnabledProperty, false);//禁用输入法
            DataObject.AddPastingHandler(this, TextBoxPasting);//粘贴时候判断
            this.MaxLength = 2;//设置长度，避免过多输入
        }

        /// <summary>
        /// 输入判定，只能输入数字 大于0
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            e.Handled = !regex.IsMatch(e.Text);
        }

        /// <summary>
        /// 滚轮改变值大小
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);
            if (!regex.IsMatch(this.Text))
            {
                return;
            }
            e.Handled = !regex.IsMatch(this.Text);
            var x = e.Source;
            if (x != null && x is NumbiricTextBox)
            {
                NumbiricTextBox tbx = x as NumbiricTextBox;
                if (e.Delta > 0)
                {
                    tbx.Text = (int.Parse(tbx.Text) + 1).ToString();
                }
                else
                {
                    tbx.Text = (int.Parse(tbx.Text) - 1).ToString();
                }
            }
        }
        //保证值不为空····························
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (string.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = "0";
            }
        }
        /// <summary>
        /// 粘贴事件检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!regex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

    }
}
