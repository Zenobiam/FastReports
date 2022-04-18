using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport
{
    /// <summary>
    /// Specifies an origin where the new objects inserted from.
    /// </summary>
    public enum InsertFrom
    {
        /// <summary>
        /// Specifies that a new object was inserted from the "Objects" toolbar or "Insert" menu.
        /// </summary>
        NewObject,

        /// <summary>
        /// Specifies that a new object was dragged from the "Dictionary" window.
        /// </summary>
        Dictionary,

        /// <summary>
        /// Specifies that a new object was pasted from the clipboard.
        /// </summary>
        Clipboard
    }

    partial class Base
    {
        #region Properties
        /// <summary>
        /// Gets a value indicating whether the object is selected in the designer.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool IsSelected
        {
            get { return Report != null && Report.Designer != null && Report.Designer.SelectedObjects.IndexOf(this) != -1; }
        }

        /// <summary>
        /// Gets a value indicating whether one of the object's parent is selected in the designer.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsParentSelected
        {
            get
            {
                if (Report.Designer == null)
                    return false;
                Base parent = Parent;
                while (parent != null)
                {
                    if (Report.Designer.SelectedObjects.IndexOf(parent) != -1)
                        return true;
                    parent = parent.Parent;
                }
                return false;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Deletes the object in the designer.
        /// </summary>
        /// <remarks>
        /// <para>This method is called when you delete the object in the designer.</para>
        /// <para>Typically this method calls the <see cref="Dispose"/> method to delete the object and all 
        /// its children. You may override it to delete the object only, and keep children.</para>
        /// </remarks>
        public virtual void Delete()
        {
            Dispose();
        }

        /// <summary>
        /// Called before inserting a new object in the designer.
        /// </summary>
        /// <remarks>
        ///   <para>Do not call this method directly. You may override it if you are developing a
        ///     new component for FastReport.</para>
        ///   <para>
        ///         Some objects are registered in the designer several times with the same object
        ///         type, but different flags. For example, the <see cref="ShapeObject"/>
        ///         represents different shapes: rectangle, roundrect, ellipse and so on. All these
        ///         shapes are registered in the designer using flags (the last parameter in this
        ///         code): 
        ///       <code>
        /// RegisteredObjects.Add(typeof(ShapeObject), "ReportPage,Shapes", 108, "Objects,Shapes,Rectangle", 0);
        /// RegisteredObjects.Add(typeof(ShapeObject), "ReportPage,Shapes", 109, "Objects,Shapes,RoundRectangle", 1);
        /// RegisteredObjects.Add(typeof(ShapeObject), "ReportPage,Shapes", 110, "Objects,Shapes,Ellipse", 2);
        /// </code>
        ///     <para>When we put the "Ellipse" object on a band, the designer creates the
        ///         <b>ShapeObject</b> instance and calls its <b>OnBeforeInsert</b> method with
        ///         <b>flags</b> value set to 2. In turn, the <b>OnBeforeInsert</b> method converts the
        ///         int value of the flags to the shape kind:</para>
        ///       <code>
        /// public override void OnBeforeInsert(int flags)
        /// {
        ///   FShape = (ShapeKind)flags;
        /// }
        /// </code>
        ///   </para>
        /// </remarks>
        /// <param name="flags">Object's flags.</param>
        public virtual void OnBeforeInsert(int flags)
        {
        }

        /// <summary>
        /// Called after the new object was inserted in the designer.
        /// </summary>
        /// <remarks>
        /// <para>Do not call this method directly. You may override it if you are developing a new component 
        /// for FastReport.</para>
        /// <para>This method is called when new object is inserted, pasted from clipboard or dragged from
        /// "Dictionary" window. You may override this method if you need to perform some actions when object
        /// is inserted. Typical implementation invokes the object's editor if "Edit after insert" flag is set
        /// in the designer options.</para>
        /// </remarks>
        /// <param name="source">The insertion source.</param>
        public virtual void OnAfterInsert(InsertFrom source)
        {
        }

        /// <summary>
        /// Called when the user selects another object in the designer.
        /// </summary>
        /// <remarks>
        /// This method is typically used by the in-place object's editor to check if selection was changed and close
        /// the editor.
        /// </remarks>
        public virtual void SelectionChanged()
        {
        }

        /// <summary>
        /// Gets the object's context menu.
        /// </summary>
        /// <returns>Null reference if object does not have a menu.</returns>
        /// <remarks>
        /// <para>Do not call this method directly. You may override it if you are developing a new component 
        /// for FastReport.</para>
        /// <para>You may use base menu classes such as <see cref="ComponentBaseMenu"/>, 
        /// <see cref="ReportComponentBaseMenu"/> to create own context menus.</para>
        /// </remarks>
        public virtual ContextMenuBase GetContextMenu()
        {
            return null;
        }
        #endregion

        private string ExtractDefaultMacrosInternal(Dictionary<string, object> macroValues, string text)
        {
            string[] copyNames = Report.PrintSettings.CopyNames;
            string copyName = "";
            if (copyNames.Length > 0)
            {
                // get zero-based index of printed copy. In the preview, use the first copy name
                int copyIndex = 0;
                if (macroValues.ContainsKey("Copy#"))
                    copyIndex = (int)macroValues["Copy#"];
                // get appropriate copy name
                if (copyIndex >= copyNames.Length)
                    copyIndex = copyNames.Length - 1;
                copyName = copyNames[copyIndex];
            }
            return text.Replace("[COPYNAME#]", copyName);
        }
    }
}