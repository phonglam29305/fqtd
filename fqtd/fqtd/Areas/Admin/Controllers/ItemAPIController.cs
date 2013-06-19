using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using fqtd.Areas.Admin.Models;

namespace fqtd.Areas.Admin.Controllers
{
    public class ItemAPIController : ApiController
    {
        private fqtdEntities db = new fqtdEntities();

        // GET api/ItemAPI
        public IEnumerable<BrandItems> GetBrandItems()
        {
            var branditems = db.BrandItems.Include(b => b.tbl_Brands).Include(b => b.tbl_Item_Location);
            return branditems.AsEnumerable();
        }

        // GET api/ItemAPI/5
        public BrandItems GetBrandItems(int id)
        {
            BrandItems branditems = db.BrandItems.Find(id);
            if (branditems == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return branditems;
        }

        // PUT api/ItemAPI/5
        public HttpResponseMessage PutBrandItems(int id, BrandItems branditems)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != branditems.ItemID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(branditems).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/ItemAPI
        public HttpResponseMessage PostBrandItems(BrandItems branditems)
        {
            if (ModelState.IsValid)
            {
                db.BrandItems.Add(branditems);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, branditems);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = branditems.ItemID }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/ItemAPI/5
        public HttpResponseMessage DeleteBrandItems(int id)
        {
            BrandItems branditems = db.BrandItems.Find(id);
            if (branditems == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.BrandItems.Remove(branditems);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, branditems);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}