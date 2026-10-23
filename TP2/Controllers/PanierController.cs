using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TP2.Models;
using TP2.Models.Help;
using TP2.Models.Repositories;
using TP2.ViewModels;

namespace TP2.Controllers
{
    public class PanierController : Controller
    {
        readonly IProductRepository productRepository;
        readonly ICategorieRepository categRepository;
        readonly IOrderRepository orderRepository;
        private readonly UserManager<IdentityUser> userManager;

        public PanierController(IProductRepository productRepository,
                                ICategorieRepository categRepository,
                                IOrderRepository orderRepository,
                                UserManager<IdentityUser> userManager)
        {
            this.productRepository = productRepository;
            this.categRepository = categRepository;
            this.orderRepository = orderRepository;
            this.userManager = userManager;
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
            Product pp = productRepository.GetById(id);
            if (pp != null)
            {
                ListeCart.Instance.AddItem(pp);
            }
            return RedirectToAction("Index");
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

        // GET: Panier/Checkout
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            ViewData["Categories"] = categRepository.GetAll();

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Path });
            }

            var cartItems = ListeCart.Instance.Items.ToList();
            var totalAmount = ListeCart.Instance.GetSubTotal();

            var viewModel = new OrderViewModel
            {
                CartItems = cartItems.Select(item => new CartItemViewModel
                {
                    ProductName = item.Prod.Name,
                    Quantity = item.quantite,
                    Price = item.Prod.Price
                }).ToList(),
                TotalAmount = totalAmount
            };

            return View(viewModel);
        }

        // POST: Panier/Checkout
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(OrderViewModel model)
        {
            ViewData["Categories"] = categRepository.GetAll();

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Utilisateur non authentifié.";
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Path });
            }

            var cartItems = ListeCart.Instance.Items.ToList();
            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Votre panier est vide.";
                return RedirectToAction("Index");
            }

            model.CartItems = cartItems.Select(item => new CartItemViewModel
            {
                ProductName = item.Prod.Name,
                Quantity = item.quantite,
                Price = item.Prod.Price
            }).ToList();
            model.TotalAmount = ListeCart.Instance.GetSubTotal();

            if (string.IsNullOrWhiteSpace(model.Address))
            {
                TempData["ErrorMessage"] = "Veuillez saisir l'adresse de livraison.";
                return View(model);
            }

            var order = new Order
            {
                CustomerName = user.UserName,
                Email = user.Email,
                Address = model.Address,
                TotalAmount = model.TotalAmount,
                OrderDate = DateTime.Now,
                UserId = user.Id,
                Items = model.CartItems.Select(item => new OrderItem
                {
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            };

            orderRepository.Add(order);
            ListeCart.Instance.Items.Clear();

            TempData["SuccessMessage"] = "Votre commande a été passée avec succès.";
            return RedirectToAction("Confirmation", new { orderId = order.Id });
        }

        // GET: Panier/Confirmation
        [Authorize]
        public IActionResult Confirmation(int orderId)
        {
            ViewData["Categories"] = categRepository.GetAll();
            var order = orderRepository.GetById(orderId);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}