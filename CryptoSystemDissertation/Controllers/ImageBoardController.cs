using CryptoSystemDissertation.BusinessLogic;
using CryptoSystemDissertation.Common;
using CryptoSystemDissertation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult ParametersImage(SendDetails senderJson)
        {
            var crtUser = SessionManager.ReturnSessionObject("User") as UserAccount;
            if (ModelState.IsValid)
            {
                if (crtUser != null)
                {
                    var imageDetails = SetImageDetails(crtUser.UserID.ToString(), senderJson.ReceiverId);
                    var parameters = this.GetEncryptParameters(imageDetails, senderJson.RSAKey);

                    return Json(new { encryptParam = parameters, imageDetails.ImageId });
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult ImageString(SendDetails senderJson)
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
                    var imageDetails = db.ImageDetails.Find(senderJson.ImageId);
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

            var aesEncrypt = new AESEncryption<Parameters>(SetParameters());
            var IV = aesEncrypt.GenerateAesKeys();
            var encryptParameters = aesEncrypt.EncryptAES();

            using (CryptoDbContext db = new CryptoDbContext())
            {              
                imageDetails.SenderId = senderId;
                imageDetails.ReceiverId = receiverId;
                imageDetails.Parameters = encryptParameters;
                imageDetails.IVAes = IV;
                imageDetails.Image = null;
                imageDetails.ImageId = Guid.NewGuid().ToString();
                db.ImageDetails.Add(imageDetails);
                db.SaveChanges();
            }
            return imageDetails;
        }

        [HttpGet]
        public ActionResult ReceiveImage()
        {
            var crtUser = SessionManager.ReturnSessionObject("User") as UserAccount;
            if (crtUser != null)
            {
                using (CryptoDbContext db = new CryptoDbContext())
                {
                    var images = db.ImageDetails.Where(r => r.ReceiverId == crtUser.UserID.ToString());
                    if (images != null && images.Any())
                    {
                        var imageId = new List<string>();
                        var senderId = new List<string>();
                        var senderName = new List<string>();
                        foreach (var image in images)
                        {
                            if (image.Image != null)
                            {
                                imageId.Add(image.ImageId.ToString());
                                senderId.Add(image.SenderId);
                                var user = db.UserAccount.Where(u => u.UserID.ToString() == image.SenderId).FirstOrDefault();
                                senderName.Add(user.FirstName + " " + user.LastName);
                            }
                            else
                            {
                                db.ImageDetails.Remove(image);
                            }

                        }
                        return Json(new
                        {
                            SenderName = senderName,
                            SenderId = senderId,
                            ImageId = imageId
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            SenderName = String.Empty,
                            SenderId = String.Empty,
                            ImageId = String.Empty
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult GetImageDetails(SendDetails send)
        {
            ViewBag.SenderId = send.SenderId;
            ViewBag.ImageId = send.ImageId;

            return View();
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
                        var parameters = this.GetEncryptParameters(image, send.RSAKey);
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

        private string GetEncryptParameters(ImageDetails image, string RSAKey)
        {
            var aesDecrypt = new AESDecryption<Parameters>(image.Parameters, image.IVAes);
            var plainParameters = aesDecrypt.DecryptParameters();

            var encryptParameters = new RSAEncryptParameters<Parameters>(RSAKey, plainParameters);
            var parameters = encryptParameters.Encrypt();

            return parameters;
        }

        private Parameters SetParameters()
        {
            var random = new RandomParameters();
            var parameters = new Parameters
            {
                Lambda = random.GenerateLambdaRandomNumber(),
                X = random.GenerateXRandomNumber(),
                T = random.GenerateTRandomNumber(),
                A = random.GenerateARandomNumber(),
                C0 = random.GenerateTRandomNumber(),
                B = random.GenerateBRandomNumber()
            };

            return parameters;
        }       
    }
}