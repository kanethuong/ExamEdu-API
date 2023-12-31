using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DTO.StudentDTO;
using BackEnd.Services;
using examedu.DTO.StudentDTO;
using examedu.Services;
using ExamEdu.DB.Models;
using ExamEdu.DTO;
using ExamEdu.DTO.PaginationDTO;
using ExamEdu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace examedu.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ITeacherService _teacherService;
        private readonly IModuleService _moduleService;
        private readonly IMapper _mapper;


        public StudentController(IStudentService studentService, ITeacherService teacherService, IModuleService moduleService, IMapper mapper)
        {
            _studentService = studentService;
            _teacherService = teacherService;
            _moduleService = moduleService;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="moduleId"></param>
        /// <returns>400 student or moduleID not exist / 404 Student dont have exam</returns>
        [HttpGet("markReport/{studentID:int}/{moduleID:int}")]
        public async Task<ActionResult<List<ModuleMarkDTO>>> MarkReport(int studentId, int moduleId)
        {
            List<ModuleMarkDTO> listResult = await _studentService.getModuleMark(studentId, moduleId);
            if (listResult == null)
            {
                return BadRequest(new ResponseDTO(400, "Student ID or Module ID not exist"));
            }
            if (listResult.Count == 0)
            {
                return NotFound(new ResponseDTO(404, "Student maynot have exam in this module"));
            }
            return Ok(listResult);
        }

        [HttpGet("{teacherId:int}/{moduleId:int}")]
        public async Task<IActionResult> GetStudents(int teacherId, int moduleId, [FromQuery] PaginationParameter paginationParameter)
        {
            //If teacher does not exist return bad request
            if (await _teacherService.IsTeacherExist(teacherId) == false)
            {
                return BadRequest(new ResponseDTO(400, "Teacher does not exist"));
            }
            //If module does not exist return bad request
            if (await _moduleService.getModuleByID(moduleId) is null)
            {
                return BadRequest(new ResponseDTO(400, "Module does not exist"));
            }

            (int totalRecord, IEnumerable<Student> students) = await _studentService.GetStudents(teacherId, moduleId, paginationParameter);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "No student found"));
            }
            IEnumerable<StudentResponse> studentsResponses = _mapper.Map<IEnumerable<StudentResponse>>(students);
            return Ok(new PaginationResponse<IEnumerable<StudentResponse>>(totalRecord, studentsResponses));
        }

        [HttpGet("list/{moduleId:int}")]
        public async Task<IActionResult> GetAllStudents(int moduleId, [FromQuery] PaginationParameter paginationParameter)
        {
            //If module does not exist return bad request
            if (await _moduleService.getModuleByID(moduleId) is null)
            {
                return BadRequest(new ResponseDTO(400, "Module does not exist"));
            }

            (int totalRecord, IEnumerable<Student> students) = await _studentService.GetAllStudents(moduleId, paginationParameter);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "No student found"));
            }
            IEnumerable<StudentResponse> studentsResponses = _mapper.Map<IEnumerable<StudentResponse>>(students);
            return Ok(new PaginationResponse<IEnumerable<StudentResponse>>(totalRecord, studentsResponses));
        }

        [HttpGet("{classModuleId:int}")]
        public async Task<IActionResult> GetStudents(int classModuleId, [FromQuery] PaginationParameter paginationParameter)
        {

            (int totalRecord, IEnumerable<Student> students) = await _studentService.GetStudents(classModuleId, paginationParameter);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "No student found"));
            }
            IEnumerable<StudentResponse> studentsResponses = _mapper.Map<IEnumerable<StudentResponse>>(students);
            return Ok(new PaginationResponse<IEnumerable<StudentResponse>>(totalRecord, studentsResponses));
        }

        //Get all students
        [HttpGet]
        public async Task<IActionResult> GetAllStudents([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<Student> students) = await _studentService.GetAllStudents(paginationParameter);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "No student found"));
            }
            IEnumerable<StudentInforResponse> studentsResponses = _mapper.Map<IEnumerable<StudentInforResponse>>(students);
            return Ok(new PaginationResponse<IEnumerable<StudentInforResponse>>(totalRecord, studentsResponses));
        }

        /// <summary>
        /// Get student list not in classModule (to add student to classModule)
        /// </summary>
        /// <param name="classModuleId"></param>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        [HttpGet("class/{classId:int}/module/{moduleId:int}/free")]
        public async Task<IActionResult> GetStudentsNotInClassModule(int classId, int moduleId, [FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<Student> students) = await _studentService.GetStudentsNotInClassModule(classId, moduleId, paginationParameter);
            IEnumerable<StudentResponse> studentsResponses = _mapper.Map<IEnumerable<StudentResponse>>(students);
            return Ok(new PaginationResponse<IEnumerable<StudentResponse>>(totalRecord, studentsResponses));
        }
        
        [HttpPost("ConvertExcelToEmailList")]
        public async Task<IActionResult> AssignClassByExcel([FromForm] IFormFile excelFile)
        {
            if (excelFile == null)
            {
                return Conflict(new ResponseDTO(409, "Excel file is null"));
            }
            var convertResult = await _studentService.ConvertExcelToStudentEmailList(excelFile);
            //item1 = list error; item2 = list email (su dung khi item1 length == 0)
            if (convertResult.Item1.Count > 0)
            {
                return BadRequest(convertResult.Item1);
            }
            return Ok(convertResult.Item2);
        }
    }
}