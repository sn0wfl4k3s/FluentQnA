using FluentAssertions;
using FluentQnA.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Xunit;

namespace FluentQnA.UnitTest
{
    public class UnitTest1
    {
        private readonly IEnumerable<QnA> _knowledgebase;

        public UnitTest1()
        {
            _knowledgebase = new List<QnA>
            {
                new QnA { Question = "Quem é você?", Answer = "Sou um serviço para responder perguntas de usuários." },
                new QnA { Question = "O que você é?", Answer = "Sou um serviço para responder perguntas de usuários." },
                new QnA { Question = "O que você faz?", Answer = "Sou um serviço para responder perguntas de usuários." },
                new QnA { Question = "O que você pode fazer?", Answer = "Sou um serviço para responder perguntas de usuários." },
                new QnA { Question = "Você possui suporte para aplicações web?", Answer = "Sim, basta acrescentar na CollectionServices" },
                new QnA { Question = "posso usá-lo em aplicações web?", Answer = "Sim, basta acrescentar na CollectionServices" },
                new QnA { Question = "Qual o seu autor?", Answer = "Snowflakes" },
            };
        }

        [Fact]
        public void Usando_em_console_applications()
        {
            IFluentQnA fluentService = new FluentQnAService(_knowledgebase);

            fluentService.TrainingModel();

            QnAResult answer = fluentService.GetAnswer("O que é você?");

            answer.Answer.Should().NotBeNullOrEmpty();
            answer.Answer.Should().StartWith("Sou um serviço");
        }

        [Fact]
        public void Usando_em_web_applications()
        {
            IServiceCollection collection = new ServiceCollection();

            collection.AddFluentQnA(knowledgebase: _knowledgebase, trainInStartup: true);

            ServiceProvider provider = collection.BuildServiceProvider();

            IFluentQnA fluentService = provider.GetService<IFluentQnA>();

            QnAResult answer = fluentService.GetAnswer("O que é você?");

            answer.Answer.Should().NotBeNullOrEmpty();
            answer.Answer.Should().StartWith("Sou um serviço");
        }
    }
}
