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
    public partial class Form_password : Form
    {
        
        public Form_password()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void Form_password_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox_Show_Hide.Checked)
            {
                textBoxPass.UseSystemPasswordChar = true;
            }
            else
            {
                textBoxPass.UseSystemPasswordChar = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    
        const int MIN_LENGTH = 6;
        
        private void button1_Click(object sender, EventArgs e)
        {
            /*  if (textBoxPass.Text == null)
              {
                  MessageBox.Show("Password Can't be empty.");
                  this.Close();
              }*/
            if (textBoxPass.Text.Trim() == string.Empty)
            {
                MessageBox.Show("This Field cannot left empty!");
                return; // return because we don't want to run normal code of buton click
            }
            if (textBoxPass.TextLength <  MIN_LENGTH )
            {
                MessageBox.Show("Password Must be Longer than 6 letters!");
                return;
            }
            returnValue();
            this.Close();
        }
        private void textBoxPass_TextChanged(object sender, EventArgs e)
        {

        }
        public string returnValue()
        {
            string return_pass = textBoxPass.Text;
            return return_pass;
        }

    }
}
