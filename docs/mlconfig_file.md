mlconfig file
=============

In this section it will be briefly described the structure of the mlconfig file,
and how to create it from scratch. The mlconfig file consist of 8 keywords:

1.  configid: - unique identifier of the mlconfig,

2.  metadata:- meta information about dataset.

3.  features: - defines features for the model

4.  labels: - defines labels for the model

5.  network: - defines neural network model to be trained

6.  learning: - defines learning parameters,

7.  training: - defines training parameters,

8.  path – defines paths to files needed during training and evaluation.

Each above keyword consists of several parameters and values. The syntax of the
mlconfig file allows you to create as many empty lines as you like. In case you
want to add comment in the file, the sentence must begin with exclamation ‘!’.
Order of the keywords is irrelevant.

For example, the following content represent typical mlconfig file:

>   !\*\*\*\*\*\*\*ANNdotNET v1.0\*\*\*\*\*\*\*\*\*\*\*\*

>   !Iris mlconfig file iris.mlconfig

>   !configid represent the unique identified of the configuration

>   **modelid**:33fe0968-d640-4b53-97dc-982dcf2b1cad

>   !metada contains information about data set.

>   **metadata**:\|Column01:sepal_length;Numeric;Feature;Ignore;
>   \|Column02:sepal_width;Numeric;Feature;Ignore;
>   \|Column03:petal_length;Numeric;Feature;Ignore;
>   \|Column04:petal_width;Numeric;Feature;Ignore;
>   \|Column05:species;Category;Label;Ignore;setosa;versicolor;virginica

>   !Information about features. The line contains

>   ! two groups of features: NumericFeatures and Product fetaure

>   **features**:\|NumFeatures 4 0 \|Product 10 0

>   !Information about label

>   **labels**:\|species 3 0

>   !Network configuration

>   **network**:\|Layer:Normalization 0 0 0 None 0 0 \|Layer:Dense 5 0 0 ReLU 0
>   0 \|Layer:Dense 3 0 0 Softmax 0 0

>   !Learning parameter information

>   learning:\|Type:SGDLearner \|LRate:0.01 \|Momentum:1
>   \|Loss:CrossEntropyWithSoftmax\|Eval:ClassificationAccuracy\|L1:0\|L2:0

>   !Training parameters information

>   **training**:\|Type:default \|BatchSize:65 \|Epochs:1000 \|Normalization:0
>   \|RandomizeBatch:False \|SaveWhileTraining:1 \|ProgressFrequency:50
>   \|ContinueTraining:0
>   \|TrainedModel:models\\model_at_952of1000_epochs_TimeSpan_636720117054117391

>   !Components of the mlconfig paths

>   **paths**:\|Training:data\\mldataset_train
>   \|Validation:data\\mldataset_valid \|Test:data\\mldataset_valid
>   \|TempModels:temp_models \|Models:models \|Result:FFModel_result.csv
>   \|Logs:log

configid
--------

this GUID value is generated automatically and supposed to be unique identifier
for the mlconfig file. When mlconfig file is created manualy, it can be any
string value.

metada:
-------

The metadata keyword contains meta information about dataset. The information is
arranged as list of columns, where each column contains: name, type, kind of
variable and missing value type.

>   metadata:\|Column01:[name] [type] [kind] [missingtype]  
>   \|Column02:[name] [type] [kind] [missingtype]  
>   ...

The number of columns must be the same as number of dimensions of features and
labels.

features: and labels:
---------------------

When describing features and labels in mlconfig file, the following signatures
should be applied:

>   features:\|[featurename1] [dimension] [isSparsedata]  
>   \|[featurename2] [dimension] [isSparsedata]  
>   ...

>   labels:\|[labelname1] [dimension] [isSparsedata]  
>   \|[labelname2] [dimension] [isSparsedata]  
>   ...

Each feature and label must be defined with 3 parameters:

-   name,

-   dimension,

-   sparsdata.

So, in case of iris dataset features are define:

>   features:\|irismeasures 4 0  
>   labels:\|species 3 0

It means the iris dataset has 4 features which is grouped in “irismeasures”
name, with 4 dimensions, which identifies 4 features and it is not sparse data.
The labels is defined as species with one-hot encoding vector of 3 classes

Assume we have an example for features and labels definition with the following
case:

>   features:\|year 3 1 \|month 12 1\|shop 52 1\|item 5100 1\|cnt_past3m 3 0

>   labels:\|item_cnt_month 1 1 0

As can be seen, in this example there are 5 different group of features. The
features are described in the following text:

1.  feature name = year, with dimension of 3, and it is sparse data (1).

