using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RSM.Service.Library.Extensions
{
	public static class Linq
	{
		static readonly Random Randomizer = new Random((int)DateTime.Now.Ticks);

		public static IQueryable<T> ConditionalWhere<T>(this IQueryable<T> source, bool includePredicate, Expression<Func<T, bool>> predicate)
		{
			return (includePredicate) ? source.Where(predicate) : source;
		}

		public static T RandomElement<T>(this IEnumerable<T> source)
		{
			T current = default(T);

			var c = source.Count();
			var r = Randomizer.Next(c);

			current = source.Skip(r).First();
			
			return current;
		}

		public static IEnumerable<T> RandomElements<T>(this IEnumerable<T> source, int number)
		{
			return source.OrderBy(r => Randomizer.Next()).Take(number);
		}
	}
}
