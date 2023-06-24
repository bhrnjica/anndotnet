# ANNdotNET v2.0 alpha - deep learning on dotNET
[![license](https://img.shields.io/github/license/mashape/apistatus.svg?maxAge=2592000)](https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md)

![ANNdotNET Logo](./docs/images/annLogo_start2.png)

ANNdotNET is an open-source project written in C# that aims to simplify deep learning/machine learning tasks: 
- data preparation
- model creation, 
- visualy designing deep network, 
- training process, 
- model validation and testing, and
- deployment. 

For all above tasks AnndotNet provides a GUI tool that you can create very complex deep learning model in just a few clicks.
The whole model can be save in MLConfig json file.  

Unlike version v1 that is based on Microsoft CNTK, the AnndotNET v2 implements 
ML Engine based on Tensorflow, allowing users to perform deep learning operations 
with the cutting - edge DL framework.

## Features
Graphical user interface for deep learning tasks, eliminating the need for extensive programming knowledge.
Focus on network development and training process using a classic desktop approach rather than coding.
Reduced time spent on debugging source code, allowing more focus on exploring different configurations and parameter variants.
Ideal for engineers and users who are not familiar with programming languages.

- Graphical user interface for easy and intuitive deep learning model creation.
- Simplified setup of deep network architecture, trainers, optimizers, and data preprocessing.
- Ability to save the entire model configuration in an MLConfig JSON file for easy replication and sharing.
- Built on Tensorflow, providing users with a powerful and versatile deep learning framework.

## Documentation and Resources

The project is hosted on GitHub: http://github.com/bhrnjica/anndotnet
Comprehensive project documentation can be found in the project wiki.

## Pre-Calculated Projects

ANNdotNET includes dozens of pre-calculated projects that are available in the installer. These projects cover various problem domains, such as regression, binary and multi-class classification, image classification, time series analysis, and more. 
Each project provides examples of different neural network configurations, including feed-forward networks, deep neural networks, LSTM recurrent networks, embedding layers, and drop-out layers.

Users can open these pre-calculated projects from the Start page or via the CMD tool. 
They serve as starting points for experimentation and can be modified by adjusting the network configuration, learning parameters, and training settings. Additionally, users can create new machine learning configurations based on these projects.

# Getting Started

To get started with ANNdotNET, follow the installation instructions in the project wiki. The wiki also provides detailed usage guides, tutorials, and examples to help you make the most of ANNdotNET for your deep learning tasks.

We welcome contributions from the community to enhance and improve ANNdotNET. If you encounter any issues or have suggestions, please open an issue on the project's GitHub repository.





















ANNdotNET –  is an open source project for deep learning written in C#. The aim of the project is to create, train, validate, test and deploy deep learning models. 
One of the main project component is ANNdotNET ML Engine which is based on Tensorflow.Net. The project supposed to be GUI tool for deep learning with data preprocessing, model evaluation, exporting and deploying. 
 
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

