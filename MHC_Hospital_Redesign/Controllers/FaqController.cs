using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;
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
        private ApplicationDbContext db = new ApplicationDbContext();

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
        client.BaseAddress = new Uri("http://mhcredesign-env.eba-skfg7g3r.us-east-2.elasticbeanstalk.com/api/");
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

        /// <summary>
        /// Lists all Faq irespective of the category according to FaqSort
        /// </summary>
        /// <param name="Search">Search bar to search Faq by KeyWord</param>
        /// <returns>List OF Faq Ordered By Faq Sort</returns>
        /// <example>
        ///   // GET: Faq/List
        /// </example>
        public ActionResult List(string Search = null)
        {
            //Objective : communicate without our faq data api to retrive a list of faqs
            //curl http://mhcredesign-env.eba-skfg7g3r.us-east-2.elasticbeanstalk.com/listfaqs

            ListFaq ViewModel = new ListFaq();
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
                ViewModel.IsAdmin = true;
            else ViewModel.IsAdmin = false;

            string url = "faqdata/listfaqs";
            if (Search != null)
            {
                url += "?Search=" + Search;
            }
            HttpResponseMessage response = client.GetAsync(url).Result;


            if (response.IsSuccessStatusCode)
            {

                //comment
                IEnumerable<FaqDto> SelectedFaq = response.Content.ReadAsAsync<IEnumerable<FaqDto>>().Result;
                //Return the list fo faq sorted in order by the faq sort column in asscending order
                //Search function to search through the list of Faqs

                ViewModel.Faqs = SelectedFaq;
              
                
                return View(ViewModel); ;
            
            }

            else
            {
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Update the Faq once they have been sorted .This fucntion is still in progress as its not working.
        /// Further investigation and refinement is required
        /// </summary>
        /// <param name="itemIds"></param>
        /// <returns>Uodated Faq Sort </returns>
        public ActionResult UpdateFaqPosition(string itemIds)
        {
            int count = 1;
            List<int> itemIdList = new List<int>();
            itemIdList = itemIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            foreach (var itemId in itemIdList)
            {
                try
                {
                    Faq item = db.Faqs.Where(x => x.FaqID == itemId).FirstOrDefault();
                    item.FaqSort = count;
                    db.Faqs.AddOrUpdate(item);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    continue;
                }
                count++;
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Displays Detailed Faq using the Id from the List Faq.
        /// </summary>
        /// <param name="id">Faq Id as an Id</param>
        /// <returns>Detailed View of a Particular Faq</returns>
        /// <example>
        /// // GET: Faq/Details/5
        /// </example>

        public ActionResult Details(int id)
        {
            DetailsFaq ViewModel = new DetailsFaq();
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
                ViewModel.IsAdmin = true;
            else ViewModel.IsAdmin = false;
            //Objective : communicate without our aircraft data api to retrive  one aircraft
            //curl http://mhcredesign-env.eba-skfg7g3r.us-east-2.elasticbeanstalk.com/api/faqdata/findfaq/{id}


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


        /// <summary>
        /// Associalte an Faq with a Category in a Many to Many Relationship.
        /// </summary>
        /// <param name="id">Faq Id as an Id</param>
        /// <param name="FaqCategoryID">category Id as the key</param>
        /// <returns>Assoicated a Faq Category with an Faq</returns>
        /// <example>
        //POST: Faq/Associate/{faqid}
        /// </example>
       
        [HttpPost]
       [Authorize(Roles ="Admin")]
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
       
        /// <summary>
        /// UnAssociates an Faq with a Category in a Many to Many Relationship.
        /// </summary>
        /// <param name="id">Faq Id as an Id</param>
        /// <param name="FaqCategoryID">category Id as the key</param>
        /// <returns>UnAssoicated a Faq Category with an Faq</returns>
        /// <example>
        //POST: Faq/UnAssociate/{id}?FaqCategoryID={FaqCategoryID}
        /// </example>
        [HttpGet]
        [Authorize(Roles = "Admin")]
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


        /// <summary>
        /// Adds a New Faq To the List
        /// </summary>
        /// <returns>Creates a New Faq</returns>
        /// <example>
        // POST: Faq/Create
        /// </example>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Faq faq)

        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("the json payload is : ");
            //objective: AdditionalMetadataAttribute a new faq into our system using API
            //curl -H "Content-Type:application/json" -d @aircraft.json http://mhcredesign-env.eba-skfg7g3r.us-east-2.elasticbeanstalk.com//api/faqdata/addfaq
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

        /// <summary>
        ///Finds the information for a particular Faq to be updated using Id
        /// </summary>
        ///  /// <param name="id">Faq Id a a key</param>
        /// <returns>Information about a particular Faq</returns>
        /// <example>
        // GET: Faq/Edit/5
        /// </example>
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {

            string url = "faqdata/findfaq/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            FaqDto selectedFaq = response.Content.ReadAsAsync<FaqDto>().Result;
            return View(selectedFaq);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="faq"></param>
        /// <returns></returns>
        /// <example>
        /// // POST: Faq/Update/5
        /// </example>
        [HttpPost]
        [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// Gnenerates a Confirmation Prompt grabbing the information of a particular Faq by its Id
        /// </summary>
        /// <param name="id">Faq Id as the id</param>
        /// <returns>
        /// information about a particulater Faq</returns>
        /// <example>
        ///  // GET: Faq/DeleteConfirm/5
        /// </example>
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "faqdata/findfaq/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            FaqDto selectedfaq = response.Content.ReadAsAsync<FaqDto>().Result;
            return View(selectedfaq);
        }

        /// <summary>
        /// Deletes a  Particular Faq
        /// </summary>
        /// <param name="id">Faq id as the id</param>
        /// <returns>
        /// Deletes ana faq from the list , no returns
        /// </returns>
        /// <example>
        /// // POST: Faq/Delete/5
        /// </example>
        [HttpPost]
        [Authorize(Roles = "Admin")]
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

