using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP2.Models;
using TP2.Models.Repositories;



namespace TP2.Controllers
{
    [Authorize(Roles = "Admin,Manager")]

    public class CategoryController : Controller
    {
        readonly ICategorieRepository CategRepository;

        public CategoryController(ICategorieRepository categRepository)
        {
            CategRepository = categRepository;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            return View(categories);
        }

        public ActionResult Details(int id)
        {
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            return View(CategRepository.GetById(id));
        }

        public ActionResult Create()
        {
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category c)
        {
            if (ModelState.IsValid)
            {
                CategRepository.Add(c);
                return RedirectToAction(nameof(Index));
            }
            return View(c);
        }

        public ActionResult Edit(int id)
        {
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            return View(CategRepository.GetById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category c)
        {
            if (ModelState.IsValid)
            {
                CategRepository.Update(c);
                return RedirectToAction(nameof(Index));
            }
            return View(c);
        }

        public ActionResult Delete(int id)
        {
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            return View(CategRepository.GetById(id));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CategRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}