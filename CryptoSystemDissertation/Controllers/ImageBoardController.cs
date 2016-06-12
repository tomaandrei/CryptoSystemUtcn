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
                if (crtUser == null)
                {
                    return RedirectToAction("Login");
                }
                using (CryptoDbContext db = new CryptoDbContext())
                {
                    var imageDetails = db.ImageDetails.Where(i => i.SenderId == crtUser.UserID.ToString() && i.ReceiverId == senderJson.ReceiverId).FirstOrDefault();
                    if (imageDetails == null)
                    {
                        ViewBag.Message = "Error something was wrong!";
                        return null;
                    }
                    imageDetails.Image = senderJson.Image;
                    db.SaveChanges();
                    ViewBag.Message = "The image was successfully add in Database!";    
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

        [HttpPost]
        public ActionResult ReceiveImage()
        {
            var crtUser = SessionManager.ReturnSessionObject("User") as UserAccount;
            if (crtUser != null)
            {
                using (CryptoDbContext db = new CryptoDbContext())
                {
                    var receive = db.ImageDetails.Where(r => r.ReceiverId == crtUser.UserID.ToString()).FirstOrDefault();
                    if (receive != null)
                    {
                        return Json(new { message = "You have an image" });
                    }
                    else
                    {
                        return Json(new { message = "No image" });
                    }
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult ReceiveImage(Sender sender)
        {
            var crtUser = SessionManager.ReturnSessionObject("User") as UserAccount;
            if (crtUser != null)
            {
                using (CryptoDbContext db = new CryptoDbContext())
                {
                    var receive = db.ImageDetails.Where(r => r.ReceiverId == crtUser.UserID.ToString()).FirstOrDefault();
                    if(receive!=null)
                    {
                        var encryptParameters = new EncryptParameters<Parameters>(sender.RSAKeyXML, receive.Parameters);
                        var parameters = encryptParameters.Encrypt();
                        db.ImageDetails.Remove(receive);
                        db.SaveChanges();
                        return Json(new { Parameters = parameters, EncryptImage = receive.Image });
                    }
                    else
                    {
                        return Json(new { Message = "No image" });
                    }
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        } 
    }
}