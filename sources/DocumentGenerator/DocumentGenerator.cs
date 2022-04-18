using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DocumentGenerator.Extensions;
using DocumentGenerator.Models;
using FastReport;
using FastReport.Export.OoXML;
using FastReport.Export.Pdf;
using FastReport.Table;
using FastReport.Utils;
using System.IO;

namespace DocumentGenerator
{

    /// <summary>
    /// Создать объект типа DocumentGenerator
    /// Вызвать функцию GetReport(XmlDocument _document) с параметром типа XmlDocument 
    /// Появиться предсмотр отчета
    /// Docx файл отчета находиться в папке Debug запускаемого проекта
    /// </summary>


    public class DocumentGenerator
    {
        //private readonly string path = Path.Combine(Environment.CurrentDirectory);
        private string _savePath { get; set; } // Путь к итоговому файлу

        public void GetReportToStream(System.Xml.XmlDocument _document, string exportFormat,
                              MemoryStream reportStream)
        {
            //_savePath = savePath;
            if (_document != null)
            {
                var document = _document;
                XDocument xdoc = _document.ToXDocument(); // Данные для создания моделей блоков 
                XmlElement xRoot = document.DocumentElement; // Данные для формирования содержания блоков
                CreateReport(xRoot, xdoc, exportFormat, reportStream);
            }
            else
            {
                Debug.WriteLine("файл отсутствует или является пустым");
            }
        }

        public void GetReport(System.Xml.XmlDocument _document,
                              string exportFormat,
                              string savePath)
        {
            _savePath = savePath;
            if (_document != null)
            {
                var document = _document;
                XDocument xdoc = _document.ToXDocument(); // Данные для создания моделей блоков 
                XmlElement xRoot = document.DocumentElement; // Данные для формирования содержания блоков
                CreateReport(xRoot, xdoc, exportFormat);
            }
            else
            {
                Debug.WriteLine("файл отсутствует или является пустым");
            }
        }


