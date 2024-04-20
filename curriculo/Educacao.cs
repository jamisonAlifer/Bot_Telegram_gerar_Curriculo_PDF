using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotv1.Pdf
{
    public class Educacao
    {
        public string CursoDiploma { get; set; }
        public string NomeInstituicao { get; set; }
        public string Localizacao { get; set; }
        public string DataInicio { get; set; }
        public string DataTermino { get; set; }
        public List<string> Realizacoes { get; set; }

        public Educacao()
        {
            Realizacoes = new List<string>();
        }
    }
}
