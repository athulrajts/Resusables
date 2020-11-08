using System;
using Xunit;
using KEI.Infrastructure.Configuration;

namespace KEI.Infrastructure.Core.Tests
{
    public class DataContainerTests
    {

        #region Creation 

        [Theory]
        [InlineData(typeof(POCO),6)]
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
                var actual = item.Value;

                if (expected is Enum && item.Value is Selector s)
                {
                    var enumValue = Enum.Parse(expected.GetType(), s.SelectedItem);
                    Assert.Equal(expected, enumValue);
                }
                else
                {
                    Assert.Equal(expected, actual);
                }
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
                var actual = item.Value;
                Assert.Equal(expected, actual);
            }
        }

        #endregion

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

            var serializedData = XmlHelper.Serialize(data);
            var serializedProperty = XmlHelper.Serialize(property);

            var recreatedData = XmlHelper.DeserializeFromString<DataContainer>(serializedData);
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
        public void DataContainerBase_Get_MustGetCorrectValue()
        {
            const string PROP_NAME = "IntProperty";

            IPropertyContainer property = PropertyContainerBuilder.Create()
                .WithProperty(PROP_NAME, 42)
                .Build();

            int prop = 0;
            bool containsProperty = property.Get(PROP_NAME, ref prop);

            Assert.True(containsProperty);
            Assert.NotEqual((decimal)0, prop, 0);
            Assert.Equal(42, prop);

            int prop2 = property.Get<int>(PROP_NAME);

            Assert.NotEqual((decimal)0, prop2);
            Assert.Equal(42, prop2);
        }

        [Fact]
        public void DataContainerBase_Get_MustReturnDefaultIfNotPresent()
        {
            const string PROP_NAME = "IntProperty";

            IPropertyContainer property = PropertyContainerBuilder.Create()
                .WithProperty(PROP_NAME, 42)
                .Build();

            int prop = 0;
            bool containsProperty = property.Get("blah", ref prop);

            Assert.False(containsProperty);
            Assert.Equal(default, prop);

            int prop2 = property.Get<int>("blah");

            Assert.Equal(default, prop2);
        }

        [Fact]
        public void DataContainerBase_Set_MustSetValue()
        {
            const string PROP_NAME = "IntProperty";
            const int VALUE = 42;
            const int SET_VALUE = 14;

            IPropertyContainer property = PropertyContainerBuilder.Create()
                .WithProperty(PROP_NAME, VALUE)
                .Build();

            property.Set(PROP_NAME, SET_VALUE);

            int value = 0;
            property.Get(PROP_NAME, ref value);
            int value2 = property.Get<int>(PROP_NAME);

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

            Assert.False(property.ContainsProperty(PROP_NAME));

            property.Put(PROP_NAME, SET_VALUE);

            Assert.True(property.ContainsProperty(PROP_NAME));

            int value = 0;
            property.Get(PROP_NAME, ref value);

            Assert.Equal(SET_VALUE, value);
        }

        [Fact]
        public void DataContainerBase_Put_MustUpdateValueIfPresent()
        {
            const string PROP_NAME = "IntProperty";
            const int VALUE = 42;
            const int SET_VALUE = 14;

            IPropertyContainer property = PropertyContainerBuilder.Create()
                .WithProperty(PROP_NAME, VALUE)
                .Build();

            int originalValue = 0;
            property.Get(PROP_NAME, ref originalValue);

            Assert.True(property.ContainsProperty(PROP_NAME));

            property.Put(PROP_NAME, SET_VALUE);

            Assert.True(property.ContainsProperty(PROP_NAME));

            int value = 0;
            property.Get(PROP_NAME, ref value);

            Assert.NotEqual(originalValue, value);
            Assert.Equal(SET_VALUE, value);
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
                .WithProperty(PROP_NAME, VALUE)
                .Build();

            var bindingTarget = new BindingTestObject();

            Assert.NotEqual(VALUE, bindingTarget.IntProperty);

            pc.SetBinding(PROP_NAME, () => bindingTarget.IntProperty, BindingMode.OneWay);

            Assert.Equal(VALUE, bindingTarget.IntProperty);

            pc.Set(PROP_NAME, NEW_VALUE);

            Assert.Equal(NEW_VALUE, bindingTarget.IntProperty);

            bindingTarget.IntProperty = 32;

            int propertyValue = 0;
            pc.Get(PROP_NAME, ref propertyValue);

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
                .WithProperty(PROP_NAME, VALUE)
                .Build();

            var bindingTarget = new BindingTestObject();

            Assert.NotEqual(VALUE, bindingTarget.IntProperty);

            property.SetBinding(PROP_NAME, () => bindingTarget.IntProperty, BindingMode.TwoWay);

            Assert.Equal(VALUE, bindingTarget.IntProperty);

            property.Set(PROP_NAME, NEW_VALUE);

            Assert.Equal(NEW_VALUE, bindingTarget.IntProperty);

            bindingTarget.IntProperty = 32;

            int propertyValue = 0;
            property.Get(PROP_NAME, ref propertyValue);

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
                .WithProperty(PROP_NAME, VALUE)
                .Build();

            var bindingTarget = new BindingTestObject();

            Assert.NotEqual(VALUE, bindingTarget.IntProperty);

            property.SetBinding(PROP_NAME, () => bindingTarget.IntProperty, BindingMode.OneWayToSource);

            Assert.Equal(VALUE, bindingTarget.IntProperty);

            property.Set(PROP_NAME, NEW_VALUE);

            Assert.NotEqual(NEW_VALUE, bindingTarget.IntProperty);
            Assert.Equal(VALUE, bindingTarget.IntProperty);

            bindingTarget.IntProperty = 32;

            int propertyValue = 0;
            property.Get(PROP_NAME, ref propertyValue);

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
                .WithProperty(PROP_NAME, VALUE)
                .Build();

            var bindingTarget = new BindingTestObject();

            Assert.NotEqual(VALUE, bindingTarget.IntProperty);

            property.SetBinding(PROP_NAME, () => bindingTarget.IntProperty, BindingMode.OneTime);

            Assert.Equal(VALUE, bindingTarget.IntProperty);

            property.Set(PROP_NAME, NEW_VALUE);

            Assert.NotEqual(NEW_VALUE, bindingTarget.IntProperty);
            Assert.Equal(VALUE, bindingTarget.IntProperty);

            bindingTarget.IntProperty = 32;

            int propertyValue = 0;
            property.Get(PROP_NAME, ref propertyValue);

            property.RemoveBinding(PROP_NAME, () => bindingTarget.IntProperty);

            Assert.NotEqual(32, propertyValue);
            Assert.Equal(NEW_VALUE, propertyValue);
        }

        #endregion
    }
}
