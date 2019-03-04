using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProServer.Helper
{
    /// <summary>
    /// 画操作的助手
    /// </summary>
    public static class PaintHelper
    {
        private static ImageAttributes _ImageAtt;
        private static ImageAttributes ImageAtt
        {
            get
            {
                if (_ImageAtt == null)
                {
                    _ImageAtt = new ImageAttributes();
                    _ImageAtt.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                }
                return _ImageAtt;
            }
        }

        /// <summary>
        /// 获得文本格式
        /// </summary>
        public static StringFormat GetDrawTextStringFormat(StringAlignment horz, StringAlignment vert, bool wrap)
        {
            StringFormat result = new StringFormat
            {
                Alignment = horz,
                LineAlignment = vert
            };

            result.FormatFlags &= StringFormatFlags.LineLimit;
            result.FormatFlags ^= StringFormatFlags.MeasureTrailingSpaces;
            if (!wrap)
                result.FormatFlags = result.FormatFlags | StringFormatFlags.NoWrap;
            else if ((result.FormatFlags | StringFormatFlags.NoWrap) == result.FormatFlags)
                result.FormatFlags = result.FormatFlags ^ StringFormatFlags.NoWrap;

            //result.FormatFlags |= StringFormatFlags.LineLimit;
            return result;
        }

        /// <summary>
        /// 获得文本格式
        /// </summary>
        public static TextFormatFlags GetDrawTextStringFormatFlags(StringAlignment horz, StringAlignment vert, bool wrap)
        {
            TextFormatFlags r = horz == StringAlignment.Near ? TextFormatFlags.Left : (horz == StringAlignment.Far ? TextFormatFlags.Right : TextFormatFlags.HorizontalCenter);
            r |= TextFormatFlags.EndEllipsis;//不能有TextFormatFlags.WordBreak
            switch (vert)
            {
                case StringAlignment.Near:
                    r |= TextFormatFlags.Top;
                    break;
                case StringAlignment.Center:
                    r |= TextFormatFlags.VerticalCenter;
                    break;
                case StringAlignment.Far:
                    r |= TextFormatFlags.Bottom;
                    break;
            }

            if (wrap)
                r |= TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;

            return r;
        }

        /// <summary>
        /// 取颜色的RGB(int)值
        /// </summary>
        /// <param name="c">颜色</param>
        /// <returns>RGB(int)值</returns>
        public static int RGB(Color c)
        {
            return RGB(c.R, c.G, c.B);
        }

        /// <summary>
        /// 取颜色的RGB(int)值
        /// </summary>
        /// <param name="r">Color.R</param>
        /// <param name="g">Color.G</param>
        /// <param name="b">Color.B</param>
        /// <returns>RGB(int)值</returns>
        public static int RGB(byte r, byte g, byte b)
        {
            return (r | (g << 8) | (b << 16));
        }

        /// <summary>
        /// 画image到g的指定区域(不使用平滑的画)
        /// </summary>
        public static void DrawImage(Graphics g, Image image, Rectangle rect)
        {
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.DrawImage(image, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, ImageAtt);
        }

        /// <summary>
        /// 画一个文本
        /// </summary>
        public static void DrawText(Graphics g, Rectangle rect, string text, Font font, Color color, StringAlignment horz, StringAlignment vert, bool wrap)
        {
            using (SolidBrush brush = new SolidBrush(color))
            {
                //g.DrawString(text, font, brush, rect, GetDrawTextStringFormat(horz, vert, wrap));
                TextRenderer.DrawText(g, text, font, rect, brush.Color, GetDrawTextStringFormatFlags(horz, vert, wrap));
            }
        }

        /// <summary>
        /// 画一个按钮
        /// 默认为文本为水平垂直居中,如果设置了左偏移，那么将按左偏移垂直居中
        /// </summary>
        /// <param name="g">操作的画面</param>
        /// <param name="image">背景图片</param>
        /// <param name="rect">画的范围</param>
        /// <param name="text">显示文本</param>
        /// <param name="font">文本字体</param>
        /// <param name="color">文本颜色</param>
        /// <param name="leftOffset">左移动量(为-1时，将自动水平居中)</param>
        public static void DrawButton(Graphics g, Image image, Rectangle rect, string text, Font font, Color color, int leftOffset)
        {
            if (image != null)
                DrawImage(g, image, rect);
            if (!string.IsNullOrEmpty(text))
            {
                if (leftOffset >= 0)
                {
                    rect.X += leftOffset;
                    DrawText(g, rect, text, font, color, StringAlignment.Near, StringAlignment.Center, false);
                }
                else
                {
                    DrawText(g, rect, text, font, color, StringAlignment.Center, StringAlignment.Center, false);
                }
            }
        }

        /// <summary>
        /// 点是否在区域内
        /// </summary>
        public static bool PtInRect(Point pt, Rectangle rect)
        {
            return PtInRect(pt.X, pt.Y, rect);
        }

        /// <summary>
        /// 点是否在区域内
        /// </summary>
        public static bool PtInRect(int x, int y, Rectangle rect)
        {
            return x >= rect.X && x <= rect.Right && y >= rect.Y && y <= rect.Bottom;
        }
    }
}
