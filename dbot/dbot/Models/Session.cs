
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dbot.Models
{
    public class Session
    {
        [Key]
        public int ID {get;set;}
        public bool VoteOpen {get;set;}
        public DateTime Timestamp {get;set;} //sqllite does not have datetime
        public string WinningIMDBId {get;set;}
        public string WinningName {get;set;}
        public string WinningYear {get;set;}
        public string NominatedBy {get;set;}

        public Session(){}
    }
}