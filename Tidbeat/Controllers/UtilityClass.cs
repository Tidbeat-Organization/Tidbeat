using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Tidbeat.Models;

namespace Tidbeat.Controllers
{
    public static class UtilityClass
    {
        public static async Task<List<ApplicationUser>> SideBarAsync(string id, string currentURL)
        {
            using (var client = new HttpClient())
            {
                Console.WriteLine(currentURL);
                // Call the second action and get the JSON result
                var response = await client.GetAsync(currentURL + "/Follows/Followies?userId=" + id);
                response.EnsureSuccessStatusCode(); // Throws an exception if the status code is not 2xx

                // Deserialize the JSON result and store it in TempData
                var jsonResult = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<ApplicationUser>>(jsonResult);
                return data;
            }
        }
    }
}