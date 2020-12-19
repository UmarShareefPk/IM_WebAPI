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
                  { "DueDate" , incident.DueDate },
                  { "Status" , incident.Status.ToUpper() },

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
                             DueDate = DateTime.Parse(rw["DueDate"].ToString()),
                             Status = rw["Status"].ToString()

                         }).ToList();

            return incidents.First();
        }

        public static List<Incident> GetAllIncidents()
        {
            var dt = new DataTable();
            var parameters = new SortedList<string, object>()
            {                 
            };

            var dbResponse = DataAccessMethods.ExecuteProcedure("GetAllIncidents", parameters);
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
                                 DueDate = DateTime.Parse(rw["DueDate"].ToString()),
                                 Status = rw["Status"].ToString()


                             }).ToList();

            return incidents;
        }

        public static IncidentsWithPage GetIncidentsPage(int pageSize , int pageNumber, string sortBy, string sortDirection, string Serach)
        {
            var dt = new DataTable();
            var parameters = new SortedList<string, object>()
            {
                 { "PageSize" , pageSize},
                 { "PageNumber" , pageNumber},
                 { "SortBy" , sortBy},
                 { "SortDirection" , sortDirection},
                 { "SearchText" , Serach},
            };

            var dbResponse = DataAccessMethods.ExecuteProcedure("GetIncidentsPage", parameters);
            var ds = dbResponse.Ds;

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            dt = ds.Tables[1];
            int total_incidents = int.Parse(ds.Tables[0].Rows[0][0].ToString());

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
                                 DueDate = DateTime.Parse(rw["DueDate"].ToString()),
                                 Status = rw["Status"].ToString()
                             }).ToList();

            return new IncidentsWithPage 
            { 
                Total_Incidents = total_incidents,
                Incidents = incidents
            };
        }

    }// end class

    public class IncidentsWithPage
    {
        public int Total_Incidents { get; set; }
        public List<Incident> Incidents { get; set; }
    }
}// end namespace