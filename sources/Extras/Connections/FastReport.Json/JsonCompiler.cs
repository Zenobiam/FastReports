using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;

namespace FastReport.Json
{
    /*
        HOW TO USE:
     
        var type = JsonCompiler.Compile(json);
        var properties = type.GetProperties();
        var obj = JsonConvert.DeserializeObject(json, type);

        Report report = new Report();
        report.Load(@"C:\report.frx");
     
        foreach (var prop in properties)
        {
            report.RegisterData((IList)prop.GetValue(obj, null), prop.Name);
        }
    */
    public class JsonCompiler
    {
        public static Type Compile(string json)
        {
            JsonClassGenerator.JsonClassGenerator gen = new JsonClassGenerator.JsonClassGenerator();
            gen.Example = json;
            gen.UseProperties = true;
            gen.Namespace = "__JSON__";
            gen.MainClass = "__JSON__";

            string source = "";
            using (StringWriter sw = new StringWriter())
            {
                gen.OutputStream = sw;
                gen.GenerateClasses();
                sw.Flush();
                source = sw.ToString();
            }

            using (CSharpCodeProvider compiler = new CSharpCodeProvider())
            {
                CompilerParameters parameters = new CompilerParameters()
                {
                    GenerateInMemory = true,
                };
                CompilerResults results = compiler.CompileAssemblyFromSource(parameters, source);
                Type type = results.CompiledAssembly.GetType("__JSON__.__JSON__");

                return type;
            }
        }
    }
}
