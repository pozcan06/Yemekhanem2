using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Data.SQLite;
using System.IO;
using System.Collections;

namespace Odev_Project
{
    public partial class frmAnaSayfa : Form
    {
        string secilenFotoPath = "";
        int aktifMenuId = 0;
        bool girisYapildiMi = false;
        string girisYapanEmail = "";




        public frmAnaSayfa()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DatabaseHelper.CreateDatabaseIfNotExists();

            dtpGunlukTarih.Value = DateTime.Now;
            dtpMenuTarihi.Value = DateTime.Now;
            dtpAnalizAy.Value = DateTime.Now;

            cmbPuan.Items.Clear();
            cmbPuan.Items.Add("1");
            cmbPuan.Items.Add("2");
            cmbPuan.Items.Add("3");
            cmbPuan.Items.Add("4");
            cmbPuan.Items.Add("5");
            cmbPuan.SelectedIndex = 4;

            GunlukMenuyuYukle();
            AylikMenuListesiniYukle();
            btnCikisYap.Visible = false;
            lblKullaniciAdi.Text = "";
        }

        private void pnlUst_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void btnFotoSec_Click(object sender, EventArgs e)
        {
            ofdFotoSec.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";

            if (ofdFotoSec.ShowDialog() == DialogResult.OK)
            {
                secilenFotoPath = ofdFotoSec.FileName;
                pbMenuFoto.Image = Image.FromFile(secilenFotoPath);
            }
        }

