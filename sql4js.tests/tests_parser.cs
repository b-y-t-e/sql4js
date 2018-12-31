using Newtonsoft.Json.Linq;
using sql4js.Parser;
using System;
using Xunit;

namespace sql4js.tests
{
    public class tests_parser
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
        public void parser_should_understand_root_name()
        {
            var script1 = @" a( p ) { 1 } ";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                "a(p){1}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_root_name_and_few_parameters()
        {
            var script1 = @" a( p, c : int, b : string ) { 1 } ";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                "a(p,c:int,b:string){1}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_invalid_object()
        {
            var script1 = @" { ""a"" } ";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{""a""}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_invalid_object_version2()
        {
            var script1 = @" { ""a"" , ""b""  } ";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{""a"",""b""}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_invalid_object_version3()
        {
            var script1 = @" { 11 } ";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{11}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_invalid_object_version4()
        {
            var script1 = @" { 22 , 33  } ";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{22,33}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_ignore_comment()
        {
            var script1 = @" 3 /* abc */ ";

            var result = new S4JParserForTests().
                Parse(script1);
            
            Assert.Equal(
                "3",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_simple_object()
        {
            var script1 = @" {  a: 1, b: 2, c: 3 } ";

            var result = new S4JParserForTests().
                Parse(script1);

            var txt = result.ToJson();

            Assert.Equal(
                "{a:1,b:2,c:3}",
                result.ToJson());
        }


        [Fact]
        public void parser_should_understand_simple_function()
        {
            var script1 = @"{ b : sql( select 1 )   }";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{b:sql(select 1)}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_simple_sql_function_wth_getdate()
        {
            var script1 = @"{ b : sql( select getdate())   }";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{b:sql(select getdate())}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_quotation_with_sql_function()
        {
            var script1 = @"{ b : "" sql( select getdate()   )  "" }";

            var result = new S4JParserForTests().
                Parse(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"{b:"" sql( select getdate()   )  ""}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_function_with_quotation1()
        {
            var script1 = @"{ b : sql(select abc('def'))   }";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{b:sql(select abc('def'))}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_function_with_quotation1_inside_quotation1()
        {
            var script1 = @"{ b : sql(select abc('d\'ef'))   }";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{b:sql(select abc('d\'ef'))}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_function_with_quotation2_inside_quotation2()
        {
            var script1 = @"{ b : sql(select abc(""d\""ef""))   }";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{b:sql(select abc(""d\""ef""))}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_function_with_quotation2_inside_quotation2_version2()
        {
            var script1 = @"{ b : sql(select abc(""d\""ff\""ef""))   }";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{b:sql(select abc(""d\""ff\""ef""))}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_function_with_quotation1_inside_quotation2()
        {
            var script1 = @"{ b : sql(select abc(""d'ff'ef""))   }";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{b:sql(select abc(""d'ff'ef""))}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_simple_function_with_comments()
        {
            var script1 = @"{ b : sql( select 1 /* abc */ )   }";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{b:sql(select 1)}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_simple_function_with_outer_comments()
        {
            var script1 = @"{ b : /*sql( select 1 /* abc */ )*/   }";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{b:}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_simple_json_object_test1()
        {
            var script1 = @"{    a : 'cos', b : sql(select 1 ), c: 'aaa' }";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{a:'cos',b:sql(select 1),c:'aaa'}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_simple_json_object_test2()
        {
            var script1 = @"{a : 'cos', sql( select 1 as val ), c: 'aaa' }";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{a:'cos',sql(select 1 as val),c:'aaa'}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_simple_json_array_test1()
        {
            var script1 = @"[1 , 2 , 3 , 'abc' ]";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"[1,2,3,'abc']",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_simple_string_value1()
        {
            var script1 = @" 'ab c ' ";

            var result = new S4JParserForTests().
                Parse(script1);

            var txt = result.ToJson();

            Assert.Equal(
                "'ab c '",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_simple_double_value1()
        {
            var script1 = @" 4324234.66 ";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                "4324234.66",
                result.ToJson());
        }


        [Fact]
        public void parser_should_ignore_comment_in_comment()
        {
            var script1 = @" 4324234.66 /* abc /* abc */ abc */ ";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                "4324234.66",
                result.ToJson());
        }

        [Fact]
        public void parser_should_ignore_comment_inside_table()
        {
            var script1 = @"[1 , 2 , /* /* abc */ */ 3 , 'abc' ]";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"[1,2,3,'abc']",
                result.ToJson());
        }

        [Fact]
        public void parser_should_ignore_comment_inside_object()
        {
            var script1 = @"{a : 'cos', /* abc*/  sql( select 1 as val ), c: 'aaa' }";

            var result = new S4JParserForTests().
                Parse(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"{a:'cos',sql(select 1 as val),c:'aaa'}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_inner_object()
        {
            var script1 = @"{a : 'cos', d : { a : 1, b : 2, c : 'abc'}, c: 'aaa' }";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{a:'cos',d:{a:1,b:2,c:'abc'},c:'aaa'}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_inner_object_and_arrays1()
        {
            var script1 = @"{  d: [ {@f : 6} ] , c: 'aaa' }";

            var result = new S4JParserForTests().
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

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{a:'cos',d:{a:1,b:2,c:'abc',d:[1,2,3,{g:8,@f:6}]},c:'aaa'}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_inner_object_and_arrays3()
        {
            var script1 = @"{d:[{@f:6}],c:'aaa'}";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                @"{d:[{@f:6}],c:'aaa'}",
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_inner_object_and_arrays4()
        {
            var script1 = @"{d:[{@f:6}],c:'aaa',d:[{a:['gg']}],e:'b'}";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                script1,
                result.ToJson());
        }

        [Fact]
        public void parser_should_understand_inner_object_and_arrays5()
        {
            var script1 = @"[{d:[{a:['gg']}]},9,1]";

            var result = new S4JParserForTests().
                Parse(script1);

            Assert.Equal(
                script1,
                result.ToJson());
        }
    }
}
