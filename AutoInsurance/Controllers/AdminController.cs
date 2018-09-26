using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoInsurance.Models;

namespace AutoInsurance.Controllers
{
    public class AdminController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();



        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            decimal QuoteTotal = insuree.Quote;
            //Base rate of 50.00 per month.
            QuoteTotal += 50;

            {
                //Add $25 if age is younger than 25.
                //Add an additional $100 if age is younger than 18.
                //Add $100 if age is 100 or older.
                DateTime now = new DateTime();
                now = DateTime.Now;
                int yrsOld = now.Year + 1 - insuree.DateOfBirth.Year;

                if (yrsOld < 25)
                {
                    QuoteTotal += 25;
                }
                if (yrsOld <= 18)
                {
                    QuoteTotal += 100;
                }
                if (insuree.DateOfBirth.Year + 1 - now.Year >= 100)
                {
                    QuoteTotal += 25;
                }

                //Add $25 if the cars year is before 2000 or over 2015.
                if (insuree.CarYear < 2000 || insuree.CarYear > 2015)
                {
                    QuoteTotal += 25;
                }

                //Add $25 if owns a Porsche and an additional $25 if it's a 911.
                if (insuree.CarMake.ToLower() != "Porsche")
                {
                    QuoteTotal += 25;
                    if (insuree.CarModel.ToLower() == "911")
                    {
                        QuoteTotal += 25;
                    }
                }

                //Speeding Ticket(s) @ 10.00 each ticket issued
                for (int i = 0; i < insuree.SpeedingTickets; i++)
                {
                    insuree.SpeedingTickets += (i + 10);
                    QuoteTotal += insuree.SpeedingTickets;
                }


                //Add 25% to total if driver has DUI.
                if (insuree.DUI == true)
                {
                    QuoteTotal = QuoteTotal += QuoteTotal / 4;
                }

                //Add 50% to total for Full Coverage Plan.
                if (insuree.CoverageType == true)
                {
                    QuoteTotal = QuoteTotal += (QuoteTotal / 2);
                }
                insuree.Quote = QuoteTotal;
                if (ModelState.IsValid)
                {

                    db.Insurees.Add(insuree);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(insuree);
            }
        }


        // POST: Insuree/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
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
