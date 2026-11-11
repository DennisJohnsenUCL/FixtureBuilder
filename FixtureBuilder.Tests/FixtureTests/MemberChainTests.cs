using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class MemberChainTests
    {
        #region Intermediate members

        private class ConstructorSetsFirstClass
        {
            public string First { get; set; }
            public string Second { get; set; } = null!;

            public ConstructorSetsFirstClass() => First = "constructor";
        }

        [Test]
        public void PropertyChain_SetsLastMember()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PropertyHolder<string>>>();

            var result = fixture.With(x => x.Value.Value, "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void FieldChain_SetsLastMember()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<FieldHolder<string>>>();

            var result = fixture.With(x => x.Value.Value, "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void ExistingMemberInChain_IsKept()
        {
            var existing = new PropertyHolder<string>();
            var fixture = TestHelper.MakeFixture<PropertyHolder<PropertyHolder<string>>>();
            fixture.With(x => x.Value, existing);

            var result = fixture.With(x => x.Value.Value, "value").Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Value, Is.SameAs(existing));
                Assert.That(result.Value.Value, Is.EqualTo("value"));
            }
        }

        [Test]
        public void NullMemberInChain_InstantiatesMemberWithAutoConstructor()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsFirstClass>>();

            var result = fixture.With(x => x.Value.Second, "value").Build();

            Assert.That(result.Value.First, Is.EqualTo("constructor"));
        }

        [Test]
        public void NullMemberInChain_CreateUninitializedOption_InstantiatesMemberUninitialized()
        {
            var options = new FixtureOptions { NestedMemberInstantiationMethod = InstantiationMethod.CreateUninitialized };
            var factory = new FixtureFactory(options);
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsFirstClass>>(factory);

            var result = fixture.With(x => x.Value.Second, "value").Build();

            Assert.That(result.Value.First, Is.Null);
        }

        [Test]
        public void NullMemberInChain_NestedInstantiationNotAllowed_ThrowsInvalidOperationException()
        {
            var options = new FixtureOptions { AllowInstantiateNestedMembers = false };
            var factory = new FixtureFactory(options);
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsFirstClass>>(factory);

            Assert.Throws<InvalidOperationException>(() => fixture.With(x => x.Value.Second, "value"));
        }

        [Test]
        public void NullMemberInChainWithoutSetter_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<PropertyHolder<string>>>();

            Assert.Throws<InvalidOperationException>(() => fixture.With(x => x.Value.Value, "value"));
        }

        [Test]
        public void NullMemberInChain_ProvidedValueForMemberType_SetsProvidedValue()
        {
            var provided = new PropertyHolder<string>();
            var factory = new FixtureFactory().With(provided);
            var fixture = TestHelper.MakeFixture<PropertyHolder<PropertyHolder<string>>>(factory);

            var result = fixture.With(x => x.Value.Value, "value").Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Value, Is.SameAs(provided));
                Assert.That(result.Value.Value, Is.EqualTo("value"));
            }
        }

        private class NullForTypeProvider<T> : ICustomProvider
        {
            public object? ResolveValue(FixtureRequest request) => request.Type == typeof(T) ? null : NoResult.Instance;
        }

        [Test]
        public void NullMemberInChain_ProviderReturnsNull_ThrowsInvalidOperationException()
        {
            var factory = new FixtureFactory().AddProvider(new NullForTypeProvider<PropertyHolder<string>>());
            var fixture = TestHelper.MakeFixture<PropertyHolder<PropertyHolder<string>>>(factory);

            Assert.Throws<InvalidOperationException>(() => fixture.With(x => x.Value.Value, "value"));
        }

        [Test]
        public void NullInterfaceMemberInChain_RootTypeLink_InstantiatesLinkedType()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<PropertyHolder<IValueInterface>>(builder => builder.AddTypeLink<IValueInterface, ImplicitImplementationClass>());
            var fixture = TestHelper.MakeFixture<PropertyHolder<IValueInterface>>(factory);

            var result = fixture.With(x => x.Value.Value, "value").Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Value, Is.TypeOf<ImplicitImplementationClass>());
                Assert.That(result.Value.Value, Is.EqualTo("value"));
            }
        }

        private class StringParameterAndPropertyClass(string label)
        {
            public string Label { get; } = label;
            public string Value { get; set; } = null!;
        }

        [Test]
        public void NullMemberInChain_RootScopedParameterValue_ConstructsMemberWithIt()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<PropertyHolder<StringParameterAndPropertyClass>>(builder => builder.WithParameter("scoped"));
            var fixture = TestHelper.MakeFixture<PropertyHolder<StringParameterAndPropertyClass>>(factory);

            var result = fixture.With(x => x.Value.Value, "value").Build();

            Assert.That(result.Value.Label, Is.EqualTo("scoped"));
        }

        #endregion

        #region Struct members

        private struct FieldStruct
        {
            public int Value;
        }

        [Test]
        public void FieldOfStructInField_SetsFieldAndWritesStructBack()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<FieldStruct>>();

            var result = fixture.With(x => x.Value.Value, 5).Build();

            Assert.That(result.Value.Value, Is.EqualTo(5));
        }

        [Test]
        public void PropertyOfStructInProperty_SetsPropertyAndWritesStructBack()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PropertyHolderStruct<int>>>();

            var result = fixture.With(x => x.Value.Value, 5).Build();

            Assert.That(result.Value.Value, Is.EqualTo(5));
        }

        [Test]
        public void PropertyWithoutSetterOfStructInProperty_SetsBackingFieldAndWritesStructBack()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<ReadOnlyPropertyHolderStruct<int>>>();

            var result = fixture.With(x => x.Value.Value, 5).Build();

            Assert.That(result.Value.Value, Is.EqualTo(5));
        }

        [Test]
        public void MemberOfStructInStruct_WritesBothStructsBack()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PropertyHolderStruct<PointStruct>>>();

            var result = fixture.With(x => x.Value.Value.X, 5).Build();

            Assert.That(result.Value.Value.X, Is.EqualTo(5));
        }

        [Test]
        public void MemberOfStructInPropertyWithoutSetter_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<PropertyHolderStruct<int>>>();

            Assert.Throws<InvalidOperationException>(() => fixture.With(x => x.Value.Value, 5));
        }

        [Test]
        public void MemberOfStructInPropertyWithoutSetter_ValueUnchanged_LeavesStructAsIs()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<PropertyHolderStruct<int>>>();

            var result = fixture.With(x => x.Value.Value, 0).Build();

            Assert.That(result.Value.Value, Is.Zero);
        }

        private struct MoneyStruct
        {
            public decimal Amount { get; set; }
            public string Currency { get; set; }

            // Ignores Currency, so Equals cannot tell a copy with a changed Currency from the original
            public override readonly bool Equals(object? obj) => obj is MoneyStruct other && other.Amount == Amount;
            public override readonly int GetHashCode() => Amount.GetHashCode();
        }

        [Test]
        public void MemberOfStructWithCustomEqualsInPropertyWithoutSetter_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<MoneyStruct>>();

            Assert.Throws<InvalidOperationException>(() => fixture.With(x => x.Value.Currency, "EUR"));
        }

        #endregion

        #region Nullable struct members

        [Test]
        public void MemberOfNullableStructProperty_SetsMemberAndWritesStructBack()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PointStruct?>>();
            fixture.With(x => x.Value, new PointStruct());

            var result = fixture.With(x => x.Value!.Value.X, 5).Build();

            Assert.That(result.Value?.X, Is.EqualTo(5));
        }

        [Test]
        public void MemberOfNullNullableStructProperty_InstantiatesStructAndSetsMember()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PointStruct?>>();

            var result = fixture.With(x => x.Value!.Value.X, 5).Build();

            Assert.That(result.Value?.X, Is.EqualTo(5));
        }

        [Test]
        public void MemberOfNullableStructField_SetsMemberAndWritesStructBack()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<PointStruct?>>();
            fixture.With(x => x.Value, new PointStruct());

            var result = fixture.With(x => x.Value!.Value.X, 5).Build();

            Assert.That(result.Value?.X, Is.EqualTo(5));
        }

        private struct SegmentStruct
        {
            public PointStruct? End { get; set; }
        }

        [Test]
        public void MemberOfNullableStructInStruct_WritesChangesBackThroughEveryLevel()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<SegmentStruct>>();

            var result = fixture.With(x => x.Value.End!.Value.X, 5).Build();

            Assert.That(result.Value.End?.X, Is.EqualTo(5));
        }

        #endregion
    }
}
