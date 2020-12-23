using IM.Models;
using IM.SQL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace IM.ApiControllers
{
    public class IncidentsController : ApiController
    {
        [HttpPost]
        public IHttpActionResult AddIncident()
        {            
            Incident incident = new Incident();
            incident.Title = HttpContext.Current.Request["Title"];
            incident.Description = HttpContext.Current.Request["Description"];
            incident.AdditionalData = HttpContext.Current.Request["AdditionalDeta"];
            incident.AssignedTo = HttpContext.Current.Request["AssignedTo"];
            incident.CreatedBy = HttpContext.Current.Request["CreatedBy"];
            incident.DueDate = DateTime.Parse(HttpContext.Current.Request["DueDate"]);
            incident.StartTime = DateTime.Parse(HttpContext.Current.Request["StartTime"]);
            incident.Status = HttpContext.Current.Request["Status"];

           // return null;

            DateTime dt = new DateTime();
            if (incident == null || string.IsNullOrWhiteSpace(incident.CreatedBy) || !DateTime.TryParse(incident.CreatedAT.ToString(), out dt)
                 || string.IsNullOrWhiteSpace(incident.AssignedTo) || string.IsNullOrWhiteSpace(incident.Title) 
                 || string.IsNullOrWhiteSpace(incident.Description) 
                 || !DateTime.TryParse(incident.StartTime.ToString(), out dt) || !DateTime.TryParse(incident.DueDate.ToString(), out dt)
                 || string.IsNullOrWhiteSpace(incident.Status)
                )
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Please enter all required fields and make sure datetime fields are in correct format."));
            }

            string[] statusValidValues = 
                                    { 
                                            "N", // New 
                                            "O", // Open
                                            "I", // In Progress
                                            "C", // Closed
                                            "A"  // Approved
                                    };

            if (!statusValidValues.Contains(incident.Status.ToUpper()))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Status Value. Valid values are N,O,I,C,A"));

            var dbResponse = IncidentsMethods.AddIncident(incident);
            
            if (dbResponse.Error)
            {
                if (dbResponse.ErrorMsg.Contains("FK_Incidents_CreatedBy"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "This Creator Id does not exist in or system."));
                else if (dbResponse.ErrorMsg.Contains("FK_Incidents_AssignedTo"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "This Assignee Id does not exist in or system."));
                else if (dbResponse.ErrorMsg.Contains("SqlDateTime overflow"))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Incorrect Datetime value."));

                else
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Internal Error. " + dbResponse.ErrorMsg));
            }

            string incident_Id = dbResponse.Ds.Tables[0].Rows[0][0].ToString();

            if (HttpContext.Current.Request.Files.Count > 0)
            {
                    foreach (var fileName in HttpContext.Current.Request.Files.AllKeys)
                    {
                        HttpPostedFile file = HttpContext.Current.Request.Files[fileName];
                        if (file != null)
                        {
                            var attachment = new IncidentAttachments();
                            attachment.FileName = file.FileName;
                            attachment.ContentType = file.ContentType;
                            attachment.IncidentId = incident_Id;
                           
                            //var FileUniqueName = Guid.NewGuid().ToString();
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Attachments/Incidents/") + incident_Id);
                            var rootPath = HttpContext.Current.Server.MapPath("~/Attachments/Incidents/" + incident_Id);
                            var fileSavePath = System.IO.Path.Combine(rootPath, file.FileName);
                            file.SaveAs(fileSavePath);
                            
                          IncidentsMethods.AddIncidentAttachments(attachment); 
                        }
                    }//end of foreach
              
            }//end of if count > 0

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "New incident has been added."));
        }

        [HttpGet]
        public Incident IncidentById(string Id)
        {
            return IncidentsMethods.GetIncidentrById(Id);
        }

        [HttpPost]
        public void UploadFile()
        {
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                try
                {
                    foreach (var fileName in HttpContext.Current.Request.Files.AllKeys)
                    {
                        HttpPostedFile file = HttpContext.Current.Request.Files[fileName];
                        if (file != null)
                        {
                           
                            var FileActualName = file.FileName;
                            var FileExt = Path.GetExtension(file.FileName);
                            var ContentType = file.ContentType;                          
                            var FileUniqueName = Guid.NewGuid().ToString();                           
                            var rootPath = HttpContext.Current.Server.MapPath("~/Attachments");
                            var fileSavePath = System.IO.Path.Combine(rootPath, FileUniqueName + FileExt);                          
                            file.SaveAs(fileSavePath);     
                        }
                    }//end of foreach
                }
                catch (Exception ex)
                { }
            }//end of if count > 0

            var age = HttpContext.Current.Request["Age"];
        }


        [HttpGet]
        public Object DownloadFile(string uniqueName)
        {
            string ContentType = "image/png";
            //Physical Path of Root Folder
            var rootPath = System.Web.HttpContext.Current.Server.MapPath("~/Attachments");        
            
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                var fileFullPath = System.IO.Path.Combine(rootPath, uniqueName + ".png");

                byte[] file = System.IO.File.ReadAllBytes(fileFullPath);
                System.IO.MemoryStream ms = new System.IO.MemoryStream(file);

                response.Content = new ByteArrayContent(file);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                //String mimeType = MimeType.GetMimeType(file); //You may do your hard coding here based on file extension

                response.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);// obj.DocumentName.Substring(obj.DocumentName.LastIndexOf(".") + 1, 3);
                response.Content.Headers.ContentDisposition.FileName = uniqueName + ".png";
                return response;         

        }

        // GET api/<controller>/5
        [HttpGet]
        public List<Incident> GetAllIncidents()
        {
            return IncidentsMethods.GetAllIncidents();
        }

       // [Authorize]
        [HttpGet]
        public IncidentsWithPage GetIncidentsWithPage(int PageSize, int PageNumber, string SortBy, string SortDirection, string Search)
        {
            return IncidentsMethods.GetIncidentsPage(PageSize, PageNumber, SortBy, SortDirection , Search);
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}