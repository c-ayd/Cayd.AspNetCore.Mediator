using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Utilities
{
    public static class TaskConverter
    {
        public static async Task<object?> Convert<T>(Task<T> task)
            => await task;
    }
}
