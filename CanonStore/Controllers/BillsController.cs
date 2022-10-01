using CanonStore.Models;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;

namespace CanonStore.Controllers
{
    public class BillsController : Controller
    {
        db_CanonStoreEntities ctx = new db_CanonStoreEntities();
        // GET: Bills
        public ActionResult Index()
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    List<Bill> bills = ctx.Bills.OrderByDescending(b=>b.Date_Created).ToList();
                    Session["Bill"] = bills;
                    return View(bills);
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");

        }
        [HttpPost]
        public ActionResult Index(string date, string searchBy)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    List<Bill> bills = new List<Bill>();
                    List<Sumby> sumbies = new List<Sumby>();
                    DateTime dateTime = new DateTime();
                    if (date != null)
                    {
                        try
                        {
                            dateTime = Convert.ToDateTime(date);
                        }
                        catch (Exception)
                        {
                            return RedirectToAction("Index");
                        }

                        if (searchBy == "Day")
                        {
                            bills = ctx.Bills.Where(b => b.Date_Created.Month == dateTime.Month && b.Date_Created.Year == dateTime.Year && b.Date_Created.Day == dateTime.Day).ToList();
                            ViewBag.searchBill = "There are " + bills.Count + " bills on " + dateTime.ToString("dd/MM/yyyy");
                            //Sum sales by date
                            try
                            {
                                foreach(var item in ctx.proc_sumsalesbydate((DateTime?)dateTime))
                                {
                                    Sumby sumby = new Sumby
                                    {
                                        Day = item.Date_Created.ToString("dd/MM/yyyy"),
                                        Total = (float)item.Total
                                    };
                                    sumbies.Add(sumby);
                                }
                                ViewBag.proc = sumbies;
                                ViewBag.co = 1;
                            }
                            catch
                            {

                            }

                        }
                        else if (searchBy == "Month")
                        {
                            bills = ctx.Bills.Where(b => b.Date_Created.Month == dateTime.Month && b.Date_Created.Year == dateTime.Year).ToList();
                            ViewBag.searchBill = "There are " + bills.Count + " bills on " + dateTime.ToString("MM/yyyy");
                            try
                            {
                                foreach (var item in ctx.proc_sumsalesbymonth((DateTime?)dateTime))
                                {
                                    Sumby sumby = new Sumby
                                    {
                                        Day = item.Date_Created,
                                        Total = (float)item.Total
                                    };
                                    sumbies.Add(sumby);
                                }
                                ViewBag.proc = sumbies;
                               
                                ViewBag.co = 2;
                            }
                            catch
                            {

                            }
                        }
                        else if (searchBy == "Year")
                        {
                            bills = ctx.Bills.Where(b => b.Date_Created.Year == dateTime.Year).ToList();
                            ViewBag.searchBill = "There are " + bills.Count + " bills on " + dateTime.ToString("yyyy");
                            try
                            {
                                foreach (var item in ctx.proc_sumsalesbyyear((DateTime?)dateTime))
                                {
                                    Sumby sumby = new Sumby
                                    {
                                        Day = item.Date_Created.ToString(),
                                        Total = (float)item.Total
                                    };
                                    sumbies.Add(sumby);
                                }
                                ViewBag.proc = sumbies;
                              
                                ViewBag.co = 3;
                            }
                            catch
                            {

                            }
                        }
                        else if (searchBy == null)
                        {
                            bills = ctx.Bills.OrderByDescending(b => b.Date_Created).ToList();
                        }


                    }
                    else
                    {
                        bills = ctx.Bills.OrderByDescending(b => b.Date_Created).ToList();

                    }
                    if (bills.Count == 0)
                    {
                        ViewBag.searchBill = "No Bills on this day";
                    }
                    Session["BillSumby"] = sumbies;
                    Session["Bill"] = bills;
                    return View(bills);
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
                return View();

            }
            return RedirectToAction("Index", "Login");
        }
        public ActionResult Detail(string id)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    Bill bill = ctx.Bills.Where(c => c.IdBill == id).SingleOrDefault();

                    List<Bill_details> bill_Details = ctx.Bill_details.Where(c => c.IdBill == id).ToList();

                    ViewBag.bill_Details = bill_Details;

                    Customer customer = ctx.Customers.Where(c => c.Id == bill.IdCustomer).SingleOrDefault();
                    ViewBag.customer = customer;


                    Employee emloyee = ctx.Employees.Where(c => c.Id == bill.IdEmployee).SingleOrDefault();
                    ViewBag.emloyee = emloyee;

                    return View(bill);
                }
                catch
                {
                    return Content("Error");
                }

            }
            return RedirectToAction("Index", "Login");
        }
        public ActionResult Edit(String id)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    List<Customer> customers = ctx.Customers.ToList();
                    ViewBag.customers = customers;

                    List<Employee> emloyees = ctx.Employees.ToList();
                    ViewBag.emloyees = emloyees;

                    List<Bill_Status> bill_Statuses = ctx.Bill_Status.ToList();
                    ViewBag.bill_Statuses = bill_Statuses;

                    Bill bill = ctx.Bills.Where(c => c.IdBill == id).SingleOrDefault();
                    return View(bill);
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");

        }
        [HttpPost]
        public ActionResult Edit(Bill bill)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    Bill oldBill = ctx.Bills.Where(c => c.IdBill == bill.IdBill).SingleOrDefault();


                    //oldBill.IdCustomer = bill.IdCustomer;

                    oldBill.IdEmployee = bill.IdEmployee;
                    oldBill.Status = bill.Status;
                    //oldBill.Total = bill.Total;
                    //oldBill.Date_Created = bill.Date_Created;
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
        public ActionResult Delete(string id)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    Bill bill = ctx.Bills.Where(c => c.IdBill == id).SingleOrDefault();

                    List<Bill_details> bill_Details = ctx.Bill_details.Where(c => c.IdBill == id).ToList();

                    ViewBag.bill_Details = bill_Details;

                    Customer customer = ctx.Customers.Where(c => c.Id == bill.IdCustomer).SingleOrDefault();
                    ViewBag.customer = customer;


                    Employee emloyee = ctx.Employees.Where(c => c.Id == bill.IdEmployee).SingleOrDefault();
                    ViewBag.emloyee = emloyee;

                    return View(bill);
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");

        }

        public ActionResult DeleteConfirm(string id)
        {
            if (Session["EmployeeId"] != null)
            {
                try
                {
                    Bill bill = ctx.Bills.Where(c => c.IdBill == id).SingleOrDefault();
                    List<Bill_details> bill_Details = ctx.Bill_details.Where(c => c.IdBill == id).ToList();
                    ctx.Bills.Remove(bill);
                    foreach (var item in bill_Details)
                    {
                        ctx.Bill_details.Remove(item);
                    }
                    //xoa
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

        public ActionResult ExportExcel(string time)
        {
            if (Session["EmployeeId"] != null)
            {
                if (time != null)
                {
                    time = "SearchBy" + time;
                }
                else
                {
                    time = DateTime.Now.Date.ToString("dd_MM_yyyy");
                }

                List<Bill> bills = (List<Bill>)Session["Bill"];
                if (bills.Count != 0)
                {
                    try
                    {
                        DataTable dt = new DataTable("Grid");
                        dt.Columns.AddRange(new DataColumn[7] { new DataColumn("IdBill"),
                                            new DataColumn("Customer"),
                                            new DataColumn("Total"),
                                            new DataColumn("Status"),
                                            new DataColumn("Discount") ,
                                            new DataColumn("Discount Value") ,
                                            new DataColumn("Date Created")  });
                        foreach (var bill in bills)
                        {
                            Customer customer = ctx.Customers.Where(c => c.Id == bill.IdCustomer).FirstOrDefault();
                            Discount discount = ctx.Discounts.Where(d => d.Dis_Code == bill.Discount_code).FirstOrDefault();
                            Bill_Status bill_Status = ctx.Bill_Status.Where(bs => bs.IdStatus == bill.Status).FirstOrDefault();

                            dt.Rows.Add(bill.IdBill, customer.Name, "$" + "  " + bill.Total.Value.ToString("#,##0.00"), bill_Status.NameStatus, discount.Dis_Code, discount.Discount_Value, bill.Date_Created);
                        }
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            wb.Worksheets.Add(dt).Columns().AdjustToContents();
                            using (MemoryStream stream = new MemoryStream())
                            {
                                wb.SaveAs(stream);
                                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Bills Table_" + time + ".xlsx");
                            }
                        }
                    }
                    catch
                    {
                        return Content("Error");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Bills");
                }
            }
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        public ActionResult ExportPDF()
        {
            if (Session["EmployeeId"] != null)
            {
                List<Bill> bills = (List<Bill>)Session["Bill"];
                List<Sumby> sumbies = (List<Sumby>)Session["BillSumby"];
                try
                {
                    BillsReport billsReport = new BillsReport();
                    byte[] abytes = billsReport.PrepareReport(bills,sumbies);
                    return File(abytes, "application/pdf");
                }
                catch
                {
                    return Content("Error");
                }
            }
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        public ActionResult ExportPDFDetail()
        {
            string id = Request.Form["IdBill"];

            Bill bill = ctx.Bills.Where(c => c.IdBill == id).SingleOrDefault();

            List<Bill_details> bill_Details = ctx.Bill_details.Where(c => c.IdBill == id).ToList();

            ViewBag.bill_Details = bill_Details;

            Customer customer = ctx.Customers.Where(c => c.Id == bill.IdCustomer).SingleOrDefault();
            ViewBag.customer = customer;


            Employee emloyee = ctx.Employees.Where(c => c.Id == bill.IdEmployee).SingleOrDefault();
            ViewBag.emloyee = emloyee;
            try
            {
                BillsDetailReport billsDetailReport = new BillsDetailReport();
                byte[] abytes = billsDetailReport.PrepareReport(id);
                return File(abytes, "application/pdf");
            }
            catch
            {
                return Content("Error");
            }

        }
    }
}