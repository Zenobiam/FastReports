using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.IO.Compression;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Utils
{
  partial class ResourceLoader
  {
    /// <summary>
    /// Gets a bitmap from specified assembly resource.
    /// </summary>
    /// <param name="assembly">Assembly name.</param>
    /// <param name="resource">Resource name.</param>
    /// <returns>Bitmap object.</returns>
    public static Bitmap GetBitmap(string assembly, string resource)
    {
      using (Stream stream = GetStream(assembly, resource))
      using (Bitmap bmp = new Bitmap(stream))
      {
        // To avoid the requirement to keep a stream alive, we clone a bitmap.
        Bitmap result = ImageHelper.CloneBitmap(bmp);
                if (resource.ToLower() != "buttons.png" && resource.ToLower() != "splash.png")
                    return DpiHelper.ConvertBitmap(result);
        return result;
      }
    }

    /// <summary>
    /// Gets a bitmap from specified FastReport assembly resource.
    /// </summary>
    /// <param name="resource">Resource name.</param>
    /// <returns>Bitmap object.</returns>
    public static Bitmap GetBitmap(string resource)
    {
      return GetBitmap("FastReport", resource);
    }

    /// <summary>
    /// Gets a cursor from specified assembly resource.
    /// </summary>
    /// <param name="assembly">Assembly name.</param>
    /// <param name="resource">Resource name.</param>
    /// <returns>Cursor object.</returns>
    public static Cursor GetCursor(string assembly, string resource)
    {
      Stream stream = GetStream(assembly, resource);
      Cursor result = new Cursor(stream);
      stream.Dispose();
      return result;
    }

    /// <summary>
    /// Gets a cursor from specified FastReport assembly resource.
    /// </summary>
    /// <param name="resource">Resource name.</param>
    /// <returns>Cursor object.</returns>
    public static Cursor GetCursor(string resource)
    {
      return GetCursor("FastReport", resource);
    }

    /// <summary>
    /// Gets an icon from specified assembly resource.
    /// </summary>
    /// <param name="assembly">Assembly name.</param>
    /// <param name="resource">Resource name.</param>
    /// <returns>Icon object.</returns>
    public static Icon GetIcon(string assembly, string resource)
    {
      Stream stream = GetStream(assembly, resource);
      Icon result = new Icon(stream);
      stream.Dispose();
      return result;
    }

    /// <summary>
    /// Gets an icon from specified FastReport assembly resource.
    /// </summary>
    /// <param name="resource">Resource name.</param>
    /// <returns>Icon object.</returns>
    public static Icon GetIcon(string resource)
    {
      return GetIcon("FastReport", resource);
    }
  }
}
