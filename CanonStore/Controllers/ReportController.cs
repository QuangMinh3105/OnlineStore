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
    public class ReportController : Controller
    {
        db_CanonStoreEntities ctx = new db_CanonStoreEntities();
        // GET: Report
        public ActionResult Index()
        {
          
            return View();
        }
    }
}