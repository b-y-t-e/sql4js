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
    /// TODO:  add test for sqlite, add test for configuration, add test for parameter types
    /// </summary>
    public class tests_execution_sql
    {
        [Fact]
        async public void executor_should_understand_simple_sql()
        {
            var script1 = @" sql( select 1  ) ";

            var result = await new S4JExecutorForTests().
                Execute(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"1",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_parameters_in_sql()
        {
            var script1 = @" method(param1) sql( select @param1 + 1  ) ";

            var result = await new S4JExecutorForTests().
                Execute(script1, 199);

            var txt = result.ToJson();

            Assert.Equal(
                @"200",
                result.ToJson());
        }
    }
}
