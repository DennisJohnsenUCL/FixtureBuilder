using FixtureBuilder.FixtureContexts;
using FixtureBuilder.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.UninitializedProviders
{
    internal sealed class MemberInitializerTests
    {
        private Mock<IDataMemberSkipFilter> _skipFilter;
        private Mock<IFixtureUninitializedProvider> _uninitializedProvider;
        private MemberInitializer _sut;

        public class TestClassWithProperty
        {
            public string? Name { get; set; }
        }

        public class TestClassWithField
        {
            public string? Value;
        }

        public class TestClassWithMultipleProperties
        {
            public string? First { get; set; }
            public string? Second { get; set; }
        }

        public class TestClassWithValueType
        {
            public int Count { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            _skipFilter = new Mock<IDataMemberSkipFilter>();
            _uninitializedProvider = new Mock<IFixtureUninitializedProvider>();
            _sut = new MemberInitializer(_skipFilter.Object, _uninitializedProvider.Object);
        }

        #region Constructor

        [Test]
        public void Constructor_WithValidArguments_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new MemberInitializer(_skipFilter.Object, _uninitializedProvider.Object));
        }

        [Test]
        public void Constructor_WithNullSkipFilter_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new MemberInitializer(null!, _uninitializedProvider.Object));

            Assert.That(ex!.ParamName, Is.EqualTo("skipFilter"));
        }

        [Test]
        public void Constructor_WithNullUninitializedProvider_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new MemberInitializer(_skipFilter.Object, null!));

            Assert.That(ex!.ParamName, Is.EqualTo("uninitializedProvider"));
        }

        #endregion

        #region InitializeMembers

        [Test]
        public void InitializeMembers_PropertyIsNull_ResolvesAndSetsValue()
        {
            var instance = new TestClassWithProperty();
            var context = Mock.Of<IFixtureContext>();
            var initMembers = InitializeMembers.All;
            var expectedValue = "resolved";

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context))
                .Returns(expectedValue);

            _sut.InitializeMembers(instance, initMembers, context);

            var result = instance.Name;

            Assert.That(result, Is.EqualTo(expectedValue));
        }

        [Test]
        public void InitializeMembers_PropertyIsAlreadySet_DoesNotCallResolve()
        {
            var instance = new TestClassWithProperty { Name = "already set" };
            var context = Mock.Of<IFixtureContext>();
            var initMembers = InitializeMembers.All;

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _sut.InitializeMembers(instance, initMembers, context);

            _uninitializedProvider.Verify(
                p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), It.IsAny<InitializeMembers>(), It.IsAny<IFixtureContext>()),
                Times.Never);
        }

        [Test]
        public void InitializeMembers_SkipFilterReturnsTrueForOneMember_StillProcessesOtherMembers()
        {
            var instance = new TestClassWithMultipleProperties();
            var context = Mock.Of<IFixtureContext>();
            var initMembers = InitializeMembers.All;
            var skipCount = 0;

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(() => skipCount++ == 0);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context))
                .Returns("resolved");

            _sut.InitializeMembers(instance, initMembers, context);

            _uninitializedProvider.Verify(
                p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context),
                Times.AtLeastOnce);
        }

        [Test]
        public void InitializeMembers_FieldIsNull_ResolvesAndSetsValue()
        {
            var instance = new TestClassWithField();
            var context = Mock.Of<IFixtureContext>();
            var initMembers = InitializeMembers.All;
            var expectedValue = "field resolved";

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context))
                .Returns(expectedValue);

            _sut.InitializeMembers(instance, initMembers, context);

            var result = instance.Value;

            Assert.That(result, Is.EqualTo(expectedValue));
        }

        [Test]
        public void InitializeMembers_ResolveReturnsNull_DoesNotSetValue()
        {
            var instance = new TestClassWithProperty();
            var context = Mock.Of<IFixtureContext>();
            var initMembers = InitializeMembers.All;

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context))
                .Returns((object?)null);

            _sut.InitializeMembers(instance, initMembers, context);

            var result = instance.Name;

            Assert.That(result, Is.Null);
        }

        [Test]
        public void InitializeMembers_WithMultipleNullProperties_ResolvesEach()
        {
            var instance = new TestClassWithMultipleProperties();
            var context = Mock.Of<IFixtureContext>();
            var initMembers = InitializeMembers.All;
            var callCount = 0;

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context))
                .Returns(() => $"value{++callCount}");

            _sut.InitializeMembers(instance, initMembers, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance.First, Is.Not.Null);
                Assert.That(instance.Second, Is.Not.Null);
            }
        }

        [Test]
        public void InitializeMembers_PassesCorrectRequestType_ForProperty()
        {
            var instance = new TestClassWithProperty();
            var context = Mock.Of<IFixtureContext>();
            var initMembers = InitializeMembers.All;
            FixtureRequest? capturedRequest = null;

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context))
                .Callback<FixtureRequest, InitializeMembers, IFixtureContext>((req, _, _) => capturedRequest = req)
                .Returns("value");

            _sut.InitializeMembers(instance, initMembers, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(capturedRequest, Is.Not.Null);
                Assert.That(capturedRequest!.Type, Is.EqualTo(typeof(string)));
                Assert.That(capturedRequest.Name, Is.EqualTo(nameof(TestClassWithProperty.Name)));
            }
        }

        [Test]
        public void InitializeMembers_DefaultValueTypeProperty_ResolvesValue()
        {
            var instance = new TestClassWithValueType();
            var context = Mock.Of<IFixtureContext>();
            var initMembers = InitializeMembers.All;

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context))
                .Returns(42);

            _sut.InitializeMembers(instance, initMembers, context);

            var result = instance.Count;

            Assert.That(result, Is.EqualTo(42));
        }

        #endregion
    }
}
