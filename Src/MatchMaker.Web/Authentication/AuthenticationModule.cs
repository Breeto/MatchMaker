using System;
using MatchMaker.Data;
using MatchMaker.Web.Common;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;


namespace MatchMaker.Web.Authentication
{

	public class AuthenticationModule : NancyModule
	{

		private const string LoginFailureMessage = "Login failed";


		public AuthenticationModule( IDatabase database, IPasswordEncryptor passwordEncryptor )
		{

			Get[Route.Login] = o => View[Route.Login, new LoginRequest()];


			Post[Route.Login] = o =>
			{
				var request = this.Bind<LoginRequest>();

				try
				{
					// TODO: Use validation attributes, or call Validate on LoginRequest

					if ( string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password) )
					{
						throw new Exception("Username and password are required");
					}

					var user = database.GetUserByName( request.UserName );

					if ( null == user )
					{
						throw new Exception( LoginFailureMessage );
					}

					var credentials = database.GetUserCredentialsByUserId( user.Id );

					if ( passwordEncryptor.Encrypt(request.Password) != credentials.EncryptedPassword )
					{
						throw new Exception( LoginFailureMessage );
					}

					var expiry = DateTime.MaxValue;

					return this.LoginAndRedirect( credentials.AuthId, expiry, Route.Home );
				}
				catch ( Exception exception )
				{
					this.AddUserMessage(exception.Message);

					return View[Route.Login, request];
				}
			};


			Get[Route.Logout] = o =>
			{
//				this.AddUserMessage("You have been logged out");

				return this.LogoutAndRedirect( Route.Login );
			};

		}

	}
}
