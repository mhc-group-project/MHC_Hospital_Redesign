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

   
    public class TemplateController : Controller

    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static TemplateController()
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
    /// Grabs the authentication cookie sent to this controller. Allows for authenticated users to make administrative changes 
    /// </summary>
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



        // GET: Template/List
        public ActionResult List()
        {
            //goal: to communicate with template data api to retrieve a list of templates
            //curl https://localhost:44338/api/templatedata/listtemplates


            string url = "templatedata/listtemplates";
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            IEnumerable<TemplateDto> templates = response.Content.ReadAsAsync<IEnumerable<TemplateDto>>().Result;
            Debug.WriteLine("Recieived #" + templates.Count() + " templates");

            return View(templates);
        }

        // GET: Template/Details/5
        public ActionResult Details(int id)
        {
            DetailsTemplate ViewModel = new DetailsTemplate();

            //goal: to communicate with template data api to retrieve a one template
            //curl https://localhost:44338/api/templatedata/findtemplate/{id}


            string url = "templatedata/findtemplate/"+ id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            TemplateDto SelectedTemplate =  response.Content.ReadAsAsync<TemplateDto>().Result;
            Debug.WriteLine("template recieved " + SelectedTemplate.TemplateName);

            ViewModel.SelectedTemplate = SelectedTemplate;
            //Show Ecard associated with template
            //Send a request to get information about an ecard associated with a particular template id
            url = "ecarddata/listecardselectedtemplate/" + id;
            IEnumerable<EcardDto> TemplateForEcard = response.Content.ReadAsAsync<IEnumerable<EcardDto>>().Result;

            ViewModel.TemplateForEcard = TemplateForEcard;



            return View(ViewModel);


        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Template/New
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            GetApplicationCookie();

            string url = "ecarddata/listecards";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<EcardDto> EcardOptions = response.Content.ReadAsAsync<IEnumerable<EcardDto>>().Result;

            return View(EcardOptions);
        }

        // POST: Template/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public ActionResult Create(Template template)
        {
            Debug.WriteLine(template.TemplateName);
            //objective: add new template into our sstem using the API
            //curl -H "Content-type:application/json" -d@template.json https://localhost:44338/api/templatedata/addtemplate
       
            string url = "templatedata/addtemplate";

            string jsonpayload = jss.Serialize(template);

            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

           HttpResponseMessage response =  client.PostAsync(url, content).Result;
            if(response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            } else
            {
                return RedirectToAction("Error");
            }


    
        }

        // GET: Template/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {

            //objective: users are able to find the template to edit
            
            //url connection
            string url = "templatedata/findtemplate/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //parse content response
            TemplateDto selectedtemplate = response.Content.ReadAsAsync<TemplateDto>().Result;
            return View(selectedtemplate);
        } 

        // POST: Template/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Template template, HttpPostedFileBase TemplatePic, HttpPostedFileBase TemplateCss)
        {
            GetApplicationCookie(); //get token credentials
            //objective: edit a existing template in our system using the api
         

            string url = "templatedata/updatetemplate/" + id;

            string jsonpayload = jss.Serialize(template);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);


            // update request is succesful, and image data is recieved
            if (response.IsSuccessStatusCode && TemplatePic != null)
            {
                //Updating the template picture as a seperate request
                Debug.WriteLine("Calling Update Image method");
                //Send over image data
                url = "TemplateData/UploadTemplatePic/" + id;
                Debug.WriteLine("Recieved template picture " + TemplatePic.FileName);

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(TemplatePic.InputStream);
                requestcontent.Add(imagecontent, "TemplatePic", TemplatePic.FileName);
                response = client.PostAsync(url, requestcontent).Result;
            }
            //update request is sucessful, and file data is recieved
            if (response.IsSuccessStatusCode && TemplateCss != null)
            {
                //Updating the template css as a seperate request
                Debug.WriteLine("Calling Update css file method");
                //Send over file data
                url = "TemplateData/UploadTemplateCss/" + id;
                Debug.WriteLine("Recieved template picture " + TemplateCss.FileName);

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent filecontent = new StreamContent(TemplateCss.InputStream);
                requestcontent.Add(filecontent, "TemplateCss", TemplateCss.FileName);
                response = client.PostAsync(url, requestcontent).Result;

                return RedirectToAction("List");
            }
            else if (response.IsSuccessStatusCode)
            {
                //No file uploaded, but the update was still succesful
                return RedirectToAction("List");
            } 
            else
            {
                return RedirectToAction("Errors");
            }
        }

        // GET: Template/DeleteConfirm/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "templatedata/findtemplate/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            TemplateDto selectedtemplate = response.Content.ReadAsAsync<TemplateDto>().Result;
            return View(selectedtemplate);
        }

        // POST: Template/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
            //objective: delete a template from the system

            string url = "templatedata/deletetemplate/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);

            if (response.IsSuccessStatusCode)
            {
                // TODO: Add delete logic here
                Debug.WriteLine("test");
                return RedirectToAction("List");
            }
            else
            {
                return View("Error");
            }
        }
    }
}
