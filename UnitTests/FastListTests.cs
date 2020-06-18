using FastEnumerationList;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace UnitTests
{
  [TestClass]
  public class FastListTests
  {
    [TestMethod]
    public void GetEnumerator_ListDeclared_GotListEnumerator()
    {
      List<int> listDeclared = new FastList<int>();

      Assert.AreEqual(GetListEnumeratorType<int>(), listDeclared.GetEnumerator().GetType());
    }

    [TestMethod]
    public void GetEnumerator_FastListDeclared_GotListEnumerator()
    {
      FastList<int> fastListDeclared = new FastList<int>();

      Assert.AreNotEqual(GetListEnumeratorType<int>(), fastListDeclared.GetEnumerator().GetType());
    }

    Type GetListEnumeratorType<T>() => new List<T>().GetEnumerator().GetType();


    [TestMethod]
    public void PerfTest()
    {
      int testCycles = 9_000;

      var listTimes_Foreach = new List<TimeSpan>();
      var fastListTimes_Foreach = new List<TimeSpan>();
      var array_Foreach = new List<TimeSpan>();

      var listTimes_For = new List<TimeSpan>();
      var fastListTimes_For = new List<TimeSpan>();
      var array_For = new List<TimeSpan>();

      long[] numbers = Enumerable
        .Range(0, 500_000)
        .Select(Convert.ToInt64)
        .ToArray();

      long refSum = checked(numbers.Sum());
      var stopWatch = new Stopwatch();

      var testList = new List<long>(numbers);
      var testFastList = new FastList<long>(numbers);

      while (testCycles-- > 0)
      {
        TestForeach(testList, listTimes_Foreach, stopWatch, refSum);
        TestForeach(testFastList, fastListTimes_Foreach, stopWatch, refSum);
        TestForeach(numbers, array_Foreach, stopWatch, refSum);

        TestFor(testList, listTimes_For, stopWatch, refSum);
        TestFor(testFastList, fastListTimes_For, stopWatch, refSum);
        TestFor_Array(numbers, array_For, stopWatch, refSum);
      }

      DiscardMinMaxValues(listTimes_Foreach);
      DiscardMinMaxValues(fastListTimes_Foreach);
      DiscardMinMaxValues(array_Foreach);

      DiscardMinMaxValues(listTimes_For);
      DiscardMinMaxValues(fastListTimes_For);
      DiscardMinMaxValues(array_For);

      TimeSpan listTimes_Foreach_Sum = Sum(listTimes_Foreach);
      TimeSpan fastListTimes_Foreach_Sum = Sum(fastListTimes_Foreach);
      TimeSpan array_Foreach_Sum = Sum(array_Foreach);
      
      TimeSpan listTimes_For_Sum = Sum(listTimes_For);
      TimeSpan fastListTimes_For_Sum = Sum(fastListTimes_For);
      TimeSpan array_For_Sum = Sum(array_For);

      var strBuilder = new StringBuilder();
            
      strBuilder.AppendLine($"{nameof(listTimes_Foreach)} {listTimes_Foreach_Sum}");
      strBuilder.AppendLine($"{nameof(fastListTimes_Foreach)} {fastListTimes_Foreach_Sum}");
      strBuilder.AppendLine($"{nameof(array_Foreach)} {array_Foreach_Sum}");
      
      strBuilder.AppendLine($"{nameof(listTimes_For)} {listTimes_For_Sum}");      
      strBuilder.AppendLine($"{nameof(fastListTimes_For)} {fastListTimes_For_Sum}");
      strBuilder.AppendLine($"{nameof(array_For)} {array_For_Sum}");
      strBuilder.AppendLine();

      string result = strBuilder.ToString();

      File.AppendAllText(@"c:\Users\JC\Desktop\res.txt", result);

      Debug.Write(result);

      Assert.IsTrue(listTimes_Foreach_Sum > fastListTimes_Foreach_Sum);
      Assert.IsTrue(array_Foreach_Sum > fastListTimes_Foreach_Sum);
    }

    static void DiscardMinMaxValues(List<TimeSpan> timeSpans)
    {
      var max = timeSpans.Max();
      var min = timeSpans.Min();

      Debug.WriteLine($"Max {max}, min {min}.");

      Remove(max);
      Remove(min);

      void Remove(TimeSpan timeSpan) => timeSpans.RemoveAt(timeSpans.IndexOf(timeSpan));
    }

    static TimeSpan Sum(List<TimeSpan> timeSpans) => timeSpans.Aggregate(new TimeSpan(0, 0, 0), (acc, ts) => acc.Add(ts));

    void TestForeach(IEnumerable<long> list, List<TimeSpan> times, Stopwatch stopwatch, long refSum)
    {

      var sum = 0L;
      stopwatch.Restart();

      foreach (long n in list)
      {
        sum += n;
      }

      stopwatch.Stop();
      times.Add(stopwatch.Elapsed);

      Assert.AreEqual(refSum, sum);
    }

    void TestFor(List<long> list, List<TimeSpan> times, Stopwatch stopwatch, long refSum)
    {

      var sum = 0L;
      stopwatch.Restart();

      for (var i = 0; i < list.Count; ++i)
      {
        sum += list[i];
      }

      stopwatch.Stop();
      times.Add(stopwatch.Elapsed);

      Assert.AreEqual(refSum, sum);
    }

    void TestFor_Array(long[] list, List<TimeSpan> times, Stopwatch stopwatch, long refSum)
    {

      var sum = 0L;
      stopwatch.Restart();

      for (var i = 0; i < list.Length; ++i)
      {
        sum += list[i];
      }

      stopwatch.Stop();
      times.Add(stopwatch.Elapsed);

      Assert.AreEqual(refSum, sum);
    }

  }
}
