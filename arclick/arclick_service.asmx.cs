using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Script.Serialization;

namespace arclick
{

    public class noterec
    {
        public string clientid { get; set; }
        public string lastupdatedby { get; set; }
        public string note { get; set; }
        public string currency { get; set; }
        public string LastUpdatedDate { get; set; }

    }

    public class detailrowrec
    {
        public string matter { get; set; }
        public string MatterName { get; set; }
        public string Invoice { get; set; }
        public string Currency { get; set; }
        public string ARAmt { get; set; }
        public string InvoiceDate { get; set; }
        public string numberofdays { get; set; }
        public string VatFlag { get; set; }

    }

    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class arclick_service : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetItems()
        {
            int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
            int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
            int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
            string rawSearch = HttpContext.Current.Request.Params["sSearch"];

            string fnServerParams = HttpContext.Current.Request.Params["fnServerParams"];


            //values determined on the front end and passed to here to build the query with the correct security measures in place. 
            string MyUserLevel = HttpContext.Current.Request.Params["myUserLevel"];
            string myAllowedSections = HttpContext.Current.Request.Params["myAllowedSections"];
            string mySections = HttpContext.Current.Request.Params["mySections"];
            string UID = HttpContext.Current.Request.Params["UID"];
            string CurrentPerson = HttpContext.Current.Request.Params["CurrentPerson"];
            string location = HttpContext.Current.Request.Params["location"];
            //string participant = HttpContext.Current.Request.Params["iParticipant"];

            var sb = new StringBuilder();

            var filteredWhere = "";

            //var wrappedSearch = "'%" + rawSearch + "%'";

            //      if (rawSearch.Length > 0)
            //     {

            //filteredWhere = "WHERE section = 50 ";

            switch (MyUserLevel)
            {
                //case "B":   NOTHING HAPPENS HERE set fixedfilterPart to empty
                //    break;
                case "D":
                case "S":
                    if (myAllowedSections.Length <= 1)
                    {
                        //fixedFilterPart = "Section = '" + mySections.ToString() +"'";
                        sb.Append(" WHERE Section = '" + mySections.ToString() + "'");
                    }
                    else
                    {
                        //fixedFilterPart += " AND Section IN (" + gMyAllowedSections$ID$ +")";
                        sb.Append(" WHERE Section IN (" + myAllowedSections.ToString() + ")");
                    }
                    break;

                case "C": //City
                    sb.Append(" WHERE Office = '" + location.ToString() + "'");
                    break;
                default:
                    break;

            }
            filteredWhere = sb.ToString();

            /*
            switch (gMyUserLevel$ID$) {
                case 'B':	// Board
                    fixedFilterPart = '';
                break;

                case 'D':	// Department Head
                    if (gMyAllowedSections$ID$.length <= 1) {
                    fixedFilterPart = "Section = '" + gMySection$ID$ +"'";
                }
                    else {
                    fixedFilterPart += " AND Section IN (" + gMyAllowedSections$ID$ +")";
                }
                break;

                case 'S':	// Section
                    if (gMyAllowedSections$ID$.length <= 1) {
                    fixedFilterPart = "Section = '" + gMySection$ID$ +"'";
                }
                    else {
                    fixedFilterPart += " AND Section IN (" + gMyAllowedSections$ID$ +")";
                }
                break;

                case 'C':	// City
                    fixedFilterPart = "Office = " + parseInt(gMyLocation$ID$);
                break;

                default: 						// Failsafe
                    fixedFilterPart = "SupervisingAttorneyId = '" + gCurrentPerson$ID$ +"'";
                break;
                */
            /* sb.Append(" WHERE ClientId LIKE ");
             sb.Append(wrappedSearch);
             sb.Append(" OR ClientName LIKE ");
             sb.Append(wrappedSearch);
             filteredWhere = sb.ToString();*/
            // }


            //ORDERING

            sb.Clear();

            string orderByClause = string.Empty;
            sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));

            sb.Append(" ");

            sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);

            orderByClause = sb.ToString();

            if (!String.IsNullOrEmpty(orderByClause))
            {

                orderByClause = orderByClause.Replace("0", ", ClientId ");
                orderByClause = orderByClause.Replace("1", ", ClientName ");


                orderByClause = orderByClause.Remove(0, 1);
            }
            else
            {
                orderByClause = "ClientId ASC";
            }
            orderByClause = "ORDER BY " + orderByClause;

            sb.Clear();

            var numberOfRowsToReturn = "";
            numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

            string query = @"SELECT *
                            FROM
	                            (SELECT row_number() OVER ({0}) AS RowNumber, *
	                             FROM
		                             (SELECT (SELECT count(InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency.ClientId)
				                              FROM
					                             InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency) AS TotalRows
			                               , ( SELECT  count(InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency.ClientId) 
                                                FROM InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency {1}) AS TotalDisplayRows			   
			                                ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].ClientId
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].ClientName
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].SupervisingAttorneyName
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].SupervisingAttorneyId
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].Currency
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].ARunder30
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].AR31to60
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].AR61to90
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].AR91to120
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].AR121to180
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].ARover180
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].TotalAR
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].Unallocated
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].GBNF
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].TrustBalance
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].Note
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].WIPunder60
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].WIPover60
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].TotalWIP
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].TotalICS
                                            ,[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency].Section
                                        FROM
			                              InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency {1}) RawResults) Results
                            WHERE
	                            RowNumber BETWEEN {2} AND {3}";


            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn);

            var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);

            //try
            //{
            conn.Open();
            //}
            //catch(Exception e )
            //{
            //   Console.WriteLine(e.ToString());
            //}

            var DB = new SqlCommand();
            DB.Connection = conn;
            DB.CommandText = query;
            var data = DB.ExecuteReader();

            var totalDisplayRecords = "";
            var totalRecords = "";
            string outputJson = string.Empty;

            var rowClass = "";
            var count = 0;

            while (data.Read())
            {

                if (totalRecords.Length == 0)
                {
                    totalRecords = data["TotalRows"].ToString();
                    totalDisplayRecords = data["TotalDisplayRows"].ToString();
                }
                sb.Append("{");
                sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
                sb.Append(",");
                sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                sb.Append(",");
                sb.AppendFormat(@"""0"": ""{0}""", data["ClientId"]);
                sb.Append(",");
                sb.AppendFormat(@"""1"": ""{0}""", data["ClientName"]);
                sb.Append(",");
                sb.AppendFormat(@"""2"": ""{0}""", data["SupervisingAttorneyName"]);
                sb.Append(",");
                sb.AppendFormat(@"""3"": ""{0}""", data["Currency"]);
                sb.Append(",");
                sb.AppendFormat(@"""4"": ""{0}""", data["ARunder30"]);
                sb.Append(",");
                sb.AppendFormat(@"""5"": ""{0}""", data["AR31to60"]);
                sb.Append(",");
                sb.AppendFormat(@"""6"": ""{0}""", data["AR61to90"]);
                sb.Append(",");
                sb.AppendFormat(@"""7"": ""{0}""", data["AR91to120"]);
                sb.Append(",");
                sb.AppendFormat(@"""8"": ""{0}""", data["AR121to180"]);
                sb.Append(",");
                sb.AppendFormat(@"""9"": ""{0}""", data["ARover180"]);
                sb.Append(",");
                sb.AppendFormat(@"""10"": ""{0}""", data["TotalAR"]);
                sb.Append(",");
                sb.AppendFormat(@"""11"": ""{0}""", data["Unallocated"]);
                sb.Append(",");
                sb.AppendFormat(@"""12"": ""{0}""", data["GBNF"]);
                sb.Append(",");
                sb.AppendFormat(@"""13"": ""{0}""", data["TrustBalance"]);
                sb.Append(",");
                sb.AppendFormat(@"""14"": ""{0}""", HttpUtility.HtmlEncode(data["Note"]));
                sb.Append(",");
                sb.AppendFormat(@"""15"": ""{0}""", data["WIPunder60"]);
                sb.Append(",");
                sb.AppendFormat(@"""16"": ""{0}""", data["WIPover60"]);
                sb.Append(",");
                sb.AppendFormat(@"""17"": ""{0}""", data["TotalWIP"]);
                sb.Append(",");
                sb.AppendFormat(@"""18"": ""{0}""", data["TotalICS"]);
                sb.Append(",");
                sb.AppendFormat(@"""19"": ""{0}""", data["Section"]);
                sb.Append(",");
                sb.AppendFormat(@"""20"": ""{0}""", data["SupervisingAttorneyId"]);
                sb.Append("},");

            }

            // handles zero records
            if (totalRecords.Length == 0)
            {
                sb.Append("{");
                sb.Append(@"""sEcho"": ");
                sb.AppendFormat(@"""{0}""", sEcho);
                sb.Append(",");
                sb.Append(@"""iTotalRecords"": 0");
                sb.Append(",");
                sb.Append(@"""iTotalDisplayRecords"": 0");
                sb.Append(", ");
                sb.Append(@"""aaData"": [ ");
                sb.Append("]}");
                outputJson = sb.ToString();

                return outputJson;
            }
            outputJson = sb.Remove(sb.Length - 1, 1).ToString();
            sb.Clear();

            sb.Append("{");
            sb.Append(@"""sEcho"": ");
            sb.AppendFormat(@"""{0}""", sEcho);
            sb.Append(",");
            sb.Append(@"""iTotalRecords"": ");
            sb.Append(totalRecords);
            sb.Append(",");
            sb.Append(@"""iTotalDisplayRecords"": ");
            sb.Append(totalDisplayRecords);
            sb.Append(", ");
            sb.Append(@"""aaData"": [ ");
            sb.Append(outputJson);
            sb.Append("]}");
            outputJson = sb.ToString();

            return outputJson;
        }


        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = true, ResponseFormat = System.Web.Script.Services.ResponseFormat.Xml)]
        public List<noterec> GetNoteDetails(string inClientID, string inSupervisingAttorney, string inCurrency)
        {
            //noterec newnoterec = new noterec();
            List<noterec> newnoterec = new List<noterec>();

            /*
             * string inClientID = HttpContext.Current.Request.Params["clientid"].ToString();
             string inSupervisingAttorney = HttpContext.Current.Request.Params["supervisingattorney"].ToString();
             string inCurrency = HttpContext.Current.Request.Params["currency"].ToString();
             */
            string strQuery = "";

            var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);

            conn.Open();
            strQuery = "SELECT clientid, LastUpdatedBy, LastUpdatedDate, note, currency FROM [EliteService].[dbo].[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency] WHERE ";
            strQuery += " ClientID = @inClientID";
            if (inSupervisingAttorney != "")
            {
                strQuery += " AND SupervisingAttorneyName = @inSupervisingAttorney";
            }
            strQuery += " AND Currency = @inCurrency";


            var DB = new SqlCommand();

            DB.Parameters.AddWithValue("@inClientID", inClientID);
            DB.Parameters.AddWithValue("@inSupervisingAttorney", inSupervisingAttorney);
            DB.Parameters.AddWithValue("@inCurrency", inCurrency);

            DB.Connection = conn;
            DB.CommandText = strQuery;
            var data = DB.ExecuteReader();

            if (data.HasRows)
            {
                while (data.Read())
                {
                    {
                        newnoterec.Add(new arclick.noterec
                        {
                            clientid = data["clientid"].ToString(),
                            LastUpdatedDate = data["LastUpdatedDate"].ToString(),
                            lastupdatedby = data["lastUpdatedBy"].ToString(),
                            note = data["note"].ToString(),
                            currency = data["currency"].ToString()
                        });

                    }
                }
            }
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //return serializer.Serialize(newnoterec);
            return newnoterec;
        }


        [WebMethod]
        public int SaveDetailNotes(string inClientID, string inSupervisingAttorney, string inCurrency, string inNote, string inUser)
        {

            if (inClientID != null && inSupervisingAttorney != null && inNote != null)
            {
                string strQuery = "";

                var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                SqlConnection conn = new SqlConnection(connectionString);

                conn.Open();
                strQuery = "UPDATE  [EliteService].[dbo].[InvestmentInClientServicesAgingByClientBySupervisingAttorneyByCurrency]  SET LastUpdatedBy = '" + inUser + "', LastUpdatedDate = GETDATE(),";
                strQuery += " note = @inNote  ";
                strQuery += " WHERE ClientID = @inClientID AND currency = @inCurrency AND SupervisingAttorneyId = @inSupervisingAttorney";

                var DB = new SqlCommand();
                DB.Parameters.AddWithValue("@inNote", inNote);
                DB.Parameters.AddWithValue("@inClientID", inClientID);
                DB.Parameters.AddWithValue("@inSupervisingAttorney", inSupervisingAttorney);
                DB.Parameters.AddWithValue("@inCurrency", inCurrency);

                DB.Connection = conn;
                DB.CommandText = strQuery;
               DB.ExecuteNonQuery();
                return 1;
            }
            else { return 0; }
            
        }

        public static int ToInt(string toParse)
        {
            int result;
            if (int.TryParse(toParse, out result)) return result;

            return result;
        }

        [WebMethod]
        public List<detailrowrec> GetDetailRow(string inClientID, string inSupervisingAttorneyID, string inCurrency)
        {
            List<detailrowrec> detailrow = new List<detailrowrec>();

            var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);


            StringBuilder strQuery = new StringBuilder("SELECT matter, MatterName, Invoice, Currency, ARAmt, InvoiceDate, [#Days], VatFlag FROM [EliteService].[dbo].[SelectARClickDetail] WHERE ");
            strQuery.Append(" client = @inClientID");
            if (inSupervisingAttorneyID != "")
            {
                strQuery.Append(" AND SupervisingAttorneyId = @inSupervisingAttorneyID");
            }
            strQuery.Append(" AND Currency = @inCurrency");


            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.Text;

            cmd.Parameters.AddWithValue("@inClientID", inClientID);
            cmd.Parameters.AddWithValue("@inSupervisingAttorneyID", inSupervisingAttorneyID);
            cmd.Parameters.AddWithValue("@inCurrency", inCurrency);

            cmd.CommandText = strQuery.ToString();


            conn.Open();

            SqlDataReader data = cmd.ExecuteReader();


            if (data.HasRows)
            {
                while (data.Read())
                {
                    {
                        detailrow.Add(new arclick.detailrowrec
                        {
                            matter = data["matter"].ToString(),
                            MatterName = data["MatterName"].ToString(),
                            Invoice = data["Invoice"].ToString(),
                            Currency = data["Currency"].ToString(),
                            ARAmt = data["ARAmt"].ToString(),
                            InvoiceDate = data["InvoiceDate"].ToString(),
                            numberofdays = data["#Days"].ToString(),
                            VatFlag = data["VatFlag"].ToString()
                        });
                    }
                }

            }
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //return serializer.Serialize(newnoterec);
            return detailrow;
        }


    }
}