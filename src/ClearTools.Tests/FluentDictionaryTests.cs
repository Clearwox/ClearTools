using Clear;

namespace ClearTools.Tests;

public class FluentDictionaryTests
{
    [Fact]
    public void Add_SingleItem_AddsItemAndReturnsInstance()
    {
        var dictionary = new FluentDictionary<string, int>();

        var result = dictionary.Add("key1", 1);

        Assert.Same(dictionary, result);
        Assert.Single(dictionary);
        Assert.Equal(1, dictionary["key1"]);
    }

    [Fact]
    public void Add_MultipleItems_AddsAllItemsAndReturnsInstance()
    {
        var dictionary = new FluentDictionary<string, int>();

        var result = dictionary.Add("key1", 1).Add("key2", 2).Add("key3", 3);

        Assert.Same(dictionary, result);
        Assert.Equal(3, dictionary.Count);
        Assert.Equal(1, dictionary["key1"]);
        Assert.Equal(2, dictionary["key2"]);
        Assert.Equal(3, dictionary["key3"]);
    }

    [Fact]
    public void Add_DuplicateKey_ThrowsArgumentException()
    {
        var dictionary = new FluentDictionary<string, int>();
        dictionary.Add("key1", 1);

        Assert.Throws<ArgumentException>(() => dictionary.Add("key1", 2));
    }

    [Fact]
    public void Add_ChainedCalls_AllowsFluentSyntax()
    {
        var result = new FluentDictionary<string, string>()
            .Add("firstName", "John")
            .Add("lastName", "Doe")
            .Add("email", "john.doe@example.com");

        Assert.Equal(3, result.Count);
        Assert.Equal("John", result["firstName"]);
        Assert.Equal("Doe", result["lastName"]);
        Assert.Equal("john.doe@example.com", result["email"]);
    }

    [Fact]
    public void Remove_ExistingKey_RemovesItemAndReturnsInstance()
    {
        var dictionary = new FluentDictionary<string, int>()
            .Add("key1", 1)
            .Add("key2", 2);

        var result = dictionary.Remove("key1");

        Assert.Same(dictionary, result);
        Assert.Single(dictionary);
        Assert.False(dictionary.ContainsKey("key1"));
        Assert.True(dictionary.ContainsKey("key2"));
    }

    [Fact]
    public void Remove_NonExistingKey_ReturnsInstanceWithoutError()
    {
        var dictionary = new FluentDictionary<string, int>()
            .Add("key1", 1);

        var result = dictionary.Remove("key2");

        Assert.Same(dictionary, result);
        Assert.Single(dictionary);
        Assert.True(dictionary.ContainsKey("key1"));
    }

    [Fact]
    public void Remove_ChainedCalls_AllowsFluentSyntax()
    {
        var result = new FluentDictionary<string, int>()
            .Add("key1", 1)
            .Add("key2", 2)
            .Add("key3", 3)
            .Remove("key1")
            .Remove("key3");

        Assert.Single(result);
        Assert.Equal(2, result["key2"]);
    }

    [Fact]
    public void Remove_AllItems_ReturnsEmptyDictionary()
    {
        var result = new FluentDictionary<string, int>()
            .Add("key1", 1)
            .Add("key2", 2)
            .Remove("key1")
            .Remove("key2");

        Assert.Empty(result);
    }

    [Fact]
    public void Create_SingleItem_ReturnsNewDictionaryWithItem()
    {
        var dictionary = FluentDictionary<string, int>.Create("key1", 1);

        Assert.NotNull(dictionary);
        Assert.Single(dictionary);
        Assert.Equal(1, dictionary["key1"]);
    }

    [Fact]
    public void Create_ThenChainAdd_AllowsFluentSyntax()
    {
        var result = FluentDictionary<string, int>.Create("key1", 1)
            .Add("key2", 2)
            .Add("key3", 3);

        Assert.Equal(3, result.Count);
        Assert.Equal(1, result["key1"]);
        Assert.Equal(2, result["key2"]);
        Assert.Equal(3, result["key3"]);
    }

    [Fact]
    public void Create_ThenChainRemove_AllowsFluentSyntax()
    {
        var result = FluentDictionary<string, int>.Create("key1", 1)
            .Add("key2", 2)
            .Remove("key1");

        Assert.Single(result);
        Assert.Equal(2, result["key2"]);
    }

    [Fact]
    public void MixedOperations_AddAndRemove_WorksCorrectly()
    {
        var result = new FluentDictionary<string, string>()
            .Add("temp1", "value1")
            .Add("keep1", "value2")
            .Add("temp2", "value3")
            .Remove("temp1")
            .Add("keep2", "value4")
            .Remove("temp2");

        Assert.Equal(2, result.Count);
        Assert.True(result.ContainsKey("keep1"));
        Assert.True(result.ContainsKey("keep2"));
        Assert.False(result.ContainsKey("temp1"));
        Assert.False(result.ContainsKey("temp2"));
    }

    [Fact]
    public void FluentDictionary_WithComplexTypes_WorksCorrectly()
    {
        var person1 = new { Name = "John", Age = 30 };
        var person2 = new { Name = "Jane", Age = 25 };

        var result = new FluentDictionary<int, object>()
            .Add(1, person1)
            .Add(2, person2);

        Assert.Equal(2, result.Count);
        Assert.Equal(person1, result[1]);
        Assert.Equal(person2, result[2]);
    }

    [Fact]
    public void FluentDictionary_InheritsFromDictionary_HasAllBaseFunctionality()
    {
        var dictionary = new FluentDictionary<string, int>()
            .Add("key1", 1)
            .Add("key2", 2);

        // Test inherited functionality
        Assert.True(dictionary.ContainsKey("key1"));
        Assert.True(dictionary.ContainsValue(1));
        Assert.Equal(2, dictionary.Keys.Count);
        Assert.Equal(2, dictionary.Values.Count);
        
        // Test TryGetValue
        Assert.True(dictionary.TryGetValue("key1", out var value));
        Assert.Equal(1, value);
    }

    [Fact]
    public void Add_WithNullKey_ThrowsArgumentNullException()
    {
        var dictionary = new FluentDictionary<string, int>();

        Assert.Throws<ArgumentNullException>(() => dictionary.Add(null, 1));
    }

    [Fact]
    public void Create_WithNullKey_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            FluentDictionary<string, int>.Create(null, 1));
    }

    [Fact]
    public void FluentDictionary_WithValueTypes_WorksCorrectly()
    {
        var result = new FluentDictionary<int, double>()
            .Add(1, 1.5)
            .Add(2, 2.5)
            .Add(3, 3.5);

        Assert.Equal(3, result.Count);
        Assert.Equal(1.5, result[1]);
        Assert.Equal(2.5, result[2]);
        Assert.Equal(3.5, result[3]);
    }

    [Fact]
    public void FluentDictionary_EmptyDictionary_HasZeroCount()
    {
        var dictionary = new FluentDictionary<string, int>();

        Assert.Empty(dictionary);
        Assert.Equal(0, dictionary.Count);
    }

    [Fact]
    public void Create_WithDifferentTypes_WorksForEachType()
    {
        var stringDict = FluentDictionary<string, string>.Create("key", "value");
        var intDict = FluentDictionary<int, int>.Create(1, 100);
        var mixedDict = FluentDictionary<string, int>.Create("age", 30);

        Assert.Equal("value", stringDict["key"]);
        Assert.Equal(100, intDict[1]);
        Assert.Equal(30, mixedDict["age"]);
    }
}
