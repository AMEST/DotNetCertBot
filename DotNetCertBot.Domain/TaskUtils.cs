using System;
using System.Threading.Tasks;

namespace DotNetCertBot.Domain
{
    public static class TaskUtils
    {
        public static async Task<T> RunWithRetry<T>(Func<Task<T>> action, int retryCount = 3)
        {
            try
            {
                var result = await Task.Run(action);
                return result;
            }
            catch when (retryCount-- > 0)
            {
            }

            return await RunWithRetry(action, retryCount);
        }

        public static async Task RunWithRetry(Func<Task> action, int retryCount = 3)
        {
            try
            {
                await Task.Run(action);
                return;
            }
            catch when (retryCount-- > 0)
            {
            }

            await RunWithRetry(action, retryCount);
        }
    }
}