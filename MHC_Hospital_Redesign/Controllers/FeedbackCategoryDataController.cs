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
    public class FeedbackCategoryDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        [ResponseType(typeof(FeedbackCategoryDto))]
        public IHttpActionResult ListFeedbackCategories()
        {
            List<FeedbackCategory> FeedbackCategories = db.FeedbackCategories.ToList();
            List<FeedbackCategoryDto> FeedbackCategoryDto = new List<FeedbackCategoryDto>();

            FeedbackCategories.ForEach(a => FeedbackCategoryDto.Add(new FeedbackCategoryDto()
            {
                FeedbackCategoryID = a.FeedbackCategoryID,
                FeedbackCategoryName = a.FeedbackCategoryName


            }));
            return Ok(FeedbackCategoryDto);
        }


        [ResponseType(typeof(FeedbackCategoryDto))]
        [HttpGet]
        public IHttpActionResult FindFeedbackCategory(int id)
        {
            FeedbackCategory FeedbackCategory = db.FeedbackCategories.Find(id);
            FeedbackCategoryDto FeedbackCategoryDto = new FeedbackCategoryDto()
            {
                FeedbackCategoryID = FeedbackCategory.FeedbackCategoryID,
                FeedbackCategoryName = FeedbackCategory.FeedbackCategoryName
                

            };

            if (FeedbackCategory == null)
            {
                return NotFound();
            }

            return Ok(FeedbackCategoryDto);
        }

        
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateFeedbackCategory(int id, FeedbackCategory feedbackCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != feedbackCategory.FeedbackCategoryID)
            {
                return BadRequest();
            }

            db.Entry(feedbackCategory).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackCategoryExists(id))
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

      


        [ResponseType(typeof(FeedbackCategory))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddFeedbackCategory(FeedbackCategory feedbackCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.FeedbackCategories.Add(feedbackCategory);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = feedbackCategory.FeedbackCategoryID }, feedbackCategory);
        }




        [ResponseType(typeof(FeedbackCategory))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteFeedbackCategory(int id)
        {
            FeedbackCategory feedbackCategory = db.FeedbackCategories.Find(id);
            if (feedbackCategory == null)
            {
                return NotFound();
            }

            db.FeedbackCategories.Remove(feedbackCategory);
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

        private bool FeedbackCategoryExists(int id)
        {
            return db.FeedbackCategories.Count(e => e.FeedbackCategoryID == id) > 0;
        }
    }
}
