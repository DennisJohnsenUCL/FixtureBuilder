using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureTests.UseAutoConstructor
{
    internal sealed class UseAutoConstructorSelfParametersTests
    {
        #region Self parameters

        private class SelfParameterOrTwoIntsClass
        {
            public string UsedConstructor { get; }

            public SelfParameterOrTwoIntsClass(int first, int second) => UsedConstructor = "two ints";
            public SelfParameterOrTwoIntsClass(SelfParameterOrTwoIntsClass? parent = null) => UsedConstructor = "self parameter";
        }

        [Test]
        public void SelfParameterConstructor_IsTriedAfterOtherConstructors()
        {
            var fixture = Fixture.New<SelfParameterOrTwoIntsClass>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.UsedConstructor, Is.EqualTo("two ints"));
        }

        #endregion

        #region Nullable self parameters

        private class Category(string name, Category? parent)
        {
            public string Name { get; } = name;
            public Category? Parent { get; } = parent;
        }

        [Test]
        public void NullableSelfParameter_ResolvesNull()
        {
            var fixture = Fixture.New<Category>();

            var result = fixture.UseAutoConstructor().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("name"));
                Assert.That(result.Parent, Is.Null);
            }
        }

        [Test]
        public void NullableSelfParameter_NullsNotPreferred_ThrowsInvalidOperationException()
        {
            var options = new FixtureOptions { PreferNullParameterValues = false };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<Category>();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());

            Assert.That(ex.Message, Does.Contain("Failed to find a constructor for Category"));
        }

        private class DefaultSelfParameterCategory(string name, DefaultSelfParameterCategory? parent = null)
        {
            public string Name { get; } = name;
            public DefaultSelfParameterCategory? Parent { get; } = parent;
        }

        [Test]
        public void SelfParameterWithDefaultValue_NullsNotPreferred_ResolvesDefaultValue()
        {
            var options = new FixtureOptions { PreferNullParameterValues = false };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<DefaultSelfParameterCategory>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Parent, Is.Null);
        }

        private class Product(Category category)
        {
            public Category Category { get; } = category;
        }

        [Test]
        public void NullableSelfParameterOfNestedType_UsesRootTypeOptions()
        {
            var options = new FixtureOptions { PreferNullParameterValues = false };
            var factory = new FixtureFactory(options)
                .WhenBuilding<Product>(builder => builder.SetOptions(rootOptions => rootOptions.PreferNullParameterValues = true));
            var fixture = factory.New<Product>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Category.Parent, Is.Null);
        }

        private class ParentParameterProvider(Category parent) : ICustomProvider
        {
            public int Calls { get; private set; }

            public object? ResolveValue(FixtureRequest request)
            {
                if (request.Type != typeof(Category) || request.Name != "parent") return NoResult.Instance;

                Calls++;
                return parent;
            }
        }

        [Test]
        public void NullableSelfParameter_NullsNotPreferredAndProviderForParameter_UsesProviderValue()
        {
            var parent = new Category("parent", null);
            var options = new FixtureOptions { PreferNullParameterValues = false };
            var factory = new FixtureFactory(options).AddProvider(new ParentParameterProvider(parent));
            var fixture = factory.New<Category>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Parent, Is.SameAs(parent));
        }

        [Test]
        public void SelfParameterResolvedByProvider_CallsProviderOnce()
        {
            var provider = new ParentParameterProvider(new Category("parent", null));
            var options = new FixtureOptions { PreferNullParameterValues = false };
            var factory = new FixtureFactory(options).AddProvider(provider);
            var fixture = factory.New<Category>();

            fixture.UseAutoConstructor().Build();

            Assert.That(provider.Calls, Is.EqualTo(1));
        }

        #endregion

        #region Linked self parameters

        private interface INode;

        private class Node : INode
        {
            public string UsedConstructor { get; }

            public Node(INode? parent) => UsedConstructor = "linked self parameter";
            public Node(string first, string second) => UsedConstructor = "two strings";
        }

        [Test]
        public void LinkedSelfParameterConstructor_IsTriedAfterOtherConstructors()
        {
            var factory = new FixtureFactory().AddTypeLink<INode, Node>();
            var fixture = factory.New<Node>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.UsedConstructor, Is.EqualTo("two strings"));
        }

        private class DefaultParameterNode : INode
        {
            public string UsedConstructor { get; }

            public DefaultParameterNode(INode parent) => UsedConstructor = "linked self parameter";
            public DefaultParameterNode(string name, INode? parent = null) => UsedConstructor = "default parameter";
        }

        [Test]
        public void LinkedSelfParameterNotResolvable_SkipsConstructor()
        {
            var factory = new FixtureFactory().AddTypeLink<INode, DefaultParameterNode>();
            var fixture = factory.New<DefaultParameterNode>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.UsedConstructor, Is.EqualTo("default parameter"));
        }

        private class Folder
        {
            public Folder? Parent { get; }

            public Folder(Folder parent) => Parent = parent;
            public Folder(string first, string second) { }
        }

        private class SpecialFolder() : Folder("first", "second");

        [Test]
        public void SelfParameterLinkedToSubtype_IsNotTriedLast()
        {
            var factory = new FixtureFactory().AddTypeLink<Folder, SpecialFolder>();
            var fixture = factory.New<Folder>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Parent, Is.TypeOf<SpecialFolder>());
        }

        private class Tree(Tree parent)
        {
            public Tree Parent { get; } = parent;
        }

        private class SpecialTree() : Tree(null!);

        [Test]
        public void SelfParameterLinkedToSubtype_OnlyConstructor_IsUsed()
        {
            var factory = new FixtureFactory().AddTypeLink<Tree, SpecialTree>();
            var fixture = factory.New<Tree>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Parent, Is.TypeOf<SpecialTree>());
        }

        #endregion
    }
}
