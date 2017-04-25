﻿<%@ Page Title="Profile" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="PitchPointsWeb.Account.Profile" Async="true" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1><%: Title %></h1>
    <div class="row">
        <div class="col s6">
            <div class="row">
                <div class="col s12 m7">
                    <div class="card">
                        <div class="card-content">
                            <h1>Info</h1>
                            <p></p>
                            <asp:Label ID="lblEmail" runat="server" Text="Email :  ">  </asp:Label><asp:Label ID="EmailLabel" runat="server" Text=""></asp:Label>
                            <p></p>
                            <asp:Label ID="lblFirstName" runat="server" Text="First :  "></asp:Label><asp:Label ID="FirstLabel" runat="server" Text="Label"></asp:Label>
                            <p></p>
                            <asp:Label ID="lblLastName" runat="server" Text="Last :  "></asp:Label><asp:Label ID="LastLabel" runat="server" Text="Label"></asp:Label>
                            <p></p>
                            <asp:Label ID="lblLifetimePts" runat="server" Text="Lifetime Points : "></asp:Label><asp:Label ID="LifeTimePointsLabel" runat="server" Text="Label"></asp:Label>
                            <p></p>
                            <asp:Label ID="lblFalls" runat="server" Text="Falls :  "></asp:Label><asp:Label ID="FallsLabel" runat="server" Text="Label"></asp:Label>
                            <p></p>
                            <asp:Label ID="lblParticipatedComps" runat="server" Text="Participated Comps : "></asp:Label><asp:Label ID="ParticipatedCompsLabel" runat="server" Text="Label"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col s6">
            <h1>Upcoming Competitions</h1>
            <asp:GridView ID="CompetitionsGridView" ItemStyle-HorizontalAlign="Center" CssClass="bordered centered highlight responsive-table" runat="server" AutoGenerateColumns="False" DataSourceID="UpCompDataSource" CellPadding="4" ForeColor="#333333" GridLines="Horizontal">
                <Columns>
                    <asp:BoundField DataField="CompTitle"
                        HeaderText="Competition Title"
                        InsertVisible="False" ReadOnly="True"
                        SortExpression="CompTitle" />
                    <asp:BoundField DataField="CompDetails"
                        HeaderText="Competition Details"
                        SortExpression="CompDetails" />
                    <asp:BoundField DataField="Description"
                        HeaderText="Description"
                        SortExpression="Description" />
                    <asp:BoundField DataField="Date"
                        HeaderText="Date"
                        SortExpression="Date" />
                    <asp:BoundField DataField="City"
                        HeaderText="City"
                        SortExpression="City" />
                    <asp:BoundField DataField="State"
                        HeaderText="State"
                        SortExpression="State" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="UpCompDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:PitchPointsDB %>" SelectCommand="GetActiveCompetitions" SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:Parameter DefaultValue="" Name="email" Type="String" />
                    <asp:Parameter DefaultValue="True" Name="onlyReturnRegistered" Type="Boolean" />
                </SelectParameters>
            </asp:SqlDataSource>
        </div>
    </div>
    <h1>Previous Competitions</h1>
    <asp:Label ID="Comp1Label" runat="server" Text=""></asp:Label>
    <asp:GridView ID="CompCompGridView" ItemStyle-HorizontalAlign="Center" CssClass="bordered centered highlight responsive-table" runat="server" AutoGenerateColumns="False" DataSourceID="CompCompDataSource" CellPadding="4" ForeColor="#333333" GridLines="Horizontal">
        <Columns>
            <asp:BoundField DataField="CompTitle"
                HeaderText="Competition Title"
                InsertVisible="False" ReadOnly="True"
                SortExpression="CompTitle" />
            <asp:BoundField DataField="Points"
                HeaderText="Points"
                SortExpression="Points" />
            <asp:BoundField DataField="Falls"
                HeaderText="Falls"
                SortExpression="Falls" />
            <asp:BoundField DataField="Rank"
                HeaderText="Rank"
                SortExpression="Rank" />
            <asp:TemplateField HeaderText="More Info">
                <ItemTemplate>
                    <asp:LinkButton ID="btnview" runat="server" href='<%# String.Format("/CompInfo.aspx?ID={0}", Eval("Id")) %>' Text="Logged Ticket" CommandName="More Info">More Info
                        </asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="CompCompDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:PitchPointsDB %>" SelectCommand="GetPastCompetitions" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter DefaultValue="" Name="email" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>
