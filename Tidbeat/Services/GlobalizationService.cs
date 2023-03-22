using System.Globalization;

namespace Tidbeat.Services {
    public class GlobalizationService {

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
