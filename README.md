# ANNdotNET
[![license](https://img.shields.io/github/license/mashape/apistatus.svg?maxAge=2592000)](https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md)
[![DOI](https://zenodo.org/badge/126162816.svg)](https://zenodo.org/badge/latestdoi/126162816)
![developed by](https://avatars3.githubusercontent.com/u/12556447?s=55&u=f2cd3be70373c9654b9d53a4f69ddfd7a8ed6596&v=4=)

![ANNdotNET Logo](./src/tool/anndotnet.wnd/Images/annLogo_start2.png)

ANNdotNET –  is an open source project for deep learning written in C# and supports .NET and .NET Core platform. The main purpose of the project is creating and training deep learning models. One of the main project component is ANNdotNET ML Engine which is based on Microsoft Cognitive Toolkit, CNTK. The project supposed to be GUI tool for CNTK library with extensions in data preprocessing, model evaluation,
 exporting and deploying. 
 
The project is hosted at http://github.com/bhrnjica/anndotnet, and the project documentation can be found at the project wiki pages at https://github.com/bhrnjica/anndotnet/wiki.  

The process of creating, training, evaluating and exporting models is provided from the GUI Application
 and does not require knowledge for supported programming languages. The ANNdotNET is ideal in several scenarios:

- more focus on network development and training process using classic desktop approach, instead of focusing on coding, 
- less time spending on debugging source code, more focusing on different configuration and parameter variants,
- ideal for engineers/users who are not familiar with programming languages, 
- in case the problem requires coding custom models, or training process, ANNdotNET CMD provides high level of API for such implementation,
- all ml configurations developed with GUI tool,can be handled with CMD tool and vice versa.  

There are dozens of pre-calculated projects included in the installer which can be opened from the Start page as well as from CMD tool. The annprojects are
 based on famous datasets from several categories: regression, binary and multi class classification problems, image classifications, times series, etc.
In pre-calculated projects the user can find how to use various neural network configurations e.g. feed forward,
 deep neural network, LSTM recurrent nets, embedding and drop out layers. Also, each project can be modified
 in terms of change its network configuration, learning and training parameters, as well as create new ml configurations.


![ANNdotNET Logo](./docs/images/anndotnet_startwnd.png)


In order to handle with machine learning configuration file (*mlconfig*), ANNdotNET provides **visual network designer** (VN Designer) capable of creating neural networks of any
 configurations and any combination of layers. The VN Designer is based on layer concept, where user can easily add, delete or modify nn layers as simply as manipulating with the list view items.


![ANNetwork Designer](./docs/images/annetwork_designer.png)     


# Software Requirements
ANNdotNET is x64 Windows desktop application running on .NET Framework 4.7.2. and .NET Core 2.0. In order to run the application, the following software components need to be installed:

* Windows 10 with x64 architecture
* .NET Framework 4.7.2 +, and .NET Core 2.+
* [Visual C++ 2017 version 15.4 v14.11 toolset](https://aka.ms/vs/15/release/vc_redist.x64.exe)
* [Visual C++ Redistributable Packages for Visual Studio 2013](https://www.microsoft.com/en-us/download/details.aspx?id=40784)
* [CUDA 10 for GPU support](https://developer.nvidia.com/cuda-10.0-download-archive)
* [cuDNN v7.4.2 (Dec 14, 2018), for CUDA 10.0](https://developer.nvidia.com/rdp/cudnn-archive)

Note: The application is tested on clean *Windows Pro 10 1709 build*. Probably the application will run on Windows 8 and Windows 7 as well, once the user installs the prerequisites.

# How to run application
In order to run the application there are two possibilities:

## Run ANNdotNET from source code
1. Clone the GitHub repository http://github.com/bhrnjica/anndotnet 
2. Open `anndotnet.gui.net.sln` in Visual Studio 2017,
3. Setup `anndotnet.wnd` as startup project.
3. Change build architecture of the solution into x64,
4. Right click on solution item and restore Nuget Packages, 
4. Press F5 for build and run the application.  

![](./docs/images/14684be79e3fc6460a7908db00e0b616.jpg)


## Run ANNdotNET from release section
1. Got to http://github.com/bhrnjica/anndotnet/releases and find the ANNdotNET latest release,
2. Download the zip installer, and extract the content on  your disk,
3. Open extracted folder, select `anndotnet.wnd.exe` and run the application.
4. Once the application is run, select one of many pre-calculated projects
    placed on Start Page.

The following image shows Bike SHaring project opened in ANNdotNET
GUI Tool. More precisely the image shows Data preparation modul. 

![](./docs/images/anndotnet_data_preparation1.png)

Since version 1.2, ANNdotNET support creating Image Classification, so the following image shows Cat and
 Dog image classification project created by using ANNdotNET v1.2+.

![](./docs/images/anndotnet_data_preparation2.png)

## How to install ANNdotNET Excel AddIn 

In order to use ANNdotNET Export to Excel feature, the `ANNdotNET.Excel.AddIn` must be installed. In order to install Excel AddIn the following action must be performed:
- Install Microsoft Excel 64 bit version. **The ANNdotNET Excel AddIn is not compatible with Microsoft Excel 32bit version**.
- Open Excel and select: `File -> Options`

![](./docs/images/anndotnet-exceladdin-img01.png)


- From the Option Dialog select: `Add-ins->ExcelAdd-ins -> press Go Button`,

![](./docs/images/anndotnet-exceladdin-img02.png)

- From file open dialog, select: `anndotnet.exceladdIn-AddIn64.xll` file which is located at the ANNdotNET binaries folder. 

![](./docs/images/anndotnet-exceladdin-img03.png)

- In order to register AddInAdd, ANNdotNET binaries folder must be registered in system environment path.

![](./docs/images/anndotnet-exceladdin-img04.png)

In case the PATH is not added the Excel addin must be installed every time you open Excel and use the Addin.

# Tutorial and Webcast
The following short videos quickly show how to create, train and evaluate regression, binary and multi class classification models.

* [ Regression model ](https://www.youtube.com/watch?v=usSav46d-e0&list=PLTk-S955yon0pRlfTwxtSBNX9eJULo7bu&index=1)
* [ Binary classification model ](https://www.youtube.com/watch?v=dU7vhbt61ck&list=PLTk-S955yon0pRlfTwxtSBNX9eJULo7bu&index=2)
* [ Multiclass classification model ](https://www.youtube.com/watch?v=2eCH0c0yyCs&list=PLTk-S955yon0pRlfTwxtSBNX9eJULo7bu&index=3)
* [ Export options in ANNdotNET ](https://www.youtube.com/watch?v=ZP0D6esyq78&list=PLTk-S955yon0pRlfTwxtSBNX9eJULo7bu&index=4)

More info at https://bhrnjica.net/anndotnet/
