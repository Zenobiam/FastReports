using FastReport.Utils;
#if NETSTANDARD || NETCOREAPP
using FastReport.Code.CodeDom.Compiler;
#else
using System.CodeDom.Compiler;
#endif
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace FastReport.Code
{
    partial class AssemblyDescriptor
    {
        private void ErrorMsg(CompilerError ce, int line)
        {
            if (Report.Designer != null)
                Report.Designer.ErrorMsg(Res.Get("Messages,Error") + " " + ce.ErrorNumber + ": " + ce.ErrorText, line, ce.Column);
        }

        private void ErrorMsg(string errObjName, CompilerError ce)
        {
            if (Report.Designer != null)
                Report.Designer.ErrorMsg(errObjName + ": " + Res.Get("Messages,Error") + " " + ce.ErrorNumber + ": " + ce.ErrorText, errObjName);
        }

        private void ErrorMsg(string msg)
        {
            if (Report.Designer != null)
                Report.Designer.ErrorMsg(msg, null);
        }

        private void ReviewReferencedAssemblies(StringCollection assemblies)
        {
#if COMMUNITY
            List<string> replace = new List<string>();

            foreach (string str in assemblies)
            {
                if (str.ToLower().EndsWith("fastreport.dll"))
                    replace.Add(str);
            }
            foreach (string str in replace)
            {
                if (assemblies.Contains(str))
                    assemblies.Remove(str);
            }
            string fastreport_path = Path.Combine(Config.GetTempFolder(), "TopRafters42.dll");
            assemblies.Add(fastreport_path);
#endif
        }
    }
}
