using KEI.Infrastructure;
using KEI.Infrastructure.Utils;
using System;
using System.Linq;
using Xunit;

namespace DataContainer.Tests
{
    public class DataContainerTests
    {
        [Theory]
        [InlineData("MyProperty")]
        [InlineData("MyProperty2")]
        [InlineData("My_Property")]
        [InlineData("MyProperty_1")]
        public void IdentifierExtensions_IsValidIdentifierReturnsTrue(string identifierName)
        {
            Assert.True(IdentifierExtensions.IsValidIdentifier(identifierName));
        }

        [Theory]
        [InlineData("My Property")]
        [InlineData("MyProperty 2")]
        [InlineData("2MyProperty")]
        [InlineData("My$Property")]
        [InlineData("MyProperty[0]")]
        public void IdentifierExtensions_IsValidIdentifierReturnsFalse(string identifierName)
        {
            Assert.False(IdentifierExtensions.IsValidIdentifier(identifierName));
        }

        #region Access And Manipulation

        [Theory]
        [InlineData(typeof(POCO))]
        public void DataContainerBase_Morph_MustRecreatesSameObject(Type t)
        {
            var obj = Activator.CreateInstance(t);

            IDataContainer data = DataContainerBuilder.CreateObject("Untitiled", obj);
            IPropertyContainer property = PropertyContainerBuilder.CreateObject("Untitled", obj);

            var morphedData = data.Morph();
            var morphedProperty = property.Morph();

            foreach (var prop in t.GetProperties())
            {
                var expected = prop.GetValue(obj);
                var actualData = prop.GetValue(morphedData);
                var actualProeprty = prop.GetValue(morphedProperty);

                Assert.Equal(expected, actualData);
                Assert.Equal(expected, actualProeprty);
            }

        }


        [Theory]
        [InlineData(typeof(POCO))]
        public void DataContainerBase_Store_MustDeserialize(Type t)
        {
            var obj = Activator.CreateInstance(t);

            IDataContainer data = DataContainerBuilder.CreateObject("Untitiled", obj);
            IPropertyContainer property = PropertyContainerBuilder.CreateObject("Untitled", obj);

            var serializedData = XmlHelper.SerializeToString(data);
            var serializedProperty = XmlHelper.SerializeToString(property);

            var recreatedData = XmlHelper.DeserializeFromString<KEI.Infrastructure.DataContainer>(serializedData);
            var recreatedProperty = XmlHelper.DeserializeFromString<PropertyContainer>(serializedProperty);

            Assert.NotNull(recreatedData);
            Assert.NotNull(recreatedProperty);

            var morphedData = recreatedData.Morph();
            var morphedProperty = recreatedProperty.Morph();

            foreach (var prop in t.GetProperties())
            {
                var expected = prop.GetValue(obj);
                var actualData = prop.GetValue(morphedData);
                var actualProeprty = prop.GetValue(morphedProperty);

                Assert.Equal(expected, actualData);
                Assert.Equal(expected, actualProeprty);
            }

        }

        [Fact]
        public void DataContainerBase_CanSerializeAndDeserializeDefaultDataObjects()
        {
            IDataContainer Obj = DataContainerBuilder.Create("Object1")
                .Data("IntegerProperty", 14)
                .Data("CharacterProperty", '@')
                .Data("ByteProperty", (byte)255)
                .Data("BooleanProperty", true)
                .Data("DoubleProperty", 3.1412)
                .Data("ColorProperty", new Color(124, 101, 33))
                .Data("PointProperty", new Point(24, 48))
                .Data("TimeSpanProperty", TimeSpan.FromSeconds(39))
                .Data("DateTimeProperty", DateTime.Now)
                .Data("Array2DProperty", new int[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } })
                .Data("Array1DProperty", new int[] { 1, 1, 2, 3, 5, 13 })
                .Data("EnumProperty", DayOfWeek.Monday)
                .Data("ObjectAsDataContainer", new BindingTestObject())
                .Data("ObjectAsXML", new BindingTestObject(), SerializationFormat.Xml)
                .Data("ObjectAsJson", new BindingTestObject(), SerializationFormat.Json)
                .Build();

            string xml = XmlHelper.SerializeToString(Obj);

            IDataContainer newObj = XmlHelper.DeserializeFromString<KEI.Infrastructure.DataContainer>(xml);

            Assert.Equal(15, newObj.Count);
        }

