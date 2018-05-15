using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReceiveForm
{
    public partial class ReceiveForm : Form
    {
        public ReceiveForm()
        {
            InitializeComponent();
        }

        private void ShowBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show(textBox1.Text);
        }
    }
}
