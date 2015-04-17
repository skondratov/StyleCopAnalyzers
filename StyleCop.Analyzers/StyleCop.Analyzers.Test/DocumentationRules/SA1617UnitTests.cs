﻿namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;
    using Microsoft.CodeAnalysis.CodeFixes;


    /// <summary>
    /// This class contains unit tests for <see cref="SA1617VoidReturnValueMustNotBeDocumented"/>-
    /// </summary>
    public class SA1617UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithReturnValueNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    public ClassName Method() { return null; }

    /// <value>
    /// Foo
    /// </value>
    public delegate ClassName MethodDelegate();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithReturnValueWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    /// <returns>null</returns>
    public ClassName Method() { return null; }

    /// <value>
    /// Foo
    /// </value>
    /// <returns>Some value</returns>
    public delegate ClassName MethodDelegate();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ClassName Method() { return null; }

    public delegate ClassName MethodDelegate();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ClassName Method() { return null; }

    /// <inheritdoc/>
    public delegate ClassName MethodDelegate();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithoutReturnValueNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    public void Method() { }

    /// <value>
    /// Foo
    /// </value>
    public delegate void MethodDelegate();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithoutReturnValueWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    /// <returns>null</returns>
    public void Method() { }

    /// <value>
    /// Foo
    /// </value>
    /// <returns>Some value</returns>
    public delegate void MethodDelegate();
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 9),
                this.CSharpDiagnostic().WithLocation(16, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }


        [Fact]
        public async Task TestCodeFixWithNoData()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    /// <returns>null</returns>
    public void Method() { }
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    public void Method() { }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestCodeFixShareLineWithValue()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value><returns>null</returns>
    public void Method() { }
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(9, 17)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    public void Method() { }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestCodeFixBeforeValue()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <returns>null</returns> <value>
    /// Foo
    /// </value>
    public void Method() { }
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(7, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    public void Method() { }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1617VoidReturnValueMustNotBeDocumented();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1617CodeFixProvider();
        }
    }
}
