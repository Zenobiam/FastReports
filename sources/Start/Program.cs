using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Start
{
    class Program
    {
        static void Main(string[] args)
        {
            DocumentGenerator.DocumentGenerator gen = new DocumentGenerator.DocumentGenerator();

            var document = new XmlDocument();
            string path = Path.Combine(Environment.CurrentDirectory);
            document.Load(path + "/TestDocForm.xml");

            MemoryStream stream = new MemoryStream();
            
            gen.GetReport(document, "pdf", @"C:\Users\Никита\source\repos\TestFastReports\Start\saves");
            //gen.GetReportToStream(document, stream);

            Console.WriteLine(stream.Length);
        }
    }
}
