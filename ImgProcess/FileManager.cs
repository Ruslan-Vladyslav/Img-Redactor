using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgProcess
{
    public class FileManager
    {
        private Form1 form;
        private ShowMessage showmess = new ShowMessage();
        private string currentPath;

        public FileManager(Form1 form) 
        { 
            this.form = form;
        }


        public void OpenBmpFile(PictureBox picBox, Label label)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "BMP Files (*.bmp)|*.bmp";
                openFileDialog.Title = "Виберіть BMP-файл";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Bitmap image = new Bitmap(openFileDialog.FileName);

                        picBox.Image = image;
                        picBox.Size = image.Size;
                        currentPath = openFileDialog.FileName;
                        label.Visible = false;
                    }
                    catch (Exception ex)
                    {
                        showmess.ShowError($"{ex.Message}");
                    }
                }
            }
        }

        public void SaveImageFile(PictureBox picBox)
        {
            if (!IsImageLoaded(picBox)) return;

            if (string.IsNullOrEmpty(currentPath))
            {
                SaveImageAs(picBox);
            }
            else
            {
                SaveImg(currentPath, picBox.Image);
            }
        }

        public void SaveImageAs(PictureBox picBox)
        {
            if (!IsImageLoaded(picBox)) return;

            using (SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "BMP Files (*.bmp)|*.bmp|JPEG Files (*.jpg;*.jpeg)|*.jpg;*.jpeg|PNG Files (*.png)|*.png",
                Title = "Зберегти зображення як",
                FileName = "Untitled"
            })
            {
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    currentPath = saveDialog.FileName;
                    SaveImg(currentPath, picBox.Image);
                }
            }
        }

        private void SaveImg(string path, Image img)
        {
            try
            {
                string extension = Path.GetExtension(path)?.ToLower();
                System.Drawing.Imaging.ImageFormat format;

                if (extension == ".bmp")
                {
                    format = System.Drawing.Imaging.ImageFormat.Bmp;
                }
                else if (extension == ".jpg" || extension == ".jpeg")
                {
                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                }
                else if (extension == ".png")
                {
                    format = System.Drawing.Imaging.ImageFormat.Png;
                }
                else
                {
                    throw new NotSupportedException($"Unsupported file format: {extension}");
                }

                img.Save(path, format);
                showmess.ShowInfo("File successfully saved");
            }
            catch (Exception ex)
            {
                showmess.ShowError($"{ex.Message}");
            }
        }


        private bool IsImageLoaded(PictureBox picBox)
        {
            if (picBox.Image == null)
            {
                showmess.ShowError("No image to save");
                return false;
            }
            return true;
        }
    }
}