        public void CreateReport(XmlElement xRoot,
                                 XDocument xdoc,
                                 string setExport,
                                 MemoryStream reportStream = null)
        {
            using (Report testReport = new Report())
            {
                //testReport.ScriptText = "using System.Data.dll";

                Border border = new Border
                {
                    Lines = BorderLines.All,
                    Width = 3
                };
           
                #region Page
                    ReportPage testPage = new ReportPage();
                    testReport.Pages.Add(testPage);
                    testPage.CreateUniqueName();

                    /// A4 29,7 x 21 см   k = 2,475 -> horizontal
                    ///                   k = -> vertical
                    /// A5 14,8 × 21,0 cм

                    if (GetNodeValue(xRoot, "Params//Orientation").ToLower() == "horizontal") // PaperWidth (k) -> 1см = 3.78f
                        testPage.Landscape = true;// Ориентация страницы true - альбомная
                    else
                        testPage.Landscape = false;

                    #endregion
           
                #region Data
                    DataBand dataMain = new DataBand(); // Создаем тело страницы
                    testPage.Bands.Add(dataMain);
                    dataMain.CreateUniqueName();

                    dataMain.CanGrow = true;
                    dataMain.CanShrink = true;
                    //dataMain.Border = border;
                    #endregion
                           
                try
                {
                    var node = GetModel(xdoc, xRoot);
           
                    foreach (var model in node) // node блок который надо построить 
                    {
                        if (testPage.Landscape)
                        {
                            model.pageWidth = 29.7f; // ширина берется с учетом отступов по бокам длина стандартная
                            model.pageHeight = 19f;
                        }
                        else
                        {
                            model.pageWidth = 21f;
                            model.pageHeight = 27.7f;
                        }
           
                        //CreateBlock(model); //Task? TODO
                        //=> == null ? model.type.ToLower() : model._Height
           
                        switch (model.type.ToLower()) // в зависимости от типа блока запускает соответствующую функцию
                        {
                            // При добавлении нового блока добавить case и создать функцию для данного блока
                            case "fill":
                                CreateFillObject(model, dataMain);
                                break;
                            case "text":
                                CreateTextObject(model, dataMain);
                                break;
                            case "table":
                                CreateTable(xRoot, xdoc, model, dataMain);
                                break;
                            case "image":
                                CreatePicture(model, dataMain);
                                break;
                            default:
                                Debugger.Log(2, model.type, "Тип не предусмотрен");
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debugger.Log(2, e.StackTrace, e.Message);
                }


                // Сохранение файлов отчета
                //testReport.SavePrepared(path + "testTableReport.frx");
                //testReport.Save("testTableReport.frx");
                //testReport.Save(Stream)
                                

                if (reportStream != null)
                    SaveReportStream(testReport, setExport, reportStream);
                else
                    SaveReport(testReport, setExport);
            }
        }

        private void SaveReport(Report report, string exportFormat)
        {
            report.Prepare();
            Export(exportFormat, report);
            //report.Show();
        }

        private void SaveReportStream(Report report, string exportFormat, MemoryStream reportStream)
        {
            report.Prepare();
            report.Save(reportStream);
            //Export(exportFormat, report);
            //report.Show();
        }

        private IEnumerable<BlockModel> GetModel(XDocument xdoc, XmlElement xRoot)
        {
            // Количество столбцов на странице отчета
            float pageColumnsCount = float.Parse(xRoot.SelectSingleNode("Params//ColumnCount").InnerText);
            // Количество строк на странице отчета
            float pageRowsCount = float.Parse(xRoot.SelectSingleNode("Params//RowsCount").InnerText);


            /// Удалить после добавления в xml картинки
            //Image image = Image.FromFile(@"C:/Users/Никита/source/repos/TestFastReports/DocumentGenerator/Images/mat_11_01.png");
            //MemoryStream memoryStream = new MemoryStream();
            //image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            //byte[] byteImage = memoryStream.ToArray();
            //Encoding encoding = Encoding.Default;
            //string bytesstr = encoding.GetString(byteImage);
            //////////////////////////////////

            // в node храниться разметка и содержание блока
            var node = from val in xdoc.Element("DocumentForm").Element("Structure").Elements("Block")
                       select new BlockModel
                       {
                           // Разметка
                           type = val.Attribute("ContentType").Value,
                           PointX = val.Attribute("X").Value,
                           PointY = val.Attribute("Y").Value,
                           _Width = val.Attribute("Width").Value,
                           _Height = val.Attribute("Height").Value,
                           // Содержание
                           _Text = val.Element("Text")?.Value,
                           _Value = val.Element("Value")?.Value,
                           _pageColumnsCount = pageColumnsCount,
                           _pageRowsCount = pageRowsCount,
                           _srcImage = val.Element("Image")?.Value,
                           // Стиль
                           _fontName = val?.Element("Style")?.Element("FontName")?.Value,
                           _fontSize = val?.Element("Style")?.Element("FontSize")?.Value,
                           _underline = val?.Element("Style")?.Element("Underline")?.Value,
                           _bold = val?.Element("Style")?.Element("Bold")?.Value,
                           _italic = val?.Element("Style")?.Element("Italic")?.Value,
                           _strikeout = val?.Element("Style")?.Element("Strikeout")?.Value,
                           _foreColor = val?.Element("Style")?.Element("ForeColor")?.Value,
                           _backColor = val?.Element("Style")?.Element("BackColor")?.Value,
                           _textAlign = val?.Element("Style")?.Element("TextAlign")?.Value,                           
                       };

            return node;
        }

        private void Export(string setExport, Report testReport)
        {
            try
            {
                switch (setExport)
                {
                    case "pdf":
                        var exportPDF = new PDFExport();
                        exportPDF.Export(testReport, $@"{_savePath}//testTableReport.pdf");
                        break;
                    case "docx":
                        var exportDOCX = new Word2007Export();
                        exportDOCX.Export(testReport, $@"{_savePath}//testTableReport.docx");
                        break;
                    case "doc":
                        var exportDOC = new Word2007Export();
                        exportDOC.Export(testReport, $@"{_savePath}//testTableReport.docx");
                        break;
                    case "xlsx":
                        var exportXLSX = new Excel2007Export();
                        exportXLSX.Export(testReport, $@"{_savePath}//testTableReport.xlsx");
                        break;
                    default:
                        var exportDefault = new PDFExport();
                        exportDefault.Export(testReport, $@"{_savePath}//testTableReport.pdf");
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("DocumentGenerator.export: Не удалось преобразовать отчет на экспорт");
                Debugger.Log(2, e.StackTrace, e.Message);
            }
        }

        #region Создание объектов отчета
        // Объект со значением для вставки
        private void CreateFillObject(BlockModel model, BandBase _parent)
        {
            if (model != null && _parent != null)
            {
                string underline = "________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________";
                TextObject text = new TextObject
                {
                    Bounds = new RectangleF(
                        Units.Centimeters * (float.Parse(model.PointX) * (model.pageWidth / model._pageColumnsCount)),
                        Units.Centimeters * ((float.Parse(model.PointY)) * (model.pageHeight / model._pageRowsCount)),
                        Units.Centimeters * ((float.Parse(model._Width) * (model.pageWidth / model._pageColumnsCount)) - 2f),
                        Units.Centimeters * (float.Parse(model._Height) * (model.pageHeight / model._pageRowsCount))),
                    Parent = _parent,
                    HtmlTags = model._Value == "" ? false : true,
                    Text = model._Value == "" ? $"{model._Text} {underline}" : $"{model._Text} <u>{model._Value}</u>",
                    Trimming = StringTrimming.Word, // переносит текст на новую строку по ближайшему слову    
                    Fill = new SolidFill(model.SetBackColor()),
                    TextColor = model.SetForeColor(),
                    HorzAlign = model.SetAlign(),
                    Font = model.SetFontStyle(),
                };
                text.CreateUniqueName();
            }
        }
        // Текстовый объект
        private void CreateTextObject(BlockModel model, BandBase _parent)
        {
            if (model != null && _parent != null)
            {
                TextObject text = new TextObject
                {
                    Bounds = new RectangleF(
                        Units.Centimeters * (float.Parse(model.PointX) * (model.pageWidth / model._pageColumnsCount)),
                        Units.Centimeters * ((float.Parse(model.PointY)) * (model.pageHeight / model._pageRowsCount)),
                        Units.Centimeters * ((float.Parse(model._Width) * (model.pageWidth / model._pageColumnsCount)) - 2f),
                        Units.Centimeters * (float.Parse(model._Height)) * (model.pageHeight / model._pageRowsCount)),
                    Parent = _parent,
                    Text = model._Text,
                    Fill = new SolidFill(model.SetBackColor()),
                    TextColor = model.SetForeColor(),
                    HorzAlign = model.SetAlign(),
                    Font = model.SetFontStyle(),
                };
            }
        }
        // объект таблицы
        private void CreateTable(XmlElement xRoot, XDocument xDoc, BlockModel model, BandBase _parent)
        {
            if (model != null && _parent != null)
            {
                try
                {               
                    TableObject table = new TableObject
                    {
                        Parent = _parent,
                        Bounds = new RectangleF(
                            Units.Centimeters * (float.Parse(model.PointX) * (model.pageWidth / model._pageColumnsCount)), // 12 -> page columns count
                            Units.Centimeters * ((float.Parse(model.PointY)) * (model.pageHeight / model._pageRowsCount)),
                            Units.Centimeters * ((float.Parse(model._Width) * (model.pageWidth / model._pageColumnsCount)) - 2f),
                            Units.Centimeters * float.Parse(model._Height) * (model.pageHeight / model._pageRowsCount)),
                        RowCount = GetNode(xRoot, "Structure//Block//Data//Row").Count + 1,
                        ColumnCount = GetNode(xRoot, "Structure//Block//Structure//Column").Count,
                        RepeatHeaders = true,
                    };
                    table.CreateUniqueName();

                    #region Динамическое создание таблицы
                    //XmlNodeList nodes = xRoot.SelectNodes("Structure//Block");


                    //TODO динамическое создание таблицы
                    //foreach (XElement element in xDoc.Element("DocumentForm").Element("Structure").Elements("Block"))
                    //{
                    //    XAttribute attribute = element.Attribute("table");
                    //
                    //    
                    //}
                    //var items = from xe in xDoc.Element("DocumentForm").Element("Structure").Elements("Block")
                    //            where xe.Attribute("ContentType").Value == "table"
                    //            select new BlockModel
                    //            {
                    //                element = xe.Elements()
                    //            };
                    //
                    //var nodes = from node in items
                    //            where node.element.Any()
                    //
                    ////using (XmlReader reader = new XmlNodeReader(content))
                    ////{
                    ////    // ...
                    ////}
                    ////
                    //DataSet ds = new DataSet();
                    ////ds.ReadXml(xRoot);



                    /// TODO 
                    /// RepeatHeadres работает только при динамическом создании таблицы
                    /// для этого надо содержание таблицы формировать в отдельный xml файл 
                    /// и добавлять его к источнику данных таблицы
                    /// вручную через рекурсию?
                    /// 
                    #endregion

                    foreach (TableColumn column in table.Columns) // Устанавливает ширину столбца // указать в xml
                    {
                        column.Width = (Units.Centimeters * (((model.pageWidth - 2f) / table.ColumnCount))); // 2f отступ сбоку
                    }
                    foreach (TableRow row in table.Rows) // Устанавливает высоту столбца // указать в xml
                    {
                        row.Height = Units.Centimeters * ((model.pageHeight / model._pageRowsCount) * (float.Parse(model._Height) / table.RowCount)); // 2f отступ сбоку; 3 как коэффициент
                    }

                    // Строка с названиями столбцов
                    for (int i = 0; i < table.ColumnCount; i++)
                    {
                        table[i, 0].Text = GetAttrValue(xRoot, i, "Structure//Block//Structure//Column", "@Title");
                        table[i, 0].Border.Lines = BorderLines.All;
                        table[i, 0].Fill = new SolidFill(model.SetBackColor());
                        table[i, 0].TextColor = model.SetForeColor();
                        table[i, 0].HorzAlign = model.SetAlign();
                        table[i, 0].Font = model.SetFontStyle();
                    }

                    //table[0, 0].ColSpan = 2;  // Задает количество ячеек в столбце в xml указать 
                    //ColSpan для ячеек <Column Title="Вид ядерной силы" Name="stnuc_id" ColSpan = ../>

                    // Задает количество ячеек в столбце
                    for (int i = 0; i < table.ColumnCount; i++)
                    {
                        table[i, 0].ColSpan = int.Parse(GetAttrValue(xRoot, i, "Structure//Block//Structure//Column", "@ColSpan"));
                        table[i, 0].Border.Lines = BorderLines.All;
                    }

                    // Содержание таблицы
                    for (int i = 0; i < table.RowCount - 1; i++) // Rows
                    {
                        //RowSpan объединение ячеек
                        for (int j = 0; j < table.ColumnCount; j++) // Columns
                        {
                            //ColumnSpan объединение столбцов
                            table[j, i + 1].Text = GetTableContent(xRoot, j, i, "Structure//Block//Data//Row");
                            table[j, i + 1].Border.Lines = BorderLines.All;
                            //table[i, 0].Fill = new SolidFill(model.SetBackColor());
                            //table[i, 0].TextColor = model.SetForeColor();
                            //table[i, 0].HorzAlign = model.SetAlign();
                            //table[i, 0].Font = model.SetFontStyle();
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("DocumentGenerator.CreateTableObject: Не удалось создать таблицу");
                    Debugger.Log(2, e.StackTrace, e.Message);                                       
                }
            }
        }

        private void CreatePicture(BlockModel model, BandBase _parent)
        {
            PictureObject picture = new PictureObject
            {
                Bounds = new RectangleF(
                    Units.Centimeters * (float.Parse(model.PointX) * (model.pageWidth / model._pageColumnsCount)), // 12 -> page columns count
                    Units.Centimeters * ((float.Parse(model.PointY)) * (model.pageHeight / model._pageRowsCount)),
                    Units.Centimeters * ((float.Parse(model._Width) * (model.pageWidth / model._pageColumnsCount)) - 2f),
                    Units.Centimeters * float.Parse(model._Height) * (model.pageHeight / model._pageRowsCount)),
                    Parent = _parent,
                //TODO определить в каком формате получать картинку
                Image = GetImage(model._srcImage), 
            };
            picture.CreateUniqueName();
        }

        private Image GetImage(string byteImage)
        {
            try
            {
                Encoding encoding = Encoding.Default;
                //string bytesstr = encoding.GetString(byteImage);

                byte[] imgBytes = encoding.GetBytes(byteImage);

                MemoryStream stream = new MemoryStream();
                foreach (byte b1te in imgBytes) stream.WriteByte(b1te);
                Image blockImage = Image.FromStream(stream);

                return blockImage;
            }
            catch (Exception e)
            {
                Debug.WriteLine("DocumentGenerator.GetImage: недопустимый параметр");
                Debugger.Log(2, e.StackTrace, e.Message);
                return null;
            }
        }

        #endregion

        #region XML

        private string GetNodeValue(XmlElement xRoot, string root)
        {
            var node = xRoot.SelectSingleNode($"{root}");

            return node.InnerText;
        }

        private XmlNodeList GetNode(XmlElement xRoot, string root)
        {
            var node = xRoot.SelectNodes($"{root}");

            return node;
        }

        private string GetType(XmlElement xRoot, string root, string objectType)
        {
            var node = xRoot.SelectSingleNode($"{root}@ContentType");

            return node.InnerText;
        }

        private string GetNode(XmlElement xRoot, string root, string objectType)
        {
            var node = xRoot.SelectSingleNode($"{root}[@ContentType='{objectType}']");

            return node.InnerText;
        }

        private string GetAttrValue(XmlElement xRoot, int index, string root, string value) // заголовки таблицы
        {
            XmlNodeList childnodes = xRoot.SelectNodes(root); // берет запись колонки

            XmlNode node = childnodes.Item(index);

            return node.SelectSingleNode(value).Value;
        }

        private string GetTableContent(XmlElement xRoot, int indexColumn, int indexRow, string root) // содержание таблицы
        {
            XmlNodeList childnodes = xRoot.SelectNodes(root); // берет запись колонки

            return childnodes.Item(indexRow).SelectNodes(".").Item(0).SelectNodes("*").Item(indexColumn).InnerText;
        }
        #endregion
    }
}
