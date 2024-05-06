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

        #region 取得batch_id
        string Select = "";
        string Company_code = JUDGE_STRING("Company_code");
        

        string ApplicantID = JUDGE_STRING("ApplicantID");
        string ApplicantDept = JUDGE_STRING("ApplicantDept"); 

        string ApplicantName = "";
        using (SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            try{
                DataSet dsData = new DataSet();
                Conn.Open();
                string cmdstr="SELECT DisplayName FROM FSe7en_Org_MemberInfo WHERE AccountID=@AccountID";
                SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                cmd.Parameters.AddWithValue("AccountID", ApplicantID);
                ApplicantName = cmd.ExecuteScalar().ToString();
            }catch (Exception ex){
                Response.Write(ex.ToString()+"<br><br>");

            }
            Conn.Close();
            Conn.Dispose();
        }

        
        string ApplicantDeptName = "";
        using (SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            try{
                DataSet dsData = new DataSet();
                Conn.Open();
                string cmdstr="SELECT DisplayName FROM F7Organ_LView_CurrDept WHERE DeptID=@DeptID and Lang='zh-tw'";
                SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                cmd.Parameters.AddWithValue("DeptID", ApplicantDept);
                ApplicantDeptName = cmd.ExecuteScalar().ToString();
            }catch (Exception ex){
                Response.Write(ex.ToString()+"<br><br>");

            }
            Conn.Close();
            Conn.Dispose();
        }



        string json_zSYS_SIRTEC_G_MPR_POMember_Category="";
        using (SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            try{
                DataSet dsData = new DataSet();
                Conn.Open();
                string cmdstr="SELECT DISTINCT Category FROM zSYS_SIRTEC_G_MPR_POMember WHERE Company_code=@Company_code AND Enabled=1";
                SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                cmd.Parameters.AddWithValue("Company_code", Company_code);
                SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                Adapter.Fill(dsData);
                json_zSYS_SIRTEC_G_MPR_POMember_Category = JsonConvert.SerializeObject(dsData.Tables[0], Formatting.Indented);
            }catch (Exception ex){
                Response.Write(ex.ToString()+"<br><br>");

            }
            Conn.Close();
            Conn.Dispose();
        }
        //Response.Write(json_zSYS_SIRTEC_G_MPR_POMember_Category+"<br>");
		//Response.End();
        
		Select += "<select name=\"POMember_SelectItem\">";
        Select += "<option>請選擇</option>";
        Select += "<optgroup label=\"自行採購\">";
        Select += "<option values=\""+ApplicantID+"@"+ApplicantDept+"\">"+ApplicantName+"@"+ApplicantDeptName+"</option></optgroup>";
        DataTable dtItem_zSYS_SIRTEC_G_MPR_POMember_Category = JsonConvert.DeserializeObject<DataTable>(json_zSYS_SIRTEC_G_MPR_POMember_Category);
        foreach(DataRow rowItem_zSYS_SIRTEC_G_MPR_POMember_Category in dtItem_zSYS_SIRTEC_G_MPR_POMember_Category.Rows)
        {
            string Category = rowItem_zSYS_SIRTEC_G_MPR_POMember_Category["Category"].ToString();
            Select += "<optgroup label=\"" + Category + "\">";           
            
			string json_zSYS_SIRTEC_G_MPR_POMember="";
            using (SqlConnection Conn = new SqlConnection(SqlConnString))
            {
                try{
                    DataSet dsData = new DataSet();
                    Conn.Open();
                    string cmdstr="SELECT * FROM zSYS_SIRTEC_G_MPR_POMember WHERE Company_code=@Company_code AND Category=@Category AND Enabled=1";
                    SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                    cmd.Parameters.AddWithValue("Company_code", Company_code);
                    cmd.Parameters.AddWithValue("Category", Category);
                    SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                    Adapter.Fill(dsData);
                    json_zSYS_SIRTEC_G_MPR_POMember = JsonConvert.SerializeObject(dsData.Tables[0], Formatting.Indented);
                }catch (Exception ex){
                    Response.Write(ex.ToString()+"<br><br>");

                }
                Conn.Close();
                Conn.Dispose();
            }
            //Response.Write(json_zSYS_SIRTEC_G_MPR_POMember+"<br>");
				//Response.End();
				
            DataTable dtItem_zSYS_SIRTEC_G_MPR_POMember = JsonConvert.DeserializeObject<DataTable>(json_zSYS_SIRTEC_G_MPR_POMember);
            foreach(DataRow rowItem_zSYS_SIRTEC_G_MPR_POMember in dtItem_zSYS_SIRTEC_G_MPR_POMember.Rows)
            {
                string Combi_ID = rowItem_zSYS_SIRTEC_G_MPR_POMember["Combi_ID"].ToString();
                string Combi_Name = rowItem_zSYS_SIRTEC_G_MPR_POMember["Combi_Name"].ToString();
                Select += "<option values=\""+Combi_ID+"\">"+Combi_Name+"</option>";
            }

        }
        Select += "</select>";
        Response.Write(Select);
        #endregion
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
