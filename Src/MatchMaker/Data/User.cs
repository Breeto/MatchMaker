using System;


namespace MatchMaker.Data
{

	public class User
	{

		public User()
		{
			Id = Guid.NewGuid();
		}


		public Guid Id
		{
			get;
			set;
		}
	
		public string Name
		{
			get;
			set;
		}

		public override string ToString()
		{
			return Name;
		}

	}

}
