<%@ Page Title="CompInfo" Language="C#" MasterPageFile="~/Site.Master" Async="true"  AutoEventWireup="true" CodeBehind="CompInfo.aspx.cs" Inherits="PitchPointsWeb.CompInfo" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        function completeForm() {
            swal({
                title: 'Error!',
                text: 'Please make sure you fill out each form on this page.',
                type: 'error'
            });
        }
        function serverError() {
            swal({
                title: 'Error!',
                text: 'There was an issue with your registration. Please try again.',
                type: 'error'
            });
        }
        function success() {
            swal({
                title: 'Success!',
                text: 'You have successfully registered!',
                type: 'success'
            });
        }
    </script>
    <div class="form-horizontal">
        <asp:ValidationSummary runat="server" CssClass="text-danger" />
        <div class="row">
            <div class="col s6">
                <h1>General</h1>
                <div class="row">
                    <div class="col s12 m7">
                        <div class="card">
                            <div class="card-image">
                                <img id="GymImage" runat="server" src="">
                            </div>
                            <div class="card-content">

                                <asp:GridView ID="CompetitionGridView" CssClass="bordered centered responsive-table" runat="server" AutoGenerateColumns="False" DataSourceID="CompetitionInfoDataSource" CellPadding="4" ForeColor="#333333" GridLines="Horizontal" RowStyle-HorizontalAlign="Center">
                                    <Columns>
                                        <asp:BoundField DataField="CompTitle"
                                            HeaderText="Comp Title"
                                            InsertVisible="False" ReadOnly="True"
                                            SortExpression="CompTitle" />
                                        <asp:BoundField DataField="CompDetails"
                                            HeaderText="Comp Details"
                                            SortExpression="CompDetails" />
                                        <asp:BoundField DataField="Description"
                                            HeaderText="Description"
                                            SortExpression="Description" />
                                    </Columns>
                                </asp:GridView>
                                <asp:SqlDataSource ID="CompetitionInfoDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:PitchPointsDB %>" SelectCommand="GetActiveCompetitions" SelectCommandType="StoredProcedure">
                                    <SelectParameters>
                                        <asp:Parameter DefaultValue=" " Name="email" Type="String" />
                                        <asp:Parameter DefaultValue="" Name="targetComp" Type="String" />
                         
                                    </SelectParameters>
                                </asp:SqlDataSource>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col s6">
                <h1>Location</h1>
                <asp:GridView ID="LocationView" RowStyle-HorizontalAlign="Center" CssClass="bordered centered responsive-table" runat="server" AutoGenerateColumns="False" DataSourceID="LocationDataSource" CellPadding="2" ForeColor="#333333" GridLines="Horizontal">
                    <Columns>
                        <asp:BoundField DataField="Nickname"
                            HeaderText="Gym name"
                            InsertVisible="False" ReadOnly="True"
                            SortExpression="Nickname" />
                        <asp:BoundField DataField="State"
                            HeaderText=" State "
                            SortExpression="State" />
                        <asp:BoundField DataField="City"
                            HeaderText=" City "
                            SortExpression="City" />
                        <asp:BoundField DataField="Zip"
                            HeaderText="  Zip  "
                            SortExpression="Zip" />
                        <asp:BoundField DataField="AddressLine1"
                            HeaderText="Address"
                            SortExpression="AddressLine1" />
                    </Columns>
                </asp:GridView>

                <asp:SqlDataSource ID="LocationDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:PitchPointsDB %>" SelectCommand="GetLocationInfo" SelectCommandType="StoredProcedure">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="" Name="comp" Type="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>

                <h1>Competition Rules</h1>
                <asp:GridView ID="RulesView" RowStyle-HorizontalAlign="Center" CssClass="bordered centered responsive-table" runat="server" AutoGenerateColumns="False" DataSourceID="RulesDataSource" CellPadding="2" ForeColor="#333333">
                    <Columns>
                        <asp:BoundField DataField="Description"
                            HeaderText="Description"
                            SortExpression="Description" />
                    </Columns>
                </asp:GridView>
                <asp:SqlDataSource ID="RulesDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:PitchPointsDB %>" SelectCommand="GetCompetitionRules" SelectCommandType="StoredProcedure">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="" Name="CompetitionID" Type="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </div>
        </div>

        <br />
        <asp:Button runat="server" ID="btnRegister" OnClick="btnRegister_Click" Text="Register" CssClass="btn btn-primary" />
        <asp:Button runat="server" ID="btnUnregister" OnClick="btnUnregister_Click" Text="Unregister" CssClass="btn btn-primary" />

        <br />
        <br />
        <asp:Label ID="ScoreCardLabel" runat="server" Text=""></asp:Label>
            <asp:GridView ID="ScoreCardGridView" RowStyle-HorizontalAlign="Center" CssClass="bordered centered highlight responsive-table" runat="server" AutoGenerateColumns="True" DataSourceID="ScoreCardDataSource" CellPadding="4" ForeColor="#333333" GridLines="Horizontal">
                
            </asp:GridView>
            <asp:SqlDataSource ID="ScoreCardDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:PitchPointsDB %>" SelectCommand="GetScoreCard" SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:Parameter DefaultValue="" Name="email" Type="String" />
                    <asp:Parameter DefaultValue="" Name="compId" Type="Int32" />
                </SelectParameters>
            </asp:SqlDataSource>

        <div class="form-group">
            <div class="col-md-10">
            </div>
        </div>
        <div class="col-md-offset-2 col-md-10">
        </div>
        <div>
            <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
        </div>
    </div>
</asp:Content>
