using Microsoft.Extensions.DependencyInjection;

namespace FluentQnA
{
    public static class FluentQnABuilderExtension
    {
        public static void AddFluentQnA(this IServiceCollection services)
        {
            services.AddSingleton<IFluentQnA, FluentQnAImpl>(option => 
            {
                var fluent = new FluentQnAImpl();

                fluent.LoadKnowledgebase();

                fluent.TrainingModel();
                
                return fluent;
            });
        }
    }
}