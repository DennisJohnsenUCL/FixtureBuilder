namespace FixtureBuilder.Tests
{
    internal sealed partial class WithFieldTests
    {
        record TestRecord(string Text, int Number);
        [Test]
        public void RecordProperty_SetsProperty()
        {
            var fixture = Fixture.New<TestValue>().BypassConstructor().WithField(t => t.Text, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        [Test]
        public void RecordProperties_SetsProperties()
        {
            var fixture = Fixture.New<TestRecord>().BypassConstructor().WithField(t => t.Text, _text).WithField(t => t.Number, _number);
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Text, Is.EqualTo(_text));
                Assert.That(field.Number, Is.EqualTo(_number));
            }
        }

        [Test]
        public void NotARecordProperty_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Fixture.New<TestRecord>().BypassConstructor().WithField(t => t.GetHashCode(), _number));
        }

        [Test]
        public void NoRecordPropertyBackingField_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<TestRecord>().BypassConstructor().WithField(t => t.Text.Length, _number));
        }

        class NormalClass
        {
            public string Text { get; set; } = null!;
            public int Number { get; set; }
        }
        [Test]
        public void ClassProperty_SetsProperty()
        {
            var fixture = Fixture.New<TestClass>().BypassConstructor().WithField(t => t.Text, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        [Test]
        public void ClassProperties_SetsProperties()
        {
            var fixture = Fixture.New<TestClass>().BypassConstructor().WithField(t => t.Text, _text).WithField(t => t.Number, _number);
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Text, Is.EqualTo(_text));
                Assert.That(field.Number, Is.EqualTo(_number));
            }
        }

        [Test]
        public void NotAClassProperty_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Fixture.New<TestClass>().BypassConstructor().WithField(t => t.GetHashCode(), _number));
        }

        [Test]
        public void NoClassPropertyBackingField_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<TestClass>().BypassConstructor().WithField(t => t.Text.Length, _number));
        }

        class ExplicitBackingFieldClass
        {
            private readonly string _text = null!;
            public string Text => _text;
        }
        [Test]
        public void ExplicitBackingField_SetsProperty()
        {
            var fixture = Fixture.New<ExplicitBackingFieldClass>().BypassConstructor().WithField(t => t.Text, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        class ExplicitBackingFieldNoUnderscoreClass
        {
            private readonly string privateExplicitField = null!;
            public string PrivateExplicitField => privateExplicitField;
        }
        [Test]
        public void ExplicitBackingFieldNoUnderscore_SetsProperty()
        {
            var fixture = Fixture.New<ExplicitBackingFieldNoUnderscoreClass>().BypassConstructor().WithField(t => t.PrivateExplicitField, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.PrivateExplicitField, Is.EqualTo(_text));
        }

        [Test]
        public void DerivedProperty_SetsProperty()
        {
            var fixture = Fixture.New<DerivedTestClass>().BypassConstructor().WithField(t => t.Text, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        [Test]
        public void OverriddenProperty_SetsProperty()
        {
            var fixture = Fixture.New<DerivedTestClass>().BypassConstructor().WithField(t => t.Number, _number);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Number, Is.EqualTo(_number));
        }

        [Test]
        public void ImplicitInterfaceImplementation_SetsProperty()
        {
            var fixture = Fixture.New<InterfaceTestClass>().BypassConstructor().WithField(t => t.ImplicitProperty, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ImplicitProperty, Is.EqualTo(_text));
        }

        [Test]
        public void TwiceDerivedClass_PropertyInDerivedClass_SetsProperty()
        {
            var fixture = Fixture.New<TwiceDerivedClass>().BypassConstructor().WithField(p => p.Number, _number);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Number, Is.EqualTo(_number));
        }

        [Test]
        public void GenericClass_SetsProperty()
        {
            var fixture = Fixture.New<GenericClass<string>>().BypassConstructor().WithField(g => g.Value, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Value, Is.EqualTo(_text));
        }

        class NestedPropertyClass
        {
            public NestedClass NestedClass { get; set; } = null!;
        }
        [Test]
        public void NestedProperty_SetsProperty()
        {
            var fixture = Fixture.New<NestedPropertyClass>().BypassConstructor().WithField(t => t.NestedClass.Value, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass.Value, Is.EqualTo(_text));
        }

        [Test]
        public void DerivedNestedProperty_SetsProperty()
        {
            var fixture = Fixture.New<DerivedTestClass>().BypassConstructor().WithField(t => t.NestedClass.Value, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass.Value, Is.EqualTo(_text));
        }

        [Test]
        public void DeeperNestedProperty_SetsProperty()
        {
            var fixture = Fixture.New<NestedPropertyClass>().BypassConstructor().WithField(t => t.NestedClass.DeeperNestedClass.Value, _number);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass.DeeperNestedClass.Value, Is.EqualTo(_number));
        }

        [Test]
        public void FieldNameGiven_DeeperNestedProperty_SetsProperty()
        {
            var fieldName = "_privateField";

            var fixture = Fixture.New<NestedPropertyClass>().BypassConstructor().WithField(fieldName, t => t.NestedClass.DeeperNestedClass.Text, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass.DeeperNestedClass.Text, Is.EqualTo(_text));
        }

        class CollectionClass
        {
            public List<string> StringList { get; } = null!;
        }
        [Test]
        public void CollectionTypeFieldWithProp_CollectionParameters_SetsField()
        {
            var fixture = Fixture.New<CollectionClass>().BypassConstructor().WithField(t => t.StringList, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.StringList[0], Is.EqualTo(_text));
        }

        class CollectionDifferentTypeClass
        {
            private readonly List<string> _stringList = null!;
            public IReadOnlyList<string> StringList { get; } = null!;
        }
        [Test]
        public void BackingFieldDifferentCollectionType_SetsField()
        {
            var fixture = Fixture.New<CollectionDifferentTypeClass>().WithField(t => t.StringList, [_text]).Build();

            Assert.That(fixture.StringList[0], Is.EqualTo(_text));
        }
    }
}
