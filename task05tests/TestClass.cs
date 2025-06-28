using Xunit;
using System;
using System.Linq;
using task05;

namespace task05tests;

public class TestClass
{
    public int PublicField;
    private string _privateField;
    public int Property { get; set; }
    public static string StaticProperty { get; set; }
    
    public void Method() { }
    public int MethodWithParams(int a, string b) => 0;
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods().ToList();

        Assert.Contains("Method", methods);
        Assert.Contains("MethodWithParams", methods);
        Assert.DoesNotContain("PrivateMethod", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields().ToList();

        Assert.Contains("PublicField", fields);
        Assert.Contains("_privateField", fields);
        Assert.DoesNotContain("NonExistentField", fields);
    }

    [Fact]
    public void GetProperties_ReturnsAllProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties().ToList();

        Assert.Contains("Property", properties);
        Assert.Contains("StaticProperty", properties);
        Assert.Equal(2, properties.Count);
    }

    [Fact]
    public void GetMethodParams_ReturnsCorrectParameters()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var paramsInfo = analyzer.GetMethodParams("MethodWithParams").ToList();

        Assert.Contains("Int32 a", paramsInfo);
        Assert.Contains("String b", paramsInfo);
        Assert.Contains("Returns: Int32", paramsInfo);
        Assert.Equal(3, paramsInfo.Count);
    }

    [Fact]
    public void GetMethodParams_ForNonExistentMethod_ReturnsEmpty()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var paramsInfo = analyzer.GetMethodParams("NonExistentMethod");

        Assert.Empty(paramsInfo);
    }

    [Fact]
    public void HasAttribute_WhenAttributePresent_ReturnsTrue()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));
        Assert.True(analyzer.HasAttribute<SerializableAttribute>());
    }

    [Fact]
    public void HasAttribute_WhenAttributeAbsent_ReturnsFalse()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        Assert.False(analyzer.HasAttribute<SerializableAttribute>());
    }
    
    [Fact]
    public void GetPublicMethods_IncludesStaticMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(Math));
        var methods = analyzer.GetPublicMethods().ToList();

        Assert.Contains("Abs", methods);
        Assert.Contains("Sqrt", methods);
    }
    
    [Fact]
    public void GetMethodParams_Format_IsCorrect()
    {
        var analyzer = new ClassAnalyzer(typeof(string));
        var paramsInfo = analyzer.GetMethodParams("Clone").ToList();
        
        Assert.Contains("Returns: Object", paramsInfo);
    }
}