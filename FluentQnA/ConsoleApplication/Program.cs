using FluentQnA;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var service = new FluentQnAService("knowledgebase.xlsx");

                var question = "o que você pode fazer";

                var results = await service.GetAnswers(question);

                Console.WriteLine(results.First().Answer);

            }).GetAwaiter().GetResult();
        }
    }
}
