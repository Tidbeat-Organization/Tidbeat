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
        //Miss Getting the current user
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var result = _context.Follow.Include(u => u.UserFollowed).Include(f => f.UserAsker).Where(f => f.UserAsker == user).Select(us => us.UserFollowed);
                //var result = from follows in _context.Follow where follows.UserAsker == user select follows.UserFollowed;
                return Json(result.ToListAsync().Result);
            }

            return Json(null);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(int userId)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var possibleFollower = _context.Users.FindAsync(userId).Result;
                    //var result = from follows in _context.Follow where follows.UserAsker == user && follows.UserFollowed == possibleFollower select follows.UserFollowed;
                    var result = _context.Follow.Include(u => u.UserFollowed).Include(f => f.UserAsker).Where(f => f.UserAsker == user).Where(u => u.UserFollowed == possibleFollower).Select(us => us.UserFollowed);
                    if (result.Count() > 0 || possibleFollower == null)
                    {//Follow error
                        return Json("Already Exists");
                    }
                    var newFollow = new Follow() { UserAsker = user, UserFollowed = possibleFollower };
                    var finalResult = _context.Follow.AddAsync(newFollow);
                    if (finalResult.IsCompletedSuccessfully)
                    {
                        return Json("New Follower");
                    }
                    return Json("Error Ocurred");
                }
            }
            return Json(null);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnFollow(int userId)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var possibleFollower = _context.Users.FindAsync(userId).Result;
                    //var result = from follows in _context.Follow where follows.UserAsker == user && follows.UserFollowed == _context.Users.FindAsync(userId).Result select follows;
                    var result = _context.Follow.Include(u => u.UserFollowed).Include(f => f.UserAsker).Where(f => f.UserAsker == user).Where(u => u.UserFollowed == possibleFollower);
                    if (result.Count() > 0)
                    {
                        //Miss Exceptions
                        var getFollow = result.FirstAsync();
                        if (getFollow != null && getFollow.IsCompletedSuccessfully)
                        {
                            var deleteResult = _context.Follow.Remove(getFollow.Result);
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
