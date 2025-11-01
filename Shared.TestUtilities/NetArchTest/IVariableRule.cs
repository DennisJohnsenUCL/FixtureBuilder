using NetArchTest.Rules;

namespace Shared.TestUtilities.NetArchTest
{
	public interface IVariableRule
	{
		TestResult GetResultFor(Types types);
	}
}
