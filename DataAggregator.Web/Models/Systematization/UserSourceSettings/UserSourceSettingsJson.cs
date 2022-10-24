using System;
using System.Collections.Generic;
using System.Linq;
using DataAggregator.Domain.Model.DrugClassifier.Systematization;
using DataAggregator.Domain.Model.DrugClassifier.Systematization.View;

namespace DataAggregator.Web.Models.Systematization.UserSourceSettings
{
    public class UserSourceSettingsJson
    {
        public UserSourceSettingsJson()
        {            
        }
        
        public UserSourceSettingsJson(List<Source> sourcesDb, List<User> usersDb, List<UserSource> userSourcesDb, List<SourceStat> sourceStatsDb)
        {
            Periods = new List<PeriodJson>();

            foreach (var source in sourcesDb)
            {
                foreach (var period in source.Period)
                {
                    Periods.Add(new PeriodJson(period.Id, period.Name, period.Source.Id, period.Source.Name, sourceStatsDb.Where(ss => ss.PeriodId == period.Id).SingleOrDefault()));
                }
            }

            UserSources = new List<UserSourceJson>();

            foreach (var user in usersDb)
            {
                var usj = new UserSourceJson()
                {
                    UserId = new Guid(user.Id),
                    UserFullName = user.FullName,
                    DepartmentShortName = user.DepartmentShortName
                };

                var userSourceDb = userSourcesDb.SingleOrDefault(us => us.UserId == usj.UserId);
                usj.PeriodId = userSourceDb != null ? userSourceDb.PeriodId : 0;

                UserSources.Add(usj);
            }
        }

        public List<PeriodJson> Periods { get; set; } 

        public List<UserSourceJson> UserSources { get; set; } 
    }
}