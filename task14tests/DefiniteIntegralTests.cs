using Xunit;
using task14;

namespace task14tests
{
    public class DefiniteIntegralTests
    {
        [Fact]
        public void IntegralOfXFromMinus1To1()
        {
            var X = (double x) => x;
            double result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
            Assert.Equal(0, result, 4);
        }

        [Fact]
        public void IntegralOfSinFromMinus1To1()
        {
            var SIN = (double x) => Math.Sin(x);
            double result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);
            Assert.Equal(0, result, 4);
        }

        [Fact]
        public void IntegralOfTwoFrom0To5()
        {
            var TWO = (double x) => 2.0;
            double result = DefiniteIntegral.Solve(0, 5, TWO, 1e-6, 8);
            Assert.Equal(10, result, 5);
        }

        [Fact]
        public void IntegralOfXFrom0To5()
        {
            var X = (double x) => x;
            double result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
            Assert.Equal(12.5, result, 5);
        }

        [Fact]
        public void InvalidParametersThrowExceptions()
        {
            var func = (double x) => x;
            
            Assert.Throws<ArgumentException>(() => 
                DefiniteIntegral.Solve(5, 1, func, 0.1, 2));
            
            Assert.Throws<ArgumentException>(() => 
                DefiniteIntegral.Solve(0, 5, func, -0.1, 2));
            
            Assert.Throws<ArgumentException>(() => 
                DefiniteIntegral.Solve(0, 5, func, 0.1, 0));
        }
    }
}