using System;
using System.Collections.Generic;
using MatchMaker.Data;
using MatchMaker.Web.Authentication;
using MatchMaker.Web.Common;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using System.Linq;


namespace MatchMaker.Web.UserProfile
{

	public class UserProfileModule : BaseModule
	{
		private readonly IPasswordEncryptor passwordEncryptor;


		public UserProfileModule( IDatabase database, IPasswordEncryptor passwordEncryptor ) : base( database )
		{
			this.passwordEncryptor = passwordEncryptor;

			this.RequiresAuthentication();


			Get[Route.UserProfile] = o =>
			{
				return View[Route.UserProfile];
			};


			Get[Route.ChangePassword] = o =>
			{
				return View[Route.ChangePassword];
			};


			Post[Route.ChangePassword] = o =>
			{
				var request = this.Bind<ChangePasswordRequest>();

				var user = this.CurrentUser;

				try
				{
					var userCredentials = database.GetUserCredentialsByUserId( user.Id );

					if ( string.IsNullOrWhiteSpace( request.CurrentPassword ) )
					{
						throw new Exception( "Incorrect password" );
					}

					if ( passwordEncryptor.Encrypt( request.CurrentPassword ) != userCredentials.EncryptedPassword )
					{
						throw new Exception( "Incorrect password" );
					}

					if ( request.NewPassword1 != request.NewPassword2 )
					{
						throw new Exception( "Passwords don't match" );
					}

					var result = PasswordValidator.Validate( request.NewPassword1 );
					if ( !result.IsValid )
					{
						throw new Exception( string.Join( ", ", result.ValidationErrors ) );
					}

					userCredentials.EncryptedPassword = passwordEncryptor.Encrypt( request.NewPassword1 );

					database.UpdateUserCredentials( userCredentials );
				}
				catch ( Exception ex )
				{
					this.AddUserMessage( ex.Message );
					return View[Route.ChangePassword];
				}

				this.AddUserMessage( "Your password has been changed" );
				return View[Route.UserProfile];
			};


			Get[Route.SelectGames] = o =>
			{
				var model = new SelectGamesViewModel();

				var availableGames = database.GetGames();
				var gamesPlayedByUser = database.GetGamesPlayedByUser( ((UserIdentity)Context.CurrentUser).UserId );

				foreach ( var availableGame in availableGames )
				{
					model.SelectableGames.Add( new Selectable<Game>
					{
						Item = availableGame,
						IsSelected = gamesPlayedByUser.Any( game => game.Id == availableGame.Id )
					} );
				}

				return View[Route.SelectGames, model];
			};


			Post[Route.SelectGames] = o =>
			{
				var request = this.Bind<SelectGamesRequest>();

				database.SetGamesPlayedByUser(((UserIdentity)Context.CurrentUser).UserId, request.GameIds);

				return Response.AsRedirect( Route.Home );
			};
		}
	 
	}
}