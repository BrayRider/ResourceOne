using System;
using System.ServiceProcess;
using System.Threading;

namespace RSM.Service
{

    class RSMService : ServiceBase 
    {
        private Worker _worker;
        private Timer _timer;
        private TimerCallback _timerCallback;

        public RSMService()
        {
            this.ServiceName = "R1SM";
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;

        }

        protected override void OnStart(string[] args)
        {
            _worker = new Worker();

            _timerCallback = new TimerCallback(_worker.Run);
            _timer = new Timer(_timerCallback, null, 60000, 60000);



            base.OnStart(args);
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        public static void Main()
        {
            System.ServiceProcess.ServiceBase.Run(new RSMService());
        }

    }
}