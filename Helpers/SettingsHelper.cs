using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Helpers
{
    public static class SettingsHelper
    {
        public static string Get(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process).Replace("'", "");
        }
    }
}
