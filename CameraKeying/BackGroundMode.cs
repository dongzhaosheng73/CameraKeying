using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CameraKeying
{
    public class BackGroundMode
    {
        public BackGroundMode(List<PhotoData> photoDates, List<PhotoData> qr)
        {
            PhotoDates = photoDates;
            Qr = qr;
        }

        public BackGroundMode()
        {
            
        }
        public List<PhotoData> PhotoDates { set; get; }
        public List<PhotoData> Qr { set; get; }
    }

    public class PhotoData
    {
        public PhotoData()
        {
            
        }
        public double X { set; get; }
        public double Y { set; get; }
        public double Width { set; get; }
        public double Height { set; get; }
    }
}
