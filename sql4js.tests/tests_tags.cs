using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json.Linq;
using sql4js.Parser;
using sql4js.Executor;
using System;
using NUnit;
using NUnit.Framework;
using System.Collections.Generic;
using sql4js.Tokens;
using System.Threading.Tasks;

namespace sql4js.tests
{
    [TestFixture]
    public class tests_tags
    {
        [Test]
        async public Task test_simple_tags()
        {
            var script1 = @" 
#post #get
method ( a : int, b : string!, c: any ) 
'ok'
";

            var result = new S4JParserForTests().
                Parse(script1) as S4JTokenRoot;

            Assert.AreEqual(2, result.Tags.Count);

            if (!result.Tags.ContainsKey("post"))
                throw new Exception("Tag 'post' is missing");
            
            if (!result.Tags.ContainsKey("get"))
                throw new Exception("Tag 'get' is missing");
        }

        [Test]
        async public Task test_simple_value_tag()
        {
            var script1 = @" 
#post #get #permission:admin
method ( a : int, b : string!, c: any ) 
'ok'
";

            var result = new S4JParserForTests().
                Parse(script1) as S4JTokenRoot;

            Assert.AreEqual(3, result.Tags.Count);

            Assert.AreEqual("admin", result.Tags["permission"]);
        }
    }
}
