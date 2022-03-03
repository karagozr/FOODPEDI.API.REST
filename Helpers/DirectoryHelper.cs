using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Helpers
{
    public static class DirectoryHelper
    {
        private static string _basePath = $"{AppContext.BaseDirectory}";

        private static string _localData = $"{_basePath}Local Data\\";


        public static string GetLocalDataPath(string folderName)
        {
            if (!Directory.Exists(_localData))  Directory.CreateDirectory(_localData);

            if (!Directory.Exists(_localData + folderName)) Directory.CreateDirectory(_localData + folderName);

            return _localData + folderName+"\\";

        }

        
    }
}
