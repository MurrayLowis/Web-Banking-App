using Microsoft.AspNetCore.Mvc;
using MCBAAPI.Data;
using MCBAAPI.Models;
using SupportLibrary.ViewModels;

namespace MCBAAPI.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly DbAccess _dbAccess;

    public AccountController(DbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    [HttpPost]
    public string Create(AccountViewModel data)
    {
        return _dbAccess.Create(data.DeserialiseAccount());
    }

    [HttpGet("{accountNumber}")]
    public AccountViewModel Read(int accountNumber)
    {
        Account data = new()
        {
            AccountNumber = accountNumber
        };
        Account account = (Account)_dbAccess.Read(data);
        AccountViewModel accountViewModel = account.SerialiseAccount(_dbAccess);
        return accountViewModel;
    }

    [HttpGet]
    public List<AccountViewModel> ReadAll()
    {
        List<IModel> accountList = _dbAccess.ReadAll(new Account());
        List<AccountViewModel> accountViewModelList = new();
        foreach (Account account in accountList)
        {
            accountViewModelList.Add(account.SerialiseAccount(_dbAccess));
        }
        return accountViewModelList;
    }

    [HttpPut]
    public string Update(AccountViewModel data)
    {
        return _dbAccess.Update(data.DeserialiseAccount());
    }

    [HttpDelete]
    public string Delete(int accountNumber)
    {
        Account data = new()
        {
            AccountNumber = accountNumber
        };
        return _dbAccess.Delete(data);
    }
}
