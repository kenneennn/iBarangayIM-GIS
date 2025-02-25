using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace iBIM_GIS
{
    public partial class ImagePopupForm : Form
    {
        public ImagePopupForm(Image image)
        {
            InitializeComponent();
            // Set the PictureBox to display the image
            pictureBox1.Image = image;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }
    }
}
