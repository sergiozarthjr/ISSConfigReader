using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using iisConfReader.POCO;
using Microsoft.Web.Administration;
using System.Data.SqlClient;
using System;
using Newtonsoft.Json;
using CriptografiaString;
using System.Linq;

namespace iisConfReader.Controllers
{
    [Route("api/[controller]")]
    public class LevantamentoDadosController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            yield return JsonConvert.SerializeObject(RetornaListaBasesAtivas());
        }

        private Listas RetornaListaBasesAtivas()
        {
            ServerManager srv = new ServerManager();
            List<BancoDados> listaBancos = new List<BancoDados>();
            List<Erro> listaErro = new List<Erro>();
            foreach (var site in srv.Sites)
            {
                foreach (Application app in site.Applications)
                {
                    System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(app.Path, site.Name);
                    foreach (var connectStr in rootWebConfig.ConnectionStrings.ConnectionStrings)
                    {
                        try
                        {
                            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectStr.ToString());
                            if (builder.DataSource != resource.defaultConnection && !String.IsNullOrEmpty(builder.DataSource))
                            {
                                listaBancos.Add(new BancoDados { Aplicacao = site.Name, Instancia = builder.DataSource, NomeBase = builder.InitialCatalog, Data = DateTime.Now });
                            }
                            else
                            {
                                listaBancos.Add(new BancoDados { Aplicacao = site.Name, Instancia = builder.DataSource, NomeBase = builder.InitialCatalog, Data = DateTime.Now });
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
                                        listaBancos.Add(new BancoDados { Aplicacao = site.Name, Instancia = builder.DataSource, NomeBase = builder.InitialCatalog, Data = DateTime.Now });
                                    }
                                }
                                catch (Exception f)
                                {
                                    listaErro.Add(new Erro { Mensagem = f.Message, Aplicacao = app.ToString(), Data = DateTime.Now, NomeSite = site.Name });
                                }
                            }
                            else
                            {
                                listaErro.Add(new Erro { Mensagem = e.Message, Aplicacao = app.ToString(), Data = DateTime.Now, NomeSite = site.Name });
                            }
                        }
                    }
                }
            }
            return new Listas { Bancos = listaBancos.Distinct().ToList(), Erros = listaErro.Distinct().ToList() };
        }
    }
}
