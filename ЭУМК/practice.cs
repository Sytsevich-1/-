using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace ЭУМК
{
    public partial class practice : Form
    {
        public practice()
        {
            InitializeComponent();
        }

        private void practice_Load(object sender, EventArgs e)
        {
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=.\eemc.db; Version=3;"))
            {
                Connect.Open();
                string sqlExpression = "SELECT * FROM practice";
                SQLiteCommand command = new SQLiteCommand(sqlExpression, Connect);
                SQLiteDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        TreeNode[] brandNode = treeView1.Nodes.Find(reader["Наименование_работы"].ToString(), false);
                            brandNode = new TreeNode[] { treeView1.Nodes.Add(reader["Наименование_работы"].ToString(),
                                                     reader["Наименование_работы"].ToString()) };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка чтения",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (treeView1.GetNodeAt(e.X, e.Y) == null)
            {
                treeView1.SelectedNode = null;
            }
        }
        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=.\eemc.db; Version=3;"))
            {
                Connect.Open();
                string a = treeView1.SelectedNode.Name;
                string sqlExpression = "SELECT Путь_к_файлу FROM  practice WHERE Наименование_работы = '" + a + "'";
                SQLiteCommand command = new SQLiteCommand(sqlExpression, Connect);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string filename = reader["Путь_к_файлу"].ToString();
                    System.Diagnostics.Process.Start(Directory.GetCurrentDirectory() + "\\Практичеcкий раздел\\" + filename);
                }
                reader.Close();
            }
        }

    }
}
