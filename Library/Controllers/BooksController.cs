using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Models;

namespace Library.Controllers
{
  public class BooksController : Controller
  {
    private readonly LibraryContext _db;

    public BooksController(LibraryContext db)
    {
      _db = db;
    }

    public ActionResult Index(string title, string author)
    {
      IQueryable<Book> bookQuery = _db.Books
        .Include(books => books.Authors)
        .ThenInclude(join => join.Author);
      if (!string.IsNullOrEmpty(title))
      {
        Regex titleSearch = new Regex(title, RegexOptions.IgnoreCase);
        bookQuery = bookQuery.Where(books => titleSearch.IsMatch(books.Title));
      }
      if (!string.IsNullOrEmpty(author))
      {
        Regex authorSearch = new Regex(author, RegexOptions.IgnoreCase);
        bookQuery = bookQuery.Where(books => authorSearch.IsMatch(books.Authors.Author.FirstName) || authorSearch.IsMatch(books.Authors.Author.LastName));
      }
      IEnumerable<Book> bookList = bookQuery.ToList().OrderBy(books => books.Title);
      return View(bookList);
    }

    public ActionResult Create()
    {
      ViewBag.AuthorId = new MultiSelectList(_db.Authors, "AuthorId", "FullName");
      return View();
    }

    [HttpPost]
    public ActionResult Create(Book book)
    {
      _db.Books.Add(book);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = book.BookId });
    }

    public ActionResult Details(int id)
    {
      Book book = _db.Books
        .Include(books => books.Authors)
        .ThenInclude(join => join.Author)
        .Include(books => books.Copies);
      return View(book);
    }

    public ActionResult Edit(int id)
    {
      Book book = _db.Books.First(books => books.BookId == id);
      ViewBag.AuthorId = new MultiSelectList(_db.Authors, "AuthorId", "FullName");
      return View(book);
    }

    [HttpPost]
    public ActionResult Edit(Book book)
    {
      _db.Entry(book).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = book.BookId });
    }


    public ActionResult Delete(int id)
    {
      Book book = _db.Books.First(books => books.BookId == id);
      return View(book);
    }

    [HttpPost]
    public ActionResult Delete(Book book)
    {
      _db.Books.Remove(book);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}