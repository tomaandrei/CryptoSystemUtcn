using CryptoSystemDissertation.BusinessLogic;
using CryptoSystemDissertation.Common;
using CryptoSystemDissertation.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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
                    Lambda = randome.GenerateLambdaRandomNumber(),
                    X = randome.GenerateXRandomNumber()
                };
                imageDetails.Image = null;
                imageDetails.ImageId = Guid.NewGuid().ToString();
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
                    var images = db.ImageDetails.Where(r => r.ReceiverId == crtUser.UserID.ToString());
                    if (images != null)
                    {
                        var imageId = new List<string>();
                        var senderId = new List<string>();
                        var senderName = new List<string>();
                        foreach (var image in images)
                        {
                            imageId.Add(image.ImageId.ToString());
                            senderId.Add(image.SenderId);
                            var user = db.UserAccount.Where(u => u.UserID.ToString() == image.SenderId).FirstOrDefault();
                            senderName.Add(user.FirstName + " " + user.LastName);

                        }
                        return Json(new
                        {
                            SenderName = senderName,
                            SenderId = senderName,
                            ImageId = imageId
                        });
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
        public ActionResult ReceiveImage(SendDetails send)
        {
            var crtUser = SessionManager.ReturnSessionObject("User") as UserAccount;
            if (crtUser != null)
            {
                using (CryptoDbContext db = new CryptoDbContext())
                {
                    var image = db.ImageDetails.Find(send.ImageId);
                    if (image != null)
                    {
                        var imagesAndParameters = new List<string>();
                        var encryptParameters = new EncryptParameters<Parameters>(send.RSAKey, image.Parameters);
                        var parameters = encryptParameters.Encrypt();
                        db.ImageDetails.Remove(image);
                        db.SaveChanges();

                        return Json(new { Image = image.Image, Parmateres = parameters });
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