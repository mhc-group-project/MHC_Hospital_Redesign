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
    public class FeedbackDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        
        [HttpGet]
        [ResponseType(typeof(FeedbackDto))]
        public IHttpActionResult ListFeedbacks()
        {
            List<Feedback> Feedbacks = db.Feedbacks.ToList();
            List<FeedbackDto> FeedbackDtos = new List<FeedbackDto>();

            Feedbacks.ForEach(a => FeedbackDtos.Add(new FeedbackDto()
            {
                FeedbackId = a.FeedbackId,
                UserName = a.UserName,
                TimeStamp = a.TimeStamp,
                FeedbackContent = a.FeedbackContent,
                FeedbackCategoryID = a.FeedbackCategoryID
            }));

            return Ok(FeedbackDtos);
        }

        
        [HttpGet]
        [ResponseType(typeof(IEnumerable<FeedbackDto>))]
        public IHttpActionResult ListFeedbacksForFeedbackCategory(int id)
        {
            List<Feedback> Feedbacks = db.Feedbacks.Where(a => a.FeedbackCategoryID == id).ToList();
            List<FeedbackDto> FeedbackDtos = new List<FeedbackDto>();

            Feedbacks.ForEach(a => FeedbackDtos.Add(new FeedbackDto()
            {
                FeedbackId = a.FeedbackId,
                UserName = a.UserName,
                TimeStamp = a.TimeStamp,
                FeedbackContent = a.FeedbackContent,
                FeedbackCategoryID = a.FeedbackCategoryID
            }));

            return Ok(FeedbackDtos);
        }

        
        [ResponseType(typeof(FeedbackDto))]
        [HttpGet]
        public IHttpActionResult FindFeedback(int id)
        {
            Feedback Feedback = db.Feedbacks.Find(id);
            
            if (Feedback == null)
            {
                return NotFound();
            }

            FeedbackDto FeedbackDto = new FeedbackDto()
            {
                FeedbackId = Feedback.FeedbackId,
                UserName = Feedback.UserName,
                TimeStamp = Feedback.TimeStamp,
                FeedbackContent = Feedback.FeedbackContent,
                FeedbackCategoryID = Feedback.FeedbackCategoryID,
                FeedbackCategoryName = Feedback.FeedbackCategory.FeedbackCategoryName

            };

            return Ok(FeedbackDto);
        }

        
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateFeedback(int id, Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != feedback.FeedbackId)
            {

                return BadRequest();
            }

            feedback.TimeStamp = DateTime.UtcNow;

            db.Entry(feedback).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackExists(id))
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

        
        [ResponseType(typeof(Feedback))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddFeedback(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            feedback.TimeStamp = DateTime.UtcNow;

            db.Feedbacks.Add(feedback);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = feedback.FeedbackId }, feedback);
        }

        
        [ResponseType(typeof(Feedback))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteFeedback(int id)
        {
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return NotFound();
            }

            db.Feedbacks.Remove(feedback);
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

        private bool FeedbackExists(int id)
        {
            return db.Feedbacks.Count(e => e.FeedbackId == id) > 0;
        }
    }
}
