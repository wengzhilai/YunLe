using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace ProServer
{
    public class PicFun
    {
        public enum FillMode
        {
            /// <summary>
            /// 平铺
            /// </summary>
            /// <remarks></remarks>
            Title = 0,
            /// <summary>
            /// 居中
            /// </summary>
            /// <remarks></remarks>
            Center = 1,
            /// <summary>
            /// 拉伸
            /// </summary>
            /// <remarks></remarks>
            Struk = 2,
            /// <summary>
            /// 缩放
            /// </summary>
            /// <remarks></remarks>
            Zoom = 3
        }

        /// <summary>
        /// Bitmap转byte[]
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static byte[] BitmapToByte(Bitmap bmp)
        {
            if (bmp == null) return null;
            MemoryStream ms = null;
            ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] byteImage = new Byte[ms.Length];
            byteImage = ms.ToArray();
            ms.Close();
            return byteImage;
        }

        /// <summary>
        /// byte[] 转换 Bitmap
        /// </summary>
        /// <param name="byteArr"></param>
        /// <returns></returns>
        public static Bitmap ByteToBitmap(byte[] byteArr)
        {
            if (byteArr == null)
            {
                return default(Bitmap);
            }
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(byteArr);
                return new Bitmap((Image)new Bitmap(stream));
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            finally
            {
                stream.Close();
            }
        }

        /// <summary>
        /// 任意角度旋转
        /// </summary>
        /// <param name="bmp">原始图Bitmap</param>
        /// <param name="angle">旋转角度</param>
        /// <returns>输出Bitmap</returns>
        public static Bitmap KiRotate(Bitmap bmp, float angle)
        {
            Color bkColor = Color.White;
            int w = bmp.Width + 2;
            int h = bmp.Height + 2;
            PixelFormat pf = bmp.PixelFormat;
            Bitmap tmp = new Bitmap(w, h, pf);
            Graphics g = Graphics.FromImage(tmp);
            g.Clear(bkColor);
            g.DrawImageUnscaled(bmp, 1, 1);
            g.Dispose();

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, w, h));
            Matrix mtrx = new Matrix();
            mtrx.Rotate(angle);
            RectangleF rct = path.GetBounds(mtrx);

            Bitmap dst = new Bitmap((int)rct.Width, (int)rct.Height, pf);
            g = Graphics.FromImage(dst);
            g.Clear(bkColor);
            g.TranslateTransform(-rct.X, -rct.Y);
            g.RotateTransform(angle);
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(tmp, 0, 0);
            g.Dispose();
            tmp.Dispose();
            bmp.Dispose();

            return dst;
        }
        

        /// <summary>
        /// 设置灰度
        /// </summary>
        public static Bitmap SetClearGamma(Bitmap _currentBitmap,int v)
        {
            Bitmap bmap = (Bitmap)_currentBitmap.Clone();

            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    Color c = bmap.GetPixel(i, j);
                    if (c.R >= v) bmap.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                    else bmap.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                }
            }
            return (Bitmap)bmap.Clone();
        }


        /// <summary>
        /// 合并图片
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Bitmap MergerImg(Bitmap a,Bitmap b)
        {
            int maxW = a.Width;
            if (b.Width > a.Width) maxW = b.Width;
            Bitmap bit = new Bitmap(maxW, a.Height + b.Height);
            Graphics graph = Graphics.FromImage(bit);
            graph.DrawImage(bit, a.Width, a.Height + b.Height);
            graph.DrawImage(a, 0, 0);
            graph.DrawImage(b, 0, a.Height);
            return bit;
        }


        /// <summary>
        /// 在图像上插入图像
        /// </summary>
        /// <param name="back">背景</param>
        /// <param name="front">前面</param>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns></returns>
        public static Bitmap InsertImage(Bitmap back, Bitmap front, int x, int y)
        {
            Bitmap reBmp = (Bitmap)back.Clone();
            Graphics gr = Graphics.FromImage(reBmp);
            Rectangle rect = new Rectangle(x, y, front.Width, front.Height);
            gr.DrawImage(front, rect);
            return reBmp;
        }

        /// <summary>
        /// 求三点角度
        /// </summary>
        /// <param name="cen"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static float Angle(Point cen, Point first, Point second)
        {
            const double M_PI = 3.1415926535897;

            double ma_x = first.X - cen.X;
            double ma_y = first.Y - cen.Y;
            double mb_x = second.X - cen.X;
            double mb_y = second.Y - cen.Y;
            double v1 = (ma_x * mb_x) + (ma_y * mb_y);
            double ma_val = Math.Sqrt(ma_x * ma_x + ma_y * ma_y);
            double mb_val = Math.Sqrt(mb_x * mb_x + mb_y * mb_y);
            double cosM = v1 / (ma_val * mb_val);
            double angleAMB = Math.Acos(cosM) * 180 / M_PI;

            return float.Parse(angleAMB.ToString());
        }
        /// <summary>
        /// 两点之间的线相对于Y轴角度
        /// </summary>
        /// <param name="s">开始</param>
        /// <param name="e">结束</param>
        /// <returns></returns>
        public static float TwoPointToAngleY(Point s, Point e)
        {
            const float M_PI = 3.1415926535897f;
            float a = Math.Abs(s.X - e.X);
            float b = Math.Abs(s.Y - e.Y);
            if (b == 0 && a == 0)
            {
                return 0;
            }
            //double c = Math.Sqrt(a * a + b * b);
            float tmp = (float)Math.Atan(a / b) * 180 / M_PI;
            if (s.Y > e.Y && s.X > e.X)
            {
                return (90-tmp) + 90;
            }
            else if (s.Y > e.Y && s.X < e.X)
            {
                return tmp + 180;
            }
            else if (s.Y < e.Y && s.X < e.X)
            {
                return (90-tmp) + 270;
            }
            return tmp;
        }


        /// <summary>
        /// 两点距离
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double GetDistance(Point a, Point b)
        {
            int x = System.Math.Abs(b.X - a.X);
            int y = System.Math.Abs(b.Y - a.Y);
            return Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// 将tif文件转换成jpg
        /// </summary>
        /// <param name="inPath"></param>
        /// <returns></returns>
        public static string TifToJpg(string inPath)
        {
            if (inPath.Substring(inPath.LastIndexOf('.') + 1).ToLower().IndexOf("jpg")>-1)
            {
                return inPath;
            }
            if (File.Exists(inPath))
            {
                string fileName1 = inPath;
                string fileName2 = inPath.Substring(0, inPath.LastIndexOf('.')) + ".jpg";
                System.IO.FileStream stream = System.IO.File.OpenRead(fileName1);
                Bitmap bmp = new Bitmap(stream);
                System.Drawing.Image image = bmp;//得到原图
                //创建指定大小的图
                System.Drawing.Image newImage = image.GetThumbnailImage(bmp.Width, bmp.Height, null, new IntPtr());
                Graphics g = Graphics.FromImage(newImage);
                g.DrawImage(newImage, 0, 0, newImage.Width, newImage.Height); //将原图画到指定的图上
                g.Dispose();
                stream.Close();
                newImage.Save(fileName2, System.Drawing.Imaging.ImageFormat.Jpeg);
                g.Dispose();
                newImage.Dispose();
                return fileName2;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 缩放比例
        /// </summary>
        /// <param name="inBmp"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Bitmap ZoomBmp(Bitmap inBmp, double scale, int intWidth = 0, int intHeight = 0)
        {
            if (intHeight == 0) intHeight = Convert.ToInt32(scale * inBmp.Height);
            if (intWidth == 0) intWidth = Convert.ToInt32(scale * inBmp.Width);
            Bitmap outBmp = new Bitmap(inBmp, intWidth, intHeight);
            return outBmp;
        }



        public static Bitmap DrawLines(Bitmap inBmp, Point[] pointArr,string mess=null)
        {
            if (inBmp == null) return null;
            if (pointArr.Length < 2) return inBmp;
            Bitmap reBmp = (Bitmap)inBmp.Clone();
            Graphics g = Graphics.FromImage(reBmp);
            g.DrawLines(new Pen(Color.Green), pointArr);
            if (mess != null)
            {
                Font f = new Font("Arial", 12, FontStyle.Bold | FontStyle.Italic);
                g.DrawString(mess, f, new SolidBrush(Color.Red), pointArr[0].X, pointArr[0].Y);
            }
            return reBmp;
        }

        public static Bitmap DrawLines(Bitmap inBmp, Point s,Point e)
        {
            Point[] t=new Point[5];
            t[0]=s;
            t[1]=new Point(s.X,e.Y);
            t[2]=e;
            t[3]=new Point(e.X,s.Y);
            t[4]=s;

            return DrawLines(inBmp, t);
        }


        /// <summary>
        /// 将指向图像按指定的填充模式绘制到目标图像上
        /// </summary>
        /// <param name="SourceBmp">要控制填充模式的源图</param>
        /// <param name="TargetBmp">要绘制到的目标图</param>
        /// <param name="_FillMode">填充模式</param>
        /// <remarks></remarks>
        public static Bitmap ImageFillRect(Bitmap SourceBmp, int w, int h, FillMode _FillMode)
        {
            Bitmap TargetBmp = new Bitmap(w, h);
            switch (_FillMode)
            {
                case FillMode.Title:
                    using (TextureBrush Txbrus = new TextureBrush(SourceBmp))
                    {
                        Txbrus.WrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
                        using (Graphics G = Graphics.FromImage(TargetBmp))
                        {
                            G.FillRectangle(Txbrus, new Rectangle(0, 0, TargetBmp.Width - 1, TargetBmp.Height - 1));
                        }
                    }

                    break;
                case FillMode.Center:
                    using (Graphics G = Graphics.FromImage(TargetBmp))
                    {
                        int xx = (TargetBmp.Width - SourceBmp.Width) / 2;
                        int yy = (TargetBmp.Height - SourceBmp.Height) / 2;
                        G.DrawImage(SourceBmp, new Rectangle(xx, yy, SourceBmp.Width, SourceBmp.Height), new Rectangle(0, 0, SourceBmp.Width, SourceBmp.Height), GraphicsUnit.Pixel);
                    }

                    break;
                case FillMode.Struk:
                    using (Graphics G = Graphics.FromImage(TargetBmp))
                    {
                        G.DrawImage(SourceBmp, new Rectangle(0, 0, TargetBmp.Width, TargetBmp.Height), new Rectangle(0, 0, SourceBmp.Width, SourceBmp.Height), GraphicsUnit.Pixel);
                    }

                    break;
                case FillMode.Zoom:
                    double tm = 0.0;
                    int W = SourceBmp.Width;
                    int H = SourceBmp.Height;
                    if (W > TargetBmp.Width)
                    {
                        tm = TargetBmp.Width / SourceBmp.Width;
                        W = Convert.ToInt32(W * tm);
                        H = Convert.ToInt32(H * tm);
                    }
                    if (H > TargetBmp.Height)
                    {
                        tm = TargetBmp.Height / H;
                        W = Convert.ToInt32(W * tm);
                        H = Convert.ToInt32(H * tm);
                    }
                    using (Bitmap tmpBP = new Bitmap(W, H))
                    {
                        using (Graphics G2 = Graphics.FromImage(tmpBP))
                        {
                            G2.DrawImage(SourceBmp, new Rectangle(0, 0, W, H), new Rectangle(0, 0, SourceBmp.Width, SourceBmp.Height), GraphicsUnit.Pixel);
                            using (Graphics G = Graphics.FromImage(TargetBmp))
                            {
                                int xx = (TargetBmp.Width - W) / 2;
                                int yy = (TargetBmp.Height - H) / 2;
                                G.DrawImage(tmpBP, new Rectangle(xx, yy, W, H), new Rectangle(0, 0, W, H), GraphicsUnit.Pixel);
                            }
                        }
                    }

                    break;
            }
            return TargetBmp;
        }

        public static Bitmap Cut(Bitmap b, int StartX, int StartY, int iWidth, int iHeight)
        {
            if (iWidth < 1 || iHeight < 1)
                return null;
            Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
            Bitmap newB = (Bitmap)b.Clone();
            if (StartX < 0 || StartY < 0)
            {
            
                Point s_p=new Point();
                if (StartX < 0)
                {
                    iWidth = iWidth + Math.Abs(StartX);
                    s_p.X=Math.Abs(StartX);
                    StartX = 0;
                }
                if (StartY < 0)
                {
                    iHeight = iHeight + Math.Abs(StartY);
                    s_p.Y=Math.Abs(StartY);
                    StartY = 0;
                }
                bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
                Graphics _g = Graphics.FromImage(bmpOut);
                _g.DrawImage(b, s_p);
                newB = bmpOut;
            }


            if (b == null)
            {
                return null;
            }

            int w = b.Width;
            int h = b.Height;

            if (StartX >= w || StartY >= h)
            {
                return null;
            }

            if (StartX + iWidth > w)
            {
                iWidth = w - StartX;
            }

            if (StartY + iHeight > h)
            {
                iHeight = h - StartY;
            }

            try
            {
                Graphics g = Graphics.FromImage(bmpOut);
                g.DrawImage(newB, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                g.Dispose();
                return bmpOut;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        public static string ValidateMake(int maxLeng=4)
        {
            string str = "";
            //string[] code = {"0","1","2","3","4","5","6","7","8","9",
            //                "A","B","C","D","E","F","G","H","I","J","K","L","M","N",
            //                "O","P","Q","R","S","T","U","V","W","X","Y","Z","a","b",
            //                "c","d","e","f","g","h","i","j","k","l","m","n","o","p",
            //                "q","r","s","t","u","v","w","x","y","z"};
            string[] code = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            //随机产生字符串
            Random rand = new Random();
            for (int i = 0; i < maxLeng; i++)
            {
                str = str + code[rand.Next(0, code.Length)];
            }
            return str;
        }

        /// <summary>
        /// 创建验证码的图片
        /// </summary>
        /// <param name="validateCode">验证码</param>
        public static byte[] CreateValidateGraphic(string validateCode)
        {
            var mesByte = Encoding.UTF8.GetBytes(validateCode);
            Bitmap image = new Bitmap((int)Math.Ceiling(mesByte.Length * 15.0), 22);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                Font font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                                                                    Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);
                //画图片的前景干扰点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //保存图片数据
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }


        /// <summary>
        /// 生成水印
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static byte[] CreateWatermark(string message, string userName)
        {
            var mesByte = Encoding.UTF8.GetBytes(message);
            Bitmap msgTxt = new Bitmap(240, 60);
            Graphics gTxt = Graphics.FromImage(msgTxt);
            try
            {

                gTxt.Clear(Color.White);
                gTxt.DrawString(message, new Font("黑体", 16), new SolidBrush(Color.FromArgb(219, 219, 234)), 0, 0);
                gTxt.DrawString(userName + "    " + DateTime.Now.ToString("yyyy-MM-dd"), new Font("Arial", 12), new SolidBrush(Color.FromArgb(219, 219, 234)), 0, 30);


                PixelFormat pf = msgTxt.PixelFormat;
                Bitmap tmp = new Bitmap(msgTxt.Width, msgTxt.Height, pf);
                Graphics g = Graphics.FromImage(tmp);
                g.DrawImageUnscaled(msgTxt, 16, 10);
                g.Dispose();

                GraphicsPath path = new GraphicsPath();
                path.AddRectangle(new RectangleF(0f, 0f, msgTxt.Width, msgTxt.Height));
                Matrix mtrx = new Matrix();
                mtrx.Rotate(45);
                RectangleF rct = path.GetBounds(mtrx);

                Bitmap dst = new Bitmap(240, 240, pf);
                g = Graphics.FromImage(dst);
                g.Clear(Color.White);
                g.TranslateTransform(-rct.Y, 135);
                g.RotateTransform(-45);
                g.DrawImageUnscaled(tmp, -20, 50);
                g.Dispose();

                //msgTxt = KiRotate(msgTxt, 30);
                //保存图片数据
                MemoryStream stream = new MemoryStream();
                dst.Save(stream, ImageFormat.Png);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                gTxt.Dispose();
                msgTxt.Dispose();
            }
        }
    }
}
