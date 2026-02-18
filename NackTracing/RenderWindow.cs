using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NackEngine.core.render;
using Color = NackEngine.core.render.Color;
using Timer = System.Windows.Forms.Timer;


namespace NackTracing
{
    public class RenderWindow : Form
    {
        private PictureBox pictureBox;
        private Timer timer;
        private Color[] pixelBuffer;

        private Bitmap bitmap;
        private int width;
        private int height;

        public RenderWindow(int width, int height, Color[] buffer, int FPS = 2) {
            this.width = width;
            this.height = height;
            this.pixelBuffer = buffer;

            this.Text = "NackTracing - Rendering Preview";
            this.Size = new Size(width + 16, height + 39);

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            pictureBox = new PictureBox();
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.Controls.Add(pictureBox);

            bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            pictureBox.Image = bitmap;

            timer = new Timer();
            timer.Interval = 1000 / FPS;
            timer.Tick += OnTick;
            timer.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            UpdateBitmap();
            pictureBox.Refresh();
        }

        private void UpdateBitmap() {
            if (pixelBuffer == null) return;

            BitmapData bmpData = 
                    bitmap.LockBits(new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly, 
                    bitmap.PixelFormat);

            int bytesPerPixel = 3;
            int stride = bmpData.Stride;
            IntPtr ptr = bmpData.Scan0;

            int totalBytes = stride * height;
            byte[] rgbValues = new byte[totalBytes];

            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    var col = pixelBuffer[index];

                    double r = Math.Sqrt(col.Vector().X());
                    double g = Math.Sqrt(col.Vector().Y());
                    double b = Math.Sqrt(col.Vector().Z());

                    int ir = (int)(255.99 * Math.Clamp(r, 0, 0.999));
                    int ig = (int)(255.99 * Math.Clamp(g, 0, 0.999));
                    int ib = (int)(255.99 * Math.Clamp(b, 0, 0.999));

                    int byteIndex = y * stride + x * bytesPerPixel;
                    rgbValues[byteIndex] = (byte)ib;
                    rgbValues[byteIndex + 1] = (byte)ig;
                    rgbValues[byteIndex + 2] = (byte)ir;
                }
            });

            Marshal.Copy(rgbValues, 0, ptr, totalBytes);
            bitmap.UnlockBits(bmpData);
        }

    }
}
