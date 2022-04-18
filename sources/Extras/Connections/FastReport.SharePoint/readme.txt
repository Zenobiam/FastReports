SharePoint data connection for FastReport.Net
---------------------------------------------


How to compile:
---------------

- open the .sln file;
- update all Nuget libraries;
- compile the project. You will get the "FastReport.SharePoint.dll" file.


How to register this dll in FastReport:
---------------------------------------

You can do it in several ways:

1) register using the FastReport IDE
- open the report designer;
- go "View|Options..." menu;
- on the "Plugins" tab, add the FastReport.Postgres.dll;
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
      <Plugin Name="c:\.....\FastReport.SharePoint.dll"/>
    </Plugins>
  </Config>

3) register the dll programmatically
- add the "FastReport.SharePoint.dll" reference to your project;
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(SharePointDataConnection));


How to use it:
--------------

- now you should be able to create a new SharePoint data source in the 
"Data|Add Data Source..." menu.
 
This example shows how to add a new connection to the report from code:

Report report = new Report();
SharePointDataConnection conn = new SharePointDataConnection();
conn.connectionstring = "site =
conn.url = @"http://{url-address}/";
conn.username = @"{User Name}";
conn.password = @"{*********}";
conn.listid = "02914986-c503-4be6-9e8b-19068628d774";
report.Dictionary.Connections.Add(conn);
conn.CreateAllTables();
