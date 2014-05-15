using MatchMaker.Web.Common;


namespace MatchMaker.Web.Authentication
{

	public class PasswordValidator
	{

		public static ValidationReslult Validate( string password )
		{
			var result = new ValidationReslult();
			
			if ( string.IsNullOrWhiteSpace( password ) || password.Length < App.Settings.MinimumPasswordLength )
			{
				result.AddValidationError( string.Format( "Password must be at least {0} characters long", App.Settings.MinimumPasswordLength ) );
			}

			return result;
		}

	}

}
