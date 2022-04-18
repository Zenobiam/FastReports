using System;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Print
{
  internal class PageNumbersParser
  {
    private List<int> pages;

    public int Count
    {
      get { return pages.Count; }
    }

    private bool Parse(string pageNumbers, int total)
    {
      pages.Clear();
      string s = pageNumbers.Replace(" ", "");
      if (s == "") return false;

      if (s[s.Length - 1] == '-')
        s += total.ToString();
      s += ',';
      
      int i = 0; 
      int j = 0; 
      int n1 = 0;
      int n2 = 0;
      bool isRange = false;

      while (i < s.Length)
      {
        if (s[i] == ',')
        {
          n2 = int.Parse(s.Substring(j, i - j));
          j = i + 1;
          if (isRange)
          {
            while (n1 <= n2)
            {
              pages.Add(n1 - 1);
              n1++;
            }
          }
          else
            pages.Add(n2 - 1);
          isRange = false;
        }
        else if (s[i] == '-')
        {
          isRange = true;
          n1 = int.Parse(s.Substring(j, i - j));
          j = i + 1;
        }
        i++;
      }

      return true;
    }

    public bool GetPage(ref int pageNo)
    {
      if (pages.Count == 0) 
        return false;
      pageNo = pages[0];
      pages.RemoveAt(0);
      return true;
    }

    public PageNumbersParser(Report report, int curPage)
    {
      pages = new List<int>();

      int total = report.PreparedPages.Count;
      if (report.PrintSettings.PageRange == PageRange.Current)
        pages.Add(curPage - 1);
      else if (!Parse(report.PrintSettings.PageNumbers, total))
      {
        for (int i = 0; i < total; i++)
          pages.Add(i);
      }

#if Demo
      total = 5;
#endif
      // remove bad page numbers
      for (int i = 0; i < pages.Count; i++)
      {
        if (pages[i] >= total || pages[i] < 0)
        {
          pages.RemoveAt(i);
          i--;
        }  
      }  
        
      if (report.PrintSettings.PrintPages == PrintPages.Odd)
      {
        int i = 0;
        while (i < pages.Count)
        {
          if (pages[i] % 2 == 0) 
            i++;
          else
            pages.RemoveAt(i);
        }
      }
      else if (report.PrintSettings.PrintPages == PrintPages.Even)
      {
        int i = 0;
        while (i < pages.Count)
        {
          if (pages[i] % 2 != 0) 
            i++;
          else
            pages.RemoveAt(i);
        }
      }

      // Remove pages with Printable property equal false.
      for (int i = 0; i < pages.Count; i++)
      {
          if (!report.PreparedPages.GetPage(i).Printable)
          {
              pages.RemoveAt(i);
          }
      }

      if (report.PrintSettings.Reverse)
        pages.Reverse();
    }
  }
}