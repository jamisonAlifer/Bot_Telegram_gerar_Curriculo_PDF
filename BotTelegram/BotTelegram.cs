using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;



namespace TelegramBotv1.Pdf
{

    class BotTelegram
    {
        Curriculo curriculo {  get; set; }
        Modelo md { get; set; }
        public AnaliseMsg an {  get; set; }
        public TelegramBotClient botClient { get; private set; }
        public ReceiverOptions receiverOptions { get; private set; }
        public CancellationTokenSource cts { get; private set; }
        public float Context {  get; set; }    
        public int Aux { get; set; }

        public BotTelegram() {
            md = new Modelo();
            curriculo = new Curriculo();
            botClient = new TelegramBotClient("Sua Api Kei do Telegram");
            an = new AnaliseMsg();
            cts = new();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };

        }

        public async Task Bot()
        {
            Context = -1;
            botClient.StartReceiving(
               updateHandler: HandleUpdateAsync,
               pollingErrorHandler: HandlePollingErrorAsync,
               receiverOptions: receiverOptions,
               cancellationToken: cts.Token
            );
   
            //var me = await botClient.GetMeAsync();
            
           // Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            

            // Send cancellation request to stop bot
            cts.Cancel();
            
            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                var n = "";
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                    return;
                // Only process text messages
                if (message.Text is not { } messageText)
                {
                    if (Context == 0)
                    {
                        var fileId = update.Message.Photo.Last().FileId;
                        var fileInfo = await botClient.GetFileAsync(fileId);
                        var filePath = fileInfo.FilePath;
                        DateTime data = DateTime.Now;
                        var destinationFilePath = $"../../../Storage/img_{data:yyyyMMdd_HHmmss}_{message.Chat.Id}.jpg";

                        await using Stream fileStream = System.IO.File.Create(destinationFilePath);
                        await botClient.DownloadFileAsync(
                            filePath: filePath,
                            destination: fileStream,
                            cancellationToken: cancellationToken);

                        Console.WriteLine(destinationFilePath);
                        curriculo.Photo = destinationFilePath;


                        n = "Foto Salva! Me envie seu nome agora.";
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: n,
                            parseMode: ParseMode.Html,
                            cancellationToken: cancellationToken);

                    }
                    return;
                }

                var chatId = message.Chat.Id;
                string[] x = an.analisar(messageText);
                var n0 = Convert.ToInt32(x[0]);
                var n1 = Convert.ToDouble(x[1]);

                Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

