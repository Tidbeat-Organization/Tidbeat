using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
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
                    var user = await _userManager.GetUserAsync(User);
                if (user.FullName != null)
                {
                    post.User = user;
                    var band = await _context.Bands.FindAsync(post.Band.BandId);
                    if (band == null) 
                    {
                        Band newBand = new Band();
                        var SpotifyBand = await _spotifyService.GetBandAsync(post.Band.BandId);
                        newBand.BandId = post.Band.BandId;
                        newBand.Name = SpotifyBand.Name;
                        newBand.Image = SpotifyBand.Images[0].Url;
                        band = newBand;
                        _context.Bands.Add(band);
                    }
                    var song = await _context.Songs.FindAsync(post.Song.SongId);
                    if (song == null)
                    {
                        Song newSong = new Song();
                        var SpotifySong = await _spotifyService.GetSongAsync(post.Song.SongId);
                        newSong.SongId = post.Song.SongId;
                        newSong.Name = SpotifySong.Name;
                        newSong.Band = band;
                        song = newSong;
                        _context.Songs.Add(song);
                    }
                    _context.Add(post);
                    await _context.SaveChangesAsync();
                    TempData["Sucess"] = "O seu post foi criado com sucesso.";
                    return RedirectToAction(nameof(Details),post.PostId);
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
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user.Equals(post.User)) //Add for Roles
                    {
                        var band = await _context.Bands.FindAsync(post.Band.BandId);
                        if (band == null)
                        {
                            Band newBand = new Band();
                            var SpotifyBand = await _spotifyService.GetBandAsync(post.Band.BandId);
                            newBand.BandId = post.Band.BandId;
                            newBand.Name = SpotifyBand.Name;
                            newBand.Image = SpotifyBand.Images[0].Url;
                            band = newBand;
                            _context.Bands.Add(band);
                        }
                        var song = await _context.Songs.FindAsync(post.Song.SongId);
                        if (song == null)
                        {
                            Song newSong = new Song();
                            var SpotifySong = await _spotifyService.GetSongAsync(post.Song.SongId);
                            newSong.SongId = post.Song.SongId;
                            newSong.Name = SpotifySong.Name;
                            newSong.Band = band;
                            song = newSong;
                            _context.Songs.Add(song);
                        }
                        _context.Update(post);
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
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), post.PostId);
            }
            return View(post);
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
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();

                TempData["Message"] = "O seu post foi apagado com sucesso.";
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), post.PostId);
        }

        private bool PostExists(int id)
        {
          return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
