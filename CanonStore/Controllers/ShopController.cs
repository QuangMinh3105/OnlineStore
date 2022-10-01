using CanonStore.Commom;
using CanonStore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PagedList;
using PagedList.Mvc;

namespace CanonStore.Controllers
{
    [HandleError]
    public class ShopController : Controller
    {
        
        db_CanonStoreEntities ctx = new db_CanonStoreEntities();
        // GET: Shop
        public ActionResult Index()
        {
            try
            {
                List<Product> products = new List<Product>();
                //List<Product> products = ctx.Products.Take(4).ToList();
                try
                {

                    foreach (var item in ctx.proc_Top5bestsellingproduct().Take(5))
                    {
                        Product product = ctx.Products.Where(p => p.Id_product == item.Product).FirstOrDefault();
                        products.Add(product);
                    }
                }
                catch
                {
                    products = ctx.Products.Take(4).ToList();
                }
                return View(products);
            }
            catch
            {
                return Content("Error");
            }

        }
        public ActionResult Login()
        {
            try
            {
                if (Session["CustomerId"] != null)
                {
                    int id = Convert.ToInt32(Session["CustomerId"]);
                    Customer customer = ctx.Customers.Where(c => c.Id == id).SingleOrDefault();
                    List<Bill> bills = ctx.Bills.Where(b => b.IdCustomer == id).ToList();
                    ViewBag.cusBills = bills;
                    return View("AboutCustomer", customer);
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return Content("Error");
            }

        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(CustomerCheck customerCheck)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        using (db_CanonStoreEntities db = new db_CanonStoreEntities())
                        {
                            Password EncryptData = new Password();

                            customerCheck.Password = EncryptData.Encode(customerCheck.Password);
                            var obj = db.Employees.Where(a => a.UserName == customerCheck.UserName && a.Password == customerCheck.Password).FirstOrDefault();

                            if (obj != null)
                            {
                                Session["EmployeeId"] = obj.Id.ToString();
                                Session["EmployeeUserName"] = obj.UserName.ToString();
                                Session["EmployeeName"] = obj.Name.ToString();


                                return RedirectToAction("Index", "Products");
                            }

                        }
                    }
                    catch (Exception)
                    {

                    }
                    try
                    {
                        using (db_CanonStoreEntities db = new db_CanonStoreEntities())
                        {
                            //Password EncryptData = new Password();
                            //customerCheck.Password = EncryptData.Encode(customerCheck.Password);
                            var obj = db.Customers.Where(a => a.UserName == customerCheck.UserName && a.Password == customerCheck.Password).FirstOrDefault();

                            if (obj != null)
                            {
                                Session["CustomerId"] = obj.Id.ToString();
                                Session["CustomerUserName"] = obj.UserName.ToString();
                                Session["CustomerName"] = obj.Name.ToString();

                                return RedirectToAction("Index");
                            }
                            else
                            {
                                ViewBag.error = "Username or password is incorrect";
                                return View();
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                return View();
            }
            catch
            {
                return Content("Error");
            }

        }
        public ActionResult Logout()
        {
            try
            {
                Session.Clear();//remove session
                return RedirectToAction("Index");
            }
            catch
            {
                return Content("Error");
            }

        }
        public ActionResult SignUp()
        {
            try
            {
                Customer customer = new Customer();
                return View(customer);
            }
            catch
            {
                return Content("Error");
            }

        }
        [HttpPost]
        public ActionResult SignUpSave(Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    customer.DateCreated = DateTime.Now;
                    customer.Image = "Avatar.png";
                    Password EncryptData = new Password();
                    string pass = customer.Password;
                    customer.Password = EncryptData.Encode(customer.Password);
                    List<Customer> customers = ctx.Customers.Where(c => c.Password == customer.Password).ToList();

                    ctx.Customers.Add(customer);
                    ctx.SaveChanges();
                    CustomerCheck customerCheck = new CustomerCheck();
                    customerCheck.UserName = customer.UserName;
                    customerCheck.Password = pass;
                    Login(customerCheck);
                    return RedirectToAction("Index");

                }
                else
                {
                    return View("SignUp");
                }
            }
            catch
            {
                return Content("Error");
            }

        }
        public ActionResult Products(int? page, string currentFilter, string searchString, string sortOrder)
        {
            try
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
                ViewBag.PriceSortParm = sortOrder == "price" ? "price_desc" : "price";
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
                    case "price":
                        products = products.OrderBy(s => s.Price);
                        break;
                    case "price_desc":
                        products = products.OrderByDescending(s => s.Price);
                        break;
                    default:  // Name ascending 
                        products = products.OrderBy(s => s.Id_product);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                
                return View(products.ToPagedList(pageNumber, pageSize));
            }
            catch
            {
                return Content("Error");
            }

        }
        public ActionResult ProductDetails(int id)
        {
            try
            {
                Product product = ctx.Products.Where(c => c.Id_product == id).SingleOrDefault();

                List<Product> products3 = ctx.Products.Where(c => c.Type == product.Type && c.Id_product != product.Id_product).Take(4).ToList();
                ViewBag.products3 = products3;

                List<Comment> comments = ctx.Comments.Where(com => com.Id_Product == product.Id_product).OrderByDescending(com => com.Date).ToList();
                ViewBag.Comment = comments;

                Accessory accessory = ctx.Accessories.Where(a => a.Id_Acc == product.Acc_Id).FirstOrDefault();
                ViewBag.acc = accessory;

                //ViewBag.pt = product_Type;
                return View(product);
            }
            catch
            {
                return Content("Error");
            }

        }
        public ActionResult CustomerBillDetail(string id)
        {
            try
            {
                if (Session["CustomerId"] != null)
                {
                    if (id != null)
                    {
                        Bill bill = ctx.Bills.Where(c => c.IdBill == id).SingleOrDefault();

                        List<Bill_details> bill_Details = ctx.Bill_details.Where(c => c.IdBill == id).ToList();

                        ViewBag.bill_Details = bill_Details;

                        Customer customer = ctx.Customers.Where(c => c.Id == bill.IdCustomer).SingleOrDefault();
                        ViewBag.customer = customer;
                        return View(bill);
                    }
                }
                return View("Index");
            }
            catch (Exception)
            {
                return View("Index");
            }
        }
        public ActionResult CustomerEdit(int id)
        {
            try
            {
                if (Session["CustomerId"] != null)
                {
                    Customer customer = ctx.Customers.Where(c => c.Id == id).FirstOrDefault();
                    return View(customer);
                }
                else
                {
                    return View("Index");
                }
            }
            catch
            {
                return Content("Error");
            }
        }
        [HttpPost]
        public ActionResult CustomerUpdate(Customer customer)
        {
            try
            {
                if (Session["CustomerId"] != null)
                {
                    Customer oldCustomer = ctx.Customers.Where(c => c.Id == customer.Id).SingleOrDefault();

                    oldCustomer.Name = customer.Name;
                    oldCustomer.Phone = customer.Phone;
                    //oldCustomer.DayOfBirth = customer.DayOfBirth;
                    oldCustomer.Address = customer.Address;
                    oldCustomer.Email = customer.Email;

                    if (customer.ImageUpload != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(customer.ImageUpload.FileName);
                        string extension = Path.GetExtension(customer.ImageUpload.FileName);
                        fileName += extension;
                        customer.Image = "" + oldCustomer.Name + extension;
                        customer.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), oldCustomer.Name + extension));
                        oldCustomer.Image = customer.Image;
                    }

                    ctx.SaveChanges();

                    return RedirectToAction("Login");


                }
                else
                {
                    return View("Index");
                }
            }
            catch
            {
                return Content("Error");
            }

        }

        public ActionResult Cart()
        {
            try
            {
                List<ItemCart> cart = null;
                if (HttpContext.Session["yourcart"] == null)
                {
                    cart = new List<ItemCart>();
                }
                else
                {
                    cart = (List<ItemCart>)HttpContext.Session["yourcart"];
                }
                // cal total of your cart

                float total = 0;

                foreach (ItemCart it in cart)
                {
                    total += it.LineTotal;
                }

                ViewBag.Total = total;

                if (Session["Dis_code"] != null)
                {
                    ViewBag.dismess = "Discount code has been added.";
                }
                else
                {
                    ViewBag.dismess = "The discount code is not available or has expired.";
                }
                //passing to View
                return View(cart);
            }
            catch
            {
                return Content("Error");
            }

        }
        [HttpPost]
        public ActionResult AddToCart()
        {
            try
            {
                //step 1
                List<ItemCart> cart = null;
                if (HttpContext.Session["yourcart"] == null)
                {
                    cart = new List<ItemCart>();
                }
                else
                {
                    cart = (List<ItemCart>)HttpContext.Session["yourcart"];
                }

                int Id = int.Parse(Request.Form["Id_product"]);
                int count = 0;
                //check product trung
                foreach (ItemCart it in cart)
                {
                    if (it.Producti.Id_product == Id)
                    {
                        count += 1;
                    }
                }
                //luu product moi
                if (count == 0)
                {
                    Product product = ctx.Products.Where(t => t.Id_product == Id).SingleOrDefault();
                    int qty = Convert.ToInt32(Request.Form["txtQuantity"]);

                    ItemCart item = new ItemCart()
                    {
                        Producti = product,
                        Quantity = qty,
                        LineTotal = (float)(qty * product.Price)
                    };
                    //step 2
                    cart.Add(item);
                    //step 3

                    HttpContext.Session["yourcart"] = cart;
                }
                else
                {
                    int sl = 0;
                    ItemCart item = cart.Where(c => c.Producti.Id_product == Id).FirstOrDefault();

                    sl += item.Quantity;

                    cart.Remove(item);
                    Product product = ctx.Products.Where(t => t.Id_product == Id).SingleOrDefault();
                    int qty = Convert.ToInt32(Request.Form["txtQuantity"]);
                    if ((qty + sl) > product.Quantity_in_Stock)
                    {
                        sl = (int)product.Quantity_in_Stock;
                        qty = 0;

                    }
                    ItemCart item1 = new ItemCart()
                    {
                        Producti = product,
                        Quantity = qty + sl,
                        LineTotal = (float)((qty + sl) * product.Price)
                    };
                    cart.Add(item1);
                    HttpContext.Session["yourcart"] = cart;
                }
                //ItemCart 
                return RedirectToAction("ProductDetails/" + Id);
            }
            catch
            {
                return Content("Error");
            }

        }
        public ActionResult DeleteItemCart(int id)
        {
            try
            {
                List<ItemCart> cart = (List<ItemCart>)HttpContext.Session["yourcart"];
                //Product product = ctx.Products.Where(t => t.Id == id).SingleOrDefault();
                ItemCart item = cart.Where(c => c.Producti.Id_product == id).FirstOrDefault();

                cart.Remove(item);
                return RedirectToAction("Cart");
            }
            catch
            {
                return Content("Error");
            }

        }
        public ActionResult CheckDiscount()
        {
            try
            {
                string dis = Request.Form["Discount"];
                DateTime datenow = DateTime.Now;
                var discount = ctx.Discounts.Where(d => d.Dis_Code == dis && d.Date_Start <= datenow.Date && d.Date_End > datenow.Date).FirstOrDefault();
                if (discount != null)
                {
                    Session["Dis_code"] = discount.Dis_Code;
                    Session["Dis_Value"] = discount.Discount_Value;
                    //ViewBag.dismess = "Discount code has been added.";

                    return RedirectToAction("Cart");
                }
                //else
                //{
                //    ViewBag.dismess = "The discount code is not available or has expired.";
                //}
                return RedirectToAction("Cart");
            }
            catch
            {
                return Content("Error");
            }

        }
        public ActionResult DeleteDiscount()
        {
            try
            {
                Session.Remove("Dis_code");
                Session.Remove("Dis_Value");
                return RedirectToAction("Cart");
            }
            catch
            {
                return Content("Error");
            }

        }
        public ActionResult Payment()
        {
            try
            {
                if (Session["CustomerId"] != null)
                {
                    List<ItemCart> cart = null;
                    if (HttpContext.Session["yourcart"] == null)
                    {
                        cart = new List<ItemCart>();
                    }
                    else
                    {
                        cart = (List<ItemCart>)HttpContext.Session["yourcart"];
                    }
                    // cal total of your cart

                    float total = 0;

                    foreach (ItemCart it in cart)
                    {
                        total += it.LineTotal;
                    }

                    ViewBag.Total = total;

                    if (Session["Dis_code"] != null)
                    {
                        ViewBag.dismess = "Discount code has been added.";
                    }
                    else
                    {
                        ViewBag.dismess = "The discount code is not available or has expired.";
                    }
                    //passing to View
                    return View(cart);
                }
                else
                {
                    return View("Login");
                }
            }
            catch
            {
                return Content("Error");
            }

        }
        public ActionResult PaymentComfirm()
        {
            try
            {
                if (Session["CustomerId"] != null)
                {
                    List<ItemCart> cart = null;
                    if (HttpContext.Session["yourcart"] == null)
                    {
                        cart = new List<ItemCart>();
                    }
                    else
                    {
                        cart = (List<ItemCart>)HttpContext.Session["yourcart"];
                    }
                    // cal total of your cart

                    float total = 0;

                    foreach (ItemCart it in cart)
                    {

                        total += it.LineTotal;
                    }

                    if (Session["Dis_Value"] != null)
                    {
                        double dis = Convert.ToDouble(Session["Dis_Value"]);
                        total = (float)(total - (total * (dis / 100)));
                        ViewBag.Total = total;

                    }
                    else
                    {
                        ViewBag.Total = total;
                        Session["Dis_code"] = "None";
                    }
                    int countBill = 0;
                    try
                    {
                        List<Bill> bills = ctx.Bills.Where(b => b.Date_Created.Month == DateTime.Now.Month && b.Date_Created.Year == DateTime.Now.Year && b.Date_Created.Day == DateTime.Now.Day).ToList();
                        countBill = bills.Count + 1;
                    }
                    catch
                    {
                        countBill = 1;
                    }


                    Bill bill = new Bill()
                    {
                        IdBill = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "B" + countBill.ToString() + "C" + Session["CustomerId"],
                        IdCustomer = Convert.ToInt32(Session["CustomerId"]),
                        IdEmployee = 9,
                        Status = 1,
                        Date_Created = DateTime.Now.Date,
                        Total = total,
                        Discount_code = Session["Dis_code"].ToString()

                    };
                    ctx.Bills.Add(bill);
                    ctx.SaveChanges();

                    foreach (ItemCart it in cart)
                    {
                        Bill_details bill_Details = new Bill_details()
                        {
                            IdBill = bill.IdBill,///
                            IdProduct = it.Producti.Id_product,
                            Quality = it.Quantity,
                            Date_Created = DateTime.Now.Date,
                            Price = it.LineTotal
                        };
                        ctx.Bill_details.Add(bill_Details);
                        ctx.SaveChanges();
                    }

                    Session.Remove("yourcart");
                    Session.Remove("Dis_code");
                    Session.Remove("Dis_Value");
                    return RedirectToAction("Products");
                }
                else
                {
                    return View("Login");
                }
            }
            catch
            {
                return Content("Error");
            }

        }
        public ActionResult About()
        {
            try
            {
                return View();
            }
            catch
            {
                return Content("Error");
            }

        }
        public ActionResult Contact()
        {
            try
            {




                return View();



            }
            catch
            {
                return Content("Error");
            }

        }
        [HttpPost]
        public ActionResult Comments(string Id_bill, int Id)
        {
            if (Session["CustomerId"] != null)
            {
                try
                {
                    ViewBag.id = Session["CustomerId"];
                    Bill_details bill_Details = ctx.Bill_details.Where(bd => bd.IdBill == Id_bill && bd.Id == Id).FirstOrDefault();
                    return View(bill_Details);
                }
                catch
                {
                    return RedirectToAction("CustomerBillDetail/" + Id_bill);
                }
            }
            else
            {
                return View("Login");
            }

        }
        public ActionResult Rate(float Rating, string Comment, int Id_Customer, string Id_bill, int Id_Product)
        {
            if (Session["CustomerId"] != null)
            {
                try
                {
                    Comment comment = new Comment()
                    {
                        Id_Bill = Id_bill,
                        Id_Customer = Id_Customer,
                        Id_Product = Id_Product,
                        Rating = Rating,
                        Com_txt = Comment,
                        Date = DateTime.Now.Date
                    };
                    ctx.Comments.Add(comment);
                    ctx.SaveChanges();
                    return RedirectToAction("CustomerBillDetail/" + Id_bill);

                }
                catch
                {
                    return RedirectToAction("CustomerBillDetail/" + Id_bill);
                }
            }
            else
            {
                return View("Login");
            }


        }
    }

}