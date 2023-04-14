using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tidbeat.Data;
using Tidbeat.Enums;
using Tidbeat.Models;

namespace Tidbeat.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Add filtres
        // GET: Reports
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> Index()
        {
              return _context.Report != null ? 
                          View(await _context.Report.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Report'  is null.");
        }

        // GET: Reports/Details/5
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Report == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (User?.Identity.IsAuthenticated == true)
            {
                var report = await _context.Report
                .FirstOrDefaultAsync(m => m.Id == id);
                if (report == null)
                {
                    return NotFound();
                }
                if (report.Status.Equals(ReportStatus.Created))
                {
                    report.ModAssigned = user;
                    report.Status = ReportStatus.Open;
                    _context.Report.Update(report);
                }
                switch (report.ReportItemType)
                {
                    case ReportedItemType.Post:
                        var post = await _context.Posts.Include(p => p.User).Where(p => p.PostId.ToString().Equals(report.ReportItemId)).FirstOrDefaultAsync();
                        if (post != null)
                        {
                            TempData["Post"] = post;
                        }
                        else 
                        {
                            return NotFound();
                        }
                        break;
                    case ReportedItemType.Comment:
                        var comment = await _context.Comment.Include(p=>p.User).Where(p => p.CommentId.ToString().Equals(report.ReportItemId)).FirstOrDefaultAsync();
                        if (comment != null) {
                            TempData["Comment"] = comment;
                        }
                        else 
                        {
                            return NotFound();
                        }
                        break;
                    case ReportedItemType.User:
                        var userReportedAccount = await _context.Users.Where(p => p.Id.ToString().Equals(report.ReportItemId)).FirstOrDefaultAsync();
                        if (userReportedAccount != null) {
                            TempData["UserReported"] = userReportedAccount;
                        }
                        else 
                        {
                            return NotFound();
                        }
                        break;
                    default:
                        return NotFound();
                        break;
                }
                return View(report); 
            }
            return NotFound();
        }

        // POST: Reports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Reason,DetailedReason,ReportItemId,ReportItemType,Status,Date")] Report report)
        {
            if (ModelState.IsValid)
            {
                report.Id = Guid.NewGuid();
                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(report);
        }


        // POST: Reports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Reason,DetailedReason,ReportItemId,ReportItemType,Status,Date")] Report report)
        {
            if (id != report.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(report);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportExists(report.Id))
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
            return View(report);
        }

        // GET: Reports/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Report == null)
            {
                return NotFound();
            }

            var report = await _context.Report
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Report == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Report'  is null.");
            }
            var report = await _context.Report.FindAsync(id);
            if (report != null)
            {
                _context.Report.Remove(report);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportExists(Guid id)
        {
          return (_context.Report?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
