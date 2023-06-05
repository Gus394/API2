using AutoMapper;
namespace Allog2405.Api.Profiles;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        // o primeiro eh quem o objeto eh. o segundo eh quem ele quer se tornar
        CreateMap<Entities.Customer, Models.CustomerDTO>();
    }
}