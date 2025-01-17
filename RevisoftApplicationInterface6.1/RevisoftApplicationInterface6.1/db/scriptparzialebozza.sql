USE [Revisoft_sscavoneDBNew_piccolo]
GO
/****** Object:  UserDefinedTableType [dbo].[UDTT_CassaAssegni]    Script Date: 28/02/2019 13:00:30 ******/
CREATE TYPE [dbo].[UDTT_CassaAssegni] AS TABLE(
	[ID_SCHEDA] [varchar](500) NULL,
	[name] [varchar](500) NULL,
	[codice] [varchar](500) NULL,
	[importoPagato] [numeric](18, 2) NULL,
	[importoCompensato] [numeric](18, 2) NULL,
	[txtfinder] [varchar](500) NULL,
	[PeriodoDiRiferimento] [varchar](500) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UDTT_CassaTitoli]    Script Date: 28/02/2019 13:00:31 ******/
CREATE TYPE [dbo].[UDTT_CassaTitoli] AS TABLE(
	[name] [varchar](500) NULL,
	[codice] [varchar](500) NULL,
	[importoPagato] [numeric](18, 2) NULL,
	[importoCompensato] [numeric](18, 2) NULL,
	[txtfinder] [varchar](500) NULL,
	[CreditoEsistente] [varchar](500) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UDTT_CassaValoriBollati_Francobolli]    Script Date: 28/02/2019 13:00:31 ******/
CREATE TYPE [dbo].[UDTT_CassaValoriBollati_Francobolli] AS TABLE(
	[ID_SCHEDA] [varchar](500) NULL,
	[numeropezzi] [int] NULL,
	[unitario] [numeric](18, 2) NULL,
	[euro] [numeric](18, 2) NULL,
	[txtfinder] [varchar](50) NULL,
	[CreditoEsistente] [varchar](500) NULL,
	[txtSaldoSchedaContabile] [numeric](18, 2) NULL,
	[txtTotaleComplessivo] [numeric](18, 2) NULL,
	[txtDifferenza] [numeric](18, 2) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UDTT_CassaValoriBollati_Marche]    Script Date: 28/02/2019 13:00:31 ******/
CREATE TYPE [dbo].[UDTT_CassaValoriBollati_Marche] AS TABLE(
	[ID_SCHEDA] [varchar](500) NULL,
	[numeropezzi] [int] NULL,
	[unitario] [numeric](18, 2) NULL,
	[euro] [numeric](18, 2) NULL,
	[txtfinder] [varchar](50) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UDTT_CompensiERisorse_CompensoRevisione]    Script Date: 28/02/2019 13:00:31 ******/
CREATE TYPE [dbo].[UDTT_CompensiERisorse_CompensoRevisione] AS TABLE(
	[ID_SCHEDA] [varchar](500) NULL,
	[fase] [varchar](500) NULL,
	[attivita] [varchar](500) NULL,
	[esecutore] [varchar](500) NULL,
	[ore] [numeric](18, 2) NULL,
	[termini] [varchar](500) NULL,
	[txtfinder] [varchar](500) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UDTT_CompensiERisorse_EsecutoriRevisione]    Script Date: 28/02/2019 13:00:31 ******/
CREATE TYPE [dbo].[UDTT_CompensiERisorse_EsecutoriRevisione] AS TABLE(
	[ID_SCHEDA] [varchar](500) NULL,
	[nome] [varchar](500) NULL,
	[qualifica] [varchar](500) NULL,
	[txtTotale] [numeric](18, 2) NULL,
	[txtTariffaOraria] [numeric](18, 2) NULL,
	[txtCompenso] [numeric](18, 2) NULL,
	[txtfinder] [varchar](500) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UDTT_CompensiERisorse_TerminiEsecuzione]    Script Date: 28/02/2019 13:00:31 ******/
CREATE TYPE [dbo].[UDTT_CompensiERisorse_TerminiEsecuzione] AS TABLE(
	[ID_SCHEDA] [varchar](500) NULL,
	[fase] [varchar](500) NULL,
	[attivita] [varchar](500) NULL,
	[termini] [varchar](500) NULL,
	[txtfinder] [varchar](500) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UDTT_Tabella]    Script Date: 28/02/2019 13:00:31 ******/
