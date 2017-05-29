using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncSum
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentTokenSource = new CancellationTokenSource();
            var previousTokenSource = new CancellationTokenSource();
            var tasks = new List<Task>();
            Task.Run(() => { while (true) ShowTasksStatistic(tasks); });
            while (true)
            {
                var upperBound = GetUpperBound(previousTokenSource);
                var task = CalculateSumAsync(upperBound, currentTokenSource.Token);
                tasks.Add(task);
                previousTokenSource = currentTokenSource;
                currentTokenSource = new CancellationTokenSource();
            }
        }

        private static void ShowTasksStatistic(List<Task> tasks)
        {
            var done = tasks.Count(t => t.Status == TaskStatus.RanToCompletion);
            var aborted = tasks.Count(t => t.Status == TaskStatus.Canceled);
            Console.WriteLine($"Cancelled tasks count - {aborted}. Successfully completed - {done}");
            Thread.Sleep(5000);
        }

        private static int Sum(int rangeEnd, CancellationToken token)
        {
            var sumResult = 0;
            for (int i = 1; i < rangeEnd; i++) { token.ThrowIfCancellationRequested(); sumResult += i; Thread.Sleep(10); }
            return sumResult;
        }

        private static Task CalculateSumAsync(int upperBound, CancellationToken token)
        {
            return Task.Run(() => {
                var result = Sum(upperBound, token);
                Console.WriteLine($"result of sum for {upperBound} is {result}"); }, token);
        }

        private static int GetUpperBound(CancellationTokenSource tokenSource)
        {
            var enteredValue = Console.ReadLine();
            tokenSource.Cancel();
            return int.Parse(enteredValue);
        }
    }
}