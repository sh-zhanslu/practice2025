using Xunit;

public class IntegratorTests
{
    [Fact]
    public void Solve_IntegralSinX_SymmetricInterval_ReturnsZero()
    {
        var integrator = new Integrator();
        double result = integrator.Solve(Math.Sin, -100, 100, 0.1, 4);
        Assert.True(Math.Abs(result) < 1e-4);
    }

    [Fact]
    public void SolveSequential_IntegralSinX_SymmetricInterval_ReturnsZero()
    {
        var integrator = new Integrator();
        double result = integrator.SolveSequential(Math.Sin, -100, 100, 0.1);
        Assert.True(Math.Abs(result) < 1e-4);
    }
}