        [Fact]
        public void DataContainerBase_Get_MustGetCorrectValue()
        {
            const string PROP_NAME = "IntProperty";

            IPropertyContainer property = PropertyContainerBuilder.Create()
                .Property(PROP_NAME, 42)
                .Build();

            int prop = 0;
            bool containsProperty = property.GetValue(PROP_NAME, ref prop);

            Assert.True(containsProperty);
            Assert.NotEqual(0, prop);
            Assert.Equal(42, prop);

            int prop2 = property.GetValue<int>(PROP_NAME);

            Assert.NotEqual(0, prop2);
            Assert.Equal(42, prop2);
        }

        [Fact]
        public void DataContainerBase_Get_MustReturnDefaultIfNotPresent()
        {
            const string PROP_NAME = "IntProperty";

            IPropertyContainer property = PropertyContainerBuilder.Create()
                .Property(PROP_NAME, 42)
                .Build();

            int prop = 0;
            bool containsProperty = property.GetValue("blah", ref prop);

            Assert.False(containsProperty);
            Assert.Equal(default, prop);

            int prop2 = property.GetValue<int>("blah");

            Assert.Equal(default, prop2);
        }

        [Fact]
        public void DataContainerBase_Set_MustSetValue()
        {
            const string PROP_NAME = "IntProperty";
            const int VALUE = 42;
            const int SET_VALUE = 14;

            IPropertyContainer property = PropertyContainerBuilder.Create()
                .Property(PROP_NAME, VALUE)
                .Build();

            property.SetValue(PROP_NAME, SET_VALUE);

            int value = 0;
            property.GetValue(PROP_NAME, ref value);
            int value2 = property.GetValue<int>(PROP_NAME);

            Assert.NotEqual(VALUE, value);
            Assert.NotEqual(VALUE, value2);

            Assert.Equal(SET_VALUE, value);
            Assert.Equal(SET_VALUE, value);
        }

        [Fact]
        public void DataContainerBase_Put_MustAddValueIfNotPresent()
        {
            const string PROP_NAME = "IntProperty";
            const int SET_VALUE = 14;

            IPropertyContainer property = PropertyContainerBuilder.Create().Build();

            Assert.False(property.ContainsData(PROP_NAME));

            property.PutValue(PROP_NAME, SET_VALUE);

            Assert.True(property.ContainsData(PROP_NAME));

            int value = 0;
            property.GetValue(PROP_NAME, ref value);

            Assert.Equal(SET_VALUE, value);
        }

        [Fact]
        public void DataContainerBase_Put_MustUpdateValueIfPresent()
        {
            const string PROP_NAME = "IntProperty";
            const int VALUE = 42;
            const int SET_VALUE = 14;

            IPropertyContainer property = PropertyContainerBuilder.Create()
                .Property(PROP_NAME, VALUE)
                .Build();

            int originalValue = 0;
            property.GetValue(PROP_NAME, ref originalValue);

            Assert.True(property.ContainsData(PROP_NAME));

            property.PutValue(PROP_NAME, SET_VALUE);

            Assert.True(property.ContainsData(PROP_NAME));

            int value = 0;
            property.GetValue(PROP_NAME, ref value);

            Assert.NotEqual(originalValue, value);
            Assert.Equal(SET_VALUE, value);
        }

        [Fact]
        public void DataContainerBase_Morph_T_CanMoverToObject()
        {
            IDataContainer DC = DataContainerBuilder.Create("POCO")
                .Data(nameof(POCO.IntProperty), 12)
                .Data(nameof(POCO.StringProperty), "Hello")
                .Build();

            POCO morphed = DC.Morph<POCO>();

            Assert.Equal(12, morphed.IntProperty);
            Assert.Equal("Hello", morphed.StringProperty);
        }

        #endregion

        #region Binding

        [Fact]
        public void PropertyContainer_SetBinding_MustBindOneWay()
        {
            const string PROP_NAME = "IntProperty";
            const int VALUE = 42;
            const int NEW_VALUE = 14;

            IPropertyContainer pc = PropertyContainerBuilder.Create()
                .Property(PROP_NAME, VALUE)
                .Build();

            var bindingTarget = new BindingTestObject();

            Assert.NotEqual(VALUE, bindingTarget.IntProperty);

            pc.SetBinding(PROP_NAME, () => bindingTarget.IntProperty, BindingMode.OneWay);

            Assert.Equal(VALUE, bindingTarget.IntProperty);

            pc.SetValue(PROP_NAME, NEW_VALUE);

            Assert.Equal(NEW_VALUE, bindingTarget.IntProperty);

            bindingTarget.IntProperty = 32;

            int propertyValue = 0;
            pc.GetValue(PROP_NAME, ref propertyValue);

            pc.RemoveBinding(PROP_NAME, () => bindingTarget.IntProperty);

            Assert.NotEqual(32, propertyValue);
            Assert.Equal(NEW_VALUE, propertyValue);

        }

