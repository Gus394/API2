using Allog2405.Api.Models;
using Microsoft.AspNetCore.Mvc;
namespace Allog2405.Api.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Allog2405.Api.Entities;

[ApiController]
[Route("api/customers/{customerID}/addresses")]
public class AddressController : ControllerBase
{
    private readonly CustomerData _data;
    public AddressController(CustomerData data) // ?
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<AddressDTO>> GetAddresses(int customerID)
        {
            var customerFromDatabase = _data.listaCustomers.FirstOrDefault(customer => customer.id == customerID);
            if (customerFromDatabase == null) return NotFound();
            var addressesToReturn = new List<AddressDTO>();

            foreach (var address in customerFromDatabase.Addresses)
            {
                addressesToReturn.Add(new AddressDTO{
                    Id = address.Id,
                    Street = address.Street,
                    City = address.City
                });
            }
            return Ok(addressesToReturn);
        }

    // n ta transformando em dto
    [HttpGet("{addressID}", Name = "GetAdress")]
    public ActionResult<AddressDTO> GetAdress(int customerID, int addressID)
    {
        var addressesToReturn = _data.listaCustomers.FirstOrDefault(customer => customer.id == customerID)?.Addresses.FirstOrDefault(
            address => address.Id == addressID); // "?" para ver se n e null sei la
        return addressesToReturn != null ? Ok(addressesToReturn) : NotFound();
    }

    [HttpPost]
    public ActionResult<AddressDTO> CreateAddress(int customerID, AddressForCreationDTO AddressBody)
    {
        var customerToReceiveAddress = _data.listaCustomers.FirstOrDefault(c => c.id == customerID);
        if (customerToReceiveAddress == null) return NotFound();
        
        /*if (!ModelState.IsValid)
        {
            Response.ContentType = "application/problem+json";
            var problemDetailsFactory = HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var ValidationProblemDetails = problemDetailsFactory.CreateValidationProblemDetails(HttpContext, ModelState);
            ValidationProblemDetails.Status = StatusCodes.Status422UnprocessableEntity;
            return UnprocessableEntity(ValidationProblemDetails);
        }*/

        var AddressEntity = new Address()
        {
            Id = _data.listaCustomers.SelectMany(c => c.Addresses).Max(i => i.Id) + 1,
            Street = AddressBody.Street,
            City = AddressBody.City
        };

        customerToReceiveAddress.Addresses.Add(AddressEntity);
        return CreatedAtRoute(
            "GetAddress",
            new {customerID = AddressEntity.Id, addressID = AddressEntity.Id},
            AddressEntity
        );
    }

    //[HttpPost]
    //public ActionResult<IEnumerable<AddressDTO>> CreateMultipleAddresses(int customerID)

    [HttpPut("{addressID}")]
    public ActionResult UpdateAddress(int addressID, int customerID, AddressForUpdateDTO AddressForUpdate)
    {
        if (AddressForUpdate.Id != addressID) return BadRequest();
        var addressFromDatabase = _data.listaCustomers.FirstOrDefault(c => c.id == customerID).Addresses.FirstOrDefault(a => a.Id == customerID);
        if (addressFromDatabase == null) return NotFound();

        addressFromDatabase.Street = AddressForUpdate.Street;
        addressFromDatabase.City = AddressForUpdate.City;
        return NoContent();
    }

    [HttpDelete("{addressID}")]
    public ActionResult DeleteAddress(int addressID, int customerID) // nome dos parametros tem q ser igual ao da rota
    {
        var customerFromDatabase = _data.listaCustomers.FirstOrDefault(c => c.id == customerID);
        if (customerFromDatabase == null) return NotFound();
        var addressFromDatabase = customerFromDatabase.Addresses.FirstOrDefault(a => a.Id == addressID);
        if (addressFromDatabase == null) return NotFound();

        customerFromDatabase.Addresses.Remove(addressFromDatabase);
        return NoContent();
    }

}