                if (n1 >= 0.21f)
                {
                    /*
                    if (Context == 89 & messageText != "pular" &  n1 < 0.21)
                    {
                        curriculo.ObjetivoProfissional= messageText;
                        n = $"Obejtivo Profissional '{curriculo.ObjetivoProfissional}' salvo\nEnvie agora Endereço.";
                        Context = 2;
                    }*/
                    switch (n0)
                    {
                        case 0:
                            n = $"O Nome <b>{messageText}</b> foi salvo!\n\n" +
                                $"Possui alguma area ou objeitvo profissional?\n\n" +
                                "Exemplo: 'Açougeiro', 'Desenvolvedor', 'Pintor'...\n\n" +
                                "Se não, basta pular e enviar seu endereço.\n\n" +
                                "Exemplo de formatação: <b>Avenida das Margaridas, 789 - Bairro Vista Bela - Florianópolis - SC</b>";
                            curriculo.Nome = messageText;
                            Context = 1;
                            break;
                        case 1:
                            n = $"O endereço <b>{messageText}</b> foi salvo!\n" +
                                $"Agora me envie seu emil e telefone. \n" +
                                "Exemplo: <b>anderson@gmail.com, (17) 98800-3402</b>";
                            curriculo.Endereco = messageText;
                            Context = 2;
                            break;
                        case 2:
                            n = $"O e-mail e telefone: <b>{messageText}</b>, foi salvo!\n Agora me envie suas experiencias.\n\n" +
                                    "Exemplo: Engenheira Civil, Empresa ASD, Fortaleza - CE, 28/2018, 01/2023";
                            string[] str = md.SplitString(messageText);
                            if (str.Count() <= 2)
                            {
                                try
                                {
                                    curriculo.Email = str[0].Trim();
                                    curriculo.Telefone = str[1].Trim();
                                }
                                catch (Exception ex)
                                {
                                    n = ex.Message;
                                }
                            }
                            Context = 3;
                            break;
                        case 4:

                            n = $"A experiencia <b>{messageText}</b>, foi salva!\n\n" +
                                "Deseja incluir reposaponsabilidade deste cargo? Se sim," +
                                "basta digitar as resposabildiades <b>separadas por virgula</b>\n\n" +
                                "Exemplo: Manter as prateleiras e áreas de exposição de produtos organizadas e limpas, " +
                                "Emissão de notas fiscais e recibos para os clientes" +
                                "Prestar assistência e orientação aos clientes sobre produtos e serviços oferecidos.\n\n" +
                                "Se não, Informção ref. a educação. Onde concluir ensino medio, cursos, graduação\n\n" +
                                "Exemplo: Bacharelado em Ciência da Computação, Universidade Federal de Santa Catarina.";

                            Context = 7;

                            string[] str2 = md.SplitString(messageText);
                            try
                            {
                                curriculo.ExperienciasProfissionais.Add(new ExperienciaProfissional
                                {
                                    Cargo = str2[0].Trim(),
                                    NomeEmpresa = str2[1].Trim(),
                                    Localizacao = str2[2].Trim(),
                                    DataInicio = str2[3].Trim(),
                                    DataTermino = str2[4].Trim(),
                                    // Responsabilidades = new List<string>
                                    // {
                                    //     "Desenvolvimento de aplicações web utilizando ASP.NET MVC",
                                    //     "Integração com APIs externas",
                                    //     "Manutenção e atualização de sistemas legados"
                                    // }
                                });
                            }
                            catch (Exception ex)
                            {
                                n = ex.Message;
                            }
                            break;
                        case 5:
                            Context = 8;
                            n = $"Informção ref. a educação <b>{messageText}</b>, foi salvo!\n " +
                                $"Ref. a este curso/graduação tem alguma realização que deseja adiconar?\n" +
                                $"Se sim, basta me enviar todas de uma vez separado por virgula.";

                            string[] str1 = md.SplitString(messageText);
                            try
                            {
                                curriculo.Educacoes.Add(new Educacao
                                {
                                    CursoDiploma = str1[0].Trim(),
                                    NomeInstituicao = str1[1].Trim(),
                                    Localizacao = "Cidade XYZ",
                                    DataInicio = str1[2].Trim(),
                                    DataTermino = str1[3].Trim(),
                                    // Realizacoes = new List<string>
                                    //  {
                                    //     "Projeto de conclusão de curso: Desenvolvimento de um sistema de gestão de estoque"
                                    //}

                                });
                            }
                            catch (Exception ex)
                            {
                                n = ex.Message;
                            }
                            break;
                        default: n = "teste"; break;    
                    }
                }
                else if (Context == 7 || Context == 8)
                {

                    Console.WriteLine("753 cad resposabi/realiz");
                    switch (Context)
                    {

                        case 7:
                            int lastIndex1 = curriculo.ExperienciasProfissionais.Count - 1; // Obtem o índice do último item na lista Educacoes
                            if (lastIndex1 >= 0)
                            {
                                string[] str1 = md.SplitString(messageText);
                                foreach (var i in str1)
                                {
                                    curriculo.ExperienciasProfissionais[lastIndex1].
                                        Responsabilidades.Add(i.Trim());

                                }
                            }
                            n = "Responsabilidades cadastrados com sucesso!\n\n" +
                                "Agora você pode me enviar mais registros de experiencia ou" +
                                " simplesmente me enviar o grau de estudo.";
                            Context = 3;
                            break;
                        case 8:
                            int lastIndex2 = curriculo.Educacoes.Count() - 1; // Obtem o índice do último item na lista Educacoes
                            if (lastIndex2 >= 0)
                            {
                                string[] str5 = md.SplitString(messageText);
                                foreach (var i in str5)
                                {
                                    curriculo.Educacoes[lastIndex2].Realizacoes.Add(i.Trim());

                                }

                            }
                            n = "Realizações cadastrados com sucesso!\n\n" +
                                "Deseja cadastrar mais curso/graduações?\n\n" +
                                "Se sim, pode me enviar, caso contrario, por favor digite <b>Não</b>";
                            Context = 22;
                            break;
                    }


                }               
                else if (Context ==11)
                {
                    string[] str5 = md.SplitString(messageText);
                    foreach (string str in str5)
                    {
                        curriculo.Habilidades.Add(str);
                    }
                    n = "Habilidade Salvas! \n\n" +
                        "Revise os dados isneridos: \n\n"+ exibirDados() +
                        "\n\n Para proseguri digite <b>gerar  pdf</b>";
                    Context = 501;
                }
                //-------------------
                
