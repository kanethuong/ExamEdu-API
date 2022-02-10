using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTO.ExamQuestionsDTO;
using examedu.DTO.QuestionDTO;

namespace examedu.Services
{
    public interface IQuestionService
    {
        Task<List<QuestionResponse>> getQuestionByModuleLevel(int moduleID, int levelID, bool isFinalExam);
        Task<List<ExamQuestionsResponse>> GetListExamQuestionsByListQuestionId(List<int> questionIdList, bool isFinalExam);
    }
}