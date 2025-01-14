using Echo.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.Services.CloudSave;

namespace Echo.Game
{
    public class CloudSaveSystem : ISaveSystem
    {
        public async Task SaveKeyValueAsync(string key, object value)
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object> {{ key, value }});
        }

        public async Task<T> LoadKeyValueAsync<T>(string key)
        {
            var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>{key});
            if (!data.TryGetValue(key, out var dataItem))
                return default(T);

            return dataItem.Value.GetAs<T>();
        }
    }
}