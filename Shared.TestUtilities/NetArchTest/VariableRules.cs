using NetArchTest.Rules;

namespace Shared.TestUtilities.NetArchTest
{
	public class NamingRule(Type _interfaceType, string _expectedName) : IVariableRule
	{
		public TestResult GetResultFor(Types types)
		{
			return types
				.That()
				.AreNotAbstract()
				.And()
				.ImplementInterfaceInHierarchy(_interfaceType)
				.Should()
				.HaveNameEndingWith(_expectedName)
				.GetResult();
		}
	}

	public class NamespaceRule(Type _interfaceType, string _expectedNamespace) : IVariableRule
	{
		public TestResult GetResultFor(Types types)
		{
			return types
				.That()
				.AreNotAbstract()
				.And()
				.ImplementInterfaceInHierarchy(_interfaceType)
				.Should()
				.ResideInNamespace(_expectedNamespace)
				.GetResult();
		}
	}

	internal class EntityFrameworkLeakageRule(string _namespace) : IVariableRule
	{
		public TestResult GetResultFor(Types types)
		{
			return types
				.That()
				.ResideInNamespace(_namespace)
				.ShouldNot()
				.HaveDependencyOnAny("Microsoft.EntityFrameworkCore")
				.GetResult();
		}
	}

	internal class DependencyInjectionLeakageRule(string _namespace) : IVariableRule
	{
		public TestResult GetResultFor(Types types)
		{
			return types
				.That()
				.ResideInNamespace(_namespace)
				.ShouldNot()
				.HaveDependencyOnAny("Microsoft.Extensions.DependencyInjection")
				.GetResult();
		}
	}
}
