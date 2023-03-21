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
    public class SongsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISpotifyService _spotifyService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SongsController(ApplicationDbContext context, ISpotifyService spotifyService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _spotifyService = spotifyService;
            _userManager = userManager;
        }

        private async Task AddAsFavoriteAsync(ApplicationUser user, string songId)
        {
            var songIds = user.DeserializeFavoriteSongIds();
            if (songIds.Contains(songId))
            {
                return;
            }
            songIds.Add(songId);
            user.SerializeFavoriteSongIds(songIds);
            await _userManager.UpdateAsync(user);
        }

        private async Task<bool> RemoveAsFavoriteAsync(ApplicationUser user, string songId)
        {
            var songIds = user.DeserializeFavoriteSongIds();
            var success = songIds.Remove(songId);
            user.SerializeFavoriteSongIds(songIds);
            await _userManager.UpdateAsync(user);
            return success;
        }

        // GET: Songs
        public async Task<IActionResult> Index([FromQuery] string searchKey, [FromQuery] string gener, [FromQuery] string band, [FromQuery] string album, [FromQuery] string yearStart, [FromQuery] string yearEnd) {
            TempData["Search"] = searchKey;
            TempData["Gener"] = gener;
            TempData["Band"] = band;
            TempData["Album"] = album;
            TempData["YearStar"] = yearStart;
            TempData["YearEnd"] = yearEnd;
            if ((string.IsNullOrEmpty(gener) || gener == "/") && (string.IsNullOrEmpty(band) || band == "/") && (string.IsNullOrEmpty(album) || album == "/") && (string.IsNullOrEmpty(yearStart) || yearStart == "/") && (string.IsNullOrEmpty(yearEnd) || yearEnd == "/")) {
                ViewBag.Result = await _spotifyService.GetMultipleSongsAsync(searchKey);
                return View();

            }
            ViewBag.Result = await _spotifyService.GetSearchSongsbyValuesAsync(searchKey, gener, band, album, yearStart, yearEnd);
            return View();

        }

        // GET: Songs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _spotifyService.GetSongAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            var loggedUser = await _userManager.GetUserAsync(User);
            bool isFavorited;
            if (loggedUser != null) {
                isFavorited = loggedUser.FavoriteSongIds.Contains(song.Id);
            }
            else {
                isFavorited = false;
            }
            ViewBag.isFavorited = isFavorited;

            var users = await _userManager.Users.ToListAsync();
            var count = users.Count(u => u.FavoriteSongIds.Contains(id));
            ViewBag.favoritesAmount = count;

            var allPosts = _context.Posts.Include(p => p.User).Include(p => p.Song).Where(p => p.Song != null && p.Song.SongId == id).ToList();
            ViewBag.posts = allPosts;

            return View(song);
        }

        public async Task SetFavorite([FromQuery] string? songId) {
            var loggedUser = await _userManager.GetUserAsync(User);

            if (loggedUser == null) {
                return;
            }

            if (songId == null || songId == "null") {
                // loggedUser.FavoriteSongId = null;
                // await RemoveAsFavoriteAsync(loggedUser, songId);
                await _userManager.UpdateAsync(loggedUser);
            } else {
                var song = await _spotifyService.GetSongAsync(songId);
                if (song == null) {
                    return;
                } else {
                    // loggedUser.FavoriteSongId = song.Id;
                    await AddAsFavoriteAsync(loggedUser, song.Id);
                    await _userManager.UpdateAsync(loggedUser);
                }
            }
        }

        public async Task<int> GetFavoriteCount([FromQuery] string songId) {
            var users = await _userManager.Users.ToListAsync();
            var count = users.Count(u => u.FavoriteSongIds.Contains(songId));
            return count;
        }
    }
}
