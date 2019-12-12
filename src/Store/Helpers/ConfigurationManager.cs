using System.IO;
using Microsoft.Extensions.Configuration;

namespace Store.Helpers
{
    public class ConfigurationManager
    {
        public ConfigurationManager()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }
    }
}
