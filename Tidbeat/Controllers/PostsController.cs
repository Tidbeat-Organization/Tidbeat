using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Tidbeat.Data;
using Tidbeat.Models;
using Tidbeat.Services;

namespace Tidbeat.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISpotifyService _spotifyService;

        public PostsController(ApplicationDbContext context, IServiceProvider serviceProvider, ISpotifyService spotifyService)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _spotifyService = spotifyService;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
              return View(await _context.Posts.ToListAsync());
        }

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

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include: "Title,Content")] Post post)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine(post.ToString());
                var user = await _userManager.GetUserAsync(User);
                if (User?.Identity.IsAuthenticated == true)
                {
                    post.User = user;
                    var band = new Band();
                    if (!string.IsNullOrEmpty(Request.Form["BandId"])) {
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
                        song = await _context.Songs.FindAsync(Request.Form["SongId"]);
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
                            song = new Song() { Name = newSong.Name, Band = newSong.Band , SongId= newSong.SongId };
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
                    var postToSubmit =new Post();
                    if (!string.IsNullOrEmpty(song.SongId))
                    {
                        postToSubmit = new Post() { User = user, Title = post.Title, Content = post.Content, Band = band, Song = song};
                    }
                    else if (!string.IsNullOrEmpty(band.BandId))
                    {
                        postToSubmit = new Post() { User = user, Title = post.Title, Content = post.Content, Band = band};
                    }
                    else {
                        postToSubmit = new Post() { User = user, Title = post.Title, Content = post.Content};
                    }
                    var result = await _context.Posts.AddAsync(postToSubmit);
                    TempData["Sucess"] = "O seu post foi criado com sucesso.";
                    await _context.SaveChangesAsync();
                    var value = _context.Posts.OrderBy(e => e.PostId).LastAsync().Result;
                    if (value != null) {
                        return Redirect("Details/"+value.PostId);
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            // If model state is not valid, display error messages.
            Console.WriteLine("User: " + await _userManager.GetUserAsync(User));
            foreach (ModelStateEntry modelState in ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    string errorMessage = error.ErrorMessage;
                    Console.WriteLine(errorMessage);
                }
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                        _context.Update(ogPost);
                        TempData["Sucess"] = "O seu post foi atualizado com sucesso.";
                        await _context.SaveChangesAsync();
                    }
                    else {
                        TempData["Insucess"] = "Não têm permissões para modificar o post";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["Insucess"] = "Ocorreu um erro";
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

                TempData["Message"] = "O seu post foi apagado com sucesso.";
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
