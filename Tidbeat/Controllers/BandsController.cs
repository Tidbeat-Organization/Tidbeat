using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tidbeat.Data;
using Tidbeat.Models;
using Tidbeat.Services;

namespace Tidbeat.Controllers
{
    /// <summary>
    /// Controls all bands.
    /// </summary>
    public class BandsController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly ISpotifyService _spotifyService;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes needed services for the controller.
        /// </summary>
        /// <param name="context">The context of the application.</param>
        /// <param name="spotifyService">The access to the spotify API.</param>
        public BandsController(ApplicationDbContext context, ISpotifyService spotifyService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _spotifyService = spotifyService;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets all bands from the database.
        /// </summary>
        /// <param name="searchKey">The text to search with.</param>
        /// <param name="gener">The gender of the song.</param>
        /// <param name="order">The order which the info is shown.</param>
        /// <remarks>GET: Bands/</remarks>
        /// <returns>The view with the bands.</returns>
        // GET: Bands
        public async Task<IActionResult> Index([FromQuery] string searchKey, [FromQuery] string gener, [FromQuery] string order)
        {
            TempData["Search"] = searchKey;
            TempData["Gener"] = gener;
            TempData["Order"] = order;
            var results = await _spotifyService.GetSearchBandsbyValuesAsync(searchKey, gener);
            ViewBag.Result = results;
            if (order == "1") 
            {
                ViewBag.Result.Artists.Items = results.Artists.Items.OrderBy(n => n.Name).ToList();
            }
            if (order == "2")
            {
                ViewBag.Result.Artists.Items = results.Artists.Items.OrderByDescending(n => n.Name).ToList();
            }
            if (order == "3")
            {
                ViewBag.Result.Artists.Items = results.Artists.Items.OrderByDescending(n => n.Popularity).ToList();
            }
            if (order == "4")
            {
                ViewBag.Result.Artists.Items = results.Artists.Items.OrderBy(n => n.Popularity).ToList();

            }
            //Alfabeticamnete a-z
            var currentuser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentuser;

            if (User != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var request = HttpContext.Request;
                var currentUrl = string.Format("{0}://{1}", request.Scheme, request.Host);
                TempData["Friends"] = await UtilityClass.SideBarAsync(user.Id, currentUrl);
            }
            return View();

        }

        /// <summary>
        /// Gets the details of a band.
        /// </summary>
        /// <param name="id">The id of the band.</param>
        /// <remarks>GET: Bands/Details/5</remarks>
        /// <returns>The view with the details of the band.</returns>
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

            var albumAmount = await _spotifyService.GetAmountBandAlbumAsync(id);
            if (albumAmount == null) 
            {
                ViewBag.albumAmount = 0;
            }
            else 
            {
                ViewBag.albumAmount = albumAmount;
            }

            var top3Songs = await _spotifyService.GetTop3SongsAsync(id);
            ViewBag.top3Songs = top3Songs;

            var allPosts = _context.Posts.Include(p => p.User).Include(p => p.Band).Where(p => p.Band != null && p.Band.BandId == id).ToList();
            ViewBag.posts = allPosts;

            var currentuser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentuser;

            if (User != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var request = HttpContext.Request;
                var currentUrl = string.Format("{0}://{1}", request.Scheme, request.Host);
                TempData["Friends"] = await UtilityClass.SideBarAsync(user.Id, currentUrl);
            }
            return View(band);
        }
    }
}
