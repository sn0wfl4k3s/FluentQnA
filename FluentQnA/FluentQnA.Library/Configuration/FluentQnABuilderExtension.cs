using Microsoft.Extensions.DependencyInjection;

namespace FluentQnA
{
    public static class FluentQnABuilderExtension
    {
        public static void AddFluentQnA(this IServiceCollection services, string knowledgebasepath)
        {
            services.AddSingleton<IFluentQnA, FluentQnAService>(option => 
            {
                return new FluentQnAService(knowledgebasepath);
            });
        }
    }
}