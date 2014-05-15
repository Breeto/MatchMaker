namespace MatchMaker.Web.Common
{

	public class Selectable<T>
	{

		public Selectable()
		{
		}


		public Selectable( T item )
		{
			Item = item;
		}


		public T Item
		{
			get;
			set;
		}

		
		public bool IsSelected
		{
			get;
			set;
		}

	}

}

