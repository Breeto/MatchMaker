	
using System;
using Nancy;
using Nancy.Session;


namespace MatchMaker.Web
{

	public class SessionHelper
	{

		private const string SessionIdKey = "SessionId";
		private const string CurrentGameIdKey = "CurrentGameId";


		private readonly ISession session;


		public SessionHelper( NancyContext nancyContext ) : this(nancyContext.Request.Session)
		{
		}


		public SessionHelper( ISession session )
		{
			this.session = session;
		}


		public string SessionId
		{
			get
			{
				return session[SessionIdKey] as string;
			}
			set
			{
				session[SessionIdKey] = value;
			}
		}


		public Guid? CurrentGameId
		{
			get
			{
				Guid? id = null;
				try
				{
					id = (Guid?)(session[CurrentGameIdKey]);
				}
				catch
				{
				}

				return id;
			}
			set
			{
				session[CurrentGameIdKey] = value;
			}
		}

	}

}
