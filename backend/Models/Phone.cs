using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PhoneBook.Api.Models;

[Table("PHONE")]
public class Phone
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("CONTACT_ID")]
    public int ContactId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("PHONE_NUMBER")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Column("CREATED_AT")]
    public DateTime CreatedAt { get; set; }

    [JsonIgnore]
    [ForeignKey("ContactId")]
    public Contact? Contact { get; set; }
}
