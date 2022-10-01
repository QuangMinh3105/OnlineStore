using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using CanonStore.Models;
using ClosedXML.Excel;
using PagedList;
using PagedList.Mvc;
//using Excel = Microsoft.Office.Interop.Excel;


namespace CanonStore.Controllers
{
    public class ProductsController : Controller
    {
        
        db_CanonStoreEntities ctx = new db_CanonStoreEntities();
        // GET: Products

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    ViewBag.CurrentSort = sortOrder;
                    ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
                    ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";

                    if (searchString != null)
                    {
                        page = 1;
                    }
                    else
                    {
                        searchString = currentFilter;
                    }

                    ViewBag.CurrentFilter = searchString;

                    var products = from p in ctx.Products select p;

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        products = products.Where(s => s.Name.Contains(searchString));
                    }

                    switch (sortOrder)
                    {
                        case "name":
                            products = products.OrderBy(s => s.Name);
                            break;
                        case "name_desc":
                            products = products.OrderByDescending(s => s.Name);
                            break;
                        case "Price":
                            products = products.OrderBy(s => s.Price);
                            break;
                        case "price_desc":
                            products = products.OrderByDescending(s => s.Price);
                            break;
                        default:  // Name ascending 
                            products = products.OrderBy(s => s.Id_product);
                            break;
                    }

