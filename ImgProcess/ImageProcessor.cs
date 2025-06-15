using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgProcess
{
    public class ImageProcessor
    {
        private ProcessHelper helper;
        private int minValue = 0;   
        private int maxValue = 255;   

        public ImageProcessor() 
        { 
            helper = new ProcessHelper();
        }


        public Bitmap AdjustBrightness(Bitmap origImg, int bright)
        {
            return ProcessImage(origImg, (pixels, index, bytesPixel) =>
            {
                return helper.AdjustPixel(pixels, index, bytesPixel, (c) => helper.LimitBounds(c + bright, minValue, maxValue));
            });
        }

        public Bitmap AdjustContrast(Bitmap origImg, double contrast)
        {
            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;

            return ProcessImage(origImg, (pixels, index, bytesPixel) =>
            {
                return helper.AdjustPixel(pixels, index, bytesPixel, (c) =>
                {
                    double norm = c / 255.0;
                    norm = ((((norm - 0.5) * contrast) + 0.5) * 255.0);

                    return helper.LimitBounds((int)norm, minValue, maxValue);
                });
            });
        }

        public Bitmap AdjustColorRGB(Bitmap origImg, int r, int g, int b)
        {
            return ProcessImage(origImg, (pixels, i, bytesPixel) =>
            {
                byte[] result = new byte[bytesPixel];

                result[0] = (byte)helper.LimitBounds(pixels[i + 0] + b, minValue, maxValue);
                result[1] = (byte)helper.LimitBounds(pixels[i + 1] + g, minValue, maxValue);
                result[2] = (byte)helper.LimitBounds(pixels[i + 2] + r, minValue, maxValue);

                if (bytesPixel == 4)
                    result[3] = pixels[i + 3];

                return result;
            });
        }

        public Bitmap ProcessImage(Bitmap origImg, Func<byte[], int, int, byte[]> pixels)
        {
            Bitmap newImg = new Bitmap(origImg.Width, origImg.Height);

            Rectangle rect = new Rectangle(0, 0, origImg.Width, origImg.Height);

            var origData = origImg.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, origImg.PixelFormat);
            var newData = newImg.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, origImg.PixelFormat);

            int bytesPixel = Image.GetPixelFormatSize(origImg.PixelFormat) / 8;
            int bytes = origData.Stride * origImg.Height;

            byte[] origPixels = new byte[bytes];
            byte[] newPixels = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(origData.Scan0, origPixels, 0, bytes);
            origImg.UnlockBits(origData);

            for (int i = 0; i < origPixels.Length; i += bytesPixel)
            {
                byte[] procPixel = pixels(origPixels, i, bytesPixel);
                Array.Copy(procPixel, 0, newPixels, i, bytesPixel);
            }

            System.Runtime.InteropServices.Marshal.Copy(newPixels, 0, newData.Scan0, bytes);
            newImg.UnlockBits(newData);

            return newImg;
        }
    }
}












/*public Bitmap AdjustBrightness(Bitmap origImg, int brightness)
        {
            Bitmap adjustedImage = new Bitmap(origImg.Width, origImg.Height);

            for (int x = 0; x < origImg.Width; x++)
            {
                for (int y = 0; y < origImg.Height; y++)
                {
                    Color pixelColor = origImg.GetPixel(x, y);

                    int r = Clamp(pixelColor.R + brightness, 0, 255);
                    int g = Clamp(pixelColor.G + brightness, 0, 255);
                    int b = Clamp(pixelColor.B + brightness, 0, 255);

                    adjustedImage.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return adjustedImage;
        }*/






/*public Bitmap AdjustContrast(Bitmap origImg, double contrast)
{
    Bitmap adjustedImage = new Bitmap(origImg.Width, origImg.Height);
    contrast = (100.0 + contrast) / 100.0;
    contrast *= contrast;

    for (int x = 0; x < origImg.Width; x++)
    {
        for (int y = 0; y < origImg.Height; y++)
        {
            Color pixelColor = origImg.GetPixel(x, y);

            int r = Clamp((int)((((pixelColor.R / 255.0 - 0.5) * contrast) + 0.5) * 255.0), 0, 255);
            int g = Clamp((int)((((pixelColor.G / 255.0 - 0.5) * contrast) + 0.5) * 255.0), 0, 255);
            int b = Clamp((int)((((pixelColor.B / 255.0 - 0.5) * contrast) + 0.5) * 255.0), 0, 255);

            adjustedImage.SetPixel(x, y, Color.FromArgb(r, g, b));
        }
    }

    return adjustedImage;
}

public Bitmap AdjustColorRGB(Bitmap Img, int rAdjust, int gAdjust, int bAdjust)
{
    Bitmap adjustedImage = new Bitmap(Img.Width, Img.Height);


    for (int x = 0; x < Img.Width; x++)
    {
        for (int y = 0; y < Img.Height; y++)
        {

            Color pixelColor = Img.GetPixel(x, y);

            int r = Clamp(pixelColor.R + rAdjust, 0, 255);
            int g = Clamp(pixelColor.G + gAdjust, 0, 255);
            int b = Clamp(pixelColor.B + bAdjust, 0, 255);

            adjustedImage.SetPixel(x, y, Color.FromArgb(r, g, b));
        }
    }
    return adjustedImage;
}*/
