using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tech_Forum.Models
{
    public class QuestionBank
    {
        public int QuestionID;
        public string Question;
        public string[] Options;
        public string selectoptions;
        public string CorrectAnswer;
        public int DomainId;
        public int TechnologyId;
        public int count;
        public string tempstring;
        public string[] optionscount;
        public List<QuestionBank> selectedlist = new List<QuestionBank>();
        public int qno;
        public int score=0;
        public string domainlist;
        public string techlist;
        public int idtech;
        public string qid;
        public string domaintemp;

    }
}