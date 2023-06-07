using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allog2405.Api.Entities;

public class Customer {
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id {get; set;}
    [Required]
    [MaxLength(50)]
    public string firstName {get; set;} = string.Empty;
    [Required]
    [MaxLength(50)]
    public string lastName {get; set;} = string.Empty;
    //[Required]
    [MaxLength(50)]
    public string cpf {get; set;} = string.Empty;
    public ICollection<Address> Addresses {get; set;} = new List<Address>(); // singleton address
}