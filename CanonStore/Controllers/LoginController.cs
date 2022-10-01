using CanonStore.Commom;
using CanonStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CanonStore.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(EmloyeeCheck emloyeeCheck)
        {
            try
            {
                using (db_CanonStoreEntities db = new db_CanonStoreEntities())
                {
                    Password EncryptData = new Password();

                    emloyeeCheck.Password = EncryptData.Encode(emloyeeCheck.Password);
                    var obj = db.Employees.Where(a => a.UserName == emloyeeCheck.UserName && a.Password == emloyeeCheck.Password).FirstOrDefault();

                    if (obj != null)
                    {
                        Session["EmployeeId"] = obj.Id.ToString();
                        Session["EmployeeUserName"] = obj.UserName.ToString();
                        Session["EmployeeName"] = obj.Name.ToString();
                        //ViewBag.EmloyeeName = (Emloyees.Name)HttpContext.Session["EmployeeId"];
                        //ViewBag.EmloyeeName = HttpContext.Session.SessionID;

                        return RedirectToAction("Index","Products");
                    }
                    else 
                    {
                        return View();
                    }
                }
               
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Index");
        }
    }
}