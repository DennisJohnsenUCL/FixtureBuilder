using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using FixtureBuilder.Core;

namespace FixtureBuilder.Tests
{
    internal static class TestHelper
    {
        /// <summary>
        /// Creates a fixture on an uninitialized <typeparamref name="T"/>, ready for configuration methods.
        /// </summary>
        internal static IFixtureConfigurator<T> MakeFixture<T>() where T : class
        {
            var instance = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
            return Fixture.New(instance);
        }

        /// <summary>
        /// Creates a fixture from <paramref name="factory"/> on an uninitialized <typeparamref name="T"/>, ready for configuration methods.
        /// </summary>
        internal static IFixtureConfigurator<T> MakeFixture<T>(FixtureFactory factory) where T : class
        {
            var instance = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
            return factory.New(instance);
        }

        /// <summary>
        /// Creates a request for <paramref name="type"/> whose source is a field named <c>Value</c> declared as
        /// <paramref name="type"/>, with <paramref name="type"/> as the root type.
        /// </summary>
        internal static FixtureRequest MakeRequest(Type type)
        {
            var source = typeof(RequestSource<>).MakeGenericType(type).GetField(nameof(RequestSource<>.Value))!;
            return new FixtureRequest(type, source, type);
        }

        /// <summary>
        /// Creates a request whose source is <paramref name="parameter"/>, with the type declaring the parameter's method as the root type.
        /// </summary>
        internal static FixtureRequest MakeRequest(ParameterInfo parameter)
            => new(parameter.ParameterType, parameter, parameter.Member.DeclaringType!);

        /// <summary>
        /// Gets the public instance property <paramref name="name"/> of <paramref name="type"/> as a data member.
        /// </summary>
        internal static DataMemberInfo PropertyMember(Type type, string name)
            => new(type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance)!);

        /// <summary>
        /// Gets the public instance field <paramref name="name"/> of <paramref name="type"/> as a data member.
        /// </summary>
        internal static DataMemberInfo FieldMember(Type type, string name)
            => new(type.GetField(name, BindingFlags.Public | BindingFlags.Instance)!);

        /// <summary>
        /// Gets the first parameter of <paramref name="method"/>.
        /// </summary>
        internal static ParameterInfo FirstParameter(Delegate method)
            => method.Method.GetParameters()[0];

        /// <summary>
        /// Gets a parameter of type <paramref name="type"/> that has no name.
        /// </summary>
        internal static ParameterInfo UnnamedParameter(Type type)
            => new DynamicMethod("Unnamed", null, [type]).GetParameters()[0];

        private class RequestSource<T>
        {
            public T Value = default!;
        }
    }
}
