using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace ImgProcess
{
    public partial class Form1 : Form
    {
        private Bitmap originalImg;
        private Bitmap origCopy; 
        private Bitmap adjustedImg;
        private Bitmap previewImg;
        private Bitmap currentImg;

        private PictureBox pictureBox1;
        private Panel panel;

        private enum ModeChosen { None, Brightness, Contrast }
        private ModeChosen currentMode = ModeChosen.None;

        private readonly FileManager filemanager;
        private readonly Viewer viewer;
        private readonly ShowMessage showmess;
        private readonly ToolbarManager toolmanager;
        private readonly ImageProcessor imgprocessor;
        private ValueForm formInvoke;
        private readonly Stack<Bitmap> undoStack = new Stack<Bitmap>();

        private bool isChanged = false;
        private int Bright = 0;
        private int Contrast = 0;


        public Form1()
        {
            InitializeComponent();
            InitializeComponents();

            filemanager = new FileManager(this);
            viewer = new Viewer();
            showmess = new ShowMessage();
            toolmanager = new ToolbarManager(toolStrip1);
            imgprocessor = new ImageProcessor();

            InitializeToolbarButtons();
            toolmanager.InitializeToolbar();

            this.Resize += Form1_Resize;
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }



        private void Form1_Resize(object sender, EventArgs e)
        {
            panel.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 110);

            viewer.Screen_Resize(label1, pictureBox1, panel, this);
        }

        private void InitializeComponents()
        {
            panel = new Panel
            {
                Location = new Point(0, 110),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 110),
                AutoScroll = true,

                BackColor = Color.FromArgb(224, 224, 224)
            };
            this.Controls.Add(panel);

            pictureBox1 = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.AutoSize
            };
            panel.Controls.Add(pictureBox1);

            hScrollBar1.Scroll += TrackBar1_Scroll;
            hScrollBar1.Visible = false;
            button1.Visible = false;
            label2.Visible = false;
        }

        private void InitializeToolbarButtons()
        {
            toolmanager.AddTool(toolStripButton1, Properties.Resources.new_file_bmp, "Open File", openToolStripMenuItem_Click_1);
            toolmanager.AddTool(toolStripButton2, Properties.Resources.open_file_bmp, "Save File", saveAsToolStripMenuItem_Click_1);
            toolmanager.AddTool(toolStripButton3, Properties.Resources.rubber_bmp, "Delete Image", disposeImageToolStripMenuItem_Click);
            toolmanager.AddTool(toolStripButton4, Properties.Resources.info_bmp, "Information", infoToolStripMenuItem_Click);
            toolmanager.AddTool(toolStripButton5, Properties.Resources.left_bmp1, "Revert", revertToolStripMenuItem_Click);
            toolmanager.AddTool(toolStripButton6, Properties.Resources.orig_bmp, "Return orig", returnOriginalToolStripMenuItem_Click);
            toolmanager.AddTool(toolStripButton7, Properties.Resources.sun_bmp, "Brightness", brightnessToolStripMenuItem_Click);
            toolmanager.AddTool(toolStripButton8, Properties.Resources.cotrsast_bmp, "Contrast", contrastToolStripMenuItem_Click);
            toolmanager.AddTool(toolStripButton9, Properties.Resources.rgb_bmp, "RGB", correctionRGBToolStripMenuItem_Click);
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            filemanager.OpenBmpFile(pictureBox1, label1);

            if (pictureBox1.Image != null)
            {
                originalImg = new Bitmap(pictureBox1.Image);
                origCopy = new Bitmap(originalImg);
            }

            adjustedImg = originalImg;

            CenterImage();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                saveToolStripMenuItem_Click_1(sender, e);
                e.Handled = true;
            }
        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            filemanager.SaveImageFile(pictureBox1);
        }

        private void saveAsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            filemanager.SaveImageAs(pictureBox1);
        }

        private void CenterImage()
        {
            viewer.Center_Img_OnScreen(pictureBox1, panel);
        }

        private void disposeImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewer.Dispose_Img_Screen(this, pictureBox1, label1))
            {
                undoStack.Clear();
                adjustedImg = null;

                Bright = 0;
                Contrast = 0;

                HideSubMenu();
            }
            else
            {
                showmess.ShowWarning("No image to dispose");
            }
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showmess.ShowInfo("Raster image processing program");
        }

        private void colorDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton10_Click(sender, e);
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {

            int value = hScrollBar1.Value;
            label2.Text = $"{value}";

            if (toolStripButton7.Checked)
            {
                previewImg = imgprocessor.AdjustBrightness(adjustedImg, value);
            } else
            {
                previewImg = imgprocessor.AdjustContrast(adjustedImg, value);
            }

            pictureBox1.Image = previewImg;
            pictureBox1.Refresh();
        }

        private void AdjustedImageUpdated(Bitmap adjustedImage)
        {
            pictureBox1.Image = adjustedImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            undoStack.Push(new Bitmap(adjustedImg));

            int value = hScrollBar1.Value;

            if (toolStripButton7.Checked)
            {
                Bright = value;
                adjustedImg = imgprocessor.AdjustBrightness(adjustedImg, value);
            } else
            {
                Contrast = value;
                adjustedImg = imgprocessor.AdjustContrast(adjustedImg, value);
            }

            AdjustedImageUpdated(adjustedImg);
        }

        private void brightnessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!viewer.IsImageLoaded_Screen(originalImg, adjustedImg))
            {
                showmess.ShowWarning("No image loaded to adjust brightness");
                return;
            }

            if (currentMode == ModeChosen.Brightness)
            {
                HideSubMenu();
            }
            else
            {
                toolmanager.SetSubScrollBar(hScrollBar1);
                ActivateAdjustmentMode(ModeChosen.Brightness, Bright, toolStripButton7);
            }
        }

        private void contrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!viewer.IsImageLoaded_Screen(originalImg, adjustedImg))
            {
                showmess.ShowWarning("No image loaded to adjust contrast");
                return;
            }

            pictureBox1.Refresh();

            if (currentMode == ModeChosen.Contrast)
            {
                HideSubMenu();
            }
            else
            {
                toolmanager.SetSubScrollBar(hScrollBar1);
                ActivateAdjustmentMode(ModeChosen.Contrast, Contrast, toolStripButton8);
            }
        }

        private void HideSubMenu()
        {
            hScrollBar1.Visible = false;
            label2.Visible = false;
            button1.Visible = false;

            toolStripButton7.Checked = false;
            toolStripButton8.Checked = false;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;

            pictureBox1.Image = adjustedImg;
            pictureBox1.Refresh();

            currentMode = ModeChosen.None;
        }

        private void ActivateAdjustmentMode(ModeChosen mode, int value, ToolStripButton activeButton)
        {
            label2.Text = value.ToString();
            hScrollBar1.Value = value;
            hScrollBar1.Visible = true;
            label2.Visible = true;
            button1.Visible = true;

            toolStripButton7.Checked = mode == ModeChosen.Brightness;
            toolStripButton8.Checked = mode == ModeChosen.Contrast;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;

            currentMode = mode;
        }

        private void correctionRGBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!viewer.IsImageLoaded_Screen(originalImg, adjustedImg))
            {
                showmess.ShowWarning("No image loaded to adjust RGB components");
                return;
            }

            previewImg = (Bitmap)adjustedImg.Clone();

            if (formInvoke == null || formInvoke.IsDisposed)
            {
                HideSubMenu();
                toolStripButton9.Checked = true;

                formInvoke = new ValueForm(adjustedImg);

                formInvoke.ImageUpdated += (adjustedImage) =>
                {
                    currentImg = adjustedImage;
                    pictureBox1.Image = adjustedImage;
                };

                formInvoke.ButtonClicked += () =>
                {
                    undoStack.Push(new Bitmap(adjustedImg));

                    adjustedImg = currentImg;
                    AdjustedImageUpdated(adjustedImg);

                    isChanged = true;
                };

                formInvoke.FormClosed += (s, ev) =>
                {
                    toolStripButton9.Checked = false;

                    if (!isChanged)
                    {
                        pictureBox1.Image = previewImg;
                    }
                    else 
                    {
                        pictureBox1.Image = adjustedImg;
                    }

                    isChanged = false;
                };

                formInvoke.Owner = this;
                formInvoke.Show();
            }
            else
            {
                formInvoke.Focus();
            }
        }

        private void returnOriginalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (origCopy != null)
            {
                pictureBox1.Image = new Bitmap(origCopy);
                adjustedImg = new Bitmap(origCopy);

                label1.Visible = false;
                Bright = 0;
                Contrast = 0;

                HideSubMenu();
            }
            else
            {
                showmess.ShowWarning("No original image to return");
            }
        }

        private void revertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 0)
            {
                adjustedImg = undoStack.Pop();
                pictureBox1.Image = adjustedImg;
            }
            else
            {
                showmess.ShowWarning("No previous changes to revert");
            }
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (originalImg != null && adjustedImg != null)
            {
                toolStripButton10.Checked = true;

                using (ColorDialog colorDialog = new ColorDialog())
                {
                    colorDialog.AllowFullOpen = false;
                    colorDialog.FullOpen = true;
                    colorDialog.FullOpen = true;
                    colorDialog.AnyColor = true;

                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            undoStack.Push(new Bitmap(adjustedImg));

                            int redCol = colorDialog.Color.R;
                            int greenCol = colorDialog.Color.G;
                            int blueCol = colorDialog.Color.B;

                            adjustedImg = imgprocessor.AdjustColorRGB(adjustedImg, redCol, greenCol, blueCol);

                            pictureBox1.Image = adjustedImg;
                            toolStripButton10.Checked = false;
                        }
                        catch (Exception ex)
                        {
                            showmess.ShowError($"{ex.Message}");
                        }
                    } else
                    {
                        toolStripButton10.Checked = false;
                    }
                }
            }
            else
            {
                showmess.ShowWarning("No image loaded to use Color Dialog Form");
            }
        }
    }
}
