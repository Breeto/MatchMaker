using System;


namespace MatchMaker.Web.Registration
{
	public class EmailAddressFormatValidator : IEmailAddressFormatValidator
	{
		public bool IsValidFormat( string emailAddress )
		{
			try
			{
				new System.Net.Mail.MailAddress( emailAddress );
				return true;
			}
			catch ( Exception )
			{
				return false;
			}
		}
	}
}