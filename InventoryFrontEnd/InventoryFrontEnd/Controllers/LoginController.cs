using InventoryFrontEnd.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace InventoryFrontEnd.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated == true)
            {
                FormsAuthentication.RedirectFromLoginPage(User.Identity.Name, false);
            }

            return View("~/Views/Login/Index.cshtml");
        }


        [HttpPost]
        public ActionResult TryLogin(string add_user_name, string add_password)
        {
            WebRequest req;
            WebResponse res;
            string postData = "Username=" + add_user_name + "&Password=" + add_password;
            req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/LoginAccount/Login/all?" + postData);
            Byte[] data = Encoding.UTF8.GetBytes(postData);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req.ContentLength = data.Length;

            Stream stream = req.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            using (res = req.GetResponse())
            using (var reader = new StreamReader(res.GetResponseStream()))
            {
                string msg = reader.ReadToEnd();
                string[] parts = msg.Split('|');
                if (parts[0].Contains("Successfully Logged In"))
                {
                    string firstName = parts[1];
                    string lastName = parts[2];
                    string campusName = parts[3];
                    string role = parts[4];
                    Session["UserName"] = add_user_name;
                    Session["FirstName"] = firstName;
                    Session["LastName"] = lastName;
                    Session["CampusName"] = campusName;
                    Session["UserRole"] = role;
                    FormsAuthentication.RedirectFromLoginPage(add_user_name, false);
                    return new EmptyResult();
                }
                else if (msg.Contains("Invalid username or password"))
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    return Content("Unhandled API response: " + msg, "text/plain", Encoding.UTF8);
                }
            }
        }

    }
}