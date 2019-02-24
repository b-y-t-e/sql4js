using sql4js.Functions;
using sql4js.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Executor
{
    public class S4JDefaultExecutor : S4JExecutor
    {
        public S4JDefaultExecutor() :
            base(S4JDefaultStateBag.Get())
        {
            
        }
    }
}
