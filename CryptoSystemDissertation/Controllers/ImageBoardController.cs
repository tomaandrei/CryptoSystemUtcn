using CryptoSystemDissertation.BusinessLogic;
using CryptoSystemDissertation.Common;
using CryptoSystemDissertation.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoSystemDissertation.Controllers
{
    public class ImageBoardController : Controller
    {
        // GET: ImageBoard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ParametersImage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ParametersImage(Sender senderJson)
        {                     
            var crtUser = SessionManager.ReturnSessionObject("User") as UserAccount;
            if (ModelState.IsValid)
            {
                    if (crtUser != null)
                    {
                        var imageDetails = SetImageDetails(crtUser.UserID.ToString(), senderJson.ReceiverId);                      
                        var encryptParameters = new EncryptParameters<Parameters>(senderJson.RSAKeyXML, imageDetails.Parameters);
                        var parameters = encryptParameters.Encrypt();

                        return Json(new { encryptParam = parameters });
                    }
                    else
                    {
                        return RedirectToAction("Login");
                    }             
            }
            return View();
        }

        [HttpPost]
        public ActionResult ImageString(Sender senderJson)
        {
            var crtUser = SessionManager.ReturnSessionObject("User") as UserAccount;
            if (ModelState.IsValid)
            {
                if (crtUser != null)
                {
                    using (CryptoDbContext db = new CryptoDbContext())
                    {
                        var sender = db.ImageDetails.Where(u => u.SenderId == crtUser.UserID.ToString() && u.ReceiverId== senderJson.ReceiverId).FirstOrDefault();
                        if (sender != null)
                        {
                            sender.Image = senderJson.Image;
                            db.SaveChanges();
                            ViewBag.Message = "The image was successfully add in Database!";
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        private ImageDetails SetImageDetails(string senderId, string receiverId)
        {
            var imageDetails = new ImageDetails();
            using (CryptoDbContext db = new CryptoDbContext())
            {
                var randome = new RandomParameters();
                imageDetails.SenderId = senderId;
                imageDetails.ReceiverId = receiverId;
                imageDetails.Parameters = new Parameters
                {
                    ParamtersId = int.Parse(senderId),
                    Lambda = randome.GenerateLambdaRandomNumber(),
                    X = randome.GenerateXRandomNumber()
                };
                imageDetails.Image = null;
                db.ImageDetails.Add(imageDetails);
                db.SaveChanges();
            }
            return imageDetails;
        }
    }
}