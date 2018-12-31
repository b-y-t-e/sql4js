using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json.Linq;
using sql4js.Parser;
using sql4js.Executor;
using System;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            await PrepareDb();

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
            await PrepareDb();

            var script1 = @" method(param1) sql( select @param1 + 1  ) ";

            var result = await new S4JExecutorForTests().
                Execute(script1, 199);

            var txt = result.ToJson();

            Assert.Equal(
                @"200",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_object_from_sql()
        {
            await PrepareDb();

            var script1 = @" {sql( select imie, nazwisko from osoba )} ";

            var result = await new S4JExecutorForTests().
                Execute(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"{""imie"":""imie1"",""nazwisko"":""nazwisko1""}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_objects_from_sql()
        {
            await PrepareDb();

            var script1 = @" [{sql( select imie, nazwisko from osoba )}] ";

            var result = await new S4JExecutorForTests().
                Execute(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"[{""imie"":""imie1"",""nazwisko"":""nazwisko1""},{""imie"":""imie2"",""nazwisko"":""nazwisko2""}]",
                result.ToJson());
        }

        private async Task PrepareDb()
        {
            var script = @"

sql(

if object_id('dbo.osoba') is not null  
    drop table dbo.osoba

create table dbo.osoba(
    id int identity(1,1), 
    imie varchar(max), 
    nazwisko nvarchar(max), 
    wiek int, 
    dataurodzenia datetime, 
    utworzono datetime default(getdate())
)

insert into dbo.osoba(imie, nazwisko, wiek, dataurodzenia, utworzono)
select 'imie1', 'nazwisko1', 20, '2000-01-01', getdate();

insert into dbo.osoba(imie, nazwisko, wiek, dataurodzenia, utworzono)
select 'imie2', 'nazwisko2', 30, '1990-01-01', getdate();

)

";

            var result = await new S4JExecutorForTests().
                Execute(script);
        }
    }
}
