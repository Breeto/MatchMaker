using System;


namespace MatchMaker.Data
{

	public class UserCredentials
	{

		public UserCredentials()
		{
			Id = Guid.NewGuid();
			AuthId = Guid.NewGuid();
		}

		public UserCredentials( User user ) : this()
		{
			UserId = user.Id;
		}

		// Required for MongoDB
		public Guid Id
		{
			get;
			set;
		}

		public Guid UserId
		{
			get;
			set;
		}

		public Guid AuthId
		{
			get;
			set;
		}

		public string EncryptedPassword
		{
			get;
			set;
		}

		public string EmailAddress
		{
			get;
			set;
		}

	}

}

