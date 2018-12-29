using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json.Linq;
using sql4js.Parser;
using sql4js.Executor;
using System;
using Xunit;
using System.Collections.Generic;

namespace sql4js.tests
{
    /// <summary>
    /// TODO: add test for classes
    /// </summary>
    public class ut_execution
    {
        [Fact]
        async public void executor_should_understand_dunamicl_fields_and_values()
        {
            var script1 = @"{ ""a"": 1, c#(""bb"") : c#( 999 )  }";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"{""a"":1,""bb"":999}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_dunamicl_fields_and_values_no_quotes()
        {
            var script1 = @"{ a: 1, c#(""bb"") : c#( 999 )  }";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"{a:1,""bb"":999}",
                result.ToJson());
        }

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

        [Fact]
        async public void executor_should_understand_additional_items_for_array()
        {
            var script1 = @"[ 1, c#(  var list = new List<Object>(); list.Add(2); list.Add(3); return list;  )   ]";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"[1,2,3]",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_items_for_array_version2()
        {
            var script1 = @"[ 1, c#(  var dict = new Dictionary<String, Object>(); dict[""b""] = 2; dict[""c""] = 3; return dict;  )   ]";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"[1,2]",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_items_for_array_version3()
        {
            var script1 = @"[ 1, c#(  
                var list = new List<Object>();
                {
                    var dict = new Dictionary<String, Object>(); 
                    dict[""b""] = 2; 
                    dict[""c""] = 3; 
                    list.Add(dict);
                }
                {
                    var dict = new Dictionary<String, Object>(); 
                    dict[""b""] = 22; 
                    dict[""c""] = 33; 
                    list.Add(dict);
                }
                return list;  )   ]";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"[1,2,22]",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_objects_for_array()
        {
            var script1 = @"[ 1, {c#(  
                var list = new List<Object>();
                {
                    var dict = new Dictionary<String, Object>(); 
                    dict[""b""] = 2; 
                    dict[""c""] = 3; 
                    list.Add(dict);
                }
                {
                    var dict = new Dictionary<String, Object>(); 
                    dict[""b""] = 22; 
                    dict[""c""] = 33; 
                    list.Add(dict);
                }
                return list;  )}   ]";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"[1,{""b"":2,""c"":3},{""b"":22,""c"":33}]",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_objects_with_fields_for_array()
        {
            var script1 = @"[ 1, {c#(  
                var list = new List<Object>();
                {
                    var dict = new Dictionary<String, Object>(); 
                    dict[""b""] = 2; 
                    dict[""c""] = 3; 
                    list.Add(dict);
                }
                {
                    var dict = new Dictionary<String, Object>(); 
                    dict[""b""] = 22; 
                    dict[""c""] = 33; 
                    list.Add(dict);
                }
                return list;  ),d:100}   ]";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"[1,{""b"":2,""c"":3,d:100},{""b"":22,""c"":33,d:100}]",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_objects_for_array_version2()
        {
            var script1 = @"[ 1, {c#(  
                    var dict = new Dictionary<String, Object>(); 
                    dict[""b""] = 2; 
                    dict[""c""] = 3;                    
                    return dict;  )}   ]";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"[1,{""b"":2,""c"":3}]",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_fields_for_object_version2()
        {
            var script1 = @"{ a: 1, b: c#(  var dict = new Dictionary<String, Object>(); dict[""bb""] = 22; dict[""cc""] = 33; return dict;  )   }";

            var result = await new S4JDefaultExecutor().
                Execute(script1);

            Assert.Equal(
                @"{a:1,b:22}",
                result.ToJson());
        }

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
