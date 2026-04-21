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
        private Bitmap bitmap;
        private Camera camera;
        private Timer refreshTimer;

        public RenderWindow(Camera camera)
        {
            this.camera = camera;
            this.Text = "NackTracing - Live Preview";
            this.ClientSize = new Size(camera.imageWidth, camera.imageHeight);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.Black;

            pictureBox = new PictureBox();
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.Controls.Add(pictureBox);

            bitmap = new Bitmap(camera.imageWidth, camera.imageHeight, PixelFormat.Format24bppRgb);
            pictureBox.Image = bitmap;

            refreshTimer = new Timer();
            refreshTimer.Interval = 100;
            refreshTimer.Tick += OnTimerTick;
            refreshTimer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (camera.IsCompleted)
            {
                UpdateBitmap();
                this.Text = $"NackTracing - Live Preview ({camera.CurrentSample} / {camera.numSamples} Samples)";
                camera.IsCompleted = false;
            }
        }

        private void UpdateBitmap()
        {
            if (camera.PixelBuffer == null) return;

            int width = camera.imageWidth;
            int height = camera.imageHeight;

            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            int bytesPerPixel = 3;
            int totalBytes = bmpData.Stride * height;
            byte[] rgbValues = new byte[totalBytes];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    Color col = camera.PixelBuffer[index];

                    float r = (col.Vector().X() > 0) ? MathF.Sqrt(col.Vector().X()) : 0;
                    float g = (col.Vector().Y() > 0) ? MathF.Sqrt(col.Vector().Y()) : 0;
                    float b = (col.Vector().Z() > 0) ? MathF.Sqrt(col.Vector().Z()) : 0;

                    int ir = (int)(256 * Math.Clamp(r, 0.0f, 0.999f));
                    int ig = (int)(256 * Math.Clamp(g, 0.0f, 0.999f));
                    int ib = (int)(256 * Math.Clamp(b, 0.0f, 0.999f));

                    int byteIndex = y * bmpData.Stride + x * bytesPerPixel;
                    rgbValues[byteIndex] = (byte)ib;
                    rgbValues[byteIndex + 1] = (byte)ig;
                    rgbValues[byteIndex + 2] = (byte)ir;
                }
            }

            Marshal.Copy(rgbValues, 0, bmpData.Scan0, totalBytes);
            bitmap.UnlockBits(bmpData);

            pictureBox.Refresh();
        }
    }
}