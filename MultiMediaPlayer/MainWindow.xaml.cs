﻿using Microsoft.Win32;
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
using System.Windows.Threading;

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
        DispatcherTimer _timer;


        public MainWindow()
        {
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += timer_Tick;
            _timer.Start();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           PlayList.ItemsSource = _fullPaths;
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            if (_player.Source != null)
            {
                
                var currentPos = _player.Position.ToString(@"mm\:ss");
                var duration = _player.NaturalDuration.TimeSpan.ToString(@"mm\:ss");

                Title = String.Format($"{currentPos} / {duration}");
                CurrentTime.Text = currentPos;
                DurationTime.Text = duration;
            }
            else
                Title = "No file selected...";
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
            op.Multiselect = true;

            if (op.ShowDialog() == true)
            {
                String[] fileNames;
                fileNames = op.FileNames;

                //Add to array to display into listview
                for (int i = 0; i < fileNames.Length; i++)
                {
                    var info = new FileInfo(fileNames[i]);
                    _fullPaths.Add(info);
                }
                
                MessageBox.Show($"Load successful! ....\n");
                _player.Open(new Uri(fileNames[0], UriKind.Absolute));

                
               
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
