namespace OccurrencesFinder.Utilities
{
    using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReconciliationService.WebApi.Utilities
{
    public static class ThrottlingTaskRunner
    {
        public static async Task ExecuteMultipleAsync(this IEnumerable<Func<Task>> taskProducers,
                                                      int maxTasksAtOnce = 10)
        {
            var tasks = new List<Task>();
            foreach (var taskProducer in taskProducers)
            {
                tasks.Add(taskProducer());
                if (tasks.Count == maxTasksAtOnce)
                {
                    var completed = await Task.WhenAny(tasks);
                    tasks.Remove(completed);
                }
            }

            await Task.WhenAll(tasks);
        }

        public static async Task<List<T>> ExecuteMultipleWithResultsAsync<T>(
            this IEnumerable<Task<T>> taskProducers,
            int maxTasksAtOnce = 10)
        {
            var result = new List<T>();
            var tasks = new List<Task<T>>();
            foreach (var taskProducer in taskProducers)
            {
                tasks.Add(taskProducer);
                if (tasks.Count == maxTasksAtOnce)
                {
                    var completed = await Task.WhenAny(tasks);
                    result.Add(completed.Result);
                    tasks.Remove(completed);
                }
            }

            var remainingResults = await Task.WhenAll(tasks);
            result.AddRange(remainingResults);
            return result;
        }

        public static async Task<List<T>> ExecuteMultipleWithResultsAsync<T>(
            this IEnumerable<Func<Task<T>>> taskProducers,
            int maxTasksAtOnce = 10)
        {
            var result = new List<T>();
            var tasks = new List<Task<T>>();
            foreach (var taskProducer in taskProducers)
            {
                tasks.Add(taskProducer());
                if (tasks.Count == maxTasksAtOnce)
                {
                    var completed = await Task.WhenAny(tasks);
                    result.Add(completed.Result);
                    tasks.Remove(completed);
                }
            }

            var remainingResults = await Task.WhenAll(tasks);
            result.AddRange(remainingResults);
            return result;
        }
    }
}
}