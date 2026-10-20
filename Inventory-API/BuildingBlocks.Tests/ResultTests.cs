using BBE = BuildingBlocks.Error;

namespace BuildingBlocks.Tests;

public class ResultTests
{
    [Test]
    public void FailedResult_ValidErrorWithInvalidOperationException()
    {
        var testCode = "TestA";
        var testErrMessage = "TestB";
        
        var result = new BBE.Result<ImmutableRecord>(new BBE.ResultError(BBE.KindErr.BusinessRule, testCode, testErrMessage));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Kind, Is.EqualTo(BBE.KindErr.BusinessRule));
            Assert.That(result.Error.Code, Is.EqualTo(testCode));
            Assert.That(result.Error.ErrMessage, Is.EqualTo(testErrMessage));
            Assert.Throws<InvalidOperationException>(() => _ = result.Value);            
        });
    }

    [Test]
    public void Success_MatchingValueReturned()
    {
        var testA = "TestA";
        var testB = "TestB";
        var input = new MutableClass
        {
            TestA = testA,
            TestB = testB
        };

        var result = new BBE.Result<MutableClass>(input);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.SameAs(input));
            Assert.That(result.Value.TestA, Is.EqualTo(testA));
            Assert.That(result.Value.TestB, Is.EqualTo(testB));            
        });
    }

    [Test] 
    public void FailedResult_NullThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new BBE.Result<ImmutableRecord>((BBE.ResultError)null));
    }

    private class MutableClass
    {
        public string TestA { get; set; } = "";
        public string TestB { get; set; } = "";
    }

    private record ImmutableRecord(string TestA, string TestB);
}
