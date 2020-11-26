using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LP
{
    public partial class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponent();
        }

        private void buttonAppariement2Graphes_Click(object sender, EventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            mainwindow.Show();
        }

        private void buttonTestUneMethode_Click(object sender, EventArgs e)
        {
            TestUneMethodeForm testUneMethode = new TestUneMethodeForm();
            testUneMethode.Show();
        }

        private void buttonTestMultiplesMethodes_Click(object sender, EventArgs e)
        {
            Experimentations testmulti = new Experimentations();
            testmulti.Show();
        }
    }
}
