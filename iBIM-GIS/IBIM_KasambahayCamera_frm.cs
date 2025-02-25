using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Windows.Forms;

namespace iBIM_GIS
{
    public partial class IBIM_KasambahayCamera_frm : Form
    {
        private FilterInfoCollection CaptureDevices;
        private VideoCaptureDevice videoSource;
        private IBIM_Kasambahay_frm kasambahayinformation;
        public IBIM_KasambahayCamera_frm(IBIM_Kasambahay_frm form)
        {
            kasambahayinformation = form;
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(IBIM_Kasambhay_frm_FormClosing);

        }

        private void IBIM_KasambahayCamera_frm_Load(object sender, EventArgs e)
        {
            CaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CaptureDevices)
            {
                comboBox2.Items.Add(Device.Name);
            }
            comboBox2.SelectedIndex = 0;
            videoSource = new VideoCaptureDevice();
        }
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pbimage.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                videoSource = new VideoCaptureDevice(CaptureDevices[comboBox2.SelectedIndex].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(VideoSource_NewFrame);
                videoSource.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the camera is connected by verifying if the PictureBox contains an image
                if (pbimage.Image == null)
                {
                    MessageBox.Show("Please connect to the camera and capture a photo first.", "Camera Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;  // Exit the method if no image is captured
                }

                // Resize the captured image to match the target size
                Size targetSize = kasambahayinformation.GetKasambahayImageSize();
                Image resizedImage = ResizeImage(pbimage.Image, targetSize);

                // Set the captured image in the official information form
                kasambahayinformation.SetOfficialImage(resizedImage);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private Image ResizeImage(Image image, Size targetSize)
        {
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            float ratioX = (float)targetSize.Width / (float)originalWidth;
            float ratioY = (float)targetSize.Height / (float)originalHeight;
            float ratio = Math.Max(ratioX, ratioY);
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }
        private void IBIM_Kasambhay_frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop and release videoSource when closing the form
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource = null;  // Ensure it's null so it can be reinitialized
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void pbimage_Click(object sender, EventArgs e)
        {

        }
    }
}
