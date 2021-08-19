using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using MHC_Hospital_Redesign.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using MHC_Hospital_Redesign.Models.ViewModels;
using static MHC_Hospital_Redesign.Models.Appointment;

namespace MHC_Hospital_Redesign.Controllers
{
    public class AppointmentController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;
        private int DataPerPage = 10;//number of record per page

        /// <summary>
        /// HttpClient 'client' variable for sending POST and GET requests to the data access layer.
        /// </summary>
        static AppointmentController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //set cookies in header
                UseCookies = false
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
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);
            return;
        }
        [Authorize(Roles = "Patient,Doctor,Admin")]
        // GET: Appointment/List?{pageNum}
        public ActionResult List(int pageNum = 1, string searchStatus = "all")
        {
            string url = "", paginatedUrl = "";            
            if (User.IsInRole("Admin"))
            {
                paginatedUrl = "AppointmentData/GetAppointmentsPage";
                url = "AppointmentData/GetAppointments";
            }
            else if (User.IsInRole("Patient") || User.IsInRole("Doctor"))
            {
                paginatedUrl = "AppointmentData/FindUserAppointmentsPage";
                url = "AppointmentData/FindUserAppointments";
                url = url + "/" + User.Identity.GetUserId();
                paginatedUrl += "/" + User.Identity.GetUserId();
            }
            if (url != "")
            {
                url += "?searchStatus=" + searchStatus;
                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    IEnumerable<Appointment> appointments = response.Content.ReadAsAsync<IEnumerable<Appointment>>().Result;             
                    int nberItems = appointments.Count();// count the maximum number of pages
                    int maxPageNber = (int)Math.Ceiling((decimal)nberItems / DataPerPage);                    
                    if (maxPageNber < 1) maxPageNber = 1;                    
                    if (pageNum < 1) pageNum = 1;                   
                    if (pageNum > maxPageNber) pageNum = maxPageNber;                    
                    int startIndex = DataPerPage * (pageNum - 1);
                    ViewData["PageNum"] = pageNum;
                    ViewData["MaxPageNum"] = maxPageNber;
                    ViewData["searchStatus"] = searchStatus;
                    //list of appoinments according to pagination
                    paginatedUrl += "/" + startIndex + "/" + DataPerPage + "?searchStatus=" + searchStatus; ;
                    response = client.GetAsync(paginatedUrl).Result;
                    appointments = response.Content.ReadAsAsync<IEnumerable<Appointment>>().Result;
                    foreach (Appointment A1 in appointments)
                    {
                        A1.PatientUser = new ApplicationDbContext().Users.Find(A1.PatientId);
                        A1.DoctorUser = new ApplicationDbContext().Users.Find(A1.DoctorId);
                        
                    }
                    
                    return View(appointments);
                }
                else
                {
                    return RedirectToAction("Error");
                }

            }
            else
            {
                return RedirectToAction("Error");
            }


        }

        [Authorize(Roles = "Patient,Doctor,Admin")]
        // GET: Appointment/Details/5
        public ActionResult Details(int id)
        {
            string url = "AppointmentData/GetAppointment/" + id;            
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Appointment appt = response.Content.ReadAsAsync<Appointment>().Result;
                appt.PatientUser = new ApplicationDbContext().Users.Find(appt.PatientId);
                appt.DoctorUser = new ApplicationDbContext().Users.Find(appt.DoctorId);
                return View(appt);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }
       
        [Authorize(Roles = "Patient,Doctor")]
        // GET: Appointment/Create
        public ActionResult Create()
        {
            IdentityRole role = null;            
            if (User.IsInRole("Patient"))
            {
                role = new ApplicationDbContext().Roles.SingleOrDefault(m => m.Name == "Doctor");
            }
            else
            {
                role = new ApplicationDbContext().Roles.SingleOrDefault(m => m.Name == "Patient");
            }
            CreateAppointment viewModel = new CreateAppointment();
            viewModel.UsersInRole = new ApplicationDbContext().Users.Where(m => m.Roles.Any(r => r.RoleId == role.Id)).ToList();
            ViewData["user_id"] = User.Identity.GetUserId();
            return View(viewModel);
        }

        // POST: Appointment/Create
        [ValidateAntiForgeryToken()]
        [HttpPost]
        public ActionResult Create(CreateAppointment viewAppointment)
        {
            if (ModelState.IsValid)
            {
                Appointment newAppointment = new Appointment();
                newAppointment.PatientId = viewAppointment.PatientId;
                newAppointment.DoctorId = viewAppointment.DoctorId;
                newAppointment.AId = 0;                
                string url = "AppointmentData/AddAppointment";
                newAppointment.Subject = viewAppointment.Subject;
                newAppointment.Message = viewAppointment.Message;
                newAppointment.Status = AppointmentStatus.Pending;
                newAppointment.DateTime = viewAppointment.DateTime;                
                GetApplicationCookie();
                HttpContent content = new StringContent(jss.Serialize(newAppointment));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = client.PostAsync(url, content).Result;                
                Debug.WriteLine(content);              
                Debug.WriteLine(response);                
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("List", new { pageNum = 1 });
                }
                else
                {
                    return RedirectToAction("Error");

                }
            }
            else
            {
                IdentityRole role = null;               
                if (User.IsInRole("Patient"))
                {
                    role = new ApplicationDbContext().Roles.SingleOrDefault(m => m.Name == "Doctor");
                }
                else
                {
                    role = new ApplicationDbContext().Roles.SingleOrDefault(m => m.Name == "Patient");
                }
                CreateAppointment viewModel = new CreateAppointment();
                viewModel.UsersInRole = new ApplicationDbContext().Users.Where(m => m.Roles.Any(r => r.RoleId == role.Id)).ToList();
                ViewData["user_id"] = User.Identity.GetUserId();

                return View(viewAppointment);
            }
        }

        [Authorize(Roles = "Patient,Doctor")]
        // GET: Appointment/Edit/2
        public ActionResult Edit(int id)
        {
           
            string url = "AppointmentData/GetAppointment/" + id;            
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                UpdateAppointment viewModel = new UpdateAppointment();
                Appointment appointment = response.Content.ReadAsAsync<Appointment>().Result;
                viewModel.PatientUser = new ApplicationDbContext().Users.Find(appointment.PatientId);
                viewModel.DoctorUser = new ApplicationDbContext().Users.Find(appointment.DoctorId);
                viewModel.Subject = appointment.Subject;
                viewModel.Message = appointment.Message;
                viewModel.DateTime = appointment.DateTime;
                viewModel.Status = appointment.Status;
                viewModel.PatientId = appointment.PatientId;
                viewModel.DoctorId = appointment.DoctorId;
                viewModel.AId = appointment.AId;
                Debug.WriteLine(viewModel.PatientUser);
                return View(viewModel);

            }
            else
            {
                return RedirectToAction("Error");

            }
        }

        // POST: Appointment/Edit/2
        [Authorize(Roles = "Patient,Doctor")]
        [ValidateAntiForgeryToken()]
        [HttpPost]
        public ActionResult Edit(int id, UpdateAppointment viewModel)
        {            
            if (ModelState.IsValid)
            {
                Appointment appointment = new Appointment();
                appointment.AId = viewModel.AId;
                appointment.PatientId = viewModel.PatientId;
                appointment.DoctorId = viewModel.DoctorId;
                appointment.Subject = viewModel.Subject;
                appointment.DateTime = viewModel.DateTime;
                appointment.Message = viewModel.Message;
                appointment.Status = viewModel.Status;
                Debug.WriteLine(appointment.Status);               
                GetApplicationCookie();
                string url = "AppointmentData/UpdateAppointment/" + id;
                HttpContent content = new StringContent(jss.Serialize(appointment));
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
            else
            {
                return View(viewModel);
            }
        }

        [Authorize(Roles = "Patient,Doctor")]
        // GET: Appointment/Delete/1
        public ActionResult DeleteConfirm(int id)
        {
            string url = "AppointmentData/GetAppointment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Appointment appointment = response.Content.ReadAsAsync<Appointment>().Result;
                appointment.PatientUser = new ApplicationDbContext().Users.Find(appointment.PatientId);
                appointment.DoctorUser = new ApplicationDbContext().Users.Find(appointment.DoctorId);
                if (User.IsInRole("Patient"))
                    ViewData["recipientUsername"] = "Dr " + appointment.DoctorUser.FirstName + " " + appointment.DoctorUser.LastName;
                else
                    ViewData["recipientUsername"] = appointment.PatientUser.FirstName + " " + appointment.PatientUser.LastName;

                return View(appointment);

            }
            else
            {
                return RedirectToAction("Error");

            }
        }
        [Authorize(Roles = "Patient,Doctor")]
        [ValidateAntiForgeryToken()]
        // POST: Appointment/Delete/2
        [HttpPost]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();          
            string url = "AppointmentData/DeleteAppointment/" + id;
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List", new { pageNum = 1 });

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
