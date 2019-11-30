using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentQnA
{
    public interface IFluentQnA
    {
        /// <summary> The path of the training model that will be trains or has been trained. </summary>
        string TrainedModelPath { get; set; }
        /// <summary> The path of the knowledge base file in json format. </summary>
        string KnowledgeBasePath { get; set; }

        /// <summary> Load the knowledge base from the KnowledgeBasePath. KnowledgeBasePath deafult is knowledgebase.json. </summary>
        void LoadKnowledgebase();
        /// <summary> Trains the model to use the answers prediction.</summary>
        void TrainingModel();
        /// <summary> Queries the knowledge base and returns a possible answer to the question.</summary>
        /// <param name="question">The question that will be analyzed in the knowledge base.</param>
        /// <param name="minAccuracy">the minimum accuracy for the responses returned. The allowable accuracy range is from 0 to 1.</param>
        /// <returns> An enumerable set of QnAResult with the questions, answer and accuracy from each element.</returns>
        Task<IEnumerable<QnAResult>> GetAnswers(string question, float? minAccuracy);
    }
}