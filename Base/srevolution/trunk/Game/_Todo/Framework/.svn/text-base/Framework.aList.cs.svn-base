using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SrxRevo
{
    public partial class Systems
    {
        public class aList<T> : List<T>
        {
            public EventHandler update;
            public new void Add(T item)
            {
                base.Add(item);

                if (update != null)
                    update(this, EventArgs.Empty);
            }
            public new void Remove(T item)
            {
                base.Remove(item);

                if (update != null)
                    update(this, EventArgs.Empty);
            }
        }
        public class bList<T> : List<T>
        {
            public new void Add(T item)
            {
                if (base.Count <= 10)
                {
                    base.Add(item);
                }
                else
                {
                    for (byte b = 0; b < 10; b++)
                        base[b] = base[b + 1];
                    base[10] = item;
                }
            }
        }
        public class sList<T> : List<T>
        {
            public new void Add(T item)
            {
                T i = base.Find(FindNotFlyingBirds);
            }
            private bool FindNotFlyingBirds(T bird)
            {
                return false;
            }
        }
    }
}