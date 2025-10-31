using Mono.Cecil;
using NetArchTest.Rules;

namespace Shared.TestUtilities.NetArchTest
{
	public class ImplementsInterfaceInHierarchy(Type _interfaceType) : ICustomRule
	{
		public bool MeetsRule(TypeDefinition type)
		{
			while (type != null)
			{
				foreach (var item in type.Interfaces.Select(x => x.InterfaceType))
				{
					if (item.Namespace == _interfaceType.Namespace &&
						item.Name == _interfaceType.Name) return true;
					if (item.GetElementType().FullName == _interfaceType.FullName) return true;
				}
				if (type.BaseType != null)
				{
					try { type = type.BaseType.Resolve(); }
					catch { break; }
				}
				else type = null!;
			}
			return false;
		}
	}
}
