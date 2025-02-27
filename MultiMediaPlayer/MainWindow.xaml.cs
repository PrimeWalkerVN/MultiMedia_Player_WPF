﻿using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using XamlAnimatedGif;

namespace MultiMediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Define local

        String defaultFileName = "saved_play_list.txt";
        MediaPlayer _player = new MediaPlayer();
        PlayList _playList = new PlayList();
        int _currentPosition=0;
        int _shufflePos = 0;
        double _currentVolumeValue = 75.0;

        List<int> _mediaPositionList = new List<int>();
        List<int> _shufflePositionList = new List<int>();


        public bool _isPlaying = false;
        public bool _isShuffle = false;
        public bool _isLoopOne = false;
        public bool _isMute = false;

        private Uri _playUri = new Uri(@"drawables/play.png", UriKind.Relative);
        private Uri _pauseUri = new Uri(@"drawables/pause.png", UriKind.Relative);

        private Uri _loopUri = new Uri(@"drawables/loop.png", UriKind.Relative);
        private Uri _loopOneUri = new Uri(@"drawables/loop_1.png", UriKind.Relative);

        private Uri _shuffleUri = new Uri(@"drawables/shuffle.png", UriKind.Relative);
        private Uri _disShuffleUri = new Uri(@"drawables/shuffle_disable.png", UriKind.Relative);

        private Uri _volumeUri = new Uri(@"drawables/volume.png", UriKind.Relative);
        private Uri _muteUri = new Uri(@"drawables/mute.png", UriKind.Relative);



        BindingList<FileInfo> _fullPaths = new BindingList<FileInfo>();
        DispatcherTimer _timer;
        public static bool isDraggingSlider = false;
        private IKeyboardMouseEvents _hook;
        #endregion

        #region Key hook
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
            if (e.Control && e.Shift && (e.KeyCode == Keys.P))
            {
                PlayButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, PlayButton));
            }
            if (e.Control && e.Shift && (e.KeyCode == Keys.S))
            {
                StopButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, StopButton));
            }
            if (e.Control && e.Shift && (e.KeyCode == Keys.R))
            {
                ForwardButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, ForwardButton));
            }
            if (e.Control && e.Shift && (e.KeyCode == Keys.E))
            {
                PreviousButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, PreviousButton));
            }
            if (e.Control && e.Shift && e.KeyCode == Keys.J)
            {
                VolumeSlider.Value -= 10;
            }
            if (e.Control && e.Shift && e.KeyCode == Keys.K)
            {
                VolumeSlider.Value += 10;
            }
            if (e.Control && e.Shift && e.KeyCode == Keys.M)
            {
                VolumeModeButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, VolumeModeButton));
            }
        }

        #endregion

        #region Setup timer
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
            LoadPlayList(defaultFileName);
            _isPlaying = false;
            setImagePlay(_playUri);
            ((Storyboard)Resources["Storyboard"]).Begin();
            ((Storyboard)Resources["Storyboard"]).Pause();
            _player.Stop();
            
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

                    if (currentPos.Equals(duration)) {
                        
                        if (_isLoopOne == true)
                        {

                            _timer.Stop();
                            PlayPosition(_currentPosition);
                            _timer.Start();

                        }
                        else
                            ForwardButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, ForwardButton));
                    }
                }

            }
            else
                Title = "No file selected...";
        }
        #endregion

        #region Method

        private void PauseMyGif() {
            var controller = AnimationBehavior.GetAnimator(myGif);
            controller.Pause();
        }
        private void RepeatMyGif()
        {
            Uri abc = new Uri(@"images/hangfel.gif",UriKind.Relative);
            AnimationBehavior.SetSourceUri(myGif, abc);
            var controller = AnimationBehavior.GetAnimator(myGif);
            controller.Play();
        }
        private void PlayPosition(int position)
        {
            _player.Position = TimeSpan.FromSeconds(0);
            sliderDuration.Value = 0;
            if (_playList.MediaList == null) return;
            _currentPosition = position;
            if (position > _playList.TotalMedia - 1)
                _currentPosition = 0;
            if (position < 0)
                _currentPosition = _playList.TotalMedia - 1;

            if (!File.Exists(_playList.MediaList[_currentPosition])) {
                _player.Stop();
                System.Windows.MessageBox.Show($"{_playList.MediaList[_currentPosition]}.....not exists");
            }

            _player.Open(new Uri(_playList.MediaList[_currentPosition], UriKind.Absolute));
            _player.Play();
            _player.Volume = _currentVolumeValue;
            VolumeSlider.Value = _currentVolumeValue;
            _isPlaying = true;
            PlayList.SelectedIndex = _currentPosition;
            PlayList.ScrollIntoView(PlayList.SelectedItem);
            setImagePlay(_pauseUri);
            ((Storyboard)Resources["Storyboard"]).Resume();

        }

        private void PlayShuffle(int position)
        {
            _player.Position = TimeSpan.FromSeconds(0);
            sliderDuration.Value = 0;
            _currentPosition = position;
            if (position > _playList.TotalMedia - 1)
                _currentPosition = 0;
            if (position < 0)
                _currentPosition = _playList.TotalMedia - 1;
            if (!File.Exists(_playList.MediaList[_currentPosition]))
            {
                _player.Stop();
                System.Windows.MessageBox.Show($"{_playList.MediaList[_currentPosition]}.....not exists");
            }
            _player.Open(new Uri(_playList.MediaList[_currentPosition], UriKind.Absolute));
            _player.Play();
            _isPlaying = true;
            PlayList.SelectedIndex = _currentPosition;
            setImagePlay(_pauseUri);
            
        }



        private void SavePlayList(String fileName)
        {

            if (_playList.MediaList == null) return;

            var writer = new StreamWriter(fileName);

            writer.WriteLine(_playList.TotalMedia);
            writer.WriteLine(_currentPosition);

            for (int i = 0; i < _playList.TotalMedia; i++)
            {
                writer.WriteLine(_playList.MediaList[i]);
            }

            writer.Close();

            //System.Windows.MessageBox.Show("PlayList is saved");

        }

        private void LoadPlayList(String fileName)
        {
            
            if (!File.Exists(fileName)) return;

            StreamReader reader = new StreamReader(fileName);
            var firstLine = reader.ReadLine();
            int total = 0;
            bool isNum = int.TryParse(firstLine, out total);
            if (!isNum)
            {
                System.Windows.MessageBox.Show("Load failed!\n");
                return;
            };

            if (total <= 0)
            {
                System.Windows.MessageBox.Show("Load failed!\n");
                return;
            }
            var second = reader.ReadLine();
            int current = 0;
            isNum = int.TryParse(second, out current);
            if (!isNum)
            {
                System.Windows.MessageBox.Show("Load failed!\n");
                return;
            };

            ResetData();
            _currentPosition = current;
            String[] fileNames = new String[total];

            for (int i = 0; i < total; i++)
            {
                fileNames[i] = reader.ReadLine();
                _mediaPositionList.Add(i);
            }

            _playList.MediaList = fileNames;

            ShuffleModeFunc();

            //Add to array to display into listview
            for (int i = 0; i < _playList.MediaList.Length; i++)
            {
                var info = new FileInfo(_playList.MediaList[i]);
                _fullPaths.Add(info);
            }

            PlayList.ItemsSource = _fullPaths;

            if (_playList.MediaList != null)
                PlayPosition(_currentPosition);
            reader.Close();
          
        }

        private void ResetData()
        {
            _fullPaths.Clear();
            PlayList.ItemsSource = null;
            _playList.MediaList = null;
            _mediaPositionList.Clear();
            _shufflePositionList.Clear();
            _currentPosition = 0;
            _isPlaying = false;
            _isShuffle = false;
            _isLoopOne = false;

        }


        private void SetImageShuffle(Uri uri)
        {
            BitmapImage image = null;
            image = new BitmapImage(uri);
            ShuffleModeImage.Source = image;
        }

        private void SetImageLoop(Uri uri)
        {
            BitmapImage image = null;
            image = new BitmapImage(uri);
            LoopModeImage.Source = image;
        }

        private void SetImageVolume(Uri uri)
        {
            BitmapImage image = null;
            image = new BitmapImage(uri);
            VolumeModeImage.Source = image;
        }

        private void setImagePlay(Uri uri)
        {
            BitmapImage image = null;
            image = new BitmapImage(uri);
            PlayButtonImage.Source = image;
        }

        private void ShuffleModeFunc()
        {
            _shufflePositionList.Clear();
            Random random = new Random();
            List<int> temp = new List<int>(_mediaPositionList);
            while (temp.Count > 0)
            {

                var num = random.Next(temp.Count);
                _shufflePositionList.Add(temp[num]);
                temp.Remove(temp[num]);
            }

        }

        #endregion

        #region Event
        private void PlayList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddMusicButton_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            setImagePlay(_playUri);
            //_isPlaying = false;
            op.Title = "Select a picture";
            op.Filter = "All Media Files|*.wav;*.aac;*.wma;*.wmv;*.avi;*.mpg;*.mpeg;" +
                "*.m1v;*.mp2;*.mp3;*.mpa;*.mpe;*.m3u;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.cda;*.aif;*.aifc;*.aiff;*.mid;*.midi;" +
                "*.rmi;*.mkv;*.WAV;*.AAC;*.WMA;*.WMV;*.AVI;*.MPG;*.MPEG;*.M1V;*.MP2;*.MP3;*.MPA;*.MPE;*.M3U;*.MP4;*.MOV;*.3G2;*.3GP2;*.3GP;" +
                "*.3GPP;*.M4A;*.CDA;*.AIF;*.AIFC;*.AIFF;*.MID;*.MIDI;*.RMI;*.MKV";
            op.Multiselect = true;

            if (op.ShowDialog() == true)
            {
                if (_playList.MediaList != null) {
                    List<string> myCollection = new List<string>();
                    string[] filenames = op.FileNames;

                    for (int i = 0; i < _playList.TotalMedia; i++)
                    {
                        myCollection.Add(_playList.MediaList[i]);
                    }

                    for (int i = 0; i < filenames.Length; i++)
                    {
                        myCollection.Add(filenames[i]);
                    }

                    _playList.MediaList = myCollection.ToArray();

                }
                else 
                    _playList.MediaList = op.FileNames;

                _mediaPositionList.Clear();
                for (int i = 0; i < _playList.TotalMedia; i++)
                {
                    _mediaPositionList.Add(i);
                }
                ShuffleModeFunc();
                _fullPaths.Clear();
                //Add to array to display into listview
                for (int i = 0; i < _playList.MediaList.Length; i++)
                {
                    var info = new FileInfo(_playList.MediaList[i]);
                    _fullPaths.Add(info);
                }

                PlayList.ItemsSource = _fullPaths;
                if (_isPlaying == false)PlayPosition(_currentPosition);
                ((Storyboard)Resources["Storyboard"]).Resume();
                RepeatMyGif();
                PlayList.SelectedIndex = _currentPosition;
                PlayList.ScrollIntoView(PlayList.SelectedItem);
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
            if (_playList.MediaList == null) _isPlaying = true;
            if (_isPlaying == false)
            {
                _player.Play();
                ((Storyboard)Resources["Storyboard"]).Resume();
                RepeatMyGif();
                setImagePlay(_pauseUri);
            }
            else {
                _player.Pause();
                PauseMyGif();
                setImagePlay(_playUri);
                ((Storyboard)Resources["Storyboard"]).Pause();

            }
           
            _isPlaying = !_isPlaying;

        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            RepeatMyGif();
            _timer.Stop();
            if (_isShuffle == true)
            {
                PlayShuffle(_shufflePositionList[_mediaPositionList[_shufflePositionList[_shufflePos]]]);
                ++_shufflePos;
                if (_shufflePos > _playList.TotalMedia - 1)
                    _shufflePos = 0;
                if (_shufflePos < 0)
                    _shufflePos = _playList.TotalMedia - 1;
            }
            else
            {
                PlayPosition(_currentPosition - 1);
            }
            _timer.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _player.Stop();
            setImagePlay(_playUri);
            if(_isPlaying==true)PauseMyGif();
            ((Storyboard)Resources["Storyboard"]).Begin();
            ((Storyboard)Resources["Storyboard"]).Pause();
            _isPlaying = false;
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            RepeatMyGif();
            _timer.Stop();
            if (_isShuffle == true)
            {
                PlayShuffle(_shufflePositionList[_mediaPositionList[_shufflePositionList[_shufflePos]]]);
                --_shufflePos;
                if (_shufflePos > _playList.TotalMedia - 1)
                    _shufflePos = 0;
                if (_shufflePos < 0)
                    _shufflePos = _playList.TotalMedia - 1;
            }
            else {
                PlayPosition(_currentPosition + 1);
            }
            _timer.Start();

        }


        private void LoadPlayListButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new Microsoft.Win32.OpenFileDialog();
            screen.DefaultExt = "txt";
            screen.Filter = "Playlist Files|*.txt";
            if (screen.ShowDialog() == true)
            {
                LoadPlayList(screen.FileName);
            }
            else {
                System.Windows.MessageBox.Show("Load failed!\n");
            }

        }

        private void SavePlayListButton_Click(object sender, RoutedEventArgs e)
        {
            if (_playList.MediaList == null) return;
            Microsoft.Win32.SaveFileDialog saveFile = new Microsoft.Win32.SaveFileDialog();

            saveFile.DefaultExt = "txt";
            saveFile.Filter = "Playlist Files|*.txt";
            if (saveFile.ShowDialog() == true)
            {
                SavePlayList(saveFile.FileName);
                return;
            }
        }



        private void NewPlayListButton_Click(object sender, RoutedEventArgs e)
        {
            if (_playList.MediaList == null) return;
            _player.Stop();
            setImagePlay(_playUri);
            PauseMyGif();
            ((Storyboard)Resources["Storyboard"]).Pause();

            ResetData();
           
        }


        private void slider_ValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CurrentTime.Text = TimeSpan.FromSeconds(sliderDuration.Value).ToString(@"mm\:ss");
            if(isDraggingSlider==false) _player.Position = TimeSpan.FromSeconds(sliderDuration.Value);
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
            SavePlayList(defaultFileName);
            

        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
        }
        private void PlayList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int position = PlayList.SelectedIndex;
            PlayPosition(position);
            RepeatMyGif();
        }

        private void LoopModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoopOne == false)
            {
                SetImageLoop(_loopOneUri);
            }
            else
            {
                SetImageLoop(_loopUri);
            }

            _isLoopOne = !_isLoopOne;

        }

        private void ModeShuffle_Click(object sender, RoutedEventArgs e)
        {
           
            if (_isShuffle == false)
            {
                SetImageShuffle(_shuffleUri);
                ShuffleModeFunc();
            }
            else
            {
                SetImageShuffle(_disShuffleUri);

            }

            _isShuffle = !_isShuffle;
        }

        private void OnRemoveMedia_Click(object sender, RoutedEventArgs e)
        {
            int temp = _currentPosition;
            var selectedItem = PlayList.SelectedItems;
            List<int> t = new List<int>();
            int a = 0;
            while(PlayList.SelectedItems.Count > 0)
            {
                Object b = PlayList.SelectedItems[a];
                a++;
                if (PlayList.Items.IndexOf(b) != _currentPosition)
                {
                    t.Add(PlayList.Items.IndexOf(b));
                }
                if (a == selectedItem.Count)
                {
                    break;
                }
            }
            t.Sort();
            t.Reverse();

            for (int i = 0; i < t.Count; ++i)
            {
                _playList.RemoveAt(t[i]);
                if (_currentPosition > t[i]) _currentPosition--;
            }
            _mediaPositionList.Clear();
            for (int i = 0; i < _playList.TotalMedia; i++)
            {
                _mediaPositionList.Add(i);
            }
            if (_isShuffle)
            {
                ShuffleModeFunc();
            }
            _fullPaths.Clear();
            PlayList.ItemsSource = null;
            for (int i = 0; i < _playList.MediaList.Length; i++)
            {
                var info = new FileInfo(_playList.MediaList[i]);
                _fullPaths.Add(info);
            }
            PlayList.ItemsSource = _fullPaths;
            PlayList.SelectedIndex = _currentPosition;
            PlayList.ScrollIntoView(PlayList.SelectedItem);
        }

        private void myGif_Loaded(object sender, RoutedEventArgs e)
        {
            PauseMyGif();
        }

        private void VolumeModeButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            //var value = VolumeSlider.Value;
            if (_isMute == true)
            {
                _player.Volume = _currentVolumeValue;
                VolumeSlider.Value = _currentVolumeValue;
                SetImageVolume(_volumeUri);
            }
            else
            {
                _player.Volume = 0;
                VolumeSlider.Value = 0;
                SetImageVolume(_muteUri);
            }
            _isMute = !_isMute;
        }

        private void VolumeSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = (sender as Slider).Value;
            _player.Volume = value / 100;

            if(value != 0)
            {
                _currentVolumeValue = value;
                SetImageVolume(_volumeUri);
                //_isMute = true;
            }
            else
            {
                SetImageVolume(_muteUri);
                //_currentVolumeValue = value;
                //_isMute = false;
            }
        }
    }
    #endregion
}
