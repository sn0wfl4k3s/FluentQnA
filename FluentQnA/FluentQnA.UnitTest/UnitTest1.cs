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
                new QnA { Question = "Quem � voc�?", Answer = "Sou um servi�o para responder perguntas de usu�rios." },
                new QnA { Question = "O que voc� �?", Answer = "Sou um servi�o para responder perguntas de usu�rios." },
                new QnA { Question = "O que voc� faz?", Answer = "Sou um servi�o para responder perguntas de usu�rios." },
                new QnA { Question = "O que voc� pode fazer?", Answer = "Sou um servi�o para responder perguntas de usu�rios." },
                new QnA { Question = "Voc� possui suporte para aplica��es web?", Answer = "Sim, basta acrescentar na CollectionServices" },
                new QnA { Question = "posso us�-lo em aplica��es web?", Answer = "Sim, basta acrescentar na CollectionServices" },
                new QnA { Question = "Qual o seu autor?", Answer = "Snowflakes" },
            };
        }

        [Fact]
        public void Usando_em_console_applications()
        {
            IFluentQnA fluentService = new FluentQnAService(_knowledgebase);

            fluentService.TrainingModel();

            QnAResult answer = fluentService.GetAnswer("O que � voc�?");

            answer.Answer.Should().NotBeNullOrEmpty();
            answer.Answer.Should().StartWith("Sou um servi�o");
        }

        [Fact]
        public void Usando_em_web_applications()
        {
            IServiceCollection collection = new ServiceCollection();

            collection.AddFluentQnA(knowledgebase: _knowledgebase, trainInStartup: true);

            ServiceProvider provider = collection.BuildServiceProvider();

            IFluentQnA fluentService = provider.GetService<IFluentQnA>();

            QnAResult answer = fluentService.GetAnswer("O que � voc�?");

            answer.Answer.Should().NotBeNullOrEmpty();
            answer.Answer.Should().StartWith("Sou um servi�o");
        }
    }
}
