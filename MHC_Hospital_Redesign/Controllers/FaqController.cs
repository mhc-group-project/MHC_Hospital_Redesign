using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using MHC_Hospital_Redesign.Models;
using System.Web.Script.Serialization;
using MHC_Hospital_Redesign.Models.ViewModels;
using System.Diagnostics;

namespace MHC_Hospital_Redesign.Controllers
{
    public class FaqController : Controller

    { 
        private static readonly HttpClient client;
    private JavaScriptSerializer jss = new JavaScriptSerializer();

    static FaqController()
    {
        HttpClientHandler handler = new HttpClientHandler()
        {
            AllowAutoRedirect = false,
            //cookies are manually set in RequestHeader
            UseCookies = false
        };

        client = new HttpClient(handler);
        client.BaseAddress = new Uri("https://localhost:44338/api/");
    }
        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }


        // GET: Faq/List
        public ActionResult List(string search)
        {
            //Objective : communicate without our faq data api to retrive a list of faqs
            //curl https://localhost:44384/api/faqdata/listfaqs


            string url = "faqdata/listfaqs";
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {


                IEnumerable<FaqDto> SelectedFaq = response.Content.ReadAsAsync<IEnumerable<FaqDto>>().Result;
                return View(search == null ? SelectedFaq :
                    SelectedFaq.Where(x => x.FaqQuestions.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList());
            }

            else
            {
                return RedirectToAction("Error");
            }
        }


        // GET: Faq/Details/5
        public ActionResult Details(int id)
        {
            DetailsFaq ViewModel = new DetailsFaq();

            //Objective : communicate without our aircraft data api to retrive  one aircraft
            //curl https://localhost:44384/api/faqdata/findfaq/{id}


            string url = "faqdata/findfaq/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            FaqDto SelectedFaq = response.Content.ReadAsAsync<FaqDto>().Result;
            Debug.WriteLine("Faq received : ");
            Debug.WriteLine(SelectedFaq.FaqQuestions);

            ViewModel.SelectedFaq = SelectedFaq;

            ///Show associated FaqCategies with this Faq
            url = "faqcategorydata/listfaqcategoriesforfaq/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<FaqCategoryDto> LinkedFaqCategories = response.Content.ReadAsAsync<IEnumerable<FaqCategoryDto>>().Result;

            ViewModel.LinkedFaqCategories = LinkedFaqCategories;

            url = "faqcategorydata/listcategoriesnotlistingfaq/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<FaqCategoryDto> AvailableFaqCategories = response.Content.ReadAsAsync<IEnumerable<FaqCategoryDto>>().Result;

            ViewModel.AvailableFaqCategories = AvailableFaqCategories;

            return View(ViewModel);
        }

        //POST: Faq/Associate/{faqid}
        [HttpPost]
       // [Authorize]
        public ActionResult Associate(int id, int FaqCategoryID)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("Attempting to associate faq:" + id + " with faqcategory " + FaqCategoryID);

            //call our api to associate faq with category
            string url = "faqdata/associatefaqwithfaqcategory/" + id + "/" + FaqCategoryID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        //POST: Faq/UnAssociate/{id}?FaqCategoryID={FaqCategoryID}
        [HttpGet]
       // [Authorize]
        public ActionResult UnAssociate(int id, int FaqCategoryID)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("Attempting to unassociate faq :" + id + " with faqcategory " + FaqCategoryID);

            //call our api to associate aircraft with faq category
            string url = "faqdata/unassociatefaqwithfaqcategory/" + id + "/" + FaqCategoryID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Faq/New
        public ActionResult New()
        {
      
        

            return View();
        }

        // POST: Faq/Create
        [HttpPost]
       // [Authorize]
        public ActionResult Create(Faq faq)

        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("the json payload is : ");
            //objective: AdditionalMetadataAttribute a new faq into our system using API
            //curl -H "Content-Type:application/json" -d @aircraft.json https://localhost:44384/api/faqdata/addfaq
            string url = "faqdata/addfaq";



            string jsonpayload = jss.Serialize(faq);
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

        // GET: Faq/Edit/5
       // [Authorize]
        public ActionResult Edit(int id)
        {

            string url = "faqdata/findfaq/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            FaqDto selectedFaq = response.Content.ReadAsAsync<FaqDto>().Result;
            return View(selectedFaq);
        }

        // POST: Faq/Update/5
        [HttpPost]
      //  [Authorize]
        public ActionResult Update(int id, Faq faq)
        {
            GetApplicationCookie();//get token credentials
            string url = "faqdata/updatefaq/" + id;

            string jsonpayload = jss.Serialize(faq);
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

        // GET: Faq/Delete/5
      //  [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "faqdata/findfaq/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            FaqDto selectedfaq = response.Content.ReadAsAsync<FaqDto>().Result;
            return View(selectedfaq);
        }

        // POST: Aircraft/Delete/5
        [HttpPost]
       // [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "faqdata/deletefaq/" + id;
            HttpContent content = new StringContent("");
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
    }
}

