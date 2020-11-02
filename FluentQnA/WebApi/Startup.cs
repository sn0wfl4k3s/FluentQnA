using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentQnA;
using System.Collections.Generic;
using FluentQnA.Models;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // é necessário ter uma base de dados de perguntas e respostas previamente elaborado
            var knowledgebase = new List<QnA>
            {
                new QnA { Question = "Quem é você?", Answer = "Sou um serviço para responder perguntas de usuários." },
                new QnA { Question = "O que você é?", Answer = "Sou um serviço para responder perguntas de usuários." },
                new QnA { Question = "O que você faz?", Answer = "Sou um serviço para responder perguntas de usuários." },
                new QnA { Question = "O que você pode fazer?", Answer = "Sou um serviço para responder perguntas de usuários." },
                new QnA { Question = "Você possui suporte para aplicações web?", Answer = "Sim, basta acrescentar na IServiceCollection" },
                new QnA { Question = "posso usá-lo em aplicações web?", Answer = "Sim, basta acrescentar na IServiceCollection" },
                new QnA { Question = "Qual o seu autor?", Answer = "Snowflakes" },
            };

            services.AddFluentQnA(knowledgebase: knowledgebase, trainInStartup: true);

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
