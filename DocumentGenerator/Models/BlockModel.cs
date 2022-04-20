using FastReport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DocumentGenerator.Models
{
    public class BlockModel
    {
        public XElement modelStruct { get; set; }
        //public XmlElement modelStruct { get; set; }

        // Разметка
        public string type { get; set; } // Тип блока
        public string PointX { get; set; } // Координата х верхнего левого угла блока
        public string PointY { get; set; } // Координата y верхнего левого угла блока
        public string _Width { get; set; } // Ширина блока
        public string _Height { get; set; } // Высота блока

        // Содержание
        public string _Text { get; set; } // Текст находящийся в блоке
        public string _Value { get; set; } // Значение для вставки в блок(иногда используетя заместо текста)
        public float pageWidth { get; set; } // Ширина страницы
        public float pageHeight { get; set; } // Высота страницы
        public float _pageColumnsCount { get; set; } // Количество колонок на странице
        public float _pageRowsCount { get; set; } // Количество строк на странице
        public string _srcImage { get; set; } // Код картинки 

        // Стиль
        #region Style
        public string _fontName //{ get; set; } = "Arial";
        {
            get
            { return fontNameDef; }
            set
            {
                fontNameDef = (value == null || value == "") ? "Times New Roman" : value;
            }
        } // Наименование шрифта
        public string _fontSize
        {
            get
            { return fontSizeDef; }
            set
            {
                fontSizeDef = value == null ? "0" : value;
            }
        } // Размер шрифта

        public string _rowHeight
        {
            get
            { return rowHeightDef; }
            set
            {
                rowHeightDef = (value == null || value == "") ? "0" : value;
            }
        }

        public string _underline 
        {
            get
            { return underlineDef; }
            set
            { underlineDef = value == null ? "false" : value; } 
        } // Подчеркнутый текст

        public string _bold //{ get; set; } = "True";
        {
            get
            { return boldDef; }
            set
            {
                boldDef = value == null ? "false" : value;
            }
        } // Жирный текст
        public string _italic 
        {
            get
            { return italicDef; }
            set
            {
                italicDef = value == null ? "false" : value;
            }
        } // Курсив
        public string _strikeout 
        {
            get
            { return strikeoutDef; }
            set
            {
                strikeoutDef = value == null ? "false" : value;
            }
        } // Зачеркнутый текст

       

        public string _foreColor { get; set; } // Цвет текста
        public string _backColor { get; set; } // Цвет фона
        public string _textAlign { get; set; } // Выравнивание текста 

        // Значения по умолчанию
        private HorzAlign align;
        readonly private Color defaultForeColor = Color.Black;
        readonly private Color defaultBackColor = Color.White;        
        private string fontNameDef;
        private string fontSizeDef;
        private string italicDef;
        private string strikeoutDef;
        private string underlineDef;
        private string boldDef;
        private string rowHeightDef;
        #endregion

        public float SetRowHeight()
        {
            float rowHeight = 25;

            if (_rowHeight != "" && _rowHeight != null)
            {
                float.TryParse(_rowHeight, out float fParse);
                return rowHeight += fParse;
            }
            else
                return rowHeight;
        }

        public float SetFontSize()
        {
            float fontSize = 14;

            if (_fontSize != "" && _fontSize != null)
            {
                float.TryParse(_fontSize, out float fParse);
                return fontSize += fParse;
            }
            else
                return fontSize;            
        }

        public Color SetForeColor() //TODO exception неправильный формат
        {
            if (_foreColor == null || _foreColor == "")
                return defaultForeColor;
            try                
            {
                var foreColor = ColorTranslator.FromHtml(_foreColor);
                return foreColor;
            }
            catch (Exception e)
            {
                Debugger.Log(2, e.StackTrace, e.Message);
                Debug.WriteLine("BlockModel.SetForeColor: Не удалось присвоить значение цвета, установлен цвет по умолчанию");
                return defaultForeColor;
            }
        }
        public Color SetBackColor() //TODO exception неправильный формат
        {
            if (_backColor == null || _backColor == "")
                return defaultBackColor;
            try
            {
                var backColor = ColorTranslator.FromHtml(_backColor);
                return backColor;
            }
            catch (Exception e)
            {
                Debugger.Log(2, e.StackTrace, e.Message);
                Debug.WriteLine("BlockModel.SetForeColor: Не удалось присвоить значение цвета фону, установлен цвет по умолчанию");
                return defaultBackColor;
            }            
        }

        public HorzAlign SetAlign() 
        {
            if(_textAlign == null || _textAlign == "")
                return new HorzAlign();
            try
            {
                switch (_textAlign.ToLower())
                {
                    case "center":
                        align = HorzAlign.Center;
                        return align;
                    case "left":
                        align = HorzAlign.Left;
                        return align;
                    case "right":
                        align = HorzAlign.Right;
                        return align;
                    case "justify":
                        align = HorzAlign.Justify;
                        return align;
                    default:
                        align = HorzAlign.Center;
                        return align;
                }
            }
            catch (Exception e)
            {
                Debugger.Log(2, e.StackTrace, e.Message);
                Debug.WriteLine("BlockModel.SetAlign: Не удалось присвоить значение выравнивания, установлено выравнивание по умолчанию");
                return new HorzAlign();
            }            
        }
        
        public Font SetFontStyle()
        {
            try
            {
                Font font = new Font(_fontName, SetFontSize());

                if (bool.Parse(_bold))
                    font = new Font(font, font.Style ^ FontStyle.Bold);
                if (bool.Parse(_italic))
                    font = new Font(font, font.Style ^ FontStyle.Italic);
                if (bool.Parse(_underline))
                    font = new Font(font, font.Style ^ FontStyle.Underline);
                if (bool.Parse(_strikeout))
                    font = new Font(font, font.Style ^ FontStyle.Strikeout);
                else
                    font = new Font(font, font.Style ^ FontStyle.Regular);

                return font;
            }
            catch (Exception e)
            {
                Debugger.Log(2, e.StackTrace, e.Message);
                Debug.WriteLine("BlockModel.SetFontStyle: Не удалось присвоить значение шрифта, установлен шрифт по умолчанию");
                return new Font("Times New Roman", 14, FontStyle.Regular);
            }            
        }
        /// <Style>
        ///      <Font>Рамер шрифта</Font> 0 - default(12), (+)(-) - default "+,-" value
        ///      <Underline>Подчеркнутый текст</Underline> true/false
        ///      <Bold>Жирный текст</Bold> true/false
        ///      <Italic>Курсив</Italic> strue/false
        ///      <ForeColor>Цвет текста</ForeColor> string
        ///      <BackColor>Цвет фона</BackColor> string
        ///      <TextAlign>Left/Right/Center/Justify</TextAlign> string
        /// </Style>
    }
}
