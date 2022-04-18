using System.ComponentModel;

namespace FastReport.Dialog
{
  /// <summary>
  /// Base class for controls that may contain child controls.
  /// </summary>
  public partial class ParentControl : DialogControl, IParent
  {
    #region Fields
    private DialogComponentCollection controls;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the collection of child controls.
    /// </summary>
    [Browsable(false)]
    public DialogComponentCollection Controls
    {
      get { return controls; }
    }
    #endregion
    
    #region IParent
    /// <inheritdoc/>
    public virtual void GetChildObjects(ObjectCollection list)
    {
      foreach (DialogComponentBase c in controls)
      {
        list.Add(c);
      }
    }

    /// <inheritdoc/>
    public virtual bool CanContain(Base child)
    {
      return (child is DialogComponentBase);
    }

    /// <inheritdoc/>
    public virtual void AddChild(Base child)
    {
      if (child is DialogComponentBase)
        controls.Add(child as DialogComponentBase);
    }

    /// <inheritdoc/>
    public virtual void RemoveChild(Base child)
    {
      if (child is DialogComponentBase)
        controls.Remove(child as DialogComponentBase);
    }

    /// <inheritdoc/>
    public virtual int GetChildOrder(Base child)
    {
      return controls.IndexOf(child as DialogComponentBase);
    }

    /// <inheritdoc/>
    public virtual void SetChildOrder(Base child, int order)
    {
      int oldOrder = child.ZOrder;
      if (oldOrder != -1 && order != -1 && oldOrder != order)
      {
        if (order > controls.Count)
          order = controls.Count;
        if (oldOrder <= order)
          order--;
        controls.Remove(child as DialogComponentBase);
        controls.Insert(order, child as DialogComponentBase);
      }
    }

    /// <inheritdoc/>
    public virtual void UpdateLayout(float dx, float dy)
    {
      // do nothing
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <b>ParentControl</b> class with default settings. 
    /// </summary>
    public ParentControl()
    {
      controls = new DialogComponentCollection(this);
    }
  }
}
