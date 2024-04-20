using TelegramBotv1.Pdf;

namespace TelegramBotv1.Pdf
{
    public class Modelo
    {
        public void cadastrar()
        {
            Curriculo curriculo = new Curriculo
            {
                Photo = "../../../Storage/images.jpg",
                Nome = "João da Silva",
                Endereco = "Rua Exemplo, 123",
                Telefone = "(00) 1234-5678",
                Email = "joao@example.com",
                ObjetivoProfissional = "Desenvolvedor de Software",
                ExperienciasProfissionais = new List<ExperienciaProfissional>
            {
                new ExperienciaProfissional
                {
                    Cargo = "Desenvolvedor Web",
                    NomeEmpresa = "Empresa XYZ",
                    Localizacao = "Cidade ABC",
                    DataInicio = "28/2050",
                    DataTermino = "28/2050",
                    Responsabilidades = new List<string>
                    {
                        "Desenvolvimento de aplicações web utilizando ASP.NET MVC",
                        "Integração com APIs externas",
                        "Manutenção e atualização de sistemas legados"
                    }
                },

            },
                Educacoes = new List<Educacao>
            {
                new Educacao
                {
                    CursoDiploma = "Bacharelado em Ciência da Computação",
                    NomeInstituicao = "Universidade ABC",
                    Localizacao = "Cidade XYZ",
                    DataInicio = "28/2050",
                    DataTermino = "28/2050",
                    Realizacoes = new List<string>
                    {
                        "Projeto de conclusão de curso: Desenvolvimento de um sistema de gestão de estoque"
                    }

                },
            },
                Habilidades = new List<string>
            {
                "C#",
                "ASP.NET MVC",
                "HTML",
                "CSS",
                "JavaScript"
            },
                AtividadesExtracurriculares = new List<string>
            {
                "Participação em hackathons",
                "Voluntariado em eventos de tecnologia"
            }
            };

            var pdf = new GerarPdf();

           pdf.Gerar(curriculo);
        }
        public string[] SplitString(string str)
        {
            // Exemplo de entrada de frase
            //string frase = "Esta é uma frase de exemplo, com várias, vírgulas.";

            // Dividindo a frase em partes com base na vírgula
            string[] partes = str.Split(',');
  
            return partes;
        }
    }
}