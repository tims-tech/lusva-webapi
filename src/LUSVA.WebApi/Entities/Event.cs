using System;
using System.Collections.Generic;

namespace LUSVA.WebApi.Entities
{
  public class Event
  {
    public string EventId { get; set; }
    public DateTime? DateTime { get; set; }
    public Location Location { get; set; }
    public string Description { get; set; }
    public List<User> Participants { get; set; }
  }

  public class Location
  {
    public string LocationId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string StreetAddress { get; set; }

    public Event Event { get; set; }
  }
}