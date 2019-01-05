using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json.Linq;
using sql4js.Parser;
using sql4js.Executor;
using System;
using Xunit;
using System.Collections.Generic;

namespace sql4js.tests
{
    public class tests_parameters_save
    {
        [Fact]
        async public void test_int_complex_parameter_json()
        {
            await new DbForTest().PrepareDb();

            var script1 = @" 

method ( osoba : any ) 
sql( insert into osoba(imie) select @osoba_imie; ),
sql( select imie from osoba where imie = 'test' )
";

            var result = await new S4JExecutorForTests().
                ExecuteWithJsonParameters(script1, "{ imie: 'test' }");

            Assert.Equal("\"test\"", result.ToJson());
        }

    }
}
