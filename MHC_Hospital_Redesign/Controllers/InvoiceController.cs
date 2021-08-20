using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using MHC_Hospital_Redesign.Models;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace MHC_Hospital_Redesign.Controllers
{
    public class InvoiceController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static InvoiceController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://mhcredesign-env.eba-skfg7g3r.us-east-2.elasticbeanstalk.com/api/");
        }

        // GET: Invoice/List
        /// <summary>
        ///     Routes to a dynamically generated "Invoice List" Page. 
        ///     Gathers information about all the invoices in the database.
        /// </summary>
        /// <returns> A dynamic webpage which displays a List of Invoices </returns>
        /// <example>
        ///     GET: Invoice/List
        /// </example>
        [HttpGet]
        public ActionResult List()
        {
            // Objective: Communicate with Invoice Data Api to RETRIEVE a list of Invoices
            // curl https://localhost:44338/api/InvoiceData/ListInvoices
            string url = "InvoiceData/ListInvoices";

            HttpResponseMessage response = client.GetAsync(url).Result;
            Debug.WriteLine("The status code is " + response.StatusCode);

            IEnumerable<InvoiceDto> Invoices = response.Content.ReadAsAsync<IEnumerable<InvoiceDto>>().Result;
            // Debug.WriteLine("Number of invoices -> " + Invoices.Count());

            if (response.IsSuccessStatusCode)
            {
                return View(Invoices);
            } else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Invoice/Details/5
        /// <summary>
        ///     Routes to a dynamically generated "Invoice Details" Page.
        ///     Gathers information about a specific invoice from the database
        /// </summary>
        /// <param name="id"> Invoice ID </param>
        /// <returns> A dynamic webpage which provides the current information of an Invoice </returns>
        /// <example>
        ///     GET: Invoice/Details/5
        /// </example>
        [HttpGet]
        public ActionResult Details(int id)
        {
            // Objective: Communicate with Invoice Data Api to RETRIEVE an Invoice
            // curl https://localhost:44338/api/InvoiceData/FindInvoice/{id}
            string url = "InvoiceData/FindInvoice/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;
            // Debug.WriteLine("The status code is " + response.StatusCode);

            InvoiceDto SelectedInvoice = response.Content.ReadAsAsync<InvoiceDto>().Result;
            // Debug.WriteLine("Invoice Data -> " + SelectedInvoice);

            if (response.IsSuccessStatusCode)
            {
                return View(SelectedInvoice);
            } else
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

        // GET: Invoice/New
        /// <summary>
        ///     Routes to a dynamically generated "Invooice New" Page. 
        ///     Gathers information about a new Invoice from a form 
        ///     that will be added to the database.
        /// </summary>
        /// <returns> A dynamic webpage which asks the user for new information regarding an Invoice as part of a form. </returns>
        /// <example>
        ///     GET: Invoice/New
        /// </example>
        [HttpGet]
        public ActionResult New()
        {
            return View();
        }

        // POST: Invoice/Create
        /// <summary>
        ///    Receives a POST request containing information about a new Invoice, 
        ///    Conveys this information to the AddInvoice Method, inorder
        ///    to add the specific Invoice to the database.
        ///    Redirects to the "Invoice List" page.
        /// </summary>
        /// <param name="Invoice"> Invoice Object </param>
        /// <returns>
        ///     A dynamic webpage which provides a new Invoice's information.
        ///     or
        ///     A dynamic webpage which provides an Error Message.
        /// </returns>
        /// <example>
        ///     POST: Invoice/Create
        /// </example>
        [HttpPost]
        public ActionResult Create(Invoice Invoice)
        {
            // Objective: Communicate with Invoice Data Api to RETRIEVE an Invoice
            // curl -H "Content-Type:application/json" -d @invoice.json https://localhost:44338/api/InvoiceData/AddInvoice
            string url = "InvoiceData/AddInvoice";

            string jsonpayload = jss.Serialize(Invoice);
            // Debug.WriteLine("The json payload is -> " + jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if(response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            } else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Invoice/Edit/5
        /// <summary>
        ///     Routes to a dynamically generated "Invoice Edit" Page. 
        ///     That asks the user for new information as part of a form.
        ///     Gathers information from the database.
        /// </summary>
        /// <param name="id"> Invoice ID </param>
        /// <returns> A dynamic webpage which provides the current information of an Invoice </returns>
        /// <example>
        ///     GET: Invoice/Edit/5
        /// </example>
        public ActionResult Edit(int id)
        {
            // Objective: Communicate with Invoice Data Api to RETRIEVE an Invoice
            // curl https://localhost:44338/api/InvoiceData/FindInvoice/{id}
            string url = "InvoiceData/FindInvoice/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;
            // Debug.WriteLine("The status code is " + response.StatusCode);

            InvoiceDto SelectedInvoice = response.Content.ReadAsAsync<InvoiceDto>().Result;
            // Debug.WriteLine("Invoice Data -> " + SelectedInvoice);

            if (response.IsSuccessStatusCode)
            {
                return View(SelectedInvoice);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Invoice/Update/5
        /// <summary>
        ///     Receives a POST request containing information about an existing Invoice in the database, 
        ///     with new values. Conveys this information to the UpdateInvoice Method, 
        ///     and redirects to the "Invoice List" page.
        /// </summary>
        /// <param name="id"> Invoice ID </param>
        /// <param name="Invoice"> Invoice Object </param>
        /// <returns> A dynamic webpage which provides the current information of an Invoice </returns>
        /// <example>
        ///     POST: Invoice/Update/5
        /// </example>
        [HttpPost]
        public ActionResult Edit(int id, Invoice Invoice)
        {
            // Objective: Communicate with Invoice Data Api to UPDATE an Invoice
            // curl -H "Content-Type:application/json" -d @invoice.json https://localhost:44338/api/InvoiceData/UpdateInvoice/{id}
            string url = "InvoiceData/UpdateInvoice/" + id;

            string jsonpayload = jss.Serialize(Invoice);
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

        // GET: Invoice/DeleteConfirmation/5
        /// <summary>
        ///     Routes to a dynamically generated "Invoice DeleteConfirmation" Page. 
        ///     Gathers information about a specific Invoice that will be deleted from the database
        /// </summary>
        /// <param name="id"> Invoice ID </param>
        /// <returns> A dynamic webpage which provides the current information of an Invoice </returns>
        /// <example>
        ///     GET: Invoice/DeleteConfirmation/5
        /// </example>
        [HttpGet]
        public ActionResult DeleteConfirmation(int id)
        {
            // Objective: Communicate with Invoice Data Api to RETRIEVE an Invoice
            // curl https://localhost:44338/api/InvoiceData/FindInvoice/{id}
            string url = "InvoiceData/FindInvoice/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;
            // Debug.WriteLine("The status code is " + response.StatusCode);

            InvoiceDto SelectedInvoice = response.Content.ReadAsAsync<InvoiceDto>().Result;
            // Debug.WriteLine("Invoice Data -> " + SelectedInvoice);

            if (response.IsSuccessStatusCode)
            {
                return View(SelectedInvoice);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Invoice/Delete/5
        /// <summary>
        ///    Receives a POST request containing information about an existing Invoice in the database, 
        ///    Conveys this information to the DeleteInvoice Method, inorder
        ///    to remove the specific Invoice from the database.
        ///    Redirects to the "Invoice List" page.
        /// </summary>
        /// <param name="id"> Invoice ID </param>
        /// <returns> A dynamic webpage which provides the current information of an Invoice </returns>
        /// <example>
        ///     POST: Invoice/Delete/5
        /// </example>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // Objective: Communicate with Mantra Data Api to DELETE a Mantra
            // curl -d "" https://localhost:44338/api/InvoiceData/DeleteInvoice/{id}
            string url = "InvoiceData/DeleteInvoice/" + id;

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
