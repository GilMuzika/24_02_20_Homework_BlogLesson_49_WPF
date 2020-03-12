
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Newtonsoft.Json.Linq;
using System.Windows.Media.Imaging;

namespace _24_02_20_Homework_BlogLesson_49_WPF
{
    class CountriesParser
    {
        

        private string _countries_list_to_parse = @"https://www.freeflagicons.com/list/";

        public string CountriesListUrlToParse
        {
            get => _countries_list_to_parse;
            set => _countries_list_to_parse = value;
        }


        public CountriesParser(string countries_list_to_parse)
        {
            this.CountriesListUrlToParse = countries_list_to_parse;            
            Initialize();
        }
        public CountriesParser() { Initialize(); }
       
        private void Initialize()
        {

        }
        public async Task<List<Country>> Deserialize(string filePath, string pathToFlags)
        {
            var tsk = Task.Factory.StartNew(() => 
            {

                if (!File.Exists(filePath)) return null;

                string jsonValue = string.Empty;
                lock(this)
                {
                    using (Stream readingStream = new FileStream(filePath, FileMode.Open))
                    {

                        byte[] bytes = new byte[readingStream.Length];
                        int len = 0;

                        while ((len = readingStream.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            jsonValue += Encoding.Default.GetString(bytes, 0, len);
                        }
                    }

                }
                List<Country> toReturn = new List<Country>();
                List<int> first = new List<int>();
                List<int> second = new List<int>();                
                for(int i = 0; i < jsonValue.Length; i++)
                {
                    if (jsonValue[i] == '{') first.Add(i);
                    if (jsonValue[i] == '}') second.Add(i);
                }


                for (int j = 0; j < first.Count; j++)
                {
                    string str = string.Empty;
                    for (int i = 0; i < jsonValue.Length; i++)
                    {

                        if (i >= first[j])
                        {
                            if (i <= second[j]) str += jsonValue[i];
                            else break;
                        }

                    }                    
                    var tuple = GetPropPaths(string.Empty, JObject.Parse(str));


                    string[] flagsFullPaths = new DirectoryInfo(pathToFlags).GetFiles().Select(x => x.FullName).ToArray();       
                    string[] flagsNames = new DirectoryInfo(pathToFlags).GetFiles().Select(x => x.Name).ToArray();
                    ExpandoObject fromTupleDynamic = new ExpandoObject();
                    foreach (var s in tuple)
                    {
                        AddProperty(fromTupleDynamic, s.Item1, s.Item2);
                    }
                    var expandoDict = fromTupleDynamic as IDictionary<string, object>;

                    if (expandoDict.ContainsKey("name"))
                    {                        
                        expandoDict.Add("flag", flagsFullPaths.Where(x => Regex.Match(expandoDict["name"].ToString().ToLower(), x.Substring(x.LastIndexOf("\\")+1, x.LastIndexOf(".") - x.LastIndexOf("\\")-1).ToLower().Replace("_", " ")).Success).ToArray().FirstOrDefault());
                    }

                    Country country = new Country();
                    int count = 0;
                    foreach(var s in expandoDict)
                    {

                        string preValue = s.Value == null ? string.Empty : s.Value.ToString();
                        string value = count == typeof(Country).GetProperties().Length - 1 ? preValue : preValue + ", ";                       
                        typeof(Country).GetProperties()[count].SetValue(country, value);

                        count++;
                    }


                    toReturn.Add(country);
                }   
                return toReturn;
            });

            return await tsk;

        }
        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        private IEnumerable<Tuple<string, string>> GetPropPaths(string currPath, JObject obj)
        {
            foreach (var prop in obj.Properties())
            {
                var propPath = string.IsNullOrWhiteSpace(currPath) ? prop.Name : currPath + "." + prop.Name;

                if (prop.Value.Type == JTokenType.Object)
                {
                    foreach (var subProp in GetPropPaths(propPath, prop.Value as JObject))
                        yield return subProp;
                }
                else
                {
                    yield return new Tuple<string, string>(propPath, prop.Value.ToString());
                }
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }



    }

    class Country
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ISO3 { get; set; }
        public string ISO2 { get; set; }
        public string PhoneCode { get; set; }
        public string Capital { get; set; }
        public string Currency { get; set; }
        public string flag_path { get; set; }
    }
}
