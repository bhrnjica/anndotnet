annproject file
===============

In this section it will be briefly described the structure of the annproject
file. The annproject file consists of 4 keywords:

1.  project: - contains information of the project

2.  data: - contains information about raw dataset.

3.  parser: - parser information while parsing rawdataset file.

project: - contains the following parameters:

-   Name: – name of the annproject,

-   ValidationSetCount: - the size of validation dataset,

-   PrecentigeSplit: - is the validation dataset size in percentage while
    creating it,

-   MLConfigs: - list of created ml configurations,

-   Info: - project info.

For example, the following text represent typical annproject :

>   !annprojct file for daily solar production

>   project:\|Name:SolarProduction \|ValidationSetCount:20 \|PrecentigeSplit:1
>   \|MLConfigs:LSTMMLConfig \|Info:

>   !raw dataset and metadata information

>   data:\|RawData:SolarProduction_rawdata.txt
>   \|Column01:time;Ignore;Ignore;Ignore;
>   \|Column02:solar.past;Numeric;Feature;Ignore;
>   \|Column03:solar.current;Numeric;Label;Ignore;

>   !parser information

>   parser:\|RowSeparator:rn \|ColumnSeparator: ; \|Header:0 \|SkipLines:0

The text above defined the project Solar production annproject, where raw
dataset stored in SolarProduction_rawdata.txt, and contains three columns: time,
solar.past and solar.total columns. The first column (time) is marked as ignored
which means it will be excluded from the creation of the ml ready dataset. The
solar.past column is marked as feature, and solar.current is marked as label.
Both feature and label are numeric column. Those information is enough that
ANNdotNET tool can created ml ready datasets.

The last parser keyword is used while the raw datset is loaded into the memory.

