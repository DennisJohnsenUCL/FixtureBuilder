#pragma warning disable IDE0060

using System.Reflection;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.ParameterProviders;
using Moq;

namespace FixtureBuilder.Tests.ParameterProviders
{
    [TestFixture]
    internal class AutoParameterProviderTests
    {
        private AutoParameterProvider _sut;
        private Mock<IFixtureContext> _contextMock;
        private RecursiveResolveContext _recursiveResolveContext;

        [SetUp]
        public void SetUp()
        {
            _sut = new AutoParameterProvider();
            _contextMock = new Mock<IFixtureContext>();
            _recursiveResolveContext = new();
        }

        #region Helper classes for parameter extraction

        private static class ParameterSource
        {
            public static void WithDefault(int value = 42) { }
            public static void WithNullDefault(string? value = null) { }
            public static void WithNullableRefType(string? value) { }
            public static void WithNullableValueType(int? value) { }
            public static void WithNonNullableRefType(string value) { }
            public static void WithNonNullableValueType(int value) { }
        }

        private static ParameterInfo GetParam(string methodName)
        {
            return typeof(ParameterSource)
                .GetMethod(methodName, BindingFlags.Static | BindingFlags.Public)!
                .GetParameters()[0];
        }

        #endregion

        #region Default value tests

        [Test]
        public void ResolveParameterValue_ParameterHasDefaultValue_ReturnsDefault()
        {
            var paramInfo = GetParam(nameof(ParameterSource.WithDefault));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = _sut.ResolveParameterValue(paramInfo, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void ResolveParameterValue_DoNotPreferDefaultValue_ParameterHasDefaultValue_ReturnsNotDefault()
        {
            var expected = "test return value";
            var paramInfo = GetParam(nameof(ParameterSource.WithDefault));
            var options = new FixtureOptions { PreferDefaultParameterValues = false };
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(expected);

            var result = _sut.ResolveParameterValue(paramInfo, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ResolveParameterValue_ParameterHasNullDefault_ReturnsNull()
        {
            var paramInfo = GetParam(nameof(ParameterSource.WithNullDefault));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = _sut.ResolveParameterValue(paramInfo, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.Null);
        }

        #endregion

        #region Nullable parameter tests

        [Test]
        public void ResolveParameterValue_NullNotPreferred_NullableType_ReturnsNotNull()
        {
            var expected = "test value string";
            var paramInfo = GetParam(nameof(ParameterSource.WithNullableRefType));
            var options = new FixtureOptions() { PreferNullParameterValues = false };
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>())).Returns(expected);

            var result = _sut.ResolveParameterValue(paramInfo, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ResolveParameterValue_NullableReferenceType_ReturnsNull()
        {
            var paramInfo = GetParam(nameof(ParameterSource.WithNullableRefType));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = _sut.ResolveParameterValue(paramInfo, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveParameterValue_NullableValueType_ReturnsNull()
        {
            var paramInfo = GetParam(nameof(ParameterSource.WithNullableValueType));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = _sut.ResolveParameterValue(paramInfo, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.Null);
        }

        #endregion

        #region Context resolution tests

        [Test]
        public void ResolveParameterValue_LinkReturnsSubstitute_UsesLinkedType()
        {
            var paramInfo = GetParam(nameof(ParameterSource.WithNonNullableRefType));
            var linkedType = typeof(object);
            _contextMock.Setup(c => c.Link(typeof(string))).Returns(linkedType);
            _contextMock.Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns("resolved");

            _sut.ResolveParameterValue(paramInfo, _contextMock.Object, _recursiveResolveContext);

            _contextMock.Verify(c => c.ResolveValue(
                It.Is<FixtureRequest>(r => r.Type == linkedType),
                It.IsAny<IFixtureContext>()), Times.Once);
        }

        [Test]
        public void ResolveParameterValue_LinkReturnsNull_UsesOriginalType()
        {
            var paramInfo = GetParam(nameof(ParameterSource.WithNonNullableRefType));
            _contextMock.Setup(c => c.Link(typeof(string))).Returns((Type?)null);
            _contextMock.Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns("resolved");

            _sut.ResolveParameterValue(paramInfo, _contextMock.Object, _recursiveResolveContext);

            _contextMock.Verify(c => c.ResolveValue(
                It.Is<FixtureRequest>(r => r.Type == typeof(string)),
                It.IsAny<IFixtureContext>()), Times.Once);
        }

        [Test]
        public void ResolveParameterValue_ResolveValueReturnsNonNull_ReturnsThatValue()
        {
            var paramInfo = GetParam(nameof(ParameterSource.WithNonNullableRefType));
            var expected = "resolved-value";
            _contextMock.Setup(c => c.Link(It.IsAny<Type>())).Returns((Type?)null);
            _contextMock.Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(expected);

            var result = _sut.ResolveParameterValue(paramInfo, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ResolveParameterValue_ResolveValueReturnsNonNull_DoesNotCallAutoResolve()
        {
            var paramInfo = GetParam(nameof(ParameterSource.WithNonNullableRefType));
            _contextMock.Setup(c => c.Link(It.IsAny<Type>())).Returns((Type?)null);
            _contextMock.Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns("resolved");

            _sut.ResolveParameterValue(paramInfo, _contextMock.Object, _recursiveResolveContext);

            _contextMock.Verify(c => c.AutoResolve(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>(), It.IsAny<RecursiveResolveContext>()), Times.Never);
        }

        #endregion

        [Test]
        public void ResolveParameterValue_ResolveValueReturnsNull_FallsBackToAutoResolve()
        {
            var paramInfo = GetParam(nameof(ParameterSource.WithNonNullableRefType));
            var expected = "auto-resolved";
            _contextMock.Setup(c => c.Link(It.IsAny<Type>())).Returns((Type?)null);
            _contextMock.Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns((object?)null);
            _contextMock.Setup(c => c.AutoResolve(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>(), It.IsAny<RecursiveResolveContext>()))
                .Returns(expected);

            var result = _sut.ResolveParameterValue(paramInfo, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ResolveParameterValue_NonNullableValueType_ProceedsToContextResolution()
        {
            var paramInfo = GetParam(nameof(ParameterSource.WithNonNullableValueType));
            _contextMock.Setup(c => c.Link(typeof(int))).Returns((Type?)null);
            _contextMock.Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(99);

            var result = _sut.ResolveParameterValue(paramInfo, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.EqualTo(99));
        }
    }
}
