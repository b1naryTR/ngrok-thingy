using System.Net;
using System.Net.Mail;
using System.Text.Json;
using NgrokSharp;
using NgrokSharp.DTO;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        INgrokManager _ngrokManager;
        TunnelDetailDTO _tunnelDetailDTO;
        private IPAddress _ngrokIp;
        WebClient _client = new WebClient();

        public Form1()
        {
            InitializeComponent();
        }

        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new System.Net.NetworkCredential("X@gmail.com", "X"),
            EnableSsl = true,
        };

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            string ip = _client.DownloadString("https://api.ip.sb/ip");
            textBox1.Text = ip;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            _ngrokManager = new NgrokManager();

            await _ngrokManager.DownloadAndUnzipNgrokAsync();

            _ngrokManager.StartNgrok();

            var tunnel = new StartTunnelDTO
            {
                name = "reverse proxy",
                proto = "http",
                addr = "8080"
            };

            var httpResponseMessage = await _ngrokManager.StartTunnelAsync(tunnel);
            if ((int)httpResponseMessage.StatusCode == 201)
            {
                _tunnelDetailDTO =
                    JsonSerializer.Deserialize<TunnelDetailDTO>(await httpResponseMessage.Content.ReadAsStringAsync());
            }
            Uri myUri = new Uri(_tunnelDetailDTO.PublicUrl.ToString());
            _ngrokIp = Dns.GetHostAddresses(myUri.Host)[0];
            textBox2.Text = _ngrokIp.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string ip = _client.DownloadString("https://api.ip.sb/ip");
            smtpClient.Send("X@gmail.com", "X@hotmail.com", "", $"ip: {_ngrokIp} and  url: {_tunnelDetailDTO.PublicUrl}");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _ngrokManager.StopNgrok();
        }
    }
}