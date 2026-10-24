#pragma warning disable CS0649

namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class WithTests
    {
        class PropWithSetterClass
        {
            public string Text { get; set; } = null!;
        }
        [Test]
        public void PropWithSetter_SetsProperty()
        {
            var text = "Test string";

            var fixture = TestHelper.MakeFixture<PropWithSetterClass>();

            fixture.With(t => t.Text, text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Text, Is.EqualTo(text));
        }

        class PropWithSetterAndUnrecognizedFieldClass
        {
            private string _unrelatedFieldName = null!;
            public string Text { get => _unrelatedFieldName; set { _unrelatedFieldName = value; } }
        }
        [Test]
        public void PropWithSetterAndUnrecognizedField_SetsProperty()
        {
            var text = "Test string";

            var fixture = TestHelper.MakeFixture<PropWithSetterAndUnrecognizedFieldClass>();

            fixture.With(t => t.Text, text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Text, Is.EqualTo(text));
        }

        class PropWithoutSetterClass
        {
            private readonly string _text = null!;
            public string Text => _text;
        }
        [Test]
        public void PropWithoutSetter_SetsProperty()
        {
            var text = "Test string";

            var fixture = TestHelper.MakeFixture<PropWithoutSetterClass>();

            fixture.With(t => t.Text, text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Text, Is.EqualTo(text));
        }

        class PropWithoutSetterAndUnrecognizedFieldClass
        {
            private readonly string _unrelatedFieldName = null!;
            public string Text => _unrelatedFieldName;
        }
        [Test]
        public void PropWithoutSetterAndUnrecognizedField_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<PropWithoutSetterAndUnrecognizedFieldClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.With(t => t.Text, "Test"));
        }

        [Test]
        public void With_InstantiatesFixture()
        {
            var text = "Test string";

            var fixture = TestHelper.MakeFixture<PropWithSetterClass>();

            using (Assert.EnterMultipleScope())
            {
                Assert.DoesNotThrow(() => fixture.With(t => t.Text, text));
                Assert.That(TestHelper.GetFixture(fixture), Is.Not.Null);
            }
        }

        class OuterFieldClass
        {
            public InnerFieldClass Inner = null!;
        }
        class InnerFieldClass
        {
            public string Field = null!;
        }
        [Test]
        public void FieldExpression_SetsField()
        {
            var text = "hello";
            var fixture = TestHelper.MakeFixture<OuterFieldClass>();

            fixture.With(f => f.Inner.Field, text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Inner.Field, Is.EqualTo(text));
        }

        struct Point
        {
            public int X;
            public int Y { get; set; }
            private int _z;
            public readonly int Z => _z;
        }
        class StructMemberClass
        {
            public Point Origin;
            public Point Center { get; set; }
            private readonly Point _fixedCenter;
            public Point FixedCenter => _fixedCenter;
        }
        [Test]
        public void FieldOfStructMember_SetsField()
        {
            var fixture = TestHelper.MakeFixture<StructMemberClass>();

            fixture.With(s => s.Origin.X, 5);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Origin.X, Is.EqualTo(5));
        }

        [Test]
        public void PropWithSetterOfStructMember_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<StructMemberClass>();

            fixture.With(s => s.Center.Y, 5);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Center.Y, Is.EqualTo(5));
        }

        [Test]
        public void PropWithoutSetterOfStructMember_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<StructMemberClass>();

            fixture.With(s => s.Center.Z, 5);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Center.Z, Is.EqualTo(5));
        }

        [Test]
        public void MemberOfStructInPropWithoutSetter_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<StructMemberClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.With(s => s.FixedCenter.Y, 5));
        }
    }
}
