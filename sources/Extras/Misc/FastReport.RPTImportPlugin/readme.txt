Convertor from Crystal Reports (*.rpt) to FastReport .NET (*.frx) file.
-------------------------------------------


How to compile:
---------------

- you need the Crystal Reports to be installed.
- open the .sln file (please note there are 2 .sln files FastReport.RPTImportPlugin.sln
(for Visual Studio 2015 and higher);
- restore all packages in packages.config (nuget.exe restore);
- fix the "FastReport.dll" reference. By default, this dll is located in the
"C:\Program Files\FastReports\FastReport.Net Demo" folder;
- compile the project. You will get the "FastReport.RPTImportPlugin.dll" file.


How to register this dll in FastReport:
---------------------------------------

You can do it in several ways:

1) register using the FastReport IDE
- open the report designer;
- go "File|Options..." menu in Ribbon interface ("View|Options..." menu in standard interface);
- on the "Plugins" tab, add the FastReport.RPTImportPlugin.dll;
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
      <Plugin Name="c:\.....\FastReport.RPTImportPlugin.dll"/>
    </Plugins>
  </Config>

3) register the dll programmatically
- add the "FastReport.RPTImportPlugin.dll" reference to your project;
- execute the following code once at the application start:
FastReport.Design.DesignerPlugins.Add(typeof(FastReport.Design.ImportPlugins.RPT.RPTImportPlugin));


How to use it:
--------------

- now you should be able to open a Crystal Reports (*.rpt) files in the 
"File|Open..." menu.
