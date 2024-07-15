using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MAPF_System
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        { 
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { 
            Process.Start(linkLabel1.Text); 
        }

        private void button_Close_Click(object sender, EventArgs e)
        { 
            Close(); 
        }

        private void FormAlgorithm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void FormAbout_Deactivate(object sender, EventArgs e)
        {
            Close();
        }
    }
}
