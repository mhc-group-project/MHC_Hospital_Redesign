using System;
using System.IO;
using System.Web;
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
    public class EcardDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Shows all Ecards in system
        /// </summary>
        /// <returns>
        /// HEADER: 200 Ok
        /// CONTENT: All ecards in database
        /// </returns>
        /// <example>
        /// GET: api/EcardData/ListEcards
        /// </example>


        [HttpGet]
        public IEnumerable<EcardDto> ListEcards()
        {
            List<Ecard> Ecards = db.Ecards.ToList();
            List<EcardDto> EcardDtos = new List<EcardDto>();

            Ecards.ForEach(e => EcardDtos.Add(new EcardDto()
            {
                EcardId = e.EcardId,
                Message = e.Message,
                SenderName = e.SenderName,
                PatientName = e.PatientName,
                TemplateId = e.TemplateId,
                TemplateName = e.Template.TemplateName,
                TemplateHasPic = e.Template.TemplateHasPic,
                TemplatePicExtension = e.Template.TemplatePicExtension,
                TemplateStyle = e.Template.TemplateStyle
      
            }));

            return EcardDtos;
        }

        
      
       ///<summary>
       /// Returns all ecards in the system
       /// </summary>
       /// <<returns>
       /// HEADER: 200 (OK)
       /// CONTENT: A ecard in the system matching up to the ecard id primrary key
       /// or 
       /// HEADER: 404 (NOT FOUND)
       /// </returns>
       /// <example>
       /// GET: api/EcardData/FindEcard/3
       /// </example>

        [ResponseType(typeof(Ecard))]
        [HttpGet]
        public IHttpActionResult FindEcard(int id)
        {
           
            Ecard Ecard = db.Ecards.Find(id);
            EcardDto EcardDto = new EcardDto()
            {
                EcardId = Ecard.EcardId,
                Message = Ecard.Message,
                SenderName = Ecard.SenderName,
                PatientName = Ecard.PatientName,
                TemplateId = Ecard.TemplateId,
                TemplateHasPic = Ecard.Template.TemplateHasPic,
                TemplateName = Ecard.Template.TemplateName,
                TemplatePicExtension = Ecard.Template.TemplatePicExtension,
                TemplateStyle = Ecard.Template.TemplateStyle

            };

            if (Ecard == null)
            {
                return NotFound();
            }

            return Ok(EcardDto);
        }

        /// <summary>
        /// Updates a particular ecard in the system with POST data input
        /// </summary>
        /// <param name="id">Represents the ecard id primary key</param>
        /// <param name="ecard">JSON FORM data of a ecard</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/EcardData/UpdateEcard/3
        /// FORM DATA: Ecard JSON object
        /// </example>
       
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateEcard(int id, Ecard ecard)
        {
            Debug.WriteLine("I have reached the update ecard method!");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != ecard.EcardId)
            {
                Debug.WriteLine("ID mismatch");
                Debug.WriteLine("GET paramter" + id);
                Debug.WriteLine("POST paramter" + ecard.EcardId);
                return BadRequest();
            }

            db.Entry(ecard).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EcardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            Debug.WriteLine("None of the conditions trigger");
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Adds a ecard to the system
        /// </summary>
        /// <param name="ecard">JSON FORM DATA of a ecard</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Ecard ID, Ecard Data
        /// Or 
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/EcardData/AddEcard
        /// FORM DATA: Ecard JSON Object
        /// </example>

        [ResponseType(typeof(Ecard))]
        [HttpPost]
        public IHttpActionResult AddEcard(Ecard ecard)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Ecards.Add(ecard);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = ecard.EcardId }, ecard);
        }

        /// <summary>
        /// Deletes a ecard from the system by it's ID
        /// </summary>
        /// <param name="id">The primary key of the ecard</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or 
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/EcardData/DeleteEcard/3
        /// FORM DATA: (empty)
        /// </example>

        // DELETE: api/EcardData/DeleteEcard/3
        [ResponseType(typeof(Ecard))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteEcard(int id)
        {
            Ecard ecard = db.Ecards.Find(id);
            if (ecard == null)
            {
                return NotFound();
            }

            db.Ecards.Remove(ecard);
            db.SaveChanges();

            return Ok(ecard);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EcardExists(int id)
        {
            return db.Ecards.Count(e => e.EcardId == id) > 0;
        }
    }
}