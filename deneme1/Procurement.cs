﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace deneme1
{
    public partial class Procurement : Form
    {
        public Procurement()
        {
            InitializeComponent();
        }

        Model1 ent = new Model1();

        

        private void PMadd_Click(object sender, EventArgs e)
        {
            if (PMtype.Text == "" || PMamount.Text == "" || PMname.Text == "" || PMprice.Text == "")
            {
                MessageBox.Show("Missing information");
            }
            else
            {
                try
                {

                    Tedarik tblpm = new Tedarik();
                    tblpm.tedarikciadi = PMname.Text;
                    tblpm.tur = PMtype.Text;
                    tblpm.miktar = Convert.ToDouble(PMamount.Text);
                    tblpm.alisfiyat = Convert.ToDouble(PMprice.Text);
                    try
                    {
                        ProviderCheck(PMname.Text.Trim());
                        Random rand = new Random();
                        long randnum2 = (long)(rand.NextDouble() * 9999999999999) + 1000000000000;                   
                        tblpm.irsaliyenumarasi = randnum2;
     
                        ent.Tedarik.Add(tblpm);
                        ent.SaveChanges();
                        PMtype.Clear();
                        PMamount.Clear();
                        PMname.Clear();
                        PMprice.Clear();
                        guna2DataGridView1.DataSource = ent.Tedarik.ToList();
                    }
                    catch (Exception Myex)
                    {
                        MessageBox.Show(Myex.Message);
                    }

                }
                catch (Exception Myex)
                {
                    MessageBox.Show(Myex.Message);
                }
            }
        }

        private void PMdelete_Click(object sender, EventArgs e)
        {
            if (PMprice.Text == "" || PMamount.Text == "")
            {
                MessageBox.Show("Missing information");
            }
            else
            {   try
                {
                    long irsaliyeNo = Convert.ToInt64(PMprice.Text);
                    double amount = Convert.ToDouble(PMamount.Text);
                    Tedarik tblpm = new Tedarik();
                    //Tedarik tblpm = ent.Tedarik.First(f => Convert.ToInt64(f.irsaliyenumarasi) == irsaliyeNo);
                    try
                    {
                        Tuple<double, double> product = ProcurementCheck(amount, irsaliyeNo);
                        Tedarik tedarik = ent.Tedarik.Find(product.Item1);
                        tedarik.miktar = product.Item2;
                        string text = Convert.ToString(tedarik.id).Trim()+" " + Convert.ToString(tedarik.tedarikciadi).Trim() + " " + Convert.ToString(tedarik.tur).Trim() + " "+ Convert.ToString(amount).Trim()+" " + Convert.ToString(tedarik.alisfiyat).Trim() +" " + Convert.ToString(tedarik.irsaliyenumarasi).Trim() + "\r\n";
                        File.AppendAllText("tedarik.txt", text);
                        ent.SaveChanges();
                        PMtype.Clear();
                        PMamount.Clear();
                        PMname.Clear();
                        PMprice.Clear();
                        guna2DataGridView1.DataSource = ent.Tedarik.ToList();

                    }
                    catch (Exception Myex)
                    {
                        MessageBox.Show(Myex.Message);
                    }

                }
                catch (Exception Myex)
                {
                    MessageBox.Show(Myex.Message);
                }
            }
        }

        private void PMback_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm main = new MainForm();
            main.Show();
            Add_To_Odeme();
        }

        private static void ProviderCheck(string tedarikciAd)

        {
            string machineName = Environment.MachineName;
            string connectionHost = string.Format(@"Data Source={0}\SQLEXPRESS;Initial Catalog=proje_deneme;Integrated Security=True", machineName);
            string connectionString = connectionHost;
            string queryString =
                "SELECT tedarikciadi FROM dbo.Tedarikci;";
            using (SqlConnection connection = new SqlConnection(
                       connectionString))
            {
                SqlCommand command = new SqlCommand(
                    queryString, connection);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tedarikciAdiLabel = Convert.ToString(reader[0]).Trim();
                        if (tedarikciAdiLabel == tedarikciAd)
                        {
                            string succesMessage = "Basarili bir sekilde eklendi";
                            MessageBox.Show(succesMessage);
                            return;
                        }

                    }

                }
            }
            string dangerMessage = "Boyle bir tedarikci bulunmamaktadır!";
            throw new InvalidCastException(dangerMessage);
        }

        private static Tuple<double,double> ProcurementCheck(double miktarLabel,long irsaliyeNoLabel)

        {
            string machineName = Environment.MachineName;
            string connectionHost = string.Format(@"Data Source={0}\SQLEXPRESS;Initial Catalog=proje_deneme;Integrated Security=True", machineName);
            string connectionString = connectionHost;
            string queryString =
                "SELECT miktar, irsaliyenumarasi, id FROM dbo.Tedarik;";
            using (SqlConnection connection = new SqlConnection(
                       connectionString))
            {
                SqlCommand command = new SqlCommand(
                    queryString, connection);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string miktar = Convert.ToString(reader[0]).Trim();
                        string irsaliyeNo = Convert.ToString(reader[1]).Trim();
                        string id = Convert.ToString(reader[2]).Trim();


                        if (Convert.ToString(irsaliyeNoLabel) == irsaliyeNo )
                        {
                            if (Convert.ToDouble(miktar) == 0)
                            {
                                string productEmptyError = "Tedarikcide urun kalmamis!";
                                throw new InvalidCastException(productEmptyError);
                            }
                            else if(Convert.ToDouble(miktarLabel) - Convert.ToDouble(miktar) > 0 )
                            {
                                string productSizeError = "Urun sayisindan fazla urun girdiniz";
                                throw new InvalidCastException(productSizeError);
                            } else
                            {
                                string successMessage = "Basarili bir sekilde eklendi";
                                MessageBox.Show(successMessage);
                                double result = Convert.ToDouble(miktar) - Convert.ToDouble(miktarLabel);
                                return new Tuple<double, double>(Convert.ToDouble(id), result); ;
                            }

                        }


                    }

                }
            }
            string dangerMessage = "Boyle bir irsaliyeNo yoktur!";
            throw new InvalidCastException(dangerMessage);
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            PMname.Text = guna2DataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            PMtype.Text = guna2DataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            PMamount.Text = guna2DataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            PMprice.Text = guna2DataGridView1.SelectedRows[0].Cells[4].Value.ToString();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Add_To_Odeme();
            Application.Exit();
        }

        private void Add_To_Odeme()
        {
            bool isFileExist = File.Exists("tedarik.txt");
            if (isFileExist)
            {
                var lines = File.ReadAllLines("tedarik.txt");

                for (var i = 0; i < lines.Length; i++)
                {
                    Random rand = new Random();
                    long randnum2 = (long)(rand.NextDouble() * 9999999999999) + 1000000000000;
                    var line = lines[i];
                    string[] tedarik = line.Split(' ');
                    Urun tblpm = new Urun();
                    tblpm.alis_fiyat = Convert.ToDouble(tedarik[4]);
                    tblpm.tur = tedarik[2];
                    tblpm.irsaliyeno = Convert.ToInt64(tedarik[5]);
                    tblpm.miktar = Convert.ToInt16(tedarik[3]);
                    tblpm.satis_fiyat = 0;
                    tblpm.barkod = randnum2;
                    Odeme odeme = new Odeme();
                    odeme.musteriadi = "Cenkay";
                    odeme.saticiadi = tedarik[1];
                    odeme.odememiktari = Convert.ToInt16(tedarik[3]) * Convert.ToDouble(tedarik[4]);
                    DateTime dateTime = DateTime.UtcNow.Date;
                    odeme.odemetarihi = dateTime;


                    ent.Urun.Add(tblpm);
                    ent.Odeme.Add(odeme);
                    ent.SaveChanges();
                }

                File.Delete("tedarik.txt");

            }
        }

        private void Procurement_Load(object sender, EventArgs e)
        {
            guna2DataGridView1.DataSource = ent.Tedarik.ToList();
        }
    }
}