                    int pageSize = 5;
                    int pageNumber = (page ?? 1);
                    ViewBag.products = products;
                    return View(products.ToPagedList(pageNumber, pageSize));
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");
        }
   
        public ActionResult Sales()
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    List<SumProduct> sumProducts = new List<SumProduct>();
                    foreach (var item1 in ctx.proc_sumproductbymonth((DateTime?)DateTime.Now.Date))
                    {
                       
                            SumProduct sum = new SumProduct()
                            {
                                id = item1.Product,
                                Quanlity = (int)item1.Quality
                            };
                            sumProducts.Add(sum);
                        
                    }

                    return View(sumProducts);
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");
            
        }
        [HttpPost]
        public ActionResult Sales(string search, string date)
        {
            if (Session["EmployeeId"] != null)
            {
                List<SumProduct> sumProducts = new List<SumProduct>();
                try
                {
                    List<Product> products = new List<Product>();
                    
                    DateTime dateTime = new DateTime();
                    try
                    {
                        dateTime = Convert.ToDateTime(date);
                    }
                    catch (Exception)
                    {
                        dateTime = DateTime.Now.Date;
                    }
                    if (search != "")
                    {
                        products = ctx.Products.Where(s => s.Name.Contains(search)).OrderBy(s => s.Name).ToList();
                        foreach (var item in products)
                        {
                            foreach (var item1 in ctx.proc_sumproductbymonth((DateTime?)dateTime))
                            {
                                if (item.Id_product == item1.Product)
                                {
                                    SumProduct sum = new SumProduct()
                                    {
                                        id = item1.Product,
                                        Quanlity = (int)item1.Quality
                                    };
                                    sumProducts.Add(sum);
                                }
                            }
                        }

                        return View(sumProducts);
                    }
                    else
                    {
                        foreach (var item1 in ctx.proc_sumproductbymonth((DateTime?)dateTime))
                        {
                            
                                SumProduct sum = new SumProduct()
                                {
                                    id = item1.Product,
                                    Quanlity = (int)item1.Quality
                                };
                                sumProducts.Add(sum);
                            
                        }
                        return View(sumProducts);
                    }
                   
                    
                }
                catch
                {
                    return View(sumProducts);
                }
            }
            return RedirectToAction("Index", "Login");
        }
        public ActionResult Create()
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    Product product = new Product();

                    List<Product_Types> product_Types = ctx.Product_Types.ToList();
                    ViewBag.product_Types = product_Types;

                    List<Accessory> accessories = ctx.Accessories.ToList();
                    ViewBag.accessories = accessories;

                    return View(product);
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");
           
        }
        [HttpPost]
        public ActionResult Create(Product product)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (product.ImageUpload != null)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(product.ImageUpload.FileName);
                            string extension = Path.GetExtension(product.ImageUpload.FileName);
                            fileName += extension;
                            product.Image = "" + fileName;
                            product.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), fileName));
                        }
                        else 
                        {
                            product.Image = "noimage.PNG";
                        }
                      
                        ctx.Products.Add(product);
                        ctx.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        List<Product_Types> product_Types = ctx.Product_Types.ToList();
                        ViewBag.product_Types = product_Types;

                        List<Accessory> accessories = ctx.Accessories.ToList();
                        ViewBag.accessories = accessories;

                        return View("Create");
                    }
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");
          
            
        }
        public ActionResult Detail(int id)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    Product product = ctx.Products.Where(c => c.Id_product == id).SingleOrDefault();
                    return View(product);
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");
           
            
        }
        public ActionResult Edit(int id)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    List<Product_Types> product_Types = ctx.Product_Types.ToList();
                    ViewBag.product_Types = product_Types;

                    List<Accessory> accessories = ctx.Accessories.ToList();
                    ViewBag.accessories = accessories;

                    Product product = ctx.Products.Where(c => c.Id_product == id).SingleOrDefault();
                    ViewBag.productId = id;
                    //passing data / model to view
                    return View(product);
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");
           
            
        }
        [HttpPost]
        public ActionResult Edit(Product product)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    try
                    {
                        Product oldProduct = ctx.Products.Where(c => c.Id_product == product.Id_product).SingleOrDefault();

                        oldProduct.Name = product.Name;
                        oldProduct.Brand = product.Brand;
                        oldProduct.Price = product.Price;
                        oldProduct.Type = product.Type;
                        oldProduct.Acc_Id = product.Acc_Id;
                        oldProduct.Warranty = product.Warranty;
                        oldProduct.Aperture = product.Aperture;
                        oldProduct.Focal_Distance = product.Focal_Distance;
                        oldProduct.Shutter_Speed = product.Shutter_Speed;
                        oldProduct.Quantity_in_Stock = product.Quantity_in_Stock;
                        oldProduct.Description = product.Description;
                        oldProduct.Is_Available = product.Is_Available;
                        //oldProduct.IsAvailable = product.IsAvailable;
                        if (product.ImageUpload != null)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(product.ImageUpload.FileName);
                            string extension = Path.GetExtension(product.ImageUpload.FileName);
                            fileName += extension;
                            product.Image = "" + fileName;
                            product.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), fileName));
                            oldProduct.Image = product.Image;
                        }
                        else
                        {
                            product.Image = oldProduct.Image;
                        }
                        oldProduct.Image = product.Image;
                        ctx.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Edit", product.Id_product);
                    }
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");
            
        }
        public ActionResult Delete(int id)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    List<Product_Types> product_Types = ctx.Product_Types.ToList();
                    ViewBag.product_Types = product_Types;

                    List<Accessory> accessories = ctx.Accessories.ToList();
                    ViewBag.accessories = accessories;

                    Product product = ctx.Products.Where(c => c.Id_product == id).SingleOrDefault();
                    ViewBag.productId = id;
                    //passing data / model to view
                    return View(product);
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");
            
        }
        [HttpPost]
        public ActionResult DeleteConfirm(Product product)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {

                    Product product1 = ctx.Products.Where(c => c.Id_product == product.Id_product).SingleOrDefault();
                    //xoa
                    ctx.Products.Remove(product1);
                    ctx.SaveChanges();
                    //redirect view
                    return RedirectToAction("Index");
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");
           
        }
        public ActionResult ExportExcel()
        {
            if (Session["EmployeeId"] != null)
            {
                //List<Product> products = (List<Product>)Session["Product"];
                List<Product> products = ctx.Products.OrderBy(p => p.Id_product).ToList();
                try
                {
                    DataTable dt = new DataTable("Grid");
                    
                    dt.Columns.AddRange(new DataColumn[9] { new DataColumn("IdProduct"),
                                            new DataColumn("Name"),
                                            new DataColumn("Brand"),
                                            new DataColumn("Price"),
                                            new DataColumn("Type") ,
                                            new DataColumn("Accesory") ,
                                            new DataColumn("Warranty") ,
                                            new DataColumn("Description") ,
                                            new DataColumn("Date Created")  
                    });
                    
                    foreach (var product in products)
                    {
                        Product_Types product_Type = ctx.Product_Types.Where(a => a.IdType == product.Type).FirstOrDefault();
                        Accessory accessory = ctx.Accessories.Where(s => s.Id_Acc == product.Acc_Id).FirstOrDefault();

                        dt.Rows.Add(product.Id_product,product.Name,product.Brand,product.Price,product_Type.Name,accessory.Name_Acc,product.Warranty,product.Description,product.Date_Created);
                    }
                    
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        
                        wb.Worksheets.Add(dt).Columns().AdjustToContents();
                        
                        using (MemoryStream stream = new MemoryStream())
                        {
                            
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products Table_" + DateTime.Now.Date.ToString("dd_MM_yyyy") + ".xlsx");
                        }

                    }
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        public ActionResult ExportPDF()
        {
            if (Session["EmployeeId"] != null)
            {
                //List<Product> products = (List<Product>)Session["Product"];
                List<Product> products = ctx.Products.OrderBy(p => p.Id_product).ToList();
                try
                {
                    ProductsReport productsReport = new ProductsReport();
                    byte[] abytes = productsReport.PrepareReport(products);
                    return File(abytes, "application/pdf");
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");
        }
    }
}