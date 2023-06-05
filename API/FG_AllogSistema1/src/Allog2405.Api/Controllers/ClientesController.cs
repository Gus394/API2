using System.Text.RegularExpressions;
using Allog2405.Api.Models;
using Allog2405.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using AutoMapper;

namespace Allog2405.Api.Controllers;

[ApiController]
[Route("api/Customers")]
public class CustomersController : ControllerBase {

    private readonly CustomerData _data;
    private readonly IMapper _mapper;
    public CustomersController(CustomerData data, IMapper mapper) // ?
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    public ActionResult<IEnumerable<CustomerDTO>> GetCustomers()
    {
        var customerFromDatabase = _data.listaCustomers;
        var customersToReturn = _mapper.Map<IEnumerable<CustomerDTO>>(customerFromDatabase);
        return Ok(customersToReturn);
    }

    [HttpGet("{id:int:min(1)}", Name = "GetCustomerPorId")]
    public ActionResult<CustomerDTO> GetCustomerPorId(int id) {
        var Customer = _data.listaCustomers.FirstOrDefault(c => c.id == id);
        if (Customer == null)
        {
            return NotFound();
        }

        CustomerDTO CustomerResult = new CustomerDTO(Customer);
        return Ok(CustomerResult);
    }

    [HttpGet("cpf/{cpf}")]
    public ActionResult<CustomerDTO> GetCustomerPorCpf(string cpf) {
        var Customer = _data.listaCustomers.FirstOrDefault(c => c.cpf == cpf);
        if(Customer == null) return NotFound();

        CustomerDTO CustomerResult = new CustomerDTO(Customer);
        return Ok(CustomerResult);
    }

    [HttpPost]
    public ActionResult<CustomerDTO> CreateCustomer(CustomerForCreationDTO CustomerBody)
    {
        // configurar isso de forma global
        // URM mapeamento entidade bancod dados fsdfsgdfg
        // customer.addresses.add()
        // select many

        if (!ModelState.IsValid)
        {
            Response.ContentType = "application/problem+json";
            var problemDetailsFactory = HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var ValidationProblemDetails = problemDetailsFactory.CreateValidationProblemDetails(HttpContext, ModelState);
            ValidationProblemDetails.Status = StatusCodes.Status422UnprocessableEntity;
            return UnprocessableEntity(ValidationProblemDetails);
        }
        
        var CustomerEntity = new Customer()
        {
            id = _data.listaCustomers.Max(c => c.id) + 1,
            firstName = CustomerBody.FirstName,
            lastName = CustomerBody.LastName,
            cpf = CustomerBody.Cpf
        };

        _data.listaCustomers.Add(CustomerEntity);

        CustomerDTO CustomerToReturn = new CustomerDTO()
        {
            id = CustomerEntity.id,
            FirstName = CustomerEntity.firstName,
            LastName = CustomerEntity.lastName,
            CPF = CustomerEntity.cpf
        };

        return CreatedAtRoute(
            "GetCustomerPorId",
            new {id = CustomerToReturn.id},
            CustomerToReturn
        );
    }

