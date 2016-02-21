using NBLib;

namespace T8_Telecommunication_Lab5_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public Server Server = new Server();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonStartServer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ConsoleManager.Show();

            await Server.Run();
        }
    }
}
