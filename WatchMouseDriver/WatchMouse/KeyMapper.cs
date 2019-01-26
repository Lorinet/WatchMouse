using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WatchMouse
{
    public partial class KeyMapper : Form
    {
        public KeyMapper()
        {
            InitializeComponent();
            textBox1.Text = Form1.CustomKeys[0];
            textBox2.Text = Form1.CustomKeys[1];
            textBox3.Text = Form1.CustomKeys[2];
            textBox4.Text = Form1.CustomKeys[3];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1.CustomKeys[0] = textBox1.Text;
            Form1.CustomKeys[1] = textBox2.Text;
            Form1.CustomKeys[2] = textBox3.Text;
            Form1.CustomKeys[3] = textBox4.Text;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
