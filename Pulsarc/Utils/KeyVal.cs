using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.Utils
{
    public class KeyVal<Key_, Value_>
    {
        public Key_ Key { get; set; }
        public Value_ Value { get; set; }

        public KeyVal() { }

        public KeyVal(Key_ key, Value_ val)
        {
            Key = key;
            Value = val;
        }
    }
}
