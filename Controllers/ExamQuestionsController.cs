using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DTO.ExamQuestionsDTO;
using BackEnd.Services.ExamQuestions;
using examedu.Services;
using ExamEdu.DTO;
using ExamEdu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ExamQuestionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IExamQuestionsService _examQuestionsService;
        private readonly IStudentService _studentService;
        private readonly IExamService _examService;
        private readonly IQuestionService _questionService;
        public ExamQuestionsController(IMapper mapper,
                                       IExamQuestionsService examQuestionsService,
                                       IStudentService studentService,
                                       IExamService examService,
                                       IQuestionService questionService)
        {
            _mapper = mapper;
            _examQuestionsService = examQuestionsService;
            _studentService = studentService;
            _examService = examService;
            _questionService = questionService;
        }

        /// <summary>
        /// Get the exam questions of an exam
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        [HttpGet("{examId:int}")]
        public async Task<ActionResult<ExamQuestionsResponse>> GetExamQuestions(int examId, int studentId)
        {
            var exam = await _examService.getExamById(examId);
            if (exam == null)
            {
                return NotFound(new ResponseDTO(404, "Exam cannot be found!"));
            }
            var studentExamInfo = _examService.GetStudentExamInfo(studentId, examId);
            if (studentExamInfo == null)
            {
                return NotFound(new ResponseDTO(404, "You do not have this exam"));
            }
            if (studentExamInfo.FinishAt != null)
            {
                return BadRequest(new ResponseDTO(400, "You have done this exam before"));
            }
            if (exam.ExamDay > DateTime.Now)
            {
                return BadRequest(new ResponseDTO(400, $"It is not time to do the exam. Now is {DateTime.Now}"));
            }
            bool isFinalExam = _examService.IsFinalExam(examId);
            int examCode = await _examQuestionsService.GetRandomExamCodeByExamId(examId, isFinalExam);
            if(examCode == -1)
            {
                return NotFound(new ResponseDTO(404, "This exam does not have any question."));
            }
            List<int> questIdList = new List<int>();
            questIdList = await _examQuestionsService.GetListQuestionIdByExamIdAndExamCode(examId, examCode, isFinalExam);
            if (questIdList.Count <= 0)
            {
                return NotFound(new ResponseDTO(404, "This exam does not have any question."));
            }
            ExamQuestionsResponse examQuestionsResponse = _mapper.Map<ExamQuestionsResponse>(exam);
            examQuestionsResponse.MaxFinishTime = studentExamInfo.MaxFinishTime ;
            examQuestionsResponse.QuestionAnswer = await _questionService.GetListQuestionAnswerByListQuestionId(questIdList, examId, examCode, isFinalExam);
            return Ok(examQuestionsResponse);
        }

        [HttpGet("ExamDetail/{examId:int}")]
        public async Task<ActionResult<List<QuestionAnswerForViewingResponse>>> GetExamQuestionsForExamDetail(int examId)
        {
            if (_examService.IsExist(examId) == false)
            {
                return NotFound(new ResponseDTO(404, "Exam does not exist."));
            }
            bool isFinalExam = _examService.IsFinalExam(examId);
            int examCode = await _examQuestionsService.GetRandomExamCodeByExamId(examId, isFinalExam);
            if(examCode == -1)
            {
                return NotFound(new ResponseDTO(404, "This exam does not have any question."));
            }
            List<int> questIdList = new List<int>();
            questIdList = await _examQuestionsService.GetListQuestionIdByExamIdAndExamCode(examId, examCode, isFinalExam);
            if (questIdList.Count <= 0)
            {
                return NotFound(new ResponseDTO(404, "This exam does not have any question."));
            }
            List<QuestionAnswerForViewingResponse> questionAnswerResponse = await _questionService.GetListQuestionAnswerByListQuestionIdForExamDetail(questIdList, isFinalExam);
            return Ok(questionAnswerResponse);
        }
    }
}