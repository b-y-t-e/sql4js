using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json.Linq;
using sql4js.Parser;
using sql4js.Executor;
using System;
using NUnit;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sql4js.tests
{
    [TestFixture]
    public class tests_parameters_save
    {
        [Test, Order(-1)]
        async public Task prepare_db()
        {
            await new DbForTest().PrepareDb();
        }

        [Test]
        async public Task test_complex_parameter_save_sql()
        {
            // await new DbForTest().PrepareDb();

            var script1 = @" 

method ( osoba : any ) 
sql( insert into osoba(imie) select @osoba_imie; ),
sql( select imie from osoba where imie = 'test' )
";

            var result = await new S4JExecutorForTests().
                ExecuteWithJsonParameters(script1, "{ imie: 'test' }");

            Assert.AreEqual("\"test\"", result.ToJson());
        }

        [Test]
        async public Task test_complex_parameter_save_csharp()
        {
            // await new DbForTest().PrepareDb();

            var script1 = @" 

method ( osoba : any ) 
c#(  )
";

            var result = await new S4JExecutorForTests().
                ExecuteWithJsonParameters(script1, "{ imie: 'test' }");

            Assert.AreEqual("\"test\"", result.ToJson());
        }
    }
}
