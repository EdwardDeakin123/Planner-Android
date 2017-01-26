using System;

namespace Front_End.Models
{
    // This class is intended to be used when caching data to the disk.
    // It has two fields, TTL and Object.
    // If TTL is exceeded, the cache should be discarded.
    public class ObjectCacheModel<T>
    {
        // TODO use generics so this can be serialized
        public const int DEFAULT_TTL = 5;
        public DateTime TTL { get; set; }
        public T Object { get; set; }

        public ObjectCacheModel()
        {
            // When the object is created. Set a TTL of now + DEFAULT_TTL minutes.
            // This will prevent stale data from being used.
            TTL = DateTime.Now.AddMinutes(DEFAULT_TTL);
        }

        public bool Expired
        {
            get
            {
                if (TTL > DateTime.Now)
                    // The Time to Live is in the future, this cache has not expired.
                    return false;

                // The time to live is in the past, this cache has expired.
                return true;
            }
        }
    }
}