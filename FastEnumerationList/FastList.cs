using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FastEnumerationList
{
  public class FastList<T> : List<T>, IEnumerable<T>
  {
    static readonly FieldInfo _itemsInfo = typeof(List<T>).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).First(f => f.Name == "_items");

    public T[] Items => (T[])_itemsInfo.GetValue(this);

    public FastList() { }

    public FastList(IEnumerable<T> collection) : base(collection) { }

    public FastList(int capacity) : base(capacity) { }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new FastEnumerator(Items);

    new public IEnumerator GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

    public sealed class FastEnumerator : IEnumerator<T>
    {
      T[] items;
      int index;

      public FastEnumerator(T[] items)
      {
        this.items = items;
      }

      public void Dispose() { }

      public bool MoveNext() => ++index < items.Length;

      public T Current => items[index];

      object IEnumerator.Current => Current;

      void IEnumerator.Reset() => index = 0;

    }
  }
}
