using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing.MSTest;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using WinUI3Utilities.Analyzer;
using WinUI3Utilities.CodeFix;

namespace WinUI3Utilities.Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public async void TestMethod1()
    {
        var codeFixVerifier = new CodeFixVerifier<DependencyPropertyAnalyzer, DependencyPropertyCodeFixProvider001>();
        var cSharpCodeFixTest = new CSharpCodeFixTest<DependencyPropertyAnalyzer, DependencyPropertyCodeFixProvider001, MSTestVerifier>();
        await cSharpCodeFixTest.RunAsync();
    }
    //public static readonly DependencyProperty AProperty = DependencyProperty.Register(nameof(A), typeof(CodeFixTest), typeof(CodeFixTest), new(new CodeFixTest()));

    //public CodeFixTest A { get => (CodeFixTest)GetValue(AProperty); set => SetValue(AProperty, value); }

    //public static readonly DependencyProperty B = DependencyProperty.Register("D", typeof(int), typeof(CodeFixTest), new PropertyMetadata(1, TestMethod1));

    //public int C
    //{
    //    get { return (int)GetValue(B); }
    //    private set { SetValue(B, value); }
    //}

    //public static void TestMethod1(DependencyObject o, DependencyPropertyChangedEventArgs e) { }
}
