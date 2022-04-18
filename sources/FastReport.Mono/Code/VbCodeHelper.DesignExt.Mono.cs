using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.VisualBasic;
using System.Drawing;
using FastReport.Utils;
using FastReport.Data;
using FastReport.Design.PageDesigners.Code;

namespace FastReport.Code
{
    partial class VbCodeHelper
    {
        #region Public Methods
    public override List<string> GetEvents(Type eventType)
    {
      // build a string containing the event params. 
      // for example, event type "EventHandler" will generate the following string: "object,EventArgs,"
      string eventParams = "";
      MethodInfo invoke = eventType.GetMethod("Invoke");
      System.Reflection.ParameterInfo[] evparams = invoke.GetParameters();
      foreach (System.Reflection.ParameterInfo p in evparams)
      {
        eventParams += p.ParameterType.Name + ",";
      }

      List<string> eventNames = new List<string>();
      string[] lines = Editor.Text.Split(new char[] { '\n' });
      foreach (string line in lines)
      {
        string codeLine = line.Trim();
        if (codeLine.StartsWith("Private Sub"))
        {
          // split code line to words
          // for example: "Private Sub NewClick(ByVal sender As object, ByVal e As System.EventArgs)"
          string[] codeWords = codeLine.Split(new string[] { " ", ",", "(", ")", "Private", "Sub", "ByVal", "As" }, StringSplitOptions.RemoveEmptyEntries);
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
      }

      return eventNames;
    }


    public override void LocateHandler(string eventName)
    {
      string[] lines = Editor.Text.Split(new char[] { '\n' });
      int i = 0;
      foreach (string line in lines)
      {
        string codeLine = line.Trim();
        if (codeLine.StartsWith("Private Sub " + eventName))
        {
          Editor.Focus();
          Editor.Select(i, 0);
          break;
        }
        i += line.Length + 1;
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

      string[] lines = Editor.Text.Split(new char[] { '\n' });
      int charIndex = 0;
      for (int i = 0; i < lines.Length; i++)
      {
        string line = lines[i];
        charIndex += line.Length + 1;
        line = line.Trim();
        if (line.StartsWith("Public Class ReportScript"))
        {
          string insert = "    Private Sub " + eventName + eventParams + "\r\n      ";
          Editor.Text = Editor.Text.Insert(charIndex, insert + "\r\n    End Sub\r\n\r\n");
          Editor.Focus();
          Editor.Select(charIndex + insert.Length, 0);
          return true;
        }
      }
      FRMessageBox.Error(Res.Get("Messages,EventError"));
      return false;
    }
        #endregion
    }

}