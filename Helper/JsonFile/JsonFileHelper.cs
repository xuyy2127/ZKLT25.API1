using Microsoft.Extensions.Configuration.Json;

namespace ZKLT25.API.Helper.JsonFile
{
    public static class JsonFileHelper
    {
        private static IConfiguration Configuration(string fileName)
                        => new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .Add(new JsonConfigurationSource { Path = fileName, Optional = false, ReloadOnChange = true })
                            .Build();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetKeyValue(string key, string fileName = "appsettings.json")
        {
            try
            {
                return Configuration(fileName)[key] ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<T> GetKeyValue<T>(string key, string fileName = "appsettings.json")
        {
            List<T> list = new List<T>();
            Configuration(fileName).Bind(key, list);
            return list;
        }
    }
}
