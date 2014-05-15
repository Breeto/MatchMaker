namespace MatchMaker.Web.Registration
{

	public interface IEmailAddressFormatValidator
	{
		bool IsValidFormat( string emailAddress );
	}
}