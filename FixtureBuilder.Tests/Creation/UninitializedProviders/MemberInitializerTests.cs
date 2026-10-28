#pragma warning disable CS0649

using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Creation.UninitializedProviders
{
    internal sealed class MemberInitializerTests
    {
        private readonly Type _rootType = typeof(object);

        private Mock<IDataMemberSkipFilter> _skipFilter;
        private Mock<IUninitializedProvider> _uninitializedProvider;
        private Mock<IFixtureContext> _context;
        private RecursiveResolveContext _recursiveResolveContext;
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

        public class TestClassWithInterfaceProperty
        {
            public IList<int>? Items { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            _skipFilter = new Mock<IDataMemberSkipFilter>();
            _uninitializedProvider = new Mock<IUninitializedProvider>();
            _context = new Mock<IFixtureContext>();
            _context.Setup(c => c.UnwrapAndLink(It.IsAny<Type>(), It.IsAny<Type>())).Returns((Type type, Type _) => type);
            _sut = new MemberInitializer(_skipFilter.Object, _uninitializedProvider.Object);
            _recursiveResolveContext = new();
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
            var context = _context.Object;
            var initMembers = InitializeMembers.All;
            var expectedValue = "resolved";

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context, It.IsAny<RecursiveResolveContext>()))
                .Returns(expectedValue);

            _sut.InitializeMembers(instance, initMembers, _rootType, context, _recursiveResolveContext);

            var result = instance.Name;

            Assert.That(result, Is.EqualTo(expectedValue));
        }

        [Test]
        public void InitializeMembers_PropertyIsAlreadySet_DoesNotCallResolve()
        {
            var instance = new TestClassWithProperty { Name = "already set" };
            var context = _context.Object;
            var initMembers = InitializeMembers.All;

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _sut.InitializeMembers(instance, initMembers, _rootType, context, _recursiveResolveContext);

            _uninitializedProvider.Verify(
                p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), It.IsAny<InitializeMembers>(), It.IsAny<IFixtureContext>(), It.IsAny<RecursiveResolveContext>()),
                Times.Never);
        }

        [Test]
        public void InitializeMembers_SkipFilterReturnsTrueForOneMember_StillProcessesOtherMembers()
        {
            var instance = new TestClassWithMultipleProperties();
            var context = _context.Object;
            var initMembers = InitializeMembers.All;
            var skipCount = 0;

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(() => skipCount++ == 0);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context, It.IsAny<RecursiveResolveContext>()))
                .Returns("resolved");

            _sut.InitializeMembers(instance, initMembers, _rootType, context, _recursiveResolveContext);

            _uninitializedProvider.Verify(
                p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context, It.IsAny<RecursiveResolveContext>()),
                Times.AtLeastOnce);
        }

        [Test]
        public void InitializeMembers_FieldIsNull_ResolvesAndSetsValue()
        {
            var instance = new TestClassWithField();
            var context = _context.Object;
            var initMembers = InitializeMembers.All;
            var expectedValue = "field resolved";

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context, It.IsAny<RecursiveResolveContext>()))
                .Returns(expectedValue);

            _sut.InitializeMembers(instance, initMembers, _rootType, context, _recursiveResolveContext);

            var result = instance.Value;

            Assert.That(result, Is.EqualTo(expectedValue));
        }

        [Test]
        public void InitializeMembers_ResolveReturnsNull_DoesNotSetValue()
        {
            var instance = new TestClassWithProperty();
            var context = _context.Object;
            var initMembers = InitializeMembers.All;

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _sut.InitializeMembers(instance, initMembers, _rootType, context, _recursiveResolveContext);

            var result = instance.Name;

            Assert.That(result, Is.Null);
        }

        [Test]
        public void InitializeMembers_WithMultipleNullProperties_ResolvesEach()
        {
            var instance = new TestClassWithMultipleProperties();
            var context = _context.Object;
            var initMembers = InitializeMembers.All;
            var callCount = 0;

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context, It.IsAny<RecursiveResolveContext>()))
                .Returns(() => $"value{++callCount}");

            _sut.InitializeMembers(instance, initMembers, _rootType, context, _recursiveResolveContext);

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
            var context = _context.Object;
            var initMembers = InitializeMembers.All;
            FixtureRequest? capturedRequest = null;

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context, It.IsAny<RecursiveResolveContext>()))
                .Callback<FixtureRequest, InitializeMembers, IFixtureContext, RecursiveResolveContext>((req, _, _, _) => capturedRequest = req)
                .Returns("value");

            _sut.InitializeMembers(instance, initMembers, _rootType, context, _recursiveResolveContext);

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
            var context = _context.Object;
            var initMembers = InitializeMembers.All;

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context, It.IsAny<RecursiveResolveContext>()))
                .Returns(42);

            _sut.InitializeMembers(instance, initMembers, _rootType, context, _recursiveResolveContext);

            var result = instance.Count;

            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void InitializeMembers_PassesLinkedTypeToRequest()
        {
            var instance = new TestClassWithInterfaceProperty();
            var context = _context.Object;
            var initMembers = InitializeMembers.All;

            _context
                .Setup(c => c.UnwrapAndLink(typeof(IList<int>), _rootType))
                .Returns(typeof(List<int>));

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context, It.IsAny<RecursiveResolveContext>()))
                .Returns(new List<int>());

            _sut.InitializeMembers(instance, initMembers, _rootType, context, _recursiveResolveContext);

            _uninitializedProvider.Verify(
                p => p.ResolveUninitialized(It.Is<FixtureRequest>(r => r.Type == typeof(List<int>)), initMembers, context, It.IsAny<RecursiveResolveContext>()),
                Times.Once);
        }

        #endregion

        [Test]
        public void InitializeMembers_PassesRootTypeToRequest()
        {
            var instance = new TestClassWithProperty();
            var context = _context.Object;
            var initMembers = InitializeMembers.All;
            var rootType = typeof(string);
            FixtureRequest? capturedRequest = null;

            _skipFilter
                .Setup(f => f.ShouldSkip(It.IsAny<DataMemberInfo>(), initMembers))
                .Returns(false);

            _uninitializedProvider
                .Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), initMembers, context, It.IsAny<RecursiveResolveContext>()))
                .Callback<FixtureRequest, InitializeMembers, IFixtureContext, RecursiveResolveContext>((req, _, _, _) => capturedRequest = req)
                .Returns("value");

            _sut.InitializeMembers(instance, initMembers, rootType, context, _recursiveResolveContext);

            Assert.That(capturedRequest!.RootType, Is.EqualTo(typeof(string)));
        }
    }
}
