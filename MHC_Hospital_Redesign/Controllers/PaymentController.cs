using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Diagnostics;
using MHC_Hospital_Redesign.Models;

namespace MHC_Hospital_Redesign.Controllers
{
    public class PaymentController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static PaymentController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44338/api/");
        }

        // GET: Payment/List
        /// <summary>
        ///     Routes to a dynamically generated "Payment List" Page. 
        ///     Gathers information about all the payments in the database.
        /// </summary>
        /// <returns> A dynamic webpage which displays a List of Payments </returns>
        /// <example>
        ///     GET: Payment/List
        /// </example>
        [HttpGet]
        public ActionResult List()
        {
            // Objective: Communicate with Invoice Data Api to RETRIEVE a list of Payments
            // curl https://localhost:44338/api/PaymentData/ListPayments
            string url = "PaymentData/ListPayments";

            HttpResponseMessage response = client.GetAsync(url).Result;
            Debug.WriteLine("The status code is " + response.StatusCode);

            IEnumerable<PaymentDto> Payments = response.Content.ReadAsAsync<IEnumerable<PaymentDto>>().Result;
            // Debug.WriteLine("Number of payments -> " + Payments.Count());

            if (response.IsSuccessStatusCode)
            {
                return View(Payments);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Payment/Details/5
        /// <summary>
        ///     Routes to a dynamically generated "Payment Details" Page.
        ///     Gathers information about a specific payment from the database
        /// </summary>
        /// <param name="id"> Payment ID </param>
        /// <returns> A dynamic webpage which provides the current information of an Payment </returns>
        /// <example>
        ///     GET: Payment/Details/5
        /// </example>
        [HttpGet]
        public ActionResult Details(int id)
        {
            // Objective: Communicate with Invoice Data Api to RETRIEVE an Payment
            // curl https://localhost:44338/api/PaymentData/FindPayment/{id}
            string url = "PaymentData/FindPayment/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;
            // Debug.WriteLine("The status code is " + response.StatusCode);

            PaymentDto SelectedPayment = response.Content.ReadAsAsync<PaymentDto>().Result;
            // Debug.WriteLine("Payment Data -> " + SelectedPayment);

            if (response.IsSuccessStatusCode)
            {
                return View(SelectedPayment);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Invoice/Error
        /// <summary>
        ///     Routes to a dynamically generated "Error" Page.
        /// </summary>
        /// <returns> A dynamic webpage which provides an Error Message. </returns>
        /// <example>
        ///     GET: Invoice/Error
        /// </example>
        [HttpGet]
        public ActionResult Error()
        {
            return View();
        }

        // GET: Payment/New
        /// <summary>
        ///     Routes to a dynamically generated "Payment New" Page. 
        ///     Gathers information about a new Payment from a form 
        ///     that will be added to the database.
        /// </summary>
        /// <returns> A dynamic webpage which asks the user for new information regarding a Payment as part of a form. </returns>
        /// <example>
        ///     GET: Payment/New
        /// </example>
        [HttpGet]
        public ActionResult New()
        {
            return View();
        }

        // POST: Payment/Create
        /// <summary>
        ///    Receives a POST request containing information about a new Payment, 
        ///    Conveys this information to the AddPayment Method, inorder
        ///    to add the specific payment to the database.
        ///    Redirects to the "Payment List" page.
        /// </summary>
        /// <param name="Payment"> Payment Object </param>
        /// <returns>
        ///     A dynamic webpage which provides a new Payments's information.
        ///     or
        ///     A dynamic webpage which provides an Error Message.
        /// </returns>
        /// <example>
        ///     POST: Payment/Create
        /// </example>
        [HttpPost]
        public ActionResult Create(Payment Payment)
        {
            // Objective: Communicate with Payment Data Api to RETRIEVE an Payment
            // curl -H "Content-Type:application/json" -d @payment.json https://localhost:44338/api/PaymentData/AddPayment
            string url = "PaymentData/AddPayment";

            string jsonpayload = jss.Serialize(Payment);
            // Debug.WriteLine("The json payload is -> " + jsonpayload);

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

        // GET: Payment/Edit/5
        /// <summary>
        ///     Routes to a dynamically generated "Payment Edit" Page. 
        ///     That asks the user for new information as part of a form.
        ///     Gathers information from the database.
        /// </summary>
        /// <param name="id"> Payment ID </param>
        /// <returns> A dynamic webpage which provides the current information of a Payment </returns>
        /// <example>
        ///     GET: Payment/Edit/5
        /// </example>
        [HttpGet]
        public ActionResult Edit(int id)
        {
            // Objective: Communicate with Payment Data Api to RETRIEVE an Payment
            // curl https://localhost:44338/api/PayementData/FindPayment/{id}
            string url = "PaymentData/FindPayment/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;
            // Debug.WriteLine("The status code is " + response.StatusCode);

            PaymentDto SelectedPayement = response.Content.ReadAsAsync<PaymentDto>().Result;
            // Debug.WriteLine("Payment Data -> " + SelectedPayement);

            if (response.IsSuccessStatusCode)
            {
                return View(SelectedPayement);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Payment/Update/5
        /// <summary>
        ///     Receives a POST request containing information about an existing Payment in the database, 
        ///     with new values. Conveys this information to the UpdatePayment Method, 
        ///     and redirects to the "Payment List" page.
        /// </summary>
        /// <param name="id"> Payment ID </param>
        /// <param name="Payment"> Payment Object </param>
        /// <returns>  A dynamic webpage which provides the current information of a Payment </returns>
        /// <example>
        ///     POST: Payment/Update/5
        /// </example>
        [HttpPost]
        public ActionResult Update(int id, Payment Payment)
        {
            // Objective: Communicate with Payment Data Api to UPDATE an Payment
            // curl -H "Content-Type:application/json" -d @payment.json https://localhost:44338/api/PaymentData/UpdatePayment/{id}
            string url = "PaymentData/UpdatePayment/" + id;

            string jsonpayload = jss.Serialize(Payment);
            // Debug.WriteLine("The json payload is -> " + jsonpayload);

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

        // GET: Payment/DeleteConfirmation/5
        /// <summary>
        ///     Routes to a dynamically generated "Payment DeleteConfirmation" Page. 
        ///     Gathers information about a specific Payment that will be deleted from the database
        /// </summary>
        /// <param name="id"> Payment ID </param>
        /// <returns> A dynamic webpage which provides the current information of a Payment </returns>
        /// <example>
        ///     GET: Payment/DeleteConfirmation/5
        /// </example>
        [HttpGet]
        public ActionResult DeleteConfirmation(int id)
        {
            // Objective: Communicate with Payment Data Api to RETRIEVE an Payment
            // curl https://localhost:44338/api/PayementData/FindPayment/{id}
            string url = "PaymentData/FindPayment/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;
            // Debug.WriteLine("The status code is " + response.StatusCode);

            PaymentDto SelectedPayement = response.Content.ReadAsAsync<PaymentDto>().Result;
            // Debug.WriteLine("Payment Data -> " + SelectedPayement);

            if (response.IsSuccessStatusCode)
            {
                return View(SelectedPayement);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Payment/Delete/5
        /// <summary>
        ///    Receives a POST request containing information about an existing Payment in the database, 
        ///    Conveys this information to the DeletePayment Method, inorder
        ///    to remove the specific Payment from the database.
        ///    Redirects to the "Payment List" page.
        /// </summary>
        /// <param name="id"> Payment ID </param>
        /// <returns> A dynamic webpage which provides the current information of an Invoice </returns>
        /// <example>
        ///     POST: Payment/Delete/5
        /// </example>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // Objective: Communicate with Mantra Data Api to DELETE a Mantra
            // curl -d "" https://localhost:44338/api/PaymentData/DeletePayment/{id}
            string url = "PaymentData/DeletePayment/" + id;

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
