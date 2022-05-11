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
            document.Load(path + "/test_doc_form.xml");

            MemoryStream stream = new MemoryStream();
            
            gen.GetReport(document, "pdf", @"D:\MonoDocumentGenerator\TestFastReports\Start\saves\testTableReport2");
            //gen.GetReportToStream(document, stream);

            Console.WriteLine(stream.Length);
        }
    }
}
