﻿using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoodFood
{
    public partial class Dish : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.BindGrid();
            }

        }

        private void BindGrid()
        {
            string constr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            OracleCommand cmd = new OracleCommand();
            OracleConnection con = new OracleConnection(constr);
            con.Open();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM dish";
            cmd.CommandType = CommandType.Text;
            DataTable dt = new DataTable("dish");

            using (OracleDataReader sdr = cmd.ExecuteReader())
            {
                dt.Load(sdr);
            }

            con.Close();

            GridView1.DataSource = dt;
            GridView1.DataBind();

        }

        protected void Insert_Click(object sender, EventArgs e)
        {
            
            string dish = txtDish.Text.ToString();
            string local_name = txtLocalName.Text.ToString();

            string constr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (OracleConnection con = new OracleConnection(constr))
            {
                using (OracleCommand cmd = new OracleCommand($"Insert into dish values(dish_code.nextval,'{dish}','{local_name}')"))
                {

                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();



                }

            }


            this.BindGrid();

        }

        protected void Clear_Click1(object sender, EventArgs e)
        {
            txtDish.Text = "";
            txtLocalName.Text = "";
        }

        protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = GridView1.Rows[e.RowIndex];
            int dish_code = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
            string dish_name = (row.Cells[2].Controls[0] as TextBox).Text;
            string local_name = (row.Cells[3].Controls[0] as TextBox).Text;
            string constr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (OracleConnection con = new OracleConnection(constr))
            {
                using (OracleCommand cmd = new OracleCommand($"update dish set dish_name = '{dish_name}', local_name = '{local_name}' where dish_code =  '{dish_code}'"))
                {

                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();
                    con.Close();


                }
            }

            GridView1.EditIndex = -1;
            this.BindGrid();
        }


        protected void OnRowCancelingEdit(object sender, EventArgs e)
        {
            GridView1.EditIndex = -1;
            this.BindGrid();
        }

        protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int dish_code = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
            string constr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection con = new OracleConnection(constr))
            {
                using (OracleCommand cmd = new OracleCommand($"DELETE FROM dish WHERE dish_code ={dish_code}"))
                {

                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            this.BindGrid();
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowIndex != GridView1.EditIndex)
            {
                (e.Row.Cells[0].Controls[2] as LinkButton).Attributes["onclick"] = "return confirm('Do you want to delete this row?');";
            }

        }
        protected void OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            this.BindGrid();
        }
    }
}