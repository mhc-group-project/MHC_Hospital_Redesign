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
    public class ListingController : Controller
    {

        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static ListingController()
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
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
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

        // GET: Listing/List
        public ActionResult List(string SearchKey = null)
        {
            // objective: communicate with listing data api to retrieve a list of volunteer listings

            // checks if user is logged in and admin for rendering
            ListListing ViewModel = new ListListing();
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) ViewModel.IsAdmin = true; else ViewModel.IsAdmin = false;

            // establish URL communication
            string url = "listingdata/listlistings";

            if (SearchKey != null)
            {
                url += "?SearchKey=" + SearchKey;
            }

            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            //parse the content of response into IEnumerable
            IEnumerable<ListingDto> listings = response.Content.ReadAsAsync<IEnumerable<ListingDto>>().Result;
            //Debug.WriteLine("Number of listings: ");
            //Debug.WriteLine(listings.Count());

            return View(listings);
        }

        // GET: Listing/Details/5
        public ActionResult Details(int id)
        {
            DetailsListing ViewModel = new DetailsListing();

            // objective: communicate with listing data api to retrieve one listing
            // curl "https://localhost:44338/api/listingdata/findlisting/{id}"

            // establish URL communication
            string url = "listingdata/findlisting/"+id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            // parse content response
            ListingDto SelectedListing = response.Content.ReadAsAsync<ListingDto>().Result;
            //Debug.WriteLine("Listing: ");
            //Debug.WriteLine(SelectedListing.ListTitle);

           ViewModel.SelectedListing = SelectedListing;
            
            // show associated volunteers with this listing
            url = "userdata/listusersforlisting/"+id;
            response = client.GetAsync(url).Result;
            IEnumerable<ApplicationUserDto> AssignedUsers = response.Content.ReadAsAsync<IEnumerable<ApplicationUserDto>>().Result;

            ViewModel.AssignedUsers = AssignedUsers;

            //show volunteers that are not associated with this listing
            url = "userdata/listusersnotforlisting/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ApplicationUserDto> AvailableUsers = response.Content.ReadAsAsync<IEnumerable<ApplicationUserDto>>().Result;
            
            ViewModel.AvailableUsers = AvailableUsers;

            return View(ViewModel);
        }

        //POST: Listing/Associate/{id}?UserID={UserID}
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult Associate(int id, string UserID)
        {
            Debug.WriteLine("Attempting to associate ListID : " + id + " with user " + UserID);

            //call api to associate with user
            string url = "listingdata/associatelistingwithuser/" + id + "/" + UserID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        //GET: Listing/UnAssociate/{id}?UserID={UserID}
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult UnAssociate(int id, string UserID)
        {
            Debug.WriteLine("Attempting to unassociate ListID : " + id + " with user " + UserID);

            //call api to unassociate listing with user
            string url = "listingdata/unassociatelistingwithuser/" + id + "/" + UserID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }


        // GET: Listing/New
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            //using listing dto for validation
            UpdateListing ViewModel = new UpdateListing();

            //information about all departments in the system
            //GET api/departmentdata/listdepartments

            string url = "departmentdata/listdepartments/";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<DepartmentDto> DeptOptions = response.Content.ReadAsAsync<IEnumerable<DepartmentDto>>().Result;

            ViewModel.DeptOptions = DeptOptions;

            return View(ViewModel);
        }

        // POST: Listing/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Listing listing)
        {
            // gets the asp.net application cookie to authenticate on the webapi level
            GetApplicationCookie();

            // objective: add a new listing into the system using the API
            // curl -d @listing.json -H "Content-Type:application/json" https://localhost:44338/api/listingdata/addlisting
            string url = "listingdata/addlisting";

            //convert listing object to json
            string jsonpayload = jss.Serialize(listing);

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

        // GET: Listing/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            UpdateListing ViewModel = new UpdateListing();

            // objective: users are able to find the listing to edit

            // establish URL communication
            string url = "listingdata/findlisting/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            // parse content response
            ListingDto SelectedListing = response.Content.ReadAsAsync<ListingDto>().Result;
            ViewModel.SelectedListing = SelectedListing;

            // all departments to choose from when updating
            url = "departmentdata/listdepartments/";
            response = client.GetAsync(url).Result;
            IEnumerable<DepartmentDto> DeptOptions = response.Content.ReadAsAsync<IEnumerable<DepartmentDto>>().Result;

            ViewModel.DeptOptions = DeptOptions;

            return View(ViewModel);
        }

        // POST: Listing/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Listing listing)
        {

            // gets the asp.net application cookie to authenticate on the webapi level
            GetApplicationCookie();

            Debug.WriteLine("The json payload is: ");
            Debug.WriteLine(listing.ListTitle);

            //objective: edit an existing listing in our system using the api
            // curl -d @listing.json -H "Content-Type:application/json" https://localhost:44338/api/listingdata/addlisting

            string url = "listingdata/updatelisting/" + id;

            //convert listing object to json
            string jsonpayload = jss.Serialize(listing);

            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if(response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Listing/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "listingdata/findlisting/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ListingDto SelectedListing = response.Content.ReadAsAsync<ListingDto>().Result;

            return View(SelectedListing);
        }

        // POST: Listing/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            // gets the asp.net application cookie to authenticate on the webapi level
            GetApplicationCookie();

            // objective: delete a listing from the system
            // curl api/listingdata/deletelisting -d ""

            string url = "listingdata/deletelisting/" + id;
            HttpContent content = new StringContent("");

            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if(response.IsSuccessStatusCode)
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
