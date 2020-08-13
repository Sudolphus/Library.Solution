using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Library.Models;

namespace Library.Controllers
{
  public class ArticlesController : Controller
  {
    public ActionResult Index()
    {
      List<Article> allArticles = Article.GetArticles(EnvironmentalVariables.ApiKey);
      return View(allArticles);
    }
  }
}