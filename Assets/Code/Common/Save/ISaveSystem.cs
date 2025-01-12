using System.Threading.Tasks;

namespace Echo.Common
{
    public interface ISaveSystem
    {
       Task SaveKeyValue(string key, object value);
       Task<T> LoadKeyValue<T>(string key);
    }
}