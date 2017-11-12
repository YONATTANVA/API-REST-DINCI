using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiServicesDinci.Models
{
    public class Incident
    {
        
      public int IdIncident { get; set; }
      public string idIncidentType { get; set; }
      public string dateIncident { get; set; }
      public string ubicationIncident { get; set; }
      public string commetsIncident { get; set; }
      public int idStatus { get; set; }
      public int idCitizen { get; set; }
      public int idEmployee { get; set; }
        //description de id
      public string descriptionIncidentType { get; set; }
      public string descriptionStatus { get; set; }
      public string descriptionCitizen { get; set; }
      public string descriptionEmployee { get; set; }

    }
}