using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessTheNationality.Service
{
    /// <summary>
    /// This class is to maintain constants
    /// </summary>
    public static class ServiceConstants
    {
        public static readonly string ImagesFilter = "Image*.*";
        public static readonly string ImagesRegex = @"\w*\d*-(\w+)\.jpg";
    }
}
