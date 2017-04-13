﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PitchPointsWeb.Models;
using PitchPointsWeb.API;
using System.Diagnostics;
using PitchPointsWeb.Models.API;
using System.Data.SqlClient;
using System.Data;


namespace PitchPointsWeb
{
    public partial class CompInfo : Page
    {
        

        protected async void Page_Load(object sender, EventArgs e)
        {
            string empty = "";
            if (CompetitionResults.Text == empty)
            {
                var controller = new AccountController();
                var TokenModel = new TokenModel
                {
                    Token = Master.ReadToken()
                };

                var result = await controller.GetUserSnapshot(TokenModel);
                if (result.Success)
                {
                    using (var connection = MasterController.GetConnection())
                    {
                        connection.Open();
                        using (var command = new SqlCommand("GetAllUserInfo", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@email", TokenModel.Content.Email);
                            SqlDataReader rdr = command.ExecuteReader();
                            while (rdr.Read())
                            {
                                //FirstLabel.Text = (UppercaseFirst(rdr["FirstName"].ToString()));
                                //LastLabel.Text = (UppercaseFirst(rdr["LastName"].ToString()));
                                //courseNums.Add(rdr["CourseNumber"].ToString());
                            }
                            rdr.Close();
                            command.ExecuteNonQuery();
                        }
                    }

                   
                    CompCompDataSource.SelectParameters["email"].DefaultValue = TokenModel.Content.Email;
                    CompCompDataSource.SelectParameters["compId"].DefaultValue = "7";
                   
                }
                else
                {

                }
            }



            CompetitionResults.Text = "Competition Results";
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {

        }
        protected void btnUnregister_Click(object sender, EventArgs e)
        {

        }
    }
}