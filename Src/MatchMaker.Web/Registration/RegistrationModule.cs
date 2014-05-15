using System;
using MatchMaker.Common.ExtensionMethods;
using MatchMaker.Data;
using MatchMaker.Web.Authentication;
using MatchMaker.Web.Common;
using Nancy;
using Nancy.ModelBinding;


namespace MatchMaker.Web.Registration
{

	public class RegistrationModule : NancyModule
	{
		private readonly IDatabase database;
		private readonly IPasswordEncryptor passwordEncryptor;


		public RegistrationModule( IDatabase database, IEmailAddressFormatValidator emailAddressFormatValidator, IPasswordEncryptor passwordEncryptor )
		{
			this.database = database;
			this.passwordEncryptor = passwordEncryptor;


			Get[Route.Register] = o =>
			{
				var model = new RegistrationRequest();

				return View[Route.Register, model];
			};


			Post[Route.Register] = o =>
			{
				RegistrationRequest request = this.Bind<RegistrationRequest>();

				try
				{
					if ( string.IsNullOrWhiteSpace(request.UserName) || !App.Settings.UserNameSizeRange.Contains( request.UserName.Length ) )
					{
						throw new Exception( string.Format( "User name must be between {0} and {1} characters long", App.Settings.UserNameSizeRange.Min, App.Settings.UserNameSizeRange.Max ) );
					}

					if ( !request.UserName.IsAlphanumeric() )
					{
						throw new Exception( "User name can contain only alphanumeric characters" );
					}

					if ( null != database.GetUserByName( request.UserName ) )
					{
						throw new Exception("An account with that name already exists");
					}

//					if ( !emailAddressFormatValidator.IsValidFormat(request.EmailAddress) )
//					{
//						throw new Exception("Invalid email address");
//					}
//					
//					if ( null != database.GetUserCredentialsByEmailAddress( request.EmailAddress ) )
//					{
//						throw new Exception("That email address is already in use");
//					}

					var result = PasswordValidator.Validate( request.Password1 );
					if ( !result.IsValid )
					{
						throw new Exception( string.Join( ", ", result.ValidationErrors ) );
					}

					if ( request.Password1 != request.Password2 )
					{
						throw new Exception( "Passwords do not match" );
					}

					AddUserFor( request );
				}
				catch ( Exception e )
				{
					this.AddUserMessage(e.Message);
					return View[Route.Register, request];
				}

				this.AddUserMessage("Registration successful");
				return Response.AsRedirect( Route.SelectGames );
			};
		}


		private void AddUserFor( RegistrationRequest request )
		{
			var user = new User();
			user.Name = request.UserName;

			var credentials = new UserCredentials
			{
				EmailAddress = request.EmailAddress, 
				EncryptedPassword = passwordEncryptor.Encrypt( request.Password1 ), 
				AuthId = Guid.NewGuid()
			};
			database.AddUser( user, credentials );
		}
	}

}



