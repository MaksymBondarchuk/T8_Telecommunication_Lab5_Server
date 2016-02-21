using System;
using System.Windows.Controls;
using NBLib;

namespace T8_Telecommunication_Lab5_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public Server Server = new Server();
        private readonly System.Windows.Threading.DispatcherTimer _timer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            _timer.Tick += Tick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
        }

        private async void ButtonStartServer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ConsoleManager.Show();

            _timer.Start();
            await Server.Run();
            
        }

        private void Tick(object sender, EventArgs e)
        {
            TextBoxClients.Clear();
            lock (Server.Data)
            {
                foreach (var client in Server.Data.Clients)
                    TextBoxClients.Text += $"{client.Name}\n";
                //ComboBoxClient.Items.Add(new ComboBoxItem { Content = client.Name });
            }
            //TextBoxClients.Text = Server.Data.Clients.Count.ToString();
            //TextBoxClients.Text = "1";
            //Console.WriteLine(@"Timer works");
        }

        private void ComboBoxClient_DropDownOpened(object sender, EventArgs e)
        {
            ComboBoxClient.Items.Clear();
            lock (Server.Data)
            {
                foreach (var client in Server.Data.Clients)
                    ComboBoxClient.Items.Add(new ComboBoxItem { Content = client.Name });
            }
        }
    }
}
