using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FastReport.Utils;
using FastReport.Data;

namespace FastReport.Design
{
  internal class UndoRedo : IDisposable
  {
    private ReportTab owner;
    private List<UndoRedoInfo> undo;
    private List<UndoRedoInfo> redo;

    public Designer Designer
    {
      get { return owner.Designer; }
    }
    
    public Report Report
    {
      get { return owner.Report; }
    }
    
    public BlobStore BlobStore
    {
      get { return owner.BlobStore; }
    }
    
    public int UndoCount
    {
      get { return undo.Count; }
    }

    public int RedoCount
    {
      get { return redo.Count; }
    }

    public string[] UndoNames
    {
      get
      {
        string[] result = new string[UndoCount];
        for (int i = 0; i < undo.Count; i++)
        {
          result[i] = undo[i].name;
        }
        return result;
      }
    }

    public string[] RedoNames
    {
      get
      {
        string[] result = new string[RedoCount];
        for (int i = 0; i < redo.Count; i++)
        {
          result[i] = redo[i].name;
        }
        return result;
      }
    }

    private void SaveReport(Stream stream)
    {
      using (FRWriter writer = new FRWriter())
      {
        writer.SerializeTo = SerializeTo.Undo;
        writer.BlobStore = BlobStore;
        writer.Write(Report);
        writer.Save(stream);
      }
    }

    private void LoadReport(Stream stream)
    {
      ParameterCollection saveParams = new ParameterCollection(null);
      saveParams.Assign(Report.Parameters);

      Report.Clear();
      using (FRReader reader = new FRReader(Report))
      {
        reader.DeserializeFrom = SerializeTo.Undo;
        stream.Position = 0;
        reader.BlobStore = BlobStore;
        reader.Load(stream);
        reader.Read(Report);
      }

      Report.Parameters.AssignValues(saveParams);
    }

    private string GetUndoActionName(string action, string objName)
    {
      string s = "";
      if (!String.IsNullOrEmpty(objName))
        s = objName;
      else if (Designer.SelectedObjects != null)
      {
        if (Designer.SelectedObjects.Count == 1)
          s = Designer.SelectedObjects[0].Name;
        if (Designer.SelectedObjects.Count > 1)
          s = String.Format(Res.Get("Designer,UndoRedo,NObjects"), Designer.SelectedObjects.Count);
      }  
      return String.Format(Res.Get("Designer,UndoRedo," + action), s);
    }

    public void ClearUndo()
    {
      foreach (UndoRedoInfo info in undo)
      {
        info.Dispose();
      }
      undo.Clear();
      if (BlobStore != null)
        BlobStore.Clear();
    }
    
    public void ClearRedo()
    {
      foreach (UndoRedoInfo info in redo)
      {
        info.Dispose();
      }
      redo.Clear();
    }
    
    public void AddUndo(string name, string objName)
    {
      UndoRedoInfo info = new UndoRedoInfo(GetUndoActionName(name, objName));
      SaveReport(info.stream);
      undo.Insert(0, info);
    }

    public void GetUndo(int actionsCount)
    {
      if (actionsCount >= undo.Count - 1)
        actionsCount = undo.Count - 1;
      UndoRedoInfo info = undo[actionsCount];
      LoadReport(info.stream);

      for (int i = 0; i < actionsCount; i++)
      {
        info = undo[0];
        redo.Insert(0, info);
        undo.Remove(info);
      }
    }

    public void GetRedo(int actionsCount)
    {
      UndoRedoInfo info = redo[actionsCount - 1];
      LoadReport(info.stream);

      for (int i = 0; i < actionsCount; i++)
      {
        info = redo[0];
        undo.Insert(0, info);
        redo.Remove(info);
      }
    }

    public void Dispose()
    {
      ClearUndo();
      ClearRedo();
    }
    
    public UndoRedo(ReportTab tab)
    {
      owner = tab;
      undo = new List<UndoRedoInfo>();
      redo = new List<UndoRedoInfo>();
    }


    private class UndoRedoInfo : IDisposable
    {
      public string name;
      public MemoryStream stream;

      public void Dispose()
      {
        stream.Dispose();
      }

      public UndoRedoInfo(string name)
      {
                this.name = name;
        stream = new MemoryStream();
      }
    }

  }
  
}
