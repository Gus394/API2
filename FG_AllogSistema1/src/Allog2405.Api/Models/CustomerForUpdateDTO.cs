using System.ComponentModel.DataAnnotations;
namespace Allog2405.Api.Models;

public class CustomerForUpdateDTO : CustomerForManipulationDTO
{
    [Required(ErrorMessage = "You should fill out a ID")]
    public int id {get; set;}
}