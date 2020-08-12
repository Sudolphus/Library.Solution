using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Library.Models;

namespace Library.Controllers
{
  public class BooksController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public BooksController(LibraryContext db, UserManager<ApplicationUser> userManager)
    {
      _db = db;
      _userManager = userManager;
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

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Checkout(Book book)
    {
      var currentUser = await _userManager.FindByIdAsync(this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
      if (book.Number == 0)
      {
        return RedirectToAction("Details", new { id = book.BookId });
      }
      book.Number--;
      _db.Entry(book).State = EntityState.Modified;
      DateTime current = DateTime.Now;
      DateTime due = current.Add(new TimeSpan(14, 0, 0, 0));
      Patron patron = _db.Patrons.First(p => p.User == currentUser);
      _db.BookPatron.Add(new BookPatron(){BookId = book.BookId, PatronId = patron.PatronId, Returned = false, DueDate = due});
      _db.SaveChanges();
      return RedirectToAction("Details", "Patrons", new { id = patron.PatronId });
    }

    [Authorize]
    [HttpPost]
    public ActionResult Checkin(BookPatron checkoutRecord)
    {
      checkoutRecord.Returned = true;
      checkoutRecord.Book.Number++;
      _db.SaveChanges();
      return RedirectToAction("Details", "Patrons", new { id = checkoutRecord.Patron.PatronId });
    }

    [Authorize]
    public ActionResult Overdue()
    {
      DateTime current = DateTime.Now;
      IEnumerable<BookPatron> overdueBooks = _db.BookPatron
        .Where(checkout => checkout.Returned == false)
        .Where(checkout => checkout.DueDate < current)
        .Include(checkout => checkout.Book)
        .Include(checkout => checkout.Patron)
        .ToList()
        .OrderBy(checkout => checkout.DueDate);
      return View(overdueBooks);
    }
  }
}