ANNdotNET v1.2 - deep learning tool on .NET platform
============
This version brings several important updates and new features:

## The following enhancements and new features has been made in this release

- ### Image classification module
  - ``Image Classification`` project - This is huge step forward to this version. With IC project, the user can completely implements image classification with minimal steps without leaving GUI module.#39
  - ``ANNdotNET Feed`` - brings ability to share interesting ``annproject``'s to all community. Once the interesting project is added to ANNdotNET Feed it can be viewed by all uses that have installed ANNdotNET v1.2+ version.#33 
- ### Raw Data Handling Module
  - ``Time Series Generator`` - previously time series could be loaded in ``Data Import`` dialog only with one column data without header. Now, more than one column with header can be imported, and only the last one will be generated as time series, while the rest columns will remain as are.#34 #35
  - Split Raw Data Set to: ``train``, ``validation`` and ``test`` sets. Up to now the user could split raw data set on train and validation sets only.#22   
  - Export to Excel with all three data sets.#23
  - Optimization data loading and handling with huge data set.#21
- ### Neural Network Designer improvements

  - Add new Layer types: Convolution, Pooling, etc. #18
  - ``Inser`` button Insert Layer in the network at specific position.#36

### Bug Fixes
- Some WinForms Dialogs has been converted into WPF based windows in order to fix WInForm DPI issue on scalled monitors. #42
- Minor code improvements and bug fixes



ANNdotNET v1.1 - deep learning tool on .NET platform
============
This version brings upgrade of Machine Learning Engine and set of minor bug fixes identified in the application.

## The following enhancements has been made in this release
- Visualization of network configuration using Graphviz tool #32 
- The ANNdotNET MLEngine now relies on CNTK 2.6. #28 
- Information about data sets has been added to Network Page #30 
- Chart controls on Training and Evaluation pages are simplified and improved visibility. #29 
- Refresh button has been removed and added automatic model evaluation. #31 
- Wiki pages are updated with the latest version.#40

## Bug Fixes
- Test Tab Page had bug which add new rows whenever the user press Evaluate button.   
- Minor bug fixes and code improvements.#38
ANNdotNET v1.1-rc20181115 - deep learning tool on .NET platform
============
This version brings upgrade of Machine Learning Engine and set of minor bug fixes identified in the application.
## The following enhancements has been made in this release
- The ANNdotNET MLEngine now relies on CNTK 2.6. 
- Information about datasets has been added to Network Page
- Chart controls on Training and Evaluation pages are simplified and improved visibility.
- Refresh button has been removed and added automatic model evaluation.

## Bug Fixes
- Test Tab Page had bug which add new rows whenever the user press Evaluate button. 
-  
ANNdotNET v1.1-rc20181115 - deep learning tool on .NET platform
============
This version brings upgrade of Machine Learning Engine and set of minor bug fixes identified in the application.
## The following enhancements has been made in this release
- The ANNdotNET MLEngine now relies on CNTK 2.6. 
- Information about datasets has been added to Network Page
- Chart controls on Training and Evaluation pages are simplified and improved visibility.
- Refresh button has been removed and added automatic model evaluation.

## Bug Fixes
- Test Tab Page had bug which add new rows whenever the user press Evaluate button. 
-  

ANNdotNET v1.0 - deep learning tool on .NET platform
============

ANNdotNET – is an open source project for deep learning on .NET Platform. This is complete GUI solution for data preparation, training, evaluation and deployment ml models. ANNdotNET introduces the ANNdotNET Machine Learning Engine ( `MLEngine `) which is responsible for training and evaluation models defined in the mlconfig files. The `MLEngine` relies on Microsoft Cognitive Toolkit, CNTK open source library which is proved to be one of the best open source library for deep learning. Through all application's components `MLEngine` exposed all great features of the CNTK e.g. GPU support for training and evaluation, different kind of learners. `MLEngine ` also extends CNTK features with more evaluation functions (RMSE, MSE, Classification Accuracy, Coefficient of Determination, etc.), Extended Mini-batch Sources, Trainer and Evaluation models.
The process of creating, training, evaluating and exporting models is provided from the GUI Application and does not require knowledge for supported programming languages. 

The ANNdotNET is ideal in several scenarios:

- more focus on network development and training process using classic desktop approach, instead of focusing on coding, 
- less time spending on debugging source code, more focusing on different configuration and parameter variants,
- ideal for engineers/users who are not familiar with programming languages, 
- in case the problem requires coding custom models, or training process, ANNdotNET CMD provides high level of API for such implementation,
- all ml configurations developed with GUI tool,can be handled with CMD tool and vice versa.  

### How to run ANNdotNET GUI Tool from release section

This option is handy in case you don't have installed Visual Studio or you want to use the application without source code. 
The following actions should be performed:

-   Download binaries from the release section at:
     `https://github.com/bhrnjica/anndotnet/releases`,

-   Unzip the binaries on your machine and run `anndotnet.wnd.exe` exe file.

