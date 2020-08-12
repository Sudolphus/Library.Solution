using System;

namespace Library.Models
{
  public class BookPatron
  {
    public int BookPatronId { get; set; }
    public int BookId { get; set; }
    public int PatronId { get; set; }
    public Book Book { get; set; }
    public Patron Patron { get; set; }
    public DateTime DueDate { get; set; }
    public bool Returned { get; set; }
  }
}