using System;
using System.Drawing;
using System.Windows.Forms;



namespace ImgProcess
{
    public partial class ValueForm : Form
    {
        public event Action<Bitmap> ImageUpdated;
        public event Action ButtonClicked;

        private Bitmap originalImg;
        private Bitmap adjustedImage;

        private Form1 form;
        private ImageProcessor process;

        private int RedCol = 0;
        private int GreenCol = 0;
        private int BlueCol = 0;

        public ValueForm(Bitmap image)
        {
            InitializeComponent();

            originalImg = new Bitmap(image);
            form = new Form1();
            process = new ImageProcessor();

            InitializeComponents(RedCol, GreenCol, BlueCol);
        }

        public void InitializeComponents(int r, int g, int b)
        {
            trackBar1.Value = r;
            trackBar2.Value = g;
            trackBar3.Value = b;

            textBox1.Text = r.ToString();
            textBox2.Text = g.ToString();
            textBox3.Text = b.ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int r, g, b;

            textBox1.Text = Convert.ToString(trackBar1.Value);
            textBox2.Text = Convert.ToString(trackBar2.Value);
            textBox3.Text = Convert.ToString(trackBar3.Value);

            r = Convert.ToInt32(textBox1.Text);
            g = Convert.ToInt32(textBox2.Text);
            b = Convert.ToInt32(textBox3.Text);

            panel1.BackColor = Color.FromArgb(r, g, b);

            adjustedImage = process.AdjustColorRGB(originalImg, r, g, b);
            ImageUpdated?.Invoke(adjustedImage);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ButtonClicked?.Invoke();
        }

        private void ValueForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
        }

        private void ValueForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }
    }
}
