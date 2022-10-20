using System.Net;
using System.Net.Mail;
using System.Text.Json;
using NgrokSharp;
using NgrokSharp.DTO;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public async Task Main(string[] args)
        {
            INgrokManager _ngrokManager;
            _ngrokManager = new NgrokManager();
            
            await _ngrokManager.DownloadAndUnzipNgrokAsync();
            await _ngrokManager.RegisterAuthTokenAsync("X");
        
            _ngrokManager.StartNgrok();

            var tunnel = new StartTunnelDTO
            {
                name = "test",
                proto = "tcp",
                addr = "80"
            };

            var httpResponseMessage = await _ngrokManager.StartTunnelAsync(tunnel);
            if ((int)httpResponseMessage.StatusCode == 201)
            {
                var tunnelDetail = JsonSerializer.Deserialize<TunnelDetailDTO>(
                        await httpResponseMessage.Content.ReadAsStringAsync());
                var tunneladdr = tunnelDetail.PublicUrl;
            }
        }

    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new System.Net.NetworkCredential("X@gmail.com", "X"),
            EnableSsl = true,
        };

        public WebClient client = new WebClient();

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            string ip = client.DownloadString("https://api.ip.sb/ip");
            textBox1.Text = ip; 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Task.Run(() => Main(null));
            textBox2.Text = "there should be ngrok ip here";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string ip = client.DownloadString("https://api.ip.sb/ip");
            smtpClient.Send("X@gmail.com", "X@hotmail.com", "", ip + "there should be ngrok ip as well");  
        }
    }
}