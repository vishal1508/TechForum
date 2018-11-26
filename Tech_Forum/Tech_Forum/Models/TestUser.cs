using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tech_Forum.Models
{
    public class TestUser
    {
        public int count;
        public string domainsession;
        public string technologysession;
        public int[] TestId = new int[50];
        public string[] UserId = new string[50];
        public int[] Domain = new int[50];
        public string[] Domaindroplist = new string[50];
        public string[] Technologydroplist = new string[50];
        public string[] Domainlist = new string[50];
        public string[] Technologylist = new string[50];
        public int techcount;
        public byte[] QuestionsList;
        public byte[] OptionsList;
        public byte[] SelectedOptionsList;
        public byte[] CorrectAnswersList;
        public int[] Score = new int[50];
        public string Password;
        public string Name;
        public string Email;
        public string Phone;
        public int pass = 0;
        public int fail = 0;
        public int[] noofquest = new int[50];
        public string Description;
        public int Domaincount;
        public string[] Title = new string[10];
        public double[] Ratings = new double[10];
        public double averagerating;
        public string Tags;
        public int counttest;
        public int countarticle;


    }
}