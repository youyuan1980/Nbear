use CaseTests
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_mtm2_UserRole_RoleID_mtm2_Role]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[mtm2_UserRole] DROP CONSTRAINT FK_mtm2_UserRole_RoleID_mtm2_Role
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_mtm2_UserRole_UserID_mtm2_User]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[mtm2_UserRole] DROP CONSTRAINT FK_mtm2_UserRole_UserID_mtm2_User
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_mtm_UserGroup_UserID_mtm_User]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[mtm_UserGroup] DROP CONSTRAINT FK_mtm_UserGroup_UserID_mtm_User
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_mtm_UserGroup_GroupID_mtm_Group]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[mtm_UserGroup] DROP CONSTRAINT FK_mtm_UserGroup_GroupID_mtm_Group
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_AgentUser_UserID_User]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[AgentUser] DROP CONSTRAINT FK_AgentUser_UserID_User
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_LocalUser_UserID_AgentUser]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[LocalUser] DROP CONSTRAINT FK_LocalUser_UserID_AgentUser
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_UserGroup_UserID_User]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[UserGroup] DROP CONSTRAINT FK_UserGroup_UserID_User
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_UserGroup_GroupID_Group]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[UserGroup] DROP CONSTRAINT FK_UserGroup_GroupID_Group
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_AgentUserDomain_UserID_AgentUser]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[AgentUserDomain] DROP CONSTRAINT FK_AgentUserDomain_UserID_AgentUser
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_AgentUserDomain_DomainID_Domain]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[AgentUserDomain] DROP CONSTRAINT FK_AgentUserDomain_DomainID_Domain
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Master_ID_MasterParent]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Master] DROP CONSTRAINT FK_Master_ID_MasterParent
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Detail_MasterID_Master]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Detail] DROP CONSTRAINT FK_Detail_MasterID_Master
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ProductInfo_CategoryId_CategoryInfo]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ProductInfo] DROP CONSTRAINT FK_ProductInfo_CategoryId_CategoryInfo
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_cms_ArticleStatistics_ItemId_cms_Statistics]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[cms_ArticleStatistics] DROP CONSTRAINT FK_cms_ArticleStatistics_ItemId_cms_Statistics
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_m_UserInGroups_UserID_m_User]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[m_UserInGroups] DROP CONSTRAINT FK_m_UserInGroups_UserID_m_User
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_m_UserInGroups_GroupID_m_Group]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[m_UserInGroups] DROP CONSTRAINT FK_m_UserInGroups_GroupID_m_Group
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_home_PostSorts_Id_home_Sorts]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[home_PostSorts] DROP CONSTRAINT FK_home_PostSorts_Id_home_Sorts
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_TempUser_ID_TempPerson]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[TempUser] DROP CONSTRAINT FK_TempUser_ID_TempPerson
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_TempLocalUser_ID_TempUser]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[TempLocalUser] DROP CONSTRAINT FK_TempLocalUser_ID_TempUser
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[vAgentUser]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[vAgentUser]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[vLocalUser]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[vLocalUser]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[vMaster]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[vMaster]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[vcms_ArticleStatistics]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[vcms_ArticleStatistics]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[vhome_PostSorts]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[vhome_PostSorts]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[vTempUser]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[vTempUser]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[vTempLocalUser]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[vTempLocalUser]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_mtm2_Role_mtm2_UserRole]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_mtm2_Role_mtm2_UserRole]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_mtm2_User_mtm2_UserRole]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_mtm2_User_mtm2_UserRole]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_mtm_User_mtm_UserGroup]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_mtm_User_mtm_UserGroup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_mtm_Group_mtm_UserGroup]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_mtm_Group_mtm_UserGroup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_User_UserGroup]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_User_UserGroup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_Group_UserGroup]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_Group_UserGroup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_vAgentUser_AgentUserDomain]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_vAgentUser_AgentUserDomain]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_Domain_AgentUserDomain]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_Domain_AgentUserDomain]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_m_User_m_UserInGroups]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_m_User_m_UserInGroups]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_m_Group_m_UserInGroups]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_m_Group_m_UserInGroups]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mtm2_Role]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[mtm2_Role]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mtm2_User]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[mtm2_User]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mtm2_UserRole]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[mtm2_UserRole]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mtm_User]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[mtm_User]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mtm_Group]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[mtm_Group]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[mtm_UserGroup]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[mtm_UserGroup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[User]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Group]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Group]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AgentUser]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[AgentUser]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[LocalUser]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[LocalUser]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserProfile]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[UserProfile]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserGroup]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[UserGroup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AgentUserDomain]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[AgentUserDomain]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Domain]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Domain]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Team]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Team]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[MasterParent]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[MasterParent]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Master]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Master]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Detail]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Detail]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CategoryInfo]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[CategoryInfo]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ProductInfo]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ProductInfo]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Category]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Category]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[cms_Articles]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[cms_Articles]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[cms_Statistics]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[cms_Statistics]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[cms_ArticleStatistics]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[cms_ArticleStatistics]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[cms_Channels]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[cms_Channels]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[nb_PageParts]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[nb_PageParts]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[m_User]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[m_User]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[m_Group]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[m_Group]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[m_UserInGroups]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[m_UserInGroups]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[home_Sorts]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[home_Sorts]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[home_PostSorts]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[home_PostSorts]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Orders]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Orders]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[OrderItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[OrderItem]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SampleEntityWithContract]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[SampleEntityWithContract]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[game_SiteNodes]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[game_SiteNodes]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[TempPerson]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[TempPerson]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[TempUser]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[TempUser]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[TempLocalUser]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[TempLocalUser]
GO

