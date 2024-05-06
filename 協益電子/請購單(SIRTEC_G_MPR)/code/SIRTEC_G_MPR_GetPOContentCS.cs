using System;
using System.Data;
using Microsoft.Win32;
using NAWXDBCINFOIOLib;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;

public partial class SIRTEC_G_MPR_GetPOContentCS : System.Web.UI.Page
{
    /// <summary>
    public class RootObj
    {
        public string Product_Name;
        public string Product_Specification;
        public float Product_Quantity;
    }

    /// </summary>
    public class RootObject
    {
        public string SelectFlag;
        public int Num;
        public string RequisitionID;
        public string Product_Name;
        public string Product_Specification;
        public string Customer;
        public string ItemName;
        public string ItemSpec;     
        public float ItemCount;     //數量
        public float ItemPrice;     //未稅單價
        public float ItemAmount;    //未稅總額
        public float TaxRate;       //稅率
        public float TotalTax;      //稅總金額	
        public float TotalAmount;   //含稅總價	
        public string comment;      //說明
    }

    /// <summary>
    /// 主程式


    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        #region 宣告程式碼歷程


        string log = "";
        #endregion

        #region 取得基本資料
        string Path = Request.CurrentExecutionFilePath.ToString();
        string[] PathAry = Path.Split('/');
        string Page = PathAry[2];
        log += "<h1>" + Page + "</h1>";
        log += "<hr>";
        #endregion

        #region 資料庫連線字串
        string SqlConnString = LoadCmdStr("\\\\Database\\\\Project\\\\FlowBPM\\\\Sirtec\\\\connection\\\\FM7.xdbc.xmf");
        string OracleConnString = ORACLECONN();
        #endregion

        #region 取得變數
        log += "<h2>RequisitionID</h2>";
        string RequisitionID = Request["RequisitionID"]??"1";
        log += RequisitionID + "<br>";

        log += "<h2>company_code</h2>";
        string company_code = Request["company_code"]??"100";
        log += company_code + "<br>";

        log += "<h2>ProcessID</h2>";
        string ProcessID = Request["ProcessID"]??"DsntList02";
        log += ProcessID + "<br>";
        #endregion
        //Response.End();

