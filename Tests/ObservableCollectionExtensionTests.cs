using System.Collections.ObjectModel;
using Orditor.ViewModels;

namespace Tests
{
  public class ObservableCollectionExtensionTests
  {
    [Theory]
    [TestCase(0, 0)]
    [TestCase(0, 1)]
    [TestCase(1, 0)]
    [TestCase(2, 2)]
    public void Trim(int count, int index)
    {
      var expected = Math.Min(count, index);
      var collection = new ObservableCollection<string>(Enumerable.Repeat("", count));
      
      collection.Trim(index);

      Assert.That(collection.Count, Is.EqualTo(expected));
    }

    [Theory]
    [TestCase("", "")]
    [TestCase("", "a")]
    [TestCase("a", "")]
    [TestCase("a", "a")]
    [TestCase("ab", "abc")]
    [TestCase("ac", "abc")]
    [TestCase("bc", "abc")]
    [TestCase("abc", "ac")]
    [TestCase("abc", "ab")]
    [TestCase("abc", "bc")]
    [TestCase("abc", "abc")]
    [TestCase("abc", "bca")]
    [TestCase("abc", "cab")]
    public void Update(string initialContent, string newContent)
    {
      var expected = newContent.Select(c => c.ToString()).ToList();
      var collection = new ObservableCollection<string>(initialContent.Select(c => c.ToString()));

      collection.Update(expected);

      CollectionAssert.AreEqual(expected, collection);
    }
  }
}