        [Fact]
        public void PropertyContainer_SetBinding_MustBindTwoWay()
        {
            const string PROP_NAME = "IntProperty";
            const int VALUE = 42;
            const int NEW_VALUE = 14;

            IPropertyContainer property = PropertyContainerBuilder.Create()
                .Property(PROP_NAME, VALUE)
                .Build();

            var bindingTarget = new BindingTestObject();

            Assert.NotEqual(VALUE, bindingTarget.IntProperty);

            property.SetBinding(PROP_NAME, () => bindingTarget.IntProperty, BindingMode.TwoWay);

            Assert.Equal(VALUE, bindingTarget.IntProperty);

            property.SetValue(PROP_NAME, NEW_VALUE);

            Assert.Equal(NEW_VALUE, bindingTarget.IntProperty);

            bindingTarget.IntProperty = 32;

            int propertyValue = 0;
            property.GetValue(PROP_NAME, ref propertyValue);

            property.RemoveBinding(PROP_NAME, () => bindingTarget.IntProperty);

            Assert.Equal(propertyValue, bindingTarget.IntProperty);
        }

        [Fact]
        public void PropertyContainer_SetBinding_MustBindOneWayToSource()
        {
            const string PROP_NAME = "IntProperty";
            const int VALUE = 42;
            const int NEW_VALUE = 14;

            IPropertyContainer property = PropertyContainerBuilder.Create()
                .Property(PROP_NAME, VALUE)
                .Build();

            var bindingTarget = new BindingTestObject();

            Assert.NotEqual(VALUE, bindingTarget.IntProperty);

            property.SetBinding(PROP_NAME, () => bindingTarget.IntProperty, BindingMode.OneWayToSource);

            Assert.Equal(VALUE, bindingTarget.IntProperty);

            property.SetValue(PROP_NAME, NEW_VALUE);

            Assert.NotEqual(NEW_VALUE, bindingTarget.IntProperty);
            Assert.Equal(VALUE, bindingTarget.IntProperty);

            bindingTarget.IntProperty = 32;

            int propertyValue = 0;
            property.GetValue(PROP_NAME, ref propertyValue);

            property.RemoveBinding(PROP_NAME, () => bindingTarget.IntProperty);

            Assert.Equal(propertyValue, bindingTarget.IntProperty);
        }

        [Fact]
        public void PropertyContainer_SetBinding_MustBindOneTime()
        {
            const string PROP_NAME = "IntProperty";
            const int VALUE = 42;
            const int NEW_VALUE = 14;

            IPropertyContainer property = PropertyContainerBuilder.Create()
                .Property(PROP_NAME, VALUE)
                .Build();

            var bindingTarget = new BindingTestObject();

            Assert.NotEqual(VALUE, bindingTarget.IntProperty);

            property.SetBinding(PROP_NAME, () => bindingTarget.IntProperty, BindingMode.OneTime);

            Assert.Equal(VALUE, bindingTarget.IntProperty);

            property.SetValue(PROP_NAME, NEW_VALUE);

            Assert.NotEqual(NEW_VALUE, bindingTarget.IntProperty);
            Assert.Equal(VALUE, bindingTarget.IntProperty);

            bindingTarget.IntProperty = 32;

            int propertyValue = 0;
            property.GetValue(PROP_NAME, ref propertyValue);

            property.RemoveBinding(PROP_NAME, () => bindingTarget.IntProperty);

            Assert.NotEqual(32, propertyValue);
            Assert.Equal(NEW_VALUE, propertyValue);
        }

        #endregion

        #region Set Operations

