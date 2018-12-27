using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json.Linq;
using sql4js.Parser;
using sql4js.Executor;
using System;
using Xunit;

namespace sql4js.tests
{
    public class ut_execution
    {
        [Fact]
        public async void parser_method_is_should_work_fine()
        {
            for (var i = 0; i < 10; i++)
            {
                object result = await CSharpScript.EvaluateAsync(@"
int a = " + i + @";

public class osoba{
public int wiek;
public osoba(){
wiek = 2;
}
};

return a + new osoba().wiek;");
                Assert.Equal(2 + i, result);

            }
        }

        [Fact]
        async public void parser_should_understand_simple_function_with_outer_comments()
        {
            var script1 = @"{ b : c( ""abc"" + 1 )   }";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            // result

            Assert.Equal(
                @"{b:""abc1""}",
                result.ToJson());
        }
    }
}
