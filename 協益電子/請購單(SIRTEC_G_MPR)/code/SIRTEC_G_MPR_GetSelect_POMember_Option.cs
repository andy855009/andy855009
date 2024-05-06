<%@ Page Language="C#" Debug="true" ResponseEncoding="utf-8" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data.OleDb" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="Oracle.ManagedDataAccess.Client" %>
<%@ Import Namespace="Microsoft.Win32" %>
<%@ Import Namespace="NAWXDBCINFOIOLib" %>
<%@ Import Namespace="System.Threading" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e){
        #region 資料庫連線字串
        string SqlConnString = LoadCmdStr("\\\\Database\\\\Project\\\\FlowBPM\\\\Sirtec\\\\connection\\\\FM7.xdbc.xmf");
        //string OracleConnString = ORACLECONN(SqlConnString);
        #endregion

        #region 取得參數
        string Company_code = JUDGE_STRING("Company_code");
        string ApplicantID = JUDGE_STRING("ApplicantID");
        string ApplicantDept = JUDGE_STRING("ApplicantDept");
        #endregion

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

        ///json_zSYS_SIRTEC_G_MPR_POMember_Category -> json_Category
        ///json_zSYS_SIRTEC_G_MPR_POMember          -> json_POMember        
        #region 取得分類
        string option = "";

        string json_Category="";
        using (SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            try{
                DataSet dsData = new DataSet();
                Conn.Open();
                string cmdstr="select DISTINCT Category from zSYS_SIRTEC_G_MPR_POMember where Company_code=@Company_code";
                SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                cmd.Parameters.AddWithValue("Company_code", Company_code);
                SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                Adapter.Fill(dsData);
                json_Category = JsonConvert.SerializeObject(dsData.Tables[0], Formatting.Indented);
            }catch (Exception ex){
                Response.Write(ex.ToString()+"<br><br>");

            }
            Conn.Close();
            Conn.Dispose();
        }
        //Response.Write(json_Category+"<br>");
        //Response.End();
        #endregion

        #region 取得分類人員及option字串
        if (json_Category == "[]")
        {
            return;
        }
        else
        {
          
            option += "<option>請選擇</option>";
            option += "<optgroup label=\"自行採購\">";
            option += "<option values=\""+ApplicantID+"@"+ApplicantDept+"\">"+ApplicantName+"@"+ApplicantDeptName+"</option></optgroup>";
            
            DataTable dtItem_Category = JsonConvert.DeserializeObject<DataTable>(json_Category);
            foreach (DataRow rowItem_Category in dtItem_Category.Rows)
            {
                string Category = rowItem_Category["Category"].ToString();
                option += "<optgroup label=\"" + Category + "\"></optgroup>";

                string json_POMember = "";
                using (SqlConnection Conn = new SqlConnection(SqlConnString))
                {
                    try
                    {
                        DataSet dsData = new DataSet();
                        Conn.Open();
                        string cmdstr = "select * from zSYS_SIRTEC_G_MPR_POMember where Company_code=@Company_code and Category=@Category";
                        SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                        cmd.Parameters.AddWithValue("Company_code", Company_code);
                        cmd.Parameters.AddWithValue("Category", Category);
                        SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                        Adapter.Fill(dsData);
                        json_POMember = JsonConvert.SerializeObject(dsData.Tables[0], Formatting.Indented);
                    }
                    catch (Exception ex)
                    {
                        Response.Write(ex.ToString() + "<br><br>");

                    }
                    Conn.Close();
                    Conn.Dispose();
                }
                //Response.Write(json_POMember+"<br>");

                DataTable dtItem_POMember = JsonConvert.DeserializeObject<DataTable>(json_POMember);
                foreach (DataRow rowItem_POMember in dtItem_POMember.Rows)
                {
                    string Combi_ID = rowItem_POMember["Combi_ID"].ToString();
                    string Combi_Name = rowItem_POMember["Combi_Name"].ToString();
                    option += "<option values=\"" + Combi_ID + "\">" + Combi_Name + "</option>";
                }
                option += "<optgroup label=\"" + Category + "\"></optgroup>";
            }
        }
        #endregion
        //Select += "</select>";
        Response.Write(option);

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
