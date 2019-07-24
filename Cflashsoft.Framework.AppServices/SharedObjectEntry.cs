using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cflashsoft.Framework.AppServices
{
    internal class SharedObjectEntry
    {
        private int _index = 0;
        private object _value = null;
        private bool _ignoreIsDisposable = false;

        public int Index
        {
            get
            {
                return _index;
            }
        }

        public object Value
        {
            get
            {
                return _value;
            }
        }

        public bool IgnoreDisposable
        {
            get
            {
                return _ignoreIsDisposable;
            }
            set
            {
                _ignoreIsDisposable = value;
            }
        }

        public SharedObjectEntry()
        {

        }

        public SharedObjectEntry(int index, object value)
        {
            _index = index;
            _value = value;
        }
    }
}
