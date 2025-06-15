using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgProcess
{
    public class Viewer
    {
        public Viewer() { }



        public void Screen_Resize(System.Windows.Forms.Label label, PictureBox picBox, Panel panel, Form form)
        {
            Center_Img_OnScreen(picBox, panel);

            if (label.Visible)
            {
                Show_Label_Screen(label, form);
            }
        }

        public void Center_Img_OnScreen(PictureBox picBox, Panel panel)
        {
            if (picBox.Image != null)
            {
                int Width = picBox.Image.Width;
                int Height = picBox.Image.Height;

                int x = (panel.ClientSize.Width - Width) / 2;
                int y = (panel.ClientSize.Height - Height) / 2;

                picBox.Location = new Point(x, y);
            }
        }

        public bool Dispose_Img_Screen(Form form, PictureBox picBox, System.Windows.Forms.Label label)
        {
            if (picBox.Image != null)
            {
                picBox.Image.Dispose();
                picBox.Image = null;

                label.Visible = true;
                Show_Label_Screen(label, form);

                return true;
            } else
            {
                return false;
            }
        }

        private void Show_Label_Screen(System.Windows.Forms.Label label, Form form)
        {
            label.Location = new Point((form.ClientSize.Width - label.Width) / 2,
                                        (form.ClientSize.Height - label.Height) / 2);
        }

        public bool IsImageLoaded_Screen(Bitmap img1, Bitmap img2)
        {
            return !(img1 == null && img2 == null || img1 != null && img2 == null);
        }
    }
}
