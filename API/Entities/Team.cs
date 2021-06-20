using System;
using System.Collections.Generic;

namespace API.Entities
{
  public class Team
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Avatar { get; set; }
    public ICollection<AppUserTeam> AppUserTeams { get; set; }
  }
}