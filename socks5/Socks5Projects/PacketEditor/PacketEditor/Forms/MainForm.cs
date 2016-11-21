using Be.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using socks5;
using System.Net;
namespace PacketEditor
{
    public partial class MainForm : Form
    {
        socks5.Socks5Server sock5;
        public MainForm()
        {
            InitializeComponent();
            sock5 = new Socks5Server(IPAddress.Any, 1080);
            sock5.Start();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            hexBox1.ByteProvider = new DynamicByteProvider(Encoding.ASCII.GetBytes("No Packet Loaded"));
            hexBox1.ReadOnly = true;
            Utils.DataAdded += Utils_DataAdded;
        }

        void Utils_DataAdded(object sender, EventArgs e)
        {
            //Need to invoke listview.
            if (this.listView1.InvokeRequired)
            {
                this.listView1.Invoke((MethodInvoker)delegate()
                {
                    ListViewItem x = new ListViewItem();
                    DataCapture.Data data = Utils.CapturedData[Utils.CapturedData.Count - 1];
                    x.Text = (Utils.CapturedData.Count - 1).ToString();
                    x.SubItems.Add(data.Request.Address + ":" + data.Request.Port);
                    x.SubItems.Add(data.DataType.ToString());
                    x.SubItems.Add(data.Buffer.Length.ToString());
                    this.listView1.Items.Add(x);
                });
            }
            else
            {
                ListViewItem x = new ListViewItem();
                DataCapture.Data data = Utils.CapturedData[Utils.CapturedData.Count - 1];
                x.Text = (Utils.CapturedData.Count - 1).ToString();
                x.SubItems.Add(data.Request.Address + ":" + data.Request.Port);
                x.SubItems.Add(data.DataType.ToString());
                x.SubItems.Add(data.Buffer.Length.ToString());
                this.listView1.Items.Add(x);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //grab selected data from list.
            if (listView1.SelectedItems.Count <= 0) return;
            //grab text.
            this.hexBox1.ByteProvider = new DynamicByteProvider(Utils.CapturedData[listView1.SelectedIndices[0]].Buffer);
        }
    }
}
