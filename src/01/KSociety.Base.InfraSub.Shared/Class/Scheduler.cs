// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Class
{
    using System;
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

        #if NETSTANDARD2_0

        private IScheduler _schedulerJob = null;
        private IJobDetail _job;

        #elif NETSTANDARD2_1

        private IScheduler _schedulerJob = null;
        private IJobDetail _job;

        #endif

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
            var t = isf.GetScheduler();
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
            if (this._job is null)
            {
                return;
            }
            this._schedulerJob?.PauseJob(this._job.Key);
        }

        public void Resume()
        {
            if (this._job is null)
            {
                return;
            }
            this._schedulerJob?.ResumeJob(this._job.Key);
        }

        private async void StartJob<T>() where T : IJob
        {
            if (this._schedulerJob is null)
            {
                return;
            }

            this._startTimeJob = new DateTimeOffset(2008, 1, 1, 0, 0, 0, new TimeSpan(0, 0, 0));

            await this._schedulerJob.Start().ConfigureAwait(false);

            // define the job and tie it to our Job class
            var job = JobBuilder.Create<T>()
                .WithIdentity("myJob_" + this.Name, "group1_" + this.Name)
                .Build();

            // Trigger the job to run now, and then every 5 seconds
            var trigger = TriggerBuilder.Create()
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


            this._schedulerJob?.Context.Put(name, jobData);

            if (this._schedulerJob is null)
            {
                return;
            }
            // get a scheduler
            await this._schedulerJob.Start().ConfigureAwait(false);

            // define the job and tie it to our Job class
            /*IJobDetail*/
            this._job = JobBuilder.Create<T>()
                .WithIdentity("myJob_" + this.Name, "group1_" + this.Name)
                .Build();

            // Trigger the job to run now, and then every 5 seconds
            var trigger = TriggerBuilder.Create()
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
            if (this._schedulerJob is null)
            {
                return;
            }
            await this._schedulerJob.Standby().ConfigureAwait(false);
            await this._schedulerJob.Shutdown(waitForJobsToComplete: true).ConfigureAwait(false);
        }
    }
}
