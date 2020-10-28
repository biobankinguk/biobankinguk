using System.Threading.Tasks;

namespace Biobanks.DataLoader.Services
{
    public interface IDataLoadService
    {
        Task LoadData();
    }
}