        private void btnMenuKaydet_Click(object sender, EventArgs e)
        {
            if (girisYapildiMi == false)
            {
                MessageBox.Show("Menü ekleyebilmek için lütfen giriş yapınız.");
                return;
            }

            if (txtCorba.Text.Trim() == "" ||
                txtAnaYemek.Text.Trim() == "" ||
                txtYardimciYemek.Text.Trim() == "" ||
                txtTatli.Text.Trim() == "")
            {
                MessageBox.Show("Lütfen tüm yemek alanlarını doldurun.");
                return;
            }

            string kaydedilecekFotoPath = "";

            if (secilenFotoPath != "")
            {
                string imagesFolder = Path.Combine(Application.StartupPath, "Images");

                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                string fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(secilenFotoPath);
                kaydedilecekFotoPath = Path.Combine(imagesFolder, fileName);

                File.Copy(secilenFotoPath, kaydedilecekFotoPath, true);
            }

            using (SQLiteConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string query = @"
        INSERT OR REPLACE INTO Menus
        (MenuDate, Corba, AnaYemek, YardimciYemek, Tatli, ImagePath, CreatedAt)
        VALUES
        (@MenuDate, @Corba, @AnaYemek, @YardimciYemek, @Tatli, @ImagePath, @CreatedAt);";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MenuDate", dtpMenuTarihi.Value.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@Corba", txtCorba.Text.Trim());
                    command.Parameters.AddWithValue("@AnaYemek", txtAnaYemek.Text.Trim());
                    command.Parameters.AddWithValue("@YardimciYemek", txtYardimciYemek.Text.Trim());
                    command.Parameters.AddWithValue("@Tatli", txtTatli.Text.Trim());
                    command.Parameters.AddWithValue("@ImagePath", kaydedilecekFotoPath);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    command.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Menü başarıyla kaydedildi.");

            dtpGunlukTarih.Value = dtpMenuTarihi.Value;
            GunlukMenuyuYukle();
            AylikMenuListesiniYukle();

            MenuFormunuTemizle();

            tabAna.SelectedTab = tabGunlukMenu;
        }
        private void MenuFormunuTemizle()
        {
            txtCorba.Clear();
            txtAnaYemek.Clear();
            txtYardimciYemek.Clear();
            txtTatli.Clear();

            pbMenuFoto.Image = null;
            secilenFotoPath = "";

            dtpMenuTarihi.Value = DateTime.Now;
        }

        private void btnMenuTemizle_Click(object sender, EventArgs e)
        {
            MenuFormunuTemizle();
        }

        private void lblGosterYardımciYemek_Click(object sender, EventArgs e)
        {

        }

        private void GunlukMenuyuYukle()
        {
            string secilenTarih = dtpGunlukTarih.Value.ToString("yyyy-MM-dd");

            using (SQLiteConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM Menus WHERE MenuDate = @MenuDate";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MenuDate", secilenTarih);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pnlMenuKart.Visible = true;
                            lblMenuYok.Visible = false;

                            aktifMenuId = Convert.ToInt32(reader["Id"]);

                            lblKartTarih.Text = dtpGunlukTarih.Value.ToString("dd MMMM yyyy dddd");
                            lblGosterCorba.Text = "Çorba: " + reader["Corba"].ToString();
                            lblGosterAnaYemek.Text = "Ana Yemek: " + reader["AnaYemek"].ToString();
                            lblGosterYardimciYemek.Text = "Yardımcı Yemek: " + reader["YardimciYemek"].ToString();
                            lblGosterTatli.Text = "Tatlı / Meyve: " + reader["Tatli"].ToString();

                            string imagePath = reader["ImagePath"].ToString();

                            if (File.Exists(imagePath))
                            {
                                if (pbGosterFoto.Image != null)
                                {
                                    pbGosterFoto.Image.Dispose();
                                    pbGosterFoto.Image = null;
                                }

                                pbGosterFoto.Image = Image.FromFile(imagePath);
                            }
                            else
                            {
                                pbGosterFoto.Image = null;
                            }
                            YorumlariYukle();
                        }
                        else
                        {
                            aktifMenuId = 0;

                            pnlMenuKart.Visible = false;
                            lblMenuYok.Visible = true;
                            lblMenuYok.Text = "Bu tarih için menü bulunamadı.";

                            YorumlariYukle();
                        }
                    }
                }
            }
        }

        private void YorumlariYukle()
        {
            dgvYorumlar.Rows.Clear();
            dgvYorumlar.Columns.Clear();

            dgvYorumlar.Columns.Add("Rating", "Puan");
            dgvYorumlar.Columns.Add("UserEmail", "Kullanıcı");
            dgvYorumlar.Columns.Add("CommentText", "Yorum");
            dgvYorumlar.Columns.Add("CreatedAt", "Tarih");

            dgvYorumlar.ReadOnly = true;
            dgvYorumlar.AllowUserToAddRows = false;
            dgvYorumlar.AllowUserToDeleteRows = false;
            dgvYorumlar.RowHeadersVisible = false;

            dgvYorumlar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvYorumlar.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvYorumlar.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dgvYorumlar.DefaultCellStyle.ForeColor = Color.Black;
            dgvYorumlar.DefaultCellStyle.BackColor = Color.White;
            dgvYorumlar.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvYorumlar.DefaultCellStyle.SelectionBackColor = Color.LightGray;

            dgvYorumlar.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvYorumlar.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvYorumlar.EnableHeadersVisualStyles = false;

            dgvYorumlar.Columns["Rating"].FillWeight = 10;
            dgvYorumlar.Columns["UserEmail"].FillWeight = 25;
            dgvYorumlar.Columns["CommentText"].FillWeight = 45;
            dgvYorumlar.Columns["CreatedAt"].FillWeight = 25;

            dgvYorumlar.Columns["CommentText"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            if (aktifMenuId == 0)
            {
                return;
            }

            using (SQLiteConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string query = @"
        SELECT Rating, UserEmail, CommentText, CreatedAt
        FROM Comments
        WHERE MenuId = @MenuId
        ORDER BY Id DESC";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MenuId", aktifMenuId);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string yorum = reader["CommentText"].ToString();
                            string bolunmusYorum = YorumuSatirlaraBol(yorum);

                            string email = reader["UserEmail"].ToString();
                            string sansurluKullanici = KullaniciAdiniSansurle(email);

                            dgvYorumlar.Rows.Add(
                                reader["Rating"].ToString(),
                                sansurluKullanici,
                                bolunmusYorum,
                                Convert.ToDateTime(reader["CreatedAt"]).ToString("dd.MM.yyyy HH:mm")
                            );
                        }
                    }
                }
            }

            dgvYorumlar.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
        }

        private string YorumuSatirlaraBol(string yorum)
        {
            if (string.IsNullOrEmpty(yorum))
            {
                return "";
            }

            int satirUzunlugu = 30;
            string sonuc = "";

            for (int i = 0; i < yorum.Length; i += satirUzunlugu)
            {
                if (i + satirUzunlugu < yorum.Length)
                {
                    sonuc += yorum.Substring(i, satirUzunlugu) + Environment.NewLine;
                }
                else
                {
                    sonuc += yorum.Substring(i);
                }
            }

            return sonuc;
        }

        private void dtpGunlukTarih_ValueChanged(object sender, EventArgs e)
        {
            GunlukMenuyuYukle();
        }

        private void btnOncekiGun_Click(object sender, EventArgs e)
        {
            dtpGunlukTarih.Value = dtpGunlukTarih.Value.AddDays(-1);
        }

        private void btnSonrakiGun_Click(object sender, EventArgs e)
        {
            dtpGunlukTarih.Value = dtpGunlukTarih.Value.AddDays(1);
        }

        private void btnYorumEkle_Click(object sender, EventArgs e)
        {
            if (girisYapildiMi == false)
            {
                MessageBox.Show("Lütfen giriş yapınız.");
                return;
            }

            if (aktifMenuId == 0)
            {
                MessageBox.Show("Yorum yapabilmek için önce bu tarihte bir menü olmalı.");
                return;
            }

            if (txtYorum.Text.Trim() == "")
            {
                MessageBox.Show("Lütfen yorum yazın.");
                return;
            }

            using (SQLiteConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string query = @"
        INSERT INTO Comments
        (MenuId, FoodName, Rating, CommentText, UserEmail, CreatedAt)
        VALUES
        (@MenuId, @FoodName, @Rating, @CommentText, @UserEmail, @CreatedAt);";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MenuId", aktifMenuId);
                    command.Parameters.AddWithValue("@FoodName", "Genel Menü");
                    command.Parameters.AddWithValue("@Rating", Convert.ToInt32(cmbPuan.SelectedItem.ToString()));
                    command.Parameters.AddWithValue("@CommentText", txtYorum.Text.Trim());
                    command.Parameters.AddWithValue("@UserEmail", girisYapanEmail);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    command.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Yorum kaydedildi.");

            txtYorum.Clear();
            cmbPuan.SelectedIndex = 4;

            YorumlariYukle();
            AylikMenuListesiniYukle();
        }

        private void AylikMenuListesiniYukle()
        {
            string yilAy = dtpAnalizAy.Value.ToString("yyyy-MM");

            dgvAylikMenuler.Rows.Clear();
            dgvAylikMenuler.Columns.Clear();

            dgvAylikMenuler.Columns.Add("Tarih", "Tarih");
            dgvAylikMenuler.Columns.Add("Corba", "Çorba");
            dgvAylikMenuler.Columns.Add("AnaYemek", "Ana Yemek");
            dgvAylikMenuler.Columns.Add("YardimciYemek", "Yardımcı Yemek");
            dgvAylikMenuler.Columns.Add("Tatli", "Tatlı / Meyve");
            dgvAylikMenuler.Columns.Add("OrtalamaPuan", "Ortalama Puan");
            dgvAylikMenuler.Columns.Add("YorumSayisi", "Yorum Sayısı");

            dgvAylikMenuler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAylikMenuler.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvAylikMenuler.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvAylikMenuler.RowHeadersVisible = false;

            dgvAylikMenuler.DefaultCellStyle.ForeColor = Color.Black;
            dgvAylikMenuler.DefaultCellStyle.BackColor = Color.White;
            dgvAylikMenuler.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvAylikMenuler.DefaultCellStyle.SelectionBackColor = Color.LightGray;
            dgvAylikMenuler.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvAylikMenuler.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvAylikMenuler.EnableHeadersVisualStyles = false;

            dgvAylikMenuler.Columns["Tarih"].FillWeight = 18;
            dgvAylikMenuler.Columns["Corba"].FillWeight = 18;
            dgvAylikMenuler.Columns["AnaYemek"].FillWeight = 22;
            dgvAylikMenuler.Columns["YardimciYemek"].FillWeight = 22;
            dgvAylikMenuler.Columns["Tatli"].FillWeight = 22;
            dgvAylikMenuler.Columns["OrtalamaPuan"].FillWeight = 18;
            dgvAylikMenuler.Columns["YorumSayisi"].FillWeight = 15;

            using (SQLiteConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string aylikOzetQuery = @"
        SELECT 
            COUNT(c.Id) AS ToplamYorum,
            IFNULL(AVG(c.Rating), 0) AS OrtalamaPuan
        FROM Comments c
        INNER JOIN Menus m ON c.MenuId = m.Id
        WHERE substr(m.MenuDate, 1, 7) = @YilAy;";

                using (SQLiteCommand ozetCommand = new SQLiteCommand(aylikOzetQuery, connection))
                {
                    ozetCommand.Parameters.AddWithValue("@YilAy", yilAy);

                    using (SQLiteDataReader ozetReader = ozetCommand.ExecuteReader())
                    {
                        if (ozetReader.Read())
                        {
                            int toplamYorum = Convert.ToInt32(ozetReader["ToplamYorum"]);
                            double ortalamaPuan = Convert.ToDouble(ozetReader["OrtalamaPuan"]);

                            lblAylikToplamYorum.Text = "Toplam Yorum: " + toplamYorum;

                            if (toplamYorum == 0)
                            {
                                lblAylikOrtalamaPuan.Text = "Aylık Ortalama Puan: Puan yok";
                            }
                            else
                            {
                                lblAylikOrtalamaPuan.Text = "Aylık Ortalama Puan: " + ortalamaPuan.ToString("0.00");
                            }
                        }
                    }
                }

                string query = @"
        SELECT 
            m.Id,
            m.MenuDate,
            m.Corba,
            m.AnaYemek,
            m.YardimciYemek,
            m.Tatli,
            IFNULL(AVG(c.Rating), 0) AS OrtalamaPuan,
            COUNT(c.Id) AS YorumSayisi
        FROM Menus m
        LEFT JOIN Comments c ON m.Id = c.MenuId
        WHERE substr(m.MenuDate, 1, 7) = @YilAy
        GROUP BY m.Id
        ORDER BY m.MenuDate ASC;";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@YilAy", yilAy);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tarih = Convert.ToDateTime(reader["MenuDate"].ToString()).ToString("dd.MM.yyyy");

                            int yorumSayisi = Convert.ToInt32(reader["YorumSayisi"]);
                            double ortalamaPuan = Convert.ToDouble(reader["OrtalamaPuan"]);

                            string ortalamaText;

                            if (yorumSayisi == 0)
                            {
                                ortalamaText = "Puan yok";
                            }
                            else
                            {
                                ortalamaText = ortalamaPuan.ToString("0.00");
                            }

                            dgvAylikMenuler.Rows.Add(
                                tarih,
                                reader["Corba"].ToString(),
                                reader["AnaYemek"].ToString(),
                                reader["YardimciYemek"].ToString(),
                                reader["Tatli"].ToString(),
                                ortalamaText,
                                yorumSayisi.ToString()
                            );
                        }
                    }
                }
            }
        }

        private string KullaniciAdiniSansurle(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return "Bilinmiyor";
            }

            string kullaniciAdi = email.Split('@')[0];

            if (kullaniciAdi.Length <= 2)
            {
                return kullaniciAdi;
            }

            string ilkHarf = kullaniciAdi.Substring(0, 1);
            string sonHarf = kullaniciAdi.Substring(kullaniciAdi.Length - 1, 1);

            string yildizlar = new string('*', kullaniciAdi.Length - 2);

            return ilkHarf + yildizlar + sonHarf;
        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void btnAnalizYenile_Click(object sender, EventArgs e)
        {
            AylikMenuListesiniYukle();
        }

        private void dtpAnalizAy_ValueChanged(object sender, EventArgs e)
        {
            AylikMenuListesiniYukle();
        }

        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            frmGiris girisFormu = new frmGiris();

            if (girisFormu.ShowDialog() == DialogResult.OK)
            {
                girisYapildiMi = true;
                girisYapanEmail = girisFormu.GirisYapanEmail;

                string kullaniciAdi = girisYapanEmail.Split('@')[0];

                lblKullaniciAdi.Text = "Giriş yapan: " + kullaniciAdi;

                btnGirisYap.Visible = false;
                btnCikisYap.Visible = true;

                MessageBox.Show("Giriş başarılı. Artık yorum ve menü ekleyebilirsiniz.");
            }
        }

        private void btnCikisYap_Click(object sender, EventArgs e)
        {
            girisYapildiMi = false;
            girisYapanEmail = "";

            lblKullaniciAdi.Text = "";

            btnGirisYap.Visible = true;
            btnCikisYap.Visible = false;

            MessageBox.Show("Çıkış yapıldı.");
        }
    }
}
