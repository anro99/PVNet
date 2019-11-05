using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVDataSampler
{
    public partial class MainForm : Form
    {
        private DataSampler m_dataSampler;

        public MainForm()
        {
            InitializeComponent();

            m_dataSampler = new DataSampler();

        }

        private void mnuSettings_Click(object sender, EventArgs e)
        {
            var settings = new SettingsForm();

            if (DialogResult.OK == settings.ShowDialog(this))
            {

            }
        }

        private void m_btnStart_Click(object sender, EventArgs e)
        {
            m_dataSampler.Start();
        }

        private void m_btnStop_Click(object sender, EventArgs e)
        {
            m_dataSampler.Stop();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_dataSampler.Stop();
        }
    }
}
