using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MHC_Hospital_Redesign.Models;
using System.Web;
using System.IO;
using System.Diagnostics;

namespace MHC_Hospital_Redesign.Controllers
{
    public class FaqDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        /// <summary>
        /// Returns all Faqs in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Faqs in the database.
        /// </returns>
        /// <example>
        /// GET: api/FaqData/ListAircrafts
        /// </example>
        // GET: api/Faq
        [HttpGet]
        [ResponseType(typeof(FaqDto))]
        public IHttpActionResult ListFaqs()
        {
            List<Faq> Faqs = db.Faqs.ToList();
            List<FaqDto> FaqDtos = new List<FaqDto>();

            Faqs.ForEach(a => FaqDtos.Add(new FaqDto()
            {
                FaqID = a.FaqID,
                DateAdded = a.DateAdded,
                FaqQuestions = a.FaqQuestions,
                FaqAnswers = a.FaqAnswers,
                FaqSort = a.FaqSort,


            }));

            return Ok(FaqDtos);
        }


        /// <summary>
        /// Gathers information about Faqs related to a particular Category
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Faqs in the database  that match to a particular Category id
        /// </returns>
        /// <param name="id">Category ID.</param>
        /// <example>
        /// GET: api/FaqData/ListFaqsForCategory/1
        /// </example>
        // GET: api/Faq/5
        [HttpGet]
        [ResponseType(typeof(FaqDto))]
        public IHttpActionResult ListFaqsForFaqCategory(int id)
        {
            //all aircrafts that operate by countries which match with our ID
            List<Faq> Faqs = db.Faqs.Where(
                a => a.FaqCategories.Any(
                    k => k.FaqCategoryID == id
                )).ToList();
            List<FaqDto> FaqDtos = new List<FaqDto>();

            Faqs.ForEach(a => FaqDtos.Add(new FaqDto()
            {
                FaqID = a.FaqID,
                DateAdded = a.DateAdded,
                FaqQuestions = a.FaqQuestions,
                FaqAnswers = a.FaqAnswers,
                FaqSort = a.FaqSort,

            }));

            return Ok(FaqDtos);
        }

        /// <summary>
        /// Associates a particular Category with a particular faq
        /// </summary>
        /// <param name="faqid">The faq ID primary key</param>
        /// <param name="faqcategoryid">The faqcategory ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/FaqData/AssociateFAQWithFaqCategory/9/1
        /// </example>

        [HttpPost]
        [Route("api/FaqData/AssociateFaqWithFaqCategory/{faqid}/{faqcategoryid}")]
        //[Authorize]
        public IHttpActionResult AssociateFaqWithFaqCategory(int faqid, int faqcategoryid)
        {

            Faq SelectedFaq = db.Faqs.Include(a => a.FaqCategories).Where(a => a.FaqID == faqid).FirstOrDefault();
            FaqCategory SelectedFaqCategory = db.FaqCategories.Find(faqcategoryid);

            if (SelectedFaq == null || SelectedFaqCategory == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input faq id is: " + faqid);
            Debug.WriteLine("selected faq question is: " + SelectedFaq.FaqQuestions);
            Debug.WriteLine("input faq category id is: " + faqcategoryid);
            Debug.WriteLine("selected faq category name is: " + SelectedFaqCategory.CategoryName);


            SelectedFaq.FaqCategories.Add(SelectedFaqCategory);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Removes an association between a particular faqcategory and a particular faq
        /// </summary>
        /// <param name="faqid">The faq ID primary key</param>
        /// <param name="faqcategoryid">The faqcategory ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/FaqData/UAssociateFaqWithFaqCategory/9/1
        /// </example>
        [HttpPost]
        [Route("api/FaqData/UnAssociateFaqWithFaqCategory/{faqid}/{faqcategoryid}")]
       // [Authorize]
        public IHttpActionResult UnAssociateFaqWithFaqCategory(int faqid, int faqcategoryid)
        {

            Faq SelectedFaq = db.Faqs.Include(a => a.FaqCategories).Where(a => a.FaqID == faqid).FirstOrDefault();
            FaqCategory SelectedFaqCategory = db.FaqCategories.Find(faqcategoryid);

            if (SelectedFaq == null || SelectedFaqCategory == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input faq id is: " + faqid);
            Debug.WriteLine("selected faq question is: " + SelectedFaq.FaqQuestions);
            Debug.WriteLine("input faq category id is: " + faqcategoryid);
            Debug.WriteLine("selected faq category name is: " + SelectedFaqCategory.CategoryName);



            SelectedFaq.FaqCategories.Remove(SelectedFaqCategory);
            db.SaveChanges();

            return Ok();
        }


        /// <summary>
        /// Returns all Faqs in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An Faq in the system matching up to the Faq ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the faq</param>
        /// <example>
        /// GET: api/FaqData/FindFaq/5
        /// </example>
        [ResponseType(typeof(FaqDto))]
        [HttpGet]
        public IHttpActionResult FindFaq(int id)
        {
            Faq Faq = db.Faqs.Find(id);
            FaqDto FaqDto = new FaqDto()
            {
                FaqID = Faq.FaqID,
                DateAdded = Faq.DateAdded,
                FaqQuestions = Faq.FaqQuestions,
                FaqAnswers = Faq.FaqAnswers,
                FaqSort = Faq.FaqSort,

            };
            if (Faq == null)
            {
                return NotFound();
            }

            return Ok(FaqDto);
        }

        /// <summary>
        /// Updates a particular Faq in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Faq ID primary key</param>
        /// <param name="aircraft">JSON FORM DATA of an faq</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/FaqData/UpdatFaq/5
        /// FORM DATA: Faq JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
       // [Authorize]
        public IHttpActionResult UpdateFaq(int id, Faq faq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != faq.FaqID)
            {

                return BadRequest();
            }

            db.Entry(faq).State = EntityState.Modified;


            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FaqExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Adds an Faq to the system
        /// </summary>
        /// <param name="faq">JSON FORM DATA of an faq</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Faq ID, Faq Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/FaqData/AddFaq
        /// FORM DATA: Faq JSON Object
        /// </example>
        [ResponseType(typeof(Faq))]
        [HttpPost]
       // [Authorize]
        public IHttpActionResult AddFaq(Faq faq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Faqs.Add(faq);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = faq.FaqID }, faq);
        }


        /// <summary>
        /// Deletes an faq from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the faq</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/FaqData/DeleteFaq/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Faq))]
        [HttpPost]
       // [Authorize]
        public IHttpActionResult DeleteFaq(int id)
        {
            Faq faq = db.Faqs.Find(id);
            if (faq == null)
            {
                return NotFound();
            }
            

            db.Faqs.Remove(faq);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FaqExists(int id)
        {
            return db.Faqs.Count(e => e.FaqID == id) > 0;
        }
    }
}

