using Microsoft.AspNetCore.Mvc;
using MCBAAPI.Data;
using MCBAAPI.Models;
using SupportLibrary.ViewModels;

namespace MCBAAPI.Controllers;

[ApiController]
[Route("api/customer")]
public class CustomerController : ControllerBase
{
    private readonly DbAccess _dbAccess;

    public CustomerController(DbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    [HttpPost]
    public string Create(CustomerViewModel data)
    {
        return _dbAccess.Create(data.DeserialiseCustomer());
    }

    [HttpGet("{customerID}")]
    public CustomerViewModel Read(int customerID)
    {
        Customer data = new()
        {
            CustomerID = customerID
        };
        data = (Customer)_dbAccess.Read(data);
        CustomerViewModel customerViewModel = data.SerialiseCustomer(_dbAccess);
        return customerViewModel;
    }

    [HttpGet]
    public List<CustomerViewModel> ReadAll()
    {
        List<IModel> customerList = _dbAccess.ReadAll(new Customer());
        List<CustomerViewModel> customerViewModelList = new();
        foreach (Customer customer in customerList)
        {
            customerViewModelList.Add(customer.SerialiseCustomer(_dbAccess));
        }
        return customerViewModelList;
    }

    [HttpPut]
    public string Update(CustomerViewModel data)
    {
        return _dbAccess.Update(data.DeserialiseCustomer());
    }

    [HttpDelete]
    public string Delete(int customerID)
    {
        Customer data = new()
        {
            CustomerID = customerID
        };
        return _dbAccess.Delete(data);
    }
}