-  For quick start, while the application is running, select one of many pre-calculated annprojects
    placed on Start Page.

# Software Requirements
ANNdotNET is x64 Windows desktop application running on .NET Framework 4.7.2. and .NET Core 2.0. In order to run the application, the following software components need to be installed:

* Windows 10 with x64 architecture
* .NET Framework 4.7.2 +
* [Visual C++ 2017 version 15.4 v14.11 toolset](https://aka.ms/vs/15/release/vc_redist.x64.exe)
* [Visual C++ Redistributable Packages for Visual Studio 2013](https://www.microsoft.com/en-us/download/details.aspx?id=40784) 

Note: The application is tested on clean *Windows Pro 10 1709 build*. Probably the application will run on Windows 8 and Windows 7 as well, but those systems are not tested.

The project is hosted at <http://github.com/bhrnjica/anndotnet>. For more information about the project please see ANNdotNET wiki page. 


# ANNdotNET v1.0-rc20181009
ANNdotNET v1.0 - deep learning tool on .NET platform
============

ANNdotNET – is an open source project for deep learning on .NET Platform. This is complete GUI solution for data preparation, training, elvaluation and deployment ml models. ANNdotNET introduces the ANNdotNET Machine Learning Engine ( `MLEngine `) which is responsible for training and evaluation models defined in the mlconfig files. The `MLEngine ` relies on Microsoft Cognitive Toolkit, CNTK open source library which is proved to be one of the best open source library for deep learning. Through all application's components `MLEngine ` exposed all great features of the CNTK e.g. GPU support for training and evaluation, different kind of learners. `MLEngine ` also extends CNTK features with more evaluation functions (RMSE, MSE, Classification Accuracy, Coefficient of Determination, etc.), Extended Mini-batch Sources, Trainer and Evaluaton models.

- For software requirements please see previous release note.
- For instruction how to start the application please see previous release note.

## The following enhancements has been made in this release

- Export improvements for different label type (numeric, classification type)
- Export and result display for different localized PCs
- Pre-calculated `annproject`s improvements


## Bug Fixes

- Empty output classes bug fix 
- Column separator bug fix when importing `rawdataset`
- Added missing files from pre-calculated `annproject`s
- Export regression model bug fix


# ANNdotNET v1.0-rc20180929

ANNdotNET v1.0 - deep learning tool on .NET platform
============

ANNdotNET – is an open source project for deep learning on .NET Platform. This is complete GUI solution for data preparation, training, elvaluation and deployment ml models. ANNdotNET introduces the ANNdotNET Machine Learning Engine ( `MLEngine `) which is responsible for training and evaluation models defined in the mlconfig files. The `MLEngine ` relies on Microsoft Cognitive Toolkit, CNTK open source library which is proved to be one of the best open source library for deep learning. Through all application's components `MLEngine ` exposed all great features of the CNTK e.g. GPU support for training and evaluation, different kind of learners. `MLEngine ` also extends CNTK features with more evaluation functions (RMSE, MSE, Classification Accuracy, Coefficient of Determination, etc.), Extended Mini-batch Sources, Trainer and Evaluaton models.

- For software requiremens please see previous release note.
- For instruction how to start the application please see previous release note.

This release tends to stabilize the code and inlucdes some unfinished features from the previous release. This release also is features complete, and no additional features is planned to be implemented until the first version release. Probably this is the latest pre-release verion before stable release which will be released soon.


# ANNdotNET v1.0-rc20180920

ANNdotNET v1.0 - deep learning tool on .NET platform
============

ANNdotNET – is an open source project for deep learning written in C# for
creating and training deep learning models. The application relies on Microsoft
Cognitive Toolkit, CNTK, and it is supposed to be GUI tool for CNTK library with
extensions in data preprocessing, model evaluation, exporting and deploying deep
learning models. It is hosted at <http://github.com/bhrnjica/anndotnet>.


### How to run ANNdotNET GUI Tool from release section

This option is handy in case you don't have installed Visual Studio or you want to use the application without source code. 
The following actions should be performed:

-   Download binaries from the release section at:
     `https://github.com/bhrnjica/anndotnet/releases`,

-   Unzip the binaries on your machine and run `anndotnet.wnd.exe` exe file.

-  For quick start, while the application is running, select one of many pre-calculated annprojects
    placed on Start Page.

# Software Requirements
ANNdotNET is x64 Windows desktop application running on .NET Framework 4.7.2. and .NET Core 2.0. In order to run the application, the following software components need to be installed:

* Windows 10 with x64 architecture
* .NET Framework 4.7.2 +
* [Visual C++ 2017 version 15.4 v14.11 toolset](https://aka.ms/vs/15/release/vc_redist.x64.exe)
* [Visual C++ Redistributable Packages for Visual Studio 2013](https://www.microsoft.com/en-us/download/details.aspx?id=40784) 

Note: The application is tested on clean *Windows Pro 10 1709 build*. Probably the application will run on Windows 8 and Windows 7 as well, but those systems are not tested.

