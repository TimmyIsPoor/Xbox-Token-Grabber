using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AM_Token_Grabber
{
    public partial class Form1 : Form
    {
        string String;

        async void Void()
        {
            Stopwatch Stopwatch = new Stopwatch();
            Stopwatch.Start();
            for (; ; )
            {
                if (Stopwatch.Elapsed.Seconds >= 10)
                {
                    Process.Start(Environment.CurrentDirectory + "/Xbox Token Grabber.exe");
                    Close();
                }
                await Task.Delay(1000);
            }
        }

        private async void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.Url.ToString().Contains("oauth20_logout.srf"))
            {
                string[] String1 = String.Split(':');
                webBrowser1.Navigate("https://login.live.com/oauth20_authorize.srf?display=touch&scope=service%3A%3Auser.auth.xboxlive.com%3A%3AMBI_SSL&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf&locale=en&response_type=token&client_id=0000000048093EE3&username=" + String1[0]);
            }
            else if (webBrowser1.Url.ToString().Contains("@"))
            {
                bool Bool = false;
                while (!Bool)
                {
                    try
                    {
                        foreach (HtmlElement HtmlElement in webBrowser1.Document.GetElementsByTagName("div"))
                        {
                            if (HtmlElement.GetAttribute("id") == "loginHeader")
                            {
                                if (HtmlElement.InnerText == "Enter password")
                                {
                                    Bool = true;
                                    string[] String1 = String.Split(':');
                                    webBrowser1.Document.GetElementById("i0118").InnerText = String1[1];
                                    await Task.Delay(500);
                                    webBrowser1.Document.GetElementById("idSIButton9").InvokeMember("click");
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                    await Task.Delay(500);
                }
            }
            else if (webBrowser1.Url.ToString().Contains("access_token"))
            {
                try
                {
                    string[] String1 = webBrowser1.Url.ToString().Split(new string[] { "access_token=" }, StringSplitOptions.None);
                    string String2 = String1[1];
                    string[] String3 = String2.Split('&');
                    WebClient WebClient = new WebClient();
                    WebClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string String4 = await WebClient.UploadStringTaskAsync("https://user.auth.xboxlive.com/user/authenticate", "post", "{\"RelyingParty\":\"http://auth.xboxlive.com\",\"TokenType\":\"JWT\",\"Properties\":{\"AuthMethod\":\"RPS\",\"SiteName\":\"user.auth.xboxlive.com\",\"RpsTicket\":\"" + String3[0] + "\"}}");
                    string[] String5 = String4.Split(new string[] { "Token\":\"" }, StringSplitOptions.None);
                    string String6 = String5[1];
                    string[] String7 = String6.Split('"');
                    StreamWriter StreamWriter = new StreamWriter(Environment.CurrentDirectory + "/Tokens.txt", true);
                    StreamWriter.WriteLine(String7[0]);
                    StreamWriter.Close();
                    if (File.ReadAllLines(Environment.CurrentDirectory + "/Tokens.txt").Count() != File.ReadAllLines(Environment.CurrentDirectory + "/Accounts.txt").Count())
                    {
                        Process.Start(Environment.CurrentDirectory + "/Xbox Token Grabber.exe");
                    }
                    Close();
                }
                catch
                {

                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                String = File.ReadLines(Environment.CurrentDirectory + "/Accounts.txt").Skip(File.ReadAllLines(Environment.CurrentDirectory + "/Tokens.txt").Count()).FirstOrDefault();
                int Integer = File.ReadAllLines(Environment.CurrentDirectory + "/Tokens.txt").Count();
                Integer++;
                Text = "Xbox Token Grabber | Account : " + Integer + "/" + File.ReadAllLines(Environment.CurrentDirectory + "/Accounts.txt").Count() + "";
                Void();
                webBrowser1.Navigate("https://login.live.com/oauth20_logout.srf");
            }
            catch
            {
                Close();
            }
        }

        public Form1()
        {
            InitializeComponent();
            ServicePointManager.CheckCertificateRevocationList = false;
            ServicePointManager.DefaultConnectionLimit = 1000;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
        }
    }
}