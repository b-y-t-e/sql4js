using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json.Linq;
using sql4js.Parser;
using sql4js.Executor;
using System;
using Xunit;
using System.Collections.Generic;
using sql4js.Tokens;

namespace sql4js.tests
{
    public class tests_tags
    {
        [Fact]
        async public void test_simple_tags()
        {
            var script1 = @" 
#post #get
method ( a : int, b : string!, c: any ) 
'ok'
";

            var result = new S4JParserForTests().
                Parse(script1) as S4JTokenRoot;

            Assert.Equal(2, result.Tags.Count);

            if (!result.Tags.ContainsKey("post"))
                throw new Exception("Tag 'post' is missing");
            
            if (!result.Tags.ContainsKey("get"))
                throw new Exception("Tag 'get' is missing");
        }

        [Fact]
        async public void test_simple_value_tag()
        {
            var script1 = @" 
#post #get #permission:admin
method ( a : int, b : string!, c: any ) 
'ok'
";

            var result = new S4JParserForTests().
                Parse(script1) as S4JTokenRoot;

            Assert.Equal(3, result.Tags.Count);

            Assert.Equal("admin", result.Tags["permission"]);
        }
    }
}
