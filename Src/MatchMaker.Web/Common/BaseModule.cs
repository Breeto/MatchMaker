using MatchMaker.Data;
using MatchMaker.Web.Authentication;
using Nancy;


namespace MatchMaker.Web.Common
{

	public abstract class BaseModule : NancyModule
	{

		private readonly IDatabase database;
		private User currentUser = null;


		protected BaseModule( IDatabase database )
		{
			this.database = database;
		}


		protected User CurrentUser
		{
			get
			{
				if ( null == currentUser )
				{
					var userId = ( (UserIdentity)Context.CurrentUser ).UserId;
					currentUser = database.GetUserById( userId );
				}

				return currentUser;
			}
		}
		
	}
}