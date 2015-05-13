using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Nancy;
using Nancy.Session;


namespace MatchMaker.Web.Common
{

	public static class UserMessages
	{
		
		private static readonly ConcurrentDictionary<string, List<string>> userMessagesMap = new ConcurrentDictionary<string, List<string>>(); 


		/// <summary>
		/// Storing a unique session ID in the cookie-based Nancy session, and then storing the actual user messages in memory on the server.
		/// Doing it this way because something is not working correctly with storing the user messages in the actual cookie session.
		/// </summary>
		public static void Add( NancyContext context, string message )
		{
			var session = context.Request.Session;
			var sessionId = GetSessionId( session );
			
			List<string> messages;
			if(!userMessagesMap.TryGetValue(sessionId, out messages))
			{
				messages = new List<string>();
				userMessagesMap[sessionId] = messages;
			}
			messages.Add( message );
		}


		/// <summary>
		/// Gets the unique session ID for the current session.
		/// </summary>
		private static string GetSessionId( ISession session )
		{
			var sessionHelper = new SessionHelper( session );

			var sessionId = sessionHelper.SessionId;

			if ( null == sessionId )
			{
				sessionId = CreateSessionId( session );
				sessionHelper.SessionId = sessionId;
			}

			return sessionId;
		}


		private static string CreateSessionId( ISession module )
		{
			return Guid.NewGuid().ToString();
		}


		public static string[] Pop( NancyContext context )
		{
			var session = context.Request.Session;
			var sessionId = GetSessionId( session );

			List<string> messages;
			if(!userMessagesMap.TryGetValue(sessionId, out messages))
			{
				messages = new List<string>();
				userMessagesMap[sessionId] = messages;
			}
			var messageArray = messages.ToArray();
			messages.Clear();

			return messageArray;
		}

	}

}