using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebServer.Models;

namespace WebServer.Controllers
{   
    public class QUE_FeedbackQuestionsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
                

        // GET: api/QUE_FeedbackQuestions
        public IQueryable<QUE_FeedbackQuestions> GetQUE_FeedbackQuestions()
        {
            return db.QUE_FeedbackQuestions;
        }

        //////////////////////////
        // Url:.../api/QUE_FeedbackQuestions/{id:int}
        // Method: GET
        // Authorization Required: NO
        // Parameter: id of Question (id: int)
        // Result: Specified Question
        // Description:
        //     API is called when Client requests a Question with the specified id
        //////////////////////////

        [Authorize]
        [Route("test/api/Feedbacksession_protected/{id}")]
        [HttpGet]
        [ResponseType(typeof(QUE_FeedbackQuestions))]
        public async Task<IHttpActionResult> GetQUE_protectedFeedbackQuestions(int id)
        {         
            QUE_FeedbackQuestions qUE_FeedbackQuestions = await db.QUE_FeedbackQuestions.FindAsync(id);
            if (qUE_FeedbackQuestions == null)
            {
                return NotFound();
            }

            qUE_FeedbackQuestions.QUE_text = "PROTECTED";

            return Ok(qUE_FeedbackQuestions);
        }

        //////////////////////////
        // Url:.../api/QUE_FeedbackQuestions/{id:int}
        // Method: GET
        // Authorization Required: NO
        // Parameter: id of Question (id: int)
        // Result: Specified Question
        // Description:
        //     API is called when Client requests a Question with the specified id
        //////////////////////////

        [Route("test/api/Feedbacksession_unprotected/{id}")]
        [HttpGet]
        [ResponseType(typeof(QUE_FeedbackQuestions))]
        public async Task<IHttpActionResult> GetQUE_unprotectedFeedbackQuestions(int id)
        {
            QUE_FeedbackQuestions qUE_FeedbackQuestions = await db.QUE_FeedbackQuestions.FindAsync(id);
            if (qUE_FeedbackQuestions == null)
            {
                return NotFound();
            }

            qUE_FeedbackQuestions.QUE_text = "UNPROTECTED";

            return Ok(qUE_FeedbackQuestions);
        }

        // PUT: api/QUE_FeedbackQuestions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutQUE_FeedbackQuestions(int id, QUE_FeedbackQuestions qUE_FeedbackQuestions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != qUE_FeedbackQuestions.QUE_id)
            {
                return BadRequest();
            }

            db.Entry(qUE_FeedbackQuestions).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QUE_FeedbackQuestionsExists(id))
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

        // POST: api/QUE_FeedbackQuestions
        [ResponseType(typeof(QUE_FeedbackQuestions))]
        public async Task<IHttpActionResult> PostQUE_FeedbackQuestions(QUE_FeedbackQuestions qUE_FeedbackQuestions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.QUE_FeedbackQuestions.Add(qUE_FeedbackQuestions);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = qUE_FeedbackQuestions.QUE_id }, qUE_FeedbackQuestions);
        }

        // DELETE: api/QUE_FeedbackQuestions/5
        [ResponseType(typeof(QUE_FeedbackQuestions))]
        public async Task<IHttpActionResult> DeleteQUE_FeedbackQuestions(int id)
        {
            QUE_FeedbackQuestions qUE_FeedbackQuestions = await db.QUE_FeedbackQuestions.FindAsync(id);
            if (qUE_FeedbackQuestions == null)
            {
                return NotFound();
            }

            db.QUE_FeedbackQuestions.Remove(qUE_FeedbackQuestions);
            await db.SaveChangesAsync();

            return Ok(qUE_FeedbackQuestions);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QUE_FeedbackQuestionsExists(int id)
        {
            return db.QUE_FeedbackQuestions.Count(e => e.QUE_id == id) > 0;
        }
    }
}