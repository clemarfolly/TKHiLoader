using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using TKHiLoader.Common;
using TKHiLoader.DTO;
using TKHiLoader.Helper;

namespace TKHiLoader.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Variables

        private ICommand _informationButtonCommand;
        private string _playButtonText;
        private ICommand _playCommand;
        private ICommand _stopCommand;
        private Player _player;
        private string _playerLength;
        private string _playerPosition;
        private string _playerStatus;
        private ICommand _reloadCommand;
        private int _selectedDeveloperId;
        private Software _selectedSoftware;
        private bool _playerStopped;
        private ICommand _rewindCommand;
        private ICommand _forwardCommand;
        private bool _useCustomHiLoad;

        private string _wavPoints;
        private List<double> _ypoints;

        #endregion Variables

        #region Window Text Properties

        public string AppTitle { get; private set; }
        public string InformationButtonText { get; set; }
        public string InvertWavPolarityText { get; set; }
        public string OptionsText { get; set; }

        public string PlayButtonText
        {
            get
            {
                return _playButtonText;
            }
            set
            {
                _playButtonText = value;

                OnPropertyChanged("PlayButtonText");
            }
        }

        public string PlayerLength
        {
            get
            {
                return _playerLength;
            }
            set
            {
                _playerLength = value;

                OnPropertyChanged("PlayerLength");
            }
        }

        public string PlayerPosition
        {
            get
            {
                return _playerPosition;
            }
            set
            {
                _playerPosition = value;

                OnPropertyChanged("PlayerPosition");
            }
        }

        public string PlayerStatus
        {
            get
            {
                return _playerStatus;
            }
            set
            {
                _playerStatus = value;

                OnPropertyChanged("PlayerStatus");
            }
        }

        public bool PlayerStopped
        {
            get
            {
                return _playerStopped;
            }
            set
            {
                _playerStopped = value;

                OnPropertyChanged("PlayerStopped");
            }
        }

        public string PlayerPositionText { get; set; }
        public string PlayerLengthText { get; set; }
        public string PlayerText { get; set; }
        public string ProgramsText { get; set; }
        public string SelectedProgramText { get; set; }
        public string TimeBeforeStart { get; set; }
        public string TimeBeforeStartText { get; set; }
        public string UseCustomHiLoadText { get; set; }
        public string UseStandartFormatText { get; set; }

        #endregion Window Text Properties

        #region Properties

        public IList<KeyValuePair<int, string>> DeveloperList
        {
            get
            {
                var list = new List<KeyValuePair<int, string>>();

                list.Add(new KeyValuePair<int, string>(0, "Todos"));
                int i = 0;
                var develp = SoftwareList.Where(g => !string.IsNullOrWhiteSpace(g.Developer)).GroupBy(g => g.Developer).Select(g => g.Key).OrderBy(g => g);

                foreach (var item in develp)
                {
                    i++;
                    list.Add(new KeyValuePair<int, string>(i, item));
                }

                return list;
            }
        }

        public ObservableCollection<MenuItem> MenuItems { get; set; }

        public int SelectedDeveloperId
        {
            get
            {
                return _selectedDeveloperId;
            }
            set
            {
                _selectedDeveloperId = value;
                OnPropertyChanged("SelectedDeveloperId");
                OnPropertyChanged("SoftwareFiltered");
            }
        }

        public Software SelectedSoftware
        {
            get
            {
                return _selectedSoftware;
            }
            set
            {
                _selectedSoftware = value;

                PlayerLength = "-- s";
                PlayerPosition = "-- s";

                OnPropertyChanged("SelectedSoftware");
            }
        }

        public IList<Software> SoftwareFiltered
        {
            get
            {
                if (SelectedDeveloperId == 0)
                {
                    return SoftwareList;
                }

                var developer = DeveloperList.FirstOrDefault(i => i.Key == SelectedDeveloperId);

                return SoftwareList.Where(i => i.Developer == developer.Value).ToList();
            }
        }

        public IList<Software> SoftwareList { get; set; }

        public string WavPoints
        {
            get
            {
                return _wavPoints;
            }
        }

        public bool UseCustomHiLoad
        {
            get
            {
                return _useCustomHiLoad;
            }
             set
            {
                if (_useCustomHiLoad != value)
                {
                    _useCustomHiLoad = value;

                    if (!string.IsNullOrWhiteSpace(SelectedSoftware.WavFile) && File.Exists(SelectedSoftware.WavFile))
                    {
                        if (_player != null)
                        {
                            _player.Dispose();
                            _player = null;
                        }

                        File.Delete(SelectedSoftware.WavFile);
                        SelectedSoftware.WavFile = null;
                    }

                }
            }
        }

        #endregion Properties

        #region Command Properties

        public ICommand InformationButtonCommand
        {
            get
            {
                if (_informationButtonCommand == null)
                {
                    _informationButtonCommand = new RelayCommand(InformationCanExecute, InformationButton);
                }
                return _informationButtonCommand;
            }
        }

        public ICommand PlayCommand
        {
            get
            {
                if (_playCommand == null)
                {
                    _playCommand = new RelayCommand(PlayCanExecute, PlayButton);
                }
                return _playCommand;
            }
        }

        public ICommand StopCommand
        {
            get
            {
                if (_stopCommand == null)
                {
                    _stopCommand = new RelayCommand(StopCanExecute, StopButton);
                }
                return _stopCommand;
            }
        }

        public ICommand ReloadCommand
        {
            get
            {
                if (_reloadCommand == null)
                {
                    _reloadCommand = new RelayCommand((o) => { return true; }, ReloadData);
                }

                return _reloadCommand;
            }
        }

        public ICommand RewindCommand
        {
            get
            {
                if (_rewindCommand == null)
                {
                    _rewindCommand = new RelayCommand((o) => { return SelectedSoftware != null; }, RewindButton);
                }

                return _rewindCommand;
            }
        }

        public ICommand ForwardCommand
        {
            get
            {
                if (_forwardCommand == null)
                {
                    _forwardCommand = new RelayCommand((o) => { return SelectedSoftware != null; }, ForwardButton);
                }

                return _forwardCommand;
            }
        }

        #endregion Command Properties

        #region Constructor

        public MainWindowViewModel()
        {
            AppTitle = "TK HiLoader " + Assembly.GetExecutingAssembly().GetName().Version;
            ProgramsText = "Programas";
            SelectedProgramText = "Programa selecionado";
            OptionsText = "Opções";
            PlayerText = "Player";
            UseCustomHiLoadText = " Usar loader alternativo (Requer 48K de RAM)";
            TimeBeforeStartText = "Tempo de espera no início (ms) ";
            InvertWavPolarityText = " Inverter polaridade da wav";
            UseStandartFormatText = " Usar formato padrão (300bps)";
            InformationButtonText = "Informação";
            PlayerStatus = "Parado";
            PlayerPosition = "-- s";
            PlayerLength = "-- s";
            PlayButtonText = ">";
            PlayerStopped = true;
            PlayerPositionText = "Posição:";
            PlayerLengthText = "Comprim.:";

            CreateMenus();

            SelectedSoftware = new Software() { Screenshot = SoftwareHelper.DefaultPreviewImage };

            ReloadData(null);

            _ypoints = new List<double>();

            _wavPoints = string.Empty;

            for (int i = 0; i < 147; i++)
            {
                AddWavPoint(15);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_player != null)
                _player.Dispose();
        }

        #endregion Constructor

        #region Commands

        private void About(object o)
        {
            WindowManager.ShowAbout(new AboutViewModel());
        }

        private void ExportWav(object o)
        {
            if (SelectedSoftware == null)
                return;

            if (!File.Exists(SelectedSoftware.File))
                return;

            if (string.IsNullOrWhiteSpace(SelectedSoftware.WavFile) || !File.Exists(SelectedSoftware.WavFile))
            {
                SelectedSoftware.WavFile = Path.GetTempFileName();
                WavHelper.GerarWav(SelectedSoftware.File, SelectedSoftware.WavFile, UseCustomHiLoad);
            }

            WindowManager.SaveFile(SelectedSoftware.WavFile, Path.ChangeExtension(Path.GetFileName(SelectedSoftware.File), ".wav"));
        }

        private void GuickGuide(object o)
        {
            var file = HelpHelper.GetQuickGuideFile();

            if (string.IsNullOrWhiteSpace(file))
            {
                WindowManager.ShowMessage("Arquivo não encontrado.");
                return;
            }

            WindowManager.ShowInfo(new InfoViewModel("Guia Rápido", file));
        }

        private void InformationButton(object o)
        {
            WindowManager.ShowInfo(new InfoViewModel("Informações do Programa", SelectedSoftware.Information));
        }

        private void OpenFile(object o)
        {
            var file = WindowManager.OpenFile();

            if (string.IsNullOrWhiteSpace(file))
                return;

            SelectedSoftware = SoftwareHelper.GetNewSoftware(file);
        }

        private void PlayButton(object o)
        {
            if (!File.Exists(SelectedSoftware.File))
                return;

            if (string.IsNullOrWhiteSpace(SelectedSoftware.WavFile) || !File.Exists(SelectedSoftware.WavFile))
            {
                SelectedSoftware.WavFile = Path.GetTempFileName();
                WavHelper.GerarWav(SelectedSoftware.File, SelectedSoftware.WavFile, UseCustomHiLoad);
            }

            if (_player == null)
            {
                _player = new Player();
                _player.OnPlay += Player_OnPlay;
                _player.OnPause += Player_OnPause;
                _player.OnStop += Player_OnStop;
                _player.WavePositionEvent += Player_WavePositionEvent;
                _player.WaveSampleEvent += Player_WaveSampleEvent;
            }

            if (_player.IsStopped)
            {
                _player.Load(SelectedSoftware.WavFile);
            }

            if (_player.IsPlaying)
            {
                _player.Pause();
            }
            else
            {
                _player.Play();
            }
        }

        private void StopButton(object o)
        {
            if (PlayerStopped)
                return;

            _player.Stop();
        }

        private void ReloadData(object o)
        {
            SoftwareList = SoftwareHelper.GetSoftwareList();
        }

        private void Sair(object o)
        {
            WindowManager.Exit();
        }

        private void RewindButton(object o)
        {
            if (_player != null)
                _player.AddPosition(-1000);
        }

        private void ForwardButton(object o)
        {
            if (_player != null)
                _player.AddPosition(1000);
        }

        #endregion Commands

        #region Can Execute

        private bool InformationCanExecute(object o)
        {
            return SelectedSoftware != null &&
                   !string.IsNullOrWhiteSpace(SelectedSoftware.Information) &&
                   File.Exists(SelectedSoftware.Information);
        }

        private bool PlayCanExecute(object o)
        {
            return SelectedSoftware != null &&
                   !string.IsNullOrWhiteSpace(SelectedSoftware.File) &&
                   File.Exists(SelectedSoftware.File);
        }

        private bool StopCanExecute(object o)
        {
            return SelectedSoftware != null &&
                   !string.IsNullOrWhiteSpace(SelectedSoftware.File) &&
                   File.Exists(SelectedSoftware.File) && !this.PlayerStopped;
        }

        #endregion Can Execute

        #region Player Events

        private void Player_OnPause(object sender, EventArgs e)
        {
            PlayerStatus = "Pausado";
            PlayButtonText = ">";
            PlayerStopped = false;
        }

        private void Player_OnPlay(object sender, EventArgs e)
        {
            PlayerStatus = "Tocando";
            PlayButtonText = "||";
            PlayerStopped = false;
        }

        private void Player_OnStop(object sender, EventArgs e)
        {
            PlayerStatus = "Parado";
            PlayButtonText = ">";
            PlayerStopped = true;

            for (int i = 0; i < 147; i++)
            {
                AddWavPoint(15);
            }
        }

        private void Player_WavePositionEvent(TimeSpan pos, TimeSpan len)
        {
            PlayerPosition = string.Format("{0:0.00} s", pos.TotalMilliseconds / 1000.00);
            PlayerLength = string.Format("{0:0.00} s", len.TotalMilliseconds / 1000.00);
        }

        private void Player_WaveSampleEvent(float sample)
        {
            AddWavPoint(15 - (sample * 15));
        }

        #endregion Player Events

        #region Private Methods

        private void CreateMenus()
        {
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem { Header = "Arquivo",
                    MenuItems = new ObservableCollection<MenuItem>
                        {
                            new MenuItem { Header = "Abrir do Disco", Command = new RelayCommand((o) => PlayerStopped, OpenFile) },
                            new MenuItem { Header = "Exportar Arquivo Wav", Command = new RelayCommand((o) => PlayerStopped && !string.IsNullOrWhiteSpace(SelectedSoftware?.File) && File.Exists(SelectedSoftware?.File),  ExportWav) },
                            new MenuItem { Header = "Sair", Command = new RelayCommand((o) => true, Sair) }
                        }
                },
                new MenuItem { Header = "Configurações",
                                    MenuItems = new ObservableCollection<MenuItem>
                        {
                            new MenuItem { Header = "Restaurar Opções Padrão", Command = new RelayCommand((o) => false, null) },
                            new MenuItem { Header = "Abrir Configurações", Command = new RelayCommand((o) => false, null) },
                        }
                },
                new MenuItem { Header = "Ajuda",
                MenuItems = new ObservableCollection<MenuItem>
                        {
                            new MenuItem { Header = "Abrir Ajuda", Command = new RelayCommand((o) => false, null) },
                            new MenuItem { Header = "Guia Rápido", Command = new RelayCommand((o) => PlayerStopped, GuickGuide) },
                            new MenuItem { Header = "Sobre TK HiLoader", Command = new RelayCommand((o) => PlayerStopped, About) }
                        }
                },
            };
        }

        private void AddWavPoint(double y)
        {
            lock (this)
            {
                double x = 0;

                if (_ypoints.Count >= 147)
                    _ypoints.RemoveAt(0);

                _ypoints.Add(y);

                _wavPoints = string.Join(" ", _ypoints.Select(py => $"{x++},{py}"));
            }
            OnPropertyChanged("WavPoints");
        }

        #endregion Private Methods
    }
}