2.  feature name = item, with 5100 dimensions, and this feature is sparse data
    (1).

3.  feature name= cnt_past3m, with 3 dimensions, and this data is not (0)
    sparse.

4.  label name= item_cnt_month, with 1 dimension, and it is not (0) sparse data.

Definition of features and labels are closely related on how mlready dataset is
generated. In case of the above definition one row for corresponded mlready
dataset is given as:

>   \|year 2:1 \|item 3906:1 \|cnt_past3m 0.02696543 0.02696543 0.02696543
>   \|item_cnt_month 0.02696543

We can see that the row defines with 3 groups of features and one group of
labels. Two features are category and one is numeric feature. Also label is of
numeric type, since it has only one dimension.

It is recommended to review other examples to see how the features and labels
defined.

network: 
=========

network: keyword defines the network model. It can be of type:

-   default – all network parameters are provided in the ml config file,

-   custom - network model are provided as C\# method in extension project.

In order to define custom network model, the following line must be defined:

network:\|Layer:Custom

So, when first layer is of Custom type, the API tries to call the delegate
method specified by the last argument of the MachineLearning.Run. In case the
custom model implementation is not provided, the exception is thrown by
specifying the exception message.

For default network type we can define various network models. The network
keyword has the following signature:

network:\|Layer1:[Type][HDimension] [CDimension] [DropOutPrecentige]
[Activation] [Peephole] [Stabilization] \|Layer2: :[Type][HDimension]
[CDimension] [DropOutPrecentige] [Activation] [Peephole] [Stabilization] …

As can be seen, network consist of layers. Each Layer has 7 parameters which may
be define. You can have as many layers as you like. This means you can make
network of arbitrary size.

Type of network parameters can have the following value:

-   Normalization – implements normalization layer,

-   Dense – implements dense layer

-   LSTM – implements Long Short-Term memory layer,

-   DropOut -implements dropout layer.

-   Embedding- implements Embedding layer.

Other layer parameters can be summarizing to:

1.  Type [layertype] - type layer name

2.  HDimension [number] – defines the dimension of layer output.

3.  CDimension[number]- defines the dimensions of LSTM cell (only for LSTM
    layer).

4.  Activation [name] – defines the activation function for the layer.

5.  DropOut [precentige]- defines drop values in percentage (only dropout
    layer).

6.  peephole[0/1]- is the LSTM layer with peephole (only for LSTM layer),

7.  stabilization[0/1]- is the LSTM layer with stabilization (only for LSTM
    layer).

Schematic picture of sample network model which is presented as list of layer is
shown below:

![](Images/32de751099d58f038ab281c3f8b29651.png)

The last layer in the sequence must be the output layer.

Example of bike sharing network model
-------------------------------------

The Bike Sharing example can be found and opened from the Start Page of
ANNdotNET GUI Tool. The graphical representation of the model is shown on the
image above. The following text describes the network model in the mlconfig
file:

>   network:\|Layer:Normalization 0 0 0 None 0 0 \|Layer:Embedding 10 0 0 None 0
>   0 \|Layer:LSTM 240 240 0 TanH 1 1 \|Layer:Drop 0 0 20 None 0 0 \|Layer:Dense
>   20 0 0 TanH 0 0 \|Layer:Dense 1 0 0 None 0 0

The model is defined as: EmbeddedLSTM network which has several different layers
in the network. Since we are dealing with numerical data, the first layer is
Normalization which normalizes the numerical features’ value. Only numerical
values are normalized with this layer, which means other categorical features
are remain the same. Then the Embedding layer is added in order to reduce the
features number, since we have 40 features. With embedding layer, we reduce 40
features to 10, and then we add LSTM layer with 240 output dimensions. After
LSTM we add some dropout and dense layers. Since the label layer has dimension
of 1, the last layer in the network must match the output layer dimension. The
last layer doesn’t have activation function, since we except the any value.

learning:
=========

The learning: keyword defines the learning parameter. The signature of the
keyword is the following:

>   learning:\|Type:[name] \|LRate:[value] \|Momentum:[value] \|Loss:[funname]
>   \|Eval:[funname]

The learning keyword has 5 parameters:

1.  Type – defines the learning type. Currently supported learners: (SGDLearner,
    MomentumSGDLearner, FSAdaGradLearner, AdaGradLearner, AdamLearner)

2.  LRate: - indicates the learning rate which represent the real value. Example
    (\|LRate:0.01)

3.  Momentum – defines momentum for the learner. Example (\|Momentum:0.9)

4.  Loss: - defines the loss function for the learner. Example
    (\|Loss:SquaredError).

5.  Eval: - defines the function during testing and evaluation for the model.

