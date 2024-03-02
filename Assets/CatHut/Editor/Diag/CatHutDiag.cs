using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatHut
{
    public static class CatHutDiag
    {
        [Conditional("DEBUG")]
        public static void FunctionCalled()
        {
            var caller = new System.Diagnostics.StackFrame(1, false);
            Debug.WriteLine("Class:" + caller.GetMethod().DeclaringType.FullName + " called:" + caller.GetMethod().Name);
        }
    }
}
