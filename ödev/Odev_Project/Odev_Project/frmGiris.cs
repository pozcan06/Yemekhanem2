using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Odev_Project
{
    public partial class frmGiris : Form
    {

        string dogrulamaKodu = "";
        public string GirisYapanEmail = "";

        public frmGiris()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOnayKoduGonder_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (email == "")
            {
                MessageBox.Show("Lütfen e-posta adresinizi girin.");
                return;
            }

            if (!(email.EndsWith("@ogr.kocaeli.edu.tr") || email.EndsWith("@kocaeli.edu.tr")))
            {
                MessageBox.Show("Sadece @ogr.kocaeli.edu.tr veya @kocaeli.edu.tr uzantılı e-postalar kabul edilir.");
                return;
            }

            Random random = new Random();
            dogrulamaKodu = random.Next(1000, 9999).ToString();

            MessageBox.Show("Onay kodunuz: " + dogrulamaKodu, "Onay Kodu");

            txtOnayKodu.Focus();
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string girilenKod = txtOnayKodu.Text.Trim();

            if (email == "")
            {
                MessageBox.Show("Lütfen e-posta adresinizi girin.");
                return;
            }

            if (!(email.EndsWith("@ogr.kocaeli.edu.tr") || email.EndsWith("@kocaeli.edu.tr")))
            {
                MessageBox.Show("Sadece @ogr.kocaeli.edu.tr veya @kocaeli.edu.tr uzantılı e-postalar kabul edilir.");
                return;
            }

            if (dogrulamaKodu == "")
            {
                MessageBox.Show("Önce onay kodu gönderin.");
                return;
            }

            if (girilenKod == "")
            {
                MessageBox.Show("Lütfen onay kodunu girin.");
                return;
            }

            if (girilenKod != dogrulamaKodu)
            {
                MessageBox.Show("Onay kodu hatalı.");
                return;
            }

            GirisYapanEmail = email;

            MessageBox.Show("Giriş başarılı.");

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
