using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class InstantiateFixtureTests
    {
        [Test]
        public void InstantiateFixture_PassesOptionsToContext()
        {
            var options = new FixtureOptions
            {
                DefaultInstantiationMethod = InstantiationMethod.CreateUninitialized,
                DefaultInitializeMembers = InitializeMembers.All
            };
            var contextMock = new Mock<IFixtureContext>();
            contextMock.Setup(c => c.Options).Returns(options);
            var fixture = Fixture.New<string>();
            TestHelper.SetContext(fixture, contextMock.Object);

            ((Fixture<string>)fixture).InstantiateFixture();

            contextMock.Verify(c => c.InstantiateWithStrategy(
                It.Is<FixtureRequest>(r => r.Type == typeof(string)),
                It.Is<InstantiationMethod>(i => i == InstantiationMethod.CreateUninitialized),
                It.Is<InitializeMembers>(i => i == InitializeMembers.All)),
                Times.Once);
        }

        [Test]
        public void InstantiateFixture_ReturnsResultFromContext()
        {
            var expected = "test string value";
            var options = new FixtureOptions();
            var contextMock = new Mock<IFixtureContext>();
            contextMock.Setup(c => c.Options).Returns(options);
            contextMock.Setup(c => c.InstantiateWithStrategy(It.IsAny<FixtureRequest>(), It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(expected);
            var fixture = Fixture.New<string>();
            TestHelper.SetContext(fixture, contextMock.Object);

            var result = ((Fixture<string>)fixture).InstantiateFixture();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void InstantiateFixture_ImplicitInstantiationNotAllowed_ThrowsException()
        {
            var options = new FixtureOptions { AllowImplicitConstruction = false };
            var contextMock = new Mock<IFixtureContext>();
            contextMock.Setup(c => c.Options).Returns(options);
            var fixture = Fixture.New<string>();
            TestHelper.SetContext(fixture, contextMock.Object);

            Assert.Throws<InvalidOperationException>(() => ((Fixture<string>)fixture).InstantiateFixture());
        }
    }
}
