using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            var insurees = db.Insurees.ToList();

            foreach (var item in insurees)
            {
                item.Quote = CalculateQuote(item);
            }

            return View(insurees);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI_,SpeedingTIckets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                // Assign the calculated quote value
                insuree.Quote = CalculateQuote(insuree);
                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI_,SpeedingTIckets,CoverageType,Quote")] Insuree insuree)
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

        public decimal CalculateQuote(Insuree insuree)
        {
            decimal monthlyTotal = 50; // Base monthly total

            // Age calculations
            DateTime now = DateTime.Today;
            DateTime dateOfBirth;
            if (DateTime.TryParse(insuree.DateOfBirth, out dateOfBirth))
            {
                int age = now.Year - dateOfBirth.Year;
                if (now < dateOfBirth.AddYears(age))
                {
                    age--; // Decrease age if the birthday hasn't occurred yet this year
                }

                if (age <= 18)
                {
                    monthlyTotal += 100;
                }
                else if (age >= 19 && age <= 25)
                {
                    monthlyTotal += 50;
                }
                else
                {
                    monthlyTotal += 25;
                }
            }

            // Car year calculations
            if (insuree.CarYear < 2000)
            {
                monthlyTotal += 25;
            }
            else if (insuree.CarYear > 2015)
            {
                monthlyTotal += 25;
            }

            // Car make and model calculations
            if (insuree.CarMake == "Porsche")
            {
                monthlyTotal += 25;

                if (insuree.CarModel == "911 Carrera")
                {
                    monthlyTotal += 25;
                }
            }

            // Speeding ticket calculations
            monthlyTotal += insuree.SpeedingTIckets * 10;

            // DUI calculations
            if (insuree.DUI_)
            {
                monthlyTotal += monthlyTotal * 0.25m;
            }

            // Full coverage calculations
            if (insuree.CoverageType)
            {
                monthlyTotal += monthlyTotal * 0.5m;
            }

            return monthlyTotal;
        }



        public ActionResult Admin()
        {
            var insurees = db.Insurees.ToList();

            foreach (var item in insurees)
            {
                item.Quote = CalculateQuote(item);
            }

            return View(insurees);
        }




    }
}
