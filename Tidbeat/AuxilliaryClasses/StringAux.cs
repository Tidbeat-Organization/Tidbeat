namespace Tidbeat.AuxilliaryClasses
{
	public class StringAux
	{
		public static string UppercaseFirstLetter(string str)
		{
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return $"{str[0]}".ToUpper() + str.Substring(1); 
		}
	}
}
