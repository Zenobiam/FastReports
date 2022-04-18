Telegram messenger plugin for FastReport.Net.
-------------------------------------------


How to compile:
---------------

- open the .sln file;
- fix the "FastReport.dll" and the "FastReport.Bars.dll" reference. By default, this dll is located in the
"C:\Program Files\FastReports\FastReport.Net Demo" folder;
- restore NuGet packages (it requires TLSharp and related packages).


How to register this dll in FastReport:
---------------------------------------

You can do it in several ways:

1) register using the FastReport IDE
- open the report designer;
- go "View|Options..." menu;
- on the "Plugins" tab, add the FastReport.TelegramMessenger.dll;
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
      <Plugin Name="c:\.....\FastReport.TelegramMessenger.dll"/>
    </Plugins>
  </Config>

3) register the dll programmatically
- add the "FastReport.TelegramMessenger.dll" reference to your project;
- execute the following code once at the application start:
FastReport.Design.DesignerPlugins.Add(typeof(FastReport.Messaging.TelegramMessenger));


How to use it:
--------------

- you should log in to your Telegram account: enter your phone number to Log In form, enter your login code (you should get it by Telegram or SMS)
 and password (if you have it) to Auth form;
- now you should be able to send reports to Telegram messenger in the Preview "Save" menu;
- there should be created "session.dat" file that stores your session; press "Sign Out" if you want to log in like another user.
