using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
#if NETSTANDARD || NETCOREAPP
using FastReport.Code.CodeDom.Compiler;
#else
using System.CodeDom.Compiler;
#endif
using Microsoft.CSharp;
using FastReport.Editor.Syntax;
using FastReport.Editor.Syntax.Parsers;
using FastReport.Utils;
using FastReport.Data;
using FastReport.Design.PageDesigners.Code;

namespace FastReport.Code
{
    partial class CsCodeHelper
    {
        #region Protected Methods
        protected override NetSyntaxParser CreateSyntaxParser()
        {
            NetSyntaxParser parser = new CsParser();
            parser.Options = SyntaxOptions.Outline | SyntaxOptions.CodeCompletion |
              SyntaxOptions.SyntaxErrors | SyntaxOptions.QuickInfoTips | SyntaxOptions.SmartIndent;
            parser.CodeCompletionChars = SyntaxConsts.ExtendedNetCodeCompletionChars.ToCharArray();
            return parser;
        }

        protected override void CompareEvent(string eventParams, string codeLine, List<string> eventNames)
        {
            // split code line to words
            // for example: "private void NewClick(object sender, System.EventArgs e)"
            string[] codeWords = codeLine.Split(new string[] { " ", ",", "(", ")",
        "private", "public", "protected", "virtual", "override", "void" }, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            if (i < codeWords.Length)
            {
                // now we get: "NewClick object sender System.EventArgs e"
                string eventName = codeWords[i];
                string pars = "";
                i++;
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
                    Editor.Position = new Point(method.Position.X + 2, method.Position.Y + 2);
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
                eventParams = "(object sender, " + pars[1].ParameterType.Name + " e)";
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
                    Editor.Lines.Insert(endPos.Y + 1, Indent(2) + "private void " + eventName + eventParams);
                    Editor.Lines.Insert(endPos.Y + 2, Indent(2) + "{");
                    Editor.Lines.Insert(endPos.Y + 3, Indent(3));
                    Editor.Lines.Insert(endPos.Y + 4, Indent(2) + "}");
                    Editor.Focus();
                    Editor.Position = new Point(3 * CodePageSettings.TabSize, endPos.Y + 3);
                    return true;
                }
            }
            FRMessageBox.Error(Res.Get("Messages,EventError"));
            return false;
        }
        #endregion
    }

}