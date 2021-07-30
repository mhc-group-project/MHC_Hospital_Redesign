﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using MHC_Hospital_Redesign.Models;
using System.Web.Script.Serialization;

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
            client.BaseAddress = new Uri("https://localhost:44338/api/");
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
        public ActionResult List()
        {
            // objective: communicate with listing data api to retrieve a list of volunteer listings

            // establish URL communication
            string url = "listingdata/listlistings";
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

            return View(SelectedListing);
        }

        // GET: Listing/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Listing/Create
        [HttpPost]
        public ActionResult Create(Listing listing)
        {

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
        public ActionResult Edit(int id)
        {
            // objective: users are able to find the listing to edit

            // establish URL communication
            string url = "listingdata/findlisting/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            // parse content response
            ListingDto SelectedListing = response.Content.ReadAsAsync<ListingDto>().Result;

            return View(SelectedListing);
        }

        // POST: Listing/Update/5
        [HttpPost]
        public ActionResult Update(int id, Listing listing)
        {
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
        public ActionResult DeleteConfirm(int id)
        {
            string url = "listingdata/findlisting/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ListingDto SelectedListing = response.Content.ReadAsAsync<ListingDto>().Result;

            return View(SelectedListing);
        }

        // POST: Listing/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
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
