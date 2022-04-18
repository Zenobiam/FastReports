VistaDB 3.4 data connection for FastReport.Net
----------------------------------------------


How to compile:
---------------

- you need the VistaDB Data Provider to be installed. Obtain it
at http://www.vistadb.net/;
- open the .sln file;
- check that "VistaDB.NET20.dll" reference is correct. 
If Visual Studio complains about it, fix this reference as well. By default,
this dll is located in the GAC;
- compile the project. You will get the "FastReport.VistaDB.dll" file.


How to register this dll in FastReport:
---------------------------------------

You can do it in several ways:

1) register using the FastReport IDE
- open the report designer;
- go "View|Options..." menu;
- on the "Plugins" tab, add the FastReport.VistaDB.dll;
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
      <Plugin Name="c:\.....\FastReport.VistaDB.dll"/>
    </Plugins>
  </Config>

3) register the dll programmatically
- add the "FastReport.VistaDB.dll" reference to your project;
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(VistaDBDataConnection));


How to use it:
--------------

- now you should be able to create a new VistaDB data source in the 
"Data|Add Data Source..." menu.
 