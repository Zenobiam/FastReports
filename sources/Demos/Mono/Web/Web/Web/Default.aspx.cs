using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using FastReport;
using FastReport.Utils;
using FastReport.Web;
using System.Collections.Generic;
using System.IO;
using FastReport.Data;
using System.Drawing;

public partial class _Default : System.Web.UI.Page 
{

    protected void Page_Load(object sender, EventArgs e)
    {
        Version.Text = "ver." + Config.Version;
        if (WebReport1.ReportFile == String.Empty)
        {
            WebReport1.ReportFile = "~/App_Data/Barcode.frx";
        }

    }

    protected void LeftMenu_Init(object sender, EventArgs e)
    {

    }

    protected void LeftMenu_MenuItemClick(object sender, MenuEventArgs e)
    {

    }

    protected void WebReport1_StartReport(object sender, EventArgs e)
    {
        RegisterData((sender as WebReport).Report);
    }

    private void RegisterData(Report FReport)
    {
        DataSet FDataSet = new DataSet();
        
	    FDataSet.ReadXml(HttpContext.Current.Request.MapPath("~/App_Data/nwind.xml"));

        FReport.RegisterData(FDataSet, "NorthWind");

        List<Category> list = new List<Category>();
        Category category = new Category("Beverages", "Soft drinks, coffees, teas, beers");
        category.Products.Add(new Product("Chai", 18m));
        category.Products.Add(new Product("Chang", 19m));
        category.Products.Add(new Product("Ipoh coffee", 46m));
        list.Add(category);

        category = new Category("Confections", "Desserts, candies, and sweet breads");
        category.Products.Add(new Product("Chocolade", 12.75m));
        category.Products.Add(new Product("Scottish Longbreads", 12.5m));
        category.Products.Add(new Product("Tarte au sucre", 49.3m));
        list.Add(category);

        category = new Category("Seafood", "Seaweed and fish");
        category.Products.Add(new Product("Boston Crab Meat", 18.4m));
        category.Products.Add(new Product("Red caviar", 15m));
        list.Add(category);

        FReport.RegisterData(list, "Categories BusinessObject", BOConverterFlags.AllowFields, 3);
    }
}

public class Category
{
    private string FName;
    private string FDescription;
    private List<Product> FProducts;

    public string Name
    {
        get { return FName; }
    }

    public string Description
    {
        get { return FDescription; }
    }

    public List<Product> Products
    {
        get { return FProducts; }
    }

    public Category(string name, string description)
    {
        FName = name;
        FDescription = description;
        FProducts = new List<Product>();
    }
}

public class Product
{
    private string FName;
    private decimal FUnitPrice;

    public string Name
    {
        get { return FName; }
    }

    public decimal UnitPrice
    {
        get { return FUnitPrice; }
    }

    public Product(string name, decimal unitPrice)
    {
        FName = name;
        FUnitPrice = unitPrice;
    }
}
