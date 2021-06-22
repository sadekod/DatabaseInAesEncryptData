using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SQLiteConnection con;
        SQLiteDataAdapter da;
        SQLiteCommand cmd;
        DataSet ds;

        void GetList()
        {
            con = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            da = new SQLiteDataAdapter("Select *From User", con);
            ds = new DataSet();
            con.Open();
            da.Fill(ds, "User");
            dt.DataSource = ds.Tables["User"];
            con.Close();
        }
        private DataGridView dt = new DataGridView();
        private void Form1_Load(object sender, EventArgs e)
        {
            
            dt.Top=155;
            dt.Left = 50;
            dt.Width = 400;
            dt.BackgroundColor = Color.White;
            Controls.Add(dt);
            
            if (!File.Exists("MyDatabase.sqlite"))
            {
                SQLiteConnection.CreateFile("MyDatabase.sqlite");
                
                string sql = @"CREATE TABLE User(
                               ID INTEGER PRIMARY KEY AUTOINCREMENT ,
                               EncryptPassword           TEXT      NOT NULL,
                               DecryptPassword           TEXT      NOT NULL
                            );";
                con = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
                con.Open();
                cmd = new SQLiteCommand(sql, con);
                cmd.ExecuteNonQuery();
                con.Close();

            }

            //DataGridView doldur
            GetList();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            cmd = new SQLiteCommand();
            con.Open();
            cmd.Connection = con;
            string EncryptPassword = MD5Hash(textBox2.Text);
            cmd.CommandText = "update User set DecryptPassword='" + textBox2.Text + "', EncryptPassword='"+ EncryptPassword.ToString() + "' where ID=" + textBox1.Text + "";
            cmd.ExecuteNonQuery();
            con.Close();
            GetList();
        }
        //MD5 şifrelemesini yapacak sınıfı ekliyoruz
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //metnin boyutuna göre hash hesaplar
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //hesapladıktan sonra hashi alır
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //her baytı 2 hexadecimal hane olarak değiştirir
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string EncryptPassword = MD5Hash(textBox2.Text);
            cmd = new SQLiteCommand();
            con.Open();
            cmd.Connection = con;
            cmd.CommandText = "insert into User(DecryptPassword,EncryptPassword) values ('" + textBox2.Text + "','"+ EncryptPassword.ToString() + "')";
            cmd.ExecuteNonQuery();
            con.Close();
            GetList();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            cmd = new SQLiteCommand();
            con.Open();
            cmd.Connection = con;
            cmd.CommandText = "delete from User where ID=" + textBox1.Text + "";
            cmd.ExecuteNonQuery();
            con.Close();
            GetList();
        }
    }
}
