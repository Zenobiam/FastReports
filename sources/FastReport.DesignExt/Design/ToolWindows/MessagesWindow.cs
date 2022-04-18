using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents.DotNetBar.Controls;
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Design.ToolWindows
{
  /// <summary>
  /// Represents the "Messages" window.
  /// </summary>
  /// <remarks>
  /// To get this window, use the following code:
  /// <code>
  /// Designer designer;
  /// MessagesWindow window = designer.Plugins.FindType("MessagesWindow") as MessagesWindow;
  /// </code>
  /// </remarks>
  public class MessagesWindow : ToolWindowBase
  {
#if !MONO
    private ListViewEx list;
#else
    private ListView list;
#endif	

    private void FList_DoubleClick(object sender, EventArgs e)
    {
      ListViewItem item = list.SelectedItems[0];
      if (item.SubItems.Count > 1)
      {
        Designer.ActiveReportTab.SwitchToCode();
        Designer.ActiveReport.CodeHelper.Locate((int)item.SubItems[1].Tag, (int)item.SubItems[2].Tag);
      }
      else
      {
        Base obj = Designer.ActiveReport.FindObject((string)item.Tag);
        if (obj != null)
        {
          Designer.SelectedObjects.Clear();
          Designer.SelectedObjects.Add(obj);
          Designer.SelectionChanged(null);
        }
      }
    }

#if !MONO
        private void Reinit()
        {
            base.ReinitDpiSize();
            Image = DpiHelper.ConvertButton16(Res.GetImage(70));
            list.Columns[0].Width = DpiHelper.ConvertUnits(600);
        }
#endif

    /// <inheritdoc/>
    public override void Localize()
    {
      MyRes res = new MyRes("Designer,ToolWindow,Messages");
      Text = res.Get("");
      
      list.Columns.Clear();
      list.Columns.Add(res.Get("Description"));
      list.Columns.Add(res.Get("Line"));
      list.Columns.Add(res.Get("Column"));
      list.Columns[0].Width = DpiHelper.ConvertUnits(600);
    }
#if !MONO
        ///<inheritdoc/>
        public override void ReinitDpiSize()
        {
            if (!Bar.Docked)
                return;
            Reinit();
        }

        ///<inheritdoc/>
        public override void CallReinit(float raio)
        {
            Reinit();
        }
#endif

    /// <summary>
    /// Clears the message list.
    /// </summary>
    public void ClearMessages()
    {
      list.Items.Clear();
    }
    
    /// <summary>
    /// Adds a new message.
    /// </summary>
    /// <param name="description">The message text.</param>
    /// <param name="objName">The name of object related to a message.</param>
    public void AddMessage(string description, string objName)
    {
      ListViewItem item = new ListViewItem();
      item.Text = description;
      item.Tag = objName;
      list.Items.Add(item);
    }

    /// <summary>
    /// Adds a new script-related message.
    /// </summary>
    /// <param name="description">The message text.</param>
    /// <param name="line">The line of the script.</param>
    /// <param name="column">The column of the script.</param>
    public void AddMessage(string description, int line, int column)
    {
      ListViewItem item = new ListViewItem();
      item.Text = description;
      ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
      if (line != -1)
        subItem.Text = line.ToString();
      subItem.Tag = line;
      item.SubItems.Add(subItem);
      subItem = new ListViewItem.ListViewSubItem();
      if (column != -1)
        subItem.Text = column.ToString();
      subItem.Tag = column;
      item.SubItems.Add(subItem);
      list.Items.Add(item);
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MessagesWindow"/> class with default settings.
    /// </summary>
    /// <param name="designer">The report designer.</param>
    public MessagesWindow(Designer designer) : base(designer)
    {
      Name = "MessagesWindow";
#if !MONO
      Image = Res.GetImage(70);

      list = new ListViewEx();
#else
      list = new ListView();
#endif
      list.Dock = DockStyle.Fill;
      list.BorderStyle = BorderStyle.None;
      list.FullRowSelect = true;
      list.View = View.Details;
      list.HideSelection = false;
      list.DoubleClick += new EventHandler(FList_DoubleClick);
      
#if !MONO
      ParentControl.Controls.Add(list);
#else
      Controls.Add(list);
#endif
      Localize();
    }
  }
}
