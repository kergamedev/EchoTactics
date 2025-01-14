using System.Threading.Tasks;

namespace Echo.Common
{
    public interface ISaveSystem
    {
       Task SaveKeyValueAsync(string key, object value);
       Task<T> LoadKeyValueAsync<T>(string key);
    }
}