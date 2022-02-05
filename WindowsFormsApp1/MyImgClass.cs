using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace MyImgClassNameSpace
{
    /// <summary>
    /// Используется для легкой и понятной работы с изображением.
    /// <param name="_filename">указывается путь к картинке,</param>
    /// <param name="_width">ширина и </param>
    /// <param name="height">высота</param>
    /// <returns>возвращается значение img </returns>
    /// </summary>
    public class MyImgClass
    {
        private string filename;
        private int width;
        private int height;
        public Image img;

        public MyImgClass(string _filename, int _width, int _height)
        {
            filename = _filename;
            width = _width;
            height = _height;
            img = Image.FromFile(filename);
            img = ScaleImage(_width, _height);   //(img, width, height);

        }

        public static Image SetImgOpacity(Image imgPic, float imgOpac)
        {

            Bitmap bmpPic = new Bitmap(imgPic.Width, imgPic.Height);

            Graphics gfxPic = Graphics.FromImage(bmpPic);

            ColorMatrix cmxPic = new ColorMatrix();

            cmxPic.Matrix33 = imgOpac;

            ImageAttributes iaPic = new ImageAttributes();
            iaPic.SetColorMatrix(cmxPic, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            gfxPic.DrawImage(imgPic, new Rectangle(0, 0, bmpPic.Width, bmpPic.Height), 0, 0, imgPic.Width, imgPic.Height, GraphicsUnit.Pixel, iaPic);
            gfxPic.Dispose();
            return bmpPic;

        }
        /// <summary>
        /// метод масштабирования изображения.
        /// </summary>  
        /// <returns>Image</returns>
        public Image ScaleImage(int width, int height)   //(Image img, int width, int height) //static
        {
            //Image img = this.img;
            Image dest = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(dest))
            {
                gr.FillRectangle(Brushes.White, 0, 0, width, height);  // Очищаем экран
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                float srcwidth = img.Width;
                float srcheight = img.Height;
                float dstwidth = width;
                float dstheight = height;

                if (srcwidth <= dstwidth && srcheight <= dstheight)  // Исходное изображение меньше целевого
                {
                    int left = (width - img.Width) / 2;
                    int top = (height - img.Height) / 2;
                    gr.DrawImage(img, left, top, img.Width, img.Height);
                }
                else if (srcwidth / srcheight > dstwidth / dstheight)  // Пропорции исходного изображения более широкие
                {
                    float cy = srcheight / srcwidth * dstwidth;
                    float top = ((float)dstheight - cy) / 2.0f;
                    if (top < 1.0f) top = 0;
                    gr.DrawImage(img, 0, top, dstwidth, cy);
                }
                else  // Пропорции исходного изображения более узкие
                {
                    float cx = srcwidth / srcheight * dstheight;
                    float left = ((float)dstwidth - cx) / 2.0f;
                    if (left < 1.0f) left = 0;
                    gr.DrawImage(img, left, 0, cx, dstheight);
                }

                return dest;
            }


        }

        public static Image ScaleImageScreen(Image img, int width, int height)   //(Image img, int width, int height) //static
        {
            //Image img = this.img;
            Image dest = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(dest))
            {
                gr.FillRectangle(Brushes.White, 0, 0, width, height);  // Очищаем экран
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                float srcwidth = img.Width;
                float srcheight = img.Height;
                float dstwidth = width;
                float dstheight = height;

                if (srcwidth <= dstwidth && srcheight <= dstheight)  // Исходное изображение меньше целевого
                {
                    int left = (width - img.Width) / 2;
                    int top = (height - img.Height) / 2;
                    gr.DrawImage(img, left, top, img.Width, img.Height);
                }
                else if (srcwidth / srcheight > dstwidth / dstheight)  // Пропорции исходного изображения более широкие
                {
                    float cy = srcheight / srcwidth * dstwidth;
                    float top = ((float)dstheight - cy) / 2.0f;
                    if (top < 1.0f) top = 0;
                    gr.DrawImage(img, 0, top, dstwidth, cy);
                }
                else  // Пропорции исходного изображения более узкие
                {
                    float cx = srcwidth / srcheight * dstheight;
                    float left = ((float)dstwidth - cx) / 2.0f;
                    if (left < 1.0f) left = 0;
                    gr.DrawImage(img, left, 0, cx, dstheight);
                }

                return dest;
            }
        }
    }
}
