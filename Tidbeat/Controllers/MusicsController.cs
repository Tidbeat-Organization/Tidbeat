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
        public async Task<IActionResult> Index([FromQuery] string gener, [FromQuery] string band, [FromQuery] string album, [FromQuery] string yearStart, [FromQuery] string yearEnd)
        {
            if (string.IsNullOrEmpty(gener) && string.IsNullOrEmpty(band) && string.IsNullOrEmpty(album) && string.IsNullOrEmpty(yearStart) && string.IsNullOrEmpty(yearEnd))
            {
                ViewBag.Result = await _spotifyService.GetMultipleSongsAsync();
                return View();
                 
            }
            ViewBag.Result = await _spotifyService.GetSearchSongsbyValuesAsync(gener, band, album, yearStart, yearEnd);
            return View();

        }
    }
}
