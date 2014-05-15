using System.ComponentModel.DataAnnotations;


namespace MatchMaker.Web.Registration
{

//	[CustomValidation()] // Password1 == Password2
	public class RegistrationRequest
	{

//		[Required]
//		[Size(3,20)]
//		[Alphanumeric]
//		[Custom] // User name isn't already in use
		public string UserName
		{
			get;
			set;
		}

//		[AtLeast(5)]
		public string Password1
		{
			get;
			set;
		}

		public string Password2
		{
			get;
			set;
		}

//		[EmailAdress]
//		[CustomValidation()] // Email address isn't already in use
		public string EmailAddress
		{
			get;
			set;
		}

	}

}


