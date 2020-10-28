using Anndotnet.Core.Entities;
using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;

namespace Anndotnet.Core.Interface
{
    public interface ITrainer
    {
        bool Run(Session session, LearningParameters lParams, TrainingParameters tParams, Func<Session, TrainingProgress, Session> processModel);
        // public bool RunOffline(Tensor x, Tensor y, Learner lr, TrainingParameters tr, Dictionary<string,string> modelPaths);
    }
}
