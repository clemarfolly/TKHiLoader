using System.IO;
using TKHiLoader.DTO;

namespace TKHiLoader.Helper
{
    public static class ConfigHelper
    {
        private static Option _currentConfiguration;

        public static Option CurrentConfiguration
        {
            get
            {
                if (_currentConfiguration == null)
                {
                    _currentConfiguration = LoadConfig();
                }

                return _currentConfiguration;
            }
            set
            {
                _currentConfiguration = value;
                SaveConfig(_currentConfiguration);
            }
        }

        public static void SaveConfig(Option option)
        {
        }

        public static Option LoadConfig()
        {
            return new Option { L10n = "pt-BR" };
        }

        public static string GetSoftwarePath()
        {
            return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Software");
        }

        public static string GetAppPath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}