using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.ObjectModel;
using Discord.Rpc;
using Discord.Webhook;
using Discord.Rest;
using Discord.Net;
using Discord.Net.WebSockets;
using Discord.Net.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Discord_DM_Bot
{
    public partial class Form1 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        delegate void SetUserControl(UserControl usercontrol);
        delegate void SetActivate();
        delegate void SetButtoncallback();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RunBot();
        }

        private void Activate()
        {
            if (this.textBox1.InvokeRequired)
            {
                SetActivate activatecallback = new SetActivate(Activate);
                this.Invoke(activatecallback);
            }
            else
            {
                checkBox1.Enabled = true;
                button1.Enabled = false;
                textBox1.Enabled = true;
                flowLayoutPanel1.Enabled = true;
                textBox1.Enabled = true;
                label1.Enabled = true;
                button2.Enabled = true;
                button1.Enabled = false;
            }
        }

        private void Disablebutton()
        {
            if (button1.InvokeRequired)
            {
                SetButtoncallback callback = new SetButtoncallback(Disablebutton);
                this.Invoke(callback);
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private async Task _client_Connected()
        {
            Activate();
            Console.WriteLine("Connected");
            
            IReadOnlyCollection<SocketGuild> guilds = MainClass._client.Guilds;
            Console.WriteLine("Number of guilds " + guilds.Count);
            foreach (SocketGuild guild in guilds)
            {
                SocketGuild sok = MainClass._client.GetGuild(guild.Id);

                sok.DownloadUsersAsync().Wait(500);


                Console.WriteLine(sok.Name);
                ServerUserControl usercontrol = new ServerUserControl();
                if(!string.IsNullOrWhiteSpace(sok.IconUrl))
                {
                    using (WebClient wc = new WebClient())
                    {
                        byte[] imagebyte =  wc.DownloadData(sok.IconUrl);
                        using (MemoryStream ms = new MemoryStream(imagebyte))
                        {
                            
                            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                            usercontrol.SetImage(img);
                        }
                    }
                }

                usercontrol.SetComboBox("@everyone");
                usercontrol.SetRoles(sok.Roles);
                usercontrol.SetText(sok.Name);
                usercontrol.SetGuild(sok);

                AddToFlowLayout(usercontrol);
                
            }
        }

        private void AddToFlowLayout(UserControl usercontrol)
        {
            if (this.flowLayoutPanel1.InvokeRequired)
            {
                SetUserControl usercontrolcallback = new SetUserControl(AddToFlowLayout);
                this.Invoke(usercontrolcallback, new object[] { usercontrol });
            }
            else
            {
                flowLayoutPanel1.Controls.Add(usercontrol);
            }
        }
        
        private async Task RunBot()
        {
            string Token = textBox3.Text;
            if (string.IsNullOrWhiteSpace(Token))
            {
                MessageBox.Show("Please enter a valid token");
                return;
            }
            Disablebutton();

            MainClass._client = new DiscordSocketClient();
            MainClass._command = new CommandService();

            MainClass._service = new ServiceCollection()
                .AddSingleton(MainClass._client)
                .AddSingleton(MainClass._command)
                .BuildServiceProvider();

            

            

            await RegisterConnected();

            await MainClass._client.LoginAsync(TokenType.Bot,Token);


            await MainClass._client.StartAsync();


            await Task.Delay(-1);
        }

        private async Task RegisterConnected()
        {
            MainClass._client.Connected += _client_Connected;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<ulong> discorduser = new List<ulong>();
            discorduser.Add(447041400995840000);
            foreach (UserControl us in flowLayoutPanel1.Controls)
            {
                ServerUserControl server = (ServerUserControl)us;
                if (server.Checkbox())
                {
                    SocketGuild socket = server.GetGuild();
                    IReadOnlyCollection<SocketGuildUser> users = socket.Users;
                    int i = 0;
                    int count = users.Count;
                    Random rnd = new Random();

                    if (server.Getcombobox() == "HALF-MEMBERS")
                    {
                        SocketGuildUser user = users.ElementAt(rnd.Next(0,count));

                        if (i < count / 2)
                        {

                            if (!discorduser.Contains(user.Id))
                            {
                                discorduser.Add(user.Id);
                                user.SendMessageAsync(textBox1.Text);
                                i++;
                            }
                        }
                        
                    }
                    else
                    {


                        foreach (SocketGuildUser user in users)
                        {
                            if (server.Getcombobox() == "New-Members")
                            {
                                DateTimeOffset usertime = (DateTimeOffset)user.JoinedAt;
                                DateTimeOffset currentime = DateTime.Now;
                                double differencedays = (currentime.DateTime - usertime.DateTime).TotalDays;
                                double timeselected = Convert.ToDouble(server.GetCombobox2());


                                if (!discorduser.Contains(user.Id))
                                {
                                    if (differencedays < timeselected)
                                    {
                                        discorduser.Add(user.Id);
                                        user.SendMessageAsync(textBox1.Text);
                                    }
                                }
                            }
                            else
                            {
                                if (!discorduser.Contains(user.Id) && HaveRole(user, server))
                                {
                                    Console.WriteLine(user.Id + " " + user.JoinedAt);
                                    discorduser.Add(user.Id);
                                    user.SendMessageAsync(textBox1.Text);
                                }
                            }
                        }
                        
                    }
                }
                textBox1.Text = "";
            }
        }

        private bool HaveRole(SocketGuildUser user, ServerUserControl usercontrol)
        {
            if (usercontrol.Getcombobox() == "@everyone" || string.IsNullOrWhiteSpace(usercontrol.Getcombobox()))
            {
                return true;
            }
            else
            {
                bool ok = false;
                foreach (SocketRole role in user.Roles)
                {
                    if (usercontrol.Getcombobox() == role.Name)
                        ok = true;
                }

                return ok;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Process.Start("https://discordapp.com/oauth2/authorize?client_id=447041400995840000&scope=bot");
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                foreach (ServerUserControl control in flowLayoutPanel1.Controls)
                    control.SetCheckbox(true);
            }
            else
            {
                foreach (ServerUserControl control in flowLayoutPanel1.Controls)
                    control.SetCheckbox(false);
            }
        }
    }
}
