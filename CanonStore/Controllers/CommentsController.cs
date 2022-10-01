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

namespace CanonStore.Controllers
{
    public class CommentsController : Controller
    {
        db_CanonStoreEntities ctx = new db_CanonStoreEntities();
        // GET: Comments
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    ViewBag.CurrentSort = sortOrder;
                    ViewBag.RateSortParm = sortOrder == "rate" ? "rate_desc" : "rate";
                    ViewBag.DateSortParm = sortOrder == "date" ? "date_desc" : "date";

                    if (searchString != null)
                    {
                        page = 1;
                    }
                    else
                    {
                        searchString = currentFilter;
                    }

                    ViewBag.CurrentFilter = searchString;

                    var commnets = from p in ctx.Comments select p;

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        commnets = commnets.Where(s => s.Com_txt.Contains(searchString));
                    }

                    switch (sortOrder)
                    {
                        case "rate":
                            commnets = commnets.OrderBy(s => s.Rating);
                            break;
                        case "rate_desc":
                            commnets = commnets.OrderByDescending(s => s.Rating);
                            break;
                        case "date":
                            commnets = commnets.OrderBy(s => s.Date);
                            break;
                        case "date_desc":
                            commnets = commnets.OrderByDescending(s => s.Date);
                            break;
                        default:  // Name ascending 
                            commnets = commnets.OrderBy(s => s.Id);
                            break;
                    }

                    int pageSize = 5;
                    int pageNumber = (page ?? 1);
                    ViewBag.products = commnets;
                    return View(commnets.ToPagedList(pageNumber, pageSize));
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
                    Comment comment = ctx.Comments.Where(c => c.Id == id).SingleOrDefault();
                    ViewBag.productId = id;
                    //passing data / model to view
                    return View(comment);
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");


        }
        [HttpPost]
        public ActionResult Edit(Comment comment)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    try
                    {
                        Comment oldComment = ctx.Comments.Where(c => c.Id == comment.Id).SingleOrDefault();

                       
                        oldComment.Com_txt = comment.Com_txt;
                      
                        ctx.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Edit", comment.Id);
                    }
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