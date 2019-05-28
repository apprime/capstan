using System.Threading.Tasks;

namespace Capstan.Events
{
    public interface CapstanEvent
    {
        void Process();
        Task ProcessAsync();
    }
}
