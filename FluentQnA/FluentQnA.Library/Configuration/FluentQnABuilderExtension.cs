using FluentQnA.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace FluentQnA
{
    public static class FluentQnABuilderExtension
    {
        /// <summary>
        /// dependency injection for fluentQna service.
        /// </summary>
        /// <param name="knowledgebase"> knowledgebase used for training and inferencing answer </param>
        /// <param name="trainInStartup"> if train the model on initialize the application </param>
        public static void AddFluentQnA(this IServiceCollection services, 
            IEnumerable<QnA> knowledgebase, 
            bool trainInStartup = false)
        {
            services.AddSingleton<IFluentQnA, FluentQnAService>(option => 
            {
                FluentQnAService fluentService = new FluentQnAService(knowledgebase);

                if (trainInStartup)
                {
                    fluentService.TrainingModel();
                }

                return fluentService;
            });

        }
    }
}