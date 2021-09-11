

namespace dbot.Models
{
    public class Vote
    {
        /* 
        WeeklyVotes Table will be purged each session. 
        Winning movie will be recorded in Session table and in MovieNominations table. 
        */
        public int ID {get;set;}
        public Nomination Nomination {get;set;}
        public User User {get;set;}

        public string Username => User.Username;

        public Vote() {}

        public Vote(Nomination nomination,User user) 
        {
            Nomination = nomination;
            User=user;
        }
    }
}