using MLModel1_ConsoleApp1;

namespace TelegramBotv1.Pdf
{
    public class AnaliseMsg()
    {
        public string[] analisar(string msg)
        {         
            // Create single instance of sample data from first line of dataset for model input
            MLModel1.ModelInput sampleData = new MLModel1.ModelInput()
            {
                Col0 = msg+".",
            };



            Console.WriteLine("Using model to make single prediction -- Comparing actual Col1 with predicted Col1 from sample data...\n\n");


            var sortedScoresWithLabel = MLModel1.PredictAllLabels(sampleData);
            //Console.WriteLine($"{"Class",-40}{"Score",-20}");
            //Console.WriteLine($"{"-----",-40}{"-----",-20}");
            string[] res = new string[2];


            foreach (var score in sortedScoresWithLabel)
            {
                Console.WriteLine($"{score.Key,-40}{score.Value,-20}");
                res[0] = score.Key;
                res[1] = $"{score.Value}";
                break;
            }

            if (res[0] == "0") 
                Console.WriteLine("0 - Nome : " + res[1]);
            else if (res[0] == "1")
                Console.WriteLine("1 - Endereço: " + res[1]);
             else if (res[0] == "2")
                Console.WriteLine("2 - E-mail: " + res[1]);
            else if (res[0] == "4")
                Console.WriteLine("4 - Experiencia: " + res[1]);
            else 
                Console.WriteLine("5 - Educação: " + res[1]);



            //Console.WriteLine("=============== End of process, hit any key to finish ===============");
            return res;
            
        }
    }
}


