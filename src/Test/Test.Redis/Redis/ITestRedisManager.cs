using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FastNet.Framework.Redis;

namespace Test.Redis
{
    public interface ITestRedisManager 
    {
        void SetStringKey(string key,string value);
        string GetStringKey(string key);
        void RemoveStringKey(string key);

        Task<bool> LockAsync(string key);

        Task UnLockAsync(string key);


        void SetUserInfo(string key, string hashField, UserInfo value);
        UserInfo GetUserInfo(string key, int userID);
        void SetUserList(string key, List<UserInfo> value);
    }
}
