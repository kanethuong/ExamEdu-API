using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using examedu.DTO.ExamDTO;
using examedu.DTO.StudentDTO;
using ExamEdu.DB.Models;
using ExamEdu.DTO.ExamDTO;
using ExamEdu.DTO.PaginationDTO;

namespace ExamEdu.Services
{
    public interface IExamService
    {
        Task<Tuple<int, IEnumerable<Exam>>> getExamByStudentId(int studentId, PaginationParameter paginationParameter);
        Task<int> CreateExamPaperByHand(CreateExamByHandInput input);
        Task<int> CreateExamPaperAuto(CreateExamAutoInput input);
        Task<Exam> getExamById(int id);
        bool IsFinalExam(int examId);
        Tuple<int, IEnumerable<Exam>> GetExamsByClassModuleId(int classModuleId, int moduleId, PaginationParameter paginationParameter);

        Task<Tuple<int, int>> CreateExamInfo(Exam exam);

        Task<Tuple<int, IEnumerable<StudentMarkResponse>>> GetResultExamByExamId(int examId, PaginationParameter paginationParameter);
        Task<Tuple<int, IEnumerable<StudentMarkResponse>>> GetStudentListInExamByExamId(int examId, PaginationParameter paginationParameter);

        StudentExamInfo GetStudentExamInfo(int studentId, int examId);
        Task<byte[]> GenerateExamMarkReport(int examId, int classModuleId);
        Task<IEnumerable<StudentMarkResponse>> GetResultExamListByExamId(int examId);
        Task<int> UpdateExamRoom(int examId, string roomId);

        Task<int> UpdateExam(Exam exam);
        Task<Exam> GetUpdateExam(int examId);
        Task<Tuple<int, IEnumerable<Exam>>> GetAllExam(PaginationParameter paginationParameter);
        bool IsCancelled(int examId);
        bool IsExist(int examId);
        Task<int> CancelExam(int examId);
        Task<IEnumerable<ProgressExamReport>> GetAllExamResultByClassModuleId(int classModuleId, int moduleId);
        Task<byte[]> GenerateModuleProgressExamReport(int classModuleId, int moduleId);
        Task<Exam> GetExamDetailByExamId(int examId);
        Task<Tuple<int, IEnumerable<Exam>>> GetExamByProctorId(int proctorId, PaginationParameter paginationParameter);
        Task<int> UpdateMaxTimeToFinishExamOfStudent(int examId, int studentId);
    }
}