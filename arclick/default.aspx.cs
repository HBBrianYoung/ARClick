using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Globalization;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace arclick
{
    public static class PageDetails
    {
        public static int CommunityID { get; set; }
        public static string SectionID { get; set; }
        public static string DBUser { get; set; }
    }
    public static class UserDetails
    {
        public static string UserID { get; set; }
        public static string firstname { get; set; }
        public static string lastname { get; set; }
        public static string empnum { get; set; }
        public static string email { get; set; }
        public static string ext { get; set; }
        public static string CurrentPerson { get; set; }
        public static string interactionAccountName { get; set; }
        public static int jobclass { get; set; }
        public static string DeptID { get; set; }
        public static string location { get; set; }
        public static string mySections { get; set; }
        public static string UserLevel { get; set; }
        public static string myUserLevel { get; set; }
        public static string myAllowedSections { get; set; } = "";
        public static bool AllowAccess { get; set; }
        public static bool AllowSecurityPartner { get; set; }
    }

    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["windowsid"] != null)
            {
                UserDetails.UserID = Request.QueryString["windowsid"].Substring(Request.QueryString["windowsid"].LastIndexOf('\\') + 1);
                PageDetails.CommunityID = 0;
            }
            /*            else
                        {
                            string windowsId = ViewBag.windowsId;
                            UserDetails.UserID = windowsId.Replace(@"HAYNESBOONE\", "");
                            myscripts.InnerHtml = "";
                        }
                        */
            GetUserDetails(UserDetails.UserID);

            //if we update a note we need to put who the responsibile party is. In the DB we only track the last person to do an update not all the history. 
            NewNoteUpdater.Value = UserDetails.interactionAccountName;

            //output the three variables that are needed to run the queries for the arclick system

            loadDropDownLists();
        }

        protected void GetUserDetails(string UID)
        {
            //need to get what information about the logged in user from the database before doing all the below security checks. 
            string cstr = System.Configuration.ConfigurationManager.ConnectionStrings["dw_Timekeeper"].ConnectionString; ;
            using (SqlConnection connection = new SqlConnection(cstr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = System.Data.CommandType.Text;
                StringBuilder myQueryString = new StringBuilder("SELECT [tkFirst],[tklast],[Email],[tknum],[tkinit],[Namer],[UserId], [Extension], [office], [interactionAccountName] FROM [dw_HB1].[dbo].[dw_Timekeeper] WHERE status = 'active' AND UserId = @userid ");
                cmd.Parameters.AddWithValue("@userid", UserDetails.UserID);
                cmd.CommandText = myQueryString.ToString();
                connection.Open();

                SqlDataReader sdrTK = cmd.ExecuteReader();

                if (sdrTK.HasRows)
                {
                    while (sdrTK.Read())
                    {
                        {
                            UserDetails.firstname = sdrTK["tkFirst"].ToString();
                            UserDetails.empnum = sdrTK["tkinit"].ToString();
                            UserDetails.UserID = sdrTK["tkinit"].ToString();
                            UserDetails.lastname = sdrTK["tklast"].ToString();
                            UserDetails.email = sdrTK["Email"].ToString();
                            UserDetails.ext = sdrTK["Extension"].ToString();
                            UserDetails.location = sdrTK["office"].ToString();
                            UserDetails.interactionAccountName = sdrTK["interactionAccountName"].ToString();
                            UID = UserDetails.UserID;
                        }
                    }
                }
            }


            UserDetails.AllowSecurityPartner = false;

            switch (UserDetails.jobclass)
            {
                case 1001:
                    UserDetails.UserLevel = "B";
                    UserDetails.myUserLevel = "B";
                    UserDetails.AllowSecurityPartner = true;
                    UserDetails.AllowAccess = true;
                    break;
                case 1008:
                    UserDetails.UserLevel = "D";
                    UserDetails.myUserLevel = "D";
                    UserDetails.AllowSecurityPartner = true;
                    UserDetails.AllowAccess = true;

                    switch (UserDetails.mySections)
                    {

                        case "41":      //finance
                        case "43":      //Real Estate
                        case "46":      //Bankruptcy and Business Reorganization
                            UserDetails.myAllowedSections = "41,43,46";
                            break;
                        case "30":      //BT 30
                        case "31":      //BT 31
                        case "32":      //BT 32
                            UserDetails.myAllowedSections = "30,31,32";
                            break;
                        case "50":      //IP 50
                        case "51":      //IP 51
                            UserDetails.myAllowedSections = "50,51";
                            break;
                        case "70":      //Intellectual Property Litigation
                            UserDetails.myAllowedSections = "50,70";
                            break;
                        case "60":      //litigation 60
                        case "80":      //litigation 80
                        case "85":      //litigation 85
                            UserDetails.myAllowedSections = "60,80,85";
                            break;
                    }
                    break;
                case 1002:
                    UserDetails.UserLevel = "S";
                    UserDetails.myUserLevel = "S";
                    UserDetails.AllowSecurityPartner = true;
                    UserDetails.AllowAccess = true;

                    switch (UserDetails.mySections)
                    {
                        case "41":      //finance
                        case "43":      //Real Estate
                        case "46":      //Bankruptcy and Business Reorganization
                            UserDetails.myAllowedSections = "41,43,46";
                            break;
                        case "30":      //BT 30
                        case "31":      //BT 31
                        case "32":      //BT 32
                        case "33":      //BT 33
                        case "34":      //BT 34
                            UserDetails.myAllowedSections = "30,31,32,33,34";
                            break;
                        case "50":      //IP 50
                        case "51":      //IP 51
                            UserDetails.myAllowedSections = "50,51";
                            break;
                        case "70":      //Intellectual Property Litigation
                            UserDetails.myAllowedSections = "70";
                            break;
                        case "60":
                        case "80":      //Litigation 80
                        case "85":      //Litigation 85
                            UserDetails.myAllowedSections = "60,80,85";
                            break;
                    }
                    break;
                case 1004:
                    UserDetails.UserLevel = "P";       // Administrative Partner - can see anyone in same city, non-partners in others
                    UserDetails.myUserLevel = "P";
                    UserDetails.AllowSecurityPartner = false;
                    UserDetails.AllowAccess = false;

                    if ((UID == "82057") || (UID == "03903"))
                    {  //Special Office Admin Partners
                        UserDetails.UserLevel = "C";
                        UserDetails.myUserLevel = "C";
                        UserDetails.AllowAccess = true;
                    }
                    break;
                case 1005:
                    UserDetails.UserLevel = "S";       // Section Head / Practice Group Leader
                    UserDetails.myUserLevel = "S";
                    UserDetails.AllowSecurityPartner = false;
                    UserDetails.AllowAccess = false;
                    break;
                case 1006:
                    UserDetails.UserLevel = "P";       // Section Head / Practice Group Leader
                    UserDetails.myUserLevel = "P";
                    UserDetails.AllowSecurityPartner = false;
                    UserDetails.AllowAccess = false;
                    break;
                case 1010:
                    UserDetails.UserLevel = "P";       // Partner
                    UserDetails.myUserLevel = "P";
                    UserDetails.AllowSecurityPartner = false;
                    UserDetails.AllowAccess = false;
                    break;
                case 1100:
                    UserDetails.UserLevel = "P";       // Partner
                    UserDetails.myUserLevel = "P";
                    UserDetails.AllowSecurityPartner = false;
                    UserDetails.AllowAccess = false;
                    break;
                case 8010:
                    UserDetails.UserLevel = "SU-OA";
                    UserDetails.myUserLevel = "SU-OA";
                    UserDetails.AllowSecurityPartner = true;
                    UserDetails.AllowAccess = true;
                    break;
                default:   //failsafe
                    UserDetails.UserLevel = "";
                    UserDetails.myUserLevel = "";
                    UserDetails.AllowSecurityPartner = false;
                    UserDetails.AllowAccess = false;
                    break;
            }

            //community ID  - Are we going to have this type of thing?

            switch (PageDetails.CommunityID)
            {
                case 382:       // Partner Info
                                //sMySection$ID$ = gCurrentSection$ID$;
                                //gMySection$ID$ = gCurrentSection$ID$;
                    break;
                case 227:       // Attorney Profile
                    UserDetails.mySections = PageDetails.SectionID;

                    //if I am looking at an attorney profile and it is my own profile then make these settings
                    if (UID == UserDetails.UserID)
                    {
                        UserDetails.AllowSecurityPartner = true;
                        UserDetails.AllowAccess = true;
                    }
                    break;
                case 245:       // Staff Profile
                    UserDetails.mySections = PageDetails.SectionID;


                    if (UID == UserDetails.UserID)
                    {
                        UserDetails.AllowSecurityPartner = true;
                        UserDetails.AllowAccess = true;
                    }
                    break;

                case 0:     // My Pages
                    PageDetails.DBUser = UserDetails.UserID;
                    //gCurrentPerson$ID$ = gMyEmpID$ID$
                    UserDetails.AllowSecurityPartner = true;
                    UserDetails.AllowAccess = true;
                    UserDetails.myAllowedSections = PageDetails.SectionID;
                    UserDetails.mySections = "";
                    //sMySection$ID$ = gMySection$ID$;
                    break;

                default:        // Failsafe
                    PageDetails.DBUser = UserDetails.UserID;
                    //gCurrentPerson$ID$ = gMyEmpID$ID$
                    UserDetails.AllowSecurityPartner = false;
                    UserDetails.AllowAccess = false;
                    UserDetails.myAllowedSections = PageDetails.SectionID;
                    UserDetails.mySections = "";
                    //sMySection$ID$ = gMySection$ID$;
                    break;
            }




            if ((UID == "20765") || (UID == "00288") || (UID == "81031") || (UID == "82526") || (UID == "81465") || (UID == "81956") || (UID == "04828") || (UID == "00208") || (UID == "82753") || (UID == "20624") || (UID == "81122") || (UID == "82751") || (UID == "82676") || (UID == "08033"))
            {  // Executive Accounting
                UserDetails.UserLevel = "B";
                UserDetails.myUserLevel = "B";
                UserDetails.AllowSecurityPartner = true;
                UserDetails.AllowAccess = true;
                UserDetails.myAllowedSections = "";
            }
            else if ((UID == "04820") || (UID == "04827") || (UID == "x20439") || (UID == "83308"))
            {  // IT Devs
                UserDetails.UserLevel = "B";
                UserDetails.myUserLevel = "B";
                UserDetails.AllowSecurityPartner = true;
                UserDetails.AllowAccess = true;
                UserDetails.myAllowedSections = "";
                //gMyEmpID$ID$ = '08412';

            }
            else if ((UID == "81479") || (UID == "20794"))
            { // Super Users
                UserDetails.UserLevel = "SU-DH";
                UserDetails.myUserLevel = "SU-DH";
                UserDetails.AllowSecurityPartner = true;
                UserDetails.AllowAccess = true;
                UserDetails.myAllowedSections = "";
            }

            if (UserDetails.DeptID == "10")
            {
                UserDetails.UserLevel = "B";
                UserDetails.myUserLevel = "B";
                UserDetails.AllowAccess = true;
                UserDetails.myAllowedSections = "";
            }
            
            if (UID == "04820")
            {
                UserDetails.UserID = "80066";
            }
            if ((UID == "80148") || (UID == "80626") || (UID == "80802") || (UID == "81030") || (UID == "80029") || (UID == "83046") || (UID == "83396") || (UID == "83684"))
            { // Override - All Access
                UserDetails.UserLevel = "B";
                UserDetails.myUserLevel = "B";
                UserDetails.myAllowedSections = "";
                UserDetails.AllowAccess = true;
            }
            if ((UID == "80066"))
            {   // Override for Bruce Newsome 
                UserDetails.UserLevel = "D";       // Dept Head
                UserDetails.myUserLevel = "D";
                UserDetails.myAllowedSections = "30,31,32";
            }
            if ((UID == "08217"))
            {   // Override for Randall C. Brown 
                UserDetails.UserLevel = "D";       // Dept Head
                UserDetails.myUserLevel = "D";
                UserDetails.myAllowedSections = "50,51,70";
            }
            if (UID == "80626")
            {
                UserDetails.UserLevel = "S";   // Section Head level
                UserDetails.myUserLevel = "S";
                UserDetails.myAllowedSections = "";
                UserDetails.AllowAccess = true;
            }
            if (UID == "80626")
            {
                UserDetails.UserLevel = "S";   // Section Head level
                UserDetails.myUserLevel = "S";
                UserDetails.myAllowedSections = "";
                UserDetails.AllowAccess = true;
            }
            if ((UID == "04820") || (UID == "81030") || (UID == "83754") || (UID == "83308"))
            { // Override - All Access
                UserDetails.UserID = "83219";
                UserDetails.CurrentPerson = "83219";
                UserDetails.UserLevel = "B";
                UserDetails.myUserLevel = "B";
                UserDetails.myAllowedSections = "";
            }


            if(UID == "83754")
            {
                UserDetails.UserID = "83754";
                UserDetails.CurrentPerson = "83754";
                UserDetails.UserLevel = "D";       // Dept Head
                UserDetails.myUserLevel = "D";
                UserDetails.myAllowedSections = "50,51,70";
                UserDetails.AllowAccess = true;
            }


            behindvalues.Text = "MyUserLevel: " + UserDetails.myUserLevel.ToString();
            behindvalues.Text += ", myAllowedSections: " + UserDetails.myAllowedSections.ToString();
            behindvalues.Text += ", mySections:" + UserDetails.mySections.ToString();
            behindvalues.Text += ", UID: " + UID.ToString();
            behindvalues.Text += ", CurrentPerson: " + UserDetails.CurrentPerson.ToString();
            behindvalues.Text += ", Location: " + UserDetails.location.ToString();

            MyUserLevel.Value = UserDetails.myUserLevel.ToString();
            myAllowedSections.Value = UserDetails.myAllowedSections.ToString();
            mySections.Value = UserDetails.mySections.ToString();
            myUID.Value = UID.ToString();
            CurrentPerson.Value = UserDetails.CurrentPerson.ToString();
            location.Value = UserDetails.location.ToString();

            

        }
        protected void loadDropDownLists()
        {
            string con_timekeeper = System.Configuration.ConfigurationManager.ConnectionStrings["dw_Timekeeper"].ConnectionString;
            DataTable sections = new DataTable();

            using (SqlConnection connection = new SqlConnection(con_timekeeper))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT DISTINCT [Section] ,[SectionName] FROM[dw_HB1].[dbo].[dw_Timekeeper] ORDER BY sectionName", con_timekeeper);
                adapter.Fill(sections);

                drpSections.DataSource = sections;
                drpSections.DataTextField = "SectionName";
                drpSections.DataValueField = "section";
                drpSections.DataBind();
            }
            DataTable practicegroups = new DataTable();
            using (SqlConnection connection = new SqlConnection(con_timekeeper))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT DISTINCT [PracticeGroup] ,[PractGroupName] FROM[dw_HB1].[dbo].[dw_Timekeeper] ORDER BY PractGroupName", con_timekeeper);
                adapter.Fill(practicegroups);

                drpPracticeGroups.DataSource = practicegroups;
                drpPracticeGroups.DataTextField = "PractGroupName";
                drpPracticeGroups.DataValueField = "PracticeGroup";
                drpPracticeGroups.DataBind();
            }
            DataTable offices = new DataTable();
            using (SqlConnection connection = new SqlConnection(con_timekeeper))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT DISTINCT [office] ,[OfficeName] FROM[dw_HB1].[dbo].[dw_Timekeeper] ORDER BY OfficeName", con_timekeeper);
                adapter.Fill(offices);

                drpOffices.DataSource = offices;
                drpOffices.DataTextField = "OfficeName";
                drpOffices.DataValueField = "office";
                drpOffices.DataBind();
            }
        }
    }
}