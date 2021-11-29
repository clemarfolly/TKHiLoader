using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TKHiLoader.DTO;

namespace TKHiLoader.Helper
{
    public static class SoftwareHelper
    {
        public static string DefaultPreviewImage
        {
            get
            {
                return "pack://application:,,,/TKHiLoaderviewModel;component/Image/no_preview.png";
            }
        }

        public static IList<Software> GetSoftwareList()
        {
            IList<Software> softwareList = new List<Software>();

            string basePath = ConfigHelper.GetSoftwarePath();

            if (Directory.Exists(basePath))
            {
                var files = Directory.EnumerateFiles(basePath, "*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    if (!FormatHelper.Formats.Any(f => f.FileExtension == Path.GetExtension(file.ToLower())))
                        continue;

                    softwareList.Add(GetNewSoftware(file));
                }
            }

            return softwareList;
        }

        public static Software GetNewSoftware(string file)
        {
            var directory = Path.GetDirectoryName(file);
            var fileWithOutExtension = Path.GetFileNameWithoutExtension(file);

            var software = new Software
            {
                File = file,
                Name = fileWithOutExtension,
                Screenshot = DefaultPreviewImage,
            };

            //  .\Software\DeveloperName\Software.p
            //  .\Software\DeveloperName\Image\Software.[bmp,png,gif,jpg]
            //  .\Software\DeveloperName\Info\Software.[txt,htm,html]
            //  .\Software\DeveloperName\Summary\Software.[txt]

            FindDeveloper(software, directory);
            FindScreenShot(software, directory, fileWithOutExtension);
            FindInfo(software, directory, fileWithOutExtension);
            FindSummary(software, directory, fileWithOutExtension);

            return software;
        }

        private static void FindScreenShot(Software software, string directory, string fileWithOutExtension)
        {
            string[] imageValidExtensions = { ".bmp", ".png", ".gif", ".jpg" };

            foreach (var imageExtension in imageValidExtensions)
            {
                string screenshot = Path.Combine(directory, "Image", $"{fileWithOutExtension}{imageExtension}");

                if (File.Exists(screenshot))
                {
                    software.Screenshot = screenshot;
                    break;
                }
            }
        }

        private static void FindDeveloper(Software software, string directory)
        {
            var dirs = directory.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

            int pos = 0;
            bool seeking = false;
            for (int i = 0; i < dirs.Count(); i++)
            {
                if (dirs[i].ToLower() == "software")
                {
                    seeking = true;
                    continue;
                }

                if (!seeking)
                    continue;

                pos++;

                if (pos == 1)
                    software.Developer = dirs[i];

                if (pos == 2)
                    software.Name = dirs[i];
            }
        }

        private static void FindInfo(Software software, string directory, string fileWithOutExtension)
        {
            string[] infoValidExtensions = { ".txt", ".htm", ".html" };

            foreach (var infoExtension in infoValidExtensions)
            {
                string info = Path.Combine(directory, "Info", $"{fileWithOutExtension}{infoExtension}");

                if (File.Exists(info))
                {
                    software.Information = info;
                    break;
                }
            }
        }

        private static void FindSummary(Software software, string directory, string fileWithOutExtension)
        {
            string[] summaryValidExtensions = { ".txt" };

            foreach (var summaryExtension in summaryValidExtensions)
            {
                string summary = Path.Combine(directory, "Summary", $"{fileWithOutExtension}{summaryExtension}");

                if (File.Exists(summary))
                {
                    software.Summary = summary;
                    break;
                }
            }
        }
    }
}