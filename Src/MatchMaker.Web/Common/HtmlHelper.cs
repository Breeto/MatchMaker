namespace MatchMaker.Web.Common
{

	public class HtmlHelper
	{
	
		public static string CheckedAttributeFor( bool @checked )
		{
			return @checked ? "checked='checked'" : string.Empty;
		}
	 
	}

}
