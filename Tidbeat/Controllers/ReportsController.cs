using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Composition;
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
using Tidbeat.DTOs.Reports;
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

        // GET: Reports
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> Index([FromQuery] string name, [FromQuery] string reason, [FromQuery] string type, [FromQuery] string state, [FromQuery] string sort)
        {
            if (_context.Report != null)
            {
                var result = await _context.Report.Include(p=>p.UserReported).ToListAsync();

                if (!string.IsNullOrEmpty(name)) 
                {
                    result = result.Where(p=>p.UserReported.FullName.Contains(name)).ToList();
                }
                switch (type?.ToLower())
                {
                    case "user":
                        result = result.Where(p => p.ReportItemType.Equals(ReportedItemType.User)).ToList();
                        break;
                    case "post":
                        result = result.Where(p => p.ReportItemType.Equals(ReportedItemType.Post)).ToList();
                        break;
                    case "comment":
                        result = result.Where(p => p.ReportItemType.Equals(ReportedItemType.Comment)).ToList();
                        break;
                    default:
                        break;
                }

                switch (state.ToLower())
                {
                    case "created":
                        result = result.Where(p => p.Status.Equals(ReportStatus.Created)).ToList();
                        break;
                    case "open":
                        result = result.Where(p => p.Status.Equals(ReportStatus.Open)).ToList();
                        break;
                    case "close":
                        result = result.Where(p => p.Status.Equals(ReportStatus.Closed)).ToList();
                        break;
                    default:
                        break;
                }

                switch (reason.ToLower())
                {
                    case "gore":
                        result = result.Where(p => p.Reason.Equals(ReportReason.GoreContent)).ToList();
                        break;
                    case "hate":
                        result = result.Where(p => p.Reason.Equals(ReportReason.HateSpeech)).ToList();
                        break;
                    case "other":
                        result = result.Where(p => p.Reason.Equals(ReportReason.Other)).ToList();
                        break;
                    case "sexual":
                        result = result.Where(p => p.Reason.Equals(ReportReason.SexualContent)).ToList();
                        break;
                    case "innappropriate":
                        result = result.Where(p => p.Reason.Equals(ReportReason.InnappropriateBehaviour)).ToList();
                        break;
                    default:
                        break;
                }

                switch (sort.ToLower())
                {
                    case "new":
                        result = result.OrderByDescending(p => p.Date).ToList();
                        break;
                    case "old":
                        result = result.OrderBy(p=>p.Date).ToList();
                        break;
                    default:
                        break;
                }
                return View(result);
            }
            return NotFound();
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
                var report = await _context.Report.Include(r => r.UserReported).Include(r => r.UserReporter)
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
                            ViewBag.Post = post;
                        }
                        else 
                        {
                            return NotFound();
                        }
                        break;
                    case ReportedItemType.Comment:
                        var comment = await _context.Comment.Include(p=>p.User).Include(p => p.post).Where(p => p.CommentId.ToString().Equals(report.ReportItemId)).FirstOrDefaultAsync();
                        if (comment != null) {
                            ViewBag.Comment = comment;
                        }
                        else 
                        {
                            return NotFound();
                        }
                        break;
                    case ReportedItemType.User:
                        var userReportedAccount = await _context.Users.Where(p => p.Id.ToString().Equals(report.ReportItemId)).FirstOrDefaultAsync();
                        if (userReportedAccount != null) {
                            ViewBag.UserReported = userReportedAccount;
                        }
                        else 
                        {
                            return NotFound();
                        }
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
        public async Task<IActionResult> Create([Bind("Reason,DetailedReason,ReportItemId,ReportItemType,UserReported")] Report report)
        {
            if (report.Reason != null && !string.IsNullOrEmpty(report.ReportItemId)) {
                if (!Enum.IsDefined(typeof(ReportReason), report.Reason))
                {
                    ModelState.AddModelError("ReportReason", "Invalid value for ReportReason.");
                    return Json("Error");

                }
                if (!Enum.IsDefined(typeof(ReportedItemType), report.ReportItemType))
                {
                    ModelState.AddModelError("ReportItemType", "Invalid value for ReportItemType.");
                    return Json("Error");

                }

                var currentUser = await _userManager.GetUserAsync(User);
                report.UserReporter = currentUser;

                if (report.ReportItemType.Equals(ReportedItemType.Post)) {
                    var post = await _context.Posts.Include(p => p.User).Where(p => p.PostId.ToString().Equals(report.ReportItemId)).FirstOrDefaultAsync();
                    if (post == null) {
                        ModelState.AddModelError("ReportItemId", "Invalid value for ReportItemId.");
                        return Json("Error");
                    }
                    report.UserReported = post.User;
                }
                else if (report.ReportItemType.Equals(ReportedItemType.Comment)) {
                    var comment = await _context.Comment.Include(c => c.User).Where(p => p.CommentId.ToString().Equals(report.ReportItemId)).FirstOrDefaultAsync();
                    if (comment == null) {
                        ModelState.AddModelError("ReportItemId", "Invalid value for ReportItemId.");
                        return Json("Error");
                    }
                    report.UserReported = comment.User;
                }
                else if (report.ReportItemType.Equals(ReportedItemType.User)) {
                    var user = await _userManager.FindByIdAsync(report.ReportItemId);
                    //var user = await _context.Users.Where(p => p.Id.ToString().Equals(report.ReportItemId)).FirstOrDefaultAsync();
                    if (user == null) {
                        ModelState.AddModelError("ReportItemId", "Invalid value for ReportItemId.");
                        return Json("Error");
                    }
                    report.UserReported = user;
                }
                else {
                    ModelState.AddModelError("ReportItemType", "Invalid value for ReportItemType.");
                    return Json("Error");
                }
                var context = new ValidationContext(report);
                var results = new List<ValidationResult>();
                var isModelValid = Validator.TryValidateObject(report, context, results, true);

                if (isModelValid)
                {
                    var sanitizer = new HtmlSanitizer();
                    var sanitizedContent = sanitizer.Sanitize(report?.DetailedReason);
                    var user = await _userManager.GetUserAsync(User);
                    if (User?.Identity.IsAuthenticated == true)
                    {
                        report.Id = Guid.NewGuid();
                        report.UserReporter = user;
                        report.Date = DateTime.Now;
                        report.Status = ReportStatus.Created;
                        report.DetailedReason = sanitizedContent;
                        _context.Add(report);
                        await _context.SaveChangesAsync();
                        return Json("Sucess");
                    }
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

        [HttpPost]
        public async Task<IActionResult> SetReportStatus([FromBody] SetReportStatusDTO setReportStatusDTO) {
             var reportId = setReportStatusDTO.ReportId;
             var status = setReportStatusDTO.Status;
             if (reportId != null) {
                var user = await _userManager.GetUserAsync(User);
                if (User?.Identity.IsAuthenticated == true) {
                    try {
                        var reportSaved = await _context.Report.Where(r => r.Id.ToString().Equals(reportId)).FirstOrDefaultAsync();
                        if (reportSaved == null) {
                            return NotFound();
                        }
                        if (status == Convert.ToInt32(ReportStatus.Open)) {
                            reportSaved.Status = ReportStatus.Open;
                        } else if (status == Convert.ToInt32(ReportStatus.Closed)){
                            reportSaved.Status = ReportStatus.Closed;
                        } else {
                            reportSaved.Status = ReportStatus.Created;
                        }
                        
                        //reportSaved.ModAssigned = user;
                        //_context.Update(reportSaved.Status);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException) {
                        if (!ReportExists(Guid.Parse(reportId))) {
                            return NotFound();
                        }
                        else {
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
