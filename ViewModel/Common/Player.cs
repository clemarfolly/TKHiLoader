using NAudio.Wave;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TKHiLoader.Common
{
    public delegate void WavePositionEventHandle(TimeSpan pos, TimeSpan len);

    public delegate void WaveSampleEventHandle(float sample);

    public class Player : IDisposable
    {
        private float _sampleAveragePositive = 0;
        private float _sampleAverageNegative = 0;
        private int _samplePosition = 0;
        private int _sampleMax = 4 * 96;
        private float _sampleAverage;


        private WaveStream _mainOutputStream;
        private WaveChannel32 _volumeStream;
        private WaveOutEvent _player;
        private Thread _wavPositionThread;
        private bool _disposed;

        public event WavePositionEventHandle WavePositionEvent;

        public event EventHandler OnPlay;

        public event EventHandler OnPause;

        public event EventHandler OnStop;

        public event WaveSampleEventHandle WaveSampleEvent;

        public bool IsPlaying
        {
            get
            {
                return _player != null && _player.PlaybackState == PlaybackState.Playing;
            }
        }

        public bool IsPaused
        {
            get
            {
                return _player != null && _player.PlaybackState == PlaybackState.Paused;
            }
        }

        public bool IsStopped
        {
            get
            {
                return _player == null || _player.PlaybackState == PlaybackState.Stopped;
            }
        }

        public Player()
        {
            _wavPositionThread = new Thread(new ThreadStart(WavPositionThreadLoop));
            _wavPositionThread.Start();
        }

        private void WavPositionThreadLoop()
        {
            while (!_disposed)
            {
                if (_player != null && _player.PlaybackState == PlaybackState.Playing)
                {
                    if (_volumeStream != null)
                        WavePositionEvent?.Invoke(_volumeStream.CurrentTime, _volumeStream.TotalTime);
                }
                System.Threading.Thread.Sleep(100);
            }
        }

        public void Load(string file)
        {
            if (_mainOutputStream != null)
            {
                _mainOutputStream.Dispose();
                _mainOutputStream = null;
            }

            _mainOutputStream = new WaveFileReader(file);

            if (_volumeStream != null)
            {
                _volumeStream.Dispose();
                _volumeStream = null;
            }

            _volumeStream = new WaveChannel32(_mainOutputStream);
            _volumeStream.PadWithZeroes = false;

            _volumeStream.Sample += _volumeStream_Sample;

            if (_player != null)
            {
                _player.Dispose();
                _player = null;
            }

            _player = new WaveOutEvent();
            _player.Init(_volumeStream);

            _player.Volume = 1;

            WavePositionEvent?.Invoke(_volumeStream.CurrentTime, _volumeStream.TotalTime);
            _player.PlaybackStopped += _player_PlaybackStopped;

            _sampleAverage = 0;
        }

        //https://stackoverflow.com/questions/26663494/algorithm-to-draw-waveform-from-audio
        //private float temp = 0;
        //private int pos = 0;
        //private int max = 4 * 24;
        //private bool inv;

        //private void _volumeStream_Sample(object sender, SampleEventArgs e)
        //{
        //    if (e.Left < 0)
        //        temp += -e.Left;
        //    else
        //        temp += e.Left;

        //    pos++;

        //    if (pos >= max)
        //    {
        //        temp = temp * 2 / pos;

        //        float t = temp;

        //        if (inv)
        //            t = -t;

        //        inv = !inv;

        //        Task.Factory.StartNew(() =>
        //        {
        //            WaveSampleEvent?.Invoke(t);
        //        });

        //        temp = 0;
        //        pos = 0;
        //    }
        //}

        private void _volumeStream_Sample(object sender, SampleEventArgs e)
        {
            if (e.Left < 0)
                _sampleAverageNegative += e.Left;
            else
                _sampleAveragePositive += e.Left;

            _samplePosition++;

            _sampleAverage = (_sampleAverage + Math.Abs(e.Left)) / 2;

            if (_samplePosition >= _sampleMax)
            {
                _sampleAveragePositive = _sampleAveragePositive * 2 / _samplePosition;
                _sampleAverageNegative = _sampleAverageNegative * 2 / _samplePosition;

                float t = _sampleAveragePositive;

                if (t < (_sampleAverage * 0.5))
                    t = 0;
                else if (t < _sampleAverage)
                    t = (float)(_sampleAverage * 0.3);

                Task.Factory.StartNew(() =>
                {
                    WaveSampleEvent?.Invoke(t);
                });

                float t2 = _sampleAverageNegative;

                if (Math.Abs(t2) < _sampleAverage * 0.5)
                    t2 = 0;
                else if (Math.Abs(t2) < _sampleAverage)
                    t2 = (float)(-_sampleAverage * 0.3);

                Task.Factory.StartNew(() =>
                {
                    WaveSampleEvent?.Invoke(t2);
                });

                _sampleAveragePositive = 0;
                _sampleAverageNegative = 0;
                _samplePosition = 0;
            }
        }


        private void _player_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            OnStop?.Invoke(this, EventArgs.Empty);
        }

        public void Play()
        {
            if (_player == null || _player.PlaybackState == PlaybackState.Playing)
                return;

            _player.Play();

            OnPlay?.Invoke(this, EventArgs.Empty);
        }

        public void Pause()
        {
            if (_player == null || _player.PlaybackState == PlaybackState.Paused)
                return;

            _player.Pause();

            OnPause?.Invoke(this, EventArgs.Empty);
        }

        public void Stop()
        {
            if (_player == null || _player.PlaybackState == PlaybackState.Stopped)
                return;

            //_player.Pause();
            _player.Stop();
            _volumeStream.CurrentTime = TimeSpan.Zero;

            WavePositionEvent?.Invoke(_volumeStream.CurrentTime, _volumeStream.TotalTime);
            OnStop?.Invoke(this, EventArgs.Empty);
        }

        public void AddPosition(int milliseconds)
        {
            if (_player == null || _player.PlaybackState == PlaybackState.Stopped)
                return;

            _volumeStream.CurrentTime = _volumeStream.CurrentTime.Add(TimeSpan.FromMilliseconds(milliseconds));
            WavePositionEvent?.Invoke(_volumeStream.CurrentTime, _volumeStream.TotalTime);
        }

        public void Dispose()
        {
            _disposed = true;
            if (_mainOutputStream != null)
            {
                _mainOutputStream.Dispose();
                _mainOutputStream = null;
            }

            if (_volumeStream != null)
            {
                _volumeStream.Dispose();
                _volumeStream = null;
            }

            if (_player != null)
            {
                _player.Dispose();
                _player = null;
            }
        }
    }
}