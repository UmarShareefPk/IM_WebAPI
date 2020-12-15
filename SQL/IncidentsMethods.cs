using IM.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace IM.SQL
{
    public class IncidentsMethods
    {

        public static DbResponse AddIncident(Incident incident)
        {
            var parameters = new SortedList<string, object>()
            {
                  { "CreatedBy" , incident.CreatedBy },
                  { "AssignedTo" , incident.AssignedTo },
                  { "Title" , incident.Title },
                  { "Description" , incident.Description },
                  { "AdditionalData" , incident.AdditionalData },
                  { "Attachment1" , incident.Attachment1 },
                  { "Attachment2" , incident.Attachment2 },
                  { "Attachment3" , incident.Attachment3 },
                  { "StartTime" , incident.StartTime },

            };
            return DataAccessMethods.ExecuteProcedure("AddNewIncident", parameters);
        }

        public static Incident GetIncidentrById(string incidentId)
        {
            var dt = new DataTable();
            var parameters = new SortedList<string, object>()
            {
                  { "Id" , incidentId},
            };

            var dbResponse = DataAccessMethods.ExecuteProcedure("GetIncidentById", parameters);
            var ds = dbResponse.Ds;

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            dt = ds.Tables[0];

            var incidents = (from rw in dt.AsEnumerable()
                         select new Incident()
                         {
                             Id = rw["Id"].ToString(),
                             CreatedBy = rw["CreatedBy"].ToString(),
                             AssignedTo = rw["AssignedTo"].ToString(),
                             CreatedAT = DateTime.Parse(rw["CreatedAT"].ToString()),
                             Title = rw["Title"].ToString(),
                             Description = rw["Description"].ToString(),
                             AdditionalData = rw["AdditionalData"].ToString(),
                             Attachment1 = rw["Attachment1"].ToString(),
                             Attachment2 = rw["Attachment2"].ToString(),
                             Attachment3 = rw["Attachment3"].ToString(),
                             StartTime = DateTime.Parse(rw["StartTime"].ToString()),

                         }).ToList();

            return incidents.First();
        }
    }// end class
}// end namespace