using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IM.Models
{
    public class Incident
    {
        public string Id { get; set; }
        public string CreatedBy { get; set; }
        public string AssignedTo { get; set; }
        public DateTime CreatedAT { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AdditionalData { get; set; }
        public string Attachment1 { get; set; }
        public string Attachment2 { get; set; }
        public string Attachment3 { get; set; }
        public DateTime StartTime { get; set; }

    }
}