    [HttpPut("{id}")]
    public ActionResult UpdateCustomer(int id, CustomerForUpdateDTO customerForUpdateDTO)
    {
        if (id != customerForUpdateDTO.id) return BadRequest();
        var customerFromDatabase = _data.listaCustomers.FirstOrDefault(customer => customer.id == id);
        if (customerFromDatabase == null) return NotFound();
        
        _mapper.Map(customerForUpdateDTO, customerFromDatabase);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteCustomer(int id)
    {
        var customerFromDatabase = _data.listaCustomers.FirstOrDefault(customer => customer.id == id);
        if (customerFromDatabase == null) return NotFound();
        _data.listaCustomers.Remove(customerFromDatabase);

        return NoContent();
    }

    [HttpPatch("{id}")]
    public ActionResult PartiallyUpdateCustomer(
        [FromBody] JsonPatchDocument<CustomerForPatchDTO> patchDocument,
        [FromRoute] int id)
        {
            var customerFromDatabase = _data.listaCustomers.FirstOrDefault(customer => customer.id == id);
            if (customerFromDatabase == null) return NotFound();
            var customerToPatch = new CustomerForPatchDTO
            {
                FirstName = customerFromDatabase.firstName,
                LastName = customerFromDatabase.lastName,
                Cpf = customerFromDatabase.cpf
            };
            patchDocument.ApplyTo(customerToPatch);

            customerFromDatabase.firstName = customerToPatch.FirstName;
            customerFromDatabase.lastName = customerToPatch.LastName;
            customerFromDatabase.cpf = customerToPatch.Cpf;

            return NoContent();
        }

    [HttpGet("with-address", Name = "GetCustomersWithAddresses")]
    public ActionResult<IEnumerable<CustomerWithAddressDTO>> GetCustomersWithAddresses()
    // basicamente a gente ja pegou o dado do banco de dados e eu to dando um select pq eu quero transformar o meu customer q e uma entidade em um dto
    // em qual dto: customerwithaddressdto, entao o select vai percorrer essa lista e ele vai me retornar a cada iteracao esse objeto ja mapeado isso se chama mapeamento
    // mesma coisa com a entidade. Serializacao e tal sei la
    {
        var customerFromDatabase = _data.listaCustomers;
        var customersToReturn = customerFromDatabase.Select(customer => new CustomerWithAddressDTO{
            id = customer.id,
            FirstName = customer.firstName,
            LastName = customer.lastName,
            Cpf = customer.cpf,
            Addresses = customer.Addresses.Select(address => new AddressDTO{
                Id = address.Id,
                City = address.City,
                Street = address.Street
            }).ToList() // para resolver o problema de conversao immplicita, pegou o icollection e "forcou" com c cedilha
        });
        return Ok(customersToReturn);
    }

    [HttpPost("with-address")]
    public ActionResult<CustomerDTO> CreateCustomerWithMultipleAddresses(CustomerWithAddressDTO cus)
    {
        int idMax = _data.listaCustomers.SelectMany(c => c.Addresses).Max(a => a.Id) + 1; // para nao ficar o mesmo id em casos de insercoes multiplas
        var CustomerEntity = new Customer()
        {
            id = _data.listaCustomers.Max(c => c.id) + 1,
            firstName = cus.FirstName,
            lastName = cus.LastName,
            cpf = cus.Cpf,
            Addresses = cus.Addresses.Select(a => new Address
            {
                Id = idMax++,
                Street = a.Street,
                City = a.City
            }).ToList()
        };

        /*var CustomerToReturn = new CustomerWithAddressDTO
        {
            id = CustomerEntity.id,
            FirstName = CustomerEntity.firstName,
            LastName = CustomerEntity.lastName,
            Cpf = CustomerEntity.cpf,
            Addresses = CustomerEntity.Addresses
        }*/

        _data.listaCustomers.Add(CustomerEntity);

        return CreatedAtRoute(
            "GetCustomersWithAddresses",
            new {},
            CustomerEntity
        );
    }

    [HttpPut("with-address/{customerID}")]
    public ActionResult UpdateWithAddresses(int customerID, CustomerWithAddressDTO cus)
    {
        int idMax = _data.listaCustomers.SelectMany(c => c.Addresses).Max(a => a.Id) + 1; // para nao ficar o mesmo id em casos de insercoes multiplas
        
        var customerFromDatabase = _data.listaCustomers.FirstOrDefault(c => c.id == customerID);
        if (customerFromDatabase == null) return NotFound();

        customerFromDatabase.firstName = cus.FirstName;
        customerFromDatabase.lastName = cus.LastName;
        customerFromDatabase.cpf = cus.Cpf;
        customerFromDatabase.Addresses = cus.Addresses.Select(a =>
        new Address{
            Id = idMax++,
            Street = a.Street,
            City = a.City
        }).ToList();

        return NoContent();
    }
    
}

// Customer Icollection<address> adresses = new list<adress>();

// existem situacoes em que nao e necessario retornar todos os dados apenas algum por exemplo so os enderecos
// vai mandar a id pesquisar e retornar da api somente os enderecos
// somente retornar os enderecos.