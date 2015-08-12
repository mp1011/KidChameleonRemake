using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public static class ExceptionExtnesions
    {

        public static Exception Flatten(this Exception ex)
        {
            var first = ex;
            var sb = new StringBuilder();

            while (ex != null)
            {
                sb.Append(ex.Message).Append("; ");
                ex = ex.InnerException;
            }

            return new Exception(sb.ToString(),first);
        }
    }
}