CREATE TYPE [dbo].[UDTT_Tabella] AS TABLE(
	[ID_SCHEDA] [varchar](50) NULL,
	[ID] [int] NULL,
	[name] [varchar](500) NULL,
	[value] [varchar](500) NULL,
	[tab] [varchar](500) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UDTT_Testi]    Script Date: 28/02/2019 13:00:31 ******/
CREATE TYPE [dbo].[UDTT_Testi] AS TABLE(
	[ID_SCHEDA] [varchar](500) NULL,
	[name] [varchar](500) NULL
)
GO
/****** Object:  StoredProcedure [dbo].[putDataTableCassaAssegni]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[putDataTableCassaAssegni]
@ID varchar(50),                                  -- ID scheda
@OBJDATATABLE dbo.UDTT_CassaAssegni READONLY

AS
BEGIN
SET NOCOUNT ON;
DELETE FROM CassaAssegni WHERE ID_SCHEDA=@ID
INSERT INTO  CassaAssegni
SELECT * FROM  @OBJDATATABLE   



END

GO
/****** Object:  StoredProcedure [dbo].[putDataTableCassaTitoli]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[putDataTableCassaTitoli]
@ID varchar(50),                                  -- ID scheda
@OBJDATATABLE dbo.UDTT_CassaTitoli READONLY

AS
BEGIN
SET NOCOUNT ON;
INSERT INTO  CassaTitoli 
SELECT  @ID AS IDSCHEDA,* FROM  @OBJDATATABLE   



END

GO
/****** Object:  StoredProcedure [dbo].[putDataTableCassaValoriBollati_Francobolli]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[putDataTableCassaValoriBollati_Francobolli]
@ID varchar(50),                                  -- ID scheda
@OBJDATATABLE dbo.UDTT_CassaValoriBollati_Francobolli READONLY

AS
BEGIN
SET NOCOUNT ON;
INSERT INTO  CassaValoriBollati_Francobolli 
SELECT * FROM  @OBJDATATABLE   



END
GO
/****** Object:  StoredProcedure [dbo].[putDataTableCassaValoriBollati_Marche]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[putDataTableCassaValoriBollati_Marche]
@ID varchar(50),                                  -- ID scheda
@OBJDATATABLE dbo.UDTT_CassaValoriBollati_Marche READONLY

AS
BEGIN
SET NOCOUNT ON;
INSERT INTO  CassaValoriBollati_Marche 
SELECT * FROM  @OBJDATATABLE   



END
GO
/****** Object:  StoredProcedure [dbo].[putDataTableCompensiERisorse_CompensoRevisione]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[putDataTableCompensiERisorse_CompensoRevisione]
@ID varchar(50),                                  -- ID scheda
@OBJDATATABLE dbo.UDTT_CompensiERisorse_CompensoRevisione READONLY

AS
BEGIN
SET NOCOUNT ON;
INSERT INTO  CompensiERisorse_CompensoRevisione 
SELECT * FROM  @OBJDATATABLE   

END


GO
/****** Object:  StoredProcedure [dbo].[putDataTableCompensiERisorse_EsecutoriRevisione]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[putDataTableCompensiERisorse_EsecutoriRevisione]
@ID varchar(50),                                  -- ID scheda
@OBJDATATABLE dbo.UDTT_CompensiERisorse_EsecutoriRevisione READONLY

AS
BEGIN
SET NOCOUNT ON;
INSERT INTO  CompensiERisorse_EsecutoriRevisione 
SELECT * FROM  @OBJDATATABLE   

END


GO
/****** Object:  StoredProcedure [dbo].[putDataTableCompensiERisorse_TerminiEsecuzione]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[putDataTableCompensiERisorse_TerminiEsecuzione]
@ID varchar(50),                                  -- ID scheda
@OBJDATATABLE dbo.UDTT_CompensiERisorse_TerminiEsecuzione READONLY

AS
BEGIN
SET NOCOUNT ON;
INSERT INTO  CompensiERisorse_TerminiEsecuzione 
SELECT * FROM  @OBJDATATABLE   

END


GO
/****** Object:  StoredProcedure [dbo].[putDataTableTabella]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE  [dbo].[putDataTableTabella]
@ID varchar(50),                               
@OBJDATATABLE dbo.UDTT_Tabella READONLY

AS
BEGIN
SET NOCOUNT ON;
DELETE FROM Tabella WHERE ID_SCHEDA=@ID
INSERT INTO  Tabella
SELECT * FROM  @OBJDATATABLE   

END
GO
/****** Object:  StoredProcedure [dbo].[putDataTableTesti]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[putDataTableTesti]
@ID varchar(50),                                  -- ID scheda
@OBJDATATABLE dbo.UDTT_Testi READONLY

AS
BEGIN
SET NOCOUNT ON;
DELETE FROM Testi WHERE ID_SCHEDA=@ID
INSERT INTO  Testi
SELECT * FROM  @OBJDATATABLE   
END

GO
/****** Object:  Table [dbo].[CassaAssegni]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CassaAssegni](
	[ID_SCHEDA] [varchar](50) NULL,
	[name] [varchar](500) NULL,
	[codice] [varchar](500) NULL,
	[importoPagato] [numeric](18, 2) NULL,
	[importoCompensato] [numeric](18, 2) NULL,
	[txtfinder] [varchar](500) NULL,
	[PeriodoDiRiferimento] [varchar](500) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CassaTitoli]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CassaTitoli](
	[ID] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[IDSCHEDA] [varchar](50) NULL,
	[name] [varchar](500) NULL,
	[codice] [varchar](500) NULL,
	[importoPagato] [numeric](18, 2) NULL,
	[importoCompensato] [numeric](18, 2) NULL,
	[txtfinder] [varchar](500) NULL,
	[CreditoEsistente] [varchar](500) NULL,
 CONSTRAINT [PK_CassaTitoli] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CassaValoriBollati_Francobolli]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CassaValoriBollati_Francobolli](
	[ID_SCHEDA] [varchar](50) NULL,
	[numeropezzi] [int] NULL,
	[unitario] [numeric](18, 2) NULL,
	[euro] [numeric](18, 2) NULL,
	[txtfinder] [varchar](50) NULL,
	[CreditoEsistente] [varchar](500) NULL,
	[txtSaldoSchedaContabile] [numeric](18, 2) NULL,
	[txtTotaleComplessivo] [numeric](18, 2) NULL,
	[txtDifferenza] [numeric](18, 2) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CassaValoriBollati_Marche]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CassaValoriBollati_Marche](
	[ID_SCHEDA] [varchar](50) NULL,
	[numeropezzi] [int] NULL,
	[unitario] [numeric](18, 2) NULL,
	[euro] [numeric](18, 2) NULL,
	[txtfinder] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CompensiERisorse_CompensoRevisione]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CompensiERisorse_CompensoRevisione](
	[ID_SCHEDA] [varchar](50) NULL,
	[fase] [varchar](50) NULL,
	[attivita] [varchar](500) NULL,
	[esecutore] [varchar](500) NULL,
	[ore] [numeric](18, 2) NULL,
	[termini] [varchar](50) NULL,
	[txtfinder] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CompensiERisorse_EsecutoriRevisione]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CompensiERisorse_EsecutoriRevisione](
	[ID_SCHEDA] [varchar](50) NULL,
	[nome] [varchar](500) NULL,
	[qualifica] [varchar](500) NULL,
	[txtTotale] [numeric](18, 2) NULL,
	[txtTariffaOraria] [numeric](18, 2) NULL,
	[txtCompenso] [numeric](18, 2) NULL,
	[txtfinder] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CompensiERisorse_TerminiEsecuzione]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CompensiERisorse_TerminiEsecuzione](
	[ID_SCHEDA] [varchar](50) NULL,
	[fase] [varchar](50) NULL,
	[attivita] [varchar](500) NULL,
	[termini] [varchar](50) NULL,
	[txtfinder] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Tabella]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tabella](
	[ID_SCHEDA] [varchar](50) NULL,
	[ID] [int] NULL,
	[name] [varchar](500) NULL,
	[value] [varchar](500) NULL,
	[tab] [varchar](500) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Testi]    Script Date: 28/02/2019 13:00:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Testi](
	[ID_SCHEDA] [varchar](50) NULL,
	[name] [varchar](500) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
