<%@ Page Language="C#" Debug="true" ResponseEncoding="utf-8" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.Net.Mail" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Microsoft.Win32" %>
<%@ Import Namespace="NAWXDBCINFOIOLib" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<% Response.Charset = "utf-8";%>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        #region 取得網頁資訊                
        string Path = Request.CurrentExecutionFilePath.ToString();
        string[] PathAry=Path.Split('/');
        /*  foreach (string i in PathAry)
        {
            Response.Write(i.ToString()+"<br>");
        }
        */
        string Page     = PathAry[2];
        Response.Write("<H1>"+Page+"</H1>");
        #endregion


        #region MSSOL Oracle資料庫字串	
        string SqlConnString = LoadCmdStr("\\\\Database\\\\Project\\\\FlowBPM\\\\Sirtec\\\\connection\\\\FM7.xdbc.xmf");
        string OracleConnString     = ORACLECONN(SqlConnString);
        #endregion

        #region 取得表單識別碼


        string RequisitionID = Request["RequisitionID"] ?? "e6ae2934-e8e9-4d41-8427-f13d633f287c";
        string DiagramID = "";
        string SerialID = "";
        using(SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            Conn.Open();
            string cmdstr = "SELECT DiagramID,SerialID FROM FSe7en_Sys_Requisition WHERE RequisitionID=@RequisitionID";
            try
            {
                SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                cmd.Parameters.AddWithValue("RequisitionID", RequisitionID);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        DiagramID = dr["DiagramID"].ToString();
                        SerialID = dr["SerialID"].ToString();
                    }
                    cmd.Cancel();
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            finally
            {
                Conn.Dispose();
                Conn.Close();
            }
        }

        string Identify = "";
        string IdentifyName = "";
        using(SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            Conn.Open();
            string cmdstr = "select Identify,DisplayName from FSe7en_Sys_DiagramList WHERE DiagramID=@DiagramID";
            try
            {
                SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                cmd.Parameters.AddWithValue("DiagramID", DiagramID);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Identify = dr["Identify"].ToString();
                        IdentifyName = dr["DisplayName"].ToString();
                    }
                    cmd.Cancel();
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            finally
            {
                Conn.Dispose();
                Conn.Close();
            }

        }

        #endregion

        #region 產生PDF
        string ConvertFileName = "";
        string ConvertPageToPDF = "http://"+Request.Url.Authority+"/sirtec/FM7_FormContent_Print_Auto_APPO.aspx?EinB64="+Base64Encode("refill=1&Identify="+Identify+"&RequisitionID="+RequisitionID+"&DisplayName="  +Server.UrlEncode(IdentifyName).ToUpper());
        //PDF檔名
        string PDFFileName=SerialID+".pdf";
        //PDF位置
        ConvertFileName = @"C:\NTWEB\AutoWeb3\Database\Project\Flow\BPM\object\" + Identify + @"\Result\" + PDFFileName;
        string UrlEncodeFileFilePath = HttpUtility.HtmlEncode(ConvertFileName);
        //執行轉換PDF命令
        string FileName     = "wkhtmltopdf.exe";
        string Arguments = "--disable-smart-shrinking --zoom 0.9 "+ConvertPageToPDF + " " + ConvertFileName;

        ProcessStartInfo info = new ProcessStartInfo(FileName, Arguments);
        info.WorkingDirectory = @"C:\Program Files\wkhtmltopdf\bin";
        try{
            Process.Start(info);
        }catch (Exception ex){
            Response.Write(ex.ToString());
        }
        #endregion

        #region 回寫EMAIL TABLE
        string josn_printinfo = "";
        using(SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            DataSet dsData = new DataSet();
            string cmdstr = "SELECT * FROM XX_FORM_SIGNSETTING_1 WHERE RequisitionID=@RequisitionID";
            SqlCommand cmd = new SqlCommand(cmdstr, Conn);
            cmd.Parameters.AddWithValue("RequisitionID", RequisitionID);
            SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
            Adapter.Fill(dsData);
            josn_printinfo = JsonConvert.SerializeObject(dsData.Tables[0], Formatting.Indented);
        }

        if (true)
        {
            DataTable dtItem_printinfo = JsonConvert.DeserializeObject<DataTable>(josn_printinfo);
            foreach (DataRow rowItem_printinfo in dtItem_printinfo.Rows)
            {
                using (SqlConnection Conn = new SqlConnection(SqlConnString))
                {
                    Conn.Open();
                    string cmdstr = "INSERT INTO zSYS_zCusEmail_Log(AutoCounter,Page,RequisitionID,Identify,CompanyCode,DisplayName,EmailLink,EmailPort,EmailSender,EmailReceiver,EmailCC,EmailSubject,EmailContent,ConvertFileName,Time,Status,SendTime,PreSendTime)VALUES((SELECT ISNULL(MAX(AutoCounter),0)+1 FROM zSYS_zCusEmail_Log),@Page,@RequisitionID,@Identify,@CompanyCode,@DisplayName,@EmailLink,@EmailPort,@EmailSender,@EmailReceiver,@EmailCC,@EmailSubject,@EmailContent,@ConvertFileName,GETDATE(),0,null,DATEADD(MINUTE,1,GETDATE()))";
                    try
                    {
                        SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("Page", Page);
                        cmd.Parameters.AddWithValue("RequisitionID", RequisitionID);
                        cmd.Parameters.AddWithValue("Identify", Identify);
                        cmd.Parameters.AddWithValue("CompanyCode", rowItem_printinfo["company_code"].ToString());
                        cmd.Parameters.AddWithValue("DisplayName", IdentifyName);
                        cmd.Parameters.AddWithValue("EmailLink", rowItem_printinfo["MailPath"].ToString());
                        cmd.Parameters.AddWithValue("EmailPort", rowItem_printinfo["MailPort"].ToString());
                        cmd.Parameters.AddWithValue("EmailSender", rowItem_printinfo["MailSender"].ToString());
                        cmd.Parameters.AddWithValue("EmailReceiver", rowItem_printinfo["EMail"].ToString());
                        cmd.Parameters.AddWithValue("EmailCC", rowItem_printinfo["MailCC"].ToString());
                        cmd.Parameters.AddWithValue("EmailSubject", rowItem_printinfo["MailSubject"].ToString());
                        cmd.Parameters.AddWithValue("EmailContent", rowItem_printinfo["MailContent"].ToString());
                        cmd.Parameters.AddWithValue("ConvertFileName", ConvertFileName);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Response.Write(ex.ToString() + "<br><br>");
                        return;
                    }
                    finally
                    {
                        Conn.Dispose();
                        Conn.Close();
                    }
                }
            }
        }

        #endregion


        Response.End();
    }
    // <summary>
    // 取得AutoWEB資料庫連線字串
    // </summary>
    // <param name="xdbcPath">xdbc.Xmf所在路徑</param>
    // <returns></returns>
    string LoadCmdStr(String xdbcPath)
    {
        string FileName = "";
        const string userRoot = "HKEY_LOCAL_MACHINE";
        const string subkey = "Software\\NewType\\AutoWeb.Net";
        const string keyName = userRoot + "\\" + subkey;
        string Path = (string)Registry.GetValue(keyName, "Root", -1);
        Path = Path.Replace("\\", "\\\\");
        XdbcInfoIO objXdbc = new XdbcInfoIO();
        FileName = Path + xdbcPath;
        objXdbc.LoadFile(FileName, "");
        string connectionString = objXdbc.XdbcConnection.sOleDBConnectString;
        connectionString=connectionString.Replace(@"Provider=SQLOLEDB.1;","");
        return connectionString;
    }

    string ORACLECONN(string SqlConnString)
    {
        string OracleConnStringStr = "";
        using (SqlConnection OraSQLCONN = new SqlConnection(SqlConnString))
        {
            OraSQLCONN.Open();
            string OraSQQLCONNstr = "select * from zSYS_zCusOracleInfo";
            SqlCommand OraSQQLCONNcmd = new SqlCommand(OraSQQLCONNstr, OraSQLCONN);
            SqlDataReader OraSQQLCONNdr = OraSQQLCONNcmd.ExecuteReader();
            while (OraSQQLCONNdr.Read())
            {
                string OracleServerDomain = OraSQQLCONNdr["OracleServerDomain"].ToString();
                string OracleServerName = OraSQQLCONNdr["OracleServerName"].ToString();
                string OracleServerPort = OraSQQLCONNdr["OracleServerPort"].ToString();
                string OracleServerUsername = OraSQQLCONNdr["OracleServerUsername"].ToString();
                string OracleServerPassword = OraSQQLCONNdr["OracleServerPassword"].ToString();


                OracleConnStringStr = "Data source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = " + OracleServerDomain + ")(PORT = "
                                                                                                                                    + OracleServerPort + "))(CONNECT_DATA =(SID = "
                                                                                                                                    + OracleServerName + ")));User Id="
                                                                                                                                    + OracleServerUsername + ";Password="
                                                                                                                                    + OracleServerPassword;

            }
            OraSQQLCONNdr.Close();
            OraSQQLCONNcmd.Cancel();

            OraSQLCONN.Close();
            OraSQLCONN.Dispose();
        }

        return OracleConnStringStr;
    }

    //Base64 加密
    public string Base64Encode(string data)
    {

        try
        {
            byte[] encData_byte = new byte[data.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
            string encodedData = Convert.ToBase64String(encData_byte);
            return encodedData;
        }
        catch (Exception e)
        {
            throw new Exception("Error in base64Encode" + e.Message);
        }

    }

    //LOG
    public static void LOG(string SqlConn, string Page, string RequisitionID, string ErrorMessage)
    {

        string LogStr = "INSERT INTO zSYS_zCusPage_Log(AutoCounter,Page,RequisitionID,ErrorMessage,Time)" +
                             "VALUES((SELECT ISNULL(MAX(AutoCounter),0)+1 FROM zSYS_zCusPage_Log),'" + Page + "','" + RequisitionID + "','" + ErrorMessage + "',GETDATE())";

        using (SqlConnection LogsqlConn = new SqlConnection(SqlConn))
        {

            LogsqlConn.Open();
            SqlCommand logcmd = new SqlCommand(LogStr, LogsqlConn);

            try
            {
                logcmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                LogsqlConn.Close();
                LogsqlConn.Dispose();
            }
        }

    }

    //錯誤訊息Mail至系統管理者












    public static void MAILLOG(string SqlConnString,string Page,string RequisitionID, string WirteMessage)
    {
        string SysAdminList="";

        string Email="";
        string DisplayName="";

        if (String.IsNullOrEmpty(RequisitionID))
        {
            RequisitionID="手動測試";
        }

        using (SqlConnection MailLogConn = new SqlConnection(SqlConnString))
        {
            try
            {

                MailLogConn.Open();
                string SySAdminstr="select (select AccountID+';' from FSe7en_Org_MemberInfo where AccountID in (SELECT  F7Organ_LView_CurrMember.AccountID FROM FSe7en_Tep_AccAuthList INNER JOIN F7Organ_LView_CurrMember ON FSe7en_Tep_AccAuthList.AccountID = F7Organ_LView_CurrMember.ACCOUNTID WHERE  FSe7en_Tep_AccAuthList.AuthID='SysAdmin'  AND F7Organ_LView_CurrMember.IsMainJob=1 AND F7Organ_LView_CurrMember.Lang='zh-tw') for xml path('')) AS SysAdminList";


                SqlCommand MailLogcmd = new SqlCommand(SySAdminstr, MailLogConn);
                SqlDataReader MailLogdr = MailLogcmd.ExecuteReader();


                while (MailLogdr.Read())
                {

                    SysAdminList    = MailLogdr["SysAdminList"].ToString();

                }
                MailLogdr.Close();

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                //Response.Write(ex.ToString());

            }



            string[] SysAdminAry=SysAdminList.Split(';');

            int SysAdminAryCount=SysAdminAry.Length-1;


            for(int i=0;i<SysAdminAryCount;i++)
            {
                string AccountID = SysAdminAry[i];

                try
                {

                    string AccountStr="select DisplayName,Email from FSe7en_Org_MemberInfo where AccountID='"+AccountID+"'";

                    SqlCommand  AccountStrcmd = new SqlCommand(AccountStr, MailLogConn);
                    SqlDataReader Accountdr = AccountStrcmd.ExecuteReader();

                    while (Accountdr.Read())
                    {

                        Email    = Accountdr["Email"].ToString();
                        DisplayName    = Accountdr["DisplayName"].ToString();

                    }
                    Accountdr.Close();

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                    //Response.Write(ex.ToString());

                }

                string LogStr = "INSERT INTO FSe7en_EMail_Bank (AutoCounter,Subject,Content,FromList,ToList,MailTime,PreField,Priority)" +
                                 "VALUES((SELECT ISNULL(MAX(AutoCounter),0)+1 FROM FSe7en_EMail_Bank),N'"+Page+"發生錯誤("+RequisitionID+")',N'"+ WirteMessage.Replace("'", "''") + "',N'"+DisplayName+"','"+Email+"',GETDATE(),0,3)";

                SqlCommand logcmd = new SqlCommand(LogStr, MailLogConn);

                try
                {
                    logcmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    //Response.Write(ex.ToString());
                }
            }
        }
    }

</script>
