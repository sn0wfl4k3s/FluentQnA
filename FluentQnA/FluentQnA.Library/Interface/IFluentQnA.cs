using FluentQnA.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentQnA
{
    public interface IFluentQnA
    {
        /// <summary> 
        /// The path of the training model that will be trains or has been trained. The default value is "trainedModel.zip"
        /// </summary>
        string TrainedModelPath { get; set; }
        
        /// <summary>
        /// Knowledgebase how is used for base to prediction the answer
        /// </summary>
        IEnumerable<QnA> Knowledgebase { get; set; }

        #region TrainingModel
        /// <summary> Trains the model to use the answers prediction.</summary>
        void TrainingModel();
        /// <summary> Trains asynchronously the model to use the answers prediction </summary>
        Task TrainingModelAsync();
        #endregion

        #region GetAnswer
        QnAResult GetAnswer(string question);
        Task<QnAResult> GetAnswerAsync(string question);
        #endregion

        #region GetAnswers
        IEnumerable<QnAResult> GetAnswers(string question);
        Task<IEnumerable<QnAResult>> GetAnswersAsync(string question);
        #endregion
    }
}