                if (messageText == "/start" || messageText.ToUpper() == "OI")
                {
                    Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Olá! Eu me chamo: <b>Luicleia</b>\n" +
                          "Até o momento so consigo gerar curriculos em PDF\n" +
                          "Para começar digite: <code>começar</code>",
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
                }
                else if(messageText.ToUpper() == "GERAR PDF")
                {
                    GerarPdf pdf = new GerarPdf();
                    var f = pdf.Gerar(curriculo);

                    {
                        await using Stream stream = System.IO.File.OpenRead(f);
                        Message sentMessage = await botClient.SendDocumentAsync(
                        chatId: chatId,
                        document: InputFile.FromStream(stream: stream, fileName: "curriculo.pdf"),
                        caption: "Curriculo de jse");
                    }
                }
                else if (messageText.ToUpper() == "COMEÇAR")
                {
                    Context = 0;
                    Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Que ótimo!\nVamos começar, me envie uma <b>foto</b> de perfil para ser adicionada ao curriculo."+
                    "Em seguindo envie seu nome completo.\nCaso não queira incluir foto, basta enviar somente o nome completo.\n\n" +
                    "Exemplo: <b>Paulo Lima Dos Santos.</b>\n\n" +
                    "Procure sempre escrever o mais identico possivel do exemplo, nossa IA ainda esta em treinamento.",
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
                }
                else if(messageText.ToUpper() == "NÃO" & Context == 22)
                {                         
                    Context = 11;
                    Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Envie seuas Habilidade.\n\n" +
                            "Exempo: <b>ASP.NET MVC, HTML, CSS, JavaScript</b>",
                            parseMode: ParseMode.Html,
                            cancellationToken: cancellationToken);
                }
                else
                {
                    if (n == "")
                    {
                        string[] list = ["🥸", "😎", "🤓", "🧐", "😮", "😯", "🙈", "👽", "👀", "👌","🌎","🤹","👩‍💻","👨‍💻","🧑‍💻","👨‍🏫","🧑‍🏫","👨‍🎓"];
                        Random random = new Random();
                        int numeroAleatorio = random.Next(0, list.Length);
                        
                        n = list[numeroAleatorio];
                    }
                    Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: n,
                            parseMode: ParseMode.Html,
                            cancellationToken: cancellationToken);

                }

               
                /*
                else if(messageText == "finalizar" & Context >= 2)
                {
                    Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Revise Seus dados:\n\n"+ exibirDados(),
                    cancellationToken: cancellationToken);
                                    
                }
                else if(messageText == "gerar pdf" & Context >= 2)
                {
                    GerarPdf pdf = new GerarPdf();
                    var f = pdf.Gerar(curriculo);

                    {
                        await using Stream stream = System.IO.File.OpenRead(f);
                        Message sentMessage = await botClient.SendDocumentAsync(
                        chatId: chatId,
                        document: InputFile.FromStream(stream: stream, fileName: "curriculo.pdf"),
                        caption: "Curriculo de jse");
                    }
                }
                else
                {
                    if (Context <= 1)
                    {
                        n = "Desculpe não entendi, tente repetir e/ou escrever de uma forma diferente 2";
                    }
                    Console.WriteLine("era pra escreve n");
                    Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: n,
                            parseMode: ParseMode.Html,
                            cancellationToken: cancellationToken);
                }
                
                     */
            }

            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(ErrorMessage);
                return Task.CompletedTask;
            }
        }
        public string exibirDados()
        {/*
            curriculo.ObjetivoProfissional = "Marceneiro";
            curriculo.Habilidades = new List<string>
            {
                "C#",
                "ASP.NET MVC",
                "HTML",
                "CSS",
                "JavaScript"
            };
            curriculo.AtividadesExtracurriculares = new List<string>
            {
                "Participação em hackathons",
                "Voluntariado em eventos de tecnologia"
            };
            */
            string str = $"Nome:{curriculo.Nome}\n\n" +
                         $"Email: {curriculo.Email}\n\n" +
                         $"Telfone:{curriculo.Telefone}\n\n" +
                         $"Obejativo:{curriculo.ObjetivoProfissional}\n\n" +
                         "Esperiencias:\n";

            curriculo.ExperienciasProfissionais.ForEach(exp =>
            {
                str += $"Cargo: {exp.Cargo}\n" +
                $"Empresa: {exp.NomeEmpresa}\n" +
                $"cidade: {exp.Localizacao}\n" +
                $"Periodo: {exp.DataInicio} - {exp.DataTermino}\n\n";
                if (exp.Responsabilidades.Count > 0)
                {
                    exp.Responsabilidades.ForEach(responsabilidade =>
                    {

                    });
                }
            });
            curriculo.Educacoes.ForEach(edu =>
            {
                str += $"Curso/Diploma: {edu.CursoDiploma}\n" +
                $"Instituição: {edu.NomeInstituicao}\n" +
                $"Periodo: {edu.DataInicio} - {edu.DataTermino}\n\n";

                if (edu.Realizacoes.Count() > 0)
                {
                    edu.Realizacoes.ForEach(realizacao =>
                    {


                    });
                }
            });

            if (curriculo.Habilidades != null && curriculo.Habilidades.Any())
            {
                str += "Habilidade: \n";
                str += string.Join(", ", curriculo.Habilidades);
            }

            if (curriculo.AtividadesExtracurriculares != null && curriculo.AtividadesExtracurriculares.Any())
            {
                str += "\n\nAtividadesEstracurriculare: \n";
                str += string.Join("\n", curriculo.AtividadesExtracurriculares);
            }
            Console.WriteLine(str);
            return str;
        }

    }
}