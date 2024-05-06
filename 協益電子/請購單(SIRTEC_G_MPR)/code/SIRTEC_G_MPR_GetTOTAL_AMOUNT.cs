<%@ Page Language="C#" Debug="true" ResponseEncoding="utf-8" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data.OleDb" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="Oracle.ManagedDataAccess.Client" %>
<%@ Import Namespace="Microsoft.Win32" %>
<%@ Import Namespace="NAWXDBCINFOIOLib" %>
<%@ Import Namespace="System.Threading" %>
<% Response.Charset = "utf-8";%>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e){
        #region 資料庫連線字串
        string SqlConnString = LoadCmdStr("\\\\Database\\\\Project\\\\FlowBPM\\\\Sirtec\\\\connection\\\\FM7.xdbc.xmf");
        //string OracleConnString = ORACLECONN(SqlConnString);
        #endregion

        string RequisitionID = Request["RequisitionID"];
        using (SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            try{
                Conn.Open();
                string cmdstr="update FM7T_SIRTEC_G_MPR_M set TOTAL_AMOUNT=(select SUM(ItemAmount) from FM7T_SIRTEC_G_MPR_D_POContent where SelectFlag='Y' and RequisitionID=@RequisitionID)";
                SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                cmd.Parameters.AddWithValue("RequisitionID", RequisitionID);
                cmd.Parameters.AddWithValue("RequisitionID", RequisitionID);
            }catch (Exception ex){
                Response.Write(ex.ToString()+"<br><br>");
            }
            Conn.Close();
            Conn.Dispose();
        }
    }
    string JUDGE_STRING(string columnname)
    {
        string value = Request[columnname];
        if (!String.IsNullOrEmpty(value))
        {
            return value;
        }
        else
        {
            Response.Write(columnname + "沒有值");
            Response.End();
            return columnname + "沒有值";
        }
    }

    // <summary>
    // 取得AutoWEB資料庫連線字串
    // </summary>
    // <param name="xdbcPath">xdbc.Xmf所在路徑</param>
    // <returns></returns>
    string LoadCmdStr(String xdbcPath){
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
    string ORACLECONN(string SqlConnString){
        string OracleConnStringStr = "";
        using (SqlConnection OraSQLCONN = new SqlConnection(SqlConnString)){
            OraSQLCONN.Open();
            string OraSQQLCONNstr = "select * from zSYS_zCusOracleInfo";
            SqlCommand OraSQQLCONNcmd = new SqlCommand(OraSQQLCONNstr, OraSQLCONN);
            SqlDataReader OraSQQLCONNdr = OraSQQLCONNcmd.ExecuteReader();
            while (OraSQQLCONNdr.Read())        {
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
    string ISNULL_String(string str){
        if(str=="null"){
            str="";
        }
        return str;
    }
    string ISNULL_Number(string str){
        if(str=="null"){
            str="0";
        }
        return str;
    }
</script>
