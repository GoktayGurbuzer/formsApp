using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using formsApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace formsApp.Controllers;

public class HomeController : Controller
{
    public HomeController()
    {
        
    }

    public IActionResult Index(string searchString, string category)
    {
        var products = Repository.Products;
        if(!string.IsNullOrEmpty(searchString))
        {
            ViewBag.SearchString = searchString;
            products = products.Where(x => x.Name!.ToLower().Contains(searchString)).ToList();
        }

        if(!string.IsNullOrEmpty(category) && category != "0")
        {
            products = products.Where(c => c.CategoryId == int.Parse(category)).ToList();
        }

        var model = new ProductViewModel
        {
            Products = products,
            Categories = Repository.Category,
            SelectedCategory = category
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(Repository.Category, "CategoryId", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product, IFormFile image)
    {
        var extension = string.Empty;
        if(image != null)
        {
            extension = Path.GetExtension(image.FileName);
        }

        if(ModelState.IsValid){
            if(image != null){
                var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);
                using(var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
            }
            product.Image = image!.FileName;
            product.ProductId = Repository.Products.Count() + 1;
            Repository.CreateProduct(product);
            return RedirectToAction("Index");
        }
        ViewBag.Categories = new SelectList(Repository.Category, "CategoryId", "Name");
        return View(product);
    }

    [HttpGet]
    public IActionResult Edit(int? id){
        if(id == null) 
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);
        if(entity == null)
        {
            return NotFound();
        }
        ViewBag.Categories = new SelectList(Repository.Category, "CategoryId", "Name");
        return View(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Product model, IFormFile? imageFile)
    {
        if(id != model.ProductId)
        {
            return NotFound();
        }

        if(ModelState.IsValid)
        {
            if(imageFile != null) 
            {
                var extension = Path.GetExtension(imageFile.FileName); // abc.jpg
                var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);

                using(var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                model.Image = randomFileName;
            }
            Repository.EditProduct(model);
            return RedirectToAction("Index");
        }
        ViewBag.Categories = new SelectList(Repository.Category, "CategoryId", "Name");
        return View(model);
    }

    public IActionResult Delete(int? id)
    {
        if(id == null){ return NotFound(); }

        var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);
        if(entity == null){ return NotFound(); }

        return View("DeleteConfirm", entity);
    }

    [HttpPost]
    public IActionResult Delete(int id, int ProductId)
    {
        if(id != ProductId){ return NotFound(); }
        var entity = Repository.Products.FirstOrDefault(p => p.ProductId == ProductId);
        if(entity == null){ return NotFound(); }

        Repository.DeleteProdcut(entity);
        return RedirectToAction("Index");
    }

    public IActionResult EditProducts(List<Product> products)
    {
        foreach(var product in products)
        {
            Repository.EditIsActive(product);
        }

        return RedirectToAction("Index");
    }
}
