using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Integrator
{
    public double Solve(Func<double, double> f, double a, double b, double h, int numberOfThreads)
    {
        long n = (long)((b - a) / h);
        if (n == 0) n = 1;
        double actualStep = (b - a) / n;
        
        if (numberOfThreads == 1)
            return SolveSequential(f, a, b, h);

        double total = 0.0;
        object lockObj = new object();
        var chunks = CreateChunks(n, numberOfThreads);

        Parallel.ForEach(chunks, new ParallelOptions { MaxDegreeOfParallelism = numberOfThreads }, chunk =>
        {
            double localSum = 0.0;
            for (long i = chunk.Item1; i < chunk.Item2; i++)
            {
                double x = a + (i + 0.5) * actualStep;
                localSum += f(x);
            }
            lock (lockObj) 
            {
                total += localSum * actualStep;
            }
        });

        return total;
    }

    public double SolveSequential(Func<double, double> f, double a, double b, double h)
    {
        long n = (long)((b - a) / h);
        if (n == 0) n = 1;
        double actualStep = (b - a) / n;
        double total = 0.0;
        
        for (long i = 0; i < n; i++)
        {
            double x = a + (i + 0.5) * actualStep;
            total += f(x) * actualStep;
        }
        return total;
    }

    private List<Tuple<long, long>> CreateChunks(long n, int numberOfThreads)
    {
        var chunks = new List<Tuple<long, long>>();
        long chunkSize = n / numberOfThreads;
        long remainder = n % numberOfThreads;
        long start = 0;

        for (int i = 0; i < numberOfThreads; i++)
        {
            long end = start + chunkSize + (i < remainder ? 1 : 0);
            chunks.Add(Tuple.Create(start, end));
            start = end;
        }
        return chunks;
    }
}