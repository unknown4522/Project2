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
    public class ApparelController : Controller
    {
        // SHOW ALL AVAILABLE APPAREL

        public ActionResult Apparel_stock(string Campus_name)
        {
            Modelall mymodel = new Modelall();
            mymodel.Allapparel_stock = Apparelstock(Campus_name);
            mymodel.Allgrades = grades(Campus_name);
            mymodel.Allsize = sizes(Campus_name);
            mymodel.Allapparel_type = clothes_type(Campus_name);
            mymodel.AllApparelavailablecounters = counterapparelavil(Campus_name);
            ViewBag.campus_name = Campus_name;
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("", "login");
            }
            else
            {
                return View("~/Views/Apparel/Apparel_stock.cshtml", mymodel);
            }

        }

        public IEnumerable<Apparel_stock> Apparelstock(string Campus_name)
        {

            IEnumerable<Apparel_stock> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/Available/");

            var consumedata = hc.GetAsync("stock?campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Apparel_stock>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Apparel_stock>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Apparel_stock>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }


        // ADD UNIFORM

        [HttpPost]
        [Route("insert/feedback/view2")]
        public ActionResult adduniform(string add_grade, string add_uniform_type, string add_sizes, string add_stocks, string add_campus_name)
        {


            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Grade_level=" + add_grade + "&Apparel_type=" + add_uniform_type + "&Size=" + add_sizes + "&Quantity=" + add_stocks + "&Campus_name=" + add_campus_name;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/ADD/Apparelavailable?" + postData);
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
                        msg = reader.ReadToEnd();


                        string referringUrl = Request.UrlReferrer != null ? Request.UrlReferrer.AbsoluteUri : "";


                        return Redirect(referringUrl);
                    }
                }
                catch (WebException ex)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    Stream receiveStream = res.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return Content("Catch: " + readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                Stream receiveStream = res.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return Content(readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                }
            }
        }

        // ADD Quantity

        [HttpPost]
        public ActionResult addstock(string add_uniform_id, string add_stock, string add_transaction, string add_grade_level, string add_clothes_type, string add_size, string add_campus_name)
        {


            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Apparel_ID=" + add_uniform_id + "&stockChange=" + add_stock + "&Transaction=" + add_transaction + "&Grade_level=" + add_grade_level + "&Apparel_type=" + add_clothes_type + "&Size=" + add_size + "&Campus_name=" + add_campus_name;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/ADD/STOCK?" + postData);
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
                        msg = reader.ReadToEnd();

                        // Get the referring URL (current page)
                        string referringUrl = Request.UrlReferrer != null ? Request.UrlReferrer.AbsoluteUri : "";

                        // Redirect back to the referring URL (current page)
                        return Redirect(referringUrl);
                    }
                }
                catch (WebException ex)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    Stream receiveStream = res.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return Content("Catch: " + readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                Stream receiveStream = res.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return Content(readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                }
            }
        }

        // SHOW ALL GRADES

        public ActionResult grade_list(string Campus_name)
        {
            Modelall mymodel = new Modelall();
            mymodel.Allgrades = grades(Campus_name);
            mymodel.AllGradecounters = countergrade(Campus_name);
            ViewBag.firstName = Session["FirstName"];
            ViewBag.LastName = Session["LastName"];
            ViewBag.campusName = Session["campusName"];
            ViewBag.campus_name = Campus_name;
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("", "login");
            }
            else
            {
                return View("~/Views/Apparel/grade_list.cshtml", mymodel);
            }
        }

        public IEnumerable<Grades> grades(string Campus_name)
        {

            IEnumerable<Grades> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/Grade/");

            var consumedata = hc.GetAsync("list?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Grades>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Grades>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Grades>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }



        //ADD Grade level
        [HttpPost]
        public ActionResult addgradelvl(string add_gradelvl, string add_campus_name)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Grade_level=" + add_gradelvl + "&Campus_name=" + add_campus_name;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/ADD/Grade?" + postData);
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
                        msg = reader.ReadToEnd();

                        // Redirect back to the referring page
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                }
                catch (WebException ex)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    Stream receiveStream = res.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return Content("Catch: " + readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                Stream receiveStream = res.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return Content(readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                }
            }
        }

        //MODIFY GRADE LEVEL

        [HttpPost]
        public ActionResult modifygrade(string updateID, string updategrade, string updatecampus)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "ID=" + updateID + "&Grade_level=" + updategrade + "&Campus_name=" + updatecampus;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/Modify/Gradelvl?" + postData);
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
                        msg = reader.ReadToEnd();

                        // Redirect back to the referring page
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                }
                catch (WebException ex)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    Stream receiveStream = res.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return Content("Catch: " + readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                Stream receiveStream = res.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return Content(readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                }
            }
        }


        //SHOW ALL APPAREL TYPE

        public ActionResult Appareltype_list(string Campus_name)
        {
            Modelall mymodel = new Modelall();
            mymodel.Allapparel_type = clothes_type(Campus_name);
            mymodel.Allappareltypecounters = counterappareltype(Campus_name);
            ViewBag.campus_name = Campus_name;
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("", "login");
            }
            else
            {
                return View("~/Views/Apparel/Appareltype_list.cshtml", mymodel);
            }
        }

        public IEnumerable<Apparel_type_list> clothes_type(string Campus_name)
        {

            IEnumerable<Apparel_type_list> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/Appareltype/");

            var consumedata = hc.GetAsync("list?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Apparel_type_list>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Apparel_type_list>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Apparel_type_list>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }


        //SHOW ALL SIZES

        public ActionResult size_list(string Campus_name)
        {
            Modelall mymodel = new Modelall();
            mymodel.Allsize = sizes(Campus_name);
            mymodel.AllSizecounters = counterSize(Campus_name);
            ViewBag.campus_name = Campus_name;
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("", "login");
            }
            else
            {
                return View("~/Views/Apparel/size_list.cshtml", mymodel);
            }

        }

        public IEnumerable<Apparel_Size> sizes(string Campus_name)
        {

            IEnumerable<Apparel_Size> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/Size/");

            var consumedata = hc.GetAsync("list?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Apparel_Size>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Apparel_Size>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Apparel_Size>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }

        //ADD SIZES
        [HttpPost]
        public ActionResult addsizefunc(string add_sizes, string add_campusname)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Size=" + add_sizes + "&Campus_name=" + add_campusname;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/add/Size?" + postData);
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
                        msg = reader.ReadToEnd();

                        // Redirect back to the referring page
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                }
                catch (WebException ex)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    Stream receiveStream = res.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return Content("Catch: " + readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                Stream receiveStream = res.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return Content(readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                }
            }
        }

        //MODIFY Sizez

        [HttpPost]
        public ActionResult modifysizes(string updateID, string updatesize, string updatecampus)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Size_ID=" + updateID + "&Size=" + updatesize + "&Campus_name=" + updatecampus;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/Modify/Sizes?" + postData);
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
                        msg = reader.ReadToEnd();

                        // Redirect back to the referring page
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                }
                catch (WebException ex)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    Stream receiveStream = res.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return Content("Catch: " + readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                Stream receiveStream = res.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return Content(readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                }
            }
        }

        // CLAIM STUB

        public ActionResult Claim_stub(string Campus_name)
        {
            Modelall mymodel = new Modelall();
            mymodel.Allclaimitems = Claimstub(Campus_name);
            mymodel.Allapparel_type = Appareltypelist(Campus_name);
            mymodel.Allgrades = grades(Campus_name);
            mymodel.Allclaimcounters = counterclaim(Campus_name);
            mymodel.Allsize = sizes(Campus_name);
            ViewBag.campus_name = Campus_name;
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("", "login");
            }
            else
            {
                return View("~/Views/Apparel/Claim_stub.cshtml", mymodel);
            }


        }

        public IEnumerable<Claim_stub> Claimstub(string Campus_name)
        {

            IEnumerable<Claim_stub> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/Claim/");

            var consumedata = hc.GetAsync("Apparel?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Claim_stub>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Claim_stub>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Claim_stub>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }

        // CLAIM PROCESS
        [HttpPost]
        public ActionResult claimapparel(string add_firstname, string add_lastname, string add_apparel, string add_reciept_number, string add_date_receive, string add_gradelevel, string add_quantity, string add_size, string add_campusname)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "First_name=" + add_firstname + "&Last_name=" + add_lastname + "&Apparel_name=" + add_apparel + "&Reciept_number=" + add_reciept_number + "&Date_receive=" + add_date_receive + "&Grade_level=" + add_gradelevel + "&Quantity=" + add_quantity + "&Size=" + add_size + "&Campus_name=" + add_campusname;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/add/claimitems?" + postData);
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
                        msg = reader.ReadToEnd();

                        
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                }
                catch (WebException ex)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    Stream receiveStream = res.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return Content("Catch: " + readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                Stream receiveStream = res.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return Content(readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                }
            }
        }


        //SHOW ALL APPAREL TYPE

        public ActionResult Clothes_list(string Campus_name)
        {
            Modelall mymodel = new Modelall();
            mymodel.Allapparel_type = Appareltypelist(Campus_name);
            ViewBag.firstName = Session["FirstName"];
            ViewBag.LastName = Session["LastName"];
            ViewBag.campusName = Session["campusName"];
            ViewBag.Campus_name = Campus_name;
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("", "login");
            }
            else
            {
                return View("~/Views/Apparel/Appareltypes.cshtml", mymodel);
            }
        }

        public IEnumerable<Apparel_type_list> Appareltypelist(string Campus_name)
        {

            IEnumerable<Apparel_type_list> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/Appareltype/");

            var consumedata = hc.GetAsync("list?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Apparel_type_list>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Apparel_type_list>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Apparel_type_list>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }

        //ADD APPAREL TYPE

        [HttpPost]
        public ActionResult Addappareltype(string add_clothes_type, string add_campus_name)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Apparel_type=" + add_clothes_type + "&Campus_name=" + add_campus_name;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/ADD/Appareltypes?" + postData);
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
                        msg = reader.ReadToEnd();

                        // Redirect back to the referring page
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                }
                catch (WebException ex)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    Stream receiveStream = res.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return Content("Catch: " + readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                Stream receiveStream = res.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return Content(readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                }
            }
        }

        //MODIFY APPAREL TYPE
        [HttpPost]
        public ActionResult modifyappareltype(string modify_apparel_type_id, string modify_apparel_type, string Modify_campus_name)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Apparel_ID=" + modify_apparel_type_id + "&Apparel_type=" + modify_apparel_type + "&Campus_name=" + Modify_campus_name;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/Modify/Apparel?" + postData);
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
                        msg = reader.ReadToEnd();

                        // Redirect back to the referring page
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                }
                catch (WebException ex)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    Stream receiveStream = res.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return Content("Catch: " + readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                Stream receiveStream = res.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return Content(readStream.ReadToEnd(), "text/plain", Encoding.UTF8);
                }
            }
        }

        //SHOW ALL APPAREL RECORDS

        public ActionResult Apparel_logs_View(string Campus_name)
        {
            Modelall mymodel = new Modelall();
            mymodel.Allapparellogs_data = Apparel_logs(Campus_name);
            mymodel.Allapparelrecordcounters = counterapparelrecords(Campus_name);
            ViewBag.campus_name = Campus_name;
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("", "login");
            }
            else
            {
                return View("~/Views/Apparel/Apparel_logs_View.cshtml", mymodel);
            }

        }

        public IEnumerable<Apparel_logs> Apparel_logs(string campus_name)
        {

            IEnumerable<Apparel_logs> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Apparel/uniformlogs/");

            var consumedata = hc.GetAsync("logs?campus_name=" + campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Apparel_logs>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Apparel_logs>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Apparel_logs>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }


        public IEnumerable<Apparelavailablecounter> counterapparelavil(string Campus_name)
        {

            IEnumerable<Apparelavailablecounter> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Dashboard/Apparel/");

            var consumedata = hc.GetAsync("Counter?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Apparelavailablecounter>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Apparelavailablecounter>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Apparelavailablecounter>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }
        //GRADE COUNTER

        public IEnumerable<Gradecounter> countergrade(string Campus_name)
        {

            IEnumerable<Gradecounter> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Dashboard/Apparelgrade/");

            var consumedata = hc.GetAsync("Counter?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Gradecounter>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Gradecounter>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Gradecounter>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }

        //Size Counter

        public IEnumerable<Sizecounter> counterSize(string Campus_name)
        {

            IEnumerable<Sizecounter> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Dashboard/Apparelsize/");

            var consumedata = hc.GetAsync("Counter?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Sizecounter>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Sizecounter>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Sizecounter>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }

        // CLAIM STUB COUNTER
        public IEnumerable<clainapparelcounter> counterclaim(string Campus_name)
        {

            IEnumerable<clainapparelcounter> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Dashboard/Apparelclaim/");

            var consumedata = hc.GetAsync("Counter?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<clainapparelcounter>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<clainapparelcounter>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<clainapparelcounter>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }

        // Apparel Type COUNTER
        public IEnumerable<appareltypecounter> counterappareltype(string Campus_name)
        {

            IEnumerable<appareltypecounter> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Dashboard/Appareltype/");

            var consumedata = hc.GetAsync("Counter?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<appareltypecounter>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<appareltypecounter>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<appareltypecounter>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }

        // Apparel Record COUNTER

        public IEnumerable<apparelrecordcounter> counterapparelrecords(string Campus_name)
        {

            IEnumerable<apparelrecordcounter> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Dashboard/Apparellogs/");

            var consumedata = hc.GetAsync("Counter?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<apparelrecordcounter>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<apparelrecordcounter>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<apparelrecordcounter>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }



    }
}