USE R1SM_Track
GO

SET NUMERIC_ROUNDABORT OFF
GO
SET XACT_ABORT, ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO

BEGIN TRANSACTION
ALTER TABLE [dbo].[aspnet_Profile] DROP CONSTRAINT [FK__aspnet_Pr__UserI__7C4F7684]
ALTER TABLE [dbo].[aspnet_Paths] DROP CONSTRAINT [FK__aspnet_Pa__Appli__787EE5A0]
ALTER TABLE [dbo].[aspnet_UsersInRoles] DROP CONSTRAINT [FK__aspnet_Us__RoleI__7E37BEF6]
ALTER TABLE [dbo].[aspnet_UsersInRoles] DROP CONSTRAINT [FK__aspnet_Us__UserI__7F2BE32F]
ALTER TABLE [dbo].[aspnet_Membership] DROP CONSTRAINT [FK__aspnet_Me__Appli__76969D2E]
ALTER TABLE [dbo].[aspnet_Membership] DROP CONSTRAINT [FK__aspnet_Me__UserI__778AC167]
ALTER TABLE [dbo].[aspnet_PersonalizationAllUsers] DROP CONSTRAINT [FK__aspnet_Pe__PathI__797309D9]
ALTER TABLE [dbo].[aspnet_PersonalizationPerUser] DROP CONSTRAINT [FK__aspnet_Pe__PathI__7A672E12]
ALTER TABLE [dbo].[aspnet_PersonalizationPerUser] DROP CONSTRAINT [FK__aspnet_Pe__UserI__7B5B524B]
ALTER TABLE [dbo].[aspnet_Users] DROP CONSTRAINT [FK__aspnet_Us__Appli__7D439ABD]
INSERT INTO [dbo].[aspnet_Users] ([UserId], [ApplicationId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'7f4e6d55-ed83-4579-815c-ec320e7e5ed1', N'c3471f9e-18b3-4aad-8c9b-0a3a48b6cc9b', N'admin', N'admin', NULL, 0, '20110214 16:36:42.140')
INSERT INTO [dbo].[aspnet_Roles] ([RoleId], [ApplicationId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'093b5921-1001-4202-9da3-8b6f9a80c19d', N'c3471f9e-18b3-4aad-8c9b-0a3a48b6cc9b', N'Administrators', N'administrators', NULL)
INSERT INTO [dbo].[aspnet_Roles] ([RoleId], [ApplicationId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'd18afc93-d759-4fd8-b848-68d0bf849883', N'c3471f9e-18b3-4aad-8c9b-0a3a48b6cc9b', N'Users', N'users', NULL)
INSERT INTO [dbo].[aspnet_Applications] ([ApplicationId], [ApplicationName], [LoweredApplicationName], [Description]) VALUES (N'c3471f9e-18b3-4aad-8c9b-0a3a48b6cc9b', N'/', N'/', NULL)
INSERT INTO [dbo].[aspnet_Membership] ([UserId], [ApplicationId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'7f4e6d55-ed83-4579-815c-ec320e7e5ed1', N'c3471f9e-18b3-4aad-8c9b-0a3a48b6cc9b', N'TUSGi0nyt9vX9qbvbeOtMkTYoyQ=', 1, N'Te80QMBksL977lr66urhfA==', NULL, N'admin@rocs.com', N'admin@rocs.com', NULL, NULL, 1, 0, '20110214 16:14:03.000', '20110214 16:36:42.140', '20110214 16:14:03.000', '17540101 00:00:00.000', 0, '17540101 00:00:00.000', 0, '17540101 00:00:00.000', NULL)
INSERT INTO [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'7f4e6d55-ed83-4579-815c-ec320e7e5ed1', N'093b5921-1001-4202-9da3-8b6f9a80c19d')
INSERT INTO [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'7f4e6d55-ed83-4579-815c-ec320e7e5ed1', N'd18afc93-d759-4fd8-b848-68d0bf849883')
INSERT INTO [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'common', N'1', 1)
INSERT INTO [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'health monitoring', N'1', 1)
INSERT INTO [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'membership', N'1', 1)
INSERT INTO [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'personalization', N'1', 1)
INSERT INTO [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'profile', N'1', 1)
INSERT INTO [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'role manager', N'1', 1)
ALTER TABLE [dbo].[aspnet_Profile] ADD CONSTRAINT [FK__aspnet_Pr__UserI__7C4F7684] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
ALTER TABLE [dbo].[aspnet_Paths] ADD CONSTRAINT [FK__aspnet_Pa__Appli__787EE5A0] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
ALTER TABLE [dbo].[aspnet_UsersInRoles] ADD CONSTRAINT [FK__aspnet_Us__RoleI__7E37BEF6] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[aspnet_Roles] ([RoleId])
ALTER TABLE [dbo].[aspnet_UsersInRoles] ADD CONSTRAINT [FK__aspnet_Us__UserI__7F2BE32F] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
ALTER TABLE [dbo].[aspnet_Membership] ADD CONSTRAINT [FK__aspnet_Me__Appli__76969D2E] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
ALTER TABLE [dbo].[aspnet_Membership] ADD CONSTRAINT [FK__aspnet_Me__UserI__778AC167] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
ALTER TABLE [dbo].[aspnet_PersonalizationAllUsers] ADD CONSTRAINT [FK__aspnet_Pe__PathI__797309D9] FOREIGN KEY ([PathId]) REFERENCES [dbo].[aspnet_Paths] ([PathId])
ALTER TABLE [dbo].[aspnet_PersonalizationPerUser] ADD CONSTRAINT [FK__aspnet_Pe__PathI__7A672E12] FOREIGN KEY ([PathId]) REFERENCES [dbo].[aspnet_Paths] ([PathId])
ALTER TABLE [dbo].[aspnet_PersonalizationPerUser] ADD CONSTRAINT [FK__aspnet_Pe__UserI__7B5B524B] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
ALTER TABLE [dbo].[aspnet_Users] ADD CONSTRAINT [FK__aspnet_Us__Appli__7D439ABD] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
COMMIT TRANSACTION
