using System;
using System.Collections.Generic;


namespace MatchMaker.Common
{

	[Serializable]
	public struct Range<T> where T : IComparable<T>
	{

		private static readonly EqualityComparer<T> EqualityComparer = EqualityComparer<T>.Default;

		private T min;
		private T max;


		public Range( T min, T max )
		{
			this.min = min;
			this.max = max;
		}


		public T Min
		{
			get
			{
				return min;
			}
			set
			{
				min = value;
			}
		}


		public T Max
		{
			get
			{
				return max;
			}
			set
			{
				max = value;
			}
		}


		public bool Contains( T value )
		{
			return
				min.CompareTo( value ) <= 0 &&
				max.CompareTo( value ) >= 0;
		}


		public bool Contains( Range<T> value )
		{
			return ( min.CompareTo( value.max ) <= 0 && max.CompareTo( value.min ) >= 0 );
		}


		public T Constrain( T comparable )
		{
			if ( min.CompareTo( comparable ) > 0 )
			{
				return min;
			}

			if ( max.CompareTo( comparable ) < 0 )
			{
				return max;
			}

			return comparable;
		}


		public override string ToString()
		{
			return string.Format( "{0} - {1}", min, max );
		}


		public override bool Equals( object obj )
		{
			if ( !( obj is Range<T> ) )
			{
				return false;
			}

			var otherRange = (Range<T>) obj;

			// don't use == operator to save method call and extra stack frame
			return EqualityComparer.Equals( min, otherRange.min ) && EqualityComparer.Equals( max, otherRange.max );
		}


		// This override calls the base implementation because since we're usually supposed
		// to override GetHashCode any time we override Equals, this is a special case because
		// this object is mutable.  I'm not sure at this point if this is okay or a bad thing!
		public override int GetHashCode()
		{
			unchecked
			{
				// 17 and 23 are not magic... really just need a pair of prime numbers
				int hash = 17;
				hash = hash * 23 + min.GetHashCode();
				hash = hash * 23 + max.GetHashCode();
				return hash;
			}
		}


		public static bool operator !=( Range<T> r1, Range<T> r2 )
		{
			return !EqualityComparer.Equals( r1.min, r2.min ) || EqualityComparer.Equals( r1.max, r2.max );
		}


		public static bool operator ==( Range<T> r1, Range<T> r2 )
		{
			return EqualityComparer.Equals( r1.min, r2.min ) && EqualityComparer.Equals( r1.max, r2.max );
		}

	}

}
