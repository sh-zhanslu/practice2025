using System;
using System.IO;
using System.Reflection;
using System.Linq;
using Xunit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using task11;

namespace task11tests;

public class CalculatorTests
{
    private const string CalculatorSource = @"
using task11;

public class Calculator : ICalculator
{
    public int Add(int a, int b) => a + b;
    public int Minus(int a, int b) => a - b;
    public int Mul(int a, int b) => a * b;
    public int Div(int a, int b) => a / b;
}";

    [Fact]
    public void Calculator_Operations_ShouldWorkCorrectly()
    {
        var calculator = CompileAndCreateCalculator();
        
        Assert.Equal(15, calculator.Add(10, 5));
        Assert.Equal(5, calculator.Minus(10, 5));
        Assert.Equal(50, calculator.Mul(10, 5));
        Assert.Equal(2, calculator.Div(10, 5));
    }

    [Fact]
    public void Div_ByZero_ShouldThrow()
    {
        var calculator = CompileAndCreateCalculator();
        Assert.Throws<DivideByZeroException>(() => calculator.Div(10, 0));
    }

    private ICalculator CompileAndCreateCalculator()
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(CalculatorSource);
        
        var references = new MetadataReference[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location)
        };

        var compilation = CSharpCompilation.Create(
            "DynamicAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);
        
        if (!result.Success)
        {
            throw new InvalidOperationException(
                $"Ошибка компиляции: {string.Join("\n", result.Diagnostics)}");
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());
        var type = assembly.GetType("Calculator");
        return (ICalculator)Activator.CreateInstance(type);
    }
}