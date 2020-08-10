using System.Collections.Generic;

namespace Library.Models
{
  public class Book
  {
    public Book()
    {
      this.Authors = new HashSet<AuthorBook>();
      this.Patrons = new HashSet<BookPatron>();
    }
    public int BookId { get; set; }
    public string Title { get; set; }
    public int CopyId { get; set; }
    public 
    public ICollection<AuthorBook> Authors { get; set; }
    public ICollection<BookPatron> Patrons { get; set; }
  }
}