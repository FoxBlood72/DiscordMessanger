using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord.WebSocket;

namespace Discord_DM_Bot
{
    public partial class ServerUserControl : UserControl
    {

        private SocketGuild guild;
        private IReadOnlyCollection<SocketRole> roles;

        public ServerUserControl()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void ServerUserControl_Load(object sender, EventArgs e)
        {

        }
        internal void SetImage(Image img)
        {
            pictureBox1.Image = img;
        }
        internal void SetText(string text)
        {
            checkBox1.Text = text;
        }
        internal void SetGuild(SocketGuild sok)
        {
            guild = sok;
        }
        internal SocketGuild GetGuild()
        {
            return guild;
        }
        internal bool Checkbox()
        {
            return checkBox1.Checked;
        }
        internal string GetName()
        {
            return checkBox1.Text;
        }
        internal void SetRoles(IReadOnlyCollection<SocketRole> rol)
        {
            roles = rol;
            foreach (SocketRole onerole in roles)
                comboBox1.Items.Add(onerole.Name);
            
        }
        internal void SetCheckbox(bool value)
        {
            checkBox1.Checked = value;
        }
        internal string Getcombobox()
        {
            return comboBox1.Text;
        }
        internal void SetComboBox(string name)
        {
            comboBox1.Text = name;
        }
        internal string GetCombobox2()
        {
            return comboBox2.Text;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "New-Members")
            {
                comboBox2.Enabled = true;
            }
            else
            {
                comboBox2.Enabled = false;
            }
        }
    }
}
