using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json.Linq;
using sql4js.Parser;
using sql4js.Executor;
using System;
using System.Linq;
using Xunit;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using sql4js.Helpers.CoreHelpers;

namespace sql4js.tests
{
    /// <summary>
    /// TODO:  add test for sqlite, add test for configuration, add test for parameter types
    /// </summary>
    public class tests_dependecies
    {
        [Fact]
        public void deserializer_cs_should_properly_deserialize_outer_lists()
        {
            string json = @"[1,{a:1,b:2,c:[6,7,{a:'a',b:'2018-01-01'}]},3,null]";

            dynamic dynObj = JsonToCsDeserializer.Deserialize(json);

            Assert.Equal(
                typeof(List<object>),
                dynObj.GetType());
        }

        [Fact]
        public void deserializer_cs_should_properly_deserialize_inner_lists()
        {
            string json = @"[1,{a:1,b:2,c:[6,7,{a:'a',b:'2018-01-01'}]},3,null]";

            dynamic dynObj = JsonToCsDeserializer.Deserialize(json);

            Assert.Equal(
                typeof(List<object>),
                dynObj[1]["c"].GetType());
        }

        [Fact]
        public void deserializer_cs_should_properly_deserialize_inner_lists_version2()
        {
            string json = @"{a:1,b:2,c:[6,7,{a:'a',b:'2018-01-01',c:[1,2,3,{a:1}]}]}";

            dynamic dynObj = JsonToCsDeserializer.Deserialize(json);

            Assert.Equal(
                typeof(List<object>),
                dynObj["c"].GetType());

            Assert.Equal(
                typeof(List<object>),
                dynObj["c"][2]["c"].GetType());
        }

        [Fact]
        public void deserializer_dynamic_should_properly_deserialize_outer_lists()
        {
            string json = @"[1,{a:1,b:2,c:[6,7,{a:'a',b:'2018-01-01'}]},3,null]";

            dynamic dynObj = JsonToDynamicDeserializer.Deserialize(json);

            Assert.Equal(
                typeof(List<object>),
                dynObj.GetType());
        }

        [Fact]
        public void deserializer_dynamic_should_properly_deserialize_inner_lists()
        {
            string json = @"[1,{a:1,b:2,c:[6,7,{a:'a',b:'2018-01-01'}]},3,null]";

            dynamic dynObj = JsonToDynamicDeserializer.Deserialize(json);

            Assert.Equal(
                typeof(List<object>),
                dynObj[1].c.GetType());
        }

        [Fact]
        public void deserializer_dynamic_should_properly_deserialize_inner_lists_version2()
        {
            string json = @"{a:1,b:2,c:[6,7,{a:'a',b:'2018-01-01',c:[1,2,3,{a:1}]}]}";

            dynamic dynObj = JsonToDynamicDeserializer.Deserialize(json);

            Assert.Equal(
                typeof(List<object>),
                dynObj.c.GetType());

            Assert.Equal(
                typeof(List<object>),
                dynObj.c[2].c.GetType());
        }

        [Fact]
        public void deserializer_dynamic_should_properly_deserialize_simple_int_as_json()
        {
            string json = @"1";

            dynamic dynObj = JsonToDynamicDeserializer.Deserialize(json);

            Assert.Equal(
                1,
                dynObj);
        }
    }

}
