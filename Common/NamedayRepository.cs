using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Common
{
    public static class NamedayRepository
    {
        private static List<NamedayModel> allNamedaysCache;

        public static async Task<List<NamedayModel>> GetAllNamedaysAsync()
        {
            if (allNamedaysCache != null)
                return allNamedaysCache;

            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/namedays_fr.json"));
            var stream = await file.OpenAsync(FileAccessMode.Read);
            var fileStream = stream.AsStream();

            var serializer = new DataContractJsonSerializer(typeof(List<NamedayModel>));
            allNamedaysCache = (List<NamedayModel>)serializer.ReadObject(fileStream);

            return allNamedaysCache;
        }

        public static async Task<string> GetTodaysNamesAsStringAsync()
        {
            var allNamedays = await GetAllNamedaysAsync();
            var todayNamedays = allNamedays.Find(d => d.Day == DateTime.Now.Day && d.Month == DateTime.Now.Month);

            return todayNamedays?.NamesAsString;
        }
    }
}
