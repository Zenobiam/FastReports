﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Globalization;
using FastReport.Export.PS;

namespace FastReport.Export.Ppml
{
    /// <summary>
    /// Contains Dashes enum
    /// </summary>
    public enum Dashes
    {
        /// <summary>
        /// Specifies the Dash.
        /// </summary>
        Dash,

        /// <summary>
        /// Specifies the Dot.
        /// </summary>
        Dot,

        /// <summary>
        /// Specifies the DashDot.
        /// </summary>
        DashDot,

        /// <summary>
        /// Specifies the DashDotDot.
        /// </summary>
        DashDotDot,

        /// <summary>
        /// Specifies the Double line.
        /// </summary>
        Double
    }
    class PPMLDocument : PSDocument
    {
        private XmlAttribute nsAttribute;
        private XmlElement root;
        private List<XmlElement> pages = new List<XmlElement>();
        private XmlElement internalData;
        private XmlElement mark;
        private XmlElement document;
        private XmlDocument doc = new XmlDocument();
        /// <summary>
        /// Create Window.
        /// </summary>
        public new void CreateWindow(string name, float Width, float Height)
        {
            windowHeight = Height;
            windowWidth = Width;
            XmlDeclaration xml_vers = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(xml_vers);

            root = doc.CreateElement("PPML");

            nsAttribute = doc.CreateAttribute("xmlns");
            nsAttribute.Value = "urn://www.podi.org/ppml/ppml3";
            root.Attributes.Append(nsAttribute);

            nsAttribute = doc.CreateAttribute("xmlns", "xsi", "http://www.w3.org/2000/xmlns/");
            nsAttribute.Value = "http://www.w3.org/2001/XMLSchema-instance";
            root.Attributes.Append(nsAttribute);

            nsAttribute = doc.CreateAttribute("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
            nsAttribute.Value = "urn://www.podi.org/ppml/ppml3 http://www.podi.org/ppml/ppml300.xsd";
            root.Attributes.Append(nsAttribute);

            nsAttribute = doc.CreateAttribute("Version");
            nsAttribute.Value = "3.0";
            root.Attributes.Append(nsAttribute);  

            doc.AppendChild(root);

            XmlElement PAGE_DESIGN = doc.CreateElement("PAGE_DESIGN");
            nsAttribute = doc.CreateAttribute("TrimBox");
            nsAttribute.Value = "0 0 " + FloatToString(Width) + " " + FloatToString(Height);
            PAGE_DESIGN.Attributes.Append(nsAttribute);
            root.AppendChild(PAGE_DESIGN);

            XmlElement JOB = doc.CreateElement("JOB");
            root.AppendChild(JOB);

            document = doc.CreateElement("DOCUMENT");
            JOB.AppendChild(document);
        }

        public void AddPage()
        {
            XmlElement PAGE = doc.CreateElement("PAGE");
            document.AppendChild(PAGE);

            mark = doc.CreateElement("MARK");
            nsAttribute = doc.CreateAttribute("Position");
            nsAttribute.Value = FloatToString(0) + " " + FloatToString(0);
            mark.Attributes.Append(nsAttribute);
            PAGE.AppendChild(mark);

            XmlElement OBJECT = doc.CreateElement("OBJECT");
            nsAttribute = doc.CreateAttribute("Position");
            nsAttribute.Value = "0 0";
            OBJECT.Attributes.Append(nsAttribute);
            mark.AppendChild(OBJECT);

            XmlElement SOURCE = doc.CreateElement("SOURCE");
            nsAttribute = doc.CreateAttribute("Format");
            nsAttribute.Value = "application/postscript";
            SOURCE.Attributes.Append(nsAttribute);
            nsAttribute = doc.CreateAttribute("Dimensions");
            nsAttribute.Value = FloatToString(windowWidth) + " " + FloatToString(windowHeight);
            SOURCE.Attributes.Append(nsAttribute);
            OBJECT.AppendChild(SOURCE);

            internalData = doc.CreateElement("INTERNAL_DATA");

            SOURCE.AppendChild(internalData);
        }
            
        /// <summary>
        /// Add image as PPMLObject
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void AddImage(string filename, float left, float top, float width, float height)
        {
            if (!String.IsNullOrEmpty(filename))
            {

                XmlElement OBJECT = doc.CreateElement("OBJECT");
                nsAttribute = doc.CreateAttribute("Position");
                nsAttribute.Value = FloatToString(left) + " " + FloatToString(windowHeight - height - top);
                OBJECT.Attributes.Append(nsAttribute);
                mark.AppendChild(OBJECT);

                XmlElement SOURCE = doc.CreateElement("SOURCE");
                nsAttribute = doc.CreateAttribute("Format");
                nsAttribute.Value = "image/jpeg";
                SOURCE.Attributes.Append(nsAttribute);
                nsAttribute = doc.CreateAttribute("Dimensions");
                nsAttribute.Value = FloatToString(width) + " " + FloatToString(height);
                SOURCE.Attributes.Append(nsAttribute);
                OBJECT.AppendChild(SOURCE);

                XmlElement EXTERNAL_DATA = doc.CreateElement("EXTERNAL_DATA");
                nsAttribute = doc.CreateAttribute("Src");
                nsAttribute.Value = filename;
                EXTERNAL_DATA.Attributes.Append(nsAttribute);
                SOURCE.AppendChild(EXTERNAL_DATA);
            }
        }

        /// <summary>
        /// Save svg file.
        /// </summary>
        public new void Save(string filename)
        {
            doc.Save(filename);
        }

        /// <summary>
        /// Save svg stream.
        /// </summary>
        public new void Save(Stream stream)
        {
            doc.Save(stream);
        }
        public new void Finish()
        {
            internalData.InnerText = psData.ToString();
            psData.Length = 0;
        }

        /// <param name="name"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public PPMLDocument(string name, float Width, float Height)
        {
            CreateWindow(name, Width, Height);        
        }
        private string FloatToString(double flt)
        {
            return ExportUtils.FloatToString(flt);
        }
    }
}
