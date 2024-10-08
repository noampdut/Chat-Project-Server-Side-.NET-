﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ex2.Data;
using ex2.Models;
using ex2.Services;

namespace ex2.Controllers
{
    public class RatesController : Controller
    {
        private IRateService rateService;
        private IUsersService userService;
        private float currentRate;
        //private User activeUser;

        public RatesController(IUsersService usersService, IRateService ratesService) {
            rateService = ratesService;
            //userService = usersService;
            //userService.Get("admin");
        }

        // GET: Rates
        public IActionResult Index() {
            
            List<Rate> rates = rateService.GetAll();
            float rate = 0;
            int numOfRates = rates.Count;

            for (int i = 0; i < numOfRates; i++)
            {
                rate += rates[i].Score;
            }
            if (numOfRates == 0)
            {
                rate = 0;
            }
            else
            {
                rate = rate / numOfRates;
            }
            ViewData["Message"] = rate;
            currentRate = rate;
            return View(rates);
        }
        [HttpPost]
        public IActionResult Index(string query)
        {
            List<Rate> rates = rateService.GetAll();
            if (rates == null)
            {
                ViewData["Message"] = currentRate;
                return View(rates);
            }
            List<Rate> filterRates = new List<Rate>();
            for (int i = 0; i < rates.Count; i++)
            {
                if (rates[i].Text.Contains(query))
                {
                    filterRates.Add(rates[i]);
                }
            }
            ViewData["Message"] = currentRate;
            return View(filterRates);
        }

        // GET: Rates/Details/5
        public IActionResult Details(int id)
        {
            return View(rateService.Get(id));
        }

        // GET: Rates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int score, string text)
        {
            rateService.Add(text, score, "admin");
            return RedirectToAction(nameof(Index));
        }

        // GET: Rates/Edit/5
        public IActionResult Edit(int id)
        {
            Rate rate = rateService.Get(id);

            return View(rateService.Get(id));
        }

        // POST: Rates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Score,Text")] Rate rate)
        {
            Rate rate1 = rateService.Get(id);
            if (ModelState.IsValid)
            {
                try
                {
                    rate1.Score = rate.Score;
                    rate1.Text = rate.Text;
                    rateService.Edit(rate1);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RateExists(rate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(rate);
        }

        // GET: Rates/Delete/5
        public IActionResult Delete(int id)
        {
            Rate rate = rateService.Get(id);
            return View(rateService.Get(id));
        }

        // POST: Rates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Rate rate = rateService.Get(id);
            rateService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private bool RateExists(int id)
        {
            Rate rate = rateService.Get(id);
            if (rate != null)
            {
                return true;
            }
            return false;
        }
    }
}
