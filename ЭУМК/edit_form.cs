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
    public partial class edit_form : Form
    {
        private SQLiteConnection Connect = null;
        private SQLiteCommandBuilder builder = null;
        //Теоретический раздел
        private SQLiteDataAdapter dataAdapter = null;
        private DataSet dataSet = null;
        private bool newRowAddint = false;
        //Практический раздел
        private SQLiteDataAdapter dataAdapter_2 = null;
        private DataSet dataSet_2 = null;
        private bool newRowAddint_2 = false;
        //Раздел конторя
        private SQLiteDataAdapter dataAdapter_3 = null;
        private DataSet dataSet_3 = null;
        private bool newRowAddint_3 = false;
        //Сам работа
        private SQLiteDataAdapter dataAdapter_4 = null;
        private DataSet dataSet_4 = null;
        private bool newRowAddint_4 = false;
        //Доп матер
        private SQLiteDataAdapter dataAdapter_5 = null;
        private DataSet dataSet_5 = null;
        private bool newRowAddint_5 = false;

        public edit_form()
        {
            InitializeComponent();
        }
        private void edit_form_Load(object sender, EventArgs e)
        {
            Connect = new SQLiteConnection(@"Data Source=.\eemc.db; Version=3;");
            Connect.Open();
            LoadData();
            LoadData_2();
            LoadData_3();
            LoadData_4();
            LoadData_5();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ReloadData();
            ReloadData_2();
            ReloadData_3();
            ReloadData_4();
            ReloadData_5();
        }

//---------------------------------------------------------------Теоретический раздел---------------------------------------------------------------------------
        private void LoadData()
        {
            try
            {
                dataAdapter = new SQLiteDataAdapter("SELECT *, 'Delete' AS [Command] FROM theory", Connect);
                builder = new SQLiteCommandBuilder(dataAdapter);
                builder.GetInsertCommand();
                builder.GetUpdateCommand();
                builder.GetDeleteCommand();

                dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "theory");

                dataGridView1.DataSource = dataSet.Tables["theory"];

                for(int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[4, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка чтения",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ReloadData()
        {
            try
            {
                dataSet.Tables["theory"].Clear();
                dataAdapter.Fill(dataSet, "theory");

                dataGridView1.DataSource = dataSet.Tables["theory"];

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[4, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка чтения",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 4)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();

                    if (task == "Delete")
                    {
                        if (MessageBox.Show("Удалить эту строку?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes)
                        {
                            int rowIndex = e.RowIndex;
                            dataGridView1.Rows.RemoveAt(rowIndex);
                            dataSet.Tables["theory"].Rows[rowIndex].Delete();
                            dataAdapter.Update(dataSet, "theory");
                        }
                    }
                    else if (task == "Insert")
                    {
                        int rowIndex = dataGridView1.Rows.Count - 2;
                        DataRow row = dataSet.Tables["theory"].NewRow();

                        row["Раздел"] = dataGridView1.Rows[rowIndex].Cells["Раздел"].Value;
                        row["Тема"] = dataGridView1.Rows[rowIndex].Cells["Тема"].Value;
                        row["Путь_к_файлу"] = dataGridView1.Rows[rowIndex].Cells["Путь_к_файлу"].Value;

                        dataSet.Tables["theory"].Rows.Add(row);
                        dataSet.Tables["theory"].Rows.RemoveAt(dataSet.Tables["theory"].Rows.Count - 1);
                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 2);
                        dataGridView1.Rows[e.RowIndex].Cells[4].Value = "Delete";

                        dataAdapter.Update(dataSet, "theory");
                        newRowAddint = false;
                    }
                    else if (task == "Update")
                    {
                        int r = e.RowIndex;

                        dataSet.Tables["theory"].Rows[r]["Раздел"] = dataGridView1.Rows[r].Cells["Раздел"].Value;
                        dataSet.Tables["theory"].Rows[r]["Тема"] = dataGridView1.Rows[r].Cells["Тема"].Value;
                        dataSet.Tables["theory"].Rows[r]["Путь_к_файлу"] = dataGridView1.Rows[r].Cells["Путь_к_файлу"].Value;

                        dataAdapter.Update(dataSet, "theory");
                        dataGridView1.Rows[e.RowIndex].Cells[4].Value = "Delete";
                    }
                    ReloadData();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if (newRowAddint == false)
                {
                    newRowAddint = true;
                    int lastRow = dataGridView1.Rows.Count-2;
                    DataGridViewRow row = dataGridView1.Rows[lastRow];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[4, lastRow] = linkCell;
                    row.Cells["Command"].Value = "Insert";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (newRowAddint == false)
                {
                    int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                    DataGridViewRow editingRow = dataGridView1.Rows[rowIndex];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[4, rowIndex] = linkCell;
                    editingRow.Cells["Command"].Value = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);

            if (dataGridView1.CurrentCell.ColumnIndex == 0)
            {
                TextBox textBox = e.Control as TextBox;
                if (textBox != null)
                {
                    textBox.KeyPress += new KeyPressEventHandler(Column_KeyPress);

                }
            }
        }
        private void Column_KeyPress(object sendet, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

//-----------------------------------------------------------------Практический раздел--------------------------------------------------------------------------

        private void LoadData_2()
        {
            try
            {
                dataAdapter_2 = new SQLiteDataAdapter("SELECT *, 'Delete' AS [Command] FROM practice", Connect);
                builder = new SQLiteCommandBuilder(dataAdapter_2);
                builder.GetInsertCommand();
                builder.GetUpdateCommand();
                builder.GetDeleteCommand();

                dataSet_2 = new DataSet();
                dataAdapter_2.Fill(dataSet_2, "practice");

                dataGridView2.DataSource = dataSet_2.Tables["practice"];

                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView2[4, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка чтения",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReloadData_2()
        {
            try
            {
                dataSet_2.Tables["practice"].Clear();
                dataAdapter_2.Fill(dataSet_2, "practice");

                dataGridView2.DataSource = dataSet_2.Tables["practice"];

                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView2[4, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка чтения",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 4)
                {
                    string task = dataGridView2.Rows[e.RowIndex].Cells[4].Value.ToString();

                    if (task == "Delete")
                    {
                        if (MessageBox.Show("Удалить эту строку?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            int rowIndex = e.RowIndex;
                            dataGridView2.Rows.RemoveAt(rowIndex);
                            dataSet_2.Tables["practice"].Rows[rowIndex].Delete();
                            dataAdapter_2.Update(dataSet_2, "practice");
                        }
                    }
                    else if (task == "Insert")
                    {
                        int rowIndex = dataGridView2.Rows.Count - 2;
                        DataRow row = dataSet_2.Tables["practice"].NewRow();

                        row["ID_theory"] = dataGridView2.Rows[rowIndex].Cells["ID_theory"].Value;
                        row["Наименование_работы"] = dataGridView2.Rows[rowIndex].Cells["Наименование_работы"].Value;
                        row["Путь_к_файлу"] = dataGridView2.Rows[rowIndex].Cells["Путь_к_файлу"].Value;

                        dataSet_2.Tables["practice"].Rows.Add(row);
                        dataSet_2.Tables["practice"].Rows.RemoveAt(dataSet_2.Tables["practice"].Rows.Count - 1);
                        dataGridView2.Rows.RemoveAt(dataGridView2.Rows.Count - 2);
                        dataGridView2.Rows[e.RowIndex].Cells[4].Value = "Delete";

                        dataAdapter_2.Update(dataSet_2, "practice");
                        newRowAddint_2 = false;
                    }
                    else if (task == "Update")
                    {
                        int r = e.RowIndex;

                        dataSet_2.Tables["practice"].Rows[r]["ID_theory"] = dataGridView2.Rows[r].Cells["ID_theory"].Value;
                        dataSet_2.Tables["practice"].Rows[r]["Наименование_работы"] = dataGridView2.Rows[r].Cells["Наименование_работы"].Value;
                        dataSet_2.Tables["practice"].Rows[r]["Путь_к_файлу"] = dataGridView2.Rows[r].Cells["Путь_к_файлу"].Value;

                        dataAdapter_2.Update(dataSet_2, "practice");
                        dataGridView2.Rows[e.RowIndex].Cells[4].Value = "Delete";
                    }
                    ReloadData_2();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView2_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if (newRowAddint_2 == false)
                {
                    newRowAddint_2 = true;
                    int lastRow = dataGridView2.Rows.Count - 2;
                    DataGridViewRow row = dataGridView2.Rows[lastRow];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView2[4, lastRow] = linkCell;
                    row.Cells["Command"].Value = "Insert";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (newRowAddint_2 == false)
                {
                    int rowIndex = dataGridView2.SelectedCells[0].RowIndex;
                    DataGridViewRow editingRow = dataGridView2.Rows[rowIndex];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView2[4, rowIndex] = linkCell;
                    editingRow.Cells["Command"].Value = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);

            if (dataGridView2.CurrentCell.ColumnIndex == 1)
            {
                TextBox textBox = e.Control as TextBox;
                if (textBox != null)
                {
                    textBox.KeyPress += new KeyPressEventHandler(Column_KeyPress);

                }
            }
        }
   
//-----------------------------------------------------------------Раздел контроля знаний--------------------------------------------------------------------------

        private void LoadData_3()
        {
            try
            {
                dataAdapter_3 = new SQLiteDataAdapter("SELECT *, 'Delete' AS [Command] FROM control", Connect);
                builder = new SQLiteCommandBuilder(dataAdapter_3);
                builder.GetInsertCommand();
                builder.GetUpdateCommand();
                builder.GetDeleteCommand();

                dataSet_3 = new DataSet();
                dataAdapter_3.Fill(dataSet_3, "control");

                dataGridView3.DataSource = dataSet_3.Tables["control"];

                for (int i = 0; i < dataGridView3.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView3[5, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка чтения",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReloadData_3()
        {
            try
            {
                dataSet_3.Tables["control"].Clear();
                dataAdapter_3.Fill(dataSet_3, "control");

                dataGridView3.DataSource = dataSet_3.Tables["control"];

                for (int i = 0; i < dataGridView3.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView3[5, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка чтения",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 5)
                {
                    string task = dataGridView3.Rows[e.RowIndex].Cells[5].Value.ToString();

                    if (task == "Delete")
                    {
                        if (MessageBox.Show("Удалить эту строку?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            int rowIndex = e.RowIndex;
                            dataGridView3.Rows.RemoveAt(rowIndex);
                            dataSet_3.Tables["control"].Rows[rowIndex].Delete();
                            dataAdapter_3.Update(dataSet_3, "control");
                        }
                    }
                    else if (task == "Insert")
                    {
                        int rowIndex = dataGridView3.Rows.Count - 2;
                        DataRow row = dataSet_3.Tables["control"].NewRow();

                        row["ID_theory"] = dataGridView3.Rows[rowIndex].Cells["ID_theory"].Value;
                        row["Раздел"] = dataGridView3.Rows[rowIndex].Cells["Раздел"].Value;
                        row["Наименование"] = dataGridView3.Rows[rowIndex].Cells["Наименование"].Value;
                        row["Путь_к_файлу"] = dataGridView3.Rows[rowIndex].Cells["Путь_к_файлу"].Value;

                        dataSet_3.Tables["control"].Rows.Add(row);
                        dataSet_3.Tables["control"].Rows.RemoveAt(dataSet_3.Tables["control"].Rows.Count - 1);
                        dataGridView3.Rows.RemoveAt(dataGridView3.Rows.Count - 2);
                        dataGridView3.Rows[e.RowIndex].Cells[5].Value = "Delete";

                        dataAdapter_3.Update(dataSet_3, "control");
                        newRowAddint_3 = false;
                    }
                    else if (task == "Update")
                    {
                        int r = e.RowIndex;

                        dataSet_3.Tables["control"].Rows[r]["ID_theory"] = dataGridView3.Rows[r].Cells["ID_theory"].Value;
                        dataSet_3.Tables["control"].Rows[r]["Раздел"] = dataGridView3.Rows[r].Cells["Раздел"].Value;
                        dataSet_3.Tables["control"].Rows[r]["Наименование"] = dataGridView3.Rows[r].Cells["Наименование"].Value;
                        dataSet_3.Tables["control"].Rows[r]["Путь_к_файлу"] = dataGridView3.Rows[r].Cells["Путь_к_файлу"].Value;

                        dataAdapter_3.Update(dataSet_3, "control");
                        dataGridView3.Rows[e.RowIndex].Cells[5].Value = "Delete";
                    }
                    ReloadData_3();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView3_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if (newRowAddint_3 == false)
                {
                    newRowAddint_3 = true;
                    int lastRow = dataGridView3.Rows.Count - 2;
                    DataGridViewRow row = dataGridView3.Rows[lastRow];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView3[5, lastRow] = linkCell;
                    row.Cells["Command"].Value = "Insert";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView3_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (newRowAddint_3 == false)
                {
                    int rowIndex = dataGridView3.SelectedCells[0].RowIndex;
                    DataGridViewRow editingRow = dataGridView3.Rows[rowIndex];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView3[5, rowIndex] = linkCell;
                    editingRow.Cells["Command"].Value = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView3_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);

            if (dataGridView3.CurrentCell.ColumnIndex == 1)
            {
                TextBox textBox = e.Control as TextBox;
                if (textBox != null)
                {
                    textBox.KeyPress += new KeyPressEventHandler(Column_KeyPress);

                }
            }
        }

//-----------------------------------------------------------------Самостоятельная работа---------------------------------------------------------------------

        private void LoadData_4()
        {
            try
            {
                dataAdapter_4 = new SQLiteDataAdapter("SELECT *, 'Delete' AS [Command] FROM ind_w", Connect);
                builder = new SQLiteCommandBuilder(dataAdapter_4);
                builder.GetInsertCommand();
                builder.GetUpdateCommand();
                builder.GetDeleteCommand();

                dataSet_4 = new DataSet();
                dataAdapter_4.Fill(dataSet_4, "ind_w");

                dataGridView4.DataSource = dataSet_4.Tables["ind_w"];

                for (int i = 0; i < dataGridView4.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView4[5, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка чтения",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReloadData_4()
        {
            try
            {
                dataSet_4.Tables["ind_w"].Clear();
                dataAdapter_4.Fill(dataSet_4, "ind_w");

                dataGridView4.DataSource = dataSet_4.Tables["ind_w"];

                for (int i = 0; i < dataGridView4.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView4[5, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка чтения",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 5)
                {
                    string task = dataGridView4.Rows[e.RowIndex].Cells[5].Value.ToString();

                    if (task == "Delete")
                    {
                        if (MessageBox.Show("Удалить эту строку?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            int rowIndex = e.RowIndex;
                            dataGridView4.Rows.RemoveAt(rowIndex);
                            dataSet_4.Tables["ind_w"].Rows[rowIndex].Delete();
                            dataAdapter_4.Update(dataSet_4, "ind_w");
                        }
                    }
                    else if (task == "Insert")
                    {
                        int rowIndex = dataGridView4.Rows.Count - 2;
                        DataRow row = dataSet_4.Tables["ind_w"].NewRow();

                        row["ID_theory"] = dataGridView4.Rows[rowIndex].Cells["ID_theory"].Value;
                        row["Раздел"] = dataGridView4.Rows[rowIndex].Cells["Раздел"].Value;
                        row["Наименование"] = dataGridView4.Rows[rowIndex].Cells["Наименование"].Value;
                        row["Путь_к_файлу"] = dataGridView4.Rows[rowIndex].Cells["Путь_к_файлу"].Value;

                        dataSet_4.Tables["ind_w"].Rows.Add(row);
                        dataSet_4.Tables["ind_w"].Rows.RemoveAt(dataSet_4.Tables["ind_w"].Rows.Count - 1);
                        dataGridView4.Rows.RemoveAt(dataGridView4.Rows.Count - 2);
                        dataGridView4.Rows[e.RowIndex].Cells[5].Value = "Delete";

                        dataAdapter_4.Update(dataSet_4, "ind_w");
                        newRowAddint_4 = false;
                    }
                    else if (task == "Update")
                    {
                        int r = e.RowIndex;
                        dataSet_4.Tables["ind_w"].Rows[r]["ID_theory"] = dataGridView4.Rows[r].Cells["ID_theory"].Value;
                        dataSet_4.Tables["ind_w"].Rows[r]["Раздел"] = dataGridView4.Rows[r].Cells["Раздел"].Value;
                        dataSet_4.Tables["ind_w"].Rows[r]["Наименование"] = dataGridView4.Rows[r].Cells["Наименование"].Value;
                        dataSet_4.Tables["ind_w"].Rows[r]["Путь_к_файлу"] = dataGridView4.Rows[r].Cells["Путь_к_файлу"].Value;

                        dataAdapter_4.Update(dataSet_4, "ind_w");
                        dataGridView4.Rows[e.RowIndex].Cells[5].Value = "Delete";
                    }
                    ReloadData_4();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView4_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if (newRowAddint_4 == false)
                {
                    newRowAddint_4 = true;
                    int lastRow = dataGridView4.Rows.Count - 2;
                    DataGridViewRow row = dataGridView4.Rows[lastRow];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView4[5, lastRow] = linkCell;
                    row.Cells["Command"].Value = "Insert";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView4_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (newRowAddint_4 == false)
                {
                    int rowIndex = dataGridView4.SelectedCells[0].RowIndex;
                    DataGridViewRow editingRow = dataGridView4.Rows[rowIndex];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView4[5, rowIndex] = linkCell;
                    editingRow.Cells["Command"].Value = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView4_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);

            if (dataGridView4.CurrentCell.ColumnIndex == 1)
            {
                TextBox textBox = e.Control as TextBox;
                if (textBox != null)
                {
                    textBox.KeyPress += new KeyPressEventHandler(Column_KeyPress);

                }
            }
        }

//-----------------------------------------------------------------Дополнительные материалы--------------------------------------------------------------------

        private void LoadData_5()
        {
            try
            {
                dataAdapter_5 = new SQLiteDataAdapter("SELECT *, 'Delete' AS [Command] FROM additional", Connect);
                builder = new SQLiteCommandBuilder(dataAdapter_5);
                builder.GetInsertCommand();
                builder.GetUpdateCommand();
                builder.GetDeleteCommand();

                dataSet_5 = new DataSet();
                dataAdapter_5.Fill(dataSet_5, "additional");

                dataGridView5.DataSource = dataSet_5.Tables["additional"];

                for (int i = 0; i < dataGridView5.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView5[5, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка чтения",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReloadData_5()
        {
            try
            {
                dataSet_5.Tables["additional"].Clear();
                dataAdapter_5.Fill(dataSet_5, "additional");

                dataGridView5.DataSource = dataSet_5.Tables["additional"];

                for (int i = 0; i < dataGridView5.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView5[5, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка чтения",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView5_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 5)
                {
                    string task = dataGridView5.Rows[e.RowIndex].Cells[5].Value.ToString();

                    if (task == "Delete")
                    {
                        if (MessageBox.Show("Удалить эту строку?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            int rowIndex = e.RowIndex;
                            dataGridView5.Rows.RemoveAt(rowIndex);
                            dataSet_5.Tables["additional"].Rows[rowIndex].Delete();
                            dataAdapter_5.Update(dataSet_5, "additional");
                        }
                    }
                    else if (task == "Insert")
                    {
                        int rowIndex = dataGridView5.Rows.Count - 2;
                        DataRow row = dataSet_5.Tables["additional"].NewRow();

                        row["ID_theory"] = dataGridView5.Rows[rowIndex].Cells["ID_theory"].Value;
                        row["Раздел"] = dataGridView5.Rows[rowIndex].Cells["Раздел"].Value;
                        row["Наименование"] = dataGridView5.Rows[rowIndex].Cells["Наименование"].Value;
                        row["Путь_к_файлу"] = dataGridView5.Rows[rowIndex].Cells["Путь_к_файлу"].Value;

                        dataSet_5.Tables["additional"].Rows.Add(row);
                        dataSet_5.Tables["additional"].Rows.RemoveAt(dataSet_5.Tables["additional"].Rows.Count - 1);
                        dataGridView5.Rows.RemoveAt(dataGridView5.Rows.Count - 2);
                        dataGridView5.Rows[e.RowIndex].Cells[5].Value = "Delete";

                        dataAdapter_5.Update(dataSet_5, "additional");
                        newRowAddint_5 = false;
                    }
                    else if (task == "Update")
                    {
                        int r = e.RowIndex;
                        dataSet_5.Tables["additional"].Rows[r]["ID_theory"] = dataGridView5.Rows[r].Cells["ID_theory"].Value;
                        dataSet_5.Tables["additional"].Rows[r]["Раздел"] = dataGridView5.Rows[r].Cells["Раздел"].Value;
                        dataSet_5.Tables["additional"].Rows[r]["Наименование"] = dataGridView5.Rows[r].Cells["Наименование"].Value;
                        dataSet_5.Tables["additional"].Rows[r]["Путь_к_файлу"] = dataGridView5.Rows[r].Cells["Путь_к_файлу"].Value;

                        dataAdapter_5.Update(dataSet_5, "additional");
                        dataGridView5.Rows[e.RowIndex].Cells[5].Value = "Delete";
                    }
                    ReloadData_5();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView5_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if (newRowAddint_5 == false)
                {
                    newRowAddint_5 = true;
                    int lastRow = dataGridView5.Rows.Count - 2;
                    DataGridViewRow row = dataGridView5.Rows[lastRow];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView5[5, lastRow] = linkCell;
                    row.Cells["Command"].Value = "Insert";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView5_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (newRowAddint_5 == false)
                {
                    int rowIndex = dataGridView5.SelectedCells[0].RowIndex;
                    DataGridViewRow editingRow = dataGridView5.Rows[rowIndex];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView5[5, rowIndex] = linkCell;
                    editingRow.Cells["Command"].Value = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView5_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);

            if (dataGridView5.CurrentCell.ColumnIndex == 1)
            {
                TextBox textBox = e.Control as TextBox;
                if (textBox != null)
                {
                    textBox.KeyPress += new KeyPressEventHandler(Column_KeyPress);

                }
            }
        }

        private void edit_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Connect.Close();
        }
    }
}
