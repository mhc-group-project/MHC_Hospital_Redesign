using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using MHC_Hospital_Redesign.Models;
using MHC_Hospital_Redesign.Models.ViewModels;
using System.Web.Script.Serialization;

namespace MHC_Hospital_Redesign.Controllers
{
    public class FaqCategoryController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static FaqCategoryController()
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
        /// Authentication cookie is grabbed which was sent to the controller
        /// Provides 
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

        // GET: FaqCategory/List
        [HttpGet]
        public ActionResult List(string search)
        {
            //objective: communicate with our FaqCategory data api to retrieve a list fo FaqCategories
            //curl https://localhost:44384/api/faqcategorydata/ListFaqCategories

            string url = "faqcategorydata/listfaqcategories";
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {


                IEnumerable<FaqCategoryDto> SelectedFaqCategory = response.Content.ReadAsAsync<IEnumerable<FaqCategoryDto>>().Result;
                return View(search == null ? SelectedFaqCategory :
                    SelectedFaqCategory.Where(x => x.CategoryName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList());
                Debug.WriteLine("The response code is ");
                Debug.WriteLine(response.StatusCode);


                Debug.WriteLine("Number of FaqCategories received : ");
                Debug.WriteLine(SelectedFaqCategory.Count());

                 return View(SelectedFaqCategory);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }






        // GET: FaqCategories/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            DetailsFaqCategory ViewModel = new DetailsFaqCategory();

            //objective: communicate with our FAQCategory data api to retrieve one Faq Category
            //curl https://localhost:44324/api/FaqCategorydata/findfaqcategory/{id}

            string url = "faqcategorydata/findfaqcategory/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            FaqCategoryDto SelectedFaqCategory = response.Content.ReadAsAsync<FaqCategoryDto>().Result;
            Debug.WriteLine("Faq Category received : ");
            Debug.WriteLine(SelectedFaqCategory.CategoryName);

            ViewModel.SelectedFaqCategory = SelectedFaqCategory;

            //show all Faqs under listed under FaqCategory
            url = "faqdata/listfaqsforfaqcategory/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<FaqDto> LinkedFaqs = response.Content.ReadAsAsync<IEnumerable<FaqDto>>().Result;

            ViewModel.LinkedFaqs = LinkedFaqs;


            return View(ViewModel);
        }
        public ActionResult Error()
        {
            return View();
        }


        // GET: FaqCAtegory/New
        [HttpGet]
      //  [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: FaqCategory/Create
        [HttpPost]
      //  [Authorize]
        public ActionResult Create(FaqCategory faqCategory)
        {

            GetApplicationCookie();//get token credentials
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(faqCategory.CategoryName);
            //objective: add a new FaqCategory into our system using the API
            //curl -H "Content-Type:application/json" -d @FaqCategory.json https://localhost:44324/api/faqcategorydata/addfaqcategory 
            string url = "faqcategorydata/addfaqcategory";


            string jsonpayload = jss.Serialize(faqCategory);
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

        // GET: FaqCategory/Edit/5
        [HttpGet]
       // [Authorize]
        public ActionResult Edit(int id)
        {

            string url = "faqcategorydata/findfaqcategory/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            FaqCategoryDto selectedFaqCategory = response.Content.ReadAsAsync<FaqCategoryDto>().Result;
            return View(selectedFaqCategory);
        }

        // POST: FaqCategory/Update/5
        [HttpPost]
     //   [Authorize]
        public ActionResult Update(int id, FaqCategory faqCategory)
        {
            GetApplicationCookie();//get token credentials
            string url = "faqcategorydata/updatefaqcategory/" + id;
            string jsonpayload = jss.Serialize(faqCategory);
            HttpContent content = new StringContent(jsonpayload);
            Debug.WriteLine("Json payload is :");
            Debug.WriteLine(jsonpayload);
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

        // GET: FaqCategory/DeleteConfirm/5
      //  [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "faqcategorydata/findfaqcategory/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
             FaqCategoryDto selectedFaqCategory = response.Content.ReadAsAsync<FaqCategoryDto>().Result;
            return View(selectedFaqCategory);
        }

        // POST: FaqCategory/Delete/5
        [HttpPost]
      //  [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "faqcategorydata/deletefaqcategory/" + id;
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