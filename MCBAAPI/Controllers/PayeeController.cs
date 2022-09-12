using Microsoft.AspNetCore.Mvc;
using MCBAAPI.Data;
using MCBAAPI.Models;
using SupportLibrary.ViewModels;

namespace MCBAAPI.Controllers;

[ApiController]
[Route("api/payee")]
public class PayeeController : ControllerBase
{
    private readonly DbAccess _dbAccess;

    public PayeeController(DbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    [HttpPost]
    public string Create(PayeeViewModel data)
    {
        return _dbAccess.Create(data.DeserialisePayee());
    }

    [HttpGet("{payeeID}")]
    public PayeeViewModel Read(int payeeID)
    {
        Payee data = new()
        {
            PayeeID = payeeID
        };
        Payee payee = (Payee)_dbAccess.Read(data);
        PayeeViewModel payeeViewModel = payee.SerialisePayee();
        return payeeViewModel;
    }

    [HttpGet]
    public List<PayeeViewModel> ReadAll()
    {
        List<IModel> payeeList = _dbAccess.ReadAll(new Payee());
        List<PayeeViewModel> payeeViewModelList = new();
        foreach (Payee payee in payeeList)
        {
            payeeViewModelList.Add(payee.SerialisePayee());
        }
        return payeeViewModelList;
    }

    [HttpPut]
    public string Update(PayeeViewModel data)
    {
        return _dbAccess.Update(data.DeserialisePayee());
    }

    [HttpDelete]
    public string Delete(int payyeID)
    {
        Payee data = new()
        {
            PayeeID = payyeID
        };
        return _dbAccess.Delete(data);
    }
}
