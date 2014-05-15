using System;
using System.Collections.Generic;
using Nancy.Security;


namespace MatchMaker.Web.Authentication
{

	public class UserIdentity : IUserIdentity
	{

		public Guid UserId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public IEnumerable<string> Claims
		{
			get;
			set;
		}

	}

}

