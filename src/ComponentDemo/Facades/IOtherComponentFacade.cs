using System.Collections.Generic;
using System.Threading.Tasks;

namespace Component.Demo.Facades
{
    public interface IOtherComponentFacade
    {
        Task<string> GetSecret();
        Task<string> PostData(List<string> data);
    }
}