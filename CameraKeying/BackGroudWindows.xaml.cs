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
using System.IO;

namespace CameraKeying
{
    /// <summary>
    /// BackGroudWindows.xaml 的交互逻辑
    /// </summary>
    public partial class BackGroudWindows : Window
    {
        private readonly string _backgroundpath = Directory.GetCurrentDirectory() + "\\background\\";
        private List<string> _backgroundList = new List<string>();
        private PageMode pagecontent { set; get; }
        public BackGroudWindows()
        {
            InitializeComponent();
        }

        private void but_back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadBackGround()
        {
            if (!Directory.Exists(_backgroundpath))
            {
                Directory.CreateDirectory(_backgroundpath);
                return;
            }

            _backgroundList = Directory.GetFiles(_backgroundpath, "*.*").
                Where(x => x.ToLower().EndsWith(".jpg") || x.ToLower().EndsWith(".jpeg") || x.ToLower().EndsWith(".png")).ToList();

            if (!_backgroundList.Any()) return;

            var n = _backgroundList.Count < 4 ? _backgroundList.Count : 4;

            for (var i = 0; i < n; i++)
            {
                var img = new Image
                {
                    Source = DTTOOLS.GDITools.LoadBitmapImage(_backgroundList[i], 0, 0, false)
                    ,DataContext = _backgroundList[i]
                };

                img.MouseDown += img_MouseDown;

                Grid.SetRow(img,1);
                Grid.SetColumn(img,i);

                backgroundgrid.Children.Add(img);
            }

            pagecontent = new PageMode(0, (int)Math.Ceiling((double)_backgroundList.Count / 4),4,_backgroundList);
            
        }

        private void NextPage()
        {
            var backlist = pagecontent.NextPage();

            if (!backlist.Any())
            {
                MessageBox.Show("已经是最后一页");
                return;
            }

            for (var i = 0; i < backlist.Count; i++)
            {
                var img = new Image
                {
                    Source = DTTOOLS.GDITools.LoadBitmapImage(_backgroundList[i], 0, 0, false)
                    ,
                    DataContext = _backgroundList[i]
                };

                img.MouseDown += img_MouseDown;

                Grid.SetRow(img, 1);
                Grid.SetColumn(img, i);

                backgroundgrid.Children.Add(img);
            }
        }

        private void AgoPage()
        {
            var backlist = pagecontent.AgoPage();

            if (!backlist.Any())
            {
                MessageBox.Show("已经是第一页");
                return;
            }

            for (var i = 0; i < backlist.Count; i++)
            {
                var img = new Image
                {
                    Source = DTTOOLS.GDITools.LoadBitmapImage(_backgroundList[i], 0, 0, false)
                    ,
                    DataContext = _backgroundList[i]
                };

                img.MouseDown += img_MouseDown;

                Grid.SetRow(img, 1);
                Grid.SetColumn(img, i);

                backgroundgrid.Children.Add(img);
            }
        }

        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;

            ReadyTakePictureWindow readwindows = new ReadyTakePictureWindow(img.DataContext.ToString());

            readwindows.Show();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            LoadBackGround();
        }

        private void but_next_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (pagecontent != null)
            {
                NextPage();
            }
            else
            {
                MessageBox.Show("未找到任何边框");
            }
        }

        private void but_ago_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (pagecontent != null)
            {
                AgoPage();
            }
            else
            {
                MessageBox.Show("未找到任何边框");
            }
        }

      
    }

    public class PageMode
    {
        public PageMode(int index, int count,int span,List<string> data)
        {
            Pageindex = index;
            Pagecount = count;
            PageSpan = span;
            PageData = data;
        }
        public int Pageindex { set; get; }

        public int Pagecount { set; get; }

        public int PageSpan { set; get; }
        
        public List<string> PageData { set; get; }

        public List<string> PageGo(int index)
        {
            if (Pagecount <= 0 || Pageindex >= Pagecount || Pageindex < 0) return new List<string>();

            var span = Pageindex * PageSpan - PageData.Count;

            int n;

            if (span < 0 && Math.Abs(span) > PageSpan)
            {
                n = PageSpan;
            }
            else if (span < 0)
            {
                n = Math.Abs(span);
            }
            else
            {
                return new List<string>();
            }

            var rList = new List<string>();

            for (int i = (Pageindex) * PageSpan; i < n; i++)
            {
                rList.Add(PageData[i]);
            }

            Pageindex = index;

            return rList;
        }

        public List<string> NextPage()
        {
            if (Pagecount <= 0 || Pageindex + 1 >= Pagecount) return new List<string>();

            return PageGo(Pageindex + 1);
        }

        public List<string> AgoPage()
        {
            if (Pagecount <= 0 || Pageindex - 1 < 0) return new List<string>();

            return PageGo(Pageindex - 1);
        }
    }
}
