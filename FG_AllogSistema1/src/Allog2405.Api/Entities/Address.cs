using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allog2405.Api.Entities;

public class Address
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get; set;}
    
    [Required]
    [MaxLength(50)]
    public string Street {get; set;} = string.Empty;

    [Required]
    [MaxLength(50)]
    public string City {get; set;} = string.Empty;
    
    [ForeignKey("CustomerId")]
    public Customer? Customer {get; set;} // propriedade de navegacao
    public int CustomerId {get; set;}
    
}