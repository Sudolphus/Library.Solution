using System.Collections.Generic;

namespace Library.Models
{
  public class Author
  {
    public Author()
    {
      this.Books = new HashSet<AuthorBook>();
      FullName = FirstName + " " + LastName;
    }
    public int AuthorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; }
    public ICollection<AuthorBook> Books { get; set; }
  }
}