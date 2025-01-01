using System.ComponentModel.DataAnnotations.Schema; // To use [Column].

namespace Northwind.EntityModels

{
public class Category
{
    // These properties map to columns in the database.
public int CategoryId { get; set; } // The primary key.
public string CategoryName { get; set; } = null!;
[Column(TypeName = "ntext")]
public string? Description { get; set; }
// Defines a navigation property for related rows.
public virtual ICollection<Product> Products { get; set; }

= new HashSet<Product>();
}
}