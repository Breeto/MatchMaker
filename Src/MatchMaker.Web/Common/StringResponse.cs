using Nancy;


namespace MatchMaker.Web.Common
{

	public class StringResponse : Response
	{

		public StringResponse( string responseString )
		{
			Contents = GetStringContents( responseString );
		}

	}

}