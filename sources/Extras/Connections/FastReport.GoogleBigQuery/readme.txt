Google Big Query data connection for FastReport.Net
---------------------------------------------------

How to compile:
---------------

- you need the Google.Apis.Bigquery.v2 to be installed. Obtain it
at http://www.nuget.org/packages/Google.Apis.Bigquery.v2/;
To install Google.Apis.Bigquery.v2 Client Library, run the following command in the Package Manager Console
(From the Tools menu, select Library Package Manager and then click Package Manager Console):
"Install-Package Google.Apis.Bigquery.v2 -Pre"
- open the .sln file;
- compile the project. You will get the "FastReport.GoogleBigQuery.dll" file.

How to register this dll in FastReport:
---------------------------------------

You can do it in several ways:

1) register using the FastReport IDE
- open the report designer;
- go "View|Options..." menu;
- on the "Plugins" tab, add the FastReport.GoogleBigQuery.dll;
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
      <Plugin Name="c:\.....\FastReport.GoogleBigQuery.dll"/>
    </Plugins>
  </Config>

3) register the dll programmatically
- add the "FastReport.GoogleBigQuery.dll" reference to your project;
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(GoogleBigQueryDataConnection));


How to use it:
--------------

- now you should be able to create a new Google Big Query data source in the 
"Data|Add Data Source..." menu. Fill "Google Project ID", "Client ID", "Client Secret" from your Google API properties.


Please feel free to contact us if you have any questions or comments support@fast-report.com
