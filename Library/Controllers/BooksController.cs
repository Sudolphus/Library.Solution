using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
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

    public ActionResult Index(string title)
    {
      IQueryable<Book> bookQuery = _db.Books
        .Include(books => books.Authors)
        .ThenInclude(join => join.Author);
      if (!string.IsNullOrEmpty(title))
      {
        Regex titleSearch = new Regex(title, RegexOptions.IgnoreCase);
        bookQuery = bookQuery.Where(books => titleSearch.IsMatch(books.Title));
      }
      IEnumerable<Book> bookList = bookQuery
        .ToList()
        .OrderBy(books => books.Title);
      return View(bookList);
    }

    public ActionResult Create()
    {
      ViewBag.AuthorId = new SelectList(_db.Authors, "AuthorId", "FullName");
      return View();
    }

    [HttpPost]
    public ActionResult Create(Book book, string AuthorId)
    {
      _db.Books.Add(book);
      int parseId = int.Parse(AuthorId);
      if (parseId != 0)
      {
        _db.AuthorBook.Add(new AuthorBook() { BookId = book.BookId, AuthorId = parseId });
      }
      book.Number = 1;
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = book.BookId });
    }

    public ActionResult Details(int id)
    {
      Book book = _db.Books
        .Include(books => books.Authors)
        .ThenInclude(join => join.Author)
        .First(b => b.BookId == id);
      return View(book);
    }

    public ActionResult Edit(int id)
    {
      Book book = _db.Books.First(books => books.BookId == id);
      ViewBag.AuthorId = new SelectList(_db.Authors, "AuthorId", "FullName");
      return View(book);
    }

    [HttpPost]
    public ActionResult Edit(Book book, int AuthorId)
    {
      if (AuthorId != 0)
      {
        _db.AuthorBook.Add(new AuthorBook() { BookId = book.BookId, AuthorId = AuthorId });
      }
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

    [HttpPost]
    public ActionResult Checkout(Book book, Patron patron)
    {
      if (book.Number == 0)
      {
        return RedirectToAction("Details", new { id = book.BookId });
      }
      book.Number--;
      DateTime current = DateTime.Now;
      DateTime due = current.Add(new TimeSpan(14, 0, 0, 0));
      _db.BookPatron.Add(new BookPatron(){BookId = book.BookId, PatronId = patron.PatronId, Returned = false, DueDate = due});
      _db.SaveChanges();
      return RedirectToAction("Details", "Patrons", new { id = patron.PatronId });
    }

    [HttpPost]
    public ActionResult Checkin(BookPatron checkoutRecord)
    {
      checkoutRecord.Returned = true;
      checkoutRecord.Book.Number++;
      _db.SaveChanges();
      return RedirectToAction("Details", "Patrons", new { id = checkoutRecord.Patron.PatronId });
    }
  }
}