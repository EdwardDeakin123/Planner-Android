using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Front_End.Models;

namespace Front_End.Backend
{
    class BackendActivityLog : Backend
    {
        public BackendActivityLog() : base()
        {
            // Set the _Resource variable in the parent class. This defines which part of the API will be queried.
            _Resource = "ActivityLog";
        }

        public async Task<ActivityLogModel> Get(int activityLogId)
        {
            // Set the command and create the query string.
            _Command = "Get";

            _Parameters.Add(new BackendParameter { Key = "activityLogId", Value = activityLogId.ToString() });

            return await GetRequestAsync<ActivityLogModel>();
        }

        public async Task<List<ActivityLogModel>> GetByUser()
        {
            // Set the command and create the query string.
            _Command = "GetByUser";

            return await GetRequestListAsync<ActivityLogModel>();
        }

        public async Task<List<ActivityLogModel>> GetByDate(DateTime date)
        {
            // Set the command and create the query string.
            _Command = "GetByDate";

            _Parameters.Add(new BackendParameter { Key = "date", Value = date.ToString("yyyy-MM-dd HH:mm:ss") });

            return await GetRequestListAsync<ActivityLogModel>();
        }

        public async Task Add(int activityId, DateTime startTime, DateTime endTime)
        {
            _Command = "Add";

            // Set the parameters and post the Add request.
            _Parameters.Add(new BackendParameter { Key = "activityId", Value = activityId.ToString() });
            _Parameters.Add(new BackendParameter { Key = "startTime", Value = startTime.ToString("yyyy-MM-dd HH:mm:ss") });
            _Parameters.Add(new BackendParameter { Key = "endTime", Value = endTime.ToString("yyyy-MM-dd HH:mm:ss") });

            await PostRequestAsync();
        }

        public async Task Update(int activityLogId, DateTime startTime, DateTime endTime)
        {
            _Command = "Update";

            // Set the parameters and post the Add request.
            _Parameters.Add(new BackendParameter { Key = "activityLogId", Value = activityLogId.ToString() });
            _Parameters.Add(new BackendParameter { Key = "startTime", Value = startTime.ToString("yyyy-MM-dd HH:mm:ss") });
            _Parameters.Add(new BackendParameter { Key = "endTime", Value = endTime.ToString("yyyy-MM-dd HH:mm:ss") });

            await PutRequestAsync();
        }

        public async Task Delete(int activityLogId)
        {
            _Command = "Delete";

            // Set the parameters and post the Add request.
            _Parameters.Add(new BackendParameter { Key = "activityLogId", Value = activityLogId.ToString() });

            await DeleteRequestAsync();
        }
    }
}