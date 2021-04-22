using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Folder_Locker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
   
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            ToolTip toolTip2 = new ToolTip();
            toolTip2.ShowAlways = true;
            toolTip2.ToolTipIcon = ToolTipIcon.Info;
            toolTip2.SetToolTip(button2, "Before registering of your device,\nPlease enable bluetooth in your device");
            textBox1.Text = "To Access Your Folder, First register your device.";
        }

        private void button2_MouseHover(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.ShowAlways = true;
            toolTip1.ToolTipIcon = ToolTipIcon.Info;
            toolTip1.SetToolTip(button2, "Before registering of your device,\nPlease enable bluetooth in your device");
            textBox1.Text = "To Secure your Folder, You must have to Register your device through Bluetooth.";
        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            
            Form2 f2 = new Form2();
            f2.ShowDialog();
          
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.ShowDialog();
        }
    }
}
