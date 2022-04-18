using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;

namespace FastReport.Utils
{
  /// <summary>
  /// Provides the message functions. 
  /// </summary>
  public static class FRMessageBox
  {
    /// <summary>
    /// Shows the Message Box with error message.
    /// </summary>
    /// <param name="msg"></param>
    public static void Error(string msg)
    {
      // MessageBoxEx works incorrect in some cases (when using FR in Outlook plugin)
      //MessageBoxEx.UseSystemLocalizedString = true;
      MessageBox.Show(msg, Res.Get("Messages,Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /// <summary>
    /// Shows Message Box with confirmation.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="buttons"></param>
    /// <returns></returns>
    public static DialogResult Confirm(string msg, MessageBoxButtons buttons)
    {
      //MessageBoxEx.UseSystemLocalizedString = true;
      return MessageBox.Show(msg, Res.Get("Messages,Confirmation"), buttons, MessageBoxIcon.Question);
    }

    /// <summary>
    /// Shows information Message Box.
    /// </summary>
    /// <param name="msg"></param>
    public static void Information(string msg)
    {
      //MessageBoxEx.UseSystemLocalizedString = true;
      MessageBox.Show(msg, Res.Get("Messages,Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

  }
}