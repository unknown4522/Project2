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
    public class InventoryController : Controller
    {

        //SHOW ALL Campus
        public ActionResult Campus()
        {
            Modelall mymodel = new Modelall();
            mymodel.Allcampus = campuses();
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("", "login");
            }
            else
            {
                return View("~/Views/Inventory/Campus.cshtml", mymodel);
            }
           

        }

        public IEnumerable<Campuses> campuses()
        {

            IEnumerable<Campuses> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/");

            var consumedata = hc.GetAsync("Campuses");
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Campuses>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Campuses>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Campuses>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }

        // ADD CAMPUS
        [HttpPost]
        [Route("insert/feedback/view2")]
        public ActionResult AddCampuscontroll(string add_campusnamevalue)
        {


            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Campus_name=" + add_campusnamevalue;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Add/Campus?" + postData);
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

        //UPDATE CAMPUS
        [HttpPost]
        public ActionResult updatecampus(string update_Campus_id, string update_campus_name)
        {


            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Campus_id=" + update_Campus_id + "&Campus_name=" + update_campus_name;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Modify/Campus?" + postData);
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

        //SHOW ALL ITEMS
        public ActionResult Item_list(string Campus_name)
        {
            Modelall mymodel = new Modelall();
            mymodel.Item_list = Allitems(Campus_name);
            mymodel.Allbrand = Brands(Campus_name);
            mymodel.Room_list = Allrooms(Campus_name);

            mymodel.Allroomcounters = counterallrooms(Campus_name);
            mymodel.AllITassetcounters = counterallITAsset(Campus_name);
            mymodel.Alllogscounters = counterlogs(Campus_name);
            ViewBag.Campus_name = Campus_name;
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("", "login");
            }
            else
            {
                return View("~/Views/Inventory/item_list.cshtml", mymodel);
            }

        }
        public IEnumerable<Items> Allitems(string Campus_name)
        {

            IEnumerable<Items> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Item/");

            var consumedata = hc.GetAsync("All?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Items>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Items>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Items>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }

        //ADD ITEMS 

        [HttpPost]
        public ActionResult createitems(string add_item_name, string add_model, string add_brand, string add_serial_number, string add_item_type, string add_item_status, string add_item_department, string add_date, string add_room, string add_campus)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Item_name=" + add_item_name + "&Model=" + add_model + "&Brand=" + add_brand + "&Serial_number=" + add_serial_number + "&Item_type=" + add_item_type + "&Item_status=" + add_item_status + "&Department=" + add_item_department + "&Date_now=" + add_date + "&Room_name=" + add_room + "&Campus_name=" + add_campus;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Add/Newitems?" + postData);
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


        //UPDATE ITEMS 
        [HttpPost]
        public ActionResult updateitems(string update_item_name, string update_model, string update_brand, string update_serial_number, string update_item_type, string update_room_name, string update_item_status, string update_department, string update_ID, string update_add_campus)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Item_name=" + update_item_name + "&Model=" + update_model + "&Brand=" + update_brand + "&Serial_number=" + update_serial_number + "&Item_type=" + update_item_type + "&Room_name=" + update_room_name + "&item_Status=" + update_item_status + "&Department=" + update_department + "&ID=" + update_ID + "&Campus_name=" + update_add_campus;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Update/Items?" + postData);
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


        //SHOW ALL BRAND

        public ActionResult brandlist(string Campus_name)
        {
            Modelall mymodel = new Modelall();
            mymodel.Allbrand = Brands(Campus_name);
            mymodel.Allbrandcounters = counterbrand(Campus_name);
            ViewBag.Campus_name = Campus_name;
            ViewBag.firstName = Session["FirstName"];
            ViewBag.LastName = Session["LastName"];
            ViewBag.campusName = Session["campusName"];
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("", "login");
            }
            else
            {
                return View("~/Views/Inventory/brandlist.cshtml", mymodel);
            }

        }

        public IEnumerable<Brand> Brands(string Campus_name)
        {

            IEnumerable<Brand> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Brand/");

            var consumedata = hc.GetAsync("List?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Brand>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Brand>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Brand>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }

        // ADD BRAND 

        [HttpPost]
        [Route("insert/feedback/view2")]
        public ActionResult addbrand(string add_brand, string add_campus)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Brand_name=" + add_brand + "&Campus_name=" + add_campus;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Add/Brand?" + postData);
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



        // MODIFY BRAND

        [HttpPost]
        public ActionResult updatebrand(string update_brand_id, string update_brand, string update_Campus_name)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Brand_ID=" + update_brand_id + "&Brand_name=" + update_brand + "&Campus_name=" + update_Campus_name;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Modify/brand?" + postData);
                    Byte[] data = Encoding.UTF8.GetBytes(postData);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                    req.ContentLength = data.Length;

                    using (Stream stream = req.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

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
            catch (Exception ex)
            {
                return Content("Catch: " + ex.Message, "text/plain", Encoding.UTF8);
            }
        }

        // SHOW ALL ROOMS 
        public ActionResult rooms(string Campus_name, string Counter)
        {
            Modelall mymodel = new Modelall();
            mymodel.Room_list = Allrooms(Campus_name);
            mymodel.AllGradecounters = countergrade(Campus_name);
            ViewBag.firstName = Session["FirstName"];
            ViewBag.LastName = Session["LastName"];
            ViewBag.campusName = Session["campusName"];
            ViewBag.campus_name = Campus_name;
            ViewBag.counter = Counter;
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("", "login");
            }
            else
            {
                return View("~/Views/Inventory/rooms.cshtml", mymodel);
            }


        }


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


        public IEnumerable<Rooms> Allrooms(string Campus_name)
        {
            try
            {
                IEnumerable<Rooms> ec = null;
                HttpClient hc = new HttpClient();
                hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Rooms/");

                var consumedata = hc.GetAsync("List?Campus_name=" + Campus_name);
                consumedata.Wait();

                var dataread = consumedata.Result;
                if (dataread.IsSuccessStatusCode)
                {
                    var results = dataread.Content.ReadAsAsync<IList<Rooms>>();
                    results.Wait();
                    ec = results.Result;
                }
                else if (dataread.StatusCode == HttpStatusCode.NotFound)
                {
                    ec = Enumerable.Empty<Rooms>();
                    ViewBag.Message = "No Rooms Found for the selected campus.";
                }
                else
                {
                    ec = Enumerable.Empty<Rooms>();
                    ViewBag.Message = "Unable to retrieve room data. Please contact support for assistance.";
                }
                return ec;
            }
            catch (Exception ex)
            {

                ViewBag.Message = "An error occurred: " + ex.Message;
                return Enumerable.Empty<Rooms>();
            }
        }


        //ADD ROOMS
        [HttpPost]
        public ActionResult createrooms(string add_room_name, string add_floor_level, string add_room_description, string add_campus_name)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Room_name=" + add_room_name + "&Floor_level=" + add_floor_level + "&Room_description=" + add_room_description + "&Campus_name=" + add_campus_name;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Add/Newroom?" + postData);
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

        //MODIFY ROOM

        [HttpPost]
        public ActionResult updaterooms(string update_room_name, string update_floorlevel, string update_room_description, string update_room_id, string add_campus_name)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "ID=" + update_room_id + "&Room_name=" + update_room_name + "&Floor_level=" + update_floorlevel + "&Room_description=" + update_room_description + "&Campus_name=" + add_campus_name;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Modify/room?" + postData);
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





        // SHOW ALL EMPLOYEE

        public ActionResult employeelist(string Campus_name, string Role)
        {
            Modelall mymodel = new Modelall();
            mymodel.Allcampus = campuses();
            mymodel.Allemployeecounters = counterallemployee(Campus_name);
            mymodel.Employees = employee_acc(Campus_name);
            ViewBag.firstName = Session["FirstName"];
            ViewBag.LastName = Session["LastName"];
            ViewBag.campusName = Session["campusName"];
            ViewBag.campus_name = Campus_name;
            ViewBag.Role = Role;
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("", "login");
            }
            else
            {
                return View("~/Views/Inventory/employeelist.cshtml", mymodel);
            }


            
        }

        public IEnumerable<Employee_Account> employee_acc(string campus_name)
        {

            IEnumerable<Employee_Account> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "api/inventory/Employee/");

            var consumedata = hc.GetAsync("List?campus_name=" + campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Employee_Account>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Employee_Account>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Employee_Account>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }



        //ADD EMPLOYEE

        [HttpPost]
        [Route("insert/feedback/view2")]
        public ActionResult addemployee(string add_employee, string add_employee_last_name, string add_campus_name)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Firstname=" + add_employee + "&Lastname=" + add_employee_last_name + "&Campus_name=" + add_campus_name;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "api/inventory/Add/employeename?" + postData);
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

        //MIDIFY EMPLOYEE 

        [HttpPost]
        [Route("insert/feedback/view2")]
        public ActionResult updateemployee(string id_employee, string update_employee, string update_employee_last_name, string update_campus_name, string update_username, string update_password, string update_role)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "ID=" + id_employee + "&Firstname=" + update_employee + "&Lastname=" + update_employee_last_name + "&Campus_name=" + update_campus_name + "&Username=" + update_username + "&Password=" + update_password + "&Role=" + update_role;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Modify/Employee?" + postData);
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


        //CREATE EMPLOYEE ACCOUNT 
        [HttpPost]
        public ActionResult createemplyeeaccount(string add_employee_name, string add_employee_lastname, string add_campus_name, string Add_Username, string add_Password, string add_Role)
        {
            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Firstname=" + add_employee_name + "&Lastname=" + add_employee_lastname + "&Campus_name=" + add_campus_name + "&Username=" + Add_Username + "&Password=" + add_Password + "&Role=" + add_Role;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/CreateAccount/Create/account?" + postData);
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


        //SHOW ALL ITEM LOGS

        public ActionResult itemlogs(string Campus_name)
        {
            Modelall mymodel = new Modelall();
            mymodel.Item_logs_data = allhistory(Campus_name);
            mymodel.Item_logs_data = mymodel.Item_logs_data
           .OrderByDescending(item => item.Date_transfer)
           .ToList();

            ViewBag.campus_name = Campus_name;
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("", "login");
            }
            else
            {
                return View("~/Views/Inventory/itemlogs.cshtml", mymodel);
            }

        }

        public IEnumerable<Item_logs> allhistory(string Campus_name)
        {

            IEnumerable<Item_logs> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Item/");

            var consumedata = hc.GetAsync("Logs?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Item_logs>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Item_logs>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Item_logs>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }



        //ITEMS INSIDE THE ROOM
        public ActionResult Insideroom(string Room_name, string Campus_name)
        {
            Modelall mymodel = new Modelall();
            mymodel.Item_list = roomname(Room_name, Campus_name);
            mymodel.Room_list = Allrooms(Campus_name);
            mymodel.Employees = employee_acc(Campus_name);
            mymodel.Allbrand = Brands(Campus_name);
            mymodel.Allcampus = campuses();
            ViewBag.Room_name = Room_name;
            ViewBag.campus_name = Campus_name;
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("", "login");
            }
            else
            {
                return View("~/Views/Inventory/insideroom.cshtml", mymodel);
            }


        }

        public IEnumerable<Items> roomname(string Room_name, string Campus_name)
        {
            IEnumerable<Items> ec = null;
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Inside/");

                    // Construct the URI with query parameters
                    string uri = $"Room?Room_name={Room_name}&Campus_name={Campus_name}";

                    var consumedata = hc.GetAsync(uri);
                    consumedata.Wait();

                    var dataread = consumedata.Result;
                    if (dataread.IsSuccessStatusCode)
                    {
                        var results = dataread.Content.ReadAsAsync<IList<Items>>();
                        results.Wait();
                        ec = results.Result;
                    }
                    else if (dataread.StatusCode == HttpStatusCode.NotFound)
                    {
                        ec = Enumerable.Empty<Items>();
                        ViewBag.Message = "No Locality Found.";
                    }
                    else
                    {
                        ec = Enumerable.Empty<Items>();
                        ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (e.g., log them)
                ViewBag.Message = "An error occurred: " + ex.Message;
                ec = Enumerable.Empty<Items>();
            }

            return ec;
        }



        //TRANSFER ITEMS

        [HttpPost]
        public ActionResult transfer(string transfer_serial_no, string transfer_room_name, string transfer_requested_by, string transfer_by, string transfer_campus_name)
        {

            try
            {
                string msg;
                try
                {
                    WebRequest req;
                    WebResponse res;
                    string postData = "Serial_number=" + transfer_serial_no + "&Room_name=" + transfer_room_name + "&Transfer_by=" + transfer_by + "&Requested_by=" + transfer_requested_by + "&Campus_name=" + transfer_campus_name;
                    req = WebRequest.Create(ConfigurationManager.AppSettings["API_Path"] + "API/Inventory/Transfer/Asset?" + postData);
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

        // ROOM COUNTER
        public IEnumerable<Roomscounters> counterallrooms(string Campus_name)
        {

            IEnumerable<Roomscounters> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Dashboard/Rooms/");

            var consumedata = hc.GetAsync("Counter?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Roomscounters>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Roomscounters>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Roomscounters>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }

        // IT ASSET COUNTER

        public IEnumerable<ITassetcounters> counterallITAsset(string Campus_name)
        {

            IEnumerable<ITassetcounters> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Dashboard/Item/");

            var consumedata = hc.GetAsync("Counter?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<ITassetcounters>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<ITassetcounters>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<ITassetcounters>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }


        // Employee COUNTER

        public IEnumerable<Employeecounters> counterallemployee(string Campus_name)
        {

            IEnumerable<Employeecounters> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Dashboard/Employee/");

            var consumedata = hc.GetAsync("Counter?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Employeecounters>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Employeecounters>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Employeecounters>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }

        // Logs COUNTER

        public IEnumerable<logscounters> counterlogs(string Campus_name)
        {

            IEnumerable<logscounters> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Dashboard/Logs/");

            var consumedata = hc.GetAsync("Counter?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<logscounters>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<logscounters>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<logscounters>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }

        //Brand Counter

        public IEnumerable<Brandcounter> counterbrand(string Campus_name)
        {

            IEnumerable<Brandcounter> ec = null;
            HttpClient hc = new HttpClient();
            hc.BaseAddress = new Uri(ConfigurationManager.AppSettings["API_Path"] + "API/Dashboard/Brand/");

            var consumedata = hc.GetAsync("Counter?Campus_name=" + Campus_name);
            consumedata.Wait();


            var dataread = consumedata.Result;
            if (dataread.IsSuccessStatusCode)
            {
                var results = dataread.Content.ReadAsAsync<IList<Brandcounter>>();
                results.Wait();
                ec = results.Result;
            }
            else if (dataread.StatusCode == HttpStatusCode.NotFound)
            {
                ec = Enumerable.Empty<Brandcounter>();
                ViewBag.Message = "No Locality Found.";
            }
            else
            {
                ec = Enumerable.Empty<Brandcounter>();
                ViewBag.Message = "Unable to display locality, please email to it@golden-success.com for help.";
            }
            return ec;
        }

    }
}