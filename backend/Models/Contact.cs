using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneBook.Api.Models;

[Table("CONTACT")]
public class Contact
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("NAME")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, 149)]
    [Column("AGE")]
    public int Age { get; set; }

    [Column("CREATED_AT")]
    public DateTime CreatedAt { get; set; }

    [Column("UPDATED_AT")]
    public DateTime UpdatedAt { get; set; }

    public ICollection<Phone> Phones { get; set; } = new List<Phone>();
}
