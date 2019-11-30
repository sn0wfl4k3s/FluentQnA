using FluentQnA;
using System;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                IFluentQnA service = new FluentQnAImpl();

                service.LoadKnowledgebase();

                service.TrainingModel();

                var metrics = await service.GetAnswers("seu nome?", 0f);

                foreach (var qna in metrics)
                {
                    Console.WriteLine($"Resposta: {qna.Answer} | Acuracia: {qna.Accuracy}");
                }

            }).GetAwaiter().GetResult();
        }
    }
}
