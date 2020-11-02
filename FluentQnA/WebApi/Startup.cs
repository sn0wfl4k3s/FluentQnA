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

            // � necess�rio ter uma base de dados de perguntas e respostas previamente elaborado
            var knowledgebase = new List<QnA>
            {
                new QnA { Question = "Quem � voc�?", Answer = "Sou um servi�o para responder perguntas de usu�rios." },
                new QnA { Question = "O que voc� �?", Answer = "Sou um servi�o para responder perguntas de usu�rios." },
                new QnA { Question = "O que voc� faz?", Answer = "Sou um servi�o para responder perguntas de usu�rios." },
                new QnA { Question = "O que voc� pode fazer?", Answer = "Sou um servi�o para responder perguntas de usu�rios." },
                new QnA { Question = "Voc� possui suporte para aplica��es web?", Answer = "Sim, basta acrescentar na IServiceCollection" },
                new QnA { Question = "posso us�-lo em aplica��es web?", Answer = "Sim, basta acrescentar na IServiceCollection" },
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
