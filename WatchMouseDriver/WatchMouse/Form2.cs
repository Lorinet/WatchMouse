using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WatchMouse
{
    public partial class Form2 : Form
    {
        public Form2(string[] al)
        {
            InitializeComponent();
            foreach(string a in al)
            {
                listBox1.Items.Add(a);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Form1.hostname = listBox1.SelectedItem.ToString();
            Close();
        }
    }
}
