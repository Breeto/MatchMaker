using System;
using MatchMaker.Data;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;


namespace MatchMaker.Web.Authentication
{

	public class UserMapper : IUserMapper
	{

		private readonly IDatabase database;


		public UserMapper(IDatabase database)
		{
			this.database = database;
		}


		public IUserIdentity GetUserFromIdentifier( Guid identifier, NancyContext context )
		{
			var credentials = database.GetUserCredentialsByAuthId( identifier );

			if ( null == credentials )
			{
				throw new ArgumentException( "No user with that identifier found" );
			}

			var user = database.GetUserById( credentials.UserId );

			return new UserIdentity()
			{
				UserId = user.Id,
				UserName = user.Name,
			};
		}

	}

}


