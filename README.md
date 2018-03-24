# ANNdotNET
![ANNdotNET Logo](https://github.com/bhrnjica/anndotnet/blob/master/ANNdotNET/ANNdotNET.Wnd.App/Images/annLogo_start2.png)

ANNdotNET is windows desktop application written in C# for creating and training ANN models. The application relies on Microsoft Cognitive Toolkit, CNTK, and it is supposed to be GUI tool for CNTK library with extensions in data preprocessing, model evaluation and exporting capabilities. It is hosted at http://github.com/bhrnjica/anndotnet

Currently supported Network Types of:
- Simple Feed Forward NN
- Deep Feed Forward NN
- Recurrent NN with LSTM

The process of creating, training, evaluating and exporting models is provided from the GUI Application and does not require knowledge for supported programming languages. The ANNdotNET is ideal for engineers which are not familiar with programming languages.

# Software Requirements
ANNdotNET is x64 Windows desktop application which is running on .NET Framework 4.7.1. In order to run the application, the following requirements must be met:

* Windows 7, 8 or 10 with x64 architecture
* .NET Framework 4.7.1
* CPU/GPU support. 

Note: The application automaticaly detect GPU capability on you machine and use it in training and evaluation, otherwize it will use CPU.

# How to run application
In order to run the application there are two possibilities:
1. Clone the GitHub repository of the application and open it in Visual Studio 2017. Change build architecture into x64, build and run the application.  
2. Download released version unzip and run ANNdotNET.exe.

The following three short videos quickly show how to create, train and evaluate reression, binary and multiclass classification models.

* [ Regression model ](https://raw.githubusercontent.com/bhrnjica/anndotnet/master/Tutorials/anndotnetv1.mp4)
* [ Binary classification model ](https://raw.githubusercontent.com/bhrnjica/anndotnet/master/Tutorials/anndotnetv2.mp4)
* [ Multiclass classification model ](https://raw.githubusercontent.com/bhrnjica/anndotnet/master/Tutorials/anndotnetv3.mp4)

More info at https://bhrnjica.net/anndotnet/
