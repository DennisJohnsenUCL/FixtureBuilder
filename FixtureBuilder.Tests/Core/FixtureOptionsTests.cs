using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.Core
{
    internal sealed class FixtureOptionsTests
    {
        #region Instantiation methods

        [Test]
        public void InstantiationMethods_Defaults_NoOverrideAndAutoConstructor()
        {
            var options = new FixtureOptions();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(options.DefaultInstantiationMethod, Is.Null);
                Assert.That(options.DefaultFixtureInstantiationMethod, Is.EqualTo(InstantiationMethod.UseAutoConstructor));
                Assert.That(options.NestedMemberInstantiationMethod, Is.EqualTo(InstantiationMethod.UseAutoConstructor));
                Assert.That(options.DefaultInstantiateInstantiationMethod, Is.EqualTo(InstantiationMethod.UseAutoConstructor));
            }
        }

        [Test]
        public void InstantiationMethods_DefaultInstantiationMethodNotSet_EachReturnsItsOwnValue()
        {
            var options = new FixtureOptions
            {
                DefaultFixtureInstantiationMethod = InstantiationMethod.CreateUninitialized,
                NestedMemberInstantiationMethod = InstantiationMethod.UseDefaultConstructor,
                DefaultInstantiateInstantiationMethod = InstantiationMethod.CreateUninitialized
            };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(options.DefaultFixtureInstantiationMethod, Is.EqualTo(InstantiationMethod.CreateUninitialized));
                Assert.That(options.NestedMemberInstantiationMethod, Is.EqualTo(InstantiationMethod.UseDefaultConstructor));
                Assert.That(options.DefaultInstantiateInstantiationMethod, Is.EqualTo(InstantiationMethod.CreateUninitialized));
            }
        }

        [Test]
        public void InstantiationMethods_DefaultInstantiationMethodSet_AllReturnIt()
        {
            var options = new FixtureOptions
            {
                DefaultFixtureInstantiationMethod = InstantiationMethod.UseAutoConstructor,
                NestedMemberInstantiationMethod = InstantiationMethod.UseDefaultConstructor,
                DefaultInstantiateInstantiationMethod = InstantiationMethod.UseAutoConstructor,
                DefaultInstantiationMethod = InstantiationMethod.CreateUninitialized
            };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(options.DefaultFixtureInstantiationMethod, Is.EqualTo(InstantiationMethod.CreateUninitialized));
                Assert.That(options.NestedMemberInstantiationMethod, Is.EqualTo(InstantiationMethod.CreateUninitialized));
                Assert.That(options.DefaultInstantiateInstantiationMethod, Is.EqualTo(InstantiationMethod.CreateUninitialized));
            }
        }

        #endregion

        #region Instances

        [Test]
        public void Default_ReturnsNewInstanceEachTime()
        {
            var first = FixtureOptions.Default;

            var second = FixtureOptions.Default;

            Assert.That(second, Is.Not.SameAs(first));
        }

        [Test]
        public void Clone_ReturnsSeparateInstanceWithSameValues()
        {
            var options = new FixtureOptions
            {
                DefaultInitializeMembers = InitializeMembers.All,
                AllowPrivateConstructors = false,
                PreferSimplestConstructor = true,
                PreferDefaultParameterValues = false,
                PreferNullParameterValues = false,
                AllowSkipUninitializableMembers = false,
                AllowInstantiateNestedMembers = false,
                DefaultFixtureInstantiationMethod = InstantiationMethod.CreateUninitialized,
                NestedMemberInstantiationMethod = InstantiationMethod.UseDefaultConstructor,
                DefaultInstantiateInstantiationMethod = InstantiationMethod.CreateUninitialized,
                AllowImplicitConstruction = false,
                AllowResolveCircularDependencies = true,
                AllowImplicitConversion = true
            };

            var clone = options.Clone();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(clone, Is.Not.SameAs(options));
                Assert.That(clone, Is.EqualTo(options).UsingPropertiesComparer());
            }
        }

        [Test]
        public void Clone_DefaultInstantiationMethodSet_CopiesIt()
        {
            var options = new FixtureOptions { DefaultInstantiationMethod = InstantiationMethod.CreateUninitialized };

            var clone = options.Clone();

            Assert.That(clone.DefaultInstantiationMethod, Is.EqualTo(InstantiationMethod.CreateUninitialized));
        }

        [Test]
        public void Clone_ChangingClone_LeavesOriginalUnchanged()
        {
            var options = new FixtureOptions();
            var clone = options.Clone();

            clone.AllowPrivateConstructors = false;

            Assert.That(options.AllowPrivateConstructors, Is.True);
        }

        #endregion
    }
}
