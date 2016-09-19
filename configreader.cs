using Microsoft.Web.Administration;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerManager srv = new ServerManager();
            foreach (var site in srv.Sites)
            {
                foreach (Application app in site.Applications)
                {
                    System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(app.Path, site.Name);
                    foreach (var connect in rootWebConfig.ConnectionStrings.ConnectionStrings) {
                        Console.WriteLine(connect.ToString());
                    }
                }
            }

        }
    }
}
