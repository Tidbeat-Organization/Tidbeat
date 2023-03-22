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

namespace Tidbeat.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfilesController(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        private async Task<List<Song>> GetFavoriteSongsAsync(ApplicationUser user)
        {
            var songIds = user.DeserializeFavoriteSongIds();
            var songs = await _context.Songs.Where(s => songIds.Contains(s.SongId)).ToListAsync();
            return songs;
        }

        private async Task<bool> RemoveSingleSongAsync(ApplicationUser user, string songId)
        {
            var songIds = user.DeserializeFavoriteSongIds();
            bool success = songIds.Remove(songId);
            user.SerializeFavoriteSongIds(songIds);
            await _userManager.UpdateAsync(user);

            return success;
        }
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var currentuser = await _userManager.GetUserAsync(User);
            var profile = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null)
            {
                return NotFound();
            }
            if (currentuser != null)
            {
                if (profile.Id == currentuser.Id)
                {
                    TempData["user"] = true;
                }
            }
            ViewBag.Posts = _context.Posts.Include(p => p.User).Where(p => p.User.Id == profile.Id).ToList();
            ViewBag.FavoriteSongs = await GetFavoriteSongsAsync(profile);
            return View(profile);
        }

        // PUT to /Profiles/RemoveFavoriteSong
        [HttpPut]
        public async Task<IActionResult> RemoveFavoriteSong(string songId)
        {
            var user = await _userManager.GetUserAsync(User);
            var success = await RemoveSingleSongAsync(user, songId);
            return success ? Ok() : NotFound("Could not remove song from favorites");
        }


        private bool ProfileExists(int id)
        {
          return (_context.Profile?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
