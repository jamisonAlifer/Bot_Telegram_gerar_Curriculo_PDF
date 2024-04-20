using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotv1.Pdf
{
    public class ExperienciaProfissional
    {
        public string Cargo { get; set; }
        public string NomeEmpresa { get; set; }
        public string Localizacao { get; set; }
        public string DataInicio { get; set; }
        public string DataTermino { get; set; }
        public List<string> Responsabilidades { get; set; }

        public ExperienciaProfissional()
        {
            Responsabilidades = new List<string>();
        }
    }
}
