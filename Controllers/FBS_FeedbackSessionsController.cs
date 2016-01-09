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
//E-Mail//
using System.Text;
using System.Net.Mail;
using System.Net.Mime;

namespace WebServer.Controllers
{
    [Authorize]
    public class FBS_FeedbackSessionsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/FBS_FeedbackSessions
        public IQueryable<FBS_FeedbackSessions> GetFBS_FeedbackSessions()
        {
            return db.FBS_FeedbackSessions;
        }

        // GET: api/FBS_FeedbackSessions/5
        [ResponseType(typeof(FBS_FeedbackSessions))]
        public async Task<IHttpActionResult> GetFBS_FeedbackSessions(int id)
        {

            /* DB Tests - T */
            //var test = from x in db.FBS_FeedbackSessions
            //           where x.ApplicationUser.Email == "Simon@TheRealUser.com"
            //           select x;

            //if (!test.Any())
            //{
            //    return NotFound();
            //}

            //return Ok(test);

            // +++++++++++++++++++++++++++++++++++++

            //FBS_FeedbackSessions fBS_FeedbackSessions = await db.FBS_FeedbackSessions.FindAsync(id);
            //if (fBS_FeedbackSessions == null)
            //{
            //    return NotFound();
            //}            

            //return Ok(fBS_FeedbackSessions);

            // ++++++++++++++++++++++++++++++++++++++

            MailMessage mailMsg = new MailMessage();

            //to
            mailMsg.To.Add(new MailAddress("t.rocket@web.de", "TobiRakete"));

            //from
            mailMsg.From = new MailAddress("tobitobinc@gmail.com", "TheProject");

            // Subject and multipart/alternative Body
            mailMsg.Subject = "First Project E-Mail";
            string text = "text body";
            string html = @"<p>html body</p>";
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            // Init SmtpClient and send
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("tobitobinc@gmail.com", ")!8291IdoU=0");
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;

            smtpClient.Send(mailMsg);

            FBS_FeedbackSessions fBS_FeedbackSessions = await db.FBS_FeedbackSessions.FindAsync(id);
            if (fBS_FeedbackSessions == null)
            {
                return NotFound();
            }

            return Ok(fBS_FeedbackSessions);
        }

        // PUT: api/FBS_FeedbackSessions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutFBS_FeedbackSessions(int id, FBS_FeedbackSessions fBS_FeedbackSessions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != fBS_FeedbackSessions.FBS_id)
            {
                return BadRequest();
            }

            db.Entry(fBS_FeedbackSessions).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FBS_FeedbackSessionsExists(id))
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

        // POST: api/FBS_FeedbackSessions
        [ResponseType(typeof(FBS_FeedbackSessions))]
        public async Task<IHttpActionResult> PostFBS_FeedbackSessions(FBS_FeedbackSessions fBS_FeedbackSessions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.FBS_FeedbackSessions.Add(fBS_FeedbackSessions);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = fBS_FeedbackSessions.FBS_id }, fBS_FeedbackSessions);
        }

        // DELETE: api/FBS_FeedbackSessions/5
        [ResponseType(typeof(FBS_FeedbackSessions))]
        public async Task<IHttpActionResult> DeleteFBS_FeedbackSessions(int id)
        {
            FBS_FeedbackSessions fBS_FeedbackSessions = await db.FBS_FeedbackSessions.FindAsync(id);
            if (fBS_FeedbackSessions == null)
            {
                return NotFound();
            }

            db.FBS_FeedbackSessions.Remove(fBS_FeedbackSessions);
            await db.SaveChangesAsync();

            return Ok(fBS_FeedbackSessions);
        }

        // Free ressources utilized by this class - T
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FBS_FeedbackSessionsExists(int id)
        {
            return db.FBS_FeedbackSessions.Count(e => e.FBS_id == id) > 0;
        }
    }
}