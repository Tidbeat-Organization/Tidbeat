using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tidbeat.Data;
using Tidbeat.Models;
using Tidbeat.Services;
namespace Tidbeat.Controllers
{
    public class MusicsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISpotifyService _spotifyService;

        public MusicsController(ApplicationDbContext context, ISpotifyService spotifyService)
        {
            _context = context;
            _spotifyService = spotifyService;
        }

        // GET: Musics
        public async Task<IActionResult> Index([FromQuery] string searchKey,[FromQuery] string gener, [FromQuery] string band, [FromQuery] string album, [FromQuery] string yearStart, [FromQuery] string yearEnd)
        {
                TempData["Search"] = searchKey;
                TempData["Gener"] = gener;
                TempData["Band"] = band;
                TempData["Album"] = album;
                TempData["YearStar"] = yearStart;
                TempData["YearEnd"] = yearEnd;
            if ((string.IsNullOrEmpty(gener) || gener=="/") && (string.IsNullOrEmpty(band) || band == "/" )&& (string.IsNullOrEmpty(album) || album == "/") && (string.IsNullOrEmpty(yearStart) || yearStart== "/") && (string.IsNullOrEmpty(yearEnd) || yearEnd == "/"))
            {
                ViewBag.Result = await _spotifyService.GetMultipleSongsAsync(searchKey);
                return View();
                 
            }
            ViewBag.Result = await _spotifyService.GetSearchSongsbyValuesAsync(searchKey,gener, band, album, yearStart, yearEnd);
            return View();

        }
    }
}
