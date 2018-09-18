using CNTK;
using NNetwork.Core.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anndotnet.unit
{
    public class BaseClass
    {
        protected DeviceDescriptor device = DeviceDescriptor.UseDefaultDevice();

        public Trainer createTrainer(Function network, Variable target, double lrvalue)
        {
            //learning rate
            // var lrate = 0.05;
            var momentum = 0.9;
            var lr = new TrainingParameterScheduleDouble(lrvalue);
            var mm = CNTKLib.MomentumAsTimeConstantSchedule(momentum);
            var l = new AdditionalLearningOptions() { l1RegularizationWeight = 0.001, l2RegularizationWeight = 0.1 };
            //network parameters
            var zParams = new ParameterVector(network.Parameters().ToList());

            //create loss and eval
            Function loss = CNTKLib.SquaredError(network, target);
            Function eval = StatMetrics.RMSError(network, target);

            //learners
            //
            var llr = new List<Learner>();
            var msgd = Learner.SGDLearner(network.Parameters(), lr,l);
            llr.Add(msgd);


            //trainer
            var trainer = Trainer.CreateTrainer(network, loss, eval, llr);
            //
            return trainer;
        }

    }
}
