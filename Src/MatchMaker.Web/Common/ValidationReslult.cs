using System.Collections.Generic;


namespace MatchMaker.Web.Common
{

	public class ValidationReslult
	{

		private readonly List<string> errors = new List<string>();


		public ValidationReslult() : this(true)
		{
		}


		public ValidationReslult( bool isValid )
		{
			IsValid = isValid;
		}


		public bool IsValid
		{
			get;
			set;
		}


		public string[] ValidationErrors
		{
			get
			{
				return errors.ToArray();
			}
		}


		public void AddValidationError( string error )
		{
			errors.Add( error );

			IsValid = false;
		}

	}

}