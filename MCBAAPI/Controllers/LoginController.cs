using Microsoft.AspNetCore.Mvc;
using MCBAAPI.Data;
using MCBAAPI.Models;
using SupportLibrary.ViewModels;
using SimpleHashing;

namespace MCBAAPI.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{
    private readonly DbAccess _dbAccess;

    public LoginController(DbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    [HttpPost]
    public string Create(LoginViewModel data)
    {
        return _dbAccess.Create(data.DeserialiseLogin());
    }

    // login takes multiple arguments so it can verify
    [HttpGet("{loginID}/{password}")]
    public LoginViewModel Read(string loginID, string password)
    {
        Login data = new()
        {
            LoginID = loginID
        };
        data = (Login)_dbAccess.Read(data);
        var loginViewModel = new LoginViewModel();
        // if input hash matches database hash return success, otherwise return empty
        if (PBKDF2.Verify(data.PasswordHash, password))
        {
            loginViewModel.CustomerID = data.CustomerID;
        }
        return loginViewModel;
    }

    [HttpGet]
    public List<LoginViewModel> ReadAll()
    {
        List<IModel> loginList = _dbAccess.ReadAll(new Login());
        List<LoginViewModel> loginViewModelList = new();
        foreach (Login login in loginList)
        {
            loginViewModelList.Add(login.SerialiseLogin());
        }
        return loginViewModelList;
    }

    [HttpPut]
    public string Update(LoginViewModel data)
    {
        return _dbAccess.Update(data.DeserialiseLogin());
    }

    [HttpDelete]
    public string Delete(string loginId)
    {
        Login data = new()
        {
            LoginID = loginId
        };
        return _dbAccess.Delete(data);
    }
}
