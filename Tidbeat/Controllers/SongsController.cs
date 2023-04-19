using System;
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
using Tidbeat.Services;

namespace Tidbeat.Controllers
{
    /// <summary>
    /// Controls favoriting songs and pages related to songs.
    /// </summary>
    public class SongsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISpotifyService _spotifyService;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes needed services for the controller.
        /// </summary>
        /// <param name="context">The context of the application.</param>
        /// <param name="spotifyService">The access to the spotify API.</param>
        /// <param name="userManager">The language localizer.</param>
        public SongsController(ApplicationDbContext context, ISpotifyService spotifyService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _spotifyService = spotifyService;
            _userManager = userManager;
        }
        private async Task<bool> AddSong(string songId)
        {
            if (_context.Songs.Any(s => s.SongId == songId))
            {
                return false;
            }
            
            var spotifySong = await _spotifyService.GetSongAsync(songId);
            var bandIfAvailable = await _context.Bands.FindAsync(spotifySong.Artists[0].Id);
            if (bandIfAvailable == null)
            {
                var spotifyBand = await _spotifyService.GetBandAsync(spotifySong.Artists[0].Id);
                bandIfAvailable = new Band() {
                    BandId = spotifyBand.Id,
                    Image = spotifyBand.Images[0].Url,
                    Name = spotifyBand.Name,
                    Gener = string.Join(',', spotifyBand.Genres.Where(s => s.Length > 1))
            };
                _context.Bands.Add(bandIfAvailable);
                _context.SaveChanges();
            }
            var songGener = await _spotifyService.GetGenresOfSong(songId);
            var song = new Song()
            {
                SongId = songId,
                Band = bandIfAvailable,
                Name = spotifySong.Name,
                Gener = string.Join(',', songGener.Where(s => s.Length > 1))
            };

            _context.Songs.Add(song);
            _context.SaveChanges();

            return true;
        }

        [Authorize]
         

        /// <summary>
        /// The action for adding the song to the user's favorite in the database.
        /// </summary>
        /// <param name="user">The user which is favoriting the song.</param>
        /// <param name="songId">The id of the song about to be favorited.</param>
        /// <returns></returns>
        public async Task AddAsFavoriteAsync(ApplicationUser user, string songId)
        {
            var songIds = user.DeserializeFavoriteSongIds();
            if (songIds.Contains(songId))
            {
                return;
            }
            await AddSong(songId);
            songIds.Add(songId);
            user.SerializeFavoriteSongIds(songIds);
            
            await _userManager.UpdateAsync(user);
            
        }

        /// <summary>
        /// The action for removing the song from the user's favorite in the database.
        /// </summary>
        /// <param name="user">The user which is removing the song from favorites.</param>
        /// <param name="songId">The id of the song about to be removed from favorites.</param>
        /// <returns></returns>
        [Authorize]
        public async Task<bool> RemoveAsFavoriteAsync(ApplicationUser user, string songId)
        {
            var songIds = user.DeserializeFavoriteSongIds();
            var success = songIds.Remove(songId);
            user.SerializeFavoriteSongIds(songIds);
            await _userManager.UpdateAsync(user);
            return success;
        }

        // GET: Songs
        /// <summary>
        /// The action for getting all the songs based on filters in the spotify API.
        /// </summary>
        /// <param name="searchKey">The text to search with.</param>
        /// <param name="gener">The gender of the song.</param>
        /// <param name="band">The band responsible for the song.</param>
        /// <param name="album">The album which the song belongs to.</param>
        /// <param name="yearStart">The start year of the interval from which the song was released.</param>
        /// <param name="yearEnd">The last year of the interval from which the song was released.</param>
        /// <remarks>GET: Songs/</remarks>
        /// <returns>The Index view with the found songs according to its filters included.</returns>
        public async Task<IActionResult> Index([FromQuery] string searchKey, [FromQuery] string gener, [FromQuery] string band, [FromQuery] string album, [FromQuery] string yearStart, [FromQuery] string yearEnd) {

            var currentuser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentuser;

            if (User != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var request = HttpContext.Request;
                var currentUrl = string.Format("{0}://{1}", request.Scheme, request.Host);
                TempData["Friends"] = await UtilityClass.SideBarAsync(user.Id, currentUrl);
            }

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
        /// <summary>
        /// The details of a single song.
        /// </summary>
        /// <param name="id">The id of the song.</param>
        /// <remarks>GET: Songs/Details/{id}</remarks>
        /// <returns>The Details view of the song.</returns>
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
            if (loggedUser != null && loggedUser.FavoriteSongIds != null)
            {
                isFavorited = loggedUser.FavoriteSongIds.Contains(song.Id);
            }
            else
            {
                isFavorited = false;
            }
            ViewBag.isFavorited = isFavorited;

            var users = await _userManager.Users.ToListAsync();
            var count = users.Count(u => string.IsNullOrEmpty(u.FavoriteSongIds) ? false : u.FavoriteSongIds.Contains(id));
            ViewBag.favoritesAmount = count;

            var allPosts = _context.Posts.Include(p => p.User).Include(p => p.Song).Where(p => p.Song != null && p.Song.SongId == id).ToList();
            ViewBag.posts = allPosts;

            var currentuser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentuser;

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var request = HttpContext.Request;
                var currentUrl = string.Format("{0}://{1}", request.Scheme, request.Host);
                TempData["Friends"] = await UtilityClass.SideBarAsync(user.Id, currentUrl);
            }

            return View(song);
        }
        
        /// <summary>
        /// The action used to set the favorite song of the user.
        /// </summary>
        /// <param name="songId">The song id about to be favorited.</param>
        /// <remarks>GET: Songs/SetFavorite?songId={id}</remarks>
        /// <returns></returns>
        [Authorize]
        public async Task SetFavorite([FromQuery] string? songId)
        {
            Console.WriteLine($"\nId: {songId}");
            var loggedUser = await _userManager.GetUserAsync(User);

            if (loggedUser == null)
            {
                return;
            }

            if (songId == null || songId == "null")
            {
                // loggedUser.FavoriteSongId = null;
                // await RemoveAsFavoriteAsync(loggedUser, songId);
                await _userManager.UpdateAsync(loggedUser);
            }
            else
            {
                var song = await _spotifyService.GetSongAsync(songId);
                Console.WriteLine($"Song: {song.Name}");
                if (song == null)
                {
                    return;
                }
                else
                {
                    // loggedUser.FavoriteSongId = song.Id;
                    await AddAsFavoriteAsync(loggedUser, song.Id);
                    Console.WriteLine($"Favorite songs now:\n");
                    foreach (var songString in loggedUser.DeserializeFavoriteSongIds())
                    {
                        Console.WriteLine($"__ {songString}");
                    }
                    await _userManager.UpdateAsync(loggedUser);
                }
            }
        }

        /// <summary>
        /// The action for getting the favorite count of a song.
        /// </summary>
        /// <param name="songId">The song id.</param>
        /// <remarks>GET: Songs/GetFavoriteCount?songId={id}</remarks>
        /// <returns>Favorite count of a song.</returns>
        public async Task<int> GetFavoriteCount([FromQuery] string songId)
        {
            var users = await _userManager.Users.ToListAsync();
            var count = users.Count(u => u.FavoriteSongIds.Contains(songId));
            return count;
        }
    }
}
