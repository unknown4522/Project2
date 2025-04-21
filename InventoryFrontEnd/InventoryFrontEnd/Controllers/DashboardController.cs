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
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index(string Campus_name)
        {
            Modelall mymodel = new Modelall();
            mymodel.Allroomcounters = counterallrooms(Campus_name);
            mymodel.AllITassetcounters = counterallITAsset(Campus_name);
            mymodel.Allemployeecounters = counterallemployee(Campus_name);
            mymodel.Alllogscounters = counterlogs(Campus_name);
            mymodel.Allbrandcounters = counterbrand(Campus_name);
            mymodel.AllApparelavailablecounters = counterapparelavil(Campus_name);
            mymodel.AllGradecounters = countergrade(Campus_name);
            mymodel.AllSizecounters = counterSize(Campus_name);
            mymodel.Allclaimcounters = counterclaim(Campus_name);
            mymodel.Allappareltypecounters = counterappareltype(Campus_name);
            mymodel.Allapparelrecordcounters = counterapparelrecords(Campus_name);
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
                return View("~/Views/Dashboard/Index.cshtml", mymodel);
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

        //APPAREL AVAILABLE COUNTER

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


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("", "login");
        }


    }
}