using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgProcess
{
    public class ShowMessage
    {
        public ShowMessage() { }


        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ShowInfo(string message)
        {
            MessageBox.Show(message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
