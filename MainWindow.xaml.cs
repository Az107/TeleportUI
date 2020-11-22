using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Teleport;

namespace Teleport.gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Teleport.Server server;
        private bool Sending = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClientConnected(string ip)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {

                ClientList.Items.Add(ip);
            }));
        }

        private void ClientDisconnected(string ip)
        {
            ClientList.Items.Remove(ip);
        }



        private void StartFileSend(string filePath)
        {
            Sending = true;
            icon.Visibility = Visibility.Collapsed;
            if (server != null) server.Stop();
            ListTitle.Text = "Clients Connected";
            ClientList.Items.Clear();
            server = new Server(filePath);
            server.ClientConnectedEvent += ClientConnected;
            FileName.Text = server.FileName;
            using (System.Drawing.Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(server.filePath))
            {
                FileIcon.Source = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            progressRing.Visibility = Visibility.Visible;
            server.Start(true);
        }

        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                StartFileSend(files[0]);


            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (server != null) server.Stop();

        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                StartFileSend(dialog.FileName);
            }
        }
    }
}
