ANNdotNET v1.0 - deep learning tool on .NET platform
============

ANNdotNET – is an open source project for deep learning written in C\# for
creating and training deep learning models. The application relies on Microsoft
Cognitive Toolkit, CNTK, and it is supposed to be GUI tool for CNTK library with
extensions in data preprocessing, model evaluation, exporting and deploying deep
learning models. It is hosted at <http://github.com/bhrnjica/anndotnet>.



### How to run ANNdotNET GUI Tool from release section

This option is handy in case you don't have installed Visual Studio or you want
to use the application without source code. The following actions should be
performed:

-   Download binaries from the release section at:
    https://github.com/bhrnjica/anndotnet/releases,

-   Unzip the binaries on your machine and run \`anndotnet.wnd.exe\` exe file.

-   Once the application is run, select one of many pre-calculated projects
    placed on Start Page.

The following image shows Iris Flower Identification project opened in ANNdotNET
GUI Tool

![](Images/58dff84cb0ae27a3172d28ec7d695e68.jpg)

### How to run ANNdotNET GUI Tool from source code

The second option is similar as in case of ANNdotNET CMD Tool. Only difference
is the startup project which in this case should be \`anndotnet.wnd\`, and it is
shown on the image below.

![](Images/14684be79e3fc6460a7908db00e0b616.jpg)

To recap the section, once the user download the source code and open it by
using Visual Studio, the **anndotnet.wnd** should be selected as startup
project. Beside startup project, user also should change the system architecture
to x64, and press F5. Once the Visual Studio compiles and builds the solution,
the ANNdotNET windows desktop application is shown on the user’s machine.
