using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Globalization;
using System.Windows.Forms.VisualStyles;
#if !MONO
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Utils
{
  partial class Res
  {
        private static Size ImageSize;
    private static List<Bitmap> FImages;
    private static ImageList FImageList;
    private static bool FImagesLoaded = false;

    private static void LoadImages()
    {            
#if !MONO && !FRCORE
            //if (!Config.DisableHighDpi)
                UIStyleUtils.EnableHighDpi();
#endif
            ImageSize = DpiHelper.ConvertUnits(new Size(16, 16));
      FImagesLoaded = true;
      FImages = new List<Bitmap>();

      using (Bitmap images = ResourceLoader.GetBitmap("buttons.png"))
      {
        int x = 0;
        int y = 0;
        bool done = false;

        do
        {
          Bitmap oneImage = new Bitmap(16, 16);
          using (Graphics g = Graphics.FromImage(oneImage))
          {
            g.DrawImage(images, new Rectangle(0, 0, 16, 16), x,y,16,16, GraphicsUnit.Pixel);
          }
          FImages.Add(oneImage);

          x += 16;
          if (x >= images.Width)
          {
            x = 0;
            y += 16;
          }
          done = y > images.Height;
        }
        while (!done);
      }
    }

    private static void CreateImageList()
    {
      FImageList = new ImageList();
            ImageSize = DpiHelper.ConvertUnits(new Size(16, 16));
      FImageList.ImageSize = DpiHelper.ConvertUnits(new Size(16, 16));
      FImageList.ColorDepth = ColorDepth.Depth32Bit;
      
      foreach (Bitmap bmp in FImages)
      {
        FImageList.Images.Add(bmp);
      }
    }
    
    internal static void AddImage(Bitmap img)
    {
        GetImages().Images.Add(img, img.GetPixel(0, 15));
        FImages.Add(img);
    }

    /// <summary>
    /// Gets the standard images used in FastReport as an <b>ImageList</b>.
    /// </summary>
    /// <returns><b>ImageList</b> object that contains standard images.</returns>
    /// <remarks>
    /// FastReport contains about 240 truecolor images of 16x16 size that are stored in one 
    /// big image side-by-side. This image can be found in FastReport resources (the "buttons.png" resource).
    /// </remarks>
    public static ImageList GetImages()
    {
      if (!FImagesLoaded)
        LoadImages();
      if (FImageList == null)
        CreateImageList();
      return FImageList;  
    }

        // <summary>
        /// Gets clone of Res.GetImage();
        /// </summary>
        public static ImageList GetImages(float ratio)
        {
            ImageList list = new ImageList();
            list.ImageSize = new Size((int)(16 * ratio), (int)(16 * ratio));
            list.ColorDepth = ColorDepth.Depth32Bit;

            foreach (Bitmap bmp in GetImages().Images)
            {
                list.Images.Add((Bitmap)bmp.Clone());
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Bitmap GetImage(int index, float ratio)
        {
            if (!FImagesLoaded)
                LoadImages();
            return new Bitmap(FImages[index], new Size((int)(ratio * 16), (int)(ratio * 16)));
        }

        /// <summary>
        /// Updates the dimensions of images in assets based on the dpi value
        /// </summary>
        public static void UpdateResourcres()
        {
            CreateImageList();
        }


        /// <summary>
        /// Gets an image with specified index.
        /// </summary>
        /// <param name="index">Image index (zero-based).</param>
        /// <returns>The image with specified index.</returns>
        /// <remarks>
        /// FastReport contains about 240 truecolor images of 16x16 size that are stored in one 
        /// big image side-by-side. This image can be found in FastReport resources (the "buttons.png" resource).
        /// </remarks>
        public static Bitmap GetImage(int index)
    {
      if (!FImagesLoaded)
        LoadImages();
      return new Bitmap(FImages[index], ImageSize);
    }

    /// <summary>
    /// Gets an image with specified index and converts it to <b>Icon</b>.
    /// </summary>
    /// <param name="index">Image index (zero-based).</param>
    /// <returns>The <b>Icon</b> object.</returns>
    public static Icon GetIcon(int index)
    {
      return Icon.FromHandle(GetImage(index).GetHicon());
    }

#if !MONO
    static Res()
    {
        // for using FastReport.dll without FastReport.Bars.dll if the designer is not shown
        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (a.ManifestModule.Name == "FastReport.Bars.dll")
            {
                RegisterLocalizeStringEventHandler();
                break;
            }
        }
    }

    private static void RegisterLocalizeStringEventHandler()
    {
        LocalizationKeys.LocalizeString += new DotNetBarManager.LocalizeStringEventHandler(LocalizationKeys_LocalizeString);
    }

    private static void LocalizationKeys_LocalizeString(object sender, LocalizeEventArgs e)
    {
      switch (e.Key)
      {
        case "barsys_autohide_tooltip":
          e.LocalizedValue = Res.Get("Designer,ToolWindow,AutoHide");
          e.Handled = true;
          break;
        case "barsys_close_tooltip":
          e.LocalizedValue = Res.Get("Designer,ToolWindow,Close");
          e.Handled = true;
          break;
        case "cust_mnu_addremove":
          e.LocalizedValue = Res.Get("Designer,Toolbar,AddOrRemove");
          e.Handled = true;
          break;
        case "sys_morebuttons":
          e.LocalizedValue = Res.Get("Designer,Toolbar,MoreButtons");
          e.Handled = true;
          break;
      }
    }
#endif
  }
  
}
