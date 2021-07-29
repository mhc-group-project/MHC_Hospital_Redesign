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
    public class FeedbackCategoryController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static FeedbackCategoryController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };


            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44337/api/");
        }

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
        // GET: FeedbackCategory/List
        public ActionResult List()
        {
            //objective: communicate with our feedbackCategory data api to retrive a list of feedbackCategories 
            //curl https://localhost:44337/api/feedbackCategorydata/listfeedbackCategories

            string url = "feedbackCategorydata/listfeedbackCategories";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<FeedbackCategoryDto> feedbackCategories = response.Content.ReadAsAsync<IEnumerable<FeedbackCategoryDto>>().Result;
            //Debug.WriteLine("Number of feedbackCategories received: ");
            //Debug.WriteLine(feedbackCategories.Count());

            return View(feedbackCategories);
        }

        // GET: FeedbackCategory/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with our feedbackCategory data api to retrive one feedbackCategory.
            //curl https://localhost:44337/api/feedbackCategorydata/findfeedbackCategory/{id}

            DetailsFeedbackCategory ViewModel = new DetailsFeedbackCategory();

            string url = "feedbackCategorydata/findfeedbackCategory/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is");
            //Debug.WriteLine(response.StatusCode);

            FeedbackCategoryDto SelectedFeedbackCategory = response.Content.ReadAsAsync<FeedbackCategoryDto>().Result;
            //Debug.WriteLine("feedbackCategory received: ");
            //Debug.WriteLine(selectedfeedbackCategory.FeedbackCategoryName);

            ViewModel.SelectedFeedbackCategory = SelectedFeedbackCategory;

            url = "feedbackdata/listfeedbacksforfeedbackcategory/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<FeedbackDto> RelatedFeedbacks = response.Content.ReadAsAsync<IEnumerable<FeedbackDto>>().Result;

            ViewModel.RelatedFeedbacks = RelatedFeedbacks;


            return View(ViewModel);
        }
        public ActionResult Error()
        {
            return View();
        }

        // GET: FeedbackCategory/New
        [Authorize]
        public ActionResult New()
        {
            //information about all feedbackCategories in the system]
            //Get api/feedbackCategorydata/listfeedbackCategories
            return View();
        }

        // POST: FeedbackCategory/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(FeedbackCategory feedbackCategory)
        {
            GetApplicationCookie();
            Debug.WriteLine("the json payload is : ");
            //objective: add a new feedbackCategory into our system using api
            //curl -H "Content-type:application/json -d @add.json https://localhost:44337/api/feedbackCategorydata/addfeedbackCategory
            string url = "feedbackCategorydata/addfeedbackCategory";

            string jsonpayload = jss.Serialize(feedbackCategory);

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

        // GET: FeedbackCategory/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {


            //the existing feedbackCategory information
            string url = "feedbackCategorydata/findfeedbackCategory/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            FeedbackCategoryDto SelectedFeedbackCategory = response.Content.ReadAsAsync<FeedbackCategoryDto>().Result;
            return View(SelectedFeedbackCategory);


        }

        // POST: FeedbackCategory/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, FeedbackCategory feedbackCategory)
        {
            GetApplicationCookie();
            string url = "feedbackCategorydata/updatefeedbackCategory/" + id;
            string jsonpayload = jss.Serialize(feedbackCategory);
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

        // GET: FeedbackCategory/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();
            string url = "feedbackCategorydata/findfeedbackCategory/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            FeedbackCategoryDto selectedfeedbackCategory = response.Content.ReadAsAsync<FeedbackCategoryDto>().Result;
            return View(selectedfeedbackCategory);
        }

        // POST: FeedbackCategory/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            string url = "feedbackCategorydata/deletefeedbackCategory/" + id;
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
