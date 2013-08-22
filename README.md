Seattle311
==========

Seattle311 is an Open311 client for Windows Phone for the City of Seattle. This application provides complete feature parity for the city's Find It, Fix It application for iPhone and Android. 

How do I make this work for my city?
====================================

The Seattle311 source code can be easily modified to work with your city by making a few small changes. Here is what you need to do:

1. Rename all instances of `Seattle311` and `Seattle 311` to a name of your choosing.
2. Modify the service client declarations in the `App.xaml.cs` file to point to your city's Open311 endpoint and specify your own API key. The code looks like this: 
``
Seattle311Client = new ServiceClient("servicerequest.seattle.gov", "YOUR_API_KEY");
``
3. Modify the service client declarations in the `App.xaml.cs` file to use your own Imgur API key. The code looks like this:
``
Seattle311Client.ImgurAPIKey = "8fdb6a32174203e";
``
4. Build, package, publish, an deploy your application to the Windows Phone Store!

License Information
===================

This software, and its dependencies, are distributed free of charge and licensed under the GNU General Public License v2. For more information about this license and the terms of use of this software, please review the LICENSE.txt file.
