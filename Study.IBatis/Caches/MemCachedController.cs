using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Enyim.Caching;
using Enyim.Caching.Memcached;
using IBatisNet.DataMapper.Configuration.Cache;

namespace Study.IBatis.Caches
{
    public sealed class MemCachedController : ICacheController
    {
        private string _keyPrefix = "IBatis:";
        private TimeSpan _timeout = TimeSpan.FromHours(24);
        private MemcachedClient _memcachedClient;

        public void Configure(System.Collections.IDictionary properties)
        {
            if (properties.Contains("keyPrefix"))
            {
                _keyPrefix = (string)properties["keyPrefix"];
            }

            if (properties.Contains("timeoutHours"))
            {
                _timeout = TimeSpan.Parse((string)properties["timeoutHours"]);
            }

            _memcachedClient = new MemcachedClient();
        }

        public void Flush()
        {
            _memcachedClient.FlushAll();
        }

        public object Remove(object key)
        {
            var result = _memcachedClient.Get(this.GetKey(key));
            if (result != null)
            {
                _memcachedClient.Remove(this.GetKey(key));
            }

            return result;
        }

        public object this[object key]
        {
            get
            {
                return _memcachedClient.Get(this.GetKey(key));
            }
            set
            {
                _memcachedClient.Store(StoreMode.Set, this.GetKey(key), value, _timeout);
            }
        }

        private string GetKey(object key)
        {
            return string.Format("{0}{1}", _keyPrefix, key);
        }
    }
}
