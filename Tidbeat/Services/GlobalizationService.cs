using System.Globalization;

namespace Tidbeat.Services {
    /// <summary>
    /// The globalization service. Takes care of the stuff related to getting countries.
    /// </summary>
    public class GlobalizationService {

        /// <summary>
        /// Gets the list of countries.
        /// </summary>
        /// <returns>A list of all countries in the world.</returns>
        public static List<string> CountryList() {
            List<string> CultureList = new List<string>();

            CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo culture in getCultureInfo) {
                RegionInfo GetRegionInfo = new RegionInfo(culture.LCID);
                if (!(CultureList.Contains(GetRegionInfo.EnglishName))) {
                    CultureList.Add(GetRegionInfo.EnglishName);
                }
            }
            CultureList.Sort();
            return CultureList;
        }
    }
}
