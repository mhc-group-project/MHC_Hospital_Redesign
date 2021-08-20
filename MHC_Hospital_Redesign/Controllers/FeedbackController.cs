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
    public class FeedbackController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static FeedbackController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,

                UseCookies = false
            };


            client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://mhcredesign-env.eba-skfg7g3r.us-east-2.elasticbeanstalk.com/api/");
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



        // GET: Feedback/List
        public ActionResult List(string SearchKey = null)
        {
            //objective: communicate with our feedback data api to retrive a list of feedbacks 
            //curl https://localhost:44338/api/feedbackdata/listfeedbacks

            string url = "feedbackdata/listfeedbacks";

            if (SearchKey != null)
            {
                url += "?SearchKey=" + SearchKey;
            }
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<FeedbackDto> feedbacks = response.Content.ReadAsAsync<IEnumerable<FeedbackDto>>().Result;
            //Debug.WriteLine("Number of feedbacks received: ");
            //Debug.WriteLine(feedbacks.Count());

            return View(feedbacks);
        }

        // GET: Feedback/Details/5
        public ActionResult Details(int id)
       {
            DetailsFeedback ViewModel = new DetailsFeedback();


            //objective: communicate with our feedback data api to retrive one feedback.
            //curl https://localhost:44338/api/feedbackdata/findfeedback/{id}

            string url = "feedbackdata/findfeedback/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is");
            //Debug.WriteLine(response.StatusCode);

            FeedbackDto SelectedFeedback = response.Content.ReadAsAsync<FeedbackDto>().Result;
            //Debug.WriteLine("feedback received: ");
            //Debug.WriteLine(SelectedFeedback.FeedbackName);

            ViewModel.SelectedFeedback = SelectedFeedback;

            return View(ViewModel);
        }


        public ActionResult Error()
        {
            return View();
        }

        // GET: Feedback/New
        [Authorize]
        public ActionResult New()
        {
            //information about all feedbackCategories in the system.
            //Get api/feedbackCategorydata/listfeedbackCategories

            string url = "feedbackCategorydata/listfeedbackCategories";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<FeedbackCategoryDto> FeedbackCategoryOptions = response.Content.ReadAsAsync<IEnumerable<FeedbackCategoryDto>>().Result;

            return View(FeedbackCategoryOptions);
        }

        // POST: Feedback/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Feedback feedback)
        {
            GetApplicationCookie();
            Debug.WriteLine("the json payload is : ");
            //objective: add a new feedback into our system using api
            //curl -H "Content-type:application/json -d @feedback.json https://localhost:44338/api/feedbackdata/addfeedback
            string url = "feedbackdata/addfeedback";


            string jsonpayload = jss.Serialize(feedback);

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

        // GET: Feedback/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            UpdateFeedback ViewModel = new UpdateFeedback();

            //the existing feedback information
            string url = "feedbackdata/findfeedback/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            FeedbackDto SelectedFeedback = response.Content.ReadAsAsync<FeedbackDto>().Result;
            ViewModel.SelectedFeedback = SelectedFeedback;

            // all feedbackCategories to choose from when updating this feedback
            //the existing feedback information
            url = "feedbackCategorydata/listfeedbackCategories";
            response = client.GetAsync(url).Result;
            IEnumerable<FeedbackCategoryDto> FeedbackCategoryOptions = response.Content.ReadAsAsync<IEnumerable<FeedbackCategoryDto>>().Result;

            ViewModel.FeedbackCategoryOptions = FeedbackCategoryOptions;

            return View(ViewModel);
        }

        // POST: Feedback/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Feedback feedback)
        {
            GetApplicationCookie();
            string url = "feedbackdata/updatefeedback/" + id;
            HttpContent content = new StringContent(jss.Serialize(feedback));
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

        // GET: Feedback/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "feedbackdata/findfeedback/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            FeedbackDto selectedfeedback = response.Content.ReadAsAsync<FeedbackDto>().Result;
            return View(selectedfeedback);
        }

        // POST: Feedback/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "feedbackdata/deletefeedback/" + id;
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
