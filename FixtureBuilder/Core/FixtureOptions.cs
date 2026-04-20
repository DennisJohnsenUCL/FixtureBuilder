using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Core
{
    public class FixtureOptions
    {
        public FixtureOptions() { }

        public static FixtureOptions Default => new();

        /// <summary>
        /// The default InitializeMembers setting used by CreateUninitialized.
        /// </summary>
        /// <remarks>
        /// Default is <see cref="InitializeMembers.None" />
        /// </remarks>
        public InitializeMembers DefaultInitializeMembers { get; set; } = InitializeMembers.None;

        /// <summary>
        /// Whether UseAutoConstructor will use private constructors.
        /// </summary>
        /// <remarks>
        /// Default is <see langword="true" /> <br /><br />
        /// Public constructors will still be preferred. Set PreferSimplestConstructor to always choose simplest constructor regardless of visibility.
        /// </remarks>
        public bool AllowPrivateConstructors { get; set; } = true;

        /// <summary>
        /// Whether UseAutoConstructor will always prefer the simplest constructor regardless of visibility.
        /// </summary>
        /// <remarks>
        /// Default is <see langword="false" /> <br /><br />
        /// AllowPrivateConstructors must be true for private constructors to be considered, even if simpler.
        /// </remarks>
        public bool PreferSimplestConstructor { get; set; } = false;

        /// <summary>
        /// Whether default values for parameters will be used or ignored by UseAutoConstructor.
        /// </summary>
        /// <remarks>
        /// Default is <see langword="true" />
        /// </remarks>
        public bool PreferDefaultParameterValues { get; set; } = true;

        /// <summary>
        /// Whether null will be used for nullable parameters by UseAutoConstructor.
        /// </summary>
        /// <remarks>
        /// Default is <see langword="true" />
        /// </remarks>
        public bool PreferNullParameterValues { get; set; } = true;

        /// <summary>
        /// Whether CreateUninitialized with InitializeMembers on is allowed to assign null for fields it cannot initialize.
        /// </summary>
        /// <remarks>
        /// Default is <see langword="true" />
        /// </remarks>
        public bool AllowSkipUninitializableMembers { get; set; } = true;

        /// <summary>
        /// Whether nested members can be instantiated if null when trying to access them or a deeper nested member.
        /// </summary>
        /// <remarks>
        /// Default is <see langword="true" />
        /// <br /><br />If <see langword="false"/>, an Expression such as c => c.Child.GrandChild will throw if Child is null.
        /// <br />Used by all methods that take an Expression Func or Expression Action
        /// </remarks>
        public bool AllowInstantiateNestedMembers { get; set; } = true;

        /// <summary>
        /// Which instantiation method will be used to instantiate the fixture if none is explicitly chosen.
        /// </summary>
        /// <remarks>
        /// Default is <see cref="InstantiationMethod.UseAutoConstructor" />
        /// <br /><br />This is triggered when the construction stage is skipped (<c>Fixture.New&lt;T&gt;().WithSetter(..)</c>).
        /// </remarks>
        public InstantiationMethod DefaultInstantiationMethod { get; set; } = InstantiationMethod.UseAutoConstructor;

        /// <summary>
        /// Which instantiation method will be used to instantiate nested members when traversing a member chain.
        /// </summary>
        /// <remarks>
        /// Default is <see cref="InstantiationMethod.UseAutoConstructor" />
        /// <br /><br />This is triggered when a null member is encountered when traversing a member chain (<c>x => x.Child.GrandChild</c>).
        /// <br />This option only applies when <see cref="AllowInstantiateNestedMembers"/> is set to true, otherwise an exception will be thrown.
        /// </remarks>
        public InstantiationMethod NestedMemberInstantiationMethod { get; set; } = InstantiationMethod.UseAutoConstructor;

        /// <summary>
        /// Whether the construction stage can be skipped, instead using the <see cref="DefaultInstantiationMethod"/>
        /// </summary>
        /// <remarks>
        /// Default is <see langword="true" />
        /// <br /><br />When <see cref="true"/>, the fixture can be configured without first explicitly calling one of the construction methods.
        /// </remarks>
        public bool AllowImplicitConstruction { get; set; } = true;

        /// <summary>
        /// Which instantiation method will be used to instantiate the member if one is not explicitly chosen when calling .Instantiate.
        /// </summary>
        /// <remarks>
        /// Default is <see cref="InstantiationMethod.UseAutoConstructor" />
        /// <br /><br />This is triggered when Instantiate is called without specifying an explicit instantiation method. (<c>fixture.Instantiate(x => x.Name)</c>).
        /// </remarks>
        public InstantiationMethod DefaultInstantiateInstantiationMethod { get; set; } = InstantiationMethod.UseAutoConstructor;
    }
}
