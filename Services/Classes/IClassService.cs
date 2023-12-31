using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExamEdu.DB.Models;
using ExamEdu.DTO.ClassDTO;
using ExamEdu.DTO.PaginationDTO;

namespace examedu.Services.Classes
{
    public interface IClassService
    {
        Task<Tuple<int, IEnumerable<Class>>> GetClasses(int teacherId, int moduleId, PaginationParameter paginationParameter);
        Task<bool> IsClassExist(int classId);
        Task<Class> GetClassBasicInforById(int classId);
        //update Class
        Task<int> UpdateClassBasicInfor(Class classUpdated);
        Task<Tuple<int, IEnumerable<Class>>> GetAllClasses(PaginationParameter paginationParameter);
        Task<bool> IsClassNameExist(string className);
        Task<int> CreateNewClass(Class classInput);
        Task<Tuple<int, IEnumerable<Student>>> GetAllStudentOfClass(int classId, PaginationParameter paginationParameter);
    }
}