Currently supported loss and evaluation functions defined directly in CNTK
library:

1.  SquaredError -used for regression models,

2.  ClassificationError - for classification problems

3.  BinaryCrossEntropy - Computes the binary cross entropy (aka logistic loss)
    between the output and target,

4.  CrossEntropyWithSoftmax - computes the cross entropy between the
    target_vector and the softmax of the output_vector.

Several additional custom functions are implemented, and they can be used in
training models:

1.  RMSError - root mean square error,

2.  MSError - mean squared error.

Example of learning parameters 
-------------------------------

Example of learning parameters in ANNdotNET GUI tool for Bike Sharing looks
like:

![](Images/43a59f2ec771889ed5a5ad8e105a647c.png)

The same set of parameters in mlconfig file looks like:

>   learning:\|Type:AdaGradLearner \|LRate:0.01 \|Momentum:0.9
>   \|Loss:SquaredError\|Eval:SquaredError\|L1:0\|L2:0

The learner type is AdaGradLearner, with 0.01 of learning rate, and 0.9 of
momentum. The loss function is Squared error, and the evaluation function is
Squared error.

training:
=========

The training: keyword defines the training parameters. Typical training
parameters defined in mlconfig file are:

>   training: \|Type:[type] \|BatchSize:[number] \|Epochs:[number]
>   \|Normalization:[Feature1 Feature2 …] \|SaveWhileTraining:[0/1]
>   \|RandomizeBatch:[0/1] \|ProgressFrequency:[0/1] \|FullTraininfSetEval:[0/1]

The first parameter is the type which indicates should we used default, or
custom implemented minibatch source. Possible values (custom, default).

The BatchSize defines the size for the batch for the trainer. Possible values
are 1 to size of training data set. Example (\|BatchSize:125)

The Epoch: defines the number of cycles the trainer processes all samples from
the training dataset. Example (\|Epochs:12)

The Normalization: defines if the network model contains the Normalization layer
which will normalize the data. Normalization layer is described
[here](https://bhrnjica.net/2018/07/13/input-normalization-as-separate-layer-in-cntk-with-c/).

SaveWhileTraining: - indicates if the MLFactory will save models during training
process. This option should be used when we expect the model will be
overstrained during training.

RandmizeBatch: - indicates if the batch will be generated randomly during
training.

ProgressFrequency: - defines how progress will be sent to caller, in order to
report about training progress.

Simple example of training keyword which is defined in Solar production example:

training:\|Type:default \|BatchSize:500 \|Epochs:1000 \|Normalization:0
\|RandomizeBatch:False \|SaveWhileTraining:1 \|ProgressFrequency:2
\|ContinueTraining:0
\|TrainedModel:models\\model_at_81of1000_epochs_TimeSpan_636720261828457405

Training process is defined with default implemented minibatch source, with 500
size of batch which will not be randomized, with no normalization, with saving
models during training, and report progress every 2 epochs. The following image
shows the training parameters in GUI tool:

![](Images/f2e2ab5a082b6a823c1362ced8452332.png)

Normalization in ANNdotNET 
===========================

Simple said, data normalization is set of tasks which transform values of any
feature in a data set into predefined number range. Usually this range is [-1,1]
, [0,1] or some other specific ranges. Data normalization plays very important
role in ML, since it can dramatically improve the training process, and simplify
settings of network parameters.

There are two main types of data normalization:

-   MinMax normalization - which transforms all values into range of [0,1],

-   Gauss Normalization or Z score normalization, which transforms the value in
    such a way that the average value is zero, and standard deviation is 1.

Beside those types there are plenty of other methods which can be used. Usually
those two are used when the size of the data set is known, otherwise we should
use some of the other methods, like log scaling, dividing every value with some
constant, etc. But why data need to be normalized? This is essential question in
ML, and the simplest answer is to provide the equal influence to all features to
change the output label. More about data normalization and scaling can be found
on this
[link](https://www.coursera.org/lecture/data-genes-medicine/data-normalization-jGN7k).

![https://bhrnjica.files.wordpress.com/2018/07/normalization_layer.png?w=584&h=295](Images/2fe28a45f1725c6511d415357a10b594.png)

As can be observed, the Normalization layer is placed between input and first
hidden layer. Also the Normalization layer contains the same neurons as input
layer and produced the output with the same dimension as the input layer.

In order to implement Normalization layer, the following requirements must be
met:

-   calculate average 

    ![\\mu](Images/7d9f7758f3b32a50f4d2d70cfd07d1e3.png)

    and standard deviation

    ![\\sigma](Images/fe49bb139724fa9a78f59e55db89b557.png)

    in training data set as well find maximum and minimum value of each feature.

-   this must be done prior to neural network model creation, since we need
    those values in the normalization layer.

-   within network model creation, the normalization layer should be define
    after input layer is defined.

Calculation of mean and standard deviation for training data set
================================================================

Before network creation, we should prepare mean and standard deviation
parameters which will be used in the Normalization layer as constants.
Hopefully, the CNTK has the static method in the Minibatch source class for this
purpose “MinibatchSource.ComputeInputPerDimMeansAndInvStdDevs”. The method takes
the whole training data set defined in the minibatch and calculate the
parameters.

//calculate mean and std for the minibatchsource

// prepare the training data

var d = new DictionaryNDArrayView, NDArrayView\>\>();

using (var mbs = MinibatchSource.TextFormatMinibatchSource(

trainingDataPath , streamConfig, MinibatchSource.FullDataSweep,false))

{

d.Add(mbs.StreamInfo("feature"), new Tuple(null, null));

//compute mean and standard deviation of the population for inputs variables

MinibatchSource.ComputeInputPerDimMeansAndInvStdDevs(mbs, d, device);

 

}

Now that we have average and std values for each feature, we can create network
with normalization layer. ANNdotNET library supports multiple group of features.
For example, the input variables can consist of categorical and numerical
features. In case of categorical features, normalization should not be applied,
so normalization layer should be created only for numerical group of features.

The following example shows two groups of features Item and Sale.

>   \|Item 1 0 0 0 0 0 0 0 0 0 \|Sale 18 10 14 6 5 18 14 6 4 19 12 20 19 12 2 6
>   16 9 13 10 2 5 5 5 6 6 10 9 12 4 \|item_cnt 3

As can be seen we have Item which is typical categorical variable represented
with One-Hot encoding vector, and Sale features which is numerical group of
features.

The MLConfig file should be defined as:

>   features:\|Item 10 0 \|Sale 30 0

>   labels:\|item_cnt 1 0

Based on the above sample, we should define normalization only for Sale feature.
So, the training parameters may be defined as:

>   training: \|Type: default \|BatchSize: 480 \|Normalization:Sale
>   \|Epochs:1000 \|SaveWhileTraining: 1 \|RandomizeBatch: 1
>   \|ProgressFrequency: 1 \|FullTrainingSetEval:1

As can be seen Normalization contains only features group which is numerical.
For example in case there are two numerical features group Sales and Prices,
normalization would be defined as: \|Normalization:Sale;Prices.

Note: features group are separated with semicolon.

Paths in mlconfig 
==================

The last keyword is paths, which contains file paths for correctly working
mlconfig.

1.  Training: - path of the training dataset file

2.  Validation: - path of the validation dataset file¸

3.  Test: - path of the test dataset file used for evaluation

4.  Tempodels: - path where models are stred during training process

5.  Models: - path where sored the best model once the training process finished

6.  Result: - full path name where the result of the evaluation process stored.

7.  Logs – path of log folder. Location of log files.

The last 6 keywords are self-explained and example of using them are presented
in the case of solar production example:

>   paths:\|Training:data\\solar_cntk_train.ctf
>   \|Validation:data\\solar_cntk_val.ctf \|Test:data\\solar_cntk_test.ctf
>   \|TempModels:temp_models \|Models:models
>   \|Result:solar_production_result.csv \|Logs:log

As can be seen, paths define training, validation and testing data sets, storing
models during training and final stage. And the path where the result will be
stored when the model will be evaluated.

Other information about mlconfig file
=====================================

The mlconfig file can define comments, which are important during explanation
about some parameters and options. All examples provided in the MLFactory
solution are commented and each parameters is explained.

Beside comment very important information is separator. There are three kinds of
separators:

1.  ‘:’ -double point

2.  ‘\|’ -vertical line,

3.  ‘ ‘ - space,

4.  ‘;’ semicolon.

Double point separates the keyword and set of parameters, as well as parameters
names of their values.

Vertical line separates keyword parameters.

Space separates parameter values, and semicolon separated list of values for the
parameter.

So, let’s see the following example:

>   network:\|Layer:LSTM 240 240 0 TanH 1 1 \|Layer:Dense 20 0 0 TanH 0 0

Network keyword is separated by ‘:’ from its parameters (\|Layer…..). Then ‘\|’
separates different network layers (:\|Layer:LSTM 240 240 0 TanH 1 1, \|Layer:
Dense 20 0 0 TanH 0 0). Each network layer is separated by vertical line \|.
Each layer’s parameters are separated by space. In case the parameter has list
of values element list are separated by semicolon.

**Note: After double point space is not allow.**
