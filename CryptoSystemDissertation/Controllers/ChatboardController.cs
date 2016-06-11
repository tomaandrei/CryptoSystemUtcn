using CryptoSystemDissertation.Common;
using CryptoSystemDissertation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoSystemDissertation.Controllers
{
    public class ChatboardController : Controller
    {
        // GET: Chatboard
        [CryptoAuthorize]
        public ActionResult Index()
        {
            using (CryptoDbContext db = new CryptoDbContext())
            {
                return View(db.UserAccount.ToList());
            }
        }
    }
}