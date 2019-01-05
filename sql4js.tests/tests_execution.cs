using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json.Linq;
using sql4js.Parser;
using sql4js.Executor;
using System;
using Xunit;
using System.Collections.Generic;

namespace sql4js.tests
{
    public class tests_execution
    {
        [Fact]
        async public void executor_should_understand_dunamicl_fields_and_values()
        {
            var script1 = @"{ ""a"": 1, c#(""bb"") : c#( 999 )  }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"{""a"":1,""bb"":999}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_empty_arguments()
        {
            var script1 = @"  method1 (param1) { ""a"": c#(Globals.param1) }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"{""a"":null}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_one_argument()
        {
            var script1 = @"  method1 (param1) { ""a"": c#(Globals.param1) }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1, 999);

            var txt = result.ToJson();

            Assert.Equal(
                @"{""a"":999}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_many_arguments()
        {
            var script1 = @"  method1 (param1, param2, param3, param4) { ""a"": c#(Globals.param1+Globals.param2+Globals.param3+Globals.param4) }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1, 1, 10, 100, 1000.0);

            var txt = result.ToJson();

            Assert.Equal(
                @"{""a"":1111.0}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_null_value_forKey()
        {
            var a = 1 + null;

            var script1 = @"   { ""a"": null, ""b"" : c#(1+(int?)Globals.a)  }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"{""a"":null,""b"":null}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_dunamicl_fields_and_values_no_quotes()
        {
            var script1 = @"{ a: 1, c#(""bb"") : c#( 999 )  }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"{a:1,""bb"":999}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_class_fields_for_object()
        {
            var script1 = @"{ a: 1, c#( 
    class osoba { public string imie; public string nazwisko; } 
    osoba o = new osoba(); 
    o.imie = ""adam""; 
    o.nazwisko = ""adsafasg""; 
    return o; )  }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"{a:1,""imie"":""adam"",""nazwisko"":""adsafasg""}",
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

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            Assert.Equal(
                @"{b:""abc1""}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_simple_function_with_outer_comments_2()
        {
            var script1 = @"{ b : c#( ""abc"" + 1 )   }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            Assert.Equal(
                @"{b:""abc1""}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_parent_values()
        {
            var script1 = @"{ a: 1, b : c#( Globals.a + 1 )   }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            Assert.Equal(
                @"{a:1,b:2}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_parent_values_version2()
        {
            var script1 = @"{ a: 1, b : c#( Globals.a + 1 ), c : c#( Globals.a + Globals.b )   }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            Assert.Equal(
                @"{a:1,b:2,c:3}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_parent_values_version3()
        {
            var script1 = @"{ a: 1, b : c#( Globals.a + 1 ), c : c#( Globals.a + Globals.b ), d: {a:10, b:c#(Globals.a+Globals.c)}   }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            Assert.Equal(
                @"{a:1,b:2,c:3,d:{a:10,b:13}}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_fields_for_object()
        {
            var script1 = @"{ a: 1, c#(  var dict = new Dictionary<String, Object>(); dict[""b""] = 2; dict[""c""] = 3; return dict;  )   }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            Assert.Equal(
                @"{a:1,""b"":2,""c"":3}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_null_fields_for_object()
        {
            var script1 = @"{ a: 1, c#(  null  ), d: 3   }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            Assert.Equal(
                @"{a:1,d:3}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_items_for_array()
        {
            var script1 = @"[ 1, c#(  var list = new List<Object>(); list.Add(2); list.Add(3); return list;  )   ]";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            Assert.Equal(
                @"[1,2,3]",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_empty_items_for_array()
        {
            var script1 = @"[ 1, c#(  var list = new List<Object>(); return list;  )   ]";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"[1]",
                result.ToJson());
        }
        
        [Fact]
        async public void executor_should_understand_additional_null_items_for_array()
        {
            var script1 = @"[ 1, c#(  return null;  )   ]";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"[1]",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_items_for_array_version2()
        {
            var script1 = @"[ 1, c#(  var dict = new Dictionary<String, Object>(); dict[""b""] = 2; dict[""c""] = 3; return dict;  )   ]";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

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

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

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

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

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

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

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

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            Assert.Equal(
                @"[1,{""b"":2,""c"":3}]",
                result.ToJson());
        }
        
        [Fact]
        async public void executor_should_understand_additional_null_objects_for_array()
        {
            var script1 = @"[ 1, {c#(  
                    null  )}   ]";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            Assert.Equal(
                @"[1]",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_fields_for_object_version2()
        {
            var script1 = @"{ a: 1, b: c#(  var dict = new Dictionary<String, Object>(); dict[""bb""] = 22; dict[""cc""] = 33; return dict;  )   }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            Assert.Equal(
                @"{a:1,b:22}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_additional_null_field_for_object()
        {
            var script1 = @"{ a: 1, b: c#(  null  )   }";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"{a:1,b:null}",
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

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            Assert.Equal(
                @"{b:3}",
                result.ToJson());
        }
    }
}
