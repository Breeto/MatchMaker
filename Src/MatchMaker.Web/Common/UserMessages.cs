using System;
using System.Collections.Generic;
using Nancy;
using Nancy.Session;
using RingtailDesign.Common.Collections;


namespace MatchMaker.Web.Common
{

	public static class UserMessages
	{
		
		private static readonly Map<string, List<string>> userMessagesMap = new Map<string, List<string>>( s => new List<string>() ); 


		/// <summary>
		/// Storing a unique session ID in the cookie-based Nancy session, and then storing the actual user messages in memory on the server.
		/// Doing it this way because something is not working correctly with storing the user messages in the actual cookie session.
		/// </summary>
		public static void Add( NancyContext context, string message )
		{
			var session = context.Request.Session;
			var sessionId = GetSessionId( session );

			userMessagesMap[sessionId].Add( message );
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

			var messages = userMessagesMap[sessionId];
			var messageArray = messages.ToArray();
			messages.Clear();

			return messageArray;
		}

	}

}