using System.Net.Http;

namespace Tidbeat.AuxilliaryClasses
{
    public class SpotifyAux
    {
        /// <summary>
        /// Don't use this method, it's just for testing.
        /// </summary>
        public static async void TestTrackApiCall()
        {
            var client = new HttpClient();
            
            var res = await client.GetAsync("https://api.spotify.com/v1/tracks/11dFghVXANMlKmJXsNCbNl");
            var resStatus = res.StatusCode;
            var resOutput = await res.Content.ReadAsStringAsync();
            Console.WriteLine($"\nSTATUS: {resStatus} ________");
            Console.WriteLine(resOutput);
        }
    }
}
