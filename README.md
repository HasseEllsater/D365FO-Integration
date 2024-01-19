# D365FO-Integration

The project here is a simple test tool to test Custom services in D365FO using JSON. The usage areas of the tool is very specific, it is primarly targeting developers creating custom services using JSON for request and response. It is also used by us in our projects by our application consultants when we perform different end to end tests in the solutions.

Feel free to download and contribute. The UI is based on MahApps.Metro and ControlzEx components, all components used is available via nuget.

The communication part of the tool is based on the samples provided by Microsoft. The UI is based on a hamburger menu sample from punker76, see the amazing code samples here https://github.com/punker76/code-samples.

The solution also consist of a setup program that let you easily distribute the app within your project, endpoints, services and id and keys can be added to simplify for the users that's testing your API if you add the information to the files in the installation.

To be able to load the setup program in Visual Studio the Extension "Microsoft Visual Studio Installer Project" must be installed first.

Visit DevMethod's site and check out the DevMethod tools for Azure TFVC projects: https://devmethod.azurewebsites.net
