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
        public float ItemCount;     //�ƶq
        public float ItemPrice;     //���|���
        public float ItemAmount;    //���|�`�B
        public float TaxRate;       //�|�v
        public float TotalTax;      //�|�`���B	
        public float TotalAmount;   //�t�|�`��	
        public string comment;      //����
    }

    /// <summary>
    /// �D�{��


    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        #region �ŧi�{���X���{


        string log = "";
        #endregion

        #region ���o�򥻸��
        string Path = Request.CurrentExecutionFilePath.ToString();
        string[] PathAry = Path.Split('/');
        string Page = PathAry[2];
        log += "<h1>" + Page + "</h1>";
        log += "<hr>";
        #endregion

        #region ��Ʈw�s�u�r��
        string SqlConnString = LoadCmdStr("\\\\Database\\\\Project\\\\FlowBPM\\\\Sirtec\\\\connection\\\\FM7.xdbc.xmf");
        string OracleConnString = ORACLECONN();
        #endregion

        #region ���o�ܼ�
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

        //���o���ʳ���Ӫ�JOSN
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
            RootObj.Product_Name = "����";
            RootObj.Product_Specification = "����";
            RootObj.Product_Quantity = 0.0F;
            string sub_strJson = JsonConvert.SerializeObject(RootObj, Formatting.Indented);
            json_FM7T_SIRTEC_G_MPR_D = "[" + sub_strJson + "]";
        }
        log += json_FM7T_SIRTEC_G_MPR_D + "<br>";

        //���o���ʳ������Ӫ�JOSN
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
            table += "<caption>������</caption>";
            
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

                    //�Ĥ@��

                    table += "<tr name='POContent_header'><th>�~�W</th><td><input type='text' name='POContent_Product_Name' value='"+ Product_Name + "' hidden>" + Product_Name + "</td><th>�W��</th><td><input type='text' name='POContent_Product_Specification' value='" + Product_Specification + "' hidden>" + Product_Specification + "</td></tr>";
                    //�ĤG��

                    table += "<tr><td colspan=\"4\">";

                    //�����Ʃ��Ӫ�

                    ///table
                    table += "<table name='POContent_table'>";
                    ///thead
                    table += "<thead>";
                    table += "<tr>";
                    string[] thAry = new string[] { " ", "��", "�ѧO�X", "�~�W", "�W��", "�t��", "�~�W", "�W��", "�ƶq", "���|���", "���|���B", "�|�v%", "�|�`���B", "�t�|�`��", "����" };
                    for (int i = 0; i < thAry.Length; i++)
                    {
                        table += "<th>" + thAry[i] + "</th>";
                    }
                    table += "</tr>";
                    table += "</thead>";

                    //tbody                
                    //����ഫ-���5�����


                    //JSON����
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
                        //�L��ƪ����g����
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
                    //table += "<tr><th>����</th><td colspan='6'><textarea name='comment'  style='width:100%'></textarea></td></tr>";
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
            //�˵��e��
            ///���ʶO�Ω��Ӫ�

            ///�J���Ʃ��Ӫ�

            ///������
            ///
            #region ���ʶO�Ω��Ӫ�


            if (true)
            {
                //���o���ʶO�Ω��Ӫ�JSON
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
                            cmdstr = "select Customer,'�D'+CONVERT(nvarchar(50), CONVERT(MONEY, SUM(TotalAmount)), 1) Amout from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID='{0}' and SelectFlag='Y' group by Customer";
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

                //���ʶO�Ω��Ӫ�(�t�}�Y)
                string table = "";
                table += "<font>���ʶO��</font><br>";
                table += "<table>";                
                ///thead
                table += "<thead>";
                string[] AmoutAry = new string[] { "�t��", "�`���ʶO��" };
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
                ///�g�J����
                htmlstr += table;
            }
            #endregion

            #region �J���Ʃ��Ӫ�



            if (true)
            {
                //���o�J���Ʃ��Ӫ�JSON
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
                                "'�D'+CONVERT(nvarchar(50), CONVERT(MONEY, POContent.ItemPrice), 1) ItemPrice," +
                                "'�D'+CONVERT(nvarchar(50), CONVERT(MONEY, POContent.ItemAmount), 1) ItemAmount," +
                                "ISNULL(TaxRate,0) TaxRate," +
                                "ISNULL(TotalTax,0) TotalTax," +
                                "'�D'+CONVERT(nvarchar(50), CONVERT(MONEY, POContent.TotalAmount), 1) TotalAmount," +
                                "POContent.Customer FROM FM7T_SIRTEC_G_MPR_D_POContent POContent inner join FM7T_SIRTEC_G_MPR_D D ON POContent.RequisitionID=D.RequisitionID AND POContent.Product_Name = d.Product_Name AND POContent.Product_Specification=D.Product_Specification WHERE POContent.RequisitionID='{0}' and POContent.SelectFlag='Y' order by D.Num asc";
                        }
                        cmdstr = String.Format(cmdstr, RequisitionID);
                        log += "<font>�J����SQL</font><br>" + cmdstr;
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

                //�J���Ʃ��Ӫ�(�t�}�Y)
                string table = "";
                table = "<font>�J����</font><br>";
                ///table
                //table += "<table border='1'>";
                table += "<table>";

                ///thead
                table += "<thead>";
                table += "<tr>";
                string[] thAry = new string[] { "��", "�ѧO�X", "�~�W", "�W��", "�t��", "�~�W", "�W��", "�ƶq", "���|���", "���|���B", "�|�v", "�|�`���B", "�t�|�`��"};
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
                    // 1:  "Product_Name": "���O���q��",
                    // 2:  "Product_Specification": "15.6'",
                    // 3:  "ItemName": "���O���q��",
                    // 4:  "ItemSpec": "Thinkbook 15",
                    // 5:  "ItemCount": 1.00,
                    // 6:  "ItemPrice": 18000.00,
                    // 7:  "ItemAmount": 18000.00,
                    // 8:  "Customer": "�I���C��" }
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
                    //"��", "�ѧO�X", "�~�W", "�W��", "�t��", "�~�W", "�W��", "�ƶq", "���|���", "�|�v", "�|�`���B", "�t�|�`��"
                    TRStr = String.Format(TRStr, Num, RequisitionID, Product_Name, Product_Specification, Customer, ItemName, ItemSpec, ItemCount, ItemPrice,ItemAmount, TaxRate, TotalTax, TotalAmount);
                    table += TRStr;
                }
                table += "</tbody>";
                table += "</table>";
					 table += "<br>";
                ///�g�J����
                htmlstr += table;
            }
            #endregion


            #region ������
            if (true)
            {
                string table = "";
                table += "<font>������</font><br>";
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

                    table += "<tr><th>�~�W</th><td>" + Product_Name + "</td><th>�W��</th><td>" + Product_Specification + "</td></tr>";
                    table += "<tr>";
                    table += "<td colspan=\"4\">";
                    //�T�w���
                    table += "<table name=\"POContent_table\">";
                    ///thead
                    table += "<thead>";
                    table += "<tr>";
                    string[] thAry = new string[] { " ", "��", "�ѧO�X", "�~�W", "�W��", "�t��", "�~�W", "�W��", "�ƶq", "���|���", "���|���B", "�|�v", "�|�`���B", "�t�|�`��", "����" };
                    for (int i = 0; i < thAry.Length; i++)
                    {
                        table += "<th>" + thAry[i] + "</th>";
                    }
                    table += "</tr>";
                    table += "</thead>";

                    ///tbody
                    table += "<tbody>";
                    //�˵�
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
                                cmdstr = "select (select case SelectFlag when 'Y' then 'V' else '' end td,'',Num td,'',RequisitionID td,'',Product_Name td,'',Product_Specification td,'',Customer td,'',ItemName td,'',ItemSpec td,'',ItemCount td,'','�D'+CONVERT(nvarchar(50), CONVERT(MONEY, ItemPrice), 1) td,'',CONVERT(nvarchar(50),CONVERT(MONEY, ItemAmount), 1) td,'',ISNULL(TaxRate,0) td,'',ISNULL(TotalTax,0) td,'','�D'+CONVERT(nvarchar(50), CONVERT(MONEY, TotalAmount), 1) td,'',comment td,'' from FM7T_SIRTEC_G_MPR_D_POContent where RequisitionID = '{0}' and Product_Name=N'{1}'  and Product_Specification=N'{2}' for xml path('tr')) TRstr";
                            }
                            cmdstr = String.Format(cmdstr, RequisitionID, Product_Name,Product_Specification);
                            log += "<font>������</font><br>" + cmdstr;
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
    // ���oAutoWEB��Ʈw�s�u�r��
    // </summary>
    // <param name="xdbcPath">xdbc.Xmf�Ҧb���|</param>
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
    /// SQL��ƪ��নJSON�r��
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
    /// �z�L��ƪ�W�٨��o�s�WSQL�r��
    /// </summary>
    /// <param name="TABLENAME"></param>
    /// <returns></returns>
    public string INSERTSTR(string TABLENAME)
    {
        string INSERTSTR = "";
        string SqlConnString = LoadCmdStr("\\\\Database\\\\Project\\\\FlowBPM\\\\Sirtec\\\\connection\\\\FM7.xdbc.xmf");

        //��D��_���o���(�r��)
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
                    Response.Write("�L���");
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
    /// �z�L��ƪ�W�٨��o�s�WSQL�r��
    /// </summary>
    /// <param name="TABLENAME"></param>
    /// <returns></returns>
    public string INSERTSTR(string TABLENAME, string TYPE)
    {
        string INSERTSTR = "";
        string SqlConnString = LoadCmdStr("\\\\Database\\\\Project\\\\FlowBPM\\\\Sirtec\\\\connection\\\\FM7.xdbc.xmf");

        //��D��_���o���(�r��)
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
                    Response.Write("�L���");
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
