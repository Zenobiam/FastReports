using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.RichTextParser
{
    enum ParserStatus
    {
        Collecting,
        Text,
        ControlTag,
        OpenBlock,
        CloseBlock
    }

    /// <summary>
    /// This class detect a RTF control sequences and text.
    /// </summary>
    class RTF_Parser
    {
        enum ParserState
        {
            Neutral,
            Control,
            Number,
            FirstNibble,
            SecondNibble,
            SkipToNext,
            RichFormatExtensions,
            CheckHyphen
        }

        ParserStatus status;
        ParserState parser_state;
        StringBuilder control;
        StringBuilder number;
        StringBuilder text;
        bool has_value;
        bool list_is_active;

        string control_tag;
        long control_num;
        string parsed_text;
        char delimiter;
        bool has_has_value;
        int indirection_counter;
        int skip_counter;
        int extensions_skip_counter;
        // 20200605
        internal bool override_default_color;
        // 20180511
        internal ParagraphFormat current_paragraph_format;
        internal RunFormat current_run_format;
        internal bool insideTable;

        #region Uniocode converters
        internal int current_lang;
        internal long font_charset;
        int default_lang;
        const int Lang_EN_US = 1033;
        Decoder current_unicode_decoder;
        Dictionary<long, Decoder> unicode_decoders = new Dictionary<long, Decoder>();
        Stack<int> lang_ids = new Stack<int>();
        Dictionary<long, long> translate_charset = new Dictionary<long, long>();
        List<byte> raw_chars = new List<byte>();
        #endregion

        public string Text { get { return parsed_text; } }
        public string Control { get { return control_tag; } }
        public long Number { get { return control_num; } }
        public char Delimiter { get { return status == ParserStatus.ControlTag ? delimiter : '\0'; } }
        public ParserStatus Status { get { return status; } }
        public bool HasValue { get { return has_has_value; } }
        public bool ListItem { get { return list_is_active; } set { list_is_active = value; } }
        public bool EndOfFile
        {
            get
            {
                if (text.Length == 0)
                    return false;
                parsed_text = text.ToString();
                return true;
            }
        }


        #region Language selector and translator
        private void SelectUnicodeDecoder(int lcid)
        {
            if (lcid == 0)
                lcid = default_lang == 0 ? Lang_EN_US : default_lang;
            if (!unicode_decoders.ContainsKey(lcid))
            {
                System.Globalization.CultureInfo ci;
                try
                {
                    ci = System.Globalization.CultureInfo.GetCultureInfo(lcid);
                }
                catch (Exception)
                {
                    ci = System.Globalization.CultureInfo.CurrentCulture;
                }
                Encoding encoder = Encoding.GetEncoding(ci.TextInfo.ANSICodePage);
                current_unicode_decoder = encoder.GetDecoder();
                unicode_decoders.Add(lcid, current_unicode_decoder);
            }
            else
            {
                current_unicode_decoder = unicode_decoders[lcid];
            }
            current_lang = lcid;
        }

        int TranslateCharset(long charset)
        {
            switch (charset)
            {
                case 0: return 1033;  // ANSI
                case 1: return default_lang; // default
                case 2: return 42; // Symbol - to fix
                case 77: return 10000; // Mac romant - to fix
                case 78: return 10001; // Mac Shift Jis - to fix
                case 79: return 10003; // Mac Hangul - to fix
                case 80: return 10008; // Mac GB2312 - to fix
                case 81: return 10002; // Mac Big5 - to fix
                //case 82: return 10002; // Johab old
                case 83: return 10005; // Mac Hebrew - to fix
                case 84: return 10004; // Mac Arabic - to fix
                case 85: return 10006; // Mac Greek - to fix
                case 86: return 10081; // Mac Turkish - to fix
                case 87: return 10021; // Mac Thai - to fix
                case 88: return 10029; // Mac East Europe - to fix
                case 89: return 10007; // Mac Russian - to fix
                case 128: return 1041; // 932; Shift JIS
                case 129: return 1042; // 949; Korean Hangul
                case 130: return default_lang; // 1361; Korean Johab
                case 134: return 2052; // 936;  GB2312
                case 136: return 1028; // 950; BIG5
                case 161: return 1032; // 1253; Greel
                case 162: return 1055; // 1254; Turkish
                case 163: return 1066; // 1258; Vietnamese
                case 177: return 1037; // 1255; Hebrew
                case 178: return 1056; // 1256; Arabic
                //case 179: return 0; // Arabic Traditional (old)
                //case 180: return 0; // Arabic user (old)
                //case 181: return 0; // Hebrew user (old)
                case 186: return 1062; // 1257;  Baltic
                case 204: return 1049; // 1251; Russian
                case 222: return 1054; // 874; Thai
                case 238: return 1045; // 1250; East Europe (Polland selected)
                case 254: return current_lang; // 437; // PC437
                case 255: return current_lang; // 850; // OEM
            }
            return current_lang;
        }
        internal void SelectCodepageByFontCharset(long charset)
        {
            int lcid = TranslateCharset(charset);
            SelectUnicodeDecoder(lcid);
        }

        private void PushLocaleDecoder()
        {
            lang_ids.Push(current_lang);
        }

        private void PopLocaleDecoder()
        {
            current_lang = lang_ids.Pop();
            SelectUnicodeDecoder(current_lang);
        }

        private void CollectCharacters()
        {
            try
            {
                byte hex = byte.Parse(number.ToString(), System.Globalization.NumberStyles.HexNumber);
                number = new StringBuilder();
                raw_chars.Add(hex);
            }
            catch (Exception e)
            {
                ;
            }
        }

        //private void TranslateUnicode(System.Globalization.NumberStyles num_style)
        //{
        //    uint unichar = uint.Parse(number.ToString(), num_style);
        //    number.Length = 0;

        //    byte[] conv = new byte[2];
        //    char[] chars = new char[2];

        //    conv[0] = (byte)unichar;
        //    conv[1] = 0;

        //    if (current_lang == 0)
        //        SelectUnicodeDecoder(default_lang == 0 ? 1033 : default_lang);

        //    current_unicode_decoder.GetChars(conv, 0, 1, chars, 0);
        //    text.Append(chars[0]);
        //}
        #endregion

        private void ControlWord()
        {
            control_tag = control.ToString();
            control_num = number.Length != 0 ? long.Parse(number.ToString()) : 0;
            if (control_tag == "lang")
            {
                if(font_charset == 0)
                    SelectUnicodeDecoder((int)control_num);
            }
            if (control_tag == "deflang")
            {
                SelectUnicodeDecoder((int)control_num);
                default_lang = current_lang;
            }
            RestoreEncodedText();
            parsed_text = text.ToString();
            control.Length = 0;
            number.Length = 0;
            text.Length = 0;
            has_has_value = has_value;
            has_value = false;
        }

        private void RestoreEncodedText()
        {
            if (raw_chars.Count != 0)
            {
                byte[] arr = raw_chars.ToArray();
                char[] result = new char[raw_chars.Count];
                int count = current_unicode_decoder.GetChars(arr, 0, raw_chars.Count, result, 0);
                char [] str = new char[count];
                Array.Copy(result, str, count);
                string text = new string(str);
                this.text.Append(text);
                raw_chars.Clear();
            }
        }
        private void AppendCharacter(char ch)
        {
            RestoreEncodedText();
            this.text.Append(ch);
        }

        internal ParserStatus ParseByte(char ch)
        {
            //Console.Write(ch);
            status = ParserStatus.Collecting;
            delimiter = ch;

            if (ch == '{')
            {
                indirection_counter++;
                PushLocaleDecoder();
            }
            if (ch == '}')
            {
                PopLocaleDecoder();
                indirection_counter--;
            }

            switch (parser_state)
            {
                case ParserState.Neutral:
                    switch (ch)
                    {
                        case '{':
                            ControlWord();
                            status = ParserStatus.OpenBlock;
                            break;

                        case '}':
                            ControlWord();
                            status = ParserStatus.CloseBlock;
                            break;

                        case '\\':
                            parser_state = ParserState.Control;
                            break;

                        default:
                            switch (ch)
                            {
                                case '\r':
                                case '\n':
                                case '\t':
                                case '\0':
                                    break;
                                default:
                                    AppendCharacter(ch);
                                    break;
                            }
                            break;
                    }
                    break;

                case ParserState.CheckHyphen:
                    if (char.IsDigit(ch))
                    {
                        number.Append('-');
                        number.Append(ch);
                        parser_state = ParserState.Number;
                        has_value = true;
                        break;
                    }
                    // Substitute Optional HYPHEN with ZERO WIDTH SPACE
                    AppendCharacter((char)8203);
                    parser_state = ParserState.Neutral;
                    status = ParseByte(ch);
                    break;

                case ParserState.Control:
                    if (char.IsLetter(ch))
                    {
                        control.Append(ch);
                    }
                    else if (ch == '-')
                    {
                        parser_state = ParserState.CheckHyphen;
                    }
                    else if (char.IsDigit(ch))
                    {
                        number.Append(ch);
                        parser_state = ParserState.Number;
                        has_value = true;
                    }
                    else if (ch == '\\')
                    {
                        if (control.Length > 0)
                        {
                            ControlWord();
                            status = ParserStatus.ControlTag;
                        }
                        else
                        {
                            AppendCharacter(ch);
                            parser_state = ParserState.Neutral;
                        }
                    }
                    else if (ch == '{')
                    {
                        if (control.Length > 0)
                        {
                            ControlWord();
                            status = ParserStatus.OpenBlock;
                        }
                        else
                        {
                            AppendCharacter(ch);
                            status = ParserStatus.Collecting;
                        }
                        parser_state = ParserState.Neutral;
                    }
                    else if (ch == '}')
                    {
                        if (control.Length > 0)
                        {
                            ControlWord();
                            status = ParserStatus.CloseBlock;
                        }
                        else
                        {
                            AppendCharacter(ch);
                            status = ParserStatus.Collecting;
                        }
                        parser_state = ParserState.Neutral;
                    }
                    else if (char.IsWhiteSpace(ch))
                    {
                        parser_state = ParserState.Neutral;
                        ControlWord();
                        status = ParserStatus.ControlTag;
                    }
                    else if (ch == '*')
                    {
#if false  // Preivous version which ignore pictures in \* control (20210211)
                        parser_state = ParserState.RichFormatExtensions;
                        if (indirection_counter == 0)
                            throw new Exception("Broken RTF format");
                        extensions_skip_counter = indirection_counter - 1;
#else
                        parser_state = ParserState.Neutral;
                        ControlWord();
                        status = ParserStatus.ControlTag;
#endif
                    }
                    else if (ch == ';')
                    {
                        parser_state = ParserState.Neutral;
                        ControlWord();
                        status = ParserStatus.ControlTag;
                    }
                    else if (ch == '\'')
                    {
                        parser_state = ParserState.FirstNibble;
                    }
                    else if (ch == '~')
                    {
                        // Non-breaking space
                        AppendCharacter((char)0x2011);
                        parser_state = ParserState.Neutral;
                    }
                    else if (ch == '_')
                    {
                        AppendCharacter((char)0xa0);
                        parser_state = ParserState.Neutral;
                    }
                    else
                        throw new Exception("RTF format not parsed");

                    break;

                case ParserState.Number:
                    if (char.IsDigit(ch))
                        number.Append(ch);
                    else
                    {
                        if (ch == '{')
                        {
                            parser_state = ParserState.Neutral;
                            status = ParserStatus.OpenBlock;
                        }
                        else if (ch == '}')
                        {
                            parser_state = ParserState.Neutral;
                            status = ParserStatus.CloseBlock;
                        }
                        else
                        {
                            if (this.control.ToString() == "u")
                            {
                                int bukva = int.Parse(number.ToString());
                                AppendCharacter((char)bukva);
                                number.Length = 0;
                                if (ch != '?')
                                {
                                    parser_state = ParserState.SkipToNext;
                                    skip_counter = 3;
                                }
                                else
                                {
                                    parser_state = ParserState.Neutral;
                                    control.Length = 0;
                                }
                                break;
                            }
                            else if (ch == '\\')
                                parser_state = ParserState.Control;
                            else if (ch == ';' || char.IsWhiteSpace(ch))
                                parser_state = ParserState.Neutral;
                            status = ParserStatus.ControlTag;
                        }
                        ControlWord();
                    }
                    break;


                case ParserState.FirstNibble:
                    parser_state = ParserState.SecondNibble;
                    number.Append(ch);
                    break;

                case ParserState.SecondNibble:
                    number.Append(ch);
                    CollectCharacters();
                    //TranslateUnicode(System.Globalization.NumberStyles.HexNumber);
                    parser_state = ParserState.Neutral;
                    break;

                case ParserState.SkipToNext:  // Ignore hexdecmal representation of the character
                    skip_counter--;
                    if (skip_counter == 0)
                    {
                        parser_state = ParserState.Neutral;
                        control.Length = 0;
                    }
                    break;

                case ParserState.RichFormatExtensions:
                    status = ParseExtensionByte(ch);
                    break;
            }
            return status;
        }

#if false // Debug
    StringBuilder dbg = new StringBuilder();
    private ParserStatus ParseExtensionByte(char ch)
    {
      if (extensions_skip_counter == indirection_counter)
      {
        dbg.Append("\n\n");
        parser_state = ParserState.Neutral;
        dbg.Clear();
      }
      else
        dbg.Append(ch);
      return ch == '{' ? ParserStatus.OpenBlock : ch == '}' ? ParserStatus.CloseBlock : ParserStatus.Collecting;
    }
#else
        private ParserStatus ParseExtensionByte(char ch)
        {
            if (extensions_skip_counter == indirection_counter)
            {
                parser_state = ParserState.Neutral;
            }
            return ch == '{' ? ParserStatus.OpenBlock : ch == '}' ? ParserStatus.CloseBlock : ParserStatus.Collecting;
        }
#endif

        internal void ResetRunFormat()
        {
            current_run_format.bold = false;
            current_run_format.italic = false;
            current_run_format.underline = false;
            current_run_format.font_size = 24;
            current_run_format.color = System.Drawing.Color.Black;
            current_run_format.BColor = System.Drawing.Color.White;
            current_run_format.FillColor = System.Drawing.Color.White;
            current_run_format.font_idx = 0;
            current_run_format.script_type = RunFormat.ScriptType.PlainText;
        }

        public void ResetParagraphFormat()
        {
            current_paragraph_format.align = ParagraphFormat.HorizontalAlign.Left;
            current_paragraph_format.line_spacing = 0;
            current_paragraph_format.space_before = 0;
            current_paragraph_format.space_after = 0;
            current_paragraph_format.left_indent = 0;
            current_paragraph_format.right_indent = 0;
            current_paragraph_format.first_line_indent = 0;
            current_paragraph_format.lnspcmult = ParagraphFormat.LnSpcMult.Exactly;
            current_paragraph_format.pnstart = 0;
            current_lang = default_lang;
            current_paragraph_format.list_id = null;
            current_paragraph_format.tab_positions = null;
        }

        internal RTF_Parser()
        {
            parser_state = ParserState.Neutral;
            control = new StringBuilder();
            number = new StringBuilder(12, 12);
            text = new StringBuilder();
            has_value = false;
            override_default_color = false;
            current_lang = 0;
            indirection_counter = 0;
            ResetRunFormat();
            ResetParagraphFormat();
        }

        static RTF_Parser()
        {
#if NETSTANDARD || NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

    }
}
