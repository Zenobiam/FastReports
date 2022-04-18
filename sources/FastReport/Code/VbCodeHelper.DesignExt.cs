using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using FastReport.Editor.Syntax;
using FastReport.Editor.Syntax.Parsers;
#if NETSTANDARD || NETCOREAPP
using FastReport.Code.VisualBasic;
#else
using Microsoft.VisualBasic;
#endif
using System.Drawing;
using FastReport.Utils;
using FastReport.Data;
using FastReport.Design.PageDesigners.Code;

namespace FastReport.Code
{
    partial class VbCodeHelper
    {
        #region Protected Methods
        protected override NetSyntaxParser CreateSyntaxParser()
        {
            NetSyntaxParser parser = new VbParser();
            parser.Options = SyntaxOptions.Outline | SyntaxOptions.CodeCompletion |
              SyntaxOptions.SyntaxErrors | SyntaxOptions.QuickInfoTips | SyntaxOptions.SmartIndent;
            parser.CodeCompletionChars = SyntaxConsts.ExtendedNetCodeCompletionChars.ToCharArray();
            return parser;
        }

        protected override void CompareEvent(string eventParams, string codeLine, List<string> eventNames)
        {
            // split code line to words
            // for example: "Private Sub NewClick(ByVal sender As object, ByVal e As System.EventArgs)"
            string[] codeWords = codeLine.Split(new string[] { " ", ",", "(", ")",
        "Private", "Public", "Protected", "Virtual", "Override", "Sub", "ByVal", "As" }, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            if (i < codeWords.Length)
            {
                // now we get: "NewClick sender object e System.EventArgs"
                string eventName = codeWords[i];
                string pars = "";
                i += 2;
                // first argument
                if (i < codeWords.Length)
                    pars = codeWords[i] + ",";
                i += 2;
                if (i < codeWords.Length)
                {
                    string secondArg = codeWords[i];
                    if (secondArg.IndexOf('.') != -1)
                    {
                        // if second argument is, for example, "System.EventArgs", take only "EventArgs" part
                        string[] splitSecondArg = secondArg.Split(new char[] { '.' });
                        secondArg = splitSecondArg[splitSecondArg.Length - 1];
                    }
                    pars += secondArg + ",";
                }
                if (String.Compare(eventParams, pars, true) == 0)
                    eventNames.Add(eventName);
            }
        }

        #endregion

        #region Public Methods
        public override void LocateHandler(string eventName)
        {
            List<ISyntaxNode> methods = new List<ISyntaxNode>();
            EnumSyntaxNodes(methods, Parser.SyntaxTree.Root, NetNodeType.Method);

            foreach (ISyntaxNode method in methods)
            {
                if (method.Name == eventName)
                {
                    Editor.Focus();
                    Editor.Position = method.Position;
                    Editor.Position = new Point(method.Position.X + 2, method.Position.Y + 1);
                    break;
                }
            }
        }

        public override bool AddHandler(Type eventType, string eventName)
        {
            // get delegate params
            MethodInfo invoke = eventType.GetMethod("Invoke");
            System.Reflection.ParameterInfo[] pars = invoke.GetParameters();
            string eventParams = "";
            if (pars.Length == 2)
                eventParams = "(ByVal sender As object, ByVal e As " + pars[1].ParameterType.Name + ")";
            else
            {
                FRMessageBox.Error(String.Format(Res.Get("Messages,DelegateError"), eventType.ToString()));
                return false;
            }

            List<ISyntaxNode> classes = new List<ISyntaxNode>();
            EnumSyntaxNodes(classes, Parser.SyntaxTree.Root, NetNodeType.Class);
            foreach (ISyntaxNode node in classes)
            {
                if (node.Name == "ReportScript")
                {
                    Point startPos = node.Position;
                    Point endPos = new Point(0, startPos.Y + node.Size.Height);
                    Editor.Lines.Insert(endPos.Y, "");
                    Editor.Lines.Insert(endPos.Y + 1, Indent(2) + "Private Sub " + eventName + eventParams);
                    Editor.Lines.Insert(endPos.Y + 2, Indent(3));
                    Editor.Lines.Insert(endPos.Y + 3, Indent(2) + "End Sub");
                    Editor.Focus();
                    Editor.Position = new Point(3 * CodePageSettings.TabSize, endPos.Y + 2);
                    return true;
                }
            }
            FRMessageBox.Error(Res.Get("Messages,EventError"));
            return false;
        }
        #endregion
    }

}