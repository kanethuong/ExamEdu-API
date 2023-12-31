using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DTO.AccountDTO;
using BackEnd.Services;
using examedu.DTO.AccountDTO;
using examedu.DTO.ExcelDTO;
using examedu.Services;
using examedu.Services.Account;
using ExamEdu.DB.Models;
using ExamEdu.DTO;
using ExamEdu.DTO.PaginationDTO;
using ExamEdu.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace examedu.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly IStudentService _studentService;
        private readonly ITeacherService _teacherService;
        private readonly IAcademicDepartmentService _academicDepartmentService;
        private readonly IAdministratorService _administratorService;

        public AccountController(IAccountService accountService, IMapper mapper, IStudentService studentService, ITeacherService teacherService, IAcademicDepartmentService academicDepartmentService, IAdministratorService administratorService)
        {
            _accountService = accountService;
            _mapper = mapper;
            _studentService = studentService;
            _teacherService = teacherService;
            _academicDepartmentService = academicDepartmentService;
            _administratorService = administratorService;
        }

        /// <summary>
        /// Get the list of account in the db with pagination config
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of account with pagination / 404: search username not found</returns>
        [HttpGet("list")]
        public ActionResult<PaginationResponse<IEnumerable<AccountResponse>>> GetAccountList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<AccountResponse> listAccount) = _accountService.GetAccountList(paginationParameter);

            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Search email not found"));
            }

            return Ok(new PaginationResponse<IEnumerable<AccountResponse>>(totalRecord, listAccount));
        }

        /// <summary>
        /// Get the list of account in the db with pagination config
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of account with pagination / 404: search username not found</returns>
        [HttpGet("deactivatedList")]
        public ActionResult<PaginationResponse<IEnumerable<AccountResponse>>> GetDeactivatedAccountList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<AccountResponse> listAccount) = _accountService.GetDeactivatedAccountList(paginationParameter);

            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Search email not found"));
            }

            return Ok(new PaginationResponse<IEnumerable<AccountResponse>>(totalRecord, listAccount));
        }

        /// <summary>
        /// Insert new account to db, send email with password generated by system to that user
        /// </summary>
        /// <param name="accountInput">Detail of account</param>
        /// <returns>201: Created / 409: Username || email || phone is existed</returns>
        [HttpPost]
        public async Task<ActionResult> CreateNewAccount([FromBody] AccountInput accountInput)
        {
            int addResult = await _accountService.InsertNewAccount(accountInput);
            if (addResult == 0)
            {
                return Conflict(new ResponseDTO(409, "Email is already existed"));
            }
            if (addResult == -1)
            {
                return BadRequest(new ResponseDTO(400, "Something went wrong. Please try again"));
            }
            return CreatedAtAction(nameof(GetAccountList), new ResponseDTO(201, "Successfully inserted"));
        }
        [HttpPost("excel")]
        public async Task<ActionResult> CreateNewAccountByExcel([FromForm] IFormFile excelFile, [FromForm] int roleId)
        {
            var convertResult = await _accountService.convertExcelToAccountInputList(excelFile);
            //item1 = list error; item2 = list account (su dung khi item1 length == 0)
            if (convertResult.Item1.Count > 0)
            {
                return BadRequest(convertResult.Item1);
            }

            foreach (var account in convertResult.Item2)
            {
                account.RoleID = roleId;
            }
            var insertResult = await _accountService.InsertListAccount(convertResult.Item2);
            if (insertResult.Item2.Count > 0)
            {
                return BadRequest(insertResult.Item2);
            }
            if (insertResult.Item1 == convertResult.Item2.Count)
            {
                return Ok(new ResponseDTO(201, "Successfully inserted"));
            }
            return BadRequest(new ResponseDTO(400, "Error when inserted, upload again for more detail"));
        }

        /// <summary>
        /// Deactivate an account in the db
        /// </summary>
        /// <param name="id">Id of that account</param>
        /// <param name="role">Role of that account</param>
        /// <returns>200: Deleted / 404: Id not found</returns>
        [HttpDelete("{id:int}/{role:int}")]
        public async Task<ActionResult> DeactivateAccount(int id, int role)
        {
            int result = await _accountService.DeactivateAccount(id, role);

            if (result == -1)
            {
                return NotFound(new ResponseDTO(404, "ID not found!"));
            }
            if (result == 0)
            {
                return BadRequest(new ResponseDTO(400, "Deactivate failed"));
            }
            return Ok(new ResponseDTO(200, "Account has been deactived successfully!"));
        }

        [HttpGet]
        public async Task<ActionResult<AccountResponse>> GetAccountInforByEmail(string email)
        {
            AccountResponse accountResponse = await _accountService.GetAccountInforByEmail(email);
            if (accountResponse == null)
            {
                return NotFound(new ResponseDTO(404, "Account not exist"));
            }
            return Ok(accountResponse);
        }

        [HttpPut("Update")]
        public async Task<ActionResult> UpdateAccount(int roleId, string currentEmail, [FromBody] UpdateAccountInput accountInput)
        {
            var existedAccount = await _accountService.GetAccountInforByEmail(currentEmail);
            if (existedAccount == null)
            {
                return NotFound(new ResponseDTO(404, "Account not exist"));
            }
            if (
                accountInput.Email.ToLower().Equals(existedAccount.Email.ToLower()) &&
                accountInput.Fullname.ToLower().Equals(existedAccount.Fullname.ToLower())
            )
            {
                return Ok(new ResponseDTO(200, "Update account success"));
            }

            int rs = await _accountService.UpdateAccount(accountInput, roleId, currentEmail);
            if (rs == -1)
            {
                return NotFound(new ResponseDTO(404, "Account not exist"));
            }
            else if (rs == -2)
            {
                return Conflict(new ResponseDTO(409, "Email is already existed"));
            }
            else if (rs == 0)
            {
                return BadRequest(new ResponseDTO(400, "Something went wrong. Update account failed"));
            }
            else
            {
                return Ok(new ResponseDTO(200, "Update account success"));
            }
        }
    }
}