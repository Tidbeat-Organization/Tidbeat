﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tidbeat.Data;
using Tidbeat.Models;

namespace Tidbeat.Controllers
{
    /// <summary>
    /// Controls all profiles.
    /// </summary>
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        //private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes needed services for the controller.
        /// </summary>
        /// <param name="context">The context of the application.</param>
        /// <param name="userManager">The language localizer.</param>
        public ProfilesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            //_serviceProvider = serviceProvider;
            _userManager = userManager;
        }

        /// <summary>
        /// Returns user's favorite songs.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <returns>A list of favorite songs.</returns>
        public async Task<List<Song>> GetFavoriteSongsAsync(ApplicationUser user)
        {
            var songIds = user.DeserializeFavoriteSongIds();
            // Print all song Ids
            Console.WriteLine("\nList of favorite song IDs (async):");
            foreach (var songString in songIds)
                Console.WriteLine($"| {songString}");
            var songs = await _context.Songs.ToListAsync();
            var favoriteSongs = songs.Where(s => songIds.Contains(s.SongId)).ToList();
            // Print all songs
            Console.WriteLine("Favorite songs (async):");
            Console.WriteLine($"Song count (async): {favoriteSongs.Count}");
            foreach (var song in favoriteSongs)
                Console.WriteLine($"| {song.Name} by {song.Band}");
            return favoriteSongs;
        }

        /// <summary>
        /// Returns a band for each song passed.
        /// </summary>
        /// <param name="songs">A list of songs.</param>
        /// <returns>A list of bands.</returns>
        public async Task<List<Band>> GetBandsOfSongs(List<Song> songs)
        {
            var bands = await _context.Songs.Select(s => s.Band).ToListAsync();
            return bands;
        }
        
        /// <summary>
        /// Removes a single song from the favorites.
        /// </summary>
        /// <param name="user">The user to remove the song from.</param>
        /// <param name="songId">The id of the song about to be removed.</param>
        /// <returns>The Profile view of the user.</returns>
        [Authorize]
        public async Task<bool> RemoveSingleSongAsync(ApplicationUser user, string songId)
        {
            var songIds = user.DeserializeFavoriteSongIds();
            bool success = songIds.Remove(songId);
            user.SerializeFavoriteSongIds(songIds);
            await _userManager.UpdateAsync(user);

            return success;
        }
        
        /// <summary>
        /// Removes a single song from the favorites.
        /// </summary>
        /// <param name="id">The song id about to be removed.</param>
        /// <returns>The Profile view of the user.</returns>
        [Authorize]
        public async Task<IActionResult> RemoveFavorite(string? id)
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
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            await RemoveSingleSongAsync(profile, song.SongId);
            // return RedirectToAction(nameof(Details), new { id = profile.Id });
            return View(profile);
        }

        /// <summary>
        /// Returns the details of a profile.
        /// </summary>
        /// <param name="id">The id of the profile.</param>
        /// <returns>The details view of the profile.</returns>
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var currentuser = await _userManager.GetUserAsync(User);
            var profile = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null || profile.Email == Configurations.InvalidUser.Email)
            {
                return NotFound();
            }
            if (currentuser != null)
            {
                if (profile.Id.Equals(currentuser.Id))
                {
                    TempData["user"] = true;
                }
            }
            
            ViewBag.CurrentUser = currentuser;
            ViewBag.Posts = await _context.Posts.Include(p => p.User).Where(p => p.User.Id == profile.Id).ToListAsync();
            ViewBag.FavoriteSongs = await GetFavoriteSongsAsync(profile);
            ViewBag.BandsOfSongs = await GetBandsOfSongs(ViewBag.FavoriteSongs);
            ViewBag.IsCurrentUser = profile.Id == currentuser?.Id;
            // Print bands of songs.
            /*
            Console.WriteLine("Bands of songs:");
            var bands = await GetBandsOfSongs(ViewBag.FavoriteSongs);
            foreach (Band band in bands)
                Console.WriteLine($"| {band.Name}");
            */
            var testFS = await GetFavoriteSongsAsync(profile);
            Console.WriteLine("\nList of favorite song IDs (Details):");
            foreach (var songString in profile.DeserializeFavoriteSongIds())
            {
                Console.WriteLine($"| {songString}");
            }
            Console.WriteLine("Favorite songs (Details):");
            Console.WriteLine($"  object: {testFS}");
            foreach (var song in testFS)
            {
                Console.WriteLine($"-- {song.Name} by {song.Band.Name}");
            }
            return View(profile);
        }

        /// <summary>
        /// Returns the edit view of a profile.
        /// </summary>
        /// <param name="id">The id of the profile.</param>
        /// <remarks>PUT: Profiles/RemoveFavoriteSong</remarks>
        /// <returns>The edit view of the profile.</returns>
        // PUT to /Profiles/RemoveFavoriteSong
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> RemoveFavoriteSong([FromQuery] string songId)
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
