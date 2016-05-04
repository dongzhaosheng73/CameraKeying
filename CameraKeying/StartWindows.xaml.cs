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
    /// StartWindows.xaml 的交互逻辑
    /// </summary>
    public partial class StartWindows : Window
    {
        public StartWindows()
        {
            InitializeComponent();
        }

        private void but_start_Click(object sender, RoutedEventArgs e)
        {
            var background = new BackGroudWindows();
            background.Show();
        }

        private void Border_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
