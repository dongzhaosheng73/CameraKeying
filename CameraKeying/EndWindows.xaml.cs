using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CameraKeying
{
    /// <summary>
    /// EndWindows.xaml 的交互逻辑
    /// </summary>
    public partial class EndWindows : Window
    {

        public EndWindows()
        {
  
            InitializeComponent();
        }

        private void but_back_Click(object sender, RoutedEventArgs e)
        {
            foreach (var window in Application.Current.Windows.Cast<Window>().Where(window => window.Title == "BackGroudWindows"))
            {
                window.Close();
            }
            this.Close();
        }
    }
}
