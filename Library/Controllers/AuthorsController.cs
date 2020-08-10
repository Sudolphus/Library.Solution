using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Models;

namespace Library.Controllers
{
  public class AuthorsController : Controller
  {
    private readonly LibraryContext _db;
    public AuthorsController(LibraryContext db)
    {
      _db = db;
    }

    public ActionResult Index(string name)
    {
      IQueryable<Author> authorQuery = _db.Authors;
      if (!string.IsNullOrEmpty(name))
      {
        Regex authorSearch = new Regex(name, RegexOptions.IgnoreCase);
        authorQuery = authorQuery.Where(authors => authorSearch.IsMatch(authors.FullName));
      }
      IEnumerable<Author> authorList = authorQuery
        .ToList()
        .OrderBy(authors => authors.LastName)
        .ThenBy(authors => authors.FirstName);
      return View(authorList);
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Author author)
    {
      _db.Authors.Add(author);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = author.AuthorId});
    }

    public ActionResult Details(int id)
    {
      Author author = _db.Authors
        .Include(authors => authors.Books)
        .ThenInclude(join => join.Book)
        .First(authors => authors.AuthorId == id);
      return View(author);
    }

    public ActionResult Edit(int id)
    {
      Author author = _db.Authors.First(a => a.AuthorId == id);
      return View(author);
    }

    [HttpPost]
    public ActionResult Edit(Author author)
    {
      _db.Entry(author).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = author.AuthorId });
    }

    public ActionResult Delete(int id)
    {
      Author author = _db.Authors.First(a => a.AuthorId == id);
      return View(author);
    }

    [HttpPost]
    public ActionResult Delete(Author author)
    {
      _db.Authors.Remove(author);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}