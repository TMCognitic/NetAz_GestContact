﻿CREATE TABLE [dbo].[Contact]
(
	[Id] INT NOT NULL IDENTITY,
	[Nom] NVARCHAR(50) NOT NULL,
	[Prenom] NVARCHAR(50) NOT NULL,
	[Email] NVARCHAR(384) NOT NULL,
	[Naissance] DATE NOT NULL,
	[Tel] NVARCHAR(20) NOT NULL, 
    CONSTRAINT [PK_Contact] PRIMARY KEY ([Id]), 
    CONSTRAINT [AK_Contact_Email] UNIQUE ([Email])
)