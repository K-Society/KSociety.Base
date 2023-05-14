using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace KSociety.Base.InfraSub.Shared.Class
{
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

            _schedulerFactJob = new StdSchedulerFactory(properties);

            Name = schedulerName;
            Quartz.Logging.LogProvider.IsDisabled = true;
            switch (tType)
            {
                case TimeType.ms:
                    _timeInterval = TimeSpan.FromMilliseconds(interval);
                    break;
                case TimeType.s:
                    _timeInterval = TimeSpan.FromSeconds(interval);
                    break;
                case TimeType.min:
                    _timeInterval = TimeSpan.FromMinutes(interval);
                    break;
                case TimeType.h:
                    _timeInterval = TimeSpan.FromHours(interval);
                    break;
            }

            GetSchedulerJobAsync(_schedulerFactJob);
        }

        private async void GetSchedulerJobAsync(ISchedulerFactory isf)
        {
            Task<IScheduler> t = isf.GetScheduler();
            _schedulerJob = await t.ConfigureAwait(false);
        }

        public void Start<T>() where T : IJob
        {
            StartJob<T>();
        }

        public void Start<T>(string name, object jobData) where T : IJob
        {
            StartJob<T>(name, jobData);
        }

        public void Stop()
        {
            StopJob();
        }

        public void Pause()
        {
            _schedulerJob.PauseJob(_job.Key);
        }

        public void Resume()
        {
            _schedulerJob.ResumeJob(_job.Key);
        }

        private async void StartJob<T>() where T : IJob
        {
            _startTimeJob = new DateTimeOffset(2008, 1, 1, 0, 0, 0, new TimeSpan(0, 0, 0));

            await _schedulerJob.Start().ConfigureAwait(false);

            // define the job and tie it to our Job class
            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity("myJob_" + Name, "group1_" + Name)
                .Build();

            // Trigger the job to run now, and then every 5 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger_" + Name, "group1_" + Name)
                .StartAt(_startTimeJob)
                .WithSimpleSchedule(x => x
                    .WithInterval(_timeInterval)
                    .RepeatForever()
                    .WithMisfireHandlingInstructionNextWithRemainingCount()
                )
                .Build();

            await _schedulerJob.ScheduleJob(job, trigger).ConfigureAwait(false);
        }

        private async void StartJob<T>(string name, object jobData) where T : IJob
        {
            _startTimeJob = new DateTimeOffset(2008, 1, 1, 0, 0, 0, new TimeSpan(0, 0, 0));
            // construct a scheduler factory


            _schedulerJob.Context.Put(name, jobData);
            // get a scheduler
            await _schedulerJob.Start().ConfigureAwait(false);

            // define the job and tie it to our Job class
            /*IJobDetail*/
            _job = JobBuilder.Create<T>()
                .WithIdentity("myJob_" + Name, "group1_" + Name)
                .Build();

            // Trigger the job to run now, and then every 5 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger_" + Name, "group1_" + Name)
                .StartAt(_startTimeJob)
                .WithSimpleSchedule(x => x
                    .WithInterval(_timeInterval)
                    .RepeatForever()
                    .WithMisfireHandlingInstructionNextWithRemainingCount()
                )
                .Build();

            await _schedulerJob.ScheduleJob(_job, trigger).ConfigureAwait(false);
        }

        private async void StopJob()
        {
            await _schedulerJob.Standby().ConfigureAwait(false);
            await _schedulerJob.Shutdown(waitForJobsToComplete: true).ConfigureAwait(false);
        }
    }
}