﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace deneme1
{
    public partial class Payback : Form
    {
        public Payback()
        {
            InitializeComponent();
        }
        Model1 ent = new Model1();


        private void Paybgetp_Click(object sender, EventArgs e)
        {

        }

        private void Paybback_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm main = new MainForm();
            main.Show();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Payback_Load(object sender, EventArgs e)
        {
            guna2DataGridView1.DataSource = ent.BorcOdeme.ToList();
        }
    }
}
