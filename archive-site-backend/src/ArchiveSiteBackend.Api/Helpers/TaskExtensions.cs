using System.Threading.Tasks;

namespace ArchiveSiteBackend.Api.Helpers {
    public static class TaskExtensions {
        public static void AwaitSynchronously(this Task task) {
            task.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static T AwaitSynchronously<T>(this Task<T> task) {
            return task.ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
