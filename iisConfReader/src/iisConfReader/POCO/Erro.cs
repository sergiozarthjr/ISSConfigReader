using System;

namespace iisConfReader.POCO
{
    public class Erro
    {
        public string Aplicacao { get; set; }
        public string NomeSite { get; set; }
        public string Mensagem { get; set; }
        public DateTime Data { get; set; }
    }
}
