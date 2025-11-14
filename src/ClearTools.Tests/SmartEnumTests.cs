using Clear;

namespace ClearTools.Tests;

public class SmartEnumTests
{
    // Test implementation of SmartEnum
    private class TestStatus : SmartEnum<TestStatus>
    {
        public static readonly TestStatus Active = new TestStatus("Active", 1);
        public static readonly TestStatus Inactive = new TestStatus("Inactive", 2);
        public static readonly TestStatus Pending = new TestStatus("Pending", 3);

        private TestStatus(string name, int value) : base(name, value) { }
    }

    // Another test implementation for isolation
    private class TestPriority : SmartEnum<TestPriority>
    {
        public static readonly TestPriority Low = new TestPriority("Low", 10);
        public static readonly TestPriority Medium = new TestPriority("Medium", 20);
        public static readonly TestPriority High = new TestPriority("High", 30);

        private TestPriority(string name, int value) : base(name, value) { }
    }

    [Fact]
    public void Constructor_SetsNameAndValue()
    {
        var status = TestStatus.Active;

        Assert.Equal("Active", status.Name);
        Assert.Equal(1, status.Value);
    }

    [Fact]
    public void ParseByValue_ValidValue_ReturnsCorrectInstance()
    {
        var result = TestStatus.Parse(1);

        Assert.NotNull(result);
        Assert.Equal("Active", result.Name);
        Assert.Equal(1, result.Value);
        Assert.Same(TestStatus.Active, result);
    }

    [Fact]
    public void ParseByValue_InvalidValue_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => TestStatus.Parse(999));
        Assert.Contains("No SmartEnum with value 999", exception.Message);
    }

    [Fact]
    public void ParseByName_ValidName_ReturnsCorrectInstance()
    {
        var result = TestStatus.Parse("Active");

        Assert.NotNull(result);
        Assert.Equal("Active", result.Name);
        Assert.Equal(1, result.Value);
        Assert.Same(TestStatus.Active, result);
    }

    [Fact]
    public void ParseByName_CaseInsensitive_ReturnsCorrectInstance()
    {
        var resultLower = TestStatus.Parse("active");
        var resultUpper = TestStatus.Parse("ACTIVE");
        var resultMixed = TestStatus.Parse("AcTiVe");

        Assert.Same(TestStatus.Active, resultLower);
        Assert.Same(TestStatus.Active, resultUpper);
        Assert.Same(TestStatus.Active, resultMixed);
    }

    [Fact]
    public void ParseByName_InvalidName_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => TestStatus.Parse("NonExistent"));
        Assert.Contains("No SmartEnum with name 'NonExistent'", exception.Message);
    }

    [Fact]
    public void ParseByName_EmptyString_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TestStatus.Parse(""));
    }

    [Fact]
    public void List_ReturnsAllInstances()
    {
        var result = TestStatus.List().ToList();

        Assert.Equal(3, result.Count);
        Assert.Contains(TestStatus.Active, result);
        Assert.Contains(TestStatus.Inactive, result);
        Assert.Contains(TestStatus.Pending, result);
    }

    [Fact]
    public void List_DifferentEnumTypes_ReturnsSeparateLists()
    {
        var statusList = TestStatus.List().ToList();
        var priorityList = TestPriority.List().ToList();

        Assert.Equal(3, statusList.Count);
        Assert.Equal(3, priorityList.Count);
        Assert.Contains(TestStatus.Active, statusList);
        Assert.Contains(TestPriority.Low, priorityList);
    }

    [Fact]
    public void ToString_ReturnsName()
    {
        var result = TestStatus.Active.ToString();

        Assert.Equal("Active", result);
    }

    [Fact]
    public void Equals_SameInstance_ReturnsTrue()
    {
        var status1 = TestStatus.Active;
        var status2 = TestStatus.Active;

        Assert.True(status1.Equals(status2));
        Assert.True(status1 == status2);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var status1 = TestStatus.Parse(1);
        var status2 = TestStatus.Parse("Active");

        Assert.True(status1.Equals(status2));
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        var status1 = TestStatus.Active;
        var status2 = TestStatus.Inactive;

        Assert.False(status1.Equals(status2));
    }

    [Fact]
    public void Equals_NullObject_ReturnsFalse()
    {
        var status = TestStatus.Active;

        Assert.False(status.Equals(null));
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var status = TestStatus.Active;
        var other = "Active";

        Assert.False(status.Equals(other));
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHashCode()
    {
        var status1 = TestStatus.Active;
        var status2 = TestStatus.Parse(1);

        var hash1 = status1.GetHashCode();
        var hash2 = status2.GetHashCode();

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_DifferentValue_ReturnsDifferentHashCode()
    {
        var status1 = TestStatus.Active;
        var status2 = TestStatus.Inactive;

        var hash1 = status1.GetHashCode();
        var hash2 = status2.GetHashCode();

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void ParseByValue_MultipleEnumTypes_DoesNotInterfere()
    {
        var status = TestStatus.Parse(1);
        var priority = TestPriority.Parse(10);

        Assert.Equal("Active", status.Name);
        Assert.Equal("Low", priority.Name);
        Assert.NotSame(status, priority);
    }

    [Fact]
    public void List_CalledMultipleTimes_ReturnsConsistentResults()
    {
        var list1 = TestStatus.List().ToList();
        var list2 = TestStatus.List().ToList();

        Assert.Equal(list1.Count, list2.Count);
        for (int i = 0; i < list1.Count; i++)
        {
            Assert.Same(list1[i], list2[i]);
        }
    }

    [Fact]
    public void ParseByValue_AllValues_ReturnsCorrectInstances()
    {
        Assert.Same(TestStatus.Active, TestStatus.Parse(1));
        Assert.Same(TestStatus.Inactive, TestStatus.Parse(2));
        Assert.Same(TestStatus.Pending, TestStatus.Parse(3));
    }

    [Fact]
    public void ParseByName_AllNames_ReturnsCorrectInstances()
    {
        Assert.Same(TestStatus.Active, TestStatus.Parse("Active"));
        Assert.Same(TestStatus.Inactive, TestStatus.Parse("Inactive"));
        Assert.Same(TestStatus.Pending, TestStatus.Parse("Pending"));
    }
}
