using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTO.AccountDTO;
using examedu.DTO.AccountDTO;
using examedu.DTO.ExcelDTO;
using ExamEdu.DTO.PaginationDTO;
using Microsoft.AspNetCore.Http;

namespace examedu.Services.Account
{
    public interface IAccountService
    {
        Tuple<int, IEnumerable<AccountResponse>> GetAccountList(PaginationParameter paginationParameter);
        Tuple<int, IEnumerable<AccountResponse>> GetDeactivatedAccountList(PaginationParameter paginationParameter);
        Task<int> InsertNewAccount(AccountInput accountInput);
        Task<int> DeactivateAccount(int id, int roleID);
        Task<Tuple<AccountResponse, string>> GetAccountByEmail(string email);
        Task<string> GetRoleName(int id);
        Task<AccountResponse> GetAccountInforByEmail(string email);
        Task<int> UpdateAccount(UpdateAccountInput accountInput, int roleId, string currEmail);
        Task<Tuple<List<CellErrorInfor>, List<AccountInput>>> convertExcelToAccountInputList(IFormFile excelFile);
        Task<Tuple<int, List<CellErrorInfor>>> InsertListAccount(List<AccountInput> listAccount);
        Task<int> UpdatePasswordAndSendEmail(string email);
    }
}