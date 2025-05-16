using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROG7311_ST10152431_Part2.Data;
using PROG7311_ST10152431_Part2.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PROG7311_ST10152431_Part2.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string farmerName, string farmerSurname, string farmerPassword)
        {
            if (_context.Farmers.Any(f => f.FarmerName == farmerName && f.FarmerSurname == farmerSurname))
            {
                ModelState.AddModelError("", "Farmer already exists.");
                return View("Privacy");
            }

            var farmer = new Farmer
            {
                FarmerName = farmerName,
                FarmerSurname = farmerSurname,
                FarmerPassword = farmerPassword
            };
            _context.Farmers.Add(farmer);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("FarmerId", farmer.FarmerId);
            HttpContext.Session.SetString("FarmerName", farmer.FarmerName);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Login(string farmerName, string farmerPassword, string employeePassword)
        {
            // Employee login
            if (!string.IsNullOrEmpty(employeePassword) && farmerName == "AgriEnergy" && employeePassword == "1234")
            {
                HttpContext.Session.SetInt32("EmployeeId", 1); // Use a fixed ID or any value
                HttpContext.Session.SetString("EmployeeName", "AgriEnergy");
                return RedirectToAction("Index");
            }

            // Farmer login
            if (string.IsNullOrEmpty(farmerPassword))
            {
                ModelState.AddModelError("farmerPassword", "The farmerPassword field is required.");
                return View("Privacy");
            }

            var farmer = _context.Farmers
                .FirstOrDefault(f => f.FarmerName == farmerName && f.FarmerPassword == farmerPassword);

            if (farmer == null)
            {
                ModelState.AddModelError("", "Invalid login.");
                return View("Privacy");
            }

            HttpContext.Session.SetInt32("FarmerId", farmer.FarmerId);
            HttpContext.Session.SetString("FarmerName", farmer.FarmerName);

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Products()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Privacy");

            var products = _context.Products
                .Include(p => p.Farmer)
                .ToList();

            return View(products);
        }

        [HttpPost]
        public IActionResult AddProduct(string productName, string productCategory, DateTime productionDate, decimal productPrice)
        {
            var farmerId = HttpContext.Session.GetInt32("FarmerId");
            var employeeId = HttpContext.Session.GetInt32("EmployeeId");

            // Only allow adding if logged in as farmer or employee
            if (farmerId == null && employeeId == null)
                return RedirectToAction("Privacy");

            // If employee, allow them to select a farmer (not shown here), otherwise use logged-in farmer
            int ownerId = farmerId ?? 0;
            if (employeeId != null && farmerId == null)
            {
                // For simplicity, assign to first farmer (customize as needed)
                var firstFarmer = _context.Farmers.FirstOrDefault();
                if (firstFarmer != null)
                    ownerId = firstFarmer.FarmerId;
            }

            var product = new Product
            {
                ProductName = productName,
                ProductCategory = productCategory,
                ProductionDate = productionDate,
                ProductPrice = productPrice,
                FarmerId = ownerId
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Products");
        }

        public IActionResult ResourceCentre()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Privacy");
            return View();
        }

        // --- Employee Management Actions ---

        public IActionResult ManageFarmers()
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
                return RedirectToAction("Privacy");

            var farmers = _context.Farmers.ToList();
            return View(farmers);
        }

        [HttpGet]
        public IActionResult EditFarmer(int id)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
                return RedirectToAction("Privacy");

            var farmer = _context.Farmers.Find(id);
            if (farmer == null) return NotFound();
            return View(farmer);
        }

        [HttpPost]
        public IActionResult EditFarmer(Farmer farmer)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
                return RedirectToAction("Privacy");

            _context.Farmers.Update(farmer);
            _context.SaveChanges();
            return RedirectToAction("ManageFarmers");
        }

        public IActionResult DeleteFarmer(int id)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
                return RedirectToAction("Privacy");

            var farmer = _context.Farmers.Find(id);
            if (farmer != null)
            {
                _context.Farmers.Remove(farmer);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageFarmers");
        }

        public IActionResult ManageProducts(string category, DateTime? productionDate, int? farmerId)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
                return RedirectToAction("Privacy");

            var productsQuery = _context.Products.Include(p => p.Farmer).AsQueryable();

            if (!string.IsNullOrEmpty(category))
                productsQuery = productsQuery.Where(p => p.ProductCategory == category);

            if (productionDate.HasValue)
                productsQuery = productsQuery.Where(p => p.ProductionDate.Date == productionDate.Value.Date);

            if (farmerId.HasValue && farmerId.Value > 0)
                productsQuery = productsQuery.Where(p => p.FarmerId == farmerId.Value);

            var products = productsQuery.ToList();

            ViewBag.CurrentCategory = category;
            ViewBag.CurrentDate = productionDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentFarmerId = farmerId;
            ViewBag.Farmers = _context.Farmers.ToList();

            return View(products);
        }


        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
                return RedirectToAction("Privacy");

            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            ViewBag.Farmers = _context.Farmers.ToList();
            return View(product);
        }


        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
                return RedirectToAction("Privacy");

            // Only update the properties you want, and attach the entity properly
            var existingProduct = _context.Products.Find(product.ProductId);
            if (existingProduct == null)
                return NotFound();

            existingProduct.ProductName = product.ProductName;
            existingProduct.ProductCategory = product.ProductCategory;
            existingProduct.ProductionDate = product.ProductionDate;
            existingProduct.ProductPrice = product.ProductPrice;
            existingProduct.FarmerId = product.FarmerId; // If you allow changing the farmer

            _context.SaveChanges();
            return RedirectToAction("ManageProducts");
        }
        

        public IActionResult DeleteProduct(int id)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
                return RedirectToAction("Privacy");

            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageProducts");
        }


        [HttpGet]
        public IActionResult AddFarmer()
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
                return RedirectToAction("Privacy");
            return View();
        }

        [HttpPost]
        public IActionResult AddFarmer(Farmer farmer)
        {
            if (HttpContext.Session.GetInt32("EmployeeId") == null)
                return RedirectToAction("Privacy");

            if (ModelState.IsValid)
            {
                _context.Farmers.Add(farmer);
                _context.SaveChanges();
                return RedirectToAction("ManageFarmers");
            }
            return View(farmer);
        }


        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetInt32("FarmerId") != null ||
                   HttpContext.Session.GetInt32("EmployeeId") != null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
