using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json.Linq;
using sql4js.Parser;
using sql4js.Executor;
using System;
using Xunit;
using System.Collections.Generic;

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
        async public void executor_should_understand_simple_function_with_outer_comments()
        {
            var script1 = @"{ b : c#( ""abc"" + 1 )   }";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"{b:""abc1""}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_simple_function_with_outer_comments_2()
        {
            var script1 = @"{ b : c#( ""abc"" + 1 )   }";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"{b:""abc1""}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_parent_values()
        {
            var script1 = @"{ a: 1, b : c#( a + 1 )   }";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"{a:1,b:2}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_parent_values_version2()
        {
            var script1 = @"{ a: 1, b : c#( a + 1 ), c : c#( a + b )   }";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"{a:1,b:2,c:3}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_parent_values_version3()
        {
            var script1 = @"{ a: 1, b : c#( a + 1 ), c : c#( a + b ), d: {a:10, b:c#(a+c)}   }";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"{a:1,b:2,c:3,d:{a:10,b:13}}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_fields_for_object()
        {
            var script1 = @"{ a: 1, c#(  var dict = new Dictionary<String, Object>(); dict[""b""] = 2; dict[""c""] = 3; return dict;  )   }";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"{a:1,""b"":2,""c"":3}",
                result.ToJson());
        }

        /*[Fact]
        async public void executor_should_understand_additional_fields_for_object()
        {
            var script1 = @"{ a: 1, b: c#(  var dict = new Dictionary<String, Object>(); dict[""b""] = 22; dict[""c""] = 33; return dict;  )   }";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"{a:1,b:2}",
                result.ToJson());
        }*/

        [Fact]
        async public void executor_simple_csharp_function()
        {
            var script1 = @"{ b : c#( 

int abc(){
return 3;
}

return abc();

)   }";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"{b:3}",
                result.ToJson());
        }
    }
}
