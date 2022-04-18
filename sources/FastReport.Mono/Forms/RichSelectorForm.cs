using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FastReport.Forms
{
  public partial class RichSelectorForm : Form
  {
    RichObject rich_object;
    bool modified;

    /// <summary>
    /// Check if rich text was updated
    /// </summary>
    public bool IsModified { get { return modified; } }

    /// <summary>
    /// Load and save rich text 
    /// </summary>
    /// <param name="obj"></param>
    public RichSelectorForm(RichObject obj)
    {
      rich_object = obj;
      InitializeComponent();
      modified = false;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "RichText files (*.rtf)|*.rtf";
      dialog.Title = "Load rich text";

      if (dialog.ShowDialog() == DialogResult.OK)
      {
        FileStream fs = new FileStream(dialog.FileName, FileMode.Open);
        byte [] text = new byte[fs.Length];
        fs.Read(text, 0, (int) fs.Length);
        rich_object.Text = System.Text.Encoding.Default.GetString(text);
        modified = true;
      }
      dialog.Dispose();
      this.Close();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      SaveFileDialog dialog = new SaveFileDialog();
      dialog.Filter = "RichText files (*.rtf)|*.rtf";
      dialog.Title = "Save rich text to file...";

      if (dialog.ShowDialog() == DialogResult.OK)
      {
        byte[] text = System.Text.Encoding.UTF8.GetBytes(rich_object.Text);
        FileStream fs = new FileStream(dialog.FileName, FileMode.Create);
        fs.Write(text, 0, text.Length);
        fs.Close();
      }
      dialog.Dispose();
      this.Close();
    }
  }
}
