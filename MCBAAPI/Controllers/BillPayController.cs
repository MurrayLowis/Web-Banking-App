using Microsoft.AspNetCore.Mvc;
using MCBAAPI.Data;
using MCBAAPI.Models;
using SupportLibrary.ViewModels;
using MCBAAPI.Utilities;

namespace MCBAAPI.Controllers;

[ApiController]
[Route("api/billpay")]
public class BillPayController : ControllerBase
{
    private readonly DbAccess _dbAccess;

    public BillPayController(DbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    [HttpPost]
    public string Create(BillPayViewModel data)
    {
        return _dbAccess.Create(data.DeserialiseBillPay());
    }

    [HttpGet("{billPayId}")]
    public BillPayViewModel Read(int billPayId)
    {
        BillPay data = new()
        {
            BillPayID = billPayId
        };
        BillPay billPay = (BillPay)_dbAccess.Read(data);
        BillPayViewModel billPayViewModel = billPay.SerialiseBillPay();
        return billPayViewModel;
    }

    [HttpGet]
    public List<BillPayViewModel> ReadAll()
    {
        List<IModel> billPayList = _dbAccess.ReadAll(new BillPay());
        List<BillPayViewModel> billPayViewModelList = new();
        foreach (BillPay billPay in billPayList)
        {
            // don't return cancelled bill pays, or paid one-off bill pays
            if (!billPay.Cancelled && (billPay.Period == ID.MONTHLY || billPay.PaymentsDue != 0))
                billPayViewModelList.Add(billPay.SerialiseBillPay());
        }
        return billPayViewModelList;
    }

    [HttpPut]
    public string Update(BillPayViewModel data)
    {
        return _dbAccess.Update(data.DeserialiseBillPay());
    }

    [HttpDelete]
    public string Delete(int billPayID)
    {
        BillPay data = new()
        {
            BillPayID = billPayID
        };
        return _dbAccess.Delete(data);
    }
}
