using Microsoft.Web.Administration;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using CriptografiaString;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerManager srv = new ServerManager();
            List<string> result = new List<string>();
            List<string> erro = new List<string>();
            foreach (var site in srv.Sites)
            {
                foreach (Application app in site.Applications)
                {
                    System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(app.Path, site.Name);
                    foreach (var connectStr in rootWebConfig.ConnectionStrings.ConnectionStrings) {
                        try
                        {
                            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectStr.ToString());
                            if (builder.DataSource != resource.defaultConnection)
                            {
                                result.Add(String.Format(resource.resultString, site.Name, app.ToString(), builder.DataSource, builder.InitialCatalog));
                            }
                        }
                        catch (Exception e)
                        {
                            if (e.Message == resource.criptografadoSICOF)
                            {
                                try
                                {
                                    Cipher cript = new Cipher();
                                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(cript.DeCipher(connectStr.ToString()));
                                    if (builder.DataSource != resource.defaultConnection)
                                    {
                                        result.Add(String.Format(resource.resultString, site.Name, app.ToString(), builder.DataSource, builder.InitialCatalog));
                                    }
                                }catch (Exception f)
                                {
                                    erro.Add(String.Format(resource.errorString, f.Message, site.Name, app.ToString()));
                                }

                            }
                            else {
                                erro.Add(String.Format(resource.errorString, e.Message, site.Name, app.ToString()));
                            }
                        }
                    }
                }
            }
            List<string> finalResult = result.Distinct().ToList();
            StreamWriter file = new StreamWriter("C:\\textoConnectionStrings.txt");
            finalResult.ForEach(file.WriteLine);
            file.Close();
            StreamWriter file2 = new StreamWriter("C:\\ErroStrings.txt");
            erro.ForEach(file2.WriteLine);
            file2.Close();
        }
    }
}
