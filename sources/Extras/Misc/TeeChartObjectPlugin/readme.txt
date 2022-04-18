TeeChartObject - Steema TeeChart object.
-------------------------------------------

How to compile:
---------------

- open the .sln file;
- fix the "FastReport.dll" reference. By default, this dll is located in the
"C:\Program Files\FastReports\FastReport.Net Demo" folder;
- restore NuGet packages (Steema TeeChart Library must be installed - https://www.nuget.org/packages/Steema.TeeChart.NET.Standard/);
- check that "TeeChart.dll" reference is correct; if Visual Studio complains about it, fix it (you could find "TeeChart.dll" in restored package);

How to register this dll in FastReport:
---------------------------------------

You can do it in several ways:

1) register using the FastReport IDE
- open the report designer;
- go "View|Options..." menu;
- on the "Plugins" tab, add the TeeChartObjectPlugin.dll;
- restart FastReport. If you are working in the Visual Studio IDE, restart 
it as well;

2) edit manually the FastReport.config file
- by default, this file is located in the "C:\Documents and Settings\user_name\
Local Settings\Application Data\FastReport" folder;
- close any running instances of FastReport.Net;
- open the config file in any text editor and modify it in the following way:
  <?xml version="1.0" encoding="utf-8"?>
  <Config>
    ...
    <Plugins>
      <Plugin Name="c:\.....\TeeChartObjectPlugin.dll"/>
    </Plugins>
  </Config>

3) register the dll programmatically
- add the "TeeChartObjectPlugin.dll" reference to your project;
- execute the following code once at the application start:
RegisteredObjects.Add(typeof(TeeChartObject), "ReportPage", 125);

In any case you have to place TeeChart.dll and TeeChartObjectPlugin.dll in the same folder with you application!


How to use it:
--------------

- now you should be able to create Steema TeeChart objects in the Designer; you can find it on the bottom of the Toolbox.
