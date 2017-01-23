using Microsoft.Web.Administration;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CriptografiaString;
using System.Data;
using Dapper;
using issConfigReader.POCO;

namespace issConfigReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            List<banco> listaBancos = new List<banco>();
            List<erro> listaErro = new List<erro>();
            ServerManager srv = new ServerManager();
            foreach (var site in srv.Sites)
            {
                foreach (Application app in site.Applications)
                {
                    System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(app.Path, site.Name);
                    foreach (var connectStr in rootWebConfig.ConnectionStrings.ConnectionStrings) {
                        try
                        {
                            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectStr.ToString());
                            if (builder.DataSource != resource.defaultConnection && !String.IsNullOrEmpty(builder.DataSource))
                            {
                                listaBancos.Add(new banco { aplicacao = site.Name, instancia = builder.DataSource, nomeBanco = builder.InitialCatalog, data = DateTime.Now });
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
                                    if (builder.DataSource != resource.defaultConnection && !String.IsNullOrEmpty(builder.DataSource))
                                    {
                                        listaBancos.Add(new banco { aplicacao = site.Name, instancia = builder.DataSource , nomeBanco = builder.InitialCatalog , data = DateTime.Now } );
                                    }
                                }catch (Exception f)
                                {
                                    listaErro.Add(new erro { mensagem = f.Message , aplicacao = app.ToString(), data = DateTime.Now, nomeSite = site.Name});
                                }

                            }
                            else {
                                listaErro.Add(new erro { mensagem = e.Message, aplicacao = app.ToString(), data = DateTime.Now, nomeSite = site.Name });
                            }
                        }
                    }
                }
            }
            IDbConnection db = new SqlConnection(ConnectionString);
            db.Execute(resource.insertLevantamento, listaBancos);
            db.Execute(resource.insertErro, listaErro);
            db.Close();
           
        }
    }
}
