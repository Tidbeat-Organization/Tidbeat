using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Tidbeat.Models;

namespace Tidbeat.Controllers
{
    /// <summary>
    /// A utility class used mainly for the Sidebar.
    /// </summary>
    public static class UtilityClass
    {
        /// <summary>
        /// The method which fetches all the users the logged user follows.
        /// </summary>
        /// <param name="id">The id of the logged in user.</param>
        /// <param name="currentURL">The current main URL.</param>
        /// <returns>A list of users that the logged in user follows.</returns>
        public static async Task<List<ApplicationUser>> SideBarAsync(string id, string currentURL)
        {
            using (var client = new HttpClient())
            {
                Console.WriteLine(currentURL);
                // Call the second action and get the JSON result
                var response = await client.GetAsync(currentURL + "/Follows/Followers?userId=" + id);
                response.EnsureSuccessStatusCode(); // Throws an exception if the status code is not 2xx

                // Deserialize the JSON result and store it in TempData
                var jsonResult = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<ApplicationUser>>(jsonResult);
                return data;
            }
        }
    }
}