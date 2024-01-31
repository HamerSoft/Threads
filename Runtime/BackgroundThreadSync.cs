using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HamerSoft.Threads
{
    public class BackgroundThreadSync
    {
        public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter()
        {
            return Task.Run(() => {}).ConfigureAwait(false).GetAwaiter();
        }
    }
}