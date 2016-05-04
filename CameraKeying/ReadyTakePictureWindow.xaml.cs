using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DTTOOLS;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace CameraKeying
{
    
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ReadyTakePictureWindow : Window
    {
        private Timer timerframe;
        private bool framestate;
        private Capture video;
        private Bitmap bframe;
        private GDITools.AdaptationSize adsiez;
        private string _background;
        private readonly Grid _gridlight = new Grid { Background = new SolidColorBrush(Colors.White) };
        public ReadyTakePictureWindow(string background )
        {
            _background = background;
 
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            try
            {


                bframe = GDITools.LoadBitmap(Directory.GetCurrentDirectory() + "\\ImageResources\\拍照框.png");
                adsiez = GDITools.GetAdaptationSize(640, 480, 584, 645, true);
                video = new Capture(0);
                video.SetCaptureProperty(CapProp.FrameWidth, 640);
                video.SetCaptureProperty(CapProp.FrameHeight, 480);
                video.ImageGrabbed += video_ImageGrabbed;
                video.Start();
            }
            catch (Exception ex)
            {

            }

        }
        private void video_ImageGrabbed(object sender, EventArgs e)
        {
            Mat frame = new Mat();         

            video.Retrieve(frame, 0);         

            Dispatcher.Invoke(DispatcherPriority.SystemIdle, new Action(delegate()
                {
                    var temp = GDITools.SynthesisBitmapSource(frame.Bitmap, bframe, 42 + adsiez.xy.X, 58 + adsiez.xy.Y, adsiez.size.fitw,adsiez.size.fith,false);
    
                    image1.Source = temp;
                }));
        }



        private void but_back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void but_Photo_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Background = new ImageBrush(new BitmapImage(new Uri("ImageResources/倒计时背景.png", UriKind.Relative)));
            Thread threadCountdown = new Thread(Countdown);
            threadCountdown.IsBackground = true;
            threadCountdown.Start();                         
        }

        private void Countdown()
        {
            Thread.Sleep(1000);

            Dispatcher.Invoke(DispatcherPriority.SystemIdle, new Action(delegate()
            {
                LabCountdown.FontSize = 50;
                LabCountdown.Content ="Ready!";
                
            }));

                Thread.Sleep(1000);
           
            for (int i = 5; i > 0; i--)
            {
                Dispatcher.Invoke(DispatcherPriority.SystemIdle, new Action(delegate()
                {
                    LabCountdown.FontSize = 160;
                    LabCountdown.Content = i.ToString();
                }));            
                Thread.Sleep(1000);
            }

            Dispatcher.Invoke(DispatcherPriority.SystemIdle, new Action(delegate()
            {
                LabCountdown.FontSize = 50;
                LabCountdown.Content = "Go!";
            }));   

            Flashlight();  

        }


           
        private void Flashlight()
        {
           
            MainGrid.Dispatcher.Invoke(DispatcherPriority.SystemIdle, new Action(delegate()
            {
                var da = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = new Duration(TimeSpan.FromSeconds(1))
                };

                Storyboard sb = new Storyboard();
                sb.Completed += sb_Completed;
                Storyboard.SetTarget(da, _gridlight);
                Storyboard.SetTargetProperty(da, new PropertyPath(OpacityProperty));
                sb.Children.Add(da);
                sb.Begin();
                Grid.SetColumnSpan(_gridlight, 4);
                Grid.SetRowSpan(_gridlight, 5);
                MainGrid.Children.Add(_gridlight);
            }));
        }

        private void PrintPhoto()
        {
            var photo = (Bitmap)video.QueryFrame().Bitmap.Clone();

            var pointdata =  DTTOOLS.Tools.Xamlconfigex.XamlRead<BackGroundMode>(
                String.Format("{0}\\backgroundini\\{1}.xml",
                Directory.GetCurrentDirectory(),Path.GetFileNameWithoutExtension(_background)
                ));

            
            //DTTOOLS.GDITools.GreenScreenMatting(85, 155, 25, photo);

            var backgroundBitmap = GDITools.LoadBitmap(_background);


            if (pointdata != null)
            {
                foreach (var t in pointdata.PhotoDates)
                {
                    var dsize = GDITools.GetAdaptationSize((double) photo.Width, (double) photo.Height,
                    t.Width, t.Height, true);

                    photo = GDITools.SynthesisBitmap(photo, backgroundBitmap, 
                    t.X + dsize.xy.X, t.Y + dsize.xy.Y,
                        dsize.size.fitw, dsize.size.fith, true);
                }

                photo = pointdata.Qr.Aggregate(photo, (current, q) => GDITools.SynthesisBitmap(current, backgroundBitmap, q.X, q.Y, q.Width, q.Height, false));

                photo.Save(Directory.GetCurrentDirectory() + "\\temp.jpg");
            }


        }

        private void sb_Completed(object sender, EventArgs e)
        {
            PrintPhoto();
            //MainGrid.Children.Remove(_gridlight);
            //EndWindows endwindows = new EndWindows();
            //endwindows.Show();
            //Close();
        }
    }

    public static class BitmapSourceConvert
    {
        /// <summary>
        /// Delete a GDI object
        /// </summary>
        /// <param name="o">The poniter to the GDI object to be deleted</param>
        /// <returns></returns>
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }
    }
}
