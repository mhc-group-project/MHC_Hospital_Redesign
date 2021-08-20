using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using MHC_Hospital_Redesign.Models.ViewModels;
using MHC_Hospital_Redesign.Models;

namespace MHC_Hospital_Redesign.Controllers
{
    public class DepartmentController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static DepartmentController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://mhcredesign-env.eba-skfg7g3r.us-east-2.elasticbeanstalk.com/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        }
        private void GetApplicationCookie()
        {
            string token = "";
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }
        // GET: Department
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Patient,Doctor,Admin")]
        public ActionResult Details(int id)
        {
            GetApplicationCookie();
            DetailsDepartment DetailsDepartment = new DetailsDepartment();

            string url = "departmentdata/finddepartment/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;            
            if (response.IsSuccessStatusCode)
            {               
                DepartmentDto department = response.Content.ReadAsAsync<DepartmentDto>().Result;
                DetailsDepartment.Department = department;

                url = "DepartmentData/FindListingsForDepartment/" + id;
                response = client.GetAsync(url).Result;
                IEnumerable<ListingDto> Listings = response.Content.ReadAsAsync<IEnumerable<ListingDto>>().Result;
                DetailsDepartment.Jobs = Listings;
                return View(DetailsDepartment);

            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        // GET: Department/New
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }
        
        // GET: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Department Dept)
        {
            GetApplicationCookie();
            Debug.WriteLine(Dept.DepartmentName);
            string url = "DepartmentData/AddDepartment";
            Debug.WriteLine(jss.Serialize(Dept));
            HttpContent content = new StringContent(jss.Serialize(Dept));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int departmentId = response.Content.ReadAsAsync<int>().Result;

                return RedirectToAction("Details", new { id = departmentId });

            }
            else
            {
                return RedirectToAction("Error");
            }


        }
        [Authorize(Roles = "Patient,Doctor,Admin")]
        public ActionResult List()
        {
           
            string url = "departmentdata/listdepartments";
            // Send  HTTP request
            // GET : /api/departmentdata/getdepartments           
            HttpResponseMessage response = client.GetAsync(url).Result;            
            if (response.IsSuccessStatusCode)
            {               
                IEnumerable<DepartmentDto> SelectedDepartment = response.Content.ReadAsAsync<IEnumerable<DepartmentDto>>().Result;
                return View(SelectedDepartment);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        // GET: Department/Edit/2
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            string url = "departmentdata/finddepartment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;           
            if (response.IsSuccessStatusCode)
            {               
                DepartmentDto SelectedDepartment = response.Content.ReadAsAsync<DepartmentDto>().Result;
                return View(SelectedDepartment);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Department/Edit/2
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Department DepartmentData)
        {
            string url = "departmentdata/updatedepartment/" + id;
            Debug.WriteLine(jss.Serialize(DepartmentData));
            HttpContent content = new StringContent(jss.Serialize(DepartmentData));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        // GET: Department/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();
            string url = "departmentdata/finddepartment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            Debug.WriteLine(response);
            if (response.IsSuccessStatusCode)
            {               
                DepartmentDto SelectedDepartment = response.Content.ReadAsAsync<DepartmentDto>().Result;
                return View(SelectedDepartment);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        // POST: Department/Delete/2
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "departmentdata/deletedepartment/" + id;           
            HttpContent content = new StringContent("");
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

        public ActionResult Error()
        {
            return View();
        }
    }
}
