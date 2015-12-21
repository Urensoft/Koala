using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
namespace Koala_Edit
{
    public partial class DonationForm : Form
    {
        public DonationForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > 0)
            {
                Process.Start("https://urensoftware.com/store/checkout.php?donate=" + numericUpDown1.Value);
                this.Close();
            }

        }
    }
}
