using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using X.PagedList;

namespace ShopApp.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class LogController : Controller
    {
        private readonly ShopAppAspWebContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly INotyfService _toastNotification;

        public LogController(ShopAppAspWebContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, INotyfService notyfService)
        {
            _context = context;
            _environment = environment;
            _toastNotification = notyfService;
        }

        // GET: Admin/Log
        public async Task<IActionResult> Index(string? name, string? sort, int page = 1)
        {
            int limit = 10;
            page = page <= 1 ? 1 : page;
            ViewBag.names = name;
            ViewBag.sorts = sort;

            var logs = await _context.Logs.Where(x => x.TimeLogin == null || x.TimeActionRequest != null).OrderByDescending(x => x.LogId).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                logs = await _context.Logs.Where(x => x.UserName.Contains(name) || x.WorkTation.Contains(name)).OrderByDescending(x => x.LogId).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;
                switch (sort)
                {
                    case "Id-ASC":
                        logs = await _context.Logs.OrderBy(x => x.LogId).ToListAsync();
                        break;
                    case "Id-DESC":
                        logs = await _context.Logs.OrderByDescending(x => x.LogId).ToListAsync();
                        break;

                    case "Name-ASC":
                        logs = await _context.Logs.OrderBy(x => x.LogId).ToListAsync();
                        break;
                    case "Name-DESC":
                        logs = await _context.Logs.OrderByDescending(x => x.LogId).ToListAsync();
                        break;

                    case "Date-ASC":
                        logs = await _context.Logs.OrderBy(x => x.TimeLogin).ToListAsync();
                        break;
                    case "Date-DESC":
                        logs = await _context.Logs.OrderByDescending(x => x.TimeLogin).ToListAsync();
                        break;
                }

            }

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;
                switch (sort)
                {
                    case "Id-ASC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(name) || x.WorkTation.Contains(name)).OrderBy(x => x.LogId).ToListAsync();
                        break;
                    case "Id-DESC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(name) || x.WorkTation.Contains(name)).OrderByDescending(x => x.LogId).ToListAsync();
                        break;

                    case "Name-ASC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(name) || x.WorkTation.Contains(name)).OrderBy(x => x.LogId).ToListAsync();
                        break;
                    case "Name-DESC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(name) || x.WorkTation.Contains(name)).OrderByDescending(x => x.LogId).ToListAsync();
                        break;

                    case "Date-ASC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(name) || x.WorkTation.Contains(name)).OrderBy(x => x.TimeLogin).ToListAsync();
                        break;
                    case "Date-DESC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(name) || x.WorkTation.Contains(name)).OrderByDescending(x => x.TimeLogin).ToListAsync();
                        break;
                }
            }

            var pagedData = logs.ToPagedList(page, limit);
            return View(pagedData);
        }

        // GET: Admin/Log/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs
                .FirstOrDefaultAsync(m => m.LogId == id);
            if (log == null)
            {
                return NotFound();
            }

            return View(log);
        }

        // GET: Admin/Log/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Log/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LogId,UserName,WorkTation,Request,Response,IpAdress,TimeLogin,TimeLogout,TimeActionRequest")] Log log)
        {
            if (ModelState.IsValid)
            {
                _context.Add(log);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(log);
        }

        // GET: Admin/Log/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }
            return View(log);
        }

        // POST: Admin/Log/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LogId,UserName,WorkTation,Request,Response,IpAdress,TimeLogin,TimeLogout,TimeActionRequest")] Log log)
        {
            if (id != log.LogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(log);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LogExists(log.LogId))
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
            return View(log);
        }

        // GET: Admin/Log/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs
                .FirstOrDefaultAsync(m => m.LogId == id);
            if (log == null)
            {
                return NotFound();
            }

            return View(log);
        }

        // POST: Admin/Log/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var log = await _context.Logs.FindAsync(id);
            if (log != null)
            {
                _context.Logs.Remove(log);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LogExists(int id)
        {
            return _context.Logs.Any(e => e.LogId == id);
        }
    }
}
