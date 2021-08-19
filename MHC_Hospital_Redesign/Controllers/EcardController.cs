using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using MHC_Hospital_Redesign.Models;
using System.Web.Script.Serialization;
using MHC_Hospital_Redesign.Models.ViewModels;

namespace MHC_Hospital_Redesign.Controllers
{
    public class EcardController : Controller
    {

        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static EcardController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                // cookies are manually set in RequestHeader
                UseCookies = false
            };
            client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://mhcredesign-env.eba-skfg7g3r.us-east-2.elasticbeanstalk.com/api/");
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller. Allows for authenticated users to make administrative changes 
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";

            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;


            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

        // GET: Ecard/List
        public ActionResult List()
        {
            GetApplicationCookie();
            //goal: to communicate with template data api to retrieve a list of ecards
            //curl https://localhost:44338/api/templatedata/listecards


            string url = "ecarddata/listecards";
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            IEnumerable<EcardDto> ecards = response.Content.ReadAsAsync<IEnumerable<EcardDto>>().Result;
            Debug.WriteLine("Recieived #" + ecards.Count() + " ecards");

            return View(ecards);
        }

        // GET: Ecard/Details/5
        public ActionResult Details(int id)
        {
            // objective: communicate with ecard data api to retrieve one ecard
           
            DetailsEcard ViewModel = new DetailsEcard();
           

            string url = "ecarddata/findecard/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            EcardDto SelectedEcard = response.Content.ReadAsAsync<EcardDto>().Result;
            Debug.WriteLine("ecard recieved ID:" + SelectedEcard.EcardId);
            ViewModel.SelectedEcard = SelectedEcard;

            url = "templatedata/listtemplates/";
            response = client.GetAsync(url).Result;
            IEnumerable<TemplateDto> TemplateOptions = response.Content.ReadAsAsync<IEnumerable<TemplateDto>>().Result;

            ViewModel.TemplateOptions = TemplateOptions;

   

            return View(SelectedEcard);
        }

        public ActionResult Error()
        {
            return View();
        }


        // GET: Ecard/New
     
        public ActionResult New()
        {
            GetApplicationCookie();
            //information about all templates in the system
            //GET: api/templatedata/listtemplates

            string url = "templatedata/listtemplates";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<TemplateDto> TemplateOptions = response.Content.ReadAsAsync<IEnumerable<TemplateDto>>().Result;

            return View(TemplateOptions); ;
        }

        // POST: Ecard/Create
        [HttpPost]
        public ActionResult Create(Ecard ecard)
        {
            Debug.WriteLine(ecard.EcardId);
            //objective: add new template into our sstem using the API
            //curl -H "Content-type:application/json" -d@ecard.json https://localhost:44338/api/ecarddata/addecard
            string url = "ecarddata/addecard";

            string jsonpayload = jss.Serialize(ecard);

            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        // GET: Ecard/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            UpdateEcard ViewModel = new UpdateEcard();

            //existing ecard info
            string url = "ecarddata/findecard/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            EcardDto SelectedEcard = response.Content.ReadAsAsync<EcardDto>().Result;
            ViewModel.SelectedEcard = SelectedEcard;
            //all templates to choose from when updating this ecard
            //existing ecard info
             url = "templatedata/listtemplates/";
            response = client.GetAsync(url).Result;
            IEnumerable<TemplateDto> TemplateOptions = response.Content.ReadAsAsync<IEnumerable<TemplateDto>>().Result;

            ViewModel.TemplateOptions = TemplateOptions;

            return View(ViewModel);
        }

        // POST: Ecard/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Ecard ecard)
        {
            GetApplicationCookie();
            //objective: edit an existing ecard in our system using the api
   

            string url = "ecarddata/updateecard/" + id;

            string jsonpayload = jss.Serialize(ecard);

            HttpContent content = new StringContent(jsonpayload);

            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
      
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Ecard/DeleteConfirm/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "ecarddata/findecard/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            EcardDto selectedspecies = response.Content.ReadAsAsync<EcardDto>().Result;
            return View(selectedspecies);
        }

        // POST: Ecard/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            //objective: delete a ecard from the system

            string url = "ecarddata/deleteecard/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);

            if (response.IsSuccessStatusCode)
            {
                // TODO: Add delete logic here

                return RedirectToAction("List");
            }
            else
            {
                return View("Error");
            }
        }
    }
}
