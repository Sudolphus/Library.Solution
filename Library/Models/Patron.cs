using System.Collections.Generic;

namespace Library.Models
{
  public class Patron
  {
    public Patron()
    {
      this.Books = new HashSet<BookPatron>();
    }
    public int PatronId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set;}
    public string FullName { get; set; }
    public ApplicationUser User { get; set; }
    public ICollection<BookPatron> Books { get; set; }
  }
}