using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tidbeat.Data;
using Tidbeat.Models;

namespace Tidbeat.Controllers
{
    public class FollowsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        public FollowsController(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        // GET: Follows
        public async Task<IActionResult> Followers(string userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == userId);
            //var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                var result = _context.Follow.Include(u => u.UserFollowed != null ? u.UserFollowed : null).Include(f => f.UserAsker != null ? f.UserAsker : null).Where(f => f.UserAsker.Id == user.Id).Select(us => us.UserFollowed);

                //var result = from follows in _context.Follow where follows.UserAsker == user select follows.UserFollowed;
                List<ApplicationUser> applicationUsers = result.ToList();
                return Json(applicationUsers);
            }
            Console.WriteLine("Is null");
            return Json(null);
        }

        public async Task<IActionResult> Followies(string userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == userId);
            //var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                var result = _context.Follow.Include(u => u.UserFollowed).Include(f => f.UserAsker).Where(f => f.UserFollowed.Id == user.Id).Select(us => us.UserAsker);
                //var result = from follows in _context.Follow where follows.UserAsker == user select follows.UserFollowed;
                return Json(result.ToListAsync().Result);
            }
            Console.WriteLine("Is null");
            return Json(null);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(string userId)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var possibleFollower = await _context.Users
                            .FirstOrDefaultAsync(m => m.Id == userId);
                    //var result = from follows in _context.Follow where follows.UserAsker == user && follows.UserFollowed == possibleFollower select follows.UserFollowed;
                    var result = _context.Follow.Include(u => u.UserFollowed).Include(f => f.UserAsker).Where(f => f.UserAsker.Id == user.Id).Where(u => u.UserFollowed.Id == possibleFollower.Id).Select(us => us.UserFollowed);
                    if (result.Count() > 0 || possibleFollower == null)
                    {//Follow error
                        return Json("Already Exists");
                    }
                    var newFollow = new Follow() { UserAsker = user, UserFollowed = possibleFollower };
                    var finalResult = _context.Follow.AddAsync(newFollow);
                    if (finalResult.IsCompletedSuccessfully)
                    {
                        _context.SaveChanges();
                        return Json("New Follower");
                    }
                    return Json("Error Ocurred");
                }
            }
            return Json(null);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnFollow(string userId)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var possibleFollower = await _context.Users
                            .FirstOrDefaultAsync(m => m.Id == userId);
                    //var result = from follows in _context.Follow where follows.UserAsker == user && follows.UserFollowed == _context.Users.FindAsync(userId).Result select follows;
                    var result = _context.Follow.Include(u => u.UserFollowed).Include(f => f.UserAsker).Where(f => f.UserAsker.Id == user.Id).Where(u => u.UserFollowed.Id == possibleFollower.Id);
                    if (result.Count() > 0)
                    {
                        //Miss Exceptions
                        var getFollow = result.FirstAsync();
                        if (getFollow != null && getFollow.IsCompletedSuccessfully)
                        {
                            var deleteResult = _context.Follow.Remove(getFollow.Result);
                            _context.SaveChanges();
                            return Json("Sucess");
                        }
                        return Json("Error");
                    }
                    //UnFollow error
                    return Json(null);
                }
            }
            return Json(null);
        }

        private bool FollowExists(int id)
        {
            return (_context.Follow?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
