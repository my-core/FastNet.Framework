using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TCM.SuperX.DiceService.Managers
{
    public interface IRedisLockCacheManager
    {
        Task<bool> LockAsync(string key);

        Task UnLockAsync(string key);
    }
}
