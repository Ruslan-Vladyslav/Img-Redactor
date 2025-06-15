using System;
using System.Drawing;
using System.Windows.Forms;


namespace ImgProcess
{
    public class ToolbarManager
    {
        private ToolStrip toolStrip;

        public ToolbarManager(ToolStrip toolStrip)
        {
            this.toolStrip = toolStrip;
        }


        public void InitializeToolbar()
        {
            toolStrip.ImageScalingSize = new Size(24, 24);

            foreach (ToolStripItem item in toolStrip.Items)
            {
                item.Margin = new Padding(5);
            }
        }

        public void AddTool(ToolStripButton button, Image img, string tip, EventHandler click)
        {
            button.ToolTipText = tip;
            button.Image = img;
            button.Click += click;
        }

        public void SetSubScrollBar(HScrollBar bar)
        {
            bar.Minimum = 0;
            bar.Maximum = 219;
        }
    }
}
