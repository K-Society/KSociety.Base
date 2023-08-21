namespace KSociety.Base.InfraSub.Shared.Class
{
    using System;
    using System.Threading.Tasks;
    using Quartz;
    using Quartz.Impl;

    /// <summary>
    /// Scheduler
    /// </summary>
    public class Scheduler
    {
        public enum TimeType { ms, s, min, h };

        private DateTimeOffset _startTimeJob;
        private readonly ISchedulerFactory _schedulerFactJob;
        private IScheduler _schedulerJob = null;
        private IJobDetail _job;
        private readonly TimeSpan _timeInterval;

        public string Name { get; set; }

        public Scheduler(string schedulerName, int interval, TimeType tType)
        {

            var properties = new System.Collections.Specialized.NameValueCollection
            {
                ["quartz.serializer.type"] = "binary"
            };

            this._schedulerFactJob = new StdSchedulerFactory(properties);

            this.Name = schedulerName;
            Quartz.Logging.LogProvider.IsDisabled = true;
            switch (tType)
            {
                case TimeType.ms:
                    this._timeInterval = TimeSpan.FromMilliseconds(interval);
                    break;
                case TimeType.s:
                    this._timeInterval = TimeSpan.FromSeconds(interval);
                    break;
                case TimeType.min:
                    this._timeInterval = TimeSpan.FromMinutes(interval);
                    break;
                case TimeType.h:
                    this._timeInterval = TimeSpan.FromHours(interval);
                    break;
            }

            this.GetSchedulerJobAsync(this._schedulerFactJob);
        }

        private async void GetSchedulerJobAsync(ISchedulerFactory isf)
        {
            Task<IScheduler> t = isf.GetScheduler();
            this._schedulerJob = await t.ConfigureAwait(false);
        }

        public void Start<T>() where T : IJob
        {
            this.StartJob<T>();
        }

        public void Start<T>(string name, object jobData) where T : IJob
        {
            this.StartJob<T>(name, jobData);
        }

        public void Stop()
        {
            this.StopJob();
        }

        public void Pause()
        {
            this._schedulerJob.PauseJob(this._job.Key);
        }

        public void Resume()
        {
            this._schedulerJob.ResumeJob(this._job.Key);
        }

        private async void StartJob<T>() where T : IJob
        {
            this._startTimeJob = new DateTimeOffset(2008, 1, 1, 0, 0, 0, new TimeSpan(0, 0, 0));

            await this._schedulerJob.Start().ConfigureAwait(false);

            // define the job and tie it to our Job class
            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity("myJob_" + this.Name, "group1_" + this.Name)
                .Build();

            // Trigger the job to run now, and then every 5 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger_" + this.Name, "group1_" + this.Name)
                .StartAt(this._startTimeJob)
                .WithSimpleSchedule(x => x
                    .WithInterval(this._timeInterval)
                    .RepeatForever()
                    .WithMisfireHandlingInstructionNextWithRemainingCount()
                )
                .Build();

            await this._schedulerJob.ScheduleJob(job, trigger).ConfigureAwait(false);
        }

        private async void StartJob<T>(string name, object jobData) where T : IJob
        {
            this._startTimeJob = new DateTimeOffset(2008, 1, 1, 0, 0, 0, new TimeSpan(0, 0, 0));
            // construct a scheduler factory


            this._schedulerJob.Context.Put(name, jobData);
            // get a scheduler
            await this._schedulerJob.Start().ConfigureAwait(false);

            // define the job and tie it to our Job class
            /*IJobDetail*/
            this._job = JobBuilder.Create<T>()
                .WithIdentity("myJob_" + this.Name, "group1_" + this.Name)
                .Build();

            // Trigger the job to run now, and then every 5 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger_" + this.Name, "group1_" + this.Name)
                .StartAt(this._startTimeJob)
                .WithSimpleSchedule(x => x
                    .WithInterval(this._timeInterval)
                    .RepeatForever()
                    .WithMisfireHandlingInstructionNextWithRemainingCount()
                )
                .Build();

            await this._schedulerJob.ScheduleJob(this._job, trigger).ConfigureAwait(false);
        }

        private async void StopJob()
        {
            await this._schedulerJob.Standby().ConfigureAwait(false);
            await this._schedulerJob.Shutdown(waitForJobsToComplete: true).ConfigureAwait(false);
        }
    }
}
