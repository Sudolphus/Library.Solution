using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Claims;
using Library.Models;


namespace Library.Controllers
{
  [Authorize]
  public class PatronsController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public PatronsController(LibraryContext db, UserManager<ApplicationUser> userManager)
    {
      _db = db;
      _userManager = userManager;
    }

    // public ActionResult Index(string name)
    // {
    //   IQueryable<Patron> patronQuery = _db.Patrons
    //     .Include(p => p.Books)
    //     .ThenInclude(join => join.Book);
    //   if (!string.IsNullOrEmpty(name))
    //   {
    //     Regex patronSearch = new Regex(name, RegexOptions.IgnoreCase);
    //     patronQuery = patronQuery.Where(p => patronSearch.IsMatch(p.FullName));
    //   }
    //   IEnumerable<Patron> patronList = patronQuery
    //     .ToList()
    //     .OrderBy(p => p.LastName)
    //     .ThenBy(p => p.FirstName);
    //   return View(patronList);
    // }
    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      return RedirectToAction("Details", new { id = currentUser.Id });
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Patron patron)
    {
      _db.Patrons.Add(patron);
      patron.FullName = patron.FirstName + " " + patron.LastName;
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = patron.PatronId });
    }

    public ActionResult Details(int id)
    {
      Patron patron = _db.Patrons
        .Include(patrons => patrons.Books)
        .ThenInclude(join => join.Book)
        .First(patrons => patrons.PatronId == id);
      IEnumerable<BookPatron> booksCheckedOut = patron.Books
        .Where(books => books.Returned == false)
        .OrderBy(books => books.DueDate)
        .ThenBy(books => books.Book.Title);
      IEnumerable<BookPatron> bookHistory = patron.Books
        .Where(books => books.Returned == true)
        .OrderBy(books => books.DueDate)
        .ThenBy(books => books.Book.Title);
      ViewBag.CheckoutCount = booksCheckedOut.Count();
      ViewBag.HistoryCount = bookHistory.Count();
      ViewBag.Checkouts = booksCheckedOut;
      ViewBag.History = bookHistory;
      return View(patron);
    }

    public ActionResult Edit(int id)
    {
      Patron patron = _db.Patrons.First(p => p.PatronId == id);
      return View(patron);
    }

    [HttpPost]
    public ActionResult Edit(Patron patron)
    {
      patron.FullName = patron.FirstName + " " + patron.LastName;
      _db.Entry(patron).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = patron.PatronId });
    }

    public ActionResult Delete(int id)
    {
      Patron patron = _db.Patrons.First(p => p.PatronId == id);
      return View(patron);
    }

    [HttpPost]
    public ActionResult Delete(Patron patron)
    {
      _db.Patrons.Remove(patron);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}