using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

// For button click functions call
namespace System.Windows.Controls
{
    /// <summary>
    /// For allow perform button click 
    /// </summary>
    public static class MyExt
    {
        public static void PerformClick(this Button btn)
        {
            btn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
    }
}

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
        public bool MessageCompleteIsShown;

        private readonly DispatcherTimer _timer =
            new DispatcherTimer();

        private readonly Random _rand = new Random();

        public MainWindow()
        {
            InitializeComponent();

            _timer.Tick += Tick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
        }

        private async void ButtonStartServer_Click(object sender, RoutedEventArgs e)
        {
            ButtonStartServer.IsEnabled = false;
            //ConsoleManager.Show();
            TextBoxClients.Text += "Server started\n";
            _timer.Start();
            await Server.Run();
            ButtonStartServer.IsEnabled = false;
        }

        private void Tick(object sender, EventArgs e)
        {
            lock (Server.Data)
            {
                //if (PreviousClientsCount != Server.Data.Clients.Count)
                //{
                //    TextBoxClients.Clear();
                //    foreach (var client in Server.Data.Clients)
                //        TextBoxClients.Text += $"{client.Name}\n";
                //    PreviousClientsCount = Server.Data.Clients.Count;
                //}

                if (Server.Data.Log.Count != 0)
                {
                    foreach (var message in Server.Data.Log)
                        TextBoxClients.Text += $"{message}\n";
                    Server.Data.Log.Clear();
                }

                if (PreviousCompletedRowsCount != Server.Data.CompletedRows.Count)
                {
                    PrintMatrix();
                    PreviousCompletedRowsCount = Server.Data.CompletedRows.Count;
                }

                if (Server.Data.FreeRows.Count == 0 && !MessageCompleteIsShown)
                {
                    MessageCompleteIsShown = true;
                    var res = MessageBox.Show("Matrix is sorted. Do you want to generate a new one?",
                        "All work are done",
                        MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        ButtonGenerate.PerformClick();
                        MessageCompleteIsShown = false;
                    }
                }
            }
        }

        private void PrintMatrix()
        {
            lock (Server.Data)
            {
                TextBoxMatrix.Clear();
                for (var i = 0; i < Server.Data.Matrix.Count; i++)
                {
                    foreach (var t1 in Server.Data.Matrix[i])
                        TextBoxMatrix.Text += $"{t1,4}";

                    if (Server.Data.CompletedRows.Contains(i))
                        TextBoxMatrix.Text += "    ✓\n";
                    else
                        TextBoxMatrix.Text += "    ✗\n";
                }
            }
        }

        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            MessageCompleteIsShown = false;
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
