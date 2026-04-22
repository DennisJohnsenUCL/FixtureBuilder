using System.Reflection;
using System.Runtime.CompilerServices;

namespace FixtureBuilder.Creation
{
    internal class RecursiveResolveContext
    {
        private readonly HashSet<Type> _types;
        private List<(Type Type, object Shell)> _shells;

        public RecursiveResolveContext()
        {
            _types = [];
            _shells = [];
        }

        private RecursiveResolveContext(HashSet<Type> types, List<(Type Type, object Shell)> shells)
        {
            _types = types;
            _shells = shells;
        }

        public RecursiveResolveContext Branch()
        {
            return new([.. _types], _shells);
        }

        public bool Add(Type type)
        {
            return _types.Add(type);
        }

        public object AddShell(Type type)
        {
            try
            {
                var shell = RuntimeHelpers.GetUninitializedObject(type);
                _shells.Add((type, shell));
                return shell;
            }
            catch (Exception ex) { throw new InvalidOperationException("Failed to create shell object for resolving circular dependency, see inner exception.", ex); }
        }

        public void Copy(Type type, object obj)
        {
            var shells = _shells.Where(x => x.Type == type);
            if (!shells.Any()) return;

            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var value = field.GetValue(obj);
                foreach (var shell in shells)
                {
                    field.SetValue(shell.Shell, value);
                }
            }
            _shells = [.. _shells.Except(shells)];
        }
    }
}
