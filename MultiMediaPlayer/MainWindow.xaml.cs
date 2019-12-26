using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiMediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MediaPlayer _player = new MediaPlayer();
        public bool _isPlaying = false;
        BindingList<FileInfo> _fullPaths = new BindingList<FileInfo>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           PlayList.ItemsSource = _fullPaths;
        }

        private void PlayList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddMusicButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All Media Files|*.wav;*.aac;*.wma;*.wmv;*.avi;*.mpg;*.mpeg;" +
                "*.m1v;*.mp2;*.mp3;*.mpa;*.mpe;*.m3u;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.cda;*.aif;*.aifc;*.aiff;*.mid;*.midi;" +
                "*.rmi;*.mkv;*.WAV;*.AAC;*.WMA;*.WMV;*.AVI;*.MPG;*.MPEG;*.M1V;*.MP2;*.MP3;*.MPA;*.MPE;*.M3U;*.MP4;*.MOV;*.3G2;*.3GP2;*.3GP;" +
                "*.3GPP;*.M4A;*.CDA;*.AIF;*.AIFC;*.AIFF;*.MID;*.MIDI;*.RMI;*.MKV";

            if (op.ShowDialog() == true)
            {
                String fileName;
                fileName = op.FileName; 
                MessageBox.Show($"Load successful! ....{fileName}\n");
                _player.Open(new Uri(fileName, UriKind.Absolute));

                //Add to array to display into listview
                var info = new FileInfo(op.FileName);
                _fullPaths.Add(info);
                return;
            }
            else
            {
                MessageBox.Show("Load failed!\n");
                return;
            }
        }

       

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying == false)
            {
                _player.Play();
            }
            else {
                _player.Pause();
            }

            _isPlaying = !_isPlaying;
               

        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ModeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {     
                _player.Stop();
                _isPlaying = false;
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {

        }


        private void LoadPlayListButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SavePlayListButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewPlayListButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
