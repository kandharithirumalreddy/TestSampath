using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using FormSubmit.Models;
using System.Net.Mail;
using System.Configuration;
using System.Net;

namespace FormSubmit.Controllers
{
    public class formController : Controller
    {
        FormDataContext fb = new Models.FormDataContext();
        public ViewResult index()
        {
            return View(fb.FormTable.ToList());
        }
        public ActionResult create()
        {

            return View();
        }
        
        
        //The Following code will validate the form data and sends mail to to the Client
        [HttpPost]
        public ActionResult Create(FormCollection formCollection)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    FormModel formModel = new FormModel();
                    formModel.UserName = formCollection["UserName"].ToLower();
                    formModel.Email = formCollection["Email"];
                    formModel.DOB = DateTime.Parse(formCollection["DOB"]);
                    formModel.Mobilenumber = (formCollection["Mobilenumber"]);

                    var searchData = fb.FormTable.Where(x => x.UserName == formModel.UserName).SingleOrDefault();
                    if (searchData != null)
                    {
                        TempData["IsEmails"] = "User already exist ! Choose other";
                        return RedirectToAction("Index");
                    }
                    //formModel.IsEmail =bool.Parse( formCollection["IsEmail"]);
                    fb.FormTable.Add(formModel);
                    fb.SaveChanges();

                    using (MailMessage mm = new MailMessage())
                    {
                        mm.From = new MailAddress(ConfigurationManager.AppSettings["fromEmail"].ToString());
                        mm.To.Add(formModel.Email);
                        mm.Subject = "Account Activation";
                        string body = "Hello " + formModel.UserName + ",";
                        body += "<br /><br /><b>Your registration has been successful! </b><br />";
                        body += "<br />Following are your registration details <br /><br />UserName : " + formModel.UserName + "<br />Email : " + formModel.Email + "<br /> DOB : " + formModel.DOB + "<br /> MobileNumber : " + formModel.Mobilenumber + "";
                        body += "<br /><br />Thanks";
                        mm.Body = body;
                        mm.IsBodyHtml = true;

                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        NetworkCredential NetworkCred = new NetworkCredential(ConfigurationManager.AppSettings["fromEmail"].ToString(), ConfigurationManager.AppSettings["EMAILPASSWORD"].ToString());

                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);

                        TempData["IsEmails"] = "Your mail sent successfully";

                    }
                }
                return RedirectToAction("Index");
               
            }
            catch (Exception ex)
            {              
                
                TempData["IsEmails"] = "Invalid mail";
                return RedirectToAction("Index");

            }
            
        }


        //The following code will validate the already exist (registred) user
        [HttpGet]
        public ActionResult checknameAvailable(string userdata)
        {
            try
            {
                System.Threading.Thread.Sleep(200);
                var searchData = fb.FormTable.Where(x => x.UserName == userdata).SingleOrDefault();
                if (searchData != null)
                {
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


    }
}