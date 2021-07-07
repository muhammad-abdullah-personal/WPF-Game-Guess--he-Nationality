using GuessTheNationality;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GuessTheNationality.Service
{
    /// <summary>
    /// This class is used as lookup collection for images
    /// </summary>
    public class ImagesDictionary
    {
        public Dictionary<int, ImagesInformation> Images;
        public ImagesDictionary(string path)
        {
            LoadImages(path);
        }

        /// <summary>
        /// Load images from folder provided in app config
        /// </summary>
        /// <param name="path"></param>
        private void LoadImages(string path)
        {
            var regex = new Regex(ServiceConstants.ImagesRegex);
            var files = GetImagesFromDirectory(path);
            int i = 0;
            foreach (var file in files)
            {
                var matches = regex.Match(file.Name);
                var nationality = matches.Groups[1].Value;
                Images.Add(i, new ImagesInformation
                {
                    ImagePath = file.FullName,
                    Nationality = nationality
                });
                i++;
            }
        }

        private FileInfo[] GetImagesFromDirectory(string path)
        {
            Images = new Dictionary<int, ImagesInformation>();
            DirectoryInfo di = new DirectoryInfo(path);
            return di.GetFiles(ServiceConstants.ImagesFilter);
        }
    }
    public class ImagesInformation
    {
        public string Nationality { get; set; }
        public string ImagePath { get; set; }
    }

}
