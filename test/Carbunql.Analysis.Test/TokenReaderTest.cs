//using SqModel.Analysis;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit.Abstractions;

//namespace SqModelAnalysisTest;

//public class TokenReaderTest
//{
//    private readonly ITestOutputHelper Output;

//    public TokenReaderTest(ITestOutputHelper output)
//    {
//        Output = output;
//    }

//    private void LogOutput(List<string> arguments)
//    {
//        foreach (var item in arguments)
//        {
//            Output.WriteLine(item);
//        }
//    }

//    [Fact]
//    public void JoinCommand()
//    {
//        var text = "inner join, left join, left outer join, right join, right outer join, cross join";
//        using var r = new TokenReader(text);
//        var lst = r.ReadTokens().ToList();
//        LogOutput(lst);

//        Assert.Equal(11, lst.Count);
//        Assert.Equal("inner join", lst[0]);
//        Assert.Equal(",", lst[1]);
//        Assert.Equal("left join", lst[2]);
//        Assert.Equal(",", lst[3]);
//        Assert.Equal("left outer join", lst[4]);
//        Assert.Equal(",", lst[5]);
//        Assert.Equal("right join", lst[6]);
//        Assert.Equal(",", lst[7]);
//        Assert.Equal("right outer join", lst[8]);
//        Assert.Equal(",", lst[9]);
//        Assert.Equal("cross join", lst[10]);
//    }

//    [Fact]
//    public void LeftRihgtFunction()
//    {
//        var text = "left('abc', 1), right('cde', 1)";
//        using var r = new TokenReader(text);
//        var lst = r.ReadTokens().ToList();
//        LogOutput(lst);

//        Assert.Equal(9, lst.Count);
//        Assert.Equal("left", lst[0]);
//        Assert.Equal("(", lst[1]);
//        Assert.Equal("'abc', 1", lst[2]);
//        Assert.Equal(")", lst[3]);
//        Assert.Equal(",", lst[4]);
//        Assert.Equal("right", lst[5]);
//        Assert.Equal("(", lst[6]);
//        Assert.Equal("'cde', 1", lst[7]);
//        Assert.Equal(")", lst[8]);
//    }

//    [Fact]
//    public void IsNot()
//    {
//        var text = "a is b, b is not c";
//        using var r = new TokenReader(text);
//        var lst = r.ReadTokens().ToList();
//        LogOutput(lst);

//        Assert.Equal(7, lst.Count);
//        Assert.Equal("a", lst[0]);
//        Assert.Equal("is", lst[1]);
//        Assert.Equal("b", lst[2]);
//        Assert.Equal(",", lst[3]);
//        Assert.Equal("b", lst[4]);
//        Assert.Equal("is not", lst[5]);
//        Assert.Equal("c", lst[6]);
//    }

//    [Fact]
//    public void LineComment()
//    {
//        var text = "a----/*b*/\r\nc--  d  \ne";
//        using var r = new TokenReader(text);
//        var lst = r.ReadTokens().ToList();
//        LogOutput(lst);

//        Assert.Equal(7, lst.Count);
//        Assert.Equal("a", lst[0]);
//        Assert.Equal("--", lst[1]);
//        Assert.Equal("--/*b*/", lst[2]);
//        Assert.Equal("c", lst[3]);
//        Assert.Equal("--", lst[4]);
//        Assert.Equal("  d  ", lst[5]);
//        Assert.Equal("e", lst[6]);
//    }

//    [Fact]
//    public void BlockComment()
//    {
//        var text = "a/*/*** b ***/*/c";
//        using var r = new TokenReader(text);
//        var lst = r.ReadTokens().ToList();
//        LogOutput(lst);

//        Assert.Equal(4, lst.Count);
//        Assert.Equal("a", lst[0]);
//        Assert.Equal("/*", lst[1]);
//        Assert.Equal("/*** b ***/*/", lst[2]);
//        Assert.Equal("c", lst[3]);
//    }

//    [Fact]
//    public void Bracket()
//    {
//        var text = "1+((2 + 3) * (4+5))-6";
//        using var r = new TokenReader(text);
//        var lst = r.ReadTokens().ToList();
//        LogOutput(lst);

//        Assert.Equal(7, lst.Count);
//        Assert.Equal("1", lst[0]);
//        Assert.Equal("+", lst[1]);
//        Assert.Equal("(", lst[2]);
//        Assert.Equal("(2 + 3) * (4+5)", lst[3]);
//        Assert.Equal(")", lst[4]);
//        Assert.Equal("-", lst[5]);
//        Assert.Equal("6", lst[6]);
//    }

//    [Fact]
//    public void CaseWhen()
//    {
//        var text = "case when a.v1 = 1 then 1 when a.v2 = 2 then 2 else 3 end";
//        using var r = new TokenReader(text);
//        var lst = r.ReadTokens().ToList();
//        LogOutput(lst);

//        Assert.Equal(3, lst.Count);
//        Assert.Equal("case", lst[0]);
//        Assert.Equal("when a.v1 = 1 then 1 when a.v2 = 2 then 2 else 3", lst[1]);
//        Assert.Equal("end", lst[2]);
//    }

//    [Fact]
//    public void Case()
//    {
//        var text = "case a.v1 when 1 then 10 when 2 then 20 else 30 end";
//        using var r = new TokenReader(text);
//        var lst = r.ReadTokens().ToList();
//        LogOutput(lst);

//        Assert.Equal(4, lst.Count);
//        Assert.Equal("case", lst[0]);
//        Assert.Equal("a.v1", lst[1]);
//        Assert.Equal("when 1 then 10 when 2 then 20 else 30", lst[2]);
//        Assert.Equal("end", lst[3]);
//    }
//}
