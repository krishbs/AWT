using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand("select * from UserInformation where UserName =@username and Password=@password", con);
        cmd.Parameters.AddWithValue("@username", TextBox1.Text);
        cmd.Parameters.AddWithValue("@password", TextBox2.Text);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            Response.Redirect("techwareLogin.aspx");
        }
        else
        {
            Label3.Visible = true;
            Label3.Text = "Wrong Details";
        }
    }
}