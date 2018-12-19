using Newtonsoft.Json.Linq;
using sql4js.Parser;
using System;
using Xunit;

namespace sql4js.tests
{
    public class UnitTest1
    {
        [Fact]
        public void S4J_parser_test_method_is()
        {
            /*string json = @"
{
  CPU: @Intel,
  CPU2: ""Intel"",
  Drives: [
    'DVD read/writer',
    '500 gigabyte hard drive'
  ]
}";

            JObject o = JObject.Parse(json, new JsonLoadSettings() { });
            o = o;*/

            /*string json = @"
{
  CPU: 'Intel',
  CPU2: ""Intel"",
CPU3: {""test""},
  Drives: [
    'DVD read/writer',
    '500 gigabyte hard drive'
  ]
}";

            JObject o = JObject.Parse(json, new JsonLoadSettings() {  });
            o = o;*/

            var chars = @"{a:'cos'}".ToCharArray();

            Assert.
                False(
                    S4JParser.Is(chars, 0, "'".ToCharArray()));

            Assert.
                True(
                    S4JParser.Is(chars, 3, "'".ToCharArray()));

            Assert.
                True(
                    S4JParser.Is(chars, 3, ":'".ToCharArray()));

            Assert.
                True(
                    S4JParser.Is(chars, 0, "{".ToCharArray()));

            Assert.
                True(
                    S4JParser.Is(chars, 1, "{a".ToCharArray()));

            Assert.
                False(
                    S4JParser.Is(chars, 1, "{{a".ToCharArray()));

            Assert.
                False(
                    S4JParser.Is(new char[0], 1, "{".ToCharArray()));
        }

        /*[Fact]
        public void S4J_parser_no_j4s()
        {
            var script1 = @"{a:'cos'}";
            var script2 = @"{a:'cos'}";

            var result = S4JParser.Parse(script1);

            Assert.
                Equal(script2, result.SimplifiedScriptAsText);
        }*/

        [Fact]
        public void S4J_parser_simple_j4s_object_1()
        {
            var script1 = @"{    a : 'cos', b : ""select 1 "", c: 'aaa' }";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                @"{a:'cos',b:""select 1 "",c:'aaa'}",
                result.ToJson());
        }

        [Fact]
        public void S4J_parser_simple_j4s_object_2()
        {
            var script1 = @"{a : 'cos', "" select 1 as val "", c: 'aaa' }";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                @"{a:'cos',"" select 1 as val "",c:'aaa'}",
                result.ToJson());
        }

        [Fact]
        public void S4J_parser_simple_j4s_array_1()
        {
            var script1 = @"[1 , 2 , 3 , 'abc' ]";
            
            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                @"[1,2,3,'abc']",
                result.ToJson());
        }

        [Fact]
        public void S4J_parser_simple_j4s_simple_expression()
        {
            var script1 = @" 'ab c ' ";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                "'ab c '",
                result.ToJson());
        }


        /*[Fact]
        public void S4J_parser_simple_j4s_inner_quotation1()
        {
            var script1 = @"{a:'cos', b: ""select 'abc' 1"", c: 'aaa' }";
            var script2 = @"{a:'cos', b: @EXP@, c: 'aaa' }";

            var result = S4JParser.Parse(script1);

            Assert.
                Equal(script2, result.SimplifiedScriptAsText);
        }

        [Fact]
        public void S4J_parser_simple_j4s_inner_quotation2()
        {
            var script1 = @"{a:'cos', b: ""select \\""aaa\\"" 1"", c: 'aaa' }";
            var script2 = @"{a:'cos', b: @EXP@, c: 'aaa' }";

            var result = S4JParser.Parse(script1);

            Assert.
                Equal(script2, result.SimplifiedScriptAsText);
        }*/
    }
}