        //取得請購單明細表JOSN
        log += "<h2>json_FM7T_SIRTEC_G_MPR_D</h2>";
        string json_FM7T_SIRTEC_G_MPR_D = "[]";
        using (SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            try
            {
                DataSet dsData = new DataSet();
                Conn.Open();
                string cmdstr = "select * from FM7T_SIRTEC_G_MPR_D where RequisitionID='{0}'";
                cmdstr = String.Format(cmdstr, RequisitionID);
                SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                Adapter.Fill(dsData);
                json_FM7T_SIRTEC_G_MPR_D = JsonConvert.SerializeObject(dsData.Tables[0], Formatting.Indented);

            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }

        if (json_FM7T_SIRTEC_G_MPR_D == "[]")
        {
            RootObj RootObj = new RootObj();
            RootObj.Product_Name = "測試";
            RootObj.Product_Specification = "測試";
            RootObj.Product_Quantity = 0.0F;
            string sub_strJson = JsonConvert.SerializeObject(RootObj, Formatting.Indented);
            json_FM7T_SIRTEC_G_MPR_D = "[" + sub_strJson + "]";
        }
        log += json_FM7T_SIRTEC_G_MPR_D + "<br>";

        //取得採購單比價明細表JOSN
        log += "<h2>json_FM7T_SIRTEC_G_MPR_D_POContent</h2>";
        string json_FM7T_SIRTEC_G_MPR_D_POContent = "[]";
        using (SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            try
            {
                DataSet dsData = new DataSet();
                Conn.Open();
                string cmdstr = "select * from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID='{0}'";
                cmdstr = "select SelectFlag,Num,RequisitionID,Product_Name,Product_Specification,Customer,ItemName,ItemSpec,ItemCount,ItemPrice,ItemAmount,TaxRate,TotalTax,TotalAmount,comment from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID='{0}'";
                cmdstr = String.Format(cmdstr, RequisitionID);
                SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                Adapter.Fill(dsData);
                json_FM7T_SIRTEC_G_MPR_D_POContent = JsonConvert.SerializeObject(dsData.Tables[0], Formatting.Indented);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }
        log += json_FM7T_SIRTEC_G_MPR_D_POContent + "<br>";

        string htmlstr = "";
        string TRstr = "";
        if (ProcessID == "DsntList02" && ProcessID != "Applicant" &&  ProcessID != "Content" )
        {
            string table = "";
            table += "<table width='100%'>";
            table += "<caption>比價資料</caption>";
            
            DataTable dtItem = JsonConvert.DeserializeObject<DataTable>(json_FM7T_SIRTEC_G_MPR_D);
            int count = 1;

            try
            {
                foreach (DataRow rowItem in dtItem.Rows)
                {
                    table += "<tbody name='POContentInfo'>";
                    string Product_Name = rowItem["Product_Name"].ToString();
                    string Product_Specification = rowItem["Product_Specification"].ToString();
                    float Product_Quantity = float.Parse(rowItem["Product_Quantity"].ToString());

                    //第一行

                    table += "<tr name='POContent_header'><th>品名</th><td><input type='text' name='POContent_Product_Name' value='"+ Product_Name + "' hidden>" + Product_Name + "</td><th>規格</th><td><input type='text' name='POContent_Product_Specification' value='" + Product_Specification + "' hidden>" + Product_Specification + "</td></tr>";
                    //第二行

                    table += "<tr><td colspan=\"4\">";

                    //比價資料明細表

                    ///table
                    table += "<table name='POContent_table'>";
                    ///thead
                    table += "<thead>";
                    table += "<tr>";
                    string[] thAry = new string[] { " ", "序", "識別碼", "品名", "規格", "廠商", "品名", "規格", "數量", "未稅單價", "未稅金額", "稅率%", "稅總金額", "含稅總價", "說明" };
                    for (int i = 0; i < thAry.Length; i++)
                    {
                        table += "<th>" + thAry[i] + "</th>";
                    }
                    table += "</tr>";
                    table += "</thead>";

                    //tbody                
                    //資料轉換-比價5筆資料


                    //JSON筆數
                    log += "<h2>count_FM7T_SIRTEC_G_MPR_D_POContent</h2>";
                    int count_FM7T_SIRTEC_G_MPR_D_POContent = 0;
                    using (SqlConnection Conn = new SqlConnection(SqlConnString))
                    {
                        DataSet dsData = new DataSet();
                        Conn.Open();
                        //string cmdstr = "select count(*) from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID='{0}' and Product_Name = N'{1}'";
                        string cmdstr = "select count(*) from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID='{0}'";
                        cmdstr = String.Format(cmdstr, RequisitionID);
                        //cmdstr = String.Format(cmdstr, RequisitionID, Product_Name);
                        SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                        count_FM7T_SIRTEC_G_MPR_D_POContent = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                        Conn.Close();
                        Conn.Dispose();
                    }
                    log += count_FM7T_SIRTEC_G_MPR_D_POContent + "<br>";

                    string strJosn = "[]";
                    if (count_FM7T_SIRTEC_G_MPR_D_POContent == 0)
                    {
                        //無資料直接寫五筆
                        strJosn = "";
                        int n = 1;
                        for (int DataCount = 1; DataCount <= 5; DataCount++)
                        {
                            RootObject RootObject = new RootObject();
                            RootObject.SelectFlag = "N";
                            RootObject.Num = n;
                            RootObject.RequisitionID = RequisitionID;
                            RootObject.Product_Name = Product_Name;
                            RootObject.Product_Specification = Product_Specification;
                            RootObject.Customer = "";
                            RootObject.ItemName = Product_Name;
                            RootObject.ItemSpec = "";
                            RootObject.ItemCount = Product_Quantity;
                            RootObject.ItemPrice = 0.0F;
                            RootObject.ItemAmount = 0.0F;
                            RootObject.TaxRate = 0.0F;
                            RootObject.TotalTax = 0.0F;
                            RootObject.TotalAmount = 0.0F;
                            RootObject.comment = "";
                            string sub_strJson = JsonConvert.SerializeObject(RootObject, Formatting.Indented);
                            strJosn += sub_strJson + ",";
                            n++;
                        }
                        strJosn = strJosn.Remove(strJosn.Length - 1, 1);
                        strJosn = "[" + strJosn + "]";
                    }
                    else
                    {
                        strJosn = "";

                        int Count_POContentData = 0;
                        using (SqlConnection Conn = new SqlConnection(SqlConnString))
                        {
                            Conn.Open();
                            string cmdstr = "select count(*) from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID='{0}'";
                            //cmdstr = "select SelectFlag,Num,RequisitionID,Product_Name,Product_Specification,Customer,ItemName,ItemSpec,ItemCount,ItemPrice,TaxRate,TotalTax,TotalAmount,comment from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID='{0}' and Product_Name = N'{1}'";
                            cmdstr = "select count(*) from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID='{0}' and Product_Name = N'{1}'";
                            cmdstr = String.Format(cmdstr, RequisitionID, Product_Name);
                            SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                            Count_POContentData = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                            Conn.Close();
                            Conn.Dispose();                             
                        }
                        //Response.Write(Count_POContentData+"<br>");

                        RootObject RootObject = new RootObject();
                        string json_sub_FM7T_SIRTEC_G_MPR_D_POContent = "";
                        int DataCount = 1;

                        if (Count_POContentData > 0)
                        {
                            using (SqlConnection Conn = new SqlConnection(SqlConnString))
                            {
                                try
                                {
                                    DataSet dsData = new DataSet();
                                    Conn.Open();
                                    string cmdstr = "select * from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID='{0}'";
                                    cmdstr = "select SelectFlag,Num,RequisitionID,Product_Name,Product_Specification,Customer,ItemName,ItemSpec,ItemCount,ItemPrice,ItemAmount,TaxRate,TotalTax,TotalAmount,comment from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID='{0}' and Product_Name = N'{1}'";
                                    cmdstr = String.Format(cmdstr, RequisitionID, Product_Name);
                                    SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                                    SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                                    Adapter.Fill(dsData);
                                    json_sub_FM7T_SIRTEC_G_MPR_D_POContent = JsonConvert.SerializeObject(dsData.Tables[0], Formatting.Indented);
                                }
                                catch (Exception ex)
                                {
                                    Response.Write(ex.ToString());
                                }
                                finally
                                {
                                    Conn.Close();
                                    Conn.Dispose();
                                }
                            }
                            log += json_sub_FM7T_SIRTEC_G_MPR_D_POContent + "<br>";

                            DataTable dtItem_sub_FM7T_SIRTEC_G_MPR_D_POContent = JsonConvert.DeserializeObject<DataTable>(json_sub_FM7T_SIRTEC_G_MPR_D_POContent);
                            foreach (DataRow rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent in dtItem_sub_FM7T_SIRTEC_G_MPR_D_POContent.Rows)
                            {
                                RootObject.SelectFlag = rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent["SelectFlag"].ToString();
                                RootObject.Num = DataCount;
                                RootObject.RequisitionID = rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent["RequisitionID"].ToString();
                                RootObject.Product_Name = rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent["Product_Name"].ToString();
                                RootObject.Product_Specification = rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent["Product_Specification"].ToString();
                                RootObject.Customer = rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent["Customer"].ToString();
                                RootObject.ItemName = rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent["ItemName"].ToString();
                                RootObject.ItemSpec = rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent["ItemSpec"].ToString();
                                RootObject.ItemCount = float.Parse(rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent["ItemCount"].ToString());
                                RootObject.ItemPrice = float.Parse(rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent["ItemPrice"].ToString());
                                RootObject.ItemAmount = float.Parse(rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent["ItemAmount"].ToString());
                                RootObject.TaxRate = float.Parse(rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent["TaxRate"].ToString());
                                RootObject.TotalTax = float.Parse(rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent["TotalTax"].ToString());
                                RootObject.TotalAmount = float.Parse(rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent["TotalAmount"].ToString());
                                RootObject.comment = rowItem_sub_FM7T_SIRTEC_G_MPR_D_POContent["comment"].ToString();
                                string sub_strJson = JsonConvert.SerializeObject(RootObject, Formatting.Indented);
                                strJosn += sub_strJson + ",";
                                DataCount++;
                            }

                        }

                        for (int i = 1; i <= (5 - Count_POContentData); i++)
                        {
                            RootObject.SelectFlag = "N";
                            RootObject.Num = DataCount;
                            RootObject.RequisitionID = RequisitionID;
                            RootObject.Product_Name = Product_Name;
                            RootObject.Product_Specification = Product_Specification;
                            RootObject.Customer = "";
                            RootObject.ItemName = Product_Name;
                            RootObject.ItemSpec = "";
                            RootObject.ItemCount = Product_Quantity;
                            RootObject.ItemPrice = 0.0F;
                            RootObject.ItemAmount = 0.0F;
                            RootObject.comment = "";
                            string sub_strJson = JsonConvert.SerializeObject(RootObject, Formatting.Indented);
                            strJosn += sub_strJson + ",";
                            log += sub_strJson + "<br>";
                            DataCount++;
                        }
                        strJosn = strJosn.Remove(strJosn.Length - 1, 1);
                        strJosn = "[" + strJosn + "]";
                    }
                    log += strJosn + "<br>";


                    //Response.Write(strJosn+ "<br><br>");
                    table += "<tbody>";
                    DataTable dtItem_PO = JsonConvert.DeserializeObject<DataTable>(strJosn);
                    foreach (DataRow rowItem_PO in dtItem_PO.Rows)
                    {
                        table += "<tr name='POContent_tr'>";
                        foreach (DataColumn Column in dtItem_PO.Columns)
                        {
                            if (Column.ColumnName == "SelectFlag")
                            {
                                if (rowItem_PO[Column.ColumnName].ToString() == "Y")
                                {
                                    table += "<td><input type='radio' name='" + Column.ColumnName + "_" + count + "' checked></td>";
                                }
                                else
                                {
                                    table += "<td><input type='radio' name='" + Column.ColumnName + "_" + count + "'></td>";
                                }


                            }
                            else if (Column.ColumnName == "Num")
                            {
                                table += "<td align='center'><input type='text' name='" + Column.ColumnName + "' value='" + rowItem_PO[Column.ColumnName].ToString() + "' style='text-align:right' hidden>" + rowItem_PO[Column.ColumnName].ToString() + "</td>";
                            }
                            else
                            {
                                if (Column.ColumnName == "ItemCount" || Column.ColumnName == "ItemPrice" || Column.ColumnName == "ItemAmount" || Column.ColumnName == "TaxRate" || Column.ColumnName == "TotalTax" || Column.ColumnName == "TotalAmount")
                                {
                                    if (Column.ColumnName == "TaxRate")
                                    {
                                        table += "<td><input type='number' name='" + Column.ColumnName + "' value='" + rowItem_PO[Column.ColumnName].ToString() + "'></td>";
                                    }
                                    else
                                    {
                                        table += "<td><input type='number' name='" + Column.ColumnName + "' value='" + rowItem_PO[Column.ColumnName].ToString() + "'></td>";
                                    }
                                }
                                else
                                {
                                    table += "<td><input type='text' name='" + Column.ColumnName + "' value='" + rowItem_PO[Column.ColumnName].ToString() + "'></td>";
                                }
                            }
                        }
                        table += "</tr>";

                    }
                    count++;

                    table += "</tbody>";
                    //tfoot
                    //table += "<tfoot hidden>";
                    //table += "<tr><th>說明</th><td colspan='6'><textarea name='comment'  style='width:100%'></textarea></td></tr>";
                    //table += "</tfoot>";
                    table += "</table>";
                    table += "</td>";
                    table += "</tr>";
                    table += "</tbody>";
                }
                //table += "</tbody>";
                table += "</table>";
                table += "<br>";
                htmlstr += table;
            }catch(Exception ex)
            {
                Response.Write(ex.ToString());
                Response.Write(log);
            }
        }
        else
        {
            //檢視畫面
            ///採購費用明細表

            ///彙整資料明細表

            ///比價資料
            ///
            #region 採購費用明細表


            if (true)
            {
                //取得採購費用明細表JSON
                string jsonstr = "";
                using (SqlConnection Conn = new SqlConnection(SqlConnString))
                {
                    try
                    {
                        DataSet dsData = new DataSet();
                        Conn.Open();
                        string cmdstr = "";
                        if (company_code == "100" || company_code == "110" || company_code == "400")
                        {
                            cmdstr = "select Customer,'$'+replace(CONVERT(nvarchar(50), CONVERT(MONEY, SUM(TotalAmount)), 1),'.00','') Amout from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID='{0}' and SelectFlag='Y' group by Customer";
                        }
                        else
                        {
                            cmdstr = "select Customer,'￥'+CONVERT(nvarchar(50), CONVERT(MONEY, SUM(TotalAmount)), 1) Amout from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID='{0}' and SelectFlag='Y' group by Customer";
                        }
                        cmdstr = String.Format(cmdstr, RequisitionID);
                        SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                        SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                        Adapter.Fill(dsData);
                        jsonstr = JsonConvert.SerializeObject(dsData.Tables[0], Formatting.Indented);
                    }
                    catch (Exception ex)
                    {
                        Response.Write(ex.ToString());
                    }
                    finally
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }
                ///Response.Write(json_FM7T_SIRTEC_G_MPR_D_POCotent_Amout);

                //採購費用明細表(含開頭)
                string table = "";
                table += "<font>採購費用</font><br>";
                table += "<table>";                
                ///thead
                table += "<thead>";
                string[] AmoutAry = new string[] { "廠商", "總採購費用" };
                table += "<tr>";
                for (int i = 0; i < AmoutAry.Length; i++)
                {
                    table += "<th>" + AmoutAry[i] + "</th>";
                }
                table += "</tr>";
                table += "</thead>";
                ///tbody
                table += "<tbody>";
                if (jsonstr == "[]")
                {
                    table += "";
                }
                else
                {
                    DataTable dtItem = JsonConvert.DeserializeObject<DataTable>(jsonstr);
                    foreach (DataRow rowItem in dtItem.Rows)
                    {
                        table += "<tr>";
                        string Customer = rowItem["Customer"].ToString();
                        string Amout = rowItem["Amout"].ToString();
                        table += "<td>" + Customer + "</td>";
                        table += "<td style=\"text-align:right\">" + Amout + "</td>";
                        table += "</tr>";
                    }

                }
                table += "</tbody>";
                table += "</table>"; 
					 table += "<br>";
                ///寫入網頁
                htmlstr += table;
            }
            #endregion

            #region 彙整資料明細表



            if (true)
            {
                //取得彙整資料明細表JSON
                string jsonstr = "";
                using (SqlConnection Conn = new SqlConnection(SqlConnString))
                {
                    try
                    {
                        DataSet dsData = new DataSet();
                        Conn.Open();
                        string cmdstr = "";
                        if (company_code == "100" || company_code == "110" || company_code == "400")
                        {
                            cmdstr = "SELECT D.Num," +
                                "D.RequisitionID," +
                                "POContent.Product_Name," +
                                "POContent.Product_Specification," +
                                "POContent.ItemName," +
                                "POContent.ItemSpec,CONVERT(nvarchar(50), CONVERT(MONEY, POContent.ItemCount), 1) ItemCount," +
                                "'$'+Replace(CONVERT(nvarchar(50), CONVERT(MONEY, POContent.ItemPrice), 1),'.00','') ItemPrice," +
                                "'$'+Replace(CONVERT(nvarchar(50), CONVERT(MONEY, POContent.ItemAmount), 1),'.00','') ItemAmount," +
                                "Replace(CONVERT(nvarchar(50), CONVERT(MONEY, POContent.TaxRate), 1),'.00','')+'%' TaxRate," +
                                "'$'+Replace(CONVERT(nvarchar(50), CONVERT(MONEY, POContent.TotalTax), 1),'.00','') TotalTax," +
                                "'$'+Replace(CONVERT(nvarchar(50), CONVERT(MONEY, POContent.TotalAmount), 1),'.00','') TotalAmount," +
                                "POContent.Customer FROM FM7T_SIRTEC_G_MPR_D_POContent POContent inner join FM7T_SIRTEC_G_MPR_D D ON POContent.RequisitionID=D.RequisitionID AND POContent.Product_Name = d.Product_Name AND POContent.Product_Specification=D.Product_Specification WHERE POContent.RequisitionID='{0}' and POContent.SelectFlag='Y' order by D.Num asc";
                        }
                        else
                        {
                            cmdstr = "SELECT D.Num," +
                                "D.RequisitionID," +
                                "POContent.Product_Name," +
                                "POContent.Product_Specification," +
                                "POContent.ItemName," +
                                "POContent.ItemSpec," +
                                "CONVERT(nvarchar(50), CONVERT(MONEY, POContent.ItemCount), 1) ItemCount," +
                                "'￥'+CONVERT(nvarchar(50), CONVERT(MONEY, POContent.ItemPrice), 1) ItemPrice," +
                                "'￥'+CONVERT(nvarchar(50), CONVERT(MONEY, POContent.ItemAmount), 1) ItemAmount," +
                                "ISNULL(TaxRate,0) TaxRate," +
                                "ISNULL(TotalTax,0) TotalTax," +
                                "'￥'+CONVERT(nvarchar(50), CONVERT(MONEY, POContent.TotalAmount), 1) TotalAmount," +
                                "POContent.Customer FROM FM7T_SIRTEC_G_MPR_D_POContent POContent inner join FM7T_SIRTEC_G_MPR_D D ON POContent.RequisitionID=D.RequisitionID AND POContent.Product_Name = d.Product_Name AND POContent.Product_Specification=D.Product_Specification WHERE POContent.RequisitionID='{0}' and POContent.SelectFlag='Y' order by D.Num asc";
                        }
                        cmdstr = String.Format(cmdstr, RequisitionID);
                        log += "<font>彙整資料SQL</font><br>" + cmdstr;
                        SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                        SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                        Adapter.Fill(dsData);
                        jsonstr = JsonConvert.SerializeObject(dsData.Tables[0], Formatting.Indented);
                    }
                    catch (Exception ex)
                    {
                        Response.Write(ex.ToString());
                    }
                    finally
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }
                //Response.Write("<h3>json_FM7T_SIRTEC_G_MPR_D_POCotent_Fin</h3>" + jsonstr + "<br>");

                //彙整資料明細表(含開頭)
                string table = "";
                table = "<font>彙整資料</font><br>";
                ///table
                //table += "<table border='1'>";
                table += "<table>";

                ///thead
                table += "<thead>";
                table += "<tr>";
                string[] thAry = new string[] { "序", "識別碼", "品名", "規格", "廠商", "品名", "規格", "數量", "未稅單價", "未稅金額", "稅率", "稅總金額", "含稅總價"};
                for (int i = 0; i < thAry.Length; i++)
                {
                    table += "<th>" + thAry[i] + "</th>";
                }
                table += "</tr>";
                table += "</thead>";
                
                ///tbody
                table += "<tbody>";
                DataTable dtItem = JsonConvert.DeserializeObject<DataTable>(jsonstr);
                foreach (DataRow rowItem in dtItem.Rows)
                {

                    // 0:{ "Num": 1,
                    // 1:  "Product_Name": "筆記型電腦",
                    // 2:  "Product_Specification": "15.6'",
                    // 3:  "ItemName": "筆記型電腦",
                    // 4:  "ItemSpec": "Thinkbook 15",
                    // 5:  "ItemCount": 1.00,
                    // 6:  "ItemPrice": 18000.00,
                    // 7:  "ItemAmount": 18000.00,
                    // 8:  "Customer": "富邦媒體" }
                    string Num = rowItem["Num"].ToString();
                    string Product_Name = rowItem["Product_Name"].ToString();
                    string Product_Specification = rowItem["Product_Specification"].ToString();
                    string ItemName = rowItem["ItemName"].ToString();
                    string ItemSpec = rowItem["ItemSpec"].ToString();
                    decimal ItemCount = 0.00M;
                    string ItemCountSTR = rowItem["ItemCount"].ToString();
                    ItemCount = decimal.Parse(ItemCountSTR);
                    //ItemCount = Convert.ToDecimal(rowItem["ItemCount"].ToString("#0.00"));
                    string ItemPrice = rowItem["ItemPrice"].ToString();
                    string ItemAmount = rowItem["ItemAmount"].ToString();
                    string TaxRate = rowItem["TaxRate"].ToString();
                    string TotalTax = rowItem["TotalTax"].ToString();
                    string TotalAmount = rowItem["TotalAmount"].ToString();
                    string Customer = rowItem["Customer"].ToString();
                    string TRStr = "<tr><td style=\"text-align:center\">{0}</td>" +
                                       "<td>{1}</td>" +
                                       "<td>{2}</td>" +
                                       "<td>{3}</td>" +
                                       "<td>{4}</td>" +
                                       "<td>{5}</td>" +
                                       "<td>{6}</td>" +
                                       "<td style=\"text-align:right\">{7}</td>" +
                                       "<td style=\"text-align:right\">{8}</td>" +
                                       "<td style=\"text-align:right\">{9}</td>" +
                                       "<td style=\"text-align:right\">{10}</td>" +
                                       "<td style=\"text-align:right\">{11}</td>" +
                                       "<td style=\"text-align:right\">{12}</td></tr>";
                    //"序", "識別碼", "品名", "規格", "廠商", "品名", "規格", "數量", "未稅單價", "稅率", "稅總金額", "含稅總價"
                    TRStr = String.Format(TRStr, Num, RequisitionID, Product_Name, Product_Specification, Customer, ItemName, ItemSpec, ItemCount, ItemPrice,ItemAmount, TaxRate, TotalTax, TotalAmount);
                    table += TRStr;
                }
                table += "</tbody>";
                table += "</table>";
					 table += "<br>";
                ///寫入網頁
                htmlstr += table;
            }
            #endregion


            #region 比價資料
            if (true)
            {
                string table = "";
                table += "<font>比價資料</font><br>";
                table += "<table>";
                table += "<tbody>";
                DataTable dtItem_VIEW = JsonConvert.DeserializeObject<DataTable>(json_FM7T_SIRTEC_G_MPR_D);
                int DataCount = 1;
                foreach (DataRow rowItem_VIEW in dtItem_VIEW.Rows)
                {
                    string Product_Name = rowItem_VIEW["Product_Name"].ToString();
                    string Product_Specification = rowItem_VIEW["Product_Specification"].ToString();
                    string Product_Quantity = rowItem_VIEW["Product_Quantity"].ToString();
                    //Response.Write(Product_Name + "," + Product_Specification + "<br>");

                    table += "<tr><th>品名</th><td>" + Product_Name + "</td><th>規格</th><td>" + Product_Specification + "</td></tr>";
                    table += "<tr>";
                    table += "<td colspan=\"4\">";
                    //固定欄位
                    table += "<table name=\"POContent_table\">";
                    ///thead
                    table += "<thead>";
                    table += "<tr>";
                    string[] thAry = new string[] { " ", "序", "識別碼", "品名", "規格", "廠商", "品名", "規格", "數量", "未稅單價", "未稅金額", "稅率", "稅總金額", "含稅總價", "說明" };
                    for (int i = 0; i < thAry.Length; i++)
                    {
                        table += "<th>" + thAry[i] + "</th>";
                    }
                    table += "</tr>";
                    table += "</thead>";

                    ///tbody
                    table += "<tbody>";
                    //檢視
                    using (SqlConnection Conn = new SqlConnection(SqlConnString))
                    {
                        try
                        {
                            Conn.Open();
                            string cmdstr = "";

                            if (company_code == "100" || company_code == "110" || company_code == "400")
                            {
                                cmdstr = "select (select " +
                                    "case SelectFlag when 'Y' then 'V' else '' end td,''," +
                                    "Num td,''," +
                                    "RequisitionID td,''," +
                                    "Product_Name td,''," +
                                    "Product_Specification td,''," +
                                    "Customer td,''," +
                                    "ItemName td,''," +
                                    "ItemSpec td,''," +
                                    "ItemCount td,''," +
                                    "'$'+Replace(CONVERT(nvarchar(50), CONVERT(MONEY, ItemPrice), 1),'.00','') td,''," +
                                    "'$'+Replace(CONVERT(nvarchar(50), CONVERT(MONEY, ItemAmount), 1),'.00','') td,''," +
                                    "Replace(CONVERT(nvarchar(50), CONVERT(MONEY, TaxRate), 1),'.00','')+'%' td,''," +
                                    "'$'+Replace(CONVERT(nvarchar(50), CONVERT(MONEY, TotalTax), 1),'.00','') td,''," +
                                    "'$'+Replace(CONVERT(nvarchar(50), CONVERT(MONEY, TotalAmount), 1),'.00','') td,''," +
                                    "comment td,'' from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID = '{0}' and Product_Name=N'{1}' and Product_Specification=N'{2}' for xml path('tr')) TRstr";
                            }
                            else
                            {
                                cmdstr = "select (select case SelectFlag when 'Y' then 'V' else '' end td,'',Num td,'',RequisitionID td,'',Product_Name td,'',Product_Specification td,'',Customer td,'',ItemName td,'',ItemSpec td,'',ItemCount td,'','￥'+CONVERT(nvarchar(50), CONVERT(MONEY, ItemPrice), 1) td,'',CONVERT(nvarchar(50),CONVERT(MONEY, ItemAmount), 1) td,'',ISNULL(TaxRate,0) td,'',ISNULL(TotalTax,0) td,'','￥'+CONVERT(nvarchar(50), CONVERT(MONEY, TotalAmount), 1) td,'',comment td,'' from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID = '{0}' and Product_Name=N'{1}'  and Product_Specification=N'{2}' for xml path('tr')) TRstr";
                            }
                            cmdstr = String.Format(cmdstr, RequisitionID, Product_Name,Product_Specification);
                            log += "<font>比價資料</font><br>" + cmdstr;
                            SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    TRstr = dr["TRstr"].ToString();
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Response.Write(ex.ToString());
                        }
                        finally
                        {
                            Conn.Close();
                            Conn.Dispose();
                        }
                    }
                    table += TRstr;
                    table += "</tbody>";
                    table += "</table>";
                    table += "</td>";
                    table += "</tr>";
                    DataCount++;

                }
                table += "</tbody>";
                table += "</table>";
					 //table += "<br>";
                htmlstr += table;
            }
            #endregion
        }
        //htmlstr += "<br>"; 
        Response.Write(htmlstr);
        //Response.Write(log + "<hr>");
    }
    // <summary>
    // 取得AutoWEB資料庫連線字串
    // </summary>
    // <param name="xdbcPath">xdbc.Xmf所在路徑</param>
    // <returns></returns>
    public string LoadCmdStr(String xdbcPath)
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
        connectionString = connectionString.Replace(@"Provider=SQLOLEDB.1;", "");
        return connectionString;
    }
    public string ORACLECONN()
    {
        string SqlConnString = LoadCmdStr("\\\\Database\\\\Project\\\\FlowBPM\\\\Sirtec\\\\connection\\\\FM7.xdbc.xmf");
        string Domain = "";
        string Name = "";
        string Port = "";
        string Username = "";
        string Password = "";

        using (SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            try
            {
                Conn.Open();
                string cmdstr = "select * from zSYS_zCusOracleInfo";
                SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Domain = dr["OracleServerDomain"].ToString();
                    Name = dr["OracleServerName"].ToString();
                    Port = dr["OracleServerPort"].ToString();
                    Username = dr["OracleServerUsername"].ToString();
                    Password = dr["OracleServerPassword"].ToString();

                }
                dr.Close();
                cmd.Cancel();
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            Conn.Close();
            Conn.Dispose();
        }
        string OracleConnStringStr = "Data source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = {0})(PORT = {1}))(CONNECT_DATA =(SID = {2})));User Id={3};Password={4}";
        OracleConnStringStr = String.Format(OracleConnStringStr, Domain, Port, Name, Username, Password);
        return OracleConnStringStr;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="SqlConnString"></param>
    /// <returns></returns>
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
    /// <summary>
    /// SQL資料表轉成JSON字串
    /// </summary>
    /// <param name="sqlstr"></param>
    /// <param name="SqlConnString"></param>
    /// <returns></returns>
    string SqlStrConvertJson(string sqlstr)
    {
        string SqlConnString = LoadCmdStr("\\\\Database\\\\Project\\\\FlowBPM\\\\Sirtec\\\\connection\\\\FM7.xdbc.xmf");
        string jsonstr = "";
        using (SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            try
            {
                Conn.Open();
                DataSet dsData = new DataSet();
                string cmdstr = sqlstr;
                SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                Adapter.Fill(dsData);
                jsonstr = JsonConvert.SerializeObject(dsData.Tables[0], Formatting.Indented);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }
        return jsonstr;
    }

    public void SQLExecuteNonQuery(string cmdstr)
    {
        string SqlConnString = LoadCmdStr("\\\\Database\\\\Project\\\\FlowBPM\\\\Sirtec\\\\connection\\\\FM7.xdbc.xmf");
        using (SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }

    }

    public void SQLExecuteNonQuery(string[] cmdstrAry)
    {
        string SqlConnString = LoadCmdStr("\\\\Database\\\\Project\\\\FlowBPM\\\\Sirtec\\\\connection\\\\FM7.xdbc.xmf");

        for (int i = 0; i < cmdstrAry.Length; i++)
        {
            string cmdstr = cmdstrAry[i];
            using (SqlConnection Conn = new SqlConnection(SqlConnString))
            {
                try
                {
                    Conn.Open();
                    SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response.Write(ex.ToString());
                }
                finally
                {
                    Conn.Close();
                    Conn.Dispose();
                }
            }
        }
    }

    /// <summary>
    /// 透過資料表名稱取得新增SQL字串
    /// </summary>
    /// <param name="TABLENAME"></param>
    /// <returns></returns>
    public string INSERTSTR(string TABLENAME)
    {
        string INSERTSTR = "";
        string SqlConnString = LoadCmdStr("\\\\Database\\\\Project\\\\FlowBPM\\\\Sirtec\\\\connection\\\\FM7.xdbc.xmf");

        //表身D表_取得欄位(字串)
        string COLUMN = "";
        string VALUES = "";
        using (SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            try
            {
                Conn.Open();
                string cmdstr = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}'";
                cmdstr = String.Format(cmdstr, TABLENAME);
                //Response.Write(cmdstr+"<br>");                
                SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    int i = 0;
                    while (dr.Read())
                    {
                        string COLUMN_NAME = dr["COLUMN_NAME"].ToString();

                        if (i == 0)
                        {
                            COLUMN += COLUMN_NAME;
                            VALUES += COLUMN_NAME;
                        }
                        else
                        {
                            COLUMN += "," + COLUMN_NAME;
                            VALUES += ",@" + COLUMN_NAME;
                        }
                        i++;
                    }
                    dr.Close();
                    cmd.Dispose();
                }
                else
                {
                    Response.Write("無資料");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }

        }
        INSERTSTR = "INSERT INTO " + TABLENAME + "(" + COLUMN + ")VALUES(" + VALUES + ")";
        return INSERTSTR;
    }
    /// <summary>
    /// 透過資料表名稱取得新增SQL字串
    /// </summary>
    /// <param name="TABLENAME"></param>
    /// <returns></returns>
    public string INSERTSTR(string TABLENAME, string TYPE)
    {
        string INSERTSTR = "";
        string SqlConnString = LoadCmdStr("\\\\Database\\\\Project\\\\FlowBPM\\\\Sirtec\\\\connection\\\\FM7.xdbc.xmf");

        //表身D表_取得欄位(字串)
        string COLUMN = "";
        string VALUES = "";
        using (SqlConnection Conn = new SqlConnection(SqlConnString))
        {
            try
            {
                Conn.Open();
                string cmdstr = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}'";
                cmdstr = String.Format(cmdstr, TABLENAME);
                //Response.Write(cmdstr+"<br>");                
                SqlCommand cmd = new SqlCommand(cmdstr, Conn);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    int i = 0;
                    while (dr.Read())
                    {
                        string COLUMN_NAME = dr["COLUMN_NAME"].ToString();

                        if (i == 0)
                        {
                            COLUMN += COLUMN_NAME;
                            VALUES += ":" + COLUMN_NAME;
                        }
                        else
                        {
                            COLUMN += "," + COLUMN_NAME;
                            if (TYPE == "ORACLE")
                            {
                                VALUES += ",:" + COLUMN_NAME;
                            }
                        }
                        i++;
                    }
                    dr.Close();
                    cmd.Dispose();
                }
                else
                {
                    Response.Write("無資料");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }

        }
        INSERTSTR = "INSERT INTO " + TABLENAME + "(" + COLUMN + ")VALUES(" + VALUES + ")";
        return INSERTSTR;
    }
}
