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
using System.Diagnostics;

namespace MHC_Hospital_Redesign.Controllers
{
    public class FaqCategoryDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all the FaqCategories in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all FaqCategories in the database
        /// </returns>
        /// <example>
        /// GET: api/FaqCategoryData/ListFaqCategories
        /// </example>

        [HttpGet]
        [ResponseType(typeof(FaqCategoryDto))]
        public IHttpActionResult ListFaqCategories()
        {
            List<FaqCategory> FaqCategories = db.FaqCategories.ToList();
            List<FaqCategoryDto> FaqCategoryDtos = new List<FaqCategoryDto>();

            FaqCategories.ForEach(k => FaqCategoryDtos.Add(new FaqCategoryDto()
            {
                FaqCategoryID = k.FaqCategoryID,
                CategoryDateAdded = k.CategoryDateAdded,
                CategoryName = k.CategoryName,
                CategoryDescription = k.CategoryDescription,
                CategoryColor = k.CategoryColor
            })) ;
            return Ok(FaqCategoryDtos);
        }
        /// <summary>
        /// Returns all FaqCategories in the system associated with a particular Faq.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all FaqCAtegories the database which includes a particular Faq
        /// </returns>
        /// <param name="id">Faq Primary Key</param>
        /// <example>
        /// GET: api/FaqcategoryData/ListFaqCategoriesForFaq/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(FaqCategory))]
        public IHttpActionResult ListFaqCategoriesForFaq(int id)
        {
            List<FaqCategory> FaqCategories = db.FaqCategories.Where(
                k => k.Faqs.Any(
                    a => a.FaqID == id)
                ).ToList();
            List<FaqCategoryDto> FaqCategoryDtos = new List<FaqCategoryDto>();

            FaqCategories.ForEach(k => FaqCategoryDtos.Add(new FaqCategoryDto()
            {
                FaqCategoryID = k.FaqCategoryID,
                CategoryDateAdded = k.CategoryDateAdded,
                CategoryName = k.CategoryName,
                CategoryDescription = k.CategoryDescription,
                CategoryColor = k.CategoryColor
            }));

            return Ok(FaqCategoryDtos);
        }

        /// <summary>
        /// Returns FaqCategories in the system not caring for a particular Faq.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all FaqCatgories in the database not taking care of a particular Faq
        /// </returns>
        /// <param name="id">Faq Primary Key</param>
        /// <example>
        /// GET: api/FaqCategoryData/ListCategoriesNotListingFaq/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(FaqCategoryDto))]
        public IHttpActionResult ListCategoriesNotListingFaq(int id)
        {
            List<FaqCategory> FaqCategories = db.FaqCategories.Where(
                k => !k.Faqs.Any(
                    a => a.FaqID == id)
                ).ToList();
            List<FaqCategoryDto> FaqCategoryDtos = new List<FaqCategoryDto>();

            FaqCategories.ForEach(k => FaqCategoryDtos.Add(new FaqCategoryDto()
            {
                FaqCategoryID = k.FaqCategoryID,
                CategoryDateAdded = k.CategoryDateAdded,
                CategoryName = k.CategoryName,
                CategoryDescription = k.CategoryDescription,
                CategoryColor = k.CategoryColor
               
            }));

            return Ok(FaqCategoryDtos);
        }


        /// <summary>
        /// Returns all FaqCategories in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An FaqCategory in the system matching up to the FaqCategory ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the FaqCategory</param>
        /// <example>
        /// GET: api/FaqCategoryData/FindFaqCategory/5
        /// </example>
        [ResponseType(typeof(FaqCategoryDto))]
        [HttpGet]
        public IHttpActionResult FindFaqCategory(int id)
        {
            FaqCategory FaqCategory = db.FaqCategories.Find(id);
            FaqCategoryDto FaqCategoryDto = new FaqCategoryDto()
            {
                FaqCategoryID = FaqCategory.FaqCategoryID,
                CategoryDateAdded = FaqCategory.CategoryDateAdded,
                CategoryName = FaqCategory.CategoryName,
                CategoryDescription = FaqCategory.CategoryDescription,
                CategoryColor = FaqCategory.CategoryColor
            };
            if (FaqCategory == null)
            {
                return NotFound();
            }

            return Ok(FaqCategoryDto);
        }

        /// <summary>
        /// Updates a particular FaqCategory in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the FaqCategory ID primary key</param>
        /// <param name="Faqcategory">JSON FORM DATA of an FaQCategory</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/FaqCategoryData/UpdateFaqCategory/5
        /// FORM DATA:FaqCategory JSON Object
        /// </example>

        [ResponseType(typeof(void))]
        [HttpPost]
        //[Authorize]
        public IHttpActionResult UpdateFaqCategory(int id, FaqCategory FaqCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != FaqCategory.FaqCategoryID)
            {
                return BadRequest();
            }

            db.Entry(FaqCategory).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FaqCategoryExists(id))
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
        /// Adds an FaqCategory to the system
        /// </summary>
        /// <param name="FaqCategory">JSON FORM DATA of a Category</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: FaqCategory ID, FaqCategory Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/FaqCategoryData/AddFaqCategory
        /// FORM DATA:Faq Category JSON Object
        /// </example>


        [ResponseType(typeof(FaqCategory))]
        [HttpPost]
        //[Authorize]
        public IHttpActionResult AddFaqCategory(FaqCategory faqCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.FaqCategories.Add(faqCategory);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = faqCategory.FaqCategoryID }, faqCategory);
        }

        /// <summary>
        /// Deletes an FaqCategory from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of a FaqCategory</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/FaqCategoryData/DeleteFaqCategory/5
        /// FORM DATA: (empty)
        /// </example>

        [ResponseType(typeof(FaqCategory))]
        [HttpPost]
       // [Authorize]
        public IHttpActionResult DeleteFaqCategory(int id)
        {
            FaqCategory FaqCategory = db.FaqCategories.Find(id);
            if (FaqCategory == null)
            {
                return NotFound();
            }

            db.FaqCategories.Remove(FaqCategory);
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

        private bool FaqCategoryExists(int id)
        {
            return db.FaqCategories.Count(e => e.FaqCategoryID == id) > 0;
        }
    }
}