CREATE TABLE [dbo].[mtm2_Role] (
[Name] nvarchar(127) NULL,
[Describe] int NULL,
[ID] uniqueidentifier NOT NULL,
[FID] int IDENTITY (1, 1) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[mtm2_Role] WITH NOCHECK ADD
CONSTRAINT [PK_mtm2_Role] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE INDEX [FID] ON [dbo].[mtm2_Role]([FID]) ON [PRIMARY]
GO

CREATE TABLE [dbo].[mtm2_User] (
[ID] uniqueidentifier NOT NULL,
[FID] int IDENTITY (1, 1) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[mtm2_User] WITH NOCHECK ADD
CONSTRAINT [PK_mtm2_User] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[mtm2_UserRole] (
[RoleID] uniqueidentifier NOT NULL,
[UserID] uniqueidentifier NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[mtm2_UserRole] WITH NOCHECK ADD
CONSTRAINT [PK_mtm2_UserRole] PRIMARY KEY CLUSTERED
(
[RoleID],
[UserID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[mtm_User] (
[ID] int IDENTITY (1, 1) NOT NULL,
[Name] nvarchar(127) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[mtm_User] WITH NOCHECK ADD
CONSTRAINT [PK_mtm_User] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[mtm_Group] (
[ID] int IDENTITY (1, 1) NOT NULL,
[Name] nvarchar(127) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[mtm_Group] WITH NOCHECK ADD
CONSTRAINT [PK_mtm_Group] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[mtm_UserGroup] (
[UserID] int NOT NULL,
[GroupID] int NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[mtm_UserGroup] WITH NOCHECK ADD
CONSTRAINT [PK_mtm_UserGroup] PRIMARY KEY CLUSTERED
(
[UserID],
[GroupID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[User] (
[Name] ntext NULL,
[Status] int NOT NULL,
[UserID] uniqueidentifier NOT NULL,
[TeamID] uniqueidentifier NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[User] WITH NOCHECK ADD
CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED
(
[UserID]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[User] ADD
	CONSTRAINT [DF_User_Status] DEFAULT (1) FOR [Status]
GO

CREATE INDEX [Status] ON [dbo].[User]([Status] DESC) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Group] (
[GroupID] uniqueidentifier NOT NULL,
[Name] nvarchar(50) NULL,
[IsPublic] bit NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Group] WITH NOCHECK ADD
CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED
(
[GroupID]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Group] ADD
	CONSTRAINT [DF_Group_Name] DEFAULT ('default group name') FOR [Name]
GO

CREATE TABLE [dbo].[AgentUser] (
[UserID] uniqueidentifier NOT NULL,
[LoginName] nvarchar(127) NULL
) ON [PRIMARY]
GO

CREATE VIEW [dbo].[vAgentUser]
AS
SELECT [dbo].[AgentUser].[LoginName], [dbo].[User].[Name], [dbo].[User].[Status], [dbo].[User].[UserID], [dbo].[User].[TeamID]
FROM [dbo].[AgentUser] INNER JOIN [dbo].[User] ON [dbo].[AgentUser].[UserID] = [dbo].[User].[UserID]
GO

ALTER TABLE [dbo].[AgentUser] WITH NOCHECK ADD
CONSTRAINT [PK_AgentUser] PRIMARY KEY CLUSTERED
(
[UserID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[LocalUser] (
[UserID] uniqueidentifier NOT NULL,
[Password] nvarchar(127) NOT NULL
) ON [PRIMARY]
GO

CREATE VIEW [dbo].[vLocalUser]
AS
SELECT [dbo].[LocalUser].[Password], [dbo].[AgentUser].[LoginName], [dbo].[User].[Name], [dbo].[User].[Status], [dbo].[User].[UserID], [dbo].[User].[TeamID]
FROM [dbo].[LocalUser] INNER JOIN [dbo].[AgentUser] ON [dbo].[LocalUser].[UserID] = [dbo].[AgentUser].[UserID] INNER JOIN [dbo].[User] ON [dbo].[AgentUser].[UserID] = [dbo].[User].[UserID]
GO

ALTER TABLE [dbo].[LocalUser] WITH NOCHECK ADD
CONSTRAINT [PK_LocalUser] PRIMARY KEY CLUSTERED
(
[UserID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[UserProfile] (
[UserID] uniqueidentifier NOT NULL,
[ContentXml] nvarchar(127) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserProfile] WITH NOCHECK ADD
CONSTRAINT [PK_UserProfile] PRIMARY KEY CLUSTERED
(
[UserID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[UserGroup] (
[UserID] uniqueidentifier NOT NULL,
[GroupID] uniqueidentifier NOT NULL,
[Weight] int NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserGroup] WITH NOCHECK ADD
CONSTRAINT [PK_UserGroup] PRIMARY KEY CLUSTERED
(
[UserID],
[GroupID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AgentUserDomain] (
[UserID] uniqueidentifier NOT NULL,
[DomainID] uniqueidentifier NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AgentUserDomain] WITH NOCHECK ADD
CONSTRAINT [PK_AgentUserDomain] PRIMARY KEY CLUSTERED
(
[UserID],
[DomainID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Domain] (
[ID] uniqueidentifier NOT NULL,
[Name] nvarchar(127) NULL,
[Desc] nvarchar(127) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Domain] WITH NOCHECK ADD
CONSTRAINT [PK_Domain] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Team] (
[ID] uniqueidentifier NOT NULL,
[Name] nvarchar(127) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Team] WITH NOCHECK ADD
CONSTRAINT [PK_Team] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[MasterParent] (
[ID] int IDENTITY (1, 1) NOT NULL,
[Name] nvarchar(127) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MasterParent] WITH NOCHECK ADD
CONSTRAINT [PK_MasterParent] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Master] (
[ID] int NOT NULL,
[OtherData] nvarchar(127) NULL,
[IntProperty] int NULL,
[DecimalProperty] decimal NULL,
[GuidProperty] uniqueidentifier NULL
) ON [PRIMARY]
GO

CREATE VIEW [dbo].[vMaster]
AS
SELECT [dbo].[Master].[OtherData], [dbo].[Master].[IntProperty], [dbo].[Master].[DecimalProperty], [dbo].[Master].[GuidProperty], [dbo].[MasterParent].[ID], [dbo].[MasterParent].[Name]
FROM [dbo].[Master] INNER JOIN [dbo].[MasterParent] ON [dbo].[Master].[ID] = [dbo].[MasterParent].[ID]
GO

ALTER TABLE [dbo].[Master] WITH NOCHECK ADD
CONSTRAINT [PK_Master] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Detail] (
[ID] int IDENTITY (1, 1) NOT NULL,
[Name] nvarchar(127) NULL,
[MasterID] int NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Detail] WITH NOCHECK ADD
CONSTRAINT [PK_Detail] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[CategoryInfo] (
[CategoryId] varchar(10) NOT NULL,
[Name] varchar(80) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CategoryInfo] WITH NOCHECK ADD
CONSTRAINT [PK_CategoryInfo] PRIMARY KEY CLUSTERED
(
[CategoryId]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ProductInfo] (
[ProductId] varchar(10) NOT NULL,
[CategoryId] varchar(10) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProductInfo] WITH NOCHECK ADD
CONSTRAINT [PK_ProductInfo] PRIMARY KEY CLUSTERED
(
[ProductId]
) ON [PRIMARY]
GO

CREATE INDEX [CategoryId] ON [dbo].[ProductInfo]([CategoryId]) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Category] (
[ParentID] bigint NULL,
[ID] bigint IDENTITY (1, 1) NOT NULL,
[Name] nvarchar(127) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Category] WITH NOCHECK ADD
CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[cms_Articles] (
[Id] int IDENTITY (1, 1) NOT NULL,
[ChannelId] int NULL,
[Editor] nvarchar(64) NULL,
[Author] nvarchar(64) NULL,
[Source] nvarchar(256) NULL,
[Picture] nvarchar(256) NULL,
[Title] nvarchar(256) NULL,
[Body] ntext NULL,
[UpdateTime] datetime NULL,
[CreateTime] datetime NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[cms_Articles] WITH NOCHECK ADD
CONSTRAINT [PK_cms_Articles] PRIMARY KEY CLUSTERED
(
[Id]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[cms_Statistics] (
[ItemId] int NOT NULL,
[Day] int NULL,
[DayClick] int NULL,
[Week] int NULL,
[WeekClick] int NULL,
[Month] int NULL,
[MonthClick] int NULL,
[Year] int NULL,
[YearClick] int NULL,
[TotalClick] int NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[cms_Statistics] WITH NOCHECK ADD
CONSTRAINT [PK_cms_Statistics] PRIMARY KEY CLUSTERED
(
[ItemId]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[cms_ArticleStatistics] (
[ItemId] int NOT NULL
) ON [PRIMARY]
GO

CREATE VIEW [dbo].[vcms_ArticleStatistics]
AS
SELECT [dbo].[cms_Statistics].[ItemId], [dbo].[cms_Statistics].[Day], [dbo].[cms_Statistics].[DayClick], [dbo].[cms_Statistics].[Week], [dbo].[cms_Statistics].[WeekClick], [dbo].[cms_Statistics].[Month], [dbo].[cms_Statistics].[MonthClick], [dbo].[cms_Statistics].[Year], [dbo].[cms_Statistics].[YearClick], [dbo].[cms_Statistics].[TotalClick]
FROM [dbo].[cms_ArticleStatistics] INNER JOIN [dbo].[cms_Statistics] ON [dbo].[cms_ArticleStatistics].[ItemId] = [dbo].[cms_Statistics].[ItemId]
GO

ALTER TABLE [dbo].[cms_ArticleStatistics] WITH NOCHECK ADD
CONSTRAINT [PK_cms_ArticleStatistics] PRIMARY KEY CLUSTERED
(
[ItemId]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[cms_Channels] (
[Id] int IDENTITY (1, 1) NOT NULL,
[ParentId] int NULL,
[OrderNum] int NULL,
[Depth] int NULL,
[Dir] nvarchar(64) NULL,
[Title] nvarchar(128) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[cms_Channels] WITH NOCHECK ADD
CONSTRAINT [PK_cms_Channels] PRIMARY KEY CLUSTERED
(
[Id]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[nb_PageParts] (
[Id] int IDENTITY (1, 1) NOT NULL,
[Title] nvarchar(64) NULL,
[Body] ntext NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[nb_PageParts] WITH NOCHECK ADD
CONSTRAINT [PK_nb_PageParts] PRIMARY KEY CLUSTERED
(
[Id]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[m_User] (
[ID] int IDENTITY (1, 1) NOT NULL,
[Name] nvarchar(127) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[m_User] WITH NOCHECK ADD
CONSTRAINT [PK_m_User] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[m_Group] (
[ID] int IDENTITY (1, 1) NOT NULL,
[Name] nvarchar(127) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[m_Group] WITH NOCHECK ADD
CONSTRAINT [PK_m_Group] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[m_UserInGroups] (
[UserID] int NOT NULL,
[GroupID] int NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[m_UserInGroups] WITH NOCHECK ADD
CONSTRAINT [PK_m_UserInGroups] PRIMARY KEY CLUSTERED
(
[UserID],
[GroupID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[home_Sorts] (
[Id] int IDENTITY (1, 1) NOT NULL,
[OrderNum] int NULL,
[Title] nvarchar(32) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[home_Sorts] WITH NOCHECK ADD
CONSTRAINT [PK_home_Sorts] PRIMARY KEY CLUSTERED
(
[Id]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[home_PostSorts] (
[Id] int NOT NULL,
[ItemAmount] int NULL
) ON [PRIMARY]
GO

CREATE VIEW [dbo].[vhome_PostSorts]
AS
SELECT [dbo].[home_PostSorts].[ItemAmount], [dbo].[home_Sorts].[Id], [dbo].[home_Sorts].[OrderNum], [dbo].[home_Sorts].[Title]
FROM [dbo].[home_PostSorts] INNER JOIN [dbo].[home_Sorts] ON [dbo].[home_PostSorts].[Id] = [dbo].[home_Sorts].[Id]
GO

ALTER TABLE [dbo].[home_PostSorts] WITH NOCHECK ADD
CONSTRAINT [PK_home_PostSorts] PRIMARY KEY CLUSTERED
(
[Id]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Orders] (
[ID] int IDENTITY (1, 1) NOT NULL,
[Name] nvarchar(127) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Orders] WITH NOCHECK ADD
CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[OrderItem] (
[ID] int IDENTITY (1, 1) NOT NULL,
[Name] nvarchar(127) NULL,
[Order_ID] int NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[OrderItem] WITH NOCHECK ADD
CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SampleEntityWithContract] (
[ID] int IDENTITY (1, 1) NOT NULL,
[Name] nvarchar(127) NULL,
[Parent_ID] int NULL,
[Address] nvarchar(127) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SampleEntityWithContract] WITH NOCHECK ADD
CONSTRAINT [PK_SampleEntityWithContract] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[game_SiteNodes] (
[Id] int IDENTITY (1, 1) NOT NULL,
[OrderNum] int NULL,
[Parent_Id] int NULL,
[IsDisplay] bit NULL,
[Title] nvarchar(256) NULL,
[SubTitle] nvarchar(256) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[game_SiteNodes] WITH NOCHECK ADD
CONSTRAINT [PK_game_SiteNodes] PRIMARY KEY CLUSTERED
(
[Id]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TempPerson] (
[BaseID] nvarchar(127) NULL,
[ID] uniqueidentifier NOT NULL,
[C1] nvarchar(127) NULL,
[Name] nvarchar(127) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TempPerson] WITH NOCHECK ADD
CONSTRAINT [PK_TempPerson] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TempUser] (
[ID] uniqueidentifier NOT NULL,
[Email] nvarchar(127) NULL
) ON [PRIMARY]
GO

CREATE VIEW [dbo].[vTempUser]
AS
SELECT [dbo].[TempUser].[Email], [dbo].[TempPerson].[BaseID], [dbo].[TempPerson].[ID], [dbo].[TempPerson].[C1], [dbo].[TempPerson].[Name]
FROM [dbo].[TempUser] INNER JOIN [dbo].[TempPerson] ON [dbo].[TempUser].[ID] = [dbo].[TempPerson].[ID]
GO

ALTER TABLE [dbo].[TempUser] WITH NOCHECK ADD
CONSTRAINT [PK_TempUser] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TempLocalUser] (
[ID] uniqueidentifier NOT NULL,
[LoginID] nvarchar(127) NULL
) ON [PRIMARY]
GO

CREATE VIEW [dbo].[vTempLocalUser]
AS
SELECT [dbo].[TempLocalUser].[LoginID], [dbo].[TempUser].[Email], [dbo].[TempPerson].[BaseID], [dbo].[TempPerson].[ID], [dbo].[TempPerson].[C1], [dbo].[TempPerson].[Name]
FROM [dbo].[TempLocalUser] INNER JOIN [dbo].[TempUser] ON [dbo].[TempLocalUser].[ID] = [dbo].[TempUser].[ID] INNER JOIN [dbo].[TempPerson] ON [dbo].[TempUser].[ID] = [dbo].[TempPerson].[ID]
GO

ALTER TABLE [dbo].[TempLocalUser] WITH NOCHECK ADD
CONSTRAINT [PK_TempLocalUser] PRIMARY KEY CLUSTERED
(
[ID]
) ON [PRIMARY]
GO

CREATE VIEW [dbo].[v_mtm2_Role_mtm2_UserRole]
AS
SELECT [dbo].[mtm2_UserRole].[RoleID] AS [mtm2_UserRole_RoleID], [dbo].[mtm2_UserRole].[UserID] AS [mtm2_UserRole_UserID], [dbo].[mtm2_Role].[Name], [dbo].[mtm2_Role].[Describe], [dbo].[mtm2_Role].[ID], [dbo].[mtm2_Role].[FID]
FROM [dbo].[mtm2_UserRole] INNER JOIN [dbo].[mtm2_Role] ON [dbo].[mtm2_UserRole].[RoleID] = [dbo].[mtm2_Role].[ID]
GO

CREATE VIEW [dbo].[v_mtm2_User_mtm2_UserRole]
AS
SELECT [dbo].[mtm2_UserRole].[RoleID] AS [mtm2_UserRole_RoleID], [dbo].[mtm2_UserRole].[UserID] AS [mtm2_UserRole_UserID], [dbo].[mtm2_User].[ID], [dbo].[mtm2_User].[FID]
FROM [dbo].[mtm2_UserRole] INNER JOIN [dbo].[mtm2_User] ON [dbo].[mtm2_UserRole].[UserID] = [dbo].[mtm2_User].[ID]
GO

CREATE VIEW [dbo].[v_mtm_User_mtm_UserGroup]
AS
SELECT [dbo].[mtm_UserGroup].[UserID] AS [mtm_UserGroup_UserID], [dbo].[mtm_UserGroup].[GroupID] AS [mtm_UserGroup_GroupID], [dbo].[mtm_User].[ID], [dbo].[mtm_User].[Name]
FROM [dbo].[mtm_UserGroup] INNER JOIN [dbo].[mtm_User] ON [dbo].[mtm_UserGroup].[UserID] = [dbo].[mtm_User].[ID]
GO

CREATE VIEW [dbo].[v_mtm_Group_mtm_UserGroup]
AS
SELECT [dbo].[mtm_UserGroup].[UserID] AS [mtm_UserGroup_UserID], [dbo].[mtm_UserGroup].[GroupID] AS [mtm_UserGroup_GroupID], [dbo].[mtm_Group].[ID], [dbo].[mtm_Group].[Name]
FROM [dbo].[mtm_UserGroup] INNER JOIN [dbo].[mtm_Group] ON [dbo].[mtm_UserGroup].[GroupID] = [dbo].[mtm_Group].[ID]
GO

CREATE VIEW [dbo].[v_User_UserGroup]
AS
SELECT [dbo].[UserGroup].[UserID] AS [UserGroup_UserID], [dbo].[UserGroup].[GroupID] AS [UserGroup_GroupID], [dbo].[UserGroup].[Weight], [dbo].[User].[Name], [dbo].[User].[Status], [dbo].[User].[UserID], [dbo].[User].[TeamID]
FROM [dbo].[UserGroup] INNER JOIN [dbo].[User] ON [dbo].[UserGroup].[UserID] = [dbo].[User].[UserID]
GO

CREATE VIEW [dbo].[v_Group_UserGroup]
AS
SELECT [dbo].[UserGroup].[UserID] AS [UserGroup_UserID], [dbo].[UserGroup].[GroupID] AS [UserGroup_GroupID], [dbo].[UserGroup].[Weight], [dbo].[Group].[GroupID], [dbo].[Group].[Name], [dbo].[Group].[IsPublic]
FROM [dbo].[UserGroup] INNER JOIN [dbo].[Group] ON [dbo].[UserGroup].[GroupID] = [dbo].[Group].[GroupID]
GO

CREATE VIEW [dbo].[v_vAgentUser_AgentUserDomain]
AS
SELECT [dbo].[AgentUserDomain].[UserID] AS [AgentUserDomain_UserID], [dbo].[AgentUserDomain].[DomainID] AS [AgentUserDomain_DomainID], [dbo].[vAgentUser].[Name], [dbo].[vAgentUser].[Status], [dbo].[vAgentUser].[UserID], [dbo].[vAgentUser].[TeamID], [dbo].[vAgentUser].[LoginName]
FROM [dbo].[AgentUserDomain] INNER JOIN [dbo].[vAgentUser] ON [dbo].[AgentUserDomain].[UserID] = [dbo].[vAgentUser].[UserID]
GO

CREATE VIEW [dbo].[v_Domain_AgentUserDomain]
AS
SELECT [dbo].[AgentUserDomain].[UserID] AS [AgentUserDomain_UserID], [dbo].[AgentUserDomain].[DomainID] AS [AgentUserDomain_DomainID], [dbo].[Domain].[ID], [dbo].[Domain].[Name], [dbo].[Domain].[Desc]
FROM [dbo].[AgentUserDomain] INNER JOIN [dbo].[Domain] ON [dbo].[AgentUserDomain].[DomainID] = [dbo].[Domain].[ID]
GO

CREATE VIEW [dbo].[v_m_User_m_UserInGroups]
AS
SELECT [dbo].[m_UserInGroups].[UserID] AS [m_UserInGroups_UserID], [dbo].[m_UserInGroups].[GroupID] AS [m_UserInGroups_GroupID], [dbo].[m_User].[ID], [dbo].[m_User].[Name]
FROM [dbo].[m_UserInGroups] INNER JOIN [dbo].[m_User] ON [dbo].[m_UserInGroups].[UserID] = [dbo].[m_User].[ID]
GO

CREATE VIEW [dbo].[v_m_Group_m_UserInGroups]
AS
SELECT [dbo].[m_UserInGroups].[UserID] AS [m_UserInGroups_UserID], [dbo].[m_UserInGroups].[GroupID] AS [m_UserInGroups_GroupID], [dbo].[m_Group].[ID], [dbo].[m_Group].[Name]
FROM [dbo].[m_UserInGroups] INNER JOIN [dbo].[m_Group] ON [dbo].[m_UserInGroups].[GroupID] = [dbo].[m_Group].[ID]
GO

ALTER TABLE [dbo].[mtm2_UserRole] ADD CONSTRAINT [FK_mtm2_UserRole_RoleID_mtm2_Role] FOREIGN KEY ( [RoleID] ) REFERENCES [dbo].[mtm2_Role] ( [ID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[mtm2_UserRole] ADD CONSTRAINT [FK_mtm2_UserRole_UserID_mtm2_User] FOREIGN KEY ( [UserID] ) REFERENCES [dbo].[mtm2_User] ( [ID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[mtm_UserGroup] ADD CONSTRAINT [FK_mtm_UserGroup_UserID_mtm_User] FOREIGN KEY ( [UserID] ) REFERENCES [dbo].[mtm_User] ( [ID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[mtm_UserGroup] ADD CONSTRAINT [FK_mtm_UserGroup_GroupID_mtm_Group] FOREIGN KEY ( [GroupID] ) REFERENCES [dbo].[mtm_Group] ( [ID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[AgentUser] ADD CONSTRAINT [FK_AgentUser_UserID_User] FOREIGN KEY ( [UserID] ) REFERENCES [dbo].[User] ( [UserID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[LocalUser] ADD CONSTRAINT [FK_LocalUser_UserID_AgentUser] FOREIGN KEY ( [UserID] ) REFERENCES [dbo].[AgentUser] ( [UserID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[UserGroup] ADD CONSTRAINT [FK_UserGroup_UserID_User] FOREIGN KEY ( [UserID] ) REFERENCES [dbo].[User] ( [UserID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[UserGroup] ADD CONSTRAINT [FK_UserGroup_GroupID_Group] FOREIGN KEY ( [GroupID] ) REFERENCES [dbo].[Group] ( [GroupID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[AgentUserDomain] ADD CONSTRAINT [FK_AgentUserDomain_UserID_AgentUser] FOREIGN KEY ( [UserID] ) REFERENCES [dbo].[AgentUser] ( [UserID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[AgentUserDomain] ADD CONSTRAINT [FK_AgentUserDomain_DomainID_Domain] FOREIGN KEY ( [DomainID] ) REFERENCES [dbo].[Domain] ( [ID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[Master] ADD CONSTRAINT [FK_Master_ID_MasterParent] FOREIGN KEY ( [ID] ) REFERENCES [dbo].[MasterParent] ( [ID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[Detail] ADD CONSTRAINT [FK_Detail_MasterID_Master] FOREIGN KEY ( [MasterID] ) REFERENCES [dbo].[Master] ( [ID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[ProductInfo] ADD CONSTRAINT [FK_ProductInfo_CategoryId_CategoryInfo] FOREIGN KEY ( [CategoryId] ) REFERENCES [dbo].[CategoryInfo] ( [CategoryId] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[cms_ArticleStatistics] ADD CONSTRAINT [FK_cms_ArticleStatistics_ItemId_cms_Statistics] FOREIGN KEY ( [ItemId] ) REFERENCES [dbo].[cms_Statistics] ( [ItemId] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[m_UserInGroups] ADD CONSTRAINT [FK_m_UserInGroups_UserID_m_User] FOREIGN KEY ( [UserID] ) REFERENCES [dbo].[m_User] ( [ID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[m_UserInGroups] ADD CONSTRAINT [FK_m_UserInGroups_GroupID_m_Group] FOREIGN KEY ( [GroupID] ) REFERENCES [dbo].[m_Group] ( [ID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[home_PostSorts] ADD CONSTRAINT [FK_home_PostSorts_Id_home_Sorts] FOREIGN KEY ( [Id] ) REFERENCES [dbo].[home_Sorts] ( [Id] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[TempUser] ADD CONSTRAINT [FK_TempUser_ID_TempPerson] FOREIGN KEY ( [ID] ) REFERENCES [dbo].[TempPerson] ( [ID] ) NOT FOR REPLICATION
GO

ALTER TABLE [dbo].[TempLocalUser] ADD CONSTRAINT [FK_TempLocalUser_ID_TempUser] FOREIGN KEY ( [ID] ) REFERENCES [dbo].[TempUser] ( [ID] ) NOT FOR REPLICATION
GO

