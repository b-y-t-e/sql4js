using sql4js.Executor;
using System;
using System.Diagnostics;

namespace sql4j_for_profiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var script1 = @"[ 1, 2, 3 , [1, 2, 3,   { a: 1, b: 2 }  ]   ]";
            var script2 = @"[ 1, dynlan(  result = list(); return result;  )   ]";

            Stopwatch st = Stopwatch.StartNew();

            for (var i = 0; i < 1000; i++)
            {
                var task = new S4JExecutorForTests().
                    ExecuteWithParameters(script2);

                var txt = task.Result.ToJson();
            }
            st.Stop();

            Console.WriteLine("Hello World!");
        }
    }
}
