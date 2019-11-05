using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVDataSampler
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            List<string> serialPorts = new List<string>(SerialHelper.GetSerialPorts());
            if (!string.IsNullOrEmpty(AppSettings.Instance.GridMeter) && !serialPorts.Contains(AppSettings.Instance.GridMeter))
                serialPorts.Add(AppSettings.Instance.GridMeter);
            m_cbGridCounter.Items.Clear();
            serialPorts.ForEach((a_port) => m_cbGridCounter.Items.Add(a_port));
            if (!string.IsNullOrEmpty(AppSettings.Instance.GridMeter))
                m_cbGridCounter.SelectedItem = AppSettings.Instance.GridMeter;
        }

        private void m_btOK_Click(object sender, EventArgs e)
        {
            var gridCounterPort = m_cbGridCounter.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(gridCounterPort))
            {
                AppSettings.Instance.GridMeter = gridCounterPort;
                AppSettings.Instance.Save();
            }

            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
