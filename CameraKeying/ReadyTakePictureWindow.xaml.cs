using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
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
        public ReadyTakePictureWindow(string background)
        {
            _background = background;
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            try
            {
                bframe = GDITools.LoadBitmap(Directory.GetCurrentDirectory() + "\\拍照框.png");
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
                    var temp = GDITools.Synthesis(frame.Bitmap, bframe, 42 + adsiez.xy.X, 58 + adsiez.xy.Y, adsiez.size.fitw,adsiez.size.fith,false);
    
                    image1.Source = temp;
                }));
        }


        private void but_Photo_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            video.QueryFrame().Bitmap.Save(@"D:\a.jpg");
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
