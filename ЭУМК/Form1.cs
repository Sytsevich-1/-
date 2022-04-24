using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ЭУМК
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; 
                return cp;
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            theory newForm = new theory();
            newForm.Show();
        }

        private void ИнструкцияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Инструкция.pdf");
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            practice newForm = new practice();
            newForm.Show();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            control newForm = new control();
            newForm.Show();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            indw newForm = new indw();
            newForm.Show();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            additional newForm = new additional();
            newForm.Show();
        }

        private void рабочийКаталогToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string a = Directory.GetCurrentDirectory();
            System.Diagnostics.Process.Start(a);
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about newForm = new about();
            newForm.ShowDialog();
        }

        private void редактированиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            edit_form newForm = new edit_form();
            newForm.Show();
        }
    }
}
