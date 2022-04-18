
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FastReport.RichTextParser;
using FastReport.Code;
using FastReport.Utils;

namespace FastReport
{

  partial class RichObject
  {
    IList<string> expression_list = null;
    int expressionIndex;

    private void GenerateExpressionList()
    {

      FindTextArgs args = new FindTextArgs();
      string plain_text = string.Empty;
      using (RTF_DocumentParser parser = new RTF_DocumentParser())
      {
        parser.Load(Text);
        using (RTF_ToTextSaver saver = new RTF_ToTextSaver(parser.Document))
          args.Text = new FastString(saver.PlainText);
      }

      string[] brackets = Brackets.Split(new char[] { ',' });
      args.OpenBracket = brackets[0];
      args.CloseBracket = brackets[1];
      args.StartIndex = ActualTextStart;

      expression_list = new List<string>();

      while (args.StartIndex < args.Text.Length - 2)
      {
        string expression = CodeUtils.GetExpression(args, false);
        if (expression == "")
          break;
        expression_list.Add(expression);
        args.StartIndex = args.EndIndex;
      }
    }

    enum States { Wait, Open };

    private void CalculateExpressions()
    {
      if(ConvertRichText)
      {
        foreach (ReportComponentBase obj in ChildObjects)
        {
          obj.SetReport(this.Report);
          obj.GetData();
        }
      }
      char ch;
      expressionIndex = 0;
      int internal_brackets_count = 0;
      States state = States.Wait;

      if (expression_list == null)
        GenerateExpressionList();

      if(Text == null)
        return;

      using (MemoryStream rtf_stream = new MemoryStream())
      {
        using (StreamWriter rtf_writer = new StreamWriter(rtf_stream, Encoding.UTF8))
        {
          for (int i = 0; i < Text.Length; i++)
          {
            ch = Text[i];
            switch (state)
            {
              case States.Wait:
                if (ch != '[')
                  rtf_writer.Write(ch);
                else
                  state = States.Open;
                break;

              case States.Open:
                if (ch == '[')
                  internal_brackets_count++;
                if (ch != ']')
                  break;
                if (internal_brackets_count > 0)
                {
                  internal_brackets_count--;
                  break;
                }
                string formattedValue = CalcAndFormatExpression(expression_list[expressionIndex], expressionIndex);
                bool lf = false;
                foreach(char chr in formattedValue)
                {
                  if(chr=='\r')
                  {
                    rtf_writer.Write("\\line ");
                    lf = true;
                  }
                  else
                  {
                    if (chr == '\n')
                    {
                      if (lf != true)
                        rtf_writer.Write("\\line ");
                    }
                    else if ((int)chr <= 127)
                      rtf_writer.Write(chr);
                    else
                      rtf_writer.Write("\\u{0}\\'3f", (uint)chr);
                    lf = false;
                  }
                }
                expressionIndex++;
                state = States.Wait;
                break;
            }
          }
          rtf_writer.Flush();
          using (StreamReader rtf_reader = new StreamReader(rtf_stream, Encoding.UTF8))
          {
            rtf_stream.Seek(0, SeekOrigin.Begin);
            Text = rtf_reader.ReadToEnd();
          }
        }
      }
     }
    }
}
