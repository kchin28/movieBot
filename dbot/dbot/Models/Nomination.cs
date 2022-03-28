  
  
namespace dbot.Models
{      
    public class Nomination 
    {
      /* Nominations Table will be purged each session. 
      Winning movie will be recorded in Session table and in MovieNominations table. 
        
      */
      public int ID { get; set; }
      public int VotingID { get; set; }  
      public string Name { get; set; }
      public string Year { get; set; }
      public string ImdbId { get; set; }
      public User User {get;set;}
    //  public string NominatedBy => User.Username;

      public Nomination(string name, int votingId, string imdbId,User user) 
      {
          Name = name;
          VotingID = votingId;
          ImdbId = imdbId;
          User = user;
      }

      public Nomination() { }

    }
}