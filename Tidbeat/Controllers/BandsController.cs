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
    public class BandsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISpotifyService _spotifyService;

        public BandsController(ApplicationDbContext context, ISpotifyService spotifyService)
        {
            _context = context;
            _spotifyService = spotifyService;
        }

        // GET: Bands
        public async Task<IActionResult> Index([FromQuery] string searchKey, [FromQuery] string gener, [FromQuery] string order)
        {
            TempData["Search"] = searchKey;
            TempData["Gener"] = gener;
            TempData["Order"] = order;
            if ((string.IsNullOrEmpty(gener) || gener == "/"))
            {
                ViewBag.Result = await _spotifyService.GetMultipleBandsAsync(searchKey);
                return View();

            }
            var results = await _spotifyService.GetSearchBandsbyValuesAsync(searchKey, gener);
            results.Artists.Items.Sort((n1, n2) => n1.Name.CompareTo(n2.Name));//Alfabeticamnete a-z
            results.Artists.Items.Reverse();//Alfabeticamnete z-a
            results.Artists.Items.Sort((n1, n2) => n1.Popularity.CompareTo(n2.Popularity));//popularidade menos-mais
            results.Artists.Items.Reverse();//popularidade mais-menos
            ViewBag.Result = results;
            return View();

        }

        // GET: Bands/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var band = await _spotifyService.GetBandAsync(id);
            if (band == null)
            {
                return NotFound();
            }

            return View(band);
        }
    }
}
