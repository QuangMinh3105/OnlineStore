using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CanonStore.Commom;
using CanonStore.Models;

namespace CanonStore.Controllers
{
    public class EmployeesController : Controller
    {
        db_CanonStoreEntities ctx = new db_CanonStoreEntities();
        // GET: Employee
        public ActionResult Index()
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    List<Employee> employees = ctx.Employees.ToList();
                    return View(employees);
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
                    Employee employee = new Employee();

                    return View(employee);
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        public ActionResult Create(Employee employee)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        if (employee.ImageUpload != null)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(employee.ImageUpload.FileName);
                            string extension = Path.GetExtension(employee.ImageUpload.FileName);
                            employee.DateCreated = DateTime.Now.Date;
                            employee.Image = "" + employee.Name + extension;
                            employee.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), employee.Name + extension));
                        }
                        else
                        {
                            employee.Image = "Avatar.png";
                        }
                        Password EncryptData = new Password();

                        employee.Password = EncryptData.Encode(employee.Password);

                        ctx.Employees.Add(employee);
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
        public ActionResult Detail(int id)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    Employee employee = ctx.Employees.Where(c => c.Id == id).SingleOrDefault();
                    return View(employee);
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
                    Employee employee = ctx.Employees.Where(c => c.Id == id).SingleOrDefault();


                    return View(employee);
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");

        }
        [HttpPost]
        public ActionResult Edit(Employee employee)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    Employee oldEmployee = ctx.Employees.Where(c => c.Id == employee.Id).SingleOrDefault();

                    oldEmployee.Name = employee.Name;
                    oldEmployee.Phone = employee.Phone;
                    //oldCustomer.DayOfBirth = customer.DayOfBirth;

                    if (employee.ImageUpload != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(employee.ImageUpload.FileName);
                        string extension = Path.GetExtension(employee.ImageUpload.FileName);
                        fileName += extension;
                        employee.Image = "" + oldEmployee.Name + extension;
                        employee.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), oldEmployee.Name + extension));
                        oldEmployee.Image = employee.Image;
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
                    Employee employee = ctx.Employees.Where(c => c.Id == id).SingleOrDefault();

                    return View(employee);
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
                    Employee employee1 = ctx.Employees.Where(c => c.Id == id).SingleOrDefault();

                    ctx.Employees.Remove(employee1);
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
    }
}