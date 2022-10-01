using CanonStore.Commom;
using CanonStore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace CanonStore.Controllers
{
    public class CustomersController : Controller
    {
        db_CanonStoreEntities ctx = new db_CanonStoreEntities();
        // GET: Customer
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {

                    ViewBag.CurrentSort = sortOrder;
                    ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
                    

                    if (searchString != null)
                    {
                        page = 1;
                    }
                    else
                    {
                        searchString = currentFilter;
                    }

                    ViewBag.CurrentFilter = searchString;

                    var customers = from p in ctx.Customers select p;

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        customers = customers.Where(s => s.Name.Contains(searchString));
                    }

                    switch (sortOrder)
                    {
                        case "name":
                            customers = customers.OrderBy(s => s.Name);
                            break;
                        case "name_desc":
                            customers = customers.OrderByDescending(s => s.Name);
                            break;
                        default:  // Name ascending 
                            customers = customers.OrderBy(s => s.Id);
                            break;
                    }

                    int pageSize = 5;
                    int pageNumber = (page ?? 1);
                    return View(customers.ToPagedList(pageNumber, pageSize));
                }
                catch
                {
                    return Content("Error");
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
                    Customer customer = new Customer();
                    return View(customer);
                }
                catch
                {
                    return Content("Error");
                }

            }
            return RedirectToAction("Index", "Login");
            
        }
        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            if (Session["EmployeeId"] != null)
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
                        ctx.Customers.Add(customer);
                        ctx.SaveChanges();


                        return RedirectToAction("Index");
                    }
                    else
                    {
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
        public ActionResult Edit(int id)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    Customer customer = ctx.Customers.Where(c => c.Id == id).FirstOrDefault();
                    return View(customer);
                }
                catch
                {
                    return Content("Error");
                }

            }
            return RedirectToAction("Index", "Login");
           
        }
        [HttpPost]
        public ActionResult Edit(Customer customer)
        {
            if (Session["EmployeeId"] != null)
            {
                try
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

                    return RedirectToAction("Index");
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
                    Customer customer = ctx.Customers.Where(c => c.Id == id).SingleOrDefault();
                    return View(customer);
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");
          
        }
        
        public ActionResult DeleteConfirm(int id)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    Customer customer = ctx.Customers.Where(c => c.Id == id).SingleOrDefault();


                    //xoa
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                    return RedirectToAction("Index");
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
                    Customer customer = ctx.Customers.Where(c => c.Id == id).SingleOrDefault();
                    List<Bill> bills = ctx.Bills.Where(c => c.IdCustomer == id).ToList();

                    ViewBag.bills = bills;

                    return View(customer);
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