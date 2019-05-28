using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Interstates.Control.Database.Forms
{
    partial class NewProfile : Form
    {
        private string _strProfileName;

        public NewProfile(string name)
        {
            InitializeComponent();
            _strProfileName = name;
            txtProfileName.Text = _strProfileName;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public string ProfileName
        {
            get { return _strProfileName; }
            set { _strProfileName = value; }
        }

        private void txtProfileName_TextChanged(object sender, EventArgs e)
        {
            _strProfileName = txtProfileName.Text;
        }
    }
}