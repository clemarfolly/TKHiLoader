using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TKHiLoader.Helper
{
    public static class HelpHelper
    {
        public static string GetQuickGuideFile()
        {
            string basePath = ConfigHelper.GetAppPath();

            var files = Directory.EnumerateFiles(Path.Combine(basePath, "Help"), "QuickGuide*.*", SearchOption.AllDirectories);

            var quickGuideFileName = $"QuickGuide_{ConfigHelper.CurrentConfiguration.L10n}.htm";

            foreach (var file in files)
            {
                if (Path.GetFileName(file).ToLower().Contains(quickGuideFileName.ToLower()))
                {
                    return file;
                }
            }

            return null;
        }

    }
}
