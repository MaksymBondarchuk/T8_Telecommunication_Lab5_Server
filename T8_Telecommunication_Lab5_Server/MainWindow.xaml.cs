using System;
using System.Collections.Generic;
using System.Windows.Controls;
using NBLib;

namespace T8_Telecommunication_Lab5_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public const int MatrixSize = 10;
        public Server Server = new Server();
        public int PreviousCompletedRowsCount;
        public int PreviousClientsCount;

        private readonly System.Windows.Threading.DispatcherTimer _timer =
            new System.Windows.Threading.DispatcherTimer();

        private readonly Random _rand = new Random();

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
            lock (Server.Data)
            {
                if (PreviousClientsCount != Server.Data.Clients.Count)
                {
                    TextBoxClients.Clear();
                    foreach (var client in Server.Data.Clients)
                        TextBoxClients.Text += $"{client.Name}\n";
                    PreviousClientsCount = Server.Data.Clients.Count;
                }

                if (PreviousCompletedRowsCount != Server.Data.CompletedRows.Count)
                {
                    PrintMatrix();
                    PreviousCompletedRowsCount = Server.Data.CompletedRows.Count;
                }
            }
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

        private void PrintMatrix()
        {
            lock (Server.Data)
            {
                TextBoxMatrix.Clear();
                foreach (var t in Server.Data.Matrix)
                {
                    foreach (var t1 in t)
                        TextBoxMatrix.Text += $"{t1,4}";
                    TextBoxMatrix.Text += "\n";
                }
            }
        }

        private void ButtonGenerate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ButtonStartServer.IsEnabled = true;
            lock (Server.Data)
            {
                Server.Data.Matrix.Clear();
                for (var i = 0; i < MatrixSize; i++)
                {
                    Server.Data.Matrix.Add(new List<byte>(MatrixSize));
                    for (var j = 0; j < MatrixSize; j++)
                        Server.Data.Matrix[i].Add(Convert.ToByte(_rand.Next(256)));
                }

                Server.Data.CompletedRows.Clear();
                for (var i = 0; i < MatrixSize; i++)
                    Server.Data.FreeRows.Add(i);

                PrintMatrix();
            }
        }
    }
}
