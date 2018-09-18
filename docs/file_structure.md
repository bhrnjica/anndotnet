Files in ANNdotNET
==================

Introduction to mlconfig and annproject files
---------------------------------------------

The basic file of ANNdotNET is machine learning configuration file, shortly
named mlconfig. The mlconfig file holds the information about: features, labels,
learning and training parameters, neural network model and set of paths required
for running machine learning solution. Whenever an user presses Run button from
ANNdotNET GUI Tool or Run command/method from ANNdotNET CMD, the training
process start by loading the mlconfig file into application memory.

Furthermore, the ANNdotNET GUI Tool uses annproject file (\*.ann) which can hold
information of one or more mlconfig files. The user start creating new mlconfig
file first by pressing New button. Then the user must load the raw dataset. Once
the user finished with project creation and loading raw dataset in the project,
then the new mlconfig based on the project settings and raw dataset can be
created. The following image shows Breast Cancer annproject created based on the
BreatsCancer dataset. The project consists of two mlconfig files. Each ml config
file holds different ml configuration. This is very handy in case you want to
find best possible neural network model for different ml configuration. This is
natural way of doing data science.

![](Images/14ab6f259a54b6e22d50b2fcba593dc4.jpg)

On the other hand, ANNdotNET CMD is based on mlconifg file only. It can handle
only one filer per session. Any mlconfig file created with GUI tool can be run
with ANNdotNET CMD.

How to create mlconfig and annproject files
-------------------------------------------

There are two ways how to create mlconfig and annproject files:

1.  Using ANNdotNET GUI tool, or

2.  Manually by using any txt-based editor.

Press New button command in ANNdotNET GUI in order to start creating a new
project. Once the project is created, user must load raw dataset in order to
create ml config file.

To create mlconfig file manually, please see section about manualy create
mlconfig file.

File structure in ANNdotNET
---------------------------

Once the user presses the New button, the empty project is created. While
creating a new project, the project file and project folder are created on disk.
We can illustrate file and folder structure as follows:

Say we create a new project called “Project01”. The folder named “Projcet01” is
created, as well as project file named “Projct01.ann”. Those two items are shown
on the following image:

![](Images/7c1df576bf4ced8ef8bee8415aee3ebb.png)

Once the project (annproject) is created, we can load raw dataset file. The raw
dataset file (rawDataSet) is the file contains data of the problem we are going
to solve using the tool. The structure of the rawDataSet is classic table-based
textual data. For example we can load
<https://archive.ics.uci.edu/ml/machine-learning-databases/iris/iris.data> file
directly into ANNdotNET and start processing the data in order to implement ml
solution.

Once we load the rawDataSet, the project file structure now looks like on the
following image.

![](Images/ca93bf7412de5eeebd40d94ea561c057.png)

Notice, that rawDataSet file is created in the project folder and renamed
according to the ANNdotNET naming convention. So, loaded rawDataSet is renamed
to [ProjectName]_rawdata.txt

Now that we have annproject and rawDataSet, we can create ml configuration
(mlconfig) file. The mlconfig file is created when the user press Create
MLConfig button from the application’s ribbon control. Within the same project,
we can create as many ml configuration as we want, with different forms,
structure and size of training and validation dataset, also with different
network, learning and training parameters.

The following image shows the ANNdotNET project with created 4 ml
configurations: Model0, Model1, Model2 and Model3.

![ ](Images/fac7a4a653e1b72dd963eb087e03f5e3.png)

For each ml configuration separate folder is created, which offers clean and
easy way to follow file structures. Each ml configuration contains folders and
files arranged within the folder. The following image shows files and folder
structure of ANNdotNET ml configuration.

![](Images/2edfc159cd9c3a555178b966b46e9dbd.png)

Depending of stage of completeness, the ml configuration can consist of the
following folders:

-   data – contains training, validation and testing ml ready data set,

-   log – contains files of training information

-   models – files of cntk format created during various phase of training

-   temp_models – folder holding temporary model files during training. All
    content from the folder is deleted once the trainer is stopped.

Files which can be found at the ml configuration folder are:

-   mlconfig – ml configuration file, it contains the information for training,
    validation and evaluating model.

-   model checkpoint state files – files stored current state of the trainer.
    The files are needed in case when the user want to continue with training
    based on the previous trainer state.

The process of creating mlconfig file, starts by data transformation where the
rawdataset transforms into training and validation mlreadyDataset. The
mlreadyDataset is the data set which is created from the rawDataSet, with
format, features and label definition which the CNTK library can recognize.
