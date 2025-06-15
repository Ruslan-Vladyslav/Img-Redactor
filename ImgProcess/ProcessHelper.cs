using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgProcess
{
    public class ProcessHelper
    {
        public ProcessHelper() { }


        public byte[] AdjustPixel(byte[] pixels, int index, int bytes, Func<int, int> func)
        {
            byte[] adjustedPixel = new byte[bytes];

            for (int i = 0; i < bytes - 1; i++) 
            {
                adjustedPixel[i] = (byte)func(pixels[index + i]);
            }
            if (bytes == 4)
            {
                adjustedPixel[3] = pixels[index + 3];
            }

            return adjustedPixel;
        }


        public int LimitBounds(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }
    }
}
