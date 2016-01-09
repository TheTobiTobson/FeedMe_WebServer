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

        // GET: api/QUE_FeedbackQuestions/5
        [ResponseType(typeof(QUE_FeedbackQuestions))]
        public async Task<IHttpActionResult> GetQUE_FeedbackQuestions(int id)
        {


            /* DB Tests - T */
            //var test = from t in db.QUE_FeedbackQuestions
            //               .Include(tw => tw.FBS_FeedbackSessions)
            //           where t.FBS_FeedbackSessions.ApplicationUser.Id == "028ddef6-23f6-4fbd-bbb8-ad1e4fa365fd"
            //           select t;

            //if(!test.Any())
            //{
            //    return NotFound();
            //}

            //return Ok(test);
                        
            QUE_FeedbackQuestions qUE_FeedbackQuestions = await db.QUE_FeedbackQuestions.FindAsync(id);
            if (qUE_FeedbackQuestions == null)
            {
                return NotFound();
            }

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