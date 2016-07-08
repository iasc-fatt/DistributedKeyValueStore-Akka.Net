using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IASC.DistributedKeyValueStore.Common
{
    public static class Maybe
    {
        public static Maybe<T> Nothing<T>()
        {
            return new Maybe<T>();
        }

        public static Maybe<T> Just<T>(this T value)
        {
            return new Maybe<T>(value);
        }
    }

    public class Maybe<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> values;

        public Maybe()
            : this(new T[0])
        {
        }

        public Maybe(T value)
            : this(new[] { value })
        {
        }

        /// Used by serializers
        public Maybe(IEnumerable<T> otherMaybe)
        {
            if (otherMaybe == null)
                throw new ArgumentNullException("otherMaybe");
            if (otherMaybe.Count() > 1)
                throw new ArgumentException("must contain none or one element", "otherMaybe");

            this.values = otherMaybe;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}