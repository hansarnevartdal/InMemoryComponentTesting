using System.Threading.Tasks;

namespace Component.Demo.Facades
{
    public interface IOtherComponentFacade
    {
        Task<string> GetSecret();
    }
}