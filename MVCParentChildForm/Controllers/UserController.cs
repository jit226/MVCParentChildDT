using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCParentChildForm.Models;

namespace MVCParentChildForm.Controllers
{
    public class UserController : Controller
    {
        private MVCParentChildFormDataContext db = new MVCParentChildFormDataContext();

        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.OrderList = db.Orders.Where(o => o.UserId == id);
            }
            return View(user);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,EmailId,MobileNo,Orders")] User user)
        {
            if (ModelState.IsValid)
            {
                user.CreationDate = DateTime.Now;
                user.UpdateDate = DateTime.Now;
                db.Users.Add(user);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(user.Orders))
                {
                    int getUserId = user.Id;
                    string[] strOrders = user.Orders.Split('#');

                    foreach (var strOrder in strOrders)
                    {
                        if (!string.IsNullOrEmpty(strOrder))
                        {
                            string[] orderitem = strOrder.Split('~');

                            Order order = new Order();
                            order.UserId = getUserId;
                            order.ProductName = orderitem[0];
                            order.ProductType = orderitem[1];
                            order.Amount = Convert.ToDecimal(orderitem[2]);
                            db.Orders.Add(order);
                            db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.OrderList = db.Orders.Where(o => o.UserId == id);
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,EmailId,MobileNo,Orders")] User user)
        {
            if (ModelState.IsValid)
            {
                
                user.UpdateDate = DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.Entry(user).Property(u => u.CreationDate).IsModified = false;
                db.SaveChanges();

                //Clear related orders
                var removeOrders= db.Orders.Where(o => o.UserId == user.Id);
                db.Orders.RemoveRange(removeOrders);
                db.SaveChanges();
                //add related order
                if (!string.IsNullOrEmpty(user.Orders))
                {
                    int getUserId = user.Id;

                    string[] strOrders = user.Orders.Split('#');

                    foreach (var strOrder in strOrders)
                    {
                        if (!string.IsNullOrEmpty(strOrder))
                        {
                            string[] orderitem = strOrder.Split('~');

                            Order order = new Order();
                            order.UserId = getUserId;
                            order.ProductName = orderitem[0];
                            order.ProductType = orderitem[1];
                            order.Amount = Convert.ToDecimal(orderitem[2]);
                            db.Orders.Add(order);
                            db.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
