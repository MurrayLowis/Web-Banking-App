using Microsoft.AspNetCore.Mvc;
using MCBAAPI.Data;
using MCBAAPI.Models;
using SupportLibrary.ViewModels;

namespace MCBAAPI.Controllers;

[ApiController]
[Route("api/transaction")]
public class TransactionController : ControllerBase
{
    private readonly DbAccess _dbAccess;

    public TransactionController(DbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    [HttpPost]
    public string Create(TransactionViewModel data)
    {
        return _dbAccess.Create(data.DeserialiseTransaction());
    }

    [HttpGet("{transactionId}")]
    public TransactionViewModel Read(int transactionId)
    {
        Transaction data = new()
        {
            TransactionID = transactionId
        };
        Transaction transaction = (Transaction)_dbAccess.Read(data);
        TransactionViewModel transactionViewModel = transaction.SerialiseTransaction();
        return transactionViewModel;
    }

    [HttpGet]
    public List<TransactionViewModel> ReadAll()
    {
        List<IModel> transactionList = _dbAccess.ReadAll(new Transaction());
        List<TransactionViewModel> transactionViewModelList = new();
        foreach (Transaction transaction in transactionList)
        {
            transactionViewModelList.Add(transaction.SerialiseTransaction());
        }
        return transactionViewModelList;
    }

    [HttpPut]
    public string Update(TransactionViewModel data)
    {
        return _dbAccess.Update(data.DeserialiseTransaction());
    }

    [HttpDelete]
    public string Delete(int transactionId)
    {
        Transaction data = new()
        {
            TransactionID = transactionId
        };
        return _dbAccess.Delete(data);
    }
}
