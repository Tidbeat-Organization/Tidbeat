using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
                    await _context.SaveChangesAsync();
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
        // Missing Checks
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Reason,DetailedReason,ReportItemId,ReportItemType")] Report report)
        {
            if (ModelState.IsValid)
            {
                var sanitizer = new HtmlSanitizer();
                var sanitizedContent = sanitizer.Sanitize(report?.DetailedReason);
                if (string.IsNullOrEmpty(sanitizedContent))
                {
                    ModelState.AddModelError(string.Empty, "error_content");
                    return Json("Error");
                }
                var user = await _userManager.GetUserAsync(User);
                if (User?.Identity.IsAuthenticated == true)
                {
                    report.Id = Guid.NewGuid();
                    report.UserReporter = user;
                    report.Date = DateTime.Now;
                    report.Status = ReportStatus.Created;
                    _context.Add(report);
                    await _context.SaveChangesAsync();
                    return Json("Sucess");
                }
            }
            return Json("Error");
        }


        // POST: Reports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Status")] Report report)
        {
            if (id != report.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (User?.Identity.IsAuthenticated == true)
                {
                    try
                    {
                        var reportSaved = await _context.Report.Where(r => r.Id.Equals(id)).FirstOrDefaultAsync();
                        if (reportSaved == null)
                        {
                            return NotFound();
                        }
                        reportSaved.Status = report.Status;
                        reportSaved.ModAssigned = user;
                        _context.Update(reportSaved.Status);
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
                    return Json("Sucess");
                }
            }
            return NotFound();
        }

        private bool ReportExists(Guid id)
        {
          return (_context.Report?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
