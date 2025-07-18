using System;
using System.Threading;

namespace task14
{
    public static class DefiniteIntegral
    {
        public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
        {
            if (a >= b) throw new ArgumentException("a должно быть < b");
            if (step <= 0) throw new ArgumentException("Шаг +");
            if (threadsNumber <= 0) throw new ArgumentException("Количество потоков +");
            
            double totalResult = 0.0;
            object lockObj = new object();
            Thread[] threads = new Thread[threadsNumber];
            
            double segmentLength = (b - a) / threadsNumber;
            
            for (int i = 0; i < threadsNumber; i++)
            {
                double start = a + i * segmentLength;
                double end = (i == threadsNumber - 1) ? b : start + segmentLength;
                
                threads[i] = new Thread(() =>
                {
                    double localResult = CalculateSegment(start, end, function, step);
                    
                    lock (lockObj)
                    {
                        totalResult += localResult;
                    }
                });
                
                threads[i].Start();
            }
            
            foreach (var thread in threads)
            {
                thread.Join();
            }
            
            return totalResult;
        }
        
        private static double CalculateSegment(double a, double b, Func<double, double> function, double step)
        {
            double sum = 0.0;
            double current = a;
            
            while (current < b)
            {
                double next = Math.Min(current + step, b);
                double delta = next - current;
                
                sum += (function(current) + function(next)) * delta / 2;
                current = next;
            }
            
            return sum;
        }
    }
}