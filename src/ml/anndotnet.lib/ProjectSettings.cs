//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////

using ANNdotNET.Core;

namespace ANNdotNET.Lib
{
    /// <summary>
    /// Class implements project settings, a basic information of the project which can be shared across models configuration 
    /// </summary>
    public class ProjectSettings
    {
        public static readonly string m_NumFeaturesGroupName = $"|{MLFactory.m_NumFeaturesGroupName} ";
        
        /// <summary>
        /// Folder path of the project. Since project and configuration models are based on configuration files
        /// only Project folder holds the full path information. All other components are placed relative to this path.
        /// </summary>
        public string ProjectFolder { get; set; }

        /// <summary>
        /// Amount of raw data set to buld Validation dataset
        /// </summary>
        public int ValidationSetCount { get; set; }
        public ProjectType ProjectType { get;  set; }

        /// <summary>
        /// Amount of raw data set to buld test dataset
        /// </summary>
        public int TestSetCount { get; set; }

        /// <summary>
        /// Validation dataset can be defined as percentage of all instances 
        /// of raw dataset or number of instances for validation 
        /// </summary>
        public bool PercentigeSplit { get; set; }

        /// <summary>
        /// The file name of the project.
        /// </summary>
        public string ProjectFile { get;  set; }
    }
}