        [Fact]
        public void IDataContainer_Union_UsesFirstsNameForResultName()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 22)
                .Build();

            IDataContainer B = DataContainerBuilder.Create("B")
                .Data("B", 33)
                .Build();

            IDataContainer AunionB = A.Union(B);
            IDataContainer BunionA = B.Union(A);

            Assert.Equal(A.Name, AunionB.Name);
            Assert.Equal(B.Name, BunionA.Name);
        }

        [Fact]
        public void IDataContainer_Union_UsesValuesFromFirstForSameProperty()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .Data("B", 2)
                .Build();

            IDataContainer B = DataContainerBuilder.Create("B")
                .Data("A", 2)
                .Data("B", 1)
                .Build();

            IDataContainer AunionB = A.Union(B);
            IDataContainer BunionA = B.Union(A);

            int AA = (int)A["A"];
            int AB = (int)A["B"];

            int BA = (int)B["A"];
            int BB = (int)B["B"];

            int AUBA = (int)AunionB["A"];
            int AUBB = (int)AunionB["B"];
            int BUAA = (int)BunionA["A"];
            int BUAB = (int)BunionA["B"];

            Assert.Equal(AA, AUBA);
            Assert.Equal(AB, AUBB);

            Assert.Equal(BA, BUAA);
            Assert.Equal(BB, BUAB);
        }

        [Fact]
        public void IDataContainer_Union_MustWorkWithSingleLevelData()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .Data("C", 3)
                .Data("E", 5)
                .Data("G", 7)
                .Data("Z", 26)
                .Build();

            IDataContainer B = DataContainerBuilder.Create("A")
                .Data("B", 2)
                .Data("D", 4)
                .Data("F", 6)
                .Data("H", 8)
                .Data("Z", 28)
                .Build();

            IDataContainer AunionB = A.Union(B);

            Assert.Equal(9, AunionB.Count); // 4 + 4 + 1 (common)

            var keys = AunionB.GetKeys().ToList();
            keys.Sort();
            Assert.Equal("ABCDEFGHZ", string.Join("", keys)); // all keys are present.
        }

        [Fact]
        public void IDataContainer_Union_MustWorkWithMultiLevelData()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .Data("B", 2)
                .DataContainer("AA", b => b
                    .Data("A1", 11)
                    .Data("A2", 12)
                    .Data("A3", 13))
                .Data("C", 3)
                .DataContainer("CC", b => b
                    .Data("C1", 31)
                    .Data("C3", 33))
                .Build();

            IDataContainer B = DataContainerBuilder.Create("B")
                .Data("A", 1)
                .Data("B", 2)
                .DataContainer("AA", b => b
                    .Data("A4", 14)
                    .Data("A5", 15)
                    .Data("A6", 16))
                .Data("C", 3)
                .DataContainer("CC", b => b
                    .Data("C2", 32))
                .Build();

            IDataContainer AunionB = A.Union(B);

            Assert.Equal(5, AunionB.Count); // both have same keys, 5

            IDataContainer AA = (IDataContainer)AunionB["AA"];
            Assert.Equal(6, AA.Count); // 3 + 3

            IDataContainer CC = (IDataContainer)AunionB["CC"];
            Assert.Equal(3, CC.Count); // 2 + 1
        }

        [Fact]
        public void IDataContainer_Intersect_UsesValuesFromFirstForSameProperty()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .Data("B", 2)
                .Build();

            IDataContainer B = DataContainerBuilder.Create("B")
                .Data("A", 2)
                .Data("B", 1)
                .Build();

            IDataContainer AintersectB = A.Intersect(B);
            IDataContainer BintersectA = B.Intersect(A);

            int AA = (int)A["A"];
            int AB = (int)A["B"];

            int BA = (int)B["A"];
            int BB = (int)B["B"];

            int AUBA = (int)AintersectB["A"];
            int AUBB = (int)AintersectB["B"];
            int BUAA = (int)BintersectA["A"];
            int BUAB = (int)BintersectA["B"];

            Assert.Equal(AA, AUBA);
            Assert.Equal(AB, AUBB);

            Assert.Equal(BA, BUAA);
            Assert.Equal(BB, BUAB);
        }

        [Fact]
        public void IDataContainer_Intersect_UsesFirstsNameForResultName()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 22)
                .Build();

            IDataContainer B = DataContainerBuilder.Create("B")
                .Data("A", 33)
                .Build();

            IDataContainer AintersectB = A.Intersect(B);
            IDataContainer BintersectA = B.Intersect(A);

            Assert.Equal(A.Name, AintersectB.Name);
            Assert.Equal(B.Name, BintersectA.Name);
        }

        [Fact]
        public void IDataContainer_Intersect_MustWorkWithSingleLevelData()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .Data("C", 3)
                .Data("E", 5)
                .Data("G", 7)
                .Data("Z", 26)
                .Build();

            IDataContainer B = DataContainerBuilder.Create("A")
                .Data("B", 2)
                .Data("D", 4)
                .Data("F", 6)
                .Data("H", 8)
                .Data("Z", 28)
                .Build();

            IDataContainer AintersectB = A.Intersect(B);

            Assert.Equal(1, AintersectB.Count); // only 1 common

            var keys = AintersectB.GetKeys().ToList();
            keys.Sort();
            Assert.Equal("Z", string.Join("", keys)); // all keys are present.
        }

        [Fact]
        public void IDataContainer_Intersect_MustWorkWithMultiLevelData()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .Data("B", 2)
                .DataContainer("AA", b => b
                    .Data("A1", 11)
                    .Data("A2", 12)
                    .Data("A3", 13))
                .Data("C", 3)
                .DataContainer("CC", b => b
                    .Data("C1", 31)
                    .Data("C3", 33))
                .DataContainer("BB", b => b
                    .Data("B1", 21)
                    .Data("B2", 22))
                .Build();

            IDataContainer B = DataContainerBuilder.Create("B")
                .Data("A", 1)
                .Data("B", 2)
                .DataContainer("AA", b => b
                    .Data("A4", 14)
                    .Data("A5", 15)
                    .Data("A6", 16))
                .Data("C", 3)
                .DataContainer("CC", b => b
                    .Data("C2", 32))
                .DataContainer("BB", b => b
                    .Data("B1", 23)
                    .Data("B2", 24)
                    .Data("B3", 25))
                .Build();

            IDataContainer AintersectB = A.Intersect(B);

            Assert.Equal(6, AintersectB.Count); // both have same keys, 6

            IDataContainer AA = (IDataContainer)AintersectB["AA"];
            Assert.Equal(0, AA.Count); // nothing common

            IDataContainer CC = (IDataContainer)AintersectB["CC"];
            Assert.Equal(0, CC.Count); // nothing common

            IDataContainer BB = (IDataContainer)AintersectB["BB"];
            Assert.Equal(2, BB.Count); // 2 common, 1 only in B
        }

        [Fact]
        public void IDataContainer_IsIdenticalReturnsTrueForSingleLevelData()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .Data("B", 2)
                .Data("C", 3)
                .Build();

            IDataContainer B = DataContainerBuilder.Create("B")
                .Data("A", 2)
                .Data("B", 3)
                .Data("C", 4)
                .Build();

            Assert.True(A.IsIdentical(B));
            Assert.True(B.IsIdentical(A));
        }

        [Fact]
        public void IDataContainer_IsIdenticalReturnsFalseForSingleLevelData()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .Data("B", 2)
                .Build();

            IDataContainer B = DataContainerBuilder.Create("B")
                .Data("A", 2)
                .Data("B", 3)
                .Data("C", 4)
                .Build();

            Assert.False(A.IsIdentical(B));
            Assert.False(B.IsIdentical(A));
        }

        [Fact]
        public void IDataContainer_IsIdenticalReturnsTrueForMultiLevelData()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .DataContainer("AA", b => b
                    .Data("A1", 1)
                    .Data("A2", 2)
                    .Data("A3", 3))
                .Data("B", 2)
                .Data("C", 3)
                .DataContainer("CC", b => b
                    .Data("C1", 1)
                    .Data("C2", 2))
                .Build();

            IDataContainer B = DataContainerBuilder.Create("B")
                .Data("A", 2)
                .DataContainer("AA", b => b
                    .Data("A1", 1)
                    .Data("A2", 2)
                    .Data("A3", 3))
                .Data("B", 3)
                .Data("C", 4)
                .DataContainer("CC", b => b
                    .Data("C1", 1)
                    .Data("C2", 2))
                .Build();

            Assert.True(A.IsIdentical(B));
            Assert.True(B.IsIdentical(A));
        }

        [Fact]
        public void IDataContainer_IsIdenticalReturnsFalseForMultiLevelData()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .DataContainer("AA", b => b
                    .Data("A1", 1)
                    .Data("A3", 3))
                .Data("B", 2)
                .Data("C", 3)
                .DataContainer("CC", b => b
                    .Data("C1", 1)
                    .Data("C2", 2))
                .Build();

            IDataContainer B = DataContainerBuilder.Create("B")
                .Data("A", 2)
                .DataContainer("AA", b => b
                    .Data("A1", 1)
                    .Data("A2", 2)
                    .Data("A3", 3))
                .Data("B", 3)
                .Data("C", 4)
                .DataContainer("CC", b => b
                    .Data("C1", 1)
                    .Data("C2", 2))
                .Build();

            Assert.False(A.IsIdentical(B));
            Assert.False(B.IsIdentical(A));
        }

        [Fact]
        public void IDataContainer_IsIdenticalReturnsFalseForMultiLevelDataWithSameNumberOfKeys()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .DataContainer("AA", b => b
                    .Data("A1", 1)
                    .Data("A3", 3))
                .Data("B", 2)
                .Data("C", 3)
                .DataContainer("CC", b => b
                    .Data("C1", 1)
                    .Data("C2", 2))
                .Build();

            IDataContainer B = DataContainerBuilder.Create("B")
                .Data("A", 2)
                .DataContainer("AA", b => b
                    .Data("A3", 3))
                .Data("B", 3)
                .Data("C", 4)
                .DataContainer("CC", b => b
                    .Data("C1", 1)
                    .Data("C2", 2)
                    .Data("C3", 3))
                .Build();

            Assert.False(A.IsIdentical(B));
            Assert.False(B.IsIdentical(A));
        }

        [Fact]
        public void IDataContainer_Difference_RemovesCommonProperties()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .Data("B", 2)
                .Data("C", 3)
                .DataContainer("AA", b => b
                    .Data("A1", 11)
                    .Data("A2", 12))
                .Data("D", 4)
                .Build();

            IDataContainer B = DataContainerBuilder.Create("B")
                .Data("F", 1)
                .Data("B", 2)
                .Data("C", 3)
                .Data("X", 4)
                .Data("Y", 2)
                .Data("Z", 3)
                .Build();

            IDataContainer AdifferenceB = A.Difference(B);
            IDataContainer BdifferenceA = B.Difference(A);

            Assert.Equal(3, AdifferenceB.Count); // remove B,C from A
            Assert.Equal(4, BdifferenceA.Count); // remove B,C from B
        }

        [Fact]
        public void IDataContainer_Merge_MergesSingleLevelData()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .Data("B", 2)
                .Data("Z", 26)
                .Build();

            IDataContainer B = DataContainerBuilder.Create("B")
                .Data("A", 2)
                .Data("B", 3)
                .Data("C", 4)
                .Data("D", 4)
                .Build();

            A.Merge(B);

            Assert.Equal(5, A.Count);
        }

        [Fact]
        public void IDataContainer_Merge_MergesMultiLevelData()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .DataContainer("AA", b => b
                    .Data("A1", 1)
                    .Data("A3", 3))
                .Data("B", 2)
                .Data("C", 3)
                .DataContainer("CC", b => b
                    .Data("C5", 1)
                    .Data("C6", 2))
                .Build();

            IDataContainer B = DataContainerBuilder.Create("B")
                .Data("A", 2)
                .DataContainer("AA", b => b
                    .Data("A2", 3))
                .Data("B", 3)
                .Data("C", 4)
                .DataContainer("CC", b => b
                    .Data("C1", 1)
                    .Data("C2", 2)
                    .Data("C3", 3))
                .Build();

            A.Merge(B);

            Assert.Equal(5, A.Count);

            IDataContainer AA = (IDataContainer)A["AA"];
            Assert.Equal(3, AA.Count);

            IDataContainer CC = (IDataContainer)A["CC"];
            Assert.Equal(5, CC.Count);
        }

        #endregion

        #region Creation 

        [Theory]
        [InlineData(typeof(POCO), 6)]
        [InlineData(typeof(NestedPOCO), 7)]
        [InlineData(typeof(UninitializedPOCO), 4)] // Should not create data for null properties, or should it ??
        public void DataContainerBuilder_CreateObject_MustCreatesObjectWithCorrectCount(Type t, int count)
        {
            var obj = Activator.CreateInstance(t);

            IDataContainer data = DataContainerBuilder.CreateObject("Untitiled", obj);

            Assert.Equal(count, data.Count);
        }

        [Theory]
        [InlineData(typeof(POCO), 6)]
        [InlineData(typeof(NestedPOCO), 7)]
        [InlineData(typeof(UninitializedPOCO), 4)] // Should not create data for null properties, or should it ??
        public void PropertyContainerBuilder_CreateObject_MustCreatesObjectWithCorrectCount(Type t, int count)
        {
            var obj = Activator.CreateInstance(t);

            IPropertyContainer property = PropertyContainerBuilder.CreateObject("Untitled", obj);

            Assert.Equal(count, property.Count);
        }

        [Theory]
        [InlineData(typeof(POCO))]
        public void DataContainerBuilder_CreateObject_MustCreateObjectWithCorrectValue(Type t)
        {
            var obj = Activator.CreateInstance(t);

            IDataContainer data = DataContainerBuilder.CreateObject("Untitiled", obj);

            foreach (var item in data)
            {
                var expected = t.GetProperty(item.Name).GetValue(obj);
                var actual = item.GetValue();
                Assert.Equal(expected, actual);
            }
        }

        [Theory]
        [InlineData(typeof(POCO))]
        public void PropertyContainerBuilder_CreateObject_MustCreateObjectWithCorrectValue(Type t)
        {
            var obj = Activator.CreateInstance(t);

            IPropertyContainer property = PropertyContainerBuilder.CreateObject("Untitled", obj);

            foreach (var item in property)
            {
                var expected = t.GetProperty(item.Name).GetValue(obj);
                var actual = item.GetValue();
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void DataObjectFactory_GetDataObjectFor_CreatesCorrectObject()
        {
            const string Name = "Name";

            DataObject intObj = DataObjectFactory.GetDataObjectFor(Name, 1);
            DataObject byteObj = DataObjectFactory.GetDataObjectFor(Name, (byte)1);
            DataObject charObj = DataObjectFactory.GetDataObjectFor(Name, '@');
            DataObject floatObj = DataObjectFactory.GetDataObjectFor(Name, 1.42f);
            DataObject doubleObj = DataObjectFactory.GetDataObjectFor(Name, 3.14);
            DataObject stringObj = DataObjectFactory.GetDataObjectFor(Name, "hello");
            DataObject array1dObj = DataObjectFactory.GetDataObjectFor(Name, new int[] { 1, 2, 3 });
            DataObject array2dObj = DataObjectFactory.GetDataObjectFor(Name, new int[,] { { 1, 1 }, { 2, 2 } });
            DataObject colorObj = DataObjectFactory.GetDataObjectFor(Name, new Color(1, 1, 1));
            DataObject ptObject = DataObjectFactory.GetDataObjectFor(Name, new Point(1, 1));
            DataObject timeObj = DataObjectFactory.GetDataObjectFor(Name, TimeSpan.FromSeconds(1));
            DataObject dateObj = DataObjectFactory.GetDataObjectFor(Name, DateTime.Now);

            Assert.Equal(DataObjectType.Integer, intObj.Type);
            Assert.Equal(DataObjectType.Byte, byteObj.Type);
            Assert.Equal(DataObjectType.Char, charObj.Type);
            Assert.Equal(DataObjectType.Float, floatObj.Type);
            Assert.Equal(DataObjectType.Double, doubleObj.Type);
            Assert.Equal(DataObjectType.String, stringObj.Type);
            Assert.Equal(DataObjectType.Array1D, array1dObj.Type);
            Assert.Equal(DataObjectType.Array2D, array2dObj.Type);
            Assert.Equal(DataObjectType.Color, colorObj.Type);
            Assert.Equal(DataObjectType.Point, ptObject.Type);
            Assert.Equal(DataObjectType.TimeSpan, timeObj.Type);
            Assert.Equal(DataObjectType.DateTime, dateObj.Type);
        }

        [Fact]
        public void DataObjectFactory_GetPropertyObjectFor_CreatesCorrectObject()
        {
            const string Name = "Name";

            PropertyObject intObj = DataObjectFactory.GetPropertyObjectFor(Name, 1);
            PropertyObject byteObj = DataObjectFactory.GetPropertyObjectFor(Name, (byte)1);
            PropertyObject charObj = DataObjectFactory.GetPropertyObjectFor(Name, '@');
            PropertyObject floatObj = DataObjectFactory.GetPropertyObjectFor(Name, 1.42f);
            PropertyObject doubleObj = DataObjectFactory.GetPropertyObjectFor(Name, 3.14);
            PropertyObject stringObj = DataObjectFactory.GetPropertyObjectFor(Name, "hello");
            PropertyObject array1dObj = DataObjectFactory.GetPropertyObjectFor(Name, new int[] { 1, 2, 3 });
            PropertyObject array2dObj = DataObjectFactory.GetPropertyObjectFor(Name, new int[,] { { 1, 1 }, { 2, 2 } });
            PropertyObject colorObj = DataObjectFactory.GetPropertyObjectFor(Name, new Color(1, 1, 1));
            PropertyObject ptObject = DataObjectFactory.GetPropertyObjectFor(Name, new Point(1, 1));
            PropertyObject timeObj = DataObjectFactory.GetPropertyObjectFor(Name, TimeSpan.FromSeconds(1));
            PropertyObject dateObj = DataObjectFactory.GetPropertyObjectFor(Name, DateTime.Now);

            Assert.Equal(DataObjectType.Integer, intObj.Type);
            Assert.Equal(DataObjectType.Byte, byteObj.Type);
            Assert.Equal(DataObjectType.Char, charObj.Type);
            Assert.Equal(DataObjectType.Float, floatObj.Type);
            Assert.Equal(DataObjectType.Double, doubleObj.Type);
            Assert.Equal(DataObjectType.String, stringObj.Type);
            Assert.Equal(DataObjectType.Array1D, array1dObj.Type);
            Assert.Equal(DataObjectType.Array2D, array2dObj.Type);
            Assert.Equal(DataObjectType.Color, colorObj.Type);
            Assert.Equal(DataObjectType.Point, ptObject.Type);
            Assert.Equal(DataObjectType.TimeSpan, timeObj.Type);
            Assert.Equal(DataObjectType.DateTime, dateObj.Type);
        }

        #endregion

        #region Change Notification

        [Fact]
        public void IDataContainer_ShouldRaisePropertyChanged()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .Data("B", 2)
                .Data("C", 3)
                .Build();

            var listener = new PropertyChangedListener(A);

            A["A"] = 55;
            Assert.Equal("A", listener.LastChangedProperty);
            Assert.Single(listener.PropertiesChanged);

            A["B"] = 23;
            Assert.Equal("B", listener.LastChangedProperty);
            Assert.Equal(2, listener.PropertiesChanged.Count);

            A["C"] = 29;
            Assert.Equal("C", listener.LastChangedProperty);
            Assert.Equal(3, listener.PropertiesChanged.Count);
            
            Assert.Contains("A", listener.PropertiesChanged);
            Assert.Contains("B", listener.PropertiesChanged);
            Assert.Contains("C", listener.PropertiesChanged);
        }

        [Fact]
        public void IDataContainer_ShouldRaisePropertyChangedForNestedContainers()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .Data("B", 2)
                .Data("C", 3)
                .DataContainer("AA", b => b
                    .Data("A1", 23)
                    .Data("A2", 24)
                    .DataContainer("AAA", b => b
                        .Data("AA1", 3)))
                .Build();

            var listener = new PropertyChangedListener(A);

            A["A"] = 55;
            Assert.Equal("A", listener.LastChangedProperty);
            Assert.Single(listener.PropertiesChanged);

            IDataContainer AA = (IDataContainer)A["AA"];
            AA["A2"] = 42;
            Assert.Equal("AA.A2", listener.LastChangedProperty);
            Assert.Equal(2, listener.PropertiesChanged.Count);

            IDataContainer AAA = (IDataContainer)AA["AAA"];
            AAA["AA1"] = 5;
            Assert.Equal("AA.AAA.AA1", listener.LastChangedProperty);
            Assert.Equal(3, listener.PropertiesChanged.Count);

        }

        [Fact]
        public void IDataContainer_ShouldNotRaisePropertyChangedIfValueSame()
        {
            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("A", 1)
                .Data("B", 2)
                .Data("C", 3)
                .Build();

            var listener = new PropertyChangedListener(A);

            A["A"] = 1;
            Assert.True(string.IsNullOrEmpty(listener.LastChangedProperty));
            Assert.Empty(listener.PropertiesChanged);

            A["A"] = 2;
            Assert.Equal("A", listener.LastChangedProperty);
            Assert.NotEmpty(listener.PropertiesChanged);
        }

        [Fact]
        public void IDataContainer_ObjectDataObjectRaisesPropertyChangedWhenCLRObjectChanges()
        {
            BindingTestObject obj = new BindingTestObject();

            IDataContainer A = DataContainerBuilder.Create("A")
                .Data("Obj1", obj, SerializationFormat.Container)
                .Data("Obj2", obj, SerializationFormat.Xml)
                .Data("Obj3", obj, SerializationFormat.Json)
                .Build();

            var listener = new PropertyChangedListener(A);

            obj.IntProperty = 44;

            Assert.Contains("Obj1", listener.PropertiesChanged); 
            Assert.Contains("Obj2", listener.PropertiesChanged);
            Assert.Contains("Obj3", listener.PropertiesChanged);
            Assert.Equal("Obj3", listener.LastChangedProperty);
        }

        #endregion
    }
}
