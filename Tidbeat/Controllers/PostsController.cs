using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ganss.Xss;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using Tidbeat.Data;
using Tidbeat.Models;
using Tidbeat.Services;

namespace Tidbeat.Controllers
{
    /// <summary>
    /// Controls the behaviour of the posts of the application.
    /// </summary>
    public class PostsController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISpotifyService _spotifyService;
        private readonly IStringLocalizer<PostsController> _localizer;

        /// <summary>
        /// Initializes needed services like the ApplicationDbContext, the UserManager, the SpotifyService and the StringLocalizer.
        /// </summary>
        /// <param name="context">Context of the website</param>
        /// <param name="userManager">The user manager</param>
        /// <param name="spotifyService">The access to the spotify API</param>
        /// <param name="localizer">The language localizer</param>
        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ISpotifyService spotifyService, IStringLocalizer<PostsController> localizer)
        {
            _context = context;
            _userManager = userManager;
            _spotifyService = spotifyService;
            _localizer = localizer;
        }

        /// <summary>
        /// Gets all the posts of the application.
        /// </summary>
        /// <returns>All posts of the application.</returns>
        // GET: Posts
        public async Task<IActionResult> Index()
        {
              return View(await _context.Posts.ToListAsync());
        }

        /// <summary>
        /// Gets the details of a post. If the post is not found, it returns a 404 error.
        /// </summary>
        /// <remarks>GET: Posts/Details/{id}</remarks>
        /// <param name="id">The id of the post.</param>
        /// <returns>The details page of a single post.</returns>
        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(user => user.User).Include(band => band.Band).Include(song => song.Song)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            if (post.Song != null)
            {
                ViewBag.urlSong = _spotifyService.GetSongAsync(post.Song.SongId).Result.PreviewUrl;
            }
            ViewBag.currentUser = await _userManager.GetUserAsync(User);
            ViewBag.commentsPosts = _context.Comment.Include(user => user.User).Where(s => s.post.PostId == post.PostId).ToList();
            return View(post);
        }

        
        /// <summary>
        /// Finds the create view. If the user is not logged in, it redirects to the login page.
        /// </summary>
        /// <remarks>GET: Posts/Create</remarks>
        /// <param name="IdBand">The id of the band.</param>
        /// <param name="IdSong">The id of the song.</param>
        /// <returns>The create view.</returns>
        // GET: Posts/Create
        public async Task<IActionResult> CreateAsync([FromQuery] string IdBand, [FromQuery] string IdSong)
        {
            if (!User.Identity.IsAuthenticated) {
                return Redirect("/Identity/Account/Login");
            }
            ViewBag.chooseBand = null;
            ViewBag.chooseSong = null;
            if (!string.IsNullOrEmpty(IdBand)) 
            {
                ViewBag.chooseBand = _spotifyService.GetBandAsync(IdBand).Result;
            }
            if (!string.IsNullOrEmpty(IdSong))
            {
                ViewBag.chooseSong = _spotifyService.GetSongAsync(IdSong).Result;
            }
            ViewBag.songs = _spotifyService.GetMultipleSongsAsync("a").Result.Tracks.Items;
            ViewBag.bands = _spotifyService.GetMultipleBandsAsync("a").Result.Artists.Items;
            return View();
        }

        /// <summary>
        /// The create post action. If the post is valid, it saves it in the database.
        /// </summary>
        /// <param name="post">The Post object which will be validated.</param>
        /// <remarks>POST: Posts/Create</remarks>
        /// <returns>If the post is valid, returns the details of the newly created post. If its invalid, shows the error to the user in the create page.</returns>
        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include: "Title,Content")] Post post)
        {
            if (ModelState.IsValid)
            {
                var sanitizer = new HtmlSanitizer();
                var sanitizedContent = sanitizer.Sanitize(post.Content);
                if (string.IsNullOrEmpty(sanitizedContent))
                {
                    ModelState.AddModelError(string.Empty, _localizer["error_content"]);
                } else if (string.IsNullOrEmpty(post.Title)) 
                {
                    ModelState.AddModelError(string.Empty, _localizer["error_title"]);
                }
                else
                {
                    Console.WriteLine(post.ToString());
                    var user = await _userManager.GetUserAsync(User);
                    if (User?.Identity.IsAuthenticated == true)
                    {
                        post.User = user;
                        var band = new Band();
                        if (!string.IsNullOrEmpty(Request.Form["BandId"]))
                        {
                            band = await _context.Bands.FindAsync(Request.Form["BandId"]);
                            if (band == null)
                            {
                                Band newBand = new Band();
                                var SpotifyBand = await _spotifyService.GetBandAsync(Request.Form["BandId"]);
                                newBand.BandId = Request.Form["BandId"];
                                newBand.Name = SpotifyBand.Name;
                                newBand.Image = SpotifyBand.Images[0].Url;
                                band = newBand;
                                post.Band = band;
                                _context.Bands.Add(band);
                                await _context.SaveChangesAsync();
                            }
                        }
                        var song = new Song();
                        if (!string.IsNullOrEmpty(Request.Form["SongId"]))
                        {
                            song = await _context.Songs.Include(s => s.Band).FirstOrDefaultAsync(s => s.SongId.Equals(Request.Form["SongId"]));
                            if (song == null)
                            {
                                Song newSong = new Song();
                                var SpotifySong = await _spotifyService.GetSongAsync(Request.Form["SongId"]);
                                var SongBand = await _spotifyService.GetBandAsync(SpotifySong.Artists[0].Id);
                                var checkBand = _context.Bands.Find(SongBand.Id);
                                if (checkBand == null)
                                {
                                    newSong.Band = new Band() { Name = SpotifySong.Artists[0].Name, BandId = SpotifySong.Artists[0].Id, Image = SongBand.Images[0].Url };
                                    band = newSong.Band;
                                    _context.Bands.Add(newSong.Band);
                                    await _context.SaveChangesAsync();
                                }
                                else
                                {
                                    newSong.Band = checkBand;
                                    band = newSong.Band;
                                }
                                newSong.SongId = Request.Form["SongId"];
                                newSong.Name = SpotifySong.Name;
                                song = newSong;
                                post.Song = song;
                                post.Band = song.Band;
                                song = new Song() { Name = newSong.Name, Band = newSong.Band, SongId = newSong.SongId };
                                Console.WriteLine("EndSong-" + band.BandId);
                                _context.Songs.Add(song);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                band = await _context.Bands.FindAsync(song.Band.BandId);
                            }
                        }
                        Console.WriteLine("Last-" + band.BandId);
                        var postToSubmit = new Post();
                        if (!string.IsNullOrEmpty(song.SongId))
                        {
                            postToSubmit = new Post() { User = user, Title = post.Title, Content = post.Content, Band = band, Song = song };
                        }
                        else if (!string.IsNullOrEmpty(band.BandId))
                        {
                            postToSubmit = new Post() { User = user, Title = post.Title, Content = post.Content, Band = band };
                        }
                        else
                        {
                            postToSubmit = new Post() { User = user, Title = post.Title, Content = post.Content };
                        }
                        postToSubmit.CreationDate = DateTime.Now;
                        var result = await _context.Posts.AddAsync(postToSubmit);
                        TempData["Sucess"] = _localizer["your_post_was_sucessfully_created"].Value;
                        await _context.SaveChangesAsync();
                        var value = _context.Posts.OrderBy(e => e.PostId).LastAsync().Result;
                        if (value != null)
                        {
                            return Redirect("Details/" + value.PostId);
                        }
                        return RedirectToAction(nameof(Index));
                    }
                }
                return View(post);
            }
            // If model state is not valid, display error messages.
            Console.WriteLine("User: " + await _userManager.GetUserAsync(User));
            foreach (ModelStateEntry modelState in ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    string errorMessage = error.ErrorMessage;
                    ModelState.AddModelError(string.Empty, errorMessage);
                    Console.WriteLine(errorMessage);
                    TempData["error"] = errorMessage;
                }
            }

            ViewBag.songs = _spotifyService.GetMultipleSongsAsync("a").Result.Tracks.Items;
            ViewBag.bands = _spotifyService.GetMultipleBandsAsync("a").Result.Artists.Items;

            return View(post);
        }

        /// <summary>
        ///  Edit a post.
        /// </summary>
        /// <param name="id">The id of the post about to be edited.</param>
        /// <remarks>GET: Posts/Edit/{id}</remarks>
        /// <returns>The edit view.</returns>
        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!User.Identity.IsAuthenticated) {
                return Redirect("/Identity/Account/Login");
            }
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            if (post.User.Id != (await _userManager.GetUserAsync(User)).Id) {
                return Redirect("/");
            }
            return View(post);
        }

        /// <summary>
        /// Submits the edited post and stores in the database.
        /// </summary>
        /// <param name="id">The id of the post being edited.</param>
        /// <param name="post">The post object changed.</param>
        /// <remarks>POST: Posts/Edit/5</remarks>
        /// <returns>If the edit is correct, returns the details of the page. If not, shows the error to the user.</returns>
        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Content")] Post post)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    var ogPost = _context.Posts.Find(id);
                    if (ogPost == null) {
                        return NotFound();
                    }
                    if (user.Id == ogPost.User.Id) //Add for Roles
                    {
                        ogPost.Content = post.Content;
                        ogPost.Title = post.Title;
                        ogPost.EditDate = DateTime.Now;
                        ogPost.IsEdited = true;
                        _context.Update(ogPost);
                        TempData["Sucess"] = _localizer["your_post_was_sucessfully_updated"].Value;
                        await _context.SaveChangesAsync();
                    }
                    else {
                        TempData["Insucess"] = _localizer["no_permission_for_modification"].Value;
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["Insucess"] = _localizer["an_error_occurred"].Value;
                    if (!PostExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("../Details/" + id);
            }
            return Redirect("../Details/" + id); 
        }

        /// <summary>
        /// Deletes a post.
        /// </summary>
        /// <param name="id">The id of the post about to be deleted.</param>
        /// <remarks>GET: Posts/Delete/5</remarks>
        /// <returns>Returns the delete view.</returns>
        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        /// <summary>
        /// The action for deleting the post.
        /// </summary>
        /// <param name="id">The id of the post about to be deleted.</param>
        /// <remarks>POST: Posts/Delete/5</remarks>
        /// <returns>If the post exists, returns the index view of this controller. If not, returns a 404 error.</returns>
        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
            }

            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                var commentRatings = await _context.CommentRatings.Where(cr => cr.Comment.post.PostId == post.PostId).ToListAsync();
                foreach (var rating in commentRatings) {
                    _context.CommentRatings.Remove(rating);
                }
                _context.Comment.RemoveRange(_context.Comment.Where(x => x.post.PostId == post.PostId).ToList());

                var postRatings = await _context.PostRatings.Where(pr => pr.Post.PostId == post.PostId).ToListAsync();
                foreach (var rating in postRatings) {
                    _context.PostRatings.Remove(rating);
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();

                TempData["Message"] = _localizer["post_deleted_sucessfully"].Value;
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
          return _context.Posts.Any(e => e.PostId == id);
        }

        /// <summary>
        /// Used for fetching the songs for the Song dropdown list in the Create view.
        /// </summary>
        /// <remarks>GET: Posts/SongInfo?searchKey={key}</remarks>
        /// <param name="searchKey">The search key used to search for the song by its name.</param>
        /// <returns>A JSON with all found songs.</returns>
        public ActionResult SongInfo([FromQuery] string searchKey)
        {
            var songs = new List<SpotifyAPI.Web.FullTrack>();
            if (!string.IsNullOrEmpty(searchKey))
            {
                songs = _spotifyService.GetMultipleSongsAsync(searchKey).Result.Tracks.Items;
            }
            else
            {
                songs = _spotifyService.GetMultipleSongsAsync("A").Result.Tracks.Items;
            }
            return Json(songs);
        }

        /// <summary>
        /// Used for fetching the bands for the Band dropdown list in the Create view.
        /// </summary>
        /// <remarks>GET: Posts/BandInfo?searchKey={key}</remarks>
        /// <param name="searchKey">The search key used to search for the band by its name.</param>
        /// <returns>A JSON with all found bands.</returns>
        public ActionResult BandsInfo([FromQuery] string searchKey)
        {
            var bands = new List<SpotifyAPI.Web.FullArtist>();
            if (!string.IsNullOrEmpty(searchKey))
            {
                bands = _spotifyService.GetMultipleBandsAsync(searchKey).Result.Artists.Items;
            }
            else
            {
                bands = _spotifyService.GetMultipleBandsAsync("A").Result.Artists.Items;
            }
            return Json(bands);
        }
    }
}
