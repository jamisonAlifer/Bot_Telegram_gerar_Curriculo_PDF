using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using TelegramBotv1.Pdf;
using System.Data.Common;
using System;
using System.IO;
using Telegram.Bot.Types;

namespace TelegramBotv1.Pdf
{
    internal class GerarPdf
    {
       
        public GerarPdf() { }

        [Obsolete]
        public string Gerar(Curriculo curriculo) {

            DateTime dataAtual = DateTime.Now;
            var locationPdf = $"../../../Storage/curriculo_{dataAtual:yyyyMMdd_HHmmss}.pdf";
            QuestPDF.Settings.License = LicenseType.Community;

            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().ShowOnce().AlignCenter().Text(curriculo.Nome).Bold().FontSize(30);

                    
                        page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                         {
                            x.Spacing(8);
                            x.Item().PaddingVertical(0.1f).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                             if (curriculo.Photo == null || !System.IO.File.Exists(curriculo.Photo))
                             {
                                 x.Item().Text(text =>
                                 {
                                     text.Span("Endereço:").Bold();
                                     text.Span(" " + curriculo.Endereco + "\n");
                                                                                                     

                                     text.Span("Telefone:").Bold();
                                     text.Span(" " + curriculo.Telefone+ "\n");
                                                                  

                                     text.Span("E-mail:").Bold();
                                     text.Span(" " + curriculo.Email);
                                 });
                             }
                             else
                             {
                                 x.Item().Row(y =>
                                 {
                                     y.AutoItem()
                                     .Width(1.3f, Unit.Inch)
                                     .Image(curriculo.Photo)
                                     .WithRasterDpi(75);

                                     y.Spacing(10);

                                     y.RelativeItem().Column(z =>
                                     {

                                         z.Item().Text(text =>
                                         {
                                             text.Span("Endereço:").Bold();
                                             text.Span(" " + curriculo.Endereco + "\n");

                                            

                                             text.Span("Telefone:").Bold();
                                             text.Span(" " + curriculo.Telefone + "\n");
                                             
                                             

                                             text.Span("E-mail:").Bold();
                                             text.Span(" " + curriculo.Email);
                                         });
                             
                                     });
                                 });

                             }
                             
                            x.Item().PaddingVertical(0.3f).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                            //FIM DADOS PESSOAIS
                            //INICIO OBJEITVOS PORFISSIONAIS
                            if (!string.IsNullOrEmpty(curriculo.ObjetivoProfissional))
                            {
                                x.Item().Text(text =>
                                {
                                    text.Span("Objetivo Profissional:").Bold();
                                    text.Span(" " + curriculo.ObjetivoProfissional);
                                });
                                x.Item().PaddingVertical(0.5f).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                            }
                             
                             //Experiencia
                             if (curriculo.ExperienciasProfissionais != null && curriculo.ExperienciasProfissionais.Any())
                             {
                                 x.Item().Text("Experiência Profissional:").Bold();
                                 curriculo.ExperienciasProfissionais.ForEach(exp =>
                                 {
                                     x.Item().Text(text =>
                                     {
                                         text.Span($"{exp.DataInicio} " + $" a {exp.DataTermino}");
                                         text.Span(" - Cargo:").Bold();
                                         text.Span(" " + exp.Cargo);
                                         text.Span(" - " + exp.NomeEmpresa);
                                         text.Span(" - " + exp.Localizacao + "\n");
                                        

                                         if (exp.Responsabilidades.Count() > 0)
                                         {
                                             text.ParagraphSpacing(15, Unit.Centimetre);
                                             text.Span("Responsabilidade: \n").SemiBold();
                                             
                                             // Adicionando identação manualmente com espaços
                                             exp.Responsabilidades.ForEach(responsabilidade =>
                                             {
                                                 text.Span("  ");
                                                 text.Span("  ");
                                                 text.Span("  • " + responsabilidade+"\n");

                                             });
                                         }
                                     });
                                 });
                                 x.Item().PaddingVertical(0.1f).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                             }
                             //edu
                             if (curriculo.Educacoes != null && curriculo.Educacoes.Any())
                             {
                                 x.Item().Text("Educação").Bold();

                                 curriculo.Educacoes.ForEach(edu =>
                                 {
                                     x.Item().Text(text =>
                                     {
                                         text.Span($"• {edu.CursoDiploma} - na instituição {edu.NomeInstituicao} - {edu.Localizacao}\n");
                                       
                                         text.Span("Duração:").SemiBold();
                                         text.Span($" de {edu.DataInicio} a {edu.DataTermino}\n");
                                         //Crusos
                                         if (edu.Realizacoes.Count() > 0)
                                         {
                                            
                                             text.Span("Realizações: \n").SemiBold();
                                             
                                             edu.Realizacoes.ForEach(realizacao =>
                                             {
                                                 text.Span("  ");
                                                 text.Span("  ");
                                                 text.Span("  • " + realizacao);

                                             });
                                         }
                                     });
                                 });
                                 x.Item().PaddingVertical(0.5f).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                             }

                             if (curriculo.Habilidades != null && curriculo.Habilidades.Any())
                             {
                                 x.Item().Text("Habilidades:").Bold();
                                 x.Item().Text(string.Join(", ", curriculo.Habilidades));
                                 x.Item().PaddingVertical(0.5f).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                              }

                             if (curriculo.AtividadesExtracurriculares != null && curriculo.AtividadesExtracurriculares.Any())
                             {
                                x.Item().Text("Atividades Extracurriculares:").Bold();
                                x.Item().Text(string.Join("\n", curriculo.AtividadesExtracurriculares));
                                x.Item().PaddingVertical(0.5f).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                             }
                         });
                    page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
                });
               
            }).GeneratePdf(locationPdf);

            return locationPdf;

        }

    }
}
