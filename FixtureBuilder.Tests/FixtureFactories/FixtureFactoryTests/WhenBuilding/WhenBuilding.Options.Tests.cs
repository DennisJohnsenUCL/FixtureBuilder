using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests.WhenBuilding
{
    internal sealed class WhenBuildingOptionsTests
    {
        private class PublicAndPrivateConstructorClass
        {
            public string Name { get; }
            public string UsedConstructor { get; }

            public PublicAndPrivateConstructorClass(string name, int age)
            {
                Name = name;
                UsedConstructor = "public";
            }

            private PublicAndPrivateConstructorClass(string name)
            {
                Name = name;
                UsedConstructor = "private";
            }
        }

        #region SetOptions

        private class NullableAndDefaultParameterClass(int? count, int size = 99)
        {
            public int? Count { get; } = count;
            public int Size { get; } = size;
        }

        [Test]
        public void SetOptions_RootType_AppliesOptions()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<PrivateConstructorClass>(builder => builder.SetOptions(options => options.AllowPrivateConstructors = false));
            var fixture = factory.New<PrivateConstructorClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.Build());
        }

        [Test]
        public void SetOptions_OtherRootType_DoesNotApplyOptions()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringConstructorClass>(builder => builder.SetOptions(options => options.AllowPrivateConstructors = false));

            var result = factory.New<PrivateConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void SetOptions_StartsFromFactoryOptions()
        {
            var factory = new FixtureFactory()
                .SetOptions(options => options.PreferSimplestConstructor = true)
                .WhenBuilding<PublicAndPrivateConstructorClass>(builder => builder.SetOptions(options => options.PreferNullParameterValues = false));

            var result = factory.New<PublicAndPrivateConstructorClass>().Build();

            Assert.That(result.UsedConstructor, Is.EqualTo("private"));
        }

        [Test]
        public void SetOptions_DoesNotChangeFactoryOptions()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<PrivateConstructorClass>(builder => builder.SetOptions(options => options.AllowPrivateConstructors = false));

            Assert.That(factory.Options.AllowPrivateConstructors, Is.True);
        }

        [Test]
        public void SetOptions_ChainedCalls_AllApply()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<NullableAndDefaultParameterClass>(builder => builder
                    .SetOptions(options => options.PreferNullParameterValues = false)
                    .SetOptions(options => options.PreferDefaultParameterValues = false));

            var result = factory.New<NullableAndDefaultParameterClass>().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Count, Is.InRange(1, 10));
                Assert.That(result.Size, Is.InRange(1, 10));
            }
        }

        [Test]
        public void SetOptions_SeparateCallsForSameRootType_AllApply()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<NullableAndDefaultParameterClass>(builder => builder.SetOptions(options => options.PreferNullParameterValues = false))
                .WhenBuilding<NullableAndDefaultParameterClass>(builder => builder.SetOptions(options => options.PreferDefaultParameterValues = false));

            var result = factory.New<NullableAndDefaultParameterClass>().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Count, Is.InRange(1, 10));
                Assert.That(result.Size, Is.InRange(1, 10));
            }
        }

        [Test]
        public void SetOptions_NullAction_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<StringConstructorClass>(builder => builder.SetOptions(null!)));
        }

        #endregion

        #region Options setter

        [Test]
        public void OptionsSetter_RootType_AppliesOptions()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<PrivateConstructorClass>(builder => builder.Options = new FixtureOptions { AllowPrivateConstructors = false });
            var fixture = factory.New<PrivateConstructorClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.Build());
        }

        [Test]
        public void OptionsSetter_Null_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<StringConstructorClass>(builder => builder.Options = null!));
        }

        #endregion

        #region Combined with values

        [Test]
        public void OptionsAndValuesInOneCall_BothApply()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<PublicAndPrivateConstructorClass>(builder => builder
                    .SetOptions(options => options.PreferSimplestConstructor = true)
                    .WithParameter("scoped"));

            var result = factory.New<PublicAndPrivateConstructorClass>().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("scoped"));
                Assert.That(result.UsedConstructor, Is.EqualTo("private"));
            }
        }

        #endregion
    }
}
