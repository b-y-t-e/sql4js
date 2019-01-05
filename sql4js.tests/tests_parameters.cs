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
    /// <summary>
    /// TODO:  
    /// integration with azure functions
    /// converstion from josn to net
    /// integration with self hosting environment
    /// support for http methods (get / post / put .. ) via tags
    /// support for saving data through api (c# helpers)
    /// support for add refenernces for executing project / custom namespaces / custom dll's
    /// add test for parameter types, 
    /// support for jsonsettings, 
    /// ? support for diffirent parameters parsing styles (dynamic / json / pure .net)
    /// </summary>
    public class tests_parameters
    {
        [Fact]
        async public void test_isrequired_parameter()
        {
            var script1 = @" method ( a : int, b : string!, c: any ) sql( select 1  ) ";

            await Assert.ThrowsAsync<S4JNullParameterException>(async () =>
          {
              var result = await new S4JExecutorForTests().
                  ExecuteWithParameters(script1);
          });
        }

        [Fact]
        async public void test_valid_int_parameter()
        {
            var script1 = @" method ( a : any, b : string!, c: int ) sql( select 1  ) ";

            await Assert.ThrowsAsync<S4JInvalidParameterTypeException>(async () =>
            {
                var result = await new S4JExecutorForTests().
                    ExecuteWithParameters(script1, 4.1, "", 4.1);
            });
        }

        [Fact]
        async public void test_valid_array_parameter()
        {
            var script1 = @" method ( a : array ) sql( select 1  ) ";

            await Assert.ThrowsAsync<S4JInvalidParameterTypeException>(async () =>
            {
                var result = await new S4JExecutorForTests().
                    ExecuteWithParameters(script1,"{a:1}");
            });
        }

        [Fact]
        async public void test_valid_object_parameter()
        {
            var script1 = @" method ( a : object ) sql( select 1  ) ";

            await Assert.ThrowsAsync<S4JInvalidParameterTypeException>(async () =>
            {
                var result = await new S4JExecutorForTests().
                    ExecuteWithJsonParameters(script1, "[1,2,3,4,5]");
            });
        }

        [Fact]
        async public void test_isrequired_parameter_json()
        {
            var script1 = @" method ( a : int, b : string!, c: any ) sql( select 1  ) ";

            await Assert.ThrowsAsync<S4JNullParameterException>(async () =>
            {
                var result = await new S4JExecutorForTests().
                    ExecuteWithJsonParameters(script1);
            });
        }

        [Fact]
        async public void test_array_parameter_json()
        {
            var script1 = @" method ( c: array ) c#( Globals.c.Count  ) ";

            var result = await new S4JExecutorForTests().
                ExecuteWithJsonParameters(script1, "[1,2,3,4]");

            Assert.Equal("4", result.ToJson());
        }

        [Fact]
        async public void test_object_parameter_json()
        {
            var script1 = @" method ( c: object ) c#( Globals.c.g  ) ";

            var result = await new S4JExecutorForTests().
                ExecuteWithJsonParameters(script1, "{g:123}");

            Assert.Equal("123", result.ToJson());
        }

        [Fact]
        async public void test_int_parameter_json()
        {
            var script1 = @" method ( a : any, b : string!, c: int ) sql( select @c  ) ";

            var result = await new S4JExecutorForTests().
                ExecuteWithJsonParameters(script1, "4.1", "''", "4");

            Assert.Equal("4", result.ToJson());
        }

        [Fact]
        async public void test_int_complex_parameter_json()
        {
            var script1 = @" method ( a : any, b : string!, c: int ) sql( select @c + @a_f2  ) ";

            var result = await new S4JExecutorForTests().
                ExecuteWithJsonParameters(script1, "{ f1: 1, f2 : 2, f3: 'c' }", "''", "4");

            Assert.Equal("6", result.ToJson());
        }

    }
}
