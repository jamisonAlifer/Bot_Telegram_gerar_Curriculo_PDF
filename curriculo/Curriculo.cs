using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotv1.Pdf
{
    public class Curriculo
    {
        public string Nome { get; set; }
        public string Photo { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string ObjetivoProfissional { get; set; }
        public List<ExperienciaProfissional> ExperienciasProfissionais { get; set; }
        public List<Educacao> Educacoes { get; set; }
        public List<string> Habilidades { get; set; }
        public List<string> AtividadesExtracurriculares { get; set; }

        public Curriculo()
        {
            ExperienciasProfissionais = new List<ExperienciaProfissional>();
            Educacoes = new List<Educacao>();
            Habilidades = new List<string>();
            AtividadesExtracurriculares = new List<string>();
        }
    }
}
