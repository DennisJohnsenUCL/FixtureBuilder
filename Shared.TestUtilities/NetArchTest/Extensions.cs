using NetArchTest.Rules;

namespace Shared.TestUtilities.NetArchTest
{
	public static class Extensions
	{
		public static PredicateList ImplementInterfaceInHierarchy(this Predicate predicates, Type interfaceType)
		{
			return predicates.MeetCustomRule(new ImplementsInterfaceInHierarchy(interfaceType));
		}

		public static PredicateList ImplementInterfaceInHierarchy<T>(this Predicate predicates)
		{
			return predicates.MeetCustomRule(new ImplementsInterfaceInHierarchy(typeof(T)));
		}
	}
}
