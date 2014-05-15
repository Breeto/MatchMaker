using System;
using System.Collections.Concurrent;


namespace MatchMaker.Common
{

	/// <summary>
	/// Used for caching data that is static and referenced frequently.
	/// Intended for reducing excessive calls to the database.
	/// </summary>
	public class IdentityMap<TEntity>
	{

		private readonly Func<Guid, TEntity> provider;
		private readonly ConcurrentDictionary<Guid, TEntity> map = new ConcurrentDictionary<Guid, TEntity>();


		public IdentityMap(Func<Guid, TEntity> provider)
		{
			this.provider = provider;
		}


		public TEntity Lookup( Guid id )
		{
			return map.GetOrAdd( id, provider );
		}
	 
	}

}