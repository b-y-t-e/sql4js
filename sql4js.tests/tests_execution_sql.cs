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
                ExecuteWithParameters(script1);

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
                ExecuteWithParameters(script1, 199);

            var txt = result.ToJson();

            Assert.Equal(
                @"200",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_object_parameter_in_sql()
        {
            await PrepareDb();

            var script1 = @" method(param1) sql( select @param1_imie + '!' + @param1_nazwisko  ) ";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1, new osoba() { imie = "IMIE", nazwisko = "NAZWISKO" });

            var txt = result.ToJson();

            Assert.Equal(
                @"""IMIE!NAZWISKO""",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_list_in_object_parameter_in_sql()
        {
            await PrepareDb();

            var script1 = @" method(param1) sql( select @param1_imie + '!' + @param1_nazwisko + '!' + cast((select count(*) from @param1_rodzice) as varchar(max))  ) ";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1, new osobaWithList() { imie = "IMIE", nazwisko = "NAZWISKO", rodzice = new List<osoba>() { new osoba() { imie = "tata" }, new osoba() { imie = "mama" } } } );

            var txt = result.ToJson();

            Assert.Equal(
                @"""IMIE!NAZWISKO!2""",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_list_parameter_in_sql()
        {
            await PrepareDb();

            var script1 = @" method(param1) sql( select count(*) from @param1  ) ";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(
                    script1,
                    new List<osoba>() {
                        new osoba() { imie = "IMIE1", nazwisko = "NAZWISKO2" },
                        new osoba() { imie = "IMIE1", nazwisko = "NAZWISKO2" }
                    });

            var txt = result.ToJson();

            Assert.Equal(
                @"2",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_object_from_sql()
        {
            await PrepareDb();

            var script1 = @" {sql( select imie, nazwisko from osoba where imie = 'imie1' )} ";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"{""imie"":""imie1"",""nazwisko"":""nazwisko1""}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_object_with_inner_fields_from_sql()
        {
            await PrepareDb();

            var script1 = @" {sql( select imie, nazwisko, idrodzica from osoba where imie = 'imie1' ), ""parent"": { sql(select imie from osoba where id = @idrodzica) } } ";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"{""imie"":""imie1"",""nazwisko"":""nazwisko1"",""idrodzica"":3,""parent"":{""imie"":""imie rodzica""}}",
                result.ToJson());
        }

        [Fact]
        async public void executor_should_understand_objects_from_sql()
        {
            await PrepareDb();

            var script1 = @" [{sql( select imie, nazwisko from osoba where idrodzica is not null )}] ";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script1);

            var txt = result.ToJson();

            Assert.Equal(
                @"[{""imie"":""imie1"",""nazwisko"":""nazwisko1""},{""imie"":""imie2"",""nazwisko"":""nazwisko2""}]",
                result.ToJson());
        }

        private async Task PrepareDb()
        {
            var script = @"
[
    sql(

        if object_id('dbo.osoba') is not null  
            drop table dbo.osoba

        create table dbo.osoba(
            id int identity(1,1), 
            idrodzica int,
            imie varchar(max), 
            nazwisko nvarchar(max), 
            wiek int, 
            dataurodzenia datetime, 
            utworzono datetime default(getdate())
        )

    ),

    sql(

        insert into dbo.osoba(imie, nazwisko, wiek, dataurodzenia, utworzono)
        select 'imie1', 'nazwisko1', 20, '2000-01-01', getdate();

        insert into dbo.osoba(imie, nazwisko, wiek, dataurodzenia, utworzono)
        select 'imie2', 'nazwisko2', 30, '1990-01-01', getdate();

        insert into dbo.osoba(imie, nazwisko, wiek, dataurodzenia, utworzono)
        select 'imie rodzica', 'nazwisko rodzica', 50, '1970-01-01', getdate();

        update osoba set idrodzica = scope_identity() where imie <> 'imie rodzica';

    )
]
";

            var result = await new S4JExecutorForTests().
                ExecuteWithParameters(script);
        }
    }

    class osoba
    {
        public string imie;
        public string nazwisko;
    }

    class osobaWithList
    {
        public string imie;
        public string nazwisko;
        public List<osoba> rodzice = new List<osoba>();
    }

}
