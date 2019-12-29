﻿using Gma.System.MouseKeyHook;
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
using System.Windows.Forms;
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
        PlayList _playList = new PlayList();
        int _currentPosition=0;
        public bool _isPlaying = false;
        BindingList<FileInfo> _fullPaths = new BindingList<FileInfo>();
        DispatcherTimer _timer;
        public static bool isDraggingSlider = false;
        private IKeyboardMouseEvents _hook;

        public void Subscribe()
        {
            _hook = Hook.GlobalEvents();
            _hook.KeyUp += _hook_KeyUp;
        }

        public void Unsubscribe()
        {
            _hook.KeyUp -= _hook_KeyUp;
            _hook.Dispose();
        }

        private void _hook_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control && e.Shift && (e.KeyCode == Keys.E))
            {
                PlayButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, PlayButton));
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += timer_Tick;
            _timer.Start();

            Subscribe();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           PlayList.ItemsSource = _fullPaths;
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            if (_player.Source != null)
            {
               
                if (_player.NaturalDuration.HasTimeSpan == true && !isDraggingSlider)
                {
                    var currentPos = _player.Position.ToString(@"mm\:ss");
                    var duration = _player.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
                    Title = String.Format($"{currentPos} / {duration}");
                    CurrentTime.Text = currentPos;
                    DurationTime.Text = duration;

                    sliderDuration.Minimum = 0;
                    sliderDuration.Maximum = _player.NaturalDuration.TimeSpan.TotalSeconds;
                    sliderDuration.Value = _player.Position.TotalSeconds;

                }

            }
            else
                Title = "No file selected...";
        }

        private void PlayPosition(int position)
        {
            if (position > _playList.TotalMedia || position < 0) return;
            _currentPosition = position;
            _player.Open(new Uri(_playList.MediaList[position], UriKind.Absolute));
            _player.Play();
            _isPlaying = true;
        }

        private void PlayList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddMusicButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All Media Files|*.wav;*.aac;*.wma;*.wmv;*.avi;*.mpg;*.mpeg;" +
                "*.m1v;*.mp2;*.mp3;*.mpa;*.mpe;*.m3u;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.cda;*.aif;*.aifc;*.aiff;*.mid;*.midi;" +
                "*.rmi;*.mkv;*.WAV;*.AAC;*.WMA;*.WMV;*.AVI;*.MPG;*.MPEG;*.M1V;*.MP2;*.MP3;*.MPA;*.MPE;*.M3U;*.MP4;*.MOV;*.3G2;*.3GP2;*.3GP;" +
                "*.3GPP;*.M4A;*.CDA;*.AIF;*.AIFC;*.AIFF;*.MID;*.MIDI;*.RMI;*.MKV";
            op.Multiselect = true;

            if (op.ShowDialog() == true)
            {
                _playList.MediaList = op.FileNames;

                //Add to array to display into listview
                for (int i = 0; i < _playList.MediaList.Length; i++)
                {
                    var info = new FileInfo(_playList.MediaList[i]);
                    _fullPaths.Add(info);
                }

               // System.Windows.MessageBox.Show($"Load successful! ....\n");
                PlayPosition(_currentPosition);
                return;
            }
            else
            {
                System.Windows.MessageBox.Show("Load failed!\n");
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
            PlayPosition(_currentPosition-1);
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
            PlayPosition(_currentPosition + 1);
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



        private void slider_ValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CurrentTime.Text = TimeSpan.FromSeconds(sliderDuration.Value).ToString(@"mm\:ss");
            _player.Position = TimeSpan.FromSeconds(sliderDuration.Value);
        }

        private void sliderProcess_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDraggingSlider = true;
        }

        private void sliderProcess_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDraggingSlider = false;
            _player.Position = TimeSpan.FromSeconds(sliderDuration.Value);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _hook.KeyUp -= _hook_KeyUp;
            Unsubscribe();

        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
