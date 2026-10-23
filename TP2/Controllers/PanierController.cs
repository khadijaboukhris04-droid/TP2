using Microsoft.AspNetCore.Mvc;
using TP2.Models;
using TP2.Models.Help;
using TP2.Models.Repositories;

namespace TP2.Controllers
{
    public class PanierController : Controller
    {
        readonly IProductRepository productRepository;
        readonly ICategorieRepository categRepository;

        public PanierController(IProductRepository productRepository,
                                ICategorieRepository categRepository)
        {
            this.productRepository = productRepository;
            this.categRepository = categRepository;
        }

        public ActionResult Index()
        {
            ViewData["Categories"] = categRepository.GetAll();
            ViewBag.Liste = ListeCart.Instance.Items;
            ViewBag.total = ListeCart.Instance.GetSubTotal();
            return View();
        }

        public ActionResult AddProduct(int id)
        {
            ViewData["Categories"] = categRepository.GetAll();
            Product pp = productRepository.GetById(id);
            ListeCart.Instance.AddItem(pp);
            ViewBag.Liste = ListeCart.Instance.Items;
            ViewBag.total = ListeCart.Instance.GetSubTotal();
            return View();
        }

        [HttpPost]
        public ActionResult PlusProduct(int id)
        {
            Product pp = productRepository.GetById(id);
            ListeCart.Instance.AddItem(pp);

            Item trouve = ListeCart.Instance.Items
                .FirstOrDefault(a => a.Prod.ProductId == pp.ProductId);

            var results = new
            {
                ct = 1,
                Total = ListeCart.Instance.GetSubTotal(),
                Quatite = trouve.quantite,
                TotalRow = trouve.TotalPrice
            };
            return Json(results);
        }

        [HttpPost]
        public ActionResult MinusProduct(int id)
        {
            Product pp = productRepository.GetById(id);
            ListeCart.Instance.SetLessOneItem(pp);

            Item trouve = ListeCart.Instance.Items
                .FirstOrDefault(a => a.Prod.ProductId == pp.ProductId);

            if (trouve != null)
            {
                var results = new
                {
                    ct = 1,
                    Total = ListeCart.Instance.GetSubTotal(),
                    Quatite = trouve.quantite,
                    TotalRow = trouve.TotalPrice
                };
                return Json(results);
            }
            return Json(new { ct = 0, Total = ListeCart.Instance.GetSubTotal() });
        }

        [HttpPost]
        public ActionResult RemoveProduct(int id)
        {
            Product pp = productRepository.GetById(id);
            ListeCart.Instance.RemoveItem(pp);

            var results = new
            {
                Total = ListeCart.Instance.GetSubTotal()
            };
            return Json(results);
        }
    }
}