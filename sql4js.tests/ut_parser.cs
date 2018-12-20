using Newtonsoft.Json.Linq;
using sql4js.Parser;
using System;
using Xunit;

namespace sql4js.tests
{
    public class ut_parser
    {
        [Fact]
        public void parser_method_is_should_work_fine()
        {
            var chars = @"{a:'cos'}".ToCharArray();

            Assert.
                False(
                    S4JParserHelper.Is(chars, 0, "'".ToCharArray()));

            Assert.
                True(
                    S4JParserHelper.Is(chars, 3, "'".ToCharArray()));

            Assert.
                True(
                    S4JParserHelper.Is(chars, 2, ":'".ToCharArray()));

            Assert.
                True(
                    S4JParserHelper.Is(chars, 0, "{".ToCharArray()));

            Assert.
                True(
                    S4JParserHelper.Is(chars, 0, "{a".ToCharArray()));

            Assert.
                True(
                    S4JParserHelper.Is(chars, 1, "a:'".ToCharArray()));

            Assert.
                False(
                    S4JParserHelper.Is(chars, 0, "{{a".ToCharArray()));

            Assert.
                False(
                    S4JParserHelper.Is(new char[0], 1, "{".ToCharArray()));

            Assert.
                True(
                    S4JParserHelper.Is(chars, 8, "}".ToCharArray()));

            Assert.
                False(
                    S4JParserHelper.Is(chars, 8, "}}}".ToCharArray()));
        }
        
        [Fact]
        public void parser_should_understand_simple_json_object_test1()
        {
            var script1 = @"{    a : 'cos', b : {{select 1 }}, c: 'aaa' }";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                @"{a:'cos',b:{{select 1 }},c:'aaa'}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_simple_json_object_test2()
        {
            var script1 = @"{a : 'cos', {{ select 1 as val }}, c: 'aaa' }";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                @"{a:'cos',{{ select 1 as val }},c:'aaa'}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_simple_json_array_test1()
        {
            var script1 = @"[1 , 2 , 3 , 'abc' ]";
            
            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                @"[1,2,3,'abc']",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_simple_string_value1()
        {
            var script1 = @" 'ab c ' ";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                "'ab c '",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_simple_double_value1()
        {
            var script1 = @" 4324234.66 ";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                "4324234.66",
                result.ToJson());
        }

        [Fact]
        public void parser_should_ignore_comment()
        {
            var script1 = @" 4324234.66 /* abc */ ";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                "4324234.66",
                result.ToJson());
        }

        [Fact]
        public void parser_should_ignore_comment_in_comment()
        {
            var script1 = @" 4324234.66 /* abc /* abc */ abc */ ";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                "4324234.66",
                result.ToJson());
        }

        [Fact]
        public void parser_should_ignore_comment_inside_table()
        {
            var script1 = @"[1 , 2 , /* /* abc */ */ 3 , 'abc' ]";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                @"[1,2,3,'abc']",
                result.ToJson());
        }

        [Fact]
        public void parser_should_ignore_comment_inside_object()
        {
            var script1 = @"{a : 'cos', /* abc*/  {{ select 1 as val }}, c: 'aaa' }";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                @"{a:'cos',{{ select 1 as val }},c:'aaa'}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_inner_object()
        {
            var script1 = @"{a : 'cos', d : { a : 1, b : 2, c : 'abc'}, c: 'aaa' }";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                @"{a:'cos',d:{a:1,b:2,c:'abc'},c:'aaa'}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_inner_object_and_arrays1()
        {
            var script1 = @"{  d: [ {@f : 6} ] , c: 'aaa' }";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                @"{d:[{@f:6}],c:'aaa'}",
                result.ToJson());
        }


        [Fact]
        public void parser_should_understand_inner_object_and_arrays2()
        {
            var script1 = @"
{
    a : 'cos', 
    d : { 
        a : 1, 
        b : 2, 
        c : 'abc', 
        d: [1,2,3, {g: 8, @f : 6} ]  
    }, 
    c: 'aaa' 
}
";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                @"{a:'cos',d:{a:1,b:2,c:'abc',d:[1,2,3,{g:8,@f:6}]},c:'aaa'}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_inner_object_and_arrays3()
        {
            var script1 = @"{d:[{@f:6}],c:'aaa'}";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                @"{d:[{@f:6}],c:'aaa'}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_inner_object_and_arrays4()
        {
            var script1 = @"{d:[{@f:6}],c:'aaa',d:[{a:['gg']}],e:'b'}";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                script1,
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_inner_object_and_arrays5()
        {
            var script1 = @"[{d:[{a:['gg']}]},9,1]";

            var result = S4JParser.
                Parse(script1);

            Assert.Equal(
                script1,
                result.ToJson());
        }
    }
}
