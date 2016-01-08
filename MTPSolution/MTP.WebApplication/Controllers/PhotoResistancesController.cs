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
using MTP.Core.EntityFramework;

namespace MTP.WebApplication.Controllers
{
    public class PhotoResistancesController : ApiController
    {
        private TelemetryDB db = new TelemetryDB();

        // GET: api/PhotoResistances
        public IQueryable<PhotoResistance> GetPhotoResistances()
        {
            return db.PhotoResistances;
        }

        // GET: api/PhotoResistances/5
        [ResponseType(typeof(PhotoResistance))]
        public async Task<IHttpActionResult> GetPhotoResistance(int id)
        {
            PhotoResistance photoResistance = await db.PhotoResistances.FindAsync(id);
            if (photoResistance == null)
            {
                return NotFound();
            }

            return Ok(photoResistance);
        }

        // PUT: api/PhotoResistances/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPhotoResistance(int id, PhotoResistance photoResistance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != photoResistance.Id)
            {
                return BadRequest();
            }

            db.Entry(photoResistance).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhotoResistanceExists(id))
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

        // POST: api/PhotoResistances
        [ResponseType(typeof(PhotoResistance))]
        public async Task<IHttpActionResult> PostPhotoResistance(PhotoResistance photoResistance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(photoResistance.CollectionTime == null)
            {
                photoResistance.CollectionTime = DateTime.UtcNow;
            }
            
            db.PhotoResistances.Add(photoResistance);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhotoResistanceExists(photoResistance.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = photoResistance.Id }, photoResistance);
        }

        // DELETE: api/PhotoResistances/5
        [ResponseType(typeof(PhotoResistance))]
        public async Task<IHttpActionResult> DeletePhotoResistance(int id)
        {
            PhotoResistance photoResistance = await db.PhotoResistances.FindAsync(id);
            if (photoResistance == null)
            {
                return NotFound();
            }

            db.PhotoResistances.Remove(photoResistance);
            await db.SaveChangesAsync();

            return Ok(photoResistance);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PhotoResistanceExists(int id)
        {
            return db.PhotoResistances.Count(e => e.Id == id) > 0;
        }
    }
}