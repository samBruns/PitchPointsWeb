﻿using System;
using System.Web;
using System.Web.UI;
using PitchPointsWeb.API;
using PitchPointsWeb.Models.API;
using System.Data.SqlClient;
using System.Data;

namespace PitchPointsWeb
{
    public partial class Log_A_Climb : Page
    {

        protected async void Page_Load(object sender, EventArgs e)
        {
            Master.ReadToken();

            string empty = "";
            var TokenModel = new TokenModel
            {
                Token = Master.ReadToken()
            };

            var loggedIn = await TokenModel.Validate();
            if (loggedIn)
            {
                getActiveCompetitions.SelectParameters["email"].DefaultValue = TokenModel.Content.Email;
                getActiveCompetitions.SelectParameters["onlyReturnRegistered"].DefaultValue = "1";
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string empty = "";
            /*if (climber_id.Value != empty && witness_id.Value != empty && route_id.Value != empty && falls.Value != empty)
            {
                var controller = new RouteController();
                var logClimbModel = new LoggedClimbModel
                {
                    ClimberId = Convert.ToInt32(climber_id.Value),
                    WitnessId = Convert.ToInt32(witness_id.Value),
                    RouteId = Convert.ToInt32(route_id.Value),
                    Falls = Convert.ToInt32(falls.Value),
                };
                var result = controller.LogClimb(logClimbModel);
                if (result.Success)
                {
                    climber_id.Value = empty; witness_id.Value = empty; route_id.Value = empty; falls.Value = empty;
                    ScriptManager.RegisterStartupScript(this, GetType(), "Popup", "success();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "Popup", "serverError();", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Popup", "completeForm();", true);
            }*/
        }
    }
}