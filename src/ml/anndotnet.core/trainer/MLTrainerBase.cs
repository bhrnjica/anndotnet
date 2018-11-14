//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                                      //
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
using CNTK;
using NNetwork.Core;
using NNetwork.Core.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ANNdotNET.Core
{
    /// <summary>
    /// Base class implementation of the Trainer. Contains common function supposed to be used in all derived class
    /// </summary>
    public abstract class MLTrainerBase
    {
        #region Ctor and private members
        protected List<Variable> m_Inputs;
        protected List<Variable> m_Outputs;
        protected List<StreamConfiguration> m_StreamConfig;
        #endregion

        #region Properties

        public List<StreamConfiguration> StreamConfigurations
        {
            get
            {
                return m_StreamConfig;
            }
            set
            {
                m_StreamConfig = value;
            }
        }

        public List<Variable> InputVariables
        {
            get
            {
                return m_Inputs;
            }
            set
            {
                m_Inputs = value;
            }
        }

        public List<Variable> OutputVariables
        {
            get
            {
                return m_Outputs;
            }
            set
            {
                m_Outputs = value;
            }
        }
        #endregion

        #region Public Members
        public abstract TrainResult Train(Trainer trainer, Function network, TrainingParameters trParams,
                                MinibatchSourceEx miniBatchSource, DeviceDescriptor device, CancellationToken token, TrainingProgress progressstring, string modelCheckPoint, string historyPath);
        
        #endregion
    }
}
