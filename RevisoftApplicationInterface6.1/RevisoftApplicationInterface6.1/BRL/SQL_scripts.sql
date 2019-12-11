CREATE TABLE RUOLI (
RUO_ID smallint not null,
RUO_DESCR varchar(50) not null,
 CONSTRAINT [PK_RUOLI] PRIMARY KEY CLUSTERED 
(
	RUO_ID ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO RUOLI VALUES (0,'Da assegnare al TEAM')
GO
INSERT INTO RUOLI VALUES (1,'ADMINISTRATOR') 
GO
INSERT INTO RUOLI VALUES (2,'TEAM LEADER') 
GO
INSERT INTO RUOLI VALUES (3,'REVIEWER') 
GO
INSERT INTO RUOLI VALUES (4,'ESECUTORE') 
GO
INSERT INTO RUOLI VALUES (5,'STANDALONE')
GO
INSERT INTO RUOLI VALUES (6,'REVISORE AUTONOMO')
GO


CREATE TABLE UTENTI (
UTE_ID int identity not null,
UTE_RUO_ID smallint not null,
UTE_LOGIN varchar(50) not null,
UTE_PSW varchar(250) not null,
UTE_NOME varchar (50),
UTE_COGNOME varchar(50),
UTE_DESCR varchar(100) null,
UTE_INIZIO_VAL datetime,
UTE_FINE_VAL datetime,
UTE_UTE_ID int not null default -1,
 CONSTRAINT [PK_UTENTI] PRIMARY KEY CLUSTERED 
(
	UTE_ID ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE UTENTI  WITH CHECK ADD  CONSTRAINT [FK_UTENTI_RUOLI] FOREIGN KEY([UTE_RUO_ID])
REFERENCES [dbo].[RUOLI] ([RUO_ID])
GO

ALTER TABLE UTENTI CHECK CONSTRAINT [FK_UTENTI_RUOLI]
GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20190114-162552] ON [dbo].[UTENTI]
(
	[UTE_RUO_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


insert into UTENTI (UTE_RUO_ID,UTE_LOGIN,UTE_PSW,UTE_DESCR,UTE_UTE_ID) VALUES (1,'admin','lPO10bsBu5lfLEy/nPJHjg==','utente amministratore',0)
go


CREATE TABLE [dbo].[UTENTIXCLIENTE](
	UXC_ID int IDENTITY(1,1) NOT NULL,
	UXC_UTE_ID int NOT NULL,
	UXC_CLI_ID varchar(10) NOT NULL,
	UXC_REV_ID int null default(-1),
	UXC_REV_AUTO bit null default(0),
CONSTRAINT [PK_TEAM] PRIMARY KEY CLUSTERED 
(
	UXC_ID ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [UTENTIXCLIENTE]  WITH CHECK ADD  CONSTRAINT [FK_UTENTIXCLIENTE_UTENTI] FOREIGN KEY(UXC_UTE_ID)
REFERENCES [dbo].[UTENTI] ([UTE_ID])
GO

ALTER TABLE [UTENTIXCLIENTE] CHECK CONSTRAINT [FK_UTENTIXCLIENTE_UTENTI]
GO

ALTER TABLE [UTENTIXCLIENTE]  WITH CHECK ADD  CONSTRAINT [FK_UTENTIXCLIENTE_CLIENTI] FOREIGN KEY([UXC_CLI_ID])
REFERENCES mf.Cliente ([ID])
GO

CREATE TABLE CARTELLEXCLIENTE (
CXC_ID int identity not null,
CXC_COD_ID VARCHAR(20) NOT NULL,
CXC_UXC_ID int NOT NULL,
 CONSTRAINT PK_CARTELLEXCLIENTE PRIMARY KEY CLUSTERED 
(
	CXC_ID ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [NonClusteredIndex-20190114-161259] ON [dbo].[UTENTIXCLIENTE]
(
	[UXC_UTE_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20190114-162100] ON [dbo].[UTENTIXCLIENTE]
(
	[UXC_CLI_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20190114-162115] ON [dbo].[UTENTIXCLIENTE]
(
	[UXC_REV_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


ALTER TABLE CARTELLEXCLIENTE  WITH CHECK ADD  CONSTRAINT [FK_CARTELLEXCLIENTE_CODICI] FOREIGN KEY(CXC_COD_ID)
REFERENCES [dbo].RemapTreeNodeCodici (CODICE)
GO

ALTER TABLE CARTELLEXCLIENTE CHECK CONSTRAINT [FK_CARTELLEXCLIENTE_CODICI]
GO

ALTER TABLE CARTELLEXCLIENTE  WITH CHECK ADD  CONSTRAINT [FK_CARTELLEXCLIENTE_UTENTE] FOREIGN KEY(CXC_UXC_ID)
REFERENCES [dbo].UTENTIXCLIENTE (UXC_ID)
GO

ALTER TABLE CARTELLEXCLIENTE CHECK CONSTRAINT [FK_CARTELLEXCLIENTE_UTENTE]
GO

CREATE TABLE [dbo].[NOTEXREVISORE](
	[NXR_ID] [int] IDENTITY(1,1) NOT NULL,
	[NXR_NOTE] [varchar](max) NULL,
	[NXR_UTE_ID] [int] NOT NULL,
	[NXR_CLI_ID] [varchar](10) NOT NULL,
	[NXR_COD_ID] [varchar](20) NOT NULL,
 CONSTRAINT [PK_NOTEXREVISORE] PRIMARY KEY CLUSTERED 
(
	[NXR_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[NOTEXREVISORE]  WITH CHECK ADD  CONSTRAINT [FK_NOTEXREVISORE_Cliente] FOREIGN KEY([NXR_CLI_ID])
REFERENCES [mf].[Cliente] ([ID])
GO

ALTER TABLE [dbo].[NOTEXREVISORE] CHECK CONSTRAINT [FK_NOTEXREVISORE_Cliente]
GO

ALTER TABLE [dbo].[NOTEXREVISORE]  WITH CHECK ADD  CONSTRAINT [FK_NOTEXREVISORE_RemapTreeNodeCodici] FOREIGN KEY([NXR_COD_ID])
REFERENCES [dbo].[RemapTreeNodeCodici] ([Codice])
GO

ALTER TABLE [dbo].[NOTEXREVISORE] CHECK CONSTRAINT [FK_NOTEXREVISORE_RemapTreeNodeCodici]
GO

ALTER TABLE [dbo].[NOTEXREVISORE]  WITH CHECK ADD  CONSTRAINT [FK_NOTEXREVISORE_UTENTI] FOREIGN KEY([NXR_UTE_ID])
REFERENCES [dbo].[UTENTI] ([UTE_ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[NOTEXREVISORE] CHECK CONSTRAINT [FK_NOTEXREVISORE_UTENTI]
GO

CREATE NONCLUSTERED INDEX [NonClusteredIndex-20190114-162825] ON [dbo].[CARTELLEXCLIENTE]
(
	[CXC_COD_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [NonClusteredIndex-20190114-162840] ON [dbo].[CARTELLEXCLIENTE]
(
	[CXC_UXC_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE TABLE [dbo].[CARTELLE_BLOCCATE](
	[CBL_ID] [int] IDENTITY(1,1) NOT NULL,
	[CBL_CXC_ID] [int] NOT NULL,
 CONSTRAINT [PK_CARTELLE_BLOCCATE] PRIMARY KEY CLUSTERED 
(
	[CBL_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CARTELLE_BLOCCATE]  WITH CHECK ADD  CONSTRAINT [FK_CARTELLE_BLOCCATE_CARTELLEXCLIENTE] FOREIGN KEY([CBL_CXC_ID])
REFERENCES [dbo].[CARTELLEXCLIENTE] ([CXC_ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CARTELLE_BLOCCATE] CHECK CONSTRAINT [FK_CARTELLE_BLOCCATE_CARTELLEXCLIENTE]
GO

CREATE PROCEDURE [dbo].[SP_UpsertUser]
	@UserId		  int,
	@RoleId		  int,
	@UserLogin    varchar(50),
	@UserPsw      varchar(250),
	@UserName     varchar(50),
	@UserSurname  varchar(50),
	@UserDescr    varchar(100),
	@ClientiList  varchar(max),
	@ClientiListDisassociati varchar(max),
	@IdRuoloRevAutonomo int,
	@IdRuoloTeamLeader int
AS
BEGIN
	DECLARE @UserIdNew int
	DECLARE @RoleIdOld smallint
	DECLARE @Pos int
	DECLARE @ClienteID int
	
	DECLARE @TranStarted   bit
	SET @TranStarted = 0
	DECLARE @ERRORE int
	SET @ERRORE = 0
	
	IF( @@TRANCOUNT = 0 )
	BEGIN
		BEGIN TRANSACTION
		SET @TranStarted = 1
	END

	IF (@UserId < 0)
	BEGIN
		-- nuovo inserimento utente
		--SET NOCOUNT ON;
		INSERT INTO UTENTI (UTE_LOGIN, UTE_PSW, UTE_NOME, UTE_COGNOME, UTE_DESCR, UTE_RUO_ID) VALUES (@UserLogin, @UserPsw,@UserName,@UserSurname,@UserDescr,@RoleId);
		SELECT @UserIdNew = SCOPE_IDENTITY();
		if(@@ERROR > 0)
			SET @ERRORE = 1
		if (@RoleId = @IdRuoloRevAutonomo)
		begin
			--inserimento di un utente revisore autonomo, si devono inserire gli eventuali clienti associati
			SET @ClientiList = LTRIM(RTRIM(@ClientiList))+ ','
			SET @Pos = CHARINDEX(',', @ClientiList, 1)
			IF (REPLACE(@ClientiList, ',', '') <> '')
			BEGIN
				WHILE @Pos > 0
				BEGIN
					SET @ClienteID = LTRIM(RTRIM(LEFT(@ClientiList, @Pos - 1)))
					if (@ClienteID <> '')
					begin
						INSERT INTO UTENTIXCLIENTE (UXC_UTE_ID,UXC_CLI_ID,UXC_REV_AUTO) VALUES (@UserIdNew,@ClienteID,1)
						if(@@ERROR > 0)
							SET @ERRORE = 1			
					end

					SET @ClientiList = RIGHT(@ClientiList, LEN(@ClientiList) - @Pos)
					SET @Pos = CHARINDEX(',', @ClientiList, 1)
				END
			END
		end
					
	END
	ELSE
	BEGIN
		-- aggiornamento dati utente presente
		SELECT @RoleIdOld = UTE_RUO_ID FROM UTENTI WHERE UTE_ID = @UserId
		IF (@RoleIdOld <> @RoleId)
		BEGIN
		-- gli unici ruoli che si possonon impostare in gestione utenti da administrator sono TeamLeader,Da assegnare al team, alone e revisore autonomo		
		-- se l'utente non è piu revisore autonomo si devono eliminare le associazioni
		if (@RoleIdOld = @IdRuoloRevAutonomo)
		begin
			-- eliminione eventuali associazioni con i clienti
			delete from utentixcliente where uxc_ute_id = @UserId
			if(@@ERROR > 0)
				SET @ERRORE = 1
		end
		if (@RoleIdOld = @IdRuoloTeamLeader)
		begin
			-- se l'utente non è più teamleader si devono eliminare tuttle le associazioni con le cartelle e con i clienti
			-- cancellazione cartelle associate all'utente e ai suoi figli
			delete from cartellexcliente where cxc_uxc_id in 
			(select uxc_id from utentixcliente where uxc_ute_id in 
				(select ute_id from utenti where ute_ute_id = @UserId
				union
				select ute_id from utenti where ute_ute_id in (select ute_id from utenti where ute_ute_id = @UserId))
				union select ute_id from utenti where ute_id = @UserId
			)
			if(@@ERROR > 0)
				SET @ERRORE = 1
			
			-- cancellazione associazione anagrafica
			delete from utentixcliente where uxc_ute_id in 
			(select ute_id from utenti where ute_ute_id = @UserId
			union
			select ute_id from utenti where ute_ute_id in (select ute_id from utenti where ute_ute_id = @UserId)
			union
			select ute_id from utenti where ute_id = @UserId)
			if(@@ERROR > 0)
				SET @ERRORE = 1
		end
			
			
		END
		
		-- se è revisore autonomo si devono aggiornare/inserire le associazioni con i clienti
		if (@RoleId = @IdRuoloRevAutonomo)
			Begin
				SET @ClientiList = LTRIM(RTRIM(@ClientiList))+ ','
				SET @Pos = CHARINDEX(',', @ClientiList, 1)
				IF (REPLACE(@ClientiList, ',', '') <> '')
				BEGIN
					WHILE @Pos > 0
					BEGIN
						SET @ClienteID = LTRIM(RTRIM(LEFT(@ClientiList, @Pos - 1)))
						if (@ClienteID <> '')
						begin
							select UXC_ID from UTENTIXCLIENTE where uxc_cli_id = @ClienteID and uxc_ute_id = @UserId
							IF (@@ROWCOUNT = 0)
							BEGIN
								-- non esiste una precedente associazione => si crea la nuova associazione
								INSERT INTO UTENTIXCLIENTE (UXC_UTE_ID,UXC_CLI_ID,UXC_REV_AUTO) VALUES (@UserId,@ClienteID,1)
								if(@@ERROR > 0)
									SET @ERRORE = 1				
							END			
						end

						SET @ClientiList = RIGHT(@ClientiList, LEN(@ClientiList) - @Pos)
						SET @Pos = CHARINDEX(',', @ClientiList, 1)
					END
				END
				SET @ClientiListDisassociati = LTRIM(RTRIM(@ClientiListDisassociati))+ ','
				SET @Pos = CHARINDEX(',', @ClientiListDisassociati, 1)
				IF (REPLACE(@ClientiListDisassociati, ',', '') <> '')
				BEGIN
					WHILE @Pos > 0
					BEGIN
						SET @ClienteID = LTRIM(RTRIM(LEFT(@ClientiListDisassociati, @Pos - 1)))
						if (@ClienteID <> '')
						BEGIN		
							-- cancellazione associazioni cliente-utente che ora non c'è più
							delete from utentixcliente where uxc_cli_id = @ClienteID AND uxc_ute_id = @UserId
							if(@@ERROR > 0)
								SET @ERRORE = 1
						END
						SET @ClientiListDisassociati = RIGHT(@ClientiListDisassociati, LEN(@ClientiListDisassociati) - @Pos)
						SET @Pos = CHARINDEX(',', @ClientiListDisassociati, 1)
					END
				END
			End
		
		-- si aggiornano i dati sull'utente
		UPDATE UTENTI SET UTE_LOGIN = @UserLogin, UTE_PSW = @UserPsw, UTE_NOME = @UserName, UTE_COGNOME = @UserSurname, UTE_DESCR = @UserDescr, UTE_RUO_ID = @RoleId where UTE_ID = @UserId
		if(@@ERROR > 0)
			SET @ERRORE = 1
		-- se il ruolo è revisore autonomo e la lista contiene clienti 
		
	END

	IF( @TranStarted = 1 )
	begin
	IF (@ERRORE = 0)
		COMMIT TRANSACTION
	ELSE
		ROLLBACK
	END

	-- select inserita per comodità 
	RETURN SELECT RUO_ID FROM RUOLI WHERE RUO_ID = 1
END      
GO

CREATE PROCEDURE [dbo].[SP_DeleteUtente]
	@UserId		  int	
AS
BEGIN
	
	DECLARE @TranStarted   bit
	SET @TranStarted = 0

	DECLARE @ERRORE bit
	SET @ERRORE = 0
	
	IF( @@TRANCOUNT = 0 )
	BEGIN
		BEGIN TRANSACTION
		SET @TranStarted = 1
	END

	-- cancellazione cartelle associate all'utente e ai suoi figli
	delete from cartellexcliente where cxc_uxc_id in 
	(select uxc_id from utentixcliente where uxc_ute_id in 
		(select ute_id from utenti where ute_ute_id = @UserId
		union
		select ute_id from utenti where ute_ute_id in (select ute_id from utenti where ute_ute_id = @UserId))
		union select ute_id from utenti where ute_id = @UserId
	)
	if(@@ERROR > 0)
	BEGIN
		SET @ERRORE = 1
	END
	-- cancellazione associazione anagrafica
	delete from utentixcliente where uxc_ute_id in 
	(select ute_id from utenti where ute_ute_id = @UserId
	union
	select ute_id from utenti where ute_ute_id in (select ute_id from utenti where ute_ute_id = @UserId)
	union
	select ute_id from utenti where ute_id = @UserId)
	if(@@ERROR > 0)
	BEGIN
		SET @ERRORE = 1
	END

	update utenti set ute_ute_id = -1 where ute_id in (
	select ute_id from utenti where ute_ute_id = @UserId
	union
	select ute_id from utenti where ute_ute_id in (select ute_id from utenti where ute_ute_id = @UserId)
	union select ute_id from utenti where ute_id = @UserId
	)
	if(@@ERROR > 0)
	BEGIN
		SET @ERRORE = 1
	END

	delete from utenti where ute_id = @UserId

	if(@@ERROR > 0)
	BEGIN
		SET @ERRORE = 1
	END

	IF( @ERRORE = 0 AND @TranStarted = 1 )
		COMMIT TRANSACTION
	ELSE
		ROLLBACK

	-- select inserita per comodità 
	RETURN SELECT RUO_ID FROM RUOLI WHERE RUO_ID = 1
END
GO

CREATE PROCEDURE [dbo].[SP_UpsertTeamAdministrator](
	@TeamLeaderId int,	
	@UtentiListId varchar(max)
	)

AS
BEGIN
DECLARE @Pos int
DECLARE @Utente_id int
DECLARE @ListUtenti varchar(max)
DECLARE @TranStarted   bit
DECLARE @ESISTE bit
DECLARE @ERRORE int

SET @TranStarted = 0
IF( @@TRANCOUNT = 0 )
BEGIN
	BEGIN TRANSACTION
	SET @TranStarted = 1
END

SET @ERRORE = 0
-- si verifica se il team esiste
SET @ESISTE = 0
SELECT @ESISTE = count(*) from UTENTI where UTE_UTE_ID = @TeamLeaderId



SET @UtentiListId = LTRIM(RTRIM(@UtentiListId))+ ','
SET @Pos = CHARINDEX(',', @UtentiListId, 1)

IF (@ESISTE = 0)
BEGIN
	-- se il team non esiste 
	-- si scorre la lista utenti e si aggiorna il padre = id del team leader
	IF (REPLACE(@UtentiListId, ',', '') <> '')
	BEGIN
		WHILE @Pos > 0
		BEGIN
			SET @Utente_id = LTRIM(RTRIM(LEFT(@UtentiListId, @Pos - 1)))
			if (@Utente_id <> '')
			BEGIN		
				update utenti set ute_ute_id = @TeamLeaderId where ute_id = @Utente_id	
				if(@@ERROR > 0)
				BEGIN
					SET @ERRORE = 1
				END
						
			END
			SET @UtentiListId = RIGHT(@UtentiListId, LEN(@UtentiListId) - @Pos)
			SET @Pos = CHARINDEX(',', @UtentiListId, 1)
		END
	END
	if(@@ERROR > 0)
	BEGIN
		SET @ERRORE = 1
	END
END
ELSE
BEGIN
	SELECT UTE_ID INTO #TEAM_OLD
	FROM utenti where ute_id in (
	select ute_id from utenti where ute_ute_id = @TeamLeaderId
	union
	select ute_id from utenti where ute_ute_id in (select ute_id from utenti where ute_ute_id = @TeamLeaderId))
	if(@@ERROR > 0)
	BEGIN
		SET @ERRORE = 1
	END
	-- si scorrono gli utenti
	IF (REPLACE(@UtentiListId, ',', '') <> '')
	BEGIN
		WHILE @Pos > 0
		BEGIN
			SET @Utente_id = LTRIM(RTRIM(LEFT(@UtentiListId, @Pos - 1)))
			if (@Utente_id <> '')
			BEGIN
				SELECT UTE_ID FROM #TEAM_OLD WHERE UTE_ID = @Utente_id
				IF (@@ROWCOUNT = 0)
				BEGIN
					-- l'utente è un nuovo utente non presente nel vecchio team, si inserisce
					update utenti set ute_ute_id = @TeamLeaderId where ute_id = @Utente_id
					if(@@ERROR > 0)
					BEGIN
						SET @ERRORE = 1
					END
				END
				ELSE
				BEGIN
					-- l'utente era già presente nel team non deve essere inserito
					-- si elimina la riga dalla tabella OLD così da essere certi di lasciare nella tabella i soli utenti presenti nel solo team old
					DELETE FROM #TEAM_OLD WHERE UTE_ID = @Utente_id
					if(@@ERROR > 0)
					BEGIN
						SET @ERRORE = 1
					END
				END					
			END
			SET @UtentiListId = RIGHT(@UtentiListId, LEN(@UtentiListId) - @Pos)
			SET @Pos = CHARINDEX(',', @UtentiListId, 1)
		END
	END

	-- per ogni riga rimasta nella tabella old si esegue la funzione che disassocia le cartelle di lavoro dall'utente
	-- cancella l'associazione utente - cliente
	-- imposta ccome padre direttamente il team leader 
	DECLARE cursoreOldTeam CURSOR FOR 
		SELECT UTE_ID FROM #TEAM_OLD

	OPEN cursoreOldTeam
	FETCH NEXT FROM cursoreOldTeam INTO @Utente_id
	IF @@FETCH_STATUS = 0 
    WHILE @@FETCH_STATUS = 0
	BEGIN
		-- TO DO invocare la funzione che elimina associazione tra utente e cartelle di lavoro
		-- cancellazione cartelle associate al cliente e ai suoi figli
		delete from cartellexcliente where cxc_uxc_id in (select uxc_id from utentixcliente where uxc_ute_id in 
		(select ute_id from utenti where ute_ute_id = @Utente_id
		union
		select ute_id from utenti where ute_ute_id in (select ute_id from utenti where ute_ute_id = @Utente_id))
		union 
		select ute_id from utenti where ute_id = @Utente_id)

		if(@@ERROR > 0)
		BEGIN
			SET @ERRORE = 1
		END
		-- cancellazione associazione anagrafica
		delete from utentixcliente where uxc_ute_id in 
		(select ute_id from utenti where ute_ute_id = @Utente_id
		union
		select ute_id from utenti where ute_ute_id in (select ute_id from utenti where ute_ute_id = @Utente_id)
		union 
		select ute_id from utenti where ute_id = @Utente_id)

		if(@@ERROR > 0)
		BEGIN
			SET @ERRORE = 1
		END
		update utenti set ute_ute_id = -1 where ute_id in (
		select ute_id from utenti where ute_ute_id = @Utente_id
		union
		select ute_id from utenti where ute_ute_id in (select ute_id from utenti where ute_ute_id = @Utente_id)
		union 
		select ute_id from utenti where ute_id = @Utente_id
		)

		if(@@ERROR > 0)
		BEGIN
			SET @ERRORE = 1
		END
		FETCH NEXT FROM cursoreOldTeam INTO @Utente_id
	END
	CLOSE cursoreOldTeam
	DEALLOCATE cursoreOldTeam

	--If(OBJECT_ID('tempdb..#TEAM_OLD') Is Not Null)
	--Begin
		Drop Table #TEAM_OLD
	--End

END

IF( @TranStarted = 1 )
begin
	IF (@ERRORE = 0)
		COMMIT TRANSACTION
	ELSE
		ROLLBACK
	END	
END
GO

CREATE PROCEDURE [dbo].[SP_UpsertClientexUtente]
(
	@TeamLeaderID int,
	@ClientiList varchar(max),
	@ClientiListDisassociati varchar(max),	
	@RuoloEsecutore int
)
AS
BEGIN

DECLARE @Pos int
DECLARE @ClienteID int

DECLARE @TranStarted   bit
DECLARE @ERRORE bit

SET @ERRORE = 0

SET @TranStarted = 0
IF( @@TRANCOUNT = 0 )
BEGIN
	BEGIN TRANSACTION
	SET @TranStarted = 1
END

SET @ClientiList = LTRIM(RTRIM(@ClientiList))+ ','
SET @Pos = CHARINDEX(',', @ClientiList, 1)

IF (REPLACE(@ClientiList, ',', '') <> '')
BEGIN
	WHILE @Pos > 0
	BEGIN
		SET @ClienteID = LTRIM(RTRIM(LEFT(@ClientiList, @Pos - 1)))
		if (@ClienteID <> '')
		begin
			select UXC_ID from UTENTIXCLIENTE where uxc_cli_id = @ClienteID and uxc_ute_id = @TeamLeaderID
			IF (@@ROWCOUNT = 0)
			BEGIN
				-- non esiste una precedente associazione => si crea la nuova associazione
				INSERT INTO UTENTIXCLIENTE (UXC_UTE_ID,UXC_CLI_ID) VALUES (@TeamLeaderID,@ClienteID)
				if(@@ERROR > 0)
					SET @ERRORE = 1				
			END			
		end

		SET @ClientiList = RIGHT(@ClientiList, LEN(@ClientiList) - @Pos)
		SET @Pos = CHARINDEX(',', @ClientiList, 1)
	END
END

SET @ClientiListDisassociati = LTRIM(RTRIM(@ClientiListDisassociati))+ ','
SET @Pos = CHARINDEX(',', @ClientiListDisassociati, 1)
IF (REPLACE(@ClientiListDisassociati, ',', '') <> '')
BEGIN
	WHILE @Pos > 0
	BEGIN
		SET @ClienteID = LTRIM(RTRIM(LEFT(@ClientiListDisassociati, @Pos - 1)))
		if (@ClienteID <> '')
		BEGIN		
			-- si devono eliminare tutte le associazioni:
			-- tra le cartelle e gli esecutori associati al vecchio team leader
			-- tra il cliente e gli esecutori associati al vecchio team leader
			
			-- cancella le eventuali associazioni con le cartelle e gli esecutori del team leader per il cliente
			delete from cartellexcliente where cxc_uxc_id in (select uxc_id from utentixcliente inner join UTENTI on 
			uxc_ute_id = ute_id and ute_ruo_id = @RuoloEsecutore and ute_ute_id = @TeamLeaderID 
			where uxc_cli_id = @ClienteID)
			if(@@ERROR > 0)
				SET @ERRORE = 1


			-- cancellazione associazioni cliente-esecutori e cliente-teamleader
			delete from utentixcliente where uxc_cli_id = @ClienteID AND (uxc_ute_id = @TeamLeaderID OR uxc_ute_id in 
			(select ute_id from utenti where ute_ute_id = @TeamLeaderID and UTE_RUO_ID = @RuoloEsecutore))
			if(@@ERROR > 0)
				SET @ERRORE = 1

		END

		SET @ClientiListDisassociati = RIGHT(@ClientiListDisassociati, LEN(@ClientiListDisassociati) - @Pos)
		SET @Pos = CHARINDEX(',', @ClientiListDisassociati, 1)
	END
END
IF( @TranStarted = 1 )
begin
	IF (@ERRORE = 0)
		COMMIT TRANSACTION
	ELSE
		ROLLBACK
	END	
END
GO

CREATE  PROCEDURE [dbo].[SP_AssociaRuoli_Utenti](
	@TeamLeaderId int,	
	@UtentiRevisoriList varchar(max),
	@UtentiEsecutoriList varchar(max),
	@UtentiNonAssegnatiList varchar(max),
	@RuoloID_Revisore int,
	@RuoloID_Esecutore int,
	@RuoloID_Nessuno int
	)
AS
BEGIN
DECLARE @Pos int
DECLARE @Utente_id int
DECLARE @RuoloId_OLD int

DECLARE @TranStarted   bit
DECLARE @ERRORE int

SET @ERRORE = 0
SET @TranStarted = 0
IF( @@TRANCOUNT = 0 )
BEGIN
	BEGIN TRANSACTION
	SET @TranStarted = 1
END

SET @UtentiRevisoriList = LTRIM(RTRIM(@UtentiRevisoriList))+ ','
SET @Pos = CHARINDEX(',', @UtentiRevisoriList, 1)

IF (REPLACE(@UtentiRevisoriList, ',', '') <> '')
BEGIN
	WHILE @Pos > 0
	BEGIN
		SET @Utente_id = LTRIM(RTRIM(LEFT(@UtentiRevisoriList, @Pos - 1)))
		if (@Utente_id <> '')
		BEGIN		
			select @RuoloId_OLD = UTE_RUO_ID from UTENTI where ute_id = @Utente_id
			if (@RuoloId_OLD = @RuoloID_Esecutore)
			BEGIN
				-- l'utente aveva un ruolo da ESECUTORE, se esistono associazioni con cartelle si eliminano
				delete from cartellexcliente where cxc_uxc_id in (select uxc_id from utentixcliente where uxc_ute_id = @Utente_id)
				if(@@ERROR > 0)
				BEGIN
					SET @ERRORE = 1
				END
				-- si eliminano eventuali associazioni con i clienti
				delete from UTENTIXCLIENTE where uxc_ute_id = @Utente_id
				if(@@ERROR > 0)
				BEGIN
					SET @ERRORE = 1
				END							
			END
			
			if (@RuoloId_OLD <> @RuoloID_Revisore)
			begin
				update UTENTI set UTE_RUO_ID = @RuoloID_Revisore, UTE_UTE_ID = @TeamLeaderId where ute_id = @Utente_id
			end
			if(@@ERROR > 0)
			BEGIN
				SET @ERRORE = 1
			END
		END
		SET @UtentiRevisoriList = RIGHT(@UtentiRevisoriList, LEN(@UtentiRevisoriList) - @Pos)
		SET @Pos = CHARINDEX(',', @UtentiRevisoriList, 1)
	END
END

SET @UtentiEsecutoriList = LTRIM(RTRIM(@UtentiEsecutoriList))+ ','
SET @Pos = CHARINDEX(',', @UtentiEsecutoriList, 1)

IF (REPLACE(@UtentiEsecutoriList, ',', '') <> '')
BEGIN
	WHILE @Pos > 0
	BEGIN
		SET @Utente_id = LTRIM(RTRIM(LEFT(@UtentiEsecutoriList, @Pos - 1)))
		if (@Utente_id <> '')
		BEGIN		
			select @RuoloId_OLD = UTE_RUO_ID from UTENTI where ute_id = @Utente_id
			if (@RuoloId_OLD = @RuoloID_Revisore)
			BEGIN
				-- l'utente aveva un ruolo da REVISORE
				-- se esistono note si cancellano
				delete from NOTEXREVISORE where NXR_UTE_ID = @Utente_id
				if(@@ERROR > 0)
					SET @ERRORE = 1
				-- se esistono associazioni con cartelle si eliminano 
				-- si sganciano gli esecutori dall'utente
				delete from cartellexcliente where cxc_uxc_id in 
				(select uxc_id from utentixcliente where uxc_rev_id = @Utente_id)
				if(@@ERROR > 0)
				BEGIN
					SET @ERRORE = 1
				END	
				
				-- si cancellano le associazioni con in clienti
				delete from UTENTIXCLIENTE where uxc_rev_id = @Utente_id
				if(@@ERROR > 0)
				BEGIN
					SET @ERRORE = 1
				END						
			END
			if (@RuoloId_OLD <> @RuoloID_Esecutore)
			begin
				update UTENTI set UTE_RUO_ID = @RuoloID_Esecutore, UTE_UTE_ID = @TeamLeaderId where ute_id = @Utente_id
			end
			if(@@ERROR > 0)
			BEGIN
				SET @ERRORE = 1
			END
		END
		SET @UtentiEsecutoriList = RIGHT(@UtentiEsecutoriList, LEN(@UtentiEsecutoriList) - @Pos)
		SET @Pos = CHARINDEX(',', @UtentiEsecutoriList, 1)
	END
END

SET @UtentiNonAssegnatiList = LTRIM(RTRIM(@UtentiNonAssegnatiList))+ ','
SET @Pos = CHARINDEX(',', @UtentiNonAssegnatiList, 1)

IF (REPLACE(@UtentiNonAssegnatiList, ',', '') <> '')
BEGIN
	WHILE @Pos > 0
	BEGIN
		SET @Utente_id = LTRIM(RTRIM(LEFT(@UtentiNonAssegnatiList, @Pos - 1)))
		if (@Utente_id <> '')
		BEGIN		
			select @RuoloId_OLD = UTE_RUO_ID from UTENTI where ute_id = @Utente_id
			if (@RuoloId_OLD = @RuoloID_Revisore)
			BEGIN
				-- l'utente aveva un ruolo da REVISORE,
				-- se esistono associazioni con cartelle si eliminano 
				-- si sganciano gli esecutori dall'utente
				delete from cartellexcliente where cxc_uxc_id in 
				(select uxc_id from utentixcliente where uxc_rev_id = @Utente_id)
				if(@@ERROR > 0)
				BEGIN
					SET @ERRORE = 1
				END	
				
				-- si cancellano le associazioni con in clienti
				delete from UTENTIXCLIENTE where uxc_rev_id = @Utente_id
				if(@@ERROR > 0)
				BEGIN
					SET @ERRORE = 1
				END						
			END
			ELSE IF (@RuoloId_OLD = @RuoloID_Esecutore)
			BEGIN
				-- l'utente aveva un ruolo da ESECUTORE, se esistono associazioni con cartelle si eliminano
				delete from cartellexcliente where cxc_uxc_id in (select uxc_id from utentixcliente where uxc_ute_id = @Utente_id)
				if(@@ERROR > 0)
				BEGIN
					SET @ERRORE = 1
				END
				-- si eliminano eventuali associazioni con i clienti
				delete from UTENTIXCLIENTE where uxc_ute_id = @Utente_id
				if(@@ERROR > 0)
				BEGIN
					SET @ERRORE = 1
				END							
			END
			if (@RuoloId_OLD <> @RuoloID_Nessuno)
			begin
				update UTENTI set UTE_RUO_ID = @RuoloID_Nessuno, UTE_UTE_ID = @TeamLeaderId where ute_id = @Utente_id
			end
			if(@@ERROR > 0)
			BEGIN
				SET @ERRORE = 1
			END
		END
		SET @UtentiNonAssegnatiList = RIGHT(@UtentiNonAssegnatiList, LEN(@UtentiNonAssegnatiList) - @Pos)
		SET @Pos = CHARINDEX(',', @UtentiNonAssegnatiList, 1)
	END
END


IF( @TranStarted = 1 )
begin
	IF (@ERRORE = 0)
		COMMIT TRANSACTION
	ELSE
		ROLLBACK
	END	
END
GO

CREATE PROCEDURE [dbo].[SP_AssociaUtenti_Cliente](	
	@EsecutoriAssociati varchar(max),
	@EsecutoriNonAssociati varchar(max),
	@ClienteId varchar(10),
	@RevisoreId int,
	@TeamLeaderId int
	)
AS
BEGIN
DECLARE @Pos int
DECLARE @Utente_id int
DECLARE @UXC_ID int

DECLARE @TranStarted   bit
DECLARE @ERRORE int

SET @ERRORE = 0
SET @TranStarted = 0
IF( @@TRANCOUNT = 0 )
BEGIN
	BEGIN TRANSACTION
	SET @TranStarted = 1
END
--IF (@@ROWCOUNT = 0)
SET @EsecutoriAssociati = LTRIM(RTRIM(@EsecutoriAssociati))+ ','
SET @Pos = CHARINDEX(',', @EsecutoriAssociati, 1)

IF (REPLACE(@EsecutoriAssociati, ',', '') <> '')
begin
	WHILE @Pos > 0
	begin
		SET @Utente_id = LTRIM(RTRIM(LEFT(@EsecutoriAssociati, @Pos - 1)))
		if (@Utente_id <> '')
		begin		
			select UXC_UTE_ID from UTENTIXCLIENTE where UXC_UTE_ID = @Utente_id AND UXC_CLI_ID = @ClienteId AND UXC_REV_ID = @RevisoreId
			IF (@@ROWCOUNT = 0) -- non c'è => si inserisce
			begin
				INSERT INTO UTENTIXCLIENTE (UXC_UTE_ID,UXC_CLI_ID, UXC_REV_ID) VALUES (@Utente_id,@ClienteId,@RevisoreId)						
			end						
			if(@@ERROR > 0)
			begin
				SET @ERRORE = 1
			end			
		end
		SET @EsecutoriAssociati = RIGHT(@EsecutoriAssociati, LEN(@EsecutoriAssociati) - @Pos)
		SET @Pos = CHARINDEX(',', @EsecutoriAssociati, 1)
	end
end

SET @EsecutoriNonAssociati = LTRIM(RTRIM(@EsecutoriNonAssociati))+ ','
SET @Pos = CHARINDEX(',', @EsecutoriNonAssociati, 1)

IF (REPLACE(@EsecutoriNonAssociati, ',', '') <> '')
begin
	WHILE @Pos > 0
	begin
		SET @Utente_id = LTRIM(RTRIM(LEFT(@EsecutoriNonAssociati, @Pos - 1)))
		if (@Utente_id <> '')
		begin		
			select UXC_UTE_ID from UTENTIXCLIENTE where UXC_UTE_ID = @Utente_id AND UXC_CLI_ID = @ClienteId AND UXC_REV_ID = @RevisoreId
			IF (@@ROWCOUNT > 0)
			begin
				-- esiste un'associazione tra l'esecutore e il cliente che però è stata annullata dal team leader
				-- l'associazione si cancella 

				-- se esistono associazioni con cartelle si eliminano 				
				delete from cartellexcliente where cxc_uxc_id in 
				(select uxc_id from utentixcliente where uxc_ute_id = @Utente_id and uxc_cli_id = @ClienteId)
				if(@@ERROR > 0)
				begin
					SET @ERRORE = 1
				end	
				
				-- si sgancia l' esecutore dal cliente
				delete from UTENTIXCLIENTE where uxc_ute_id = @Utente_id  AND UXC_CLI_ID = @ClienteId AND UXC_REV_ID = @RevisoreId
				if(@@ERROR > 0)
				begin
					SET @ERRORE = 1
				end
								
			end
		end
		SET @EsecutoriNonAssociati = RIGHT(@EsecutoriNonAssociati, LEN(@EsecutoriNonAssociati) - @Pos)
		SET @Pos = CHARINDEX(',', @EsecutoriNonAssociati, 1)
	end
end

IF( @TranStarted = 1 )
begin
	IF (@ERRORE = 0)
		COMMIT TRANSACTION
	ELSE
		ROLLBACK
	END	
END
GO

CREATE PROCEDURE SP_InsertCartellaBloccata(
@idCliente int,
@idRevisore int,
@codice varchar(20)
)
as
begin

declare @CXC_ID int
set @CXC_ID = -1

SELECT @CXC_ID = CXC_ID FROM CARTELLEXCLIENTE INNER JOIN UTENTIXCLIENTE ON CXC_UXC_ID = UXC_ID 
where UXC_CLI_ID = @idCliente and UXC_REV_ID = @idRevisore and CXC_COD_ID = @codice

select * from CARTELLE_BLOCCATE where CBL_CXC_ID = @CXC_ID
if(@@ROWCOUNT = 0)
	INSERT INTO CARTELLE_BLOCCATE VALUES( @CXC_ID )

end
GO

ALTER PROCEDURE [mf].[DeleteAnagrafica]
	-- Add the parameters for the stored procedure here
@ID varchar(10)=null -- ID CLIENTE da cancellare
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
-------------------------------------------------------------------------------
declare @n int

if (@ID is null) return
set @n=(select count(*) from mf.Cliente where (ID=@ID))
if (@n<1) return
delete from mf.Incarico where (Cliente=@ID)
delete from mf.Isqc where (Cliente=@ID)
delete from mf.Revisione where (Cliente=@ID)
delete from mf.RelazioneV where (Cliente=@ID)
delete from mf.RelazioneVC where (Cliente=@ID)
delete from mf.RelazioneBC where (Cliente=@ID)
delete from mf.RelazioneB where (Cliente=@ID)
delete from mf.RelazioneBV where (Cliente=@ID)
delete from mf.Bilancio where (Cliente=@ID)
delete from mf.Verifica where (Cliente=@ID)
delete from mf.Vigilanza where (Cliente=@ID)
delete from mf.Conclusione where (Cliente=@ID)
delete from mf.Flusso where (Cliente=@ID)
delete from mf.PianificazioniVerifica where (Cliente=@ID)
delete from mf.PianificazioniVigilanza where (Cliente=@ID)
delete from doc.Documento where (Cliente=@ID) -- prima cancellare i files da disco!!!
-- cancellazione di tutti gli alberi 'Tree' e 'Dati'
delete from tree.TreeSessione where (idCliente=@ID)
delete from tree.TreeNodeSessione where (idCliente=@ID)
delete from tree.TreeNode01 where (idCliente=@ID)
delete from tree.TreeNode where (idCliente=@ID)
delete from dati.DatiValorePianificazione where (idCliente=@ID)
delete from dati.DatiValoreBV where (idCliente=@ID)
delete from dati.DatiValore06 where (idCliente=@ID)
delete from dati.DatiValore05 where (idCliente=@ID)
delete from dati.DatiValore04 where (idCliente=@ID)
delete from dati.DatiValore03 where (idCliente=@ID)
delete from dati.DatiValore02 where (idCliente=@ID)
delete from dati.DatiValore01 where (idCliente=@ID)
delete from dati.DatiValore where (idCliente=@ID)
delete from dati.DatiNode where (idCliente=@ID)
delete from dati.DatiDatoRawData where (idCliente=@ID)
delete from dati.DatiDatoFinalData where (idCliente=@ID)
delete from dati.DatiDato03 where (idCliente=@ID)
delete from dati.DatiDato02 where (idCliente=@ID)
delete from dati.DatiDato01 where (idCliente=@ID)
delete from dati.DatiDato where (idCliente=@ID)
delete from dbo.Trees where (idCliente=@ID)
-- cancellazione dei flussi
delete from flussi.FlussiAllegato where (IDCliente=@ID)
delete from flussi.FlussiValore where (IDCliente=@ID)
delete from flussi.FlussiDato where (ID=@ID)
delete from flussi.FlussiDati where (ID=@ID)
delete from flussi.Flussi where (ID=@ID)
-- cancellazione BilancioVerifica
delete from mf.ClienteBilancioVerificaAssociazione where (ID=@ID)
delete from mf.ClienteBilancioVerifica where (ID=@ID)


-------------- TEAM---------------------------------------------------------------------------------------------
-- cancellazion riferimenti in team per il cliente
--cancellazione cartelle associate ad esecutori
delete from CARTELLEXCLIENTE where CXC_UXC_ID in (select uxc_id from UTENTIXCLIENTE where uxc_cli_id = @ID)
-- cancellazione associazione utenti del team e clienti
delete from UTENTIXCLIENTE where uxc_cli_id = @ID
-------------- TEAM---------------------------------------------------------------------------------------------

-- cancellazione cliente
delete from mf.Cliente where (ID=@ID)
-- invalidazione cache
delete from dbo.xmlCache where (guid='RevisoftApp.rmdf')
delete from dbo.xmlCache where (guid='RevisoftApp.rdocf')
END
GO

CREATE PROCEDURE [mf].[DeleteAnagraficaNOCliente]
	-- Add the parameters for the stored procedure here
@ID varchar(10)=null -- ID CLIENTE da cancellare
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
-------------------------------------------------------------------------------
declare @n int

if (@ID is null) return
set @n=(select count(*) from mf.Cliente where (ID=@ID))
if (@n<1) return
delete from mf.Incarico where (Cliente=@ID)
delete from mf.Isqc where (Cliente=@ID)
delete from mf.Revisione where (Cliente=@ID)
delete from mf.RelazioneV where (Cliente=@ID)
delete from mf.RelazioneVC where (Cliente=@ID)
delete from mf.RelazioneBC where (Cliente=@ID)
delete from mf.RelazioneB where (Cliente=@ID)
delete from mf.RelazioneBV where (Cliente=@ID)
delete from mf.Bilancio where (Cliente=@ID)
delete from mf.Verifica where (Cliente=@ID)
delete from mf.Vigilanza where (Cliente=@ID)
delete from mf.Conclusione where (Cliente=@ID)
delete from mf.Flusso where (Cliente=@ID)
delete from mf.PianificazioniVerifica where (Cliente=@ID)
delete from mf.PianificazioniVigilanza where (Cliente=@ID)
delete from doc.Documento where (Cliente=@ID) -- prima cancellare i files da disco!!!
-- cancellazione di tutti gli alberi 'Tree' e 'Dati'
delete from tree.TreeSessione where (idCliente=@ID)
delete from tree.TreeNodeSessione where (idCliente=@ID)
delete from tree.TreeNode01 where (idCliente=@ID)
delete from tree.TreeNode where (idCliente=@ID)
delete from dati.DatiValorePianificazione where (idCliente=@ID)
delete from dati.DatiValoreBV where (idCliente=@ID)
delete from dati.DatiValore06 where (idCliente=@ID)
delete from dati.DatiValore05 where (idCliente=@ID)
delete from dati.DatiValore04 where (idCliente=@ID)
delete from dati.DatiValore03 where (idCliente=@ID)
delete from dati.DatiValore02 where (idCliente=@ID)
delete from dati.DatiValore01 where (idCliente=@ID)
delete from dati.DatiValore where (idCliente=@ID)
delete from dati.DatiNode where (idCliente=@ID)
delete from dati.DatiDatoRawData where (idCliente=@ID)
delete from dati.DatiDatoFinalData where (idCliente=@ID)
delete from dati.DatiDato03 where (idCliente=@ID)
delete from dati.DatiDato02 where (idCliente=@ID)
delete from dati.DatiDato01 where (idCliente=@ID)
delete from dati.DatiDato where (idCliente=@ID)
delete from dbo.Trees where (idCliente=@ID)
-- cancellazione dei flussi
delete from flussi.FlussiAllegato where (IDCliente=@ID)
delete from flussi.FlussiValore where (IDCliente=@ID)
delete from flussi.FlussiDato where (ID=@ID)
delete from flussi.FlussiDati where (ID=@ID)
delete from flussi.Flussi where (ID=@ID)
-- cancellazione BilancioVerifica
delete from mf.ClienteBilancioVerificaAssociazione where (ID=@ID)
delete from mf.ClienteBilancioVerifica where (ID=@ID)

---------------- TEAM---------------------------------------------------------------------------------------------
---- cancellazion riferimenti in team per il cliente
----cancellazione cartelle asocate ad esecutori
--delete from CARTELLEXCLIENTE where CXC_UXC_ID in (select uxc_id from UTENTIXCLIENTE where uxc_cli_id = @ID)
---- cancellazione associazione utenti team e clienti
--delete from UTENTIXCLIENTE where uxc_cli_id = @ID
---------------- TEAM---------------------------------------------------------------------------------------------

---- cancellazione cliente
--delete from mf.Cliente where (ID=@ID)

-- invalidazione cache
delete from dbo.xmlCache where (guid='RevisoftApp.rmdf')
delete from dbo.xmlCache where (guid='RevisoftApp.rdocf')
END
GO

CREATE PROCEDURE [dbo].[SP_UpsertNota]
	@idRevisore	int,
	@idCliente	varchar(10),	
	@codice		varchar(20),
	@nota		varchar(max)
AS
BEGIN
	DECLARE @idNotaNew int

	SELECT @idNotaNew = NXR_ID
	FROM NOTEXREVISORE
	WHERE @idRevisore = NXR_UTE_ID
	AND @idCliente = NXR_CLI_ID
	AND @codice = NXR_COD_ID

	IF @idNotaNew > 0
	BEGIN
		UPDATE NOTEXREVISORE SET NXR_NOTE = @nota WHERE @idNotaNew = NXR_ID
		IF @@ROWCOUNT = 0
			RETURN 0
		RETURN @idNotaNew
	END
	
	INSERT INTO NOTEXREVISORE (NXR_UTE_ID, NXR_CLI_ID, NXR_COD_ID, NXR_NOTE) 
	VALUES (@idRevisore, @idCliente, @codice, @nota);
	SELECT @idNotaNew = SCOPE_IDENTITY();
	
	RETURN  @idNotaNew
END      
GO

CREATE TABLE [dbo].[TITOLI](
	[TIT_TITOLO] [varchar](250) NULL,
	[TIT_CODICE] [varchar](20) NULL,
	[TIT_ID] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_TITOLI] PRIMARY KEY CLUSTERED 
(
	[TIT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TITOLI]  WITH CHECK ADD  CONSTRAINT [FK_TITOLI_RemapTreeNodeCodici] FOREIGN KEY([TIT_CODICE])
REFERENCES [dbo].[RemapTreeNodeCodici] ([Codice])
GO

ALTER TABLE [dbo].[TITOLI] CHECK CONSTRAINT [FK_TITOLI_RemapTreeNodeCodici]
GO

delete from titoli
go
INSERT INTO TITOLI (TIT_CODICE, TIT_TITOLO)
SELECT '1', 'ACCETTAZIONE DELL''INCARICO'  UNION ALL
SELECT '1.1', 'SOGGETTO UNICO'  UNION ALL
SELECT '1.1.1', 'Requisiti professionali'  UNION ALL
SELECT '1.1.2', 'Comunicazione altri incarichi'  UNION ALL
SELECT '1.1.3', 'Indipendenza (art. 10 D.Lgs. 39/2010 - art. 2399 C.C.)'  UNION ALL
SELECT '1.1.4', 'Calcolo indipendenza finanziaria (sindaci)'  UNION ALL
SELECT '1.1.5', 'Accettazione/Mantenimento dell''incarico'  UNION ALL
SELECT '1.1.7', 'Compensi e risorse'  UNION ALL
SELECT '1.1.8', 'Verifica obblighi antiriciclaggio'  UNION ALL
SELECT '1.1.9', 'Analisi preliminare di bilancio e passaggio consegne'  UNION ALL
SELECT '1.1.9.1', 'Bilancio Ordinario'  UNION ALL
SELECT '1.1.9.1.1', 'Bilancio Ordinario'  UNION ALL
SELECT '1.1.9.1.2', 'Bilancio Riclassificato'  UNION ALL
SELECT '1.1.9.1.3', 'Indici'  UNION ALL
SELECT '1.1.9.1.6', 'Valutazioni economiche - finanziarie'  UNION ALL
SELECT '1.1.9.2', 'Bilancio Abbreviato / Micro'  UNION ALL
SELECT '1.1.9.2.1', 'Bilancio Abbreviato / Micro'  UNION ALL
SELECT '1.1.9.2.2', 'Bilancio Riclassificato'  UNION ALL
SELECT '1.1.9.2.3', 'Indici'  UNION ALL
SELECT '1.1.9.2.6', 'Valutazioni economiche - finanziarie'  UNION ALL
SELECT '1.1.9.91', 'Consegne precedente revisore'  UNION ALL
SELECT '1.1.9.92', 'Controllo saldi apertura del bilancio'  UNION ALL
SELECT '1.1.20', 'Lettera di Incarico'  UNION ALL
SELECT '1.1.20.1', 'Introduzione'  UNION ALL
SELECT '1.1.20.2', 'Definizioni'  UNION ALL
SELECT '1.1.20.3', 'Oggetto dell’incarico'  UNION ALL
SELECT '1.1.20.4', 'Durata'  UNION ALL
SELECT '1.1.20.5', 'Obiettivi di Revisione'  UNION ALL
SELECT '1.1.20.6', 'Responsabilità della Direzione - lettera di attestazione'  UNION ALL
SELECT '1.1.20.7', 'Responsabilità del revisore'  UNION ALL
SELECT '1.1.20.8', 'Modalità di svolgimento dell''incarico'  UNION ALL
SELECT '1.1.20.8.1', 'Revisione del bilancio d''esercizio'  UNION ALL
SELECT '1.1.20.8.2', 'Verifica della regolare contabilità sociale'  UNION ALL
SELECT '1.1.20.9', 'Sottoscrizione delle dichiarazioni fiscali'  UNION ALL
SELECT '1.1.20.10', 'Personale impiegato, tempi e corrispettivi'  UNION ALL
SELECT '1.1.20.10.1', 'Personale'  UNION ALL
SELECT '1.1.20.10.2', 'Incipit Tempi e Corrispettivi'  UNION ALL
SELECT '1.1.20.10.3', 'Tempi e Corrispettivi'  UNION ALL
SELECT '1.1.20.10.4', 'Forfait annuo'  UNION ALL
SELECT '1.1.20.10.5', 'Ulteriori informazioni Tempi e corrispettivi'  UNION ALL
SELECT '1.1.20.10.6', 'Pagamenti'  UNION ALL
SELECT '1.1.20.10.7', 'Ulteriori informazioni Pagamenti'  UNION ALL
SELECT '1.1.20.11', 'Situazioni di incompatibilità'  UNION ALL
SELECT '1.1.20.12', 'Riservatezza dei dati'  UNION ALL
SELECT '1.1.20.13', 'Adempimenti in materia di Antiriciclaggio'  UNION ALL
SELECT '1.1.20.14', 'Coperture assicurative'  UNION ALL
SELECT '1.1.20.90', 'Conclusioni'  UNION ALL
SELECT '1.1.97', 'Discussioni del team'  UNION ALL
SELECT '1.1.98', 'Allegati Liberi non associati a carte di lavoro'  UNION ALL
SELECT '1.1.99', 'Tempi di Revisione'  UNION ALL
SELECT '1.21', 'SOGGETTO COLLEGIALE'  UNION ALL
SELECT '1.21.1', 'Composizione Collegio'  UNION ALL
SELECT '1.21.1.A', 'Presidente'  UNION ALL
SELECT '1.21.1.B', 'Sindaco effettivo'  UNION ALL
SELECT '1.21.1.C', 'Annotazioni'  UNION ALL
SELECT '1.21.1.C', 'Sindaco supplente'  UNION ALL
SELECT '1.21.2', 'Comunicazione altri incarichi'  UNION ALL
SELECT '1.21.3', 'Indipendenza (art. 10 D.Lgs. 39/2010 - art. 2399 C.C.)'  UNION ALL
SELECT '1.21.3.A', 'Presidente'  UNION ALL
SELECT '1.21.3.B', 'Membro Effettivo'  UNION ALL
SELECT '1.21.3.C', 'Membro Effettivo'  UNION ALL
SELECT '1.21.3.D', 'Sindaco Supplente'  UNION ALL
SELECT '1.21.3.E', 'Sindaco Supplente'  UNION ALL
SELECT '1.21.3.F', 'Valutazione collegiale sintetica di indipendenza'  UNION ALL
SELECT '1.21.4', 'Calcolo indipendenza finanziaria (sindaci)'  UNION ALL
SELECT '1.21.5', 'Accettazione/Mantenimento dell''incarico'  UNION ALL
SELECT '1.21.6', 'Verifica accettazioni altri membri'  UNION ALL
SELECT '1.21.7', 'Compensi e risorse'  UNION ALL
SELECT '1.21.7.A', 'Compensi e risorse'  UNION ALL
SELECT '1.21.7.B', 'Precisazioni'  UNION ALL
SELECT '1.21.8', 'Verifica obblighi antiriciclaggio'  UNION ALL
SELECT '1.21.9', 'Analisi preliminare di bilancio e passaggio consegne'  UNION ALL
SELECT '1.21.9.1', 'Bilancio Ordinario'  UNION ALL
SELECT '1.21.9.2', 'Bilancio Abbreviato / Micro'  UNION ALL
SELECT '1.21.9.2', 'Bilancio Riclassificato'  UNION ALL
SELECT '1.21.9.2.1', 'Bilancio Abbreviato / Micro'  UNION ALL
SELECT '1.21.9.2.2', 'Bilancio Riclassificato'  UNION ALL
SELECT '1.21.9.2.3', 'Indici'  UNION ALL
SELECT '1.21.9.2.6', 'Valutazioni economiche - finanziarie'  UNION ALL
SELECT '1.21.9.3', 'Indici'  UNION ALL
SELECT '1.21.9.6', 'Valutazioni economiche - finanziarie'  UNION ALL
SELECT '1.21.9.91', 'Consegne precedente revisore'  UNION ALL
SELECT '1.21.9.92', 'Controllo saldi apertura del bilancio'  UNION ALL
SELECT '1.21.20', 'Lettera di Incarico'  UNION ALL
SELECT '1.21.20.1', 'Introduzione'  UNION ALL
SELECT '1.21.20.2', 'Definizioni'  UNION ALL
SELECT '1.21.20.3', 'Oggetto dell’incarico'  UNION ALL
SELECT '1.21.20.4', 'Durata'  UNION ALL
SELECT '1.21.20.5', 'Obiettivi di Revisione'  UNION ALL
SELECT '1.21.20.6', 'Responsabilità della Direzione - lettera di attestazione'  UNION ALL
SELECT '1.21.20.7', 'Responsabilità del revisore'  UNION ALL
SELECT '1.21.20.8', 'Modalità di svolgimento dell''incarico'  UNION ALL
SELECT '1.21.20.8.1', 'Revisione del bilancio d''esercizio'  UNION ALL
SELECT '1.21.20.8.2', 'Verifica della regolare contabilità sociale'  UNION ALL
SELECT '1.21.20.9', 'Sottoscrizione delle dichiarazioni fiscali'  UNION ALL
SELECT '1.21.20.10', 'Personale impiegato, tempi e corrispettivi'  UNION ALL
SELECT '1.21.20.10.1', 'Personale'  UNION ALL
SELECT '1.21.20.10.2', 'Incipit Tempi e Corrispettivi'  UNION ALL
SELECT '1.21.20.10.3', 'Tempi e Corrispettivi'  UNION ALL
SELECT '1.21.20.10.4', 'Forfait annuo'  UNION ALL
SELECT '1.21.20.10.5', 'Ulteriori informazioni Tempi e corrispettivi'  UNION ALL
SELECT '1.21.20.10.6', 'Pagamenti'  UNION ALL
SELECT '1.21.20.10.7', 'Ulteriori informazioni Pagamenti'  UNION ALL
SELECT '1.21.20.11', 'Situazioni di incompatibilità'  UNION ALL
SELECT '1.21.20.12', 'Riservatezza dei dati'  UNION ALL
SELECT '1.21.20.13', 'Adempimenti in materia di Antiriciclaggio'  UNION ALL
SELECT '1.21.20.14', 'Coperture assicurative'  UNION ALL
SELECT '1.21.20.90', 'Conclusioni'  UNION ALL
SELECT '1.21.97', 'Discussioni del team'  UNION ALL
SELECT '1.21.98', 'Allegati Liberi non associati a carte di lavoro'  UNION ALL
SELECT '1.21.99', 'Tempi di Revisione'  UNION ALL
SELECT '2', 'COMPRENSIONE - RISCHIO - PIANIFICAZIONE'  UNION ALL
SELECT '2.1', 'Anagrafica della società'  UNION ALL
SELECT '2.1.1', 'Denominazione, sede iscrizioni'  UNION ALL
SELECT '2.1.1.A', 'Denominazione Società'  UNION ALL
SELECT '2.1.1.B', 'Iscrizioni'  UNION ALL
SELECT '2.1.1.B', 'Poteri'  UNION ALL
SELECT '2.1.1.C', 'Attività svolte nella sede'  UNION ALL
SELECT '2.1.2', 'Sedi secondarie'  UNION ALL
SELECT '2.1.3', 'Notizie Storiche'  UNION ALL
SELECT '2.1.4', 'Oggetto sociale, attività svolta, settore di posizionamento'  UNION ALL
SELECT '2.1.5', 'Capitale sociale, soci'  UNION ALL
SELECT '2.1.6', 'Diritti particolari dei soci e delle azioni'  UNION ALL
SELECT '2.1.7', 'Prestazioni accessorie'  UNION ALL
SELECT '2.1.8', 'Patrimoni separati'  UNION ALL
SELECT '2.1.9', 'Partecipazioni in altre società'  UNION ALL
SELECT '2.1.9.A', 'Partecipazioni in altre società'  UNION ALL
SELECT '2.1.9.B', 'Annotazioni'  UNION ALL
SELECT '2.1.10', 'Strumenti Finanziari'  UNION ALL
SELECT '2.2', 'Organi volitivi'  UNION ALL
SELECT '2.2.1', 'Amministratore unico e poteri'  UNION ALL
SELECT '2.2.1.A', 'Amministratore unico'  UNION ALL
SELECT '2.2.2', 'Consiglio di amministrazione e poteri'  UNION ALL
SELECT '2.2.2.A', 'Composizione e durata'  UNION ALL
SELECT '2.2.2.B', 'Poteri'  UNION ALL
SELECT '2.2.2.C', 'Consigliere 1'  UNION ALL
SELECT '2.2.3', 'Altre forme di amministrazione'  UNION ALL
SELECT '2.2.4', 'Poteri delegati'  UNION ALL
SELECT '2.2.4.A', 'Poteri delegati'  UNION ALL
SELECT '2.3', 'Organi di controllo'  UNION ALL
SELECT '2.3.1', 'Collegio sindacale'  UNION ALL
SELECT '2.3.1.A', 'Presidente'  UNION ALL
SELECT '2.3.1.B', 'Sindaco effettivo'  UNION ALL
SELECT '2.3.1.C', 'Annotazioni'  UNION ALL
SELECT '2.3.1.C', 'Sindaco supplente'  UNION ALL
SELECT '2.3.2', 'Sindaco Unico'  UNION ALL
SELECT '2.3.3', 'Revisore / Società di Revisione'  UNION ALL
SELECT '2.4', 'Governance (D.Leg. 231/2001)'  UNION ALL
SELECT '2.4.1', 'Necessità di adozione verifica'  UNION ALL
SELECT '2.4.2', 'Impegno per l''attuazione del modello'  UNION ALL
SELECT '2.4.3', 'Modello già applicato - verifiche'  UNION ALL
SELECT '2.4.4', 'Organismo di vigilanza'  UNION ALL
SELECT '2.4.4.A', 'Composizione e durata'  UNION ALL
SELECT '2.4.4.B', 'Membro'  UNION ALL
SELECT '2.5', 'Organizzazione operativa'  UNION ALL
SELECT '2.5.1', 'Organigramma'  UNION ALL
SELECT '2.5.2', 'Organizzazione amministrativa, contabile e principi legali'  UNION ALL
SELECT '2.5.3', 'Sistema informatico'  UNION ALL
SELECT '2.5.4', 'Rete vendita'  UNION ALL
SELECT '2.6', 'Privacy'  UNION ALL
SELECT '2.6.1', 'Check list Garante della privacy'  UNION ALL
SELECT '2.6.2', 'Note e commenti'  UNION ALL
SELECT '2.7', 'Tracciabilità  pagamenti P.A.'  UNION ALL
SELECT '2.8', 'RISCHIO INTRINSECO - AMBIENTE DI CONTROLLO'  UNION ALL
SELECT '2.8.1', 'Filosofia della direzione e stile operativo'  UNION ALL
SELECT '2.8.2', 'Struttura organizzativa'  UNION ALL
SELECT '2.8.3', 'Assegnazione di autorità e di responsabilità'  UNION ALL
SELECT '2.8.4', 'Politiche del personale'  UNION ALL
SELECT '2.8.5', 'Sistema di reporting interno e monitoraggio'  UNION ALL
SELECT '2.8.6', 'Fattori di rischio specifici'  UNION ALL
SELECT '2.8.7', 'Attribuzione rischio intrinseco (ambiente di controllo)'  UNION ALL
SELECT '2.8.7.A', 'Sommario valutazioni'  UNION ALL
SELECT '2.8.7.B', 'Valutazione del rischio complessivo'  UNION ALL
SELECT '2.8.7 BIS', 'Attribuzione rischio intrinseco (Relazione Sostitutiva)'  UNION ALL
SELECT '2.8.7 BIS.A', 'Relazione Sostitutiva'  UNION ALL
SELECT '2.8.7 BIS.B', 'Valutazione del rischio complessivo'  UNION ALL
SELECT '2.9', 'RISCHIO DI CONTROLLO - SISTEMA DI CONTROLLO INTERNO'  UNION ALL
SELECT '2.9.1', 'Ciclo vendite'  UNION ALL
SELECT '2.9.1.A', 'Ciclo Vendite'  UNION ALL
SELECT '2.9.1.B', 'Test Procedura'  UNION ALL
SELECT '2.9.1.C', 'Sommario'  UNION ALL
SELECT '2.9.1.X', 'Descrizione del Ciclo'  UNION ALL
SELECT '2.9.2', 'Ciclo acquisti'  UNION ALL
SELECT '2.9.2.A', 'Ciclo acquisti'  UNION ALL
SELECT '2.9.2.B', 'Test Procedura'  UNION ALL
SELECT '2.9.2.C', 'Sommario'  UNION ALL
SELECT '2.9.2.X', 'Descrizione del Ciclo'  UNION ALL
SELECT '2.9.3', 'Ciclo magazzino'  UNION ALL
SELECT '2.9.3.A', 'Ciclo magazzino'  UNION ALL
SELECT '2.9.3.B', 'Sommario'  UNION ALL
SELECT '2.9.3.X', 'Descrizione del Ciclo'  UNION ALL
SELECT '2.9.4', 'Ciclo tesoreria'  UNION ALL
SELECT '2.9.4.A', 'Ciclo tesoreria'  UNION ALL
SELECT '2.9.4.B', 'Sommario'  UNION ALL
SELECT '2.9.4.X', 'Descrizione del Ciclo'  UNION ALL
SELECT '2.9.5', 'Ciclo personale dipendente'  UNION ALL
SELECT '2.9.5.A', 'Ciclo personale dipendente'  UNION ALL
SELECT '2.9.5.B', 'Sommario'  UNION ALL
SELECT '2.9.5.X', 'Descrizione del Ciclo'  UNION ALL
SELECT '2.9.9', 'Rischio di Controllo - Sommario'  UNION ALL
SELECT '2.9.10', 'Osservazioni al Sistema di Controllo Interno'  UNION ALL
SELECT '2.10', 'RISCHIO DI INDIVIDUAZIONE / PIANIFICAZIONE'  UNION ALL
SELECT '2.10.1', 'Rischio di individuazione'  UNION ALL
SELECT '2.10.2', 'Osservazioni al Rischio di Individuazione'  UNION ALL
SELECT '2.10.3', 'Pianificazione senza dati di bilancio'  UNION ALL
SELECT '2.10.5', 'Pianificazione con dati di bilancio'  UNION ALL
SELECT '2.11', 'MATERIALITA'' / SIGNIFICATIVITA'''  UNION ALL
SELECT '2.11.1', 'Materialità - Sintetica'  UNION ALL
SELECT '2.11.3', 'Materialità - Dettagliata'  UNION ALL
SELECT '2.97', 'Discussioni del team'  UNION ALL
SELECT '2.98', 'Allegati Liberi non associati a carte di lavoro'  UNION ALL
SELECT '2.99', 'Tempi di Revisione'  UNION ALL
SELECT '3', 'CONTROLLO DEL BILANCIO'  UNION ALL
SELECT '3.1', 'Bilancio ordinario'  UNION ALL
SELECT '3.1.1', 'Bilancio ordinario'  UNION ALL
SELECT '3.1.2', 'Bilancio riclassificato'  UNION ALL
SELECT '3.1.3', 'Indici'  UNION ALL
SELECT '3.1.6', 'Rendiconto finanziario - controlli'  UNION ALL
SELECT '3.1.A', 'Patrimoniale Attivo'  UNION ALL
SELECT '3.1.B', 'Patrimoniale Passivo'  UNION ALL
SELECT '3.1.C', 'Conto Economico'  UNION ALL
SELECT '3.1.21', 'Controlli saldi di apertura'  UNION ALL
SELECT '3.2', 'Bilancio abbreviato'  UNION ALL
SELECT '3.2.1', 'Bilancio abbreviato'  UNION ALL
SELECT '3.2.2', 'Bilancio riclassificato'  UNION ALL
SELECT '3.2.3', 'Indici'  UNION ALL
SELECT '3.2.A', 'Patrimoniale Attivo'  UNION ALL
SELECT '3.2.B', 'Patrimoniale Passivo'  UNION ALL
SELECT '3.2.C', 'Conto Economico'  UNION ALL
SELECT '3.2.21', 'Controlli saldi di apertura'  UNION ALL
SELECT '3.3.9.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4', 'Controllo dei dati del bilancio'  UNION ALL
SELECT '3.4.1', 'Immobilizzazioni immateriali'  UNION ALL
SELECT '3.4.1.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.1.A', 'Dati Lead'  UNION ALL
SELECT '3.4.1.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.1.C', 'Errori Rilevati'  UNION ALL
SELECT '3.4.1.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.2', 'Immobilizzazioni materiali'  UNION ALL
SELECT '3.4.2.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.2.A', 'Dati Lead'  UNION ALL
SELECT '3.4.2.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.2.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.2.C', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.3', 'Immobilizzazioni finanziarie'  UNION ALL
SELECT '3.4.3.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.3.A', 'Dati Lead'  UNION ALL
SELECT '3.4.3.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.3.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.3.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.4', 'Rimanenze di magazzino'  UNION ALL
SELECT '3.4.4.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.4.A', 'Dati Lead'  UNION ALL
SELECT '3.4.4.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.4.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.4.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.5', 'Rimanenze - commesse/opere a lungo termine'  UNION ALL
SELECT '3.4.5.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.5.A', 'Dati Lead'  UNION ALL
SELECT '3.4.5.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.5.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.5.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.6', 'Attività finanziarie non immobilizzate'  UNION ALL
SELECT '3.4.6.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.6.A', 'Dati Lead'  UNION ALL
SELECT '3.4.6.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.6.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.6.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.7', 'Crediti commerciali (Clienti)'  UNION ALL
SELECT '3.4.7.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.7.A', 'Dati Lead'  UNION ALL
SELECT '3.4.7.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.7.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.7.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.8', 'Crediti e debiti infra Gruppo'  UNION ALL
SELECT '3.4.8.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.8.A', 'Dati Lead'  UNION ALL
SELECT '3.4.8.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.8.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.8.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.9', 'Crediti tributari e per imposte anticipate'  UNION ALL
SELECT '3.4.9.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.9.A', 'Dati Lead'  UNION ALL
SELECT '3.4.9.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.9.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.10', 'Crediti verso altri'  UNION ALL
SELECT '3.4.10.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.10.A', 'Dati Lead'  UNION ALL
SELECT '3.4.10.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.10.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.10.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.11', 'Cassa e banche'  UNION ALL
SELECT '3.4.11.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.11.A', 'Dati Lead'  UNION ALL
SELECT '3.4.11.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.11.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.11.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.12', 'Ratei e risconti (attivi e passivi)'  UNION ALL
SELECT '3.4.12.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.12.A', 'Dati Lead'  UNION ALL
SELECT '3.4.12.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.12.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.12.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.13', 'Patrimonio netto'  UNION ALL
SELECT '3.4.13.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.13.A', 'Dati Lead'  UNION ALL
SELECT '3.4.13.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.13.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.13.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.14', 'Fondi per rischi e oneri'  UNION ALL
SELECT '3.4.14.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.14.A', 'Dati Lead'  UNION ALL
SELECT '3.4.14.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.14.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.14.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.15', 'T.F.R. (Trattamento Fine Rapporto)'  UNION ALL
SELECT '3.4.15.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.15.A', 'Dati Lead'  UNION ALL
SELECT '3.4.15.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.15.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.15.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.16', 'Mutui e finanziamenti non bancari'  UNION ALL
SELECT '3.4.16.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.16.A', 'Dati Lead'  UNION ALL
SELECT '3.4.16.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.16.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.16.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.17', 'Debiti commerciali (Fornitori)'  UNION ALL
SELECT '3.4.17.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.17.A', 'Dati Lead'  UNION ALL
SELECT '3.4.17.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.17.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.17.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.18', 'Debiti tributari'  UNION ALL
SELECT '3.4.18.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.18.A', 'Dati Lead'  UNION ALL
SELECT '3.4.18.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.18.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.18.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.19', 'Debiti verso altri'  UNION ALL
SELECT '3.4.19.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.19.A', 'Dati Lead'  UNION ALL
SELECT '3.4.19.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.19.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.19.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.4.21', 'Conto economico'  UNION ALL
SELECT '3.4.21.0', 'Scelte da Pianificazione'  UNION ALL
SELECT '3.4.21.A', 'Dati Lead'  UNION ALL
SELECT '3.4.21.B', 'Programma di lavoro - Check list'  UNION ALL
SELECT '3.4.21.C', 'Errori rilevati'  UNION ALL
SELECT '3.4.21.D', 'Memo sui controlli eseguiti'  UNION ALL
SELECT '3.5', 'Altri controlli'  UNION ALL
SELECT '3.5.1', 'Parti correlate'  UNION ALL
SELECT '3.5.2', 'Fatti censurabili'  UNION ALL
SELECT '3.5.5', 'Relazione sulla gestione e Bilancio'  UNION ALL
SELECT '3.11', 'Bilancio Consolidato'  UNION ALL
SELECT '3.11.1', 'Bilancio Consolidato'  UNION ALL
SELECT '3.11.1.1', 'Bilancio Consolidato'  UNION ALL
SELECT '3.11.1.1.A', 'Patrimoniale Attivo'  UNION ALL
SELECT '3.11.1.1.B', 'Patrimoniale Passivo'  UNION ALL
SELECT '3.11.1.1.C', 'Conto Economico'  UNION ALL
SELECT '3.11.1.2', 'Bilancio riclassificato'  UNION ALL
SELECT '3.11.1.3', 'Indici'  UNION ALL
SELECT '3.11.2', 'Dati di Bilancio delle Componenti'  UNION ALL
SELECT '3.11.3', 'Assegnazione degli Scope alle Componenti'  UNION ALL
SELECT '3.11.5', 'Materialità - Significatività Consolidata'  UNION ALL
SELECT '3.11.7', 'Istruzioni (ai revisori delle componenti)'  UNION ALL
SELECT '3.11.9', 'Reportistica revisori componenti'  UNION ALL
SELECT '3.11.11', 'Check List'  UNION ALL
SELECT '3.11.15', 'Analisi scritture consolidamento'  UNION ALL
SELECT '3.11.15.A', 'Analisi scritture consolidamento'  UNION ALL
SELECT '3.11.15.B', 'Consolidamento - Errori Rilavati'  UNION ALL
SELECT '3.97', 'Discussioni del team'  UNION ALL
SELECT '3.98', 'Allegati Liberi non associati a carte di lavoro'  UNION ALL
SELECT '3.99', 'Tempi di Revisione'  UNION ALL
SELECT '4', 'CONTROLLO CONTABILE'  UNION ALL
SELECT '4.1', 'Libri Sociali'  UNION ALL
SELECT '4.1.1', 'Libro degli azionisti'  UNION ALL
SELECT '4.1.2', 'Libro verbali assemblee - decisioni dei soci'  UNION ALL
SELECT '4.1.3', 'Libro verbali riunioni C.d.A.'  UNION ALL
SELECT '4.1.4', 'Libro degli obbligazionisti e certificati obbligazionari'  UNION ALL
SELECT '4.1.5', 'Libro verbali assemblee obbligazionisti'  UNION ALL
SELECT '4.1.6', 'Libro del Collegio Sindacale'  UNION ALL
SELECT '4.1.7', 'Libro delle determine dell''amministratore unico'  UNION ALL
SELECT '4.1.11', 'Libri Diversi'  UNION ALL
SELECT '4.2', 'Libri e registri contabili'  UNION ALL
SELECT '4.2.1', 'Libro degli inventari'  UNION ALL
SELECT '4.2.2', 'Libro giornale / sezionali'  UNION ALL
SELECT '4.2.3', 'Registro dei beni ammortizzabili'  UNION ALL
SELECT '4.2.4', 'Registro delle fatture emesse'  UNION ALL
SELECT '4.2.5', 'Registro dei corrispettivi'  UNION ALL
SELECT '4.2.6', 'Registro delle fatture di acquisto'  UNION ALL
SELECT '4.2.7', 'Registro fatture emesse intra UE'  UNION ALL
SELECT '4.2.8', 'Registro fatture acquisto intra UE'  UNION ALL
SELECT '4.2.9', 'Registro degli omaggi'  UNION ALL
SELECT '4.2.10', 'Registri acquisti / cessione beni usati'  UNION ALL
SELECT '4.2.10.1', 'Regime analitico'  UNION ALL
SELECT '4.2.10.2', 'Regime forfettario'  UNION ALL
SELECT '4.2.10.3', 'Regime globale'  UNION ALL
SELECT '4.2.11', 'Registro delle fatture in sospeso'  UNION ALL
SELECT '4.2.12', 'Registro riepilogativo operazioni I.V.A.'  UNION ALL
SELECT '4.2.13', 'Registro delle società controllanti (IVA gruppo)'  UNION ALL
SELECT '4.2.14', 'Registro merci c/ lavorazione'  UNION ALL
SELECT '4.2.15', 'Registro merci in deposito'  UNION ALL
SELECT '4.2.16', 'Registri multiaziendali per CED'  UNION ALL
SELECT '4.2.17', 'Registri carico e scarico per CED'  UNION ALL
SELECT '4.2.18', 'Registro lettere di intenti - ricevute'  UNION ALL
SELECT '4.2.19', 'Registro lettere di intenti - emesse'  UNION ALL
SELECT '4.2.20', 'Libro unico del personale'  UNION ALL
SELECT '4.2.31', 'Scritture ausiliarie di magazzino'  UNION ALL
SELECT '4.2.31.1', 'Aggiornamento registri'  UNION ALL
SELECT '4.2.31.2', 'Check List'  UNION ALL
SELECT '4.2.32', 'Registro rifiuti'  UNION ALL
SELECT '4.3', 'Adempimenti I.V.A.'  UNION ALL
SELECT '4.3.1', 'Liquidazione d''imposta'  UNION ALL
SELECT '4.3.1.1', 'Liquidazione d''imposta'  UNION ALL
SELECT '4.3.1.2', 'Prospetto liquidazioni IVA annuale'  UNION ALL
SELECT '4.3.1.3', 'Prospetto liquidazioni IVA periodi vari'  UNION ALL
SELECT '4.3.2', 'Acconti d''imposta'  UNION ALL
SELECT '4.3.2.1', 'Metodo storico'  UNION ALL
SELECT '4.3.2.2', 'Metodo previsionale'  UNION ALL
SELECT '4.3.2.3', 'Metodo delle operazioni effettuate'  UNION ALL
SELECT '4.3.2.4', 'Mensili con contab. c/o terzi'  UNION ALL
SELECT '4.3.3', 'Richiesta rimborso d''imposta'  UNION ALL
SELECT '4.3.4', 'Plafond esportazioni'  UNION ALL
SELECT '4.3.5', 'Invio modelli Intra'  UNION ALL
SELECT '4.3.7', 'Invio operazioni - Spesometro'  UNION ALL
SELECT '4.3.8', 'Comunicazioni IVA'  UNION ALL
SELECT '4.3.9', 'Dichiarazione annuale IVA'  UNION ALL
SELECT '4.4', 'Adempimenti imposta di registro'  UNION ALL
SELECT '4.4.1', 'Registrazione contratti di locazione'  UNION ALL
SELECT '4.4.2', 'Registrazione distribuzione dividendi'  UNION ALL
SELECT '4.4.3', 'Registrazione altre operazioni'  UNION ALL
SELECT '4.5', 'Adempimenti imposte dirette'  UNION ALL
SELECT '4.5.1', 'Dichiarazione annuale dei redditi'  UNION ALL
SELECT '4.5.2', 'Dichiarazione IRAP'  UNION ALL
SELECT '4.5.3', 'Studi di settore - Ine - Società di comodo'  UNION ALL
SELECT '4.5.4', 'Dichiarazione sostituti d''imposta - Mod.770'  UNION ALL
SELECT '4.5.4.1', 'Modello 770 ritenute rapporti di lavoro'  UNION ALL
SELECT '4.5.4.2', 'Modello 770 ritenute lavoro autonomo e altre'  UNION ALL
SELECT '4.5.5', 'Consolidato fiscale'  UNION ALL
SELECT '4.5.5.1', 'Consolidato fiscale - controllata'  UNION ALL
SELECT '4.5.5.2', 'Consolidato fiscale - controllante'  UNION ALL
SELECT '4.5.6', 'Tassazione per trasparenza'  UNION ALL
SELECT '4.5.6.1', 'Adempimenti della società'  UNION ALL
SELECT '4.5.6.2', 'Adempimenti dei soci'  UNION ALL
SELECT '4.5.7', 'Acconti d''imposta'  UNION ALL
SELECT '4.6', 'Adempimenti imposte diverse'  UNION ALL
SELECT '4.6.1', 'ICI - IMU'  UNION ALL
SELECT '4.6.1.1', 'Acconti / Saldi'  UNION ALL
SELECT '4.6.1.2', 'Dichiarazioni'  UNION ALL
SELECT '4.6.2', 'Tarsu - Tia'  UNION ALL
SELECT '4.6.3', 'Imposta sulla pubblicità'  UNION ALL
SELECT '4.6.4', 'Imposte sostitutive'  UNION ALL
SELECT '4.6.5', 'TARI'  UNION ALL
SELECT '4.6.6', 'TASI'  UNION ALL
SELECT '4.6.6.1', 'Acconti / Saldi'  UNION ALL
SELECT '4.6.6.2', 'Dichiarazioni'  UNION ALL
SELECT '4.6.11', 'MUD'  UNION ALL
SELECT '4.6.21', 'CONAI'  UNION ALL
SELECT '4.7', 'Versamenti e compensazioni'  UNION ALL
SELECT '4.7.1', 'Versamenti imposte e contributi'  UNION ALL
SELECT '4.7.2', 'Compensazioni'  UNION ALL
SELECT '4.7.11', 'Versamenti F24 tributi e contributi'  UNION ALL
SELECT '4.7.12', 'Quadrature COGE / F24'  UNION ALL
SELECT '4.7.13', 'UNIEMENS e contributi previdenziali complementari'  UNION ALL
SELECT '4.7.14', 'Contributi Agenti'  UNION ALL
SELECT '4.7.15', 'CUD e Certificazioni'  UNION ALL
SELECT '4.7.31', 'Ritenute Lavoratori Autonomi'  UNION ALL
SELECT '4.8', 'Adempimenti societari'  UNION ALL
SELECT '4.8.1', 'Adempimenti del bilancio d''esercizio'  UNION ALL
SELECT '4.8.2', 'Deposito del bilancio consolidato'  UNION ALL
SELECT '4.8.3', 'Deposito atti societari'  UNION ALL
SELECT '4.9', 'Cassa'  UNION ALL
SELECT '4.9.1', 'Cassa contante'  UNION ALL
SELECT '4.9.1.1', 'Cassa euro'  UNION ALL
SELECT '4.9.1.2', 'Cassa altre Divise'  UNION ALL
SELECT '4.9.1.3', 'Check Antiriciclaggio'  UNION ALL
SELECT '4.9.2', 'Cassa valori bollati'  UNION ALL
SELECT '4.9.3', 'Cassa assegni'  UNION ALL
SELECT '4.9.4', 'Cassa titoli'  UNION ALL
SELECT '4.9.5', 'Sospesi di cassa'  UNION ALL
SELECT '4.10', 'Banche'  UNION ALL
SELECT '4.10.1', 'Rapporti bancari / affidamenti / garanzie'  UNION ALL
SELECT '4.10.2', 'Rapporti bancari / fidejussioni'  UNION ALL
SELECT '4.10.3', 'Mutui ipotecari/chirografari'  UNION ALL
SELECT '4.10.4', 'Contratti leasing'  UNION ALL
SELECT '4.10.5', 'Contratti di factoring'  UNION ALL
SELECT '4.10.6', 'Finanza derivata'  UNION ALL
SELECT '4.10.7', 'Riconciliazioni banche / contabilità'  UNION ALL
SELECT '4.10.8', 'Altro'  UNION ALL
SELECT '4.11', 'Garanzie assicurative'  UNION ALL
SELECT '4.11.1', 'Polizze automezzi'  UNION ALL
SELECT '4.11.2', 'Polizze rischi diversi'  UNION ALL
SELECT '4.12', 'Situazioni periodiche (econ./patrim./finanz.)'  UNION ALL
SELECT '4.12.1', 'Bilancio periodico - ordinario'  UNION ALL
SELECT '4.12.1.1', 'Bilancio ordinario'  UNION ALL
SELECT '4.12.1.1.A', 'Patrimoniale Attivo'  UNION ALL
SELECT '4.12.1.1.B', 'Patrimoniale Passivo'  UNION ALL
SELECT '4.12.1.1.C', 'Conto Economico'  UNION ALL
SELECT '4.12.1.2', 'Bilancio riclassificato'  UNION ALL
SELECT '4.12.1.3', 'Indici'  UNION ALL
SELECT '4.12.2', 'Bilancio periodico - abbreviato'  UNION ALL
SELECT '4.12.2.1', 'Bilancio abbreviato'  UNION ALL
SELECT '4.12.2.1.A', 'Patrimoniale Attivo'  UNION ALL
SELECT '4.12.2.1.B', 'Patrimoniale Passivo'  UNION ALL
SELECT '4.12.2.1.C', 'Conto Economico'  UNION ALL
SELECT '4.12.2.2', 'Bilancio riclassificato'  UNION ALL
SELECT '4.12.2.3', 'Indici'  UNION ALL
SELECT '4.12.11', 'Dati di periodo - check list delle Situazioni econ./patrim./finanz.'  UNION ALL
SELECT '4.12.12', 'Osservazioni e commenti'  UNION ALL
SELECT '4.13', 'Organismo di vigilanza'  UNION ALL
SELECT '4.13.1', 'Commenti alla relazione periodica dell''Organizmo di Vigilanza'  UNION ALL
SELECT '4.14', 'Antiriciclaggio'  UNION ALL
SELECT '4.14.1', 'Operazioni sospette rilevate e segnalate per antiriciclaggio'  UNION ALL
SELECT '4.14.6', 'Controlli su destinatari obblighi 231/2007 per antiriciclaggio'  UNION ALL
SELECT '4.15', 'Commenti ed osservazioni ulteriori e/o conclusivi per antiriciclaggio'  UNION ALL
SELECT '4.31', 'Test Periodici - Procedure S.C.I.'  UNION ALL
SELECT '4.31.1', 'Ciclo Vendite - Test Procedura'  UNION ALL
SELECT '4.31.2', 'Ciclo Acquisti - Test Procedura'  UNION ALL
SELECT '4.97', 'Discussioni del team'  UNION ALL
SELECT '4.98', 'Allegati Liberi non associati a carte di lavoro'  UNION ALL
SELECT '4.99', 'Tempi di Revisione'  UNION ALL
SELECT '5', 'ATTIVITA'' DI VIGILANZA'  UNION ALL
SELECT '5.0', 'Libri Sociali'  UNION ALL
SELECT '5.0.1', 'Libro degli azionisti'  UNION ALL
SELECT '5.0.2', 'Libro verbali assemblee - decisioni dei soci'  UNION ALL
SELECT '5.0.3', 'Libro verbali riunioni C.d.A.'  UNION ALL
SELECT '5.0.4', 'Libro degli obbligazionisti e certificati obbligazionari'  UNION ALL
SELECT '5.0.5', 'Libro verbali assemblee obbligazionisti'  UNION ALL
SELECT '5.0.6', 'Libro del Collegio Sindacale'  UNION ALL
SELECT '5.0.11', 'Libri diversi'  UNION ALL
SELECT '5.1', 'Ambiente di controllo e comprensione dell''impresa'  UNION ALL
SELECT '5.1.1', 'Filosofia di direzione'  UNION ALL
SELECT '5.1.2', 'Rapporti con l''organo di controllo'  UNION ALL
SELECT '5.1.3', 'Politiche del personale'  UNION ALL
SELECT '5.1.3.1', 'Qualifiche ed integrità del personale'  UNION ALL
SELECT '5.1.3.2', 'Turnover del personale'  UNION ALL
SELECT '5.1.3.3', 'Sicurezza sul lavoro'  UNION ALL
SELECT '5.1.3.4', 'Adempimenti connessi all''amministrazione del personale.'  UNION ALL
SELECT '5.1.4', 'Fattori di rischio specifici'  UNION ALL
SELECT '5.1.4.1', 'Mercato ed andamento della domanda'  UNION ALL
SELECT '5.1.4.2', 'Obsolescenza dei prodotti o servizi'  UNION ALL
SELECT '5.1.4.3', 'Aree geografiche - mercati di sbocco'  UNION ALL
SELECT '5.1.4.4', 'Capacità di innovazione'  UNION ALL
SELECT '5.1.4.5', 'Capacità di autofinanziamento e risultati economici'  UNION ALL
SELECT '5.1.4.6', 'Idoneità all''ottenimento di finanziamenti'  UNION ALL
SELECT '5.1.4.7', 'Continuità delle vendite'  UNION ALL
SELECT '5.1.4.8', 'Operazioni inusuali e infragruppo'  UNION ALL
SELECT '5.1.4.9', 'Operazioni con parti correlate'  UNION ALL
SELECT '5.1.90', 'Altri Aspetti'  UNION ALL
SELECT '5.2', 'Vigilanza sull''osservanza della legge e dello statuto.'  UNION ALL
SELECT '5.2.1', 'Partecipazioni alle assemblee dei soci'  UNION ALL
SELECT '5.2.2', 'Partecipazioni alle eventuali riunioni dell''organo amministrativo'  UNION ALL
SELECT '5.2.3', 'Partecipazioni alle eventuali assemblee degli obbligazionisti'  UNION ALL
SELECT '5.2.4', 'Partecipazioni alle eventuali riunioni di altri organi societari'  UNION ALL
SELECT '5.2.5', 'Informazioni dall''organo amministrativo'  UNION ALL
SELECT '5.2.6', 'Informazioni dagli organi di controllo delle partecipate'  UNION ALL
SELECT '5.2.7', 'Informazioni dal revisore legale'  UNION ALL
SELECT '5.2.8', 'Informazioni da terzi'  UNION ALL
SELECT '5.2.9', 'Informazioni dalla lettura dei libri sociali'  UNION ALL
SELECT '5.2.90', 'Altri Aspetti'  UNION ALL
SELECT '5.3', 'Vigilanza sui principi di corretta amministrazione'  UNION ALL
SELECT '5.3.1', 'Operazioni estranee oggetto sociale'  UNION ALL
SELECT '5.3.2', 'Operazioni in conflitto d''interessi'  UNION ALL
SELECT '5.3.3', 'Operazioni imprudenti o azzardate'  UNION ALL
SELECT '5.3.4', 'Operazioni che compromettono il patrimonio'  UNION ALL
SELECT '5.3.5', 'Operazioni che modificano i diritti dei soci'  UNION ALL
SELECT '5.3.6', 'Operazioni in contrasto con le delibere'  UNION ALL
SELECT '5.3.7', 'Atti pregiudizievoli degli amministratori'  UNION ALL
SELECT '5.3.8', 'Scelte gestionali degli amministratori'  UNION ALL
SELECT '5.3.9', 'Rischiosità ed effetti delle scelte degli amministratori'  UNION ALL
SELECT '5.3.90', 'Altri Aspetti'  UNION ALL
SELECT '5.4', 'Adeguatezza dell''assetto organizzativo'  UNION ALL
SELECT '5.4.1', 'Separazione di responsabilità nelle funzioni'  UNION ALL
SELECT '5.4.2', 'Deleghe e poteri di ciascuna funzione'  UNION ALL
SELECT '5.4.3', 'Verifica del lavoro dei collaboratori'  UNION ALL
SELECT '5.4.4', 'Organigramma'  UNION ALL
SELECT '5.4.5', 'Esercizio effettivo dei poteri'  UNION ALL
SELECT '5.4.6', 'Presenza e competenza del personale'  UNION ALL
SELECT '5.4.7', 'Diffusione delle direttive'  UNION ALL
SELECT '5.4.90', 'Altri Aspetti'  UNION ALL
SELECT '5.5', 'Sistema di Controllo Interno'  UNION ALL
SELECT '5.5.1', 'Verifica del S.C.I. nel suo complesso'  UNION ALL
SELECT '5.5.2', 'Obiettivi strategici'  UNION ALL
SELECT '5.5.3', 'Obiettivi operativi'  UNION ALL
SELECT '5.5.4', 'Obiettivi di reporting'  UNION ALL
SELECT '5.5.5', 'Obiettivi di conformità'  UNION ALL
SELECT '5.5.90', 'Altri Aspetti'  UNION ALL
SELECT '5.6', 'Sistema amministrativo - contabile'  UNION ALL
SELECT '5.6.1', 'Il sistema ammin./contabile nel suo complesso'  UNION ALL
SELECT '5.6.2', 'Rilevazioni contabili'  UNION ALL
SELECT '5.6.3', 'Salvaguardia del patrimonio'  UNION ALL
SELECT '5.6.4', 'Bilancio di esercizio'  UNION ALL
SELECT '5.6.5', 'Sistema informatico'  UNION ALL
SELECT '5.6.90', 'Altri Aspetti'  UNION ALL
SELECT '5.80', 'Colloqui con gli Organi'  UNION ALL
SELECT '5.80.1', 'Colloqui con Organo Amministrativo'  UNION ALL
SELECT '5.80.2', 'Colloqui con Revisore'  UNION ALL
SELECT '5.80.3', 'Colloqui con ODV'  UNION ALL
SELECT '5.80.6', 'Colloqui con cariche apicali'  UNION ALL
SELECT '5.80.8', 'Colloqui con terzi'  UNION ALL
SELECT '5.80.9', 'Colloqui diversi'  UNION ALL
SELECT '5.90', 'Commenti - testo libero'  UNION ALL
SELECT '5.97', 'Discussioni tra sindaci'  UNION ALL
SELECT '5.98', 'Allegati Liberi non associati a carte di lavoro'  UNION ALL
SELECT '5.99', 'Tempi di Revisione'  UNION ALL
SELECT '5.IP', 'Introduzione Personale'  UNION ALL
SELECT '5.VI', 'Verbale d''insediamento Collegio sindacale'  UNION ALL
SELECT '5.VI.1', 'Dichiarazioni ed attività preliminari'  UNION ALL
SELECT '5.VI.3', 'Documentazione'  UNION ALL
SELECT '5.VI.5', 'Pianificazione dell''attività'  UNION ALL
SELECT '5.VI.9', 'Altre attività'  UNION ALL
SELECT '6', 'PIANIFICAZIONE CONTROLLO CONTABILE' UNION ALL
SELECT '6.1', 'Criteri per la scelta della periodicità' UNION ALL
SELECT '6.1.1', 'Settore Operativo' UNION ALL
SELECT '6.1.2', 'Attività specifica esercitata' UNION ALL
SELECT '6.1.3', 'Complessità organizzativa' UNION ALL
SELECT '6.1.4', 'Numerosità e frammentazione delle operazioni' UNION ALL
SELECT '6.1.5', 'Risultanze delle precedenti verifiche' UNION ALL
SELECT '6.1.6', 'Risultanze del precedente revisore' UNION ALL
SELECT '6.1.7', 'Incarichi amministrativi' UNION ALL
SELECT '6.1.8', 'Organizzazione amministrativa' UNION ALL
SELECT '6.1.9', 'Altre' UNION ALL
SELECT '6.1.10', 'Conclusioni' UNION ALL
SELECT '6.11', 'Pianificazione delle sessioni' UNION ALL
SELECT '7', 'PIANIFICAZIONE ATTIVITÀ DI VIGILANZA' UNION ALL
SELECT '7.1', 'Criteri addotatti per la pianificazione' UNION ALL
SELECT '7.11', 'Pianificazione delle sessioni'  UNION ALL
SELECT '9', 'CONCLUSIONI'  UNION ALL
SELECT '9.1', 'Eventi successivi'  UNION ALL
SELECT '9.2', 'Continuità aziendale'  UNION ALL
SELECT '9.3', 'Confronto Materialità'  UNION ALL
SELECT '9.4', 'Sommario delle rettifiche'  UNION ALL
SELECT '9.9', 'Promemoria conclusivo'  UNION ALL
SELECT '9.80', 'Management Letter'  UNION ALL
SELECT '9.80.1', 'Destinatari'  UNION ALL
SELECT '9.80.2', 'Incipit'  UNION ALL
SELECT '9.80.5', 'Separazione di Funzioni'  UNION ALL
SELECT '9.80.7', 'Ingerenza della Direzione'  UNION ALL
SELECT '9.80.8.2', 'Documentazione'  UNION ALL
SELECT '9.80.9', 'Sistema di controllo interno'  UNION ALL
SELECT '9.80.11', 'Idoneità del personale dipendente'  UNION ALL
SELECT '9.80.13', 'Sistema di IT'  UNION ALL
SELECT '9.80.15', 'Prevenzione frodi'  UNION ALL
SELECT '9.80.17', 'Parti correlate'  UNION ALL
SELECT '9.80.21', 'Altri aspetti'  UNION ALL
SELECT '9.80.91', 'Rilievi nei controlli di bilancio'  UNION ALL
SELECT '9.80.99', 'Conclusioni'  UNION ALL
SELECT '9.90', 'Lettera di Attestazione'  UNION ALL
SELECT '9.90.1', 'Bilancio'  UNION ALL
SELECT '9.90.2', 'Incipit'  UNION ALL
SELECT '9.90.3', 'Finalità dell''incarico'  UNION ALL
SELECT '9.90.4', 'Coerenza fra bilancio e relazione sulla gestione'  UNION ALL
SELECT '9.90.5', 'Continuità aziendale'  UNION ALL
SELECT '9.90.6', 'Assetto organizzativo, amministrativo e contabile'  UNION ALL
SELECT '9.90.7', 'Significatività degli errori'  UNION ALL
SELECT '9.90.8', 'Lo scrivente conferma specificamente'  UNION ALL
SELECT '9.90.8.1', 'Scritture contabili'  UNION ALL
SELECT '9.90.8.3', 'Operazioni extracontabili'  UNION ALL
SELECT '9.90.8.4', 'Controllo interno'  UNION ALL
SELECT '9.90.8.5', 'Frodi'  UNION ALL
SELECT '9.90.8.6', 'Fatti censurabili'  UNION ALL
SELECT '9.90.8.7', 'Bilancio'  UNION ALL
SELECT '9.90.9', 'Conclusione'  UNION ALL
SELECT '9.97', 'Discussioni del team'  UNION ALL
SELECT '9.98', 'Allegati Liberi non associati a carte di lavoro'  UNION ALL
SELECT '9.99', 'Tempi di Revisione'  UNION ALL
SELECT '91', 'RELAZIONE SOLO REVISIONE LEGALE'  UNION ALL
SELECT '91.1', 'Giudizio'  UNION ALL
SELECT '91.2', 'Responsabilità degli amministratori per il bilancio e del Collegio sindacale per il bilancio dell’esercizio'  UNION ALL
SELECT '91.3', 'Responsabilità del revisore per la revisione contabile del bilancio d’esercizio'  UNION ALL
SELECT '91.41', 'Richiamo d''informativa'  UNION ALL
SELECT '91.51', 'Altri Aspetti'  UNION ALL
SELECT '91.61', 'Coerenza fra la relazione sulla gestione ed il bilancio'  UNION ALL
SELECT '91.61.1', 'Testo Introduttivo'  UNION ALL
SELECT '91.91', 'Dissenso di un membro dell''Organo di revisione'  UNION ALL
SELECT '91.99', 'Luogo e data di emissione'  UNION ALL
SELECT '92', 'RELAZIONE SOLO REVISIONE LEGALE'  UNION ALL
SELECT '92.1', 'Giudizio'  UNION ALL
SELECT '92.2', 'Responsabilità degli amministratori per il bilancio e del Collegio sindacale per il bilancio dell’esercizio'  UNION ALL
SELECT '92.3', 'Responsabilità del revisore per la revisione contabile del bilancio d’esercizio'  UNION ALL
SELECT '92.41', 'Richiamo d''informativa'  UNION ALL
SELECT '92.51', 'Altri Aspetti'  UNION ALL
SELECT '92.61', 'Coerenza fra la relazione sulla gestione ed il bilancio'  UNION ALL
SELECT '92.91', 'Dissenso di un membro dell''Organo di revisione'  UNION ALL
SELECT '92.99', 'Luogo e data di emissione'  UNION ALL
SELECT '93', 'RELAZIONE SULL''ATTIVITA'' DI VIGILANZA'  UNION ALL
SELECT '93.1', 'Destinatari e Bilancio oggetto della revisione'  UNION ALL
SELECT '93.6', 'B1 - Attività di vigilanza ex art. 2403 e segg. Cod. Civ.'  UNION ALL
SELECT '93.6.1', 'Testo introduttivo'  UNION ALL
SELECT '93.6.2', 'Opzioni per vigilanza legge e statuto'  UNION ALL
SELECT '93.6.3', 'Opzioni per partecipazioni assemblee, ecc.'  UNION ALL
SELECT '93.6.4', 'Opzioni per acquisizione informazioni affari sociali'  UNION ALL
SELECT '93.6.5', 'Opzioni per acquisizione informazioni sulla gestione'  UNION ALL
SELECT '93.6.6', 'Opzioni per informazioni organo controllo partecipate'  UNION ALL
SELECT '93.6.7', 'Opzioni per preposto a sistema di controllo interno'  UNION ALL
SELECT '93.6.8', 'Opzioni per Organismo di vigilanza'  UNION ALL
SELECT '93.6.9', 'Opzioni per assetto organizzativo'  UNION ALL
SELECT '93.6.10', 'Opzioni per sistema amministrativo contabile'  UNION ALL
SELECT '93.6.11', 'Opzioni per ispezioni'  UNION ALL
SELECT '93.6.12', 'Opzioni per denunzie'  UNION ALL
SELECT '93.6.13', 'Opzioni per pareri'  UNION ALL
SELECT '93.6.14', 'Opzioni per fatti significativi'  UNION ALL
SELECT '93.6.90', 'Altri aspetti'  UNION ALL
SELECT '93.11', 'B2 - Bilancio'  UNION ALL
SELECT '93.11.1', 'Testo introduttivo'  UNION ALL
SELECT '93.11.2', 'Struttura del bilancio'  UNION ALL
SELECT '93.11.3', 'Relazione sulla gestione'  UNION ALL
SELECT '93.11.4', 'Deroghe formazione bilancio (art. 2423, c. 5, Cod. Civ.)'  UNION ALL
SELECT '93.11.5', 'Iscrizione costi di impianto ed ampliamento'  UNION ALL
SELECT '93.11.6', 'Iscrizione costi di sviluppo'  UNION ALL
SELECT '93.11.8', 'Iscrizione costi di avviamento'  UNION ALL
SELECT '93.11.90', 'Altri aspetti'  UNION ALL
SELECT '93.21', 'B3 - Osservazioni e proposte in ordine all’approvazione del bilancio'  UNION ALL
SELECT '93.31', 'Dissenso di un membro dell''Organo di controllo'  UNION ALL
SELECT '93.99', 'Luogo e data di emissione'  UNION ALL
SELECT '94', 'RELAZIONE SULL''ATTIVITA'' DI VIGILANZA'  UNION ALL
SELECT '94.1', 'Destinatari e Bilancio oggetto della revisione'  UNION ALL
SELECT '94.6', 'B1 - Attività di vigilanza ex art. 2403 e segg. Cod. Civ.'  UNION ALL
SELECT '94.6.1', 'Testo introduttivo'  UNION ALL
SELECT '94.6.2', 'Opzioni per vigilanza legge e statuto'  UNION ALL
SELECT '94.6.3', 'Opzioni per partecipazioni assemblee, ecc.'  UNION ALL
SELECT '94.6.4', 'Opzioni per acquisizione informazioni affari sociali'  UNION ALL
SELECT '94.6.5', 'Opzioni per acquisizione informazioni sulla gestione'  UNION ALL
SELECT '94.6.6', 'Opzioni per informazioni organo controllo partecipate'  UNION ALL
SELECT '94.6.7', 'Opzioni per preposto a sistema di controllo interno'  UNION ALL
SELECT '94.6.8', 'Opzioni per Organismo di vigilanza'  UNION ALL
SELECT '94.6.9', 'Opzioni per assetto organizzativo'  UNION ALL
SELECT '94.6.10', 'Opzioni per sistema amministrativo contabile'  UNION ALL
SELECT '94.6.11', 'Opzioni per ispezioni'  UNION ALL
SELECT '94.6.12', 'Opzioni per denunzie'  UNION ALL
SELECT '94.6.13', 'Opzioni per pareri'  UNION ALL
SELECT '94.6.14', 'Opzioni per fatti significativi'  UNION ALL
SELECT '94.6.90', 'Altri aspetti'  UNION ALL
SELECT '94.11', 'B2 - Bilancio'  UNION ALL
SELECT '94.11.1', 'Testo introduttivo'  UNION ALL
SELECT '94.11.2', 'Struttura del bilancio'  UNION ALL
SELECT '94.11.3', 'Relazione sulla gestione'  UNION ALL
SELECT '94.11.4', 'Deroghe formazione bilancio (art. 2423, c. 5, Cod. Civ.)'  UNION ALL
SELECT '94.11.5', 'Iscrizione costi di impianto ed ampliamento'  UNION ALL
SELECT '94.11.6', 'Iscrizione costi di sviluppo'  UNION ALL
SELECT '94.11.8', 'Iscrizione costi di avviamento'  UNION ALL
SELECT '94.11.90', 'Altri aspetti'  UNION ALL
SELECT '94.21', 'B3 - Osservazioni e proposte in ordine all’approvazione del bilancio'  UNION ALL
SELECT '94.31', 'Dissenso di un membro dell''Organo di controllo'  UNION ALL
SELECT '94.99', 'Luogo e data di emissione'  UNION ALL
SELECT '95', 'RELAZIONE REVISIONE LEGALE E ATTIVITA'' DI VIGILANZA'  UNION ALL
SELECT '95.1', 'Destinatari e Bilancio oggetto della revisione'  UNION ALL
SELECT '95.99', 'Luogo e data di emissione'  UNION ALL
SELECT '95.100', 'SEZIONE A'  UNION ALL
SELECT '95.101', 'Giudizio'  UNION ALL
SELECT '95.101.01', 'Giudizio Positivo'  UNION ALL
SELECT '95.101.01.1', 'Giudizio Positivo'  UNION ALL
SELECT '95.101.01.2', 'Elementi alla base del giudizio'  UNION ALL
SELECT '95.101.11', 'Giudizio con rilievi'  UNION ALL
SELECT '95.101.11.1', 'Giudizio con rilievi'  UNION ALL
SELECT '95.101.11.2', 'Elementi alla base del giudizio con rilievi'  UNION ALL
SELECT '95.101.11.6', 'Elementi alla base del giudizio per limitazione all''attività di controllo'  UNION ALL
SELECT '95.101.11.11', 'Elementi alla base del giudizio per mancanza di sufficienti elementi probativi'  UNION ALL
SELECT '95.101.21', 'Giudizio negativo'  UNION ALL
SELECT '95.101.21.1', 'Giudizio negativo'  UNION ALL
SELECT '95.101.21.2', 'Elementi alla base del giudizio'  UNION ALL
SELECT '95.101.31', 'Dichiarazione di impossibilità di esprimere un giudizio'  UNION ALL
SELECT '95.101.31.1', 'Dichiarazione di impossibilità di esprimere un giudizio'  UNION ALL
SELECT '95.101.31.2', 'Elementi alla base dichiarazione di impossibilità di esprimere un giudizio'  UNION ALL
SELECT '95.102', 'Responsabilità degli amministratori per il bilancio e del Collegio sindacale per il bilancio dell’esercizio'  UNION ALL
SELECT '95.103', 'Responsabilità del revisore per la revisione contabile del bilancio d’esercizio'  UNION ALL
SELECT '95.141', 'Richiamo d''informativa'  UNION ALL
SELECT '95.151', 'Altri Aspetti'  UNION ALL
SELECT '95.161', 'Coerenza fra la relazione sulla gestione ed il bilancio'  UNION ALL
SELECT '95.161.1', 'Testo Introduttivo'  UNION ALL
SELECT '95.161.2', 'Coerenza fra la relazione sulla gestione ed il bilancio'  UNION ALL
SELECT '95.191', 'Dissenso di un membro dell''Organo di revisione'  UNION ALL
SELECT '95.300', 'SEZIONE B'  UNION ALL
SELECT '95.306', 'B1 - Attività di vigilanza ex art. 2403 e segg. Cod. Civ.'  UNION ALL
SELECT '95.306.1', 'Testo introduttivo'  UNION ALL
SELECT '95.306.2', 'Opzioni per vigilanza legge e statuto'  UNION ALL
SELECT '95.306.3', 'Opzioni per partecipazioni assemblee, ecc.'  UNION ALL
SELECT '95.306.4', 'Opzioni per acquisizione informazioni affari sociali'  UNION ALL
SELECT '95.306.5', 'Opzioni per acquisizione informazioni sulla gestione'  UNION ALL
SELECT '95.306.6', 'Opzioni per informazioni organo controllo partecipate'  UNION ALL
SELECT '95.306.7', 'Opzioni per preposto a sistema di controllo interno'  UNION ALL
SELECT '95.306.8', 'Opzioni per Organismo di vigilanza'  UNION ALL
SELECT '95.306.9', 'Opzioni per assetto organizzativo'  UNION ALL
SELECT '95.306.10', 'Opzioni per sistema amministrativo contabile'  UNION ALL
SELECT '95.306.11', 'Opzioni per ispezioni'  UNION ALL
SELECT '95.306.12', 'Opzioni per denunzie'  UNION ALL
SELECT '95.306.13', 'Opzioni per pareri'  UNION ALL
SELECT '95.306.14', 'Opzioni per fatti significativi'  UNION ALL
SELECT '95.306.90', 'Altri aspetti'  UNION ALL
SELECT '95.311', 'B2 - Bilancio'  UNION ALL
SELECT '95.311.1', 'Testo introduttivo'  UNION ALL
SELECT '95.311.2', 'Struttura del bilancio'  UNION ALL
SELECT '95.311.3', 'Relazione sulla gestione'  UNION ALL
SELECT '95.311.4', 'Deroghe formazione bilancio (art. 2423, c. 5, Cod. Civ.)'  UNION ALL
SELECT '95.311.5', 'Iscrizione costi di impianto ed ampliamento'  UNION ALL
SELECT '95.311.6', 'Iscrizione costi di sviluppo'  UNION ALL
SELECT '95.311.8', 'Iscrizione costi di avviamento'  UNION ALL
SELECT '95.311.90', 'Altri aspetti'  UNION ALL
SELECT '95.321', 'B3 - Osservazioni e proposte in ordine all’approvazione del bilancio'  UNION ALL
SELECT '95.331', 'Dissenso di un membro dell''Organo di controllo'  UNION ALL
SELECT 'ISQC', 'Controllo Interno della Qualità della Revisione'  UNION ALL
SELECT 'ISQC.1', 'Responsabilità apicali'  UNION ALL
SELECT 'ISQC.1.1', 'Responsabile della revisione'  UNION ALL
SELECT 'ISQC.1.11', 'Responsabile del riesame della qualità'  UNION ALL
SELECT 'ISQC.10', 'Risorse umane'  UNION ALL
SELECT 'ISQC.10.1', 'Team di Revisione - componenti'  UNION ALL
SELECT 'ISQC.20', 'Principi etici applicabili'  UNION ALL
SELECT 'ISQC.20.1', 'Premessa'  UNION ALL
SELECT 'ISQC.20.2', 'Destinatari'  UNION ALL
SELECT 'ISQC.20.3', 'Applicazione del codice'  UNION ALL
SELECT 'ISQC.20.4', 'Violazione del codice etico'  UNION ALL
SELECT 'ISQC.20.5', 'Principi di riferimento'  UNION ALL
SELECT 'ISQC.20.6', 'Il Principio di legalità'  UNION ALL
SELECT 'ISQC.20.7', 'Rispetto della persona'  UNION ALL
SELECT 'ISQC.20.8', 'Rispetto della libera concorrenza'  UNION ALL
SELECT 'ISQC.20.9', 'Correttezza e completezza dell''informazione'  UNION ALL
SELECT 'ISQC.20.10', 'Trattamento e riservatezza delle informazioni'  UNION ALL
SELECT 'ISQC.20.11', 'Conflitto di interessi'  UNION ALL
SELECT 'ISQC.20.12', 'Utilizzo di attrezzature, dispositivi e strutture del revisore'  UNION ALL
SELECT 'ISQC.20.13', 'I clienti'  UNION ALL
SELECT 'ISQC.20.14', 'I fornitori, i partner ed i collaboratori'  UNION ALL
SELECT 'ISQC.20.15', 'Pubblica amministrazione'  UNION ALL
SELECT 'ISQC.20.50', 'Altra Disciplina'  UNION ALL
SELECT 'ISQC.20.60', 'Altra Disciplina Bis'  UNION ALL
SELECT 'ISQC.20.70', 'Altra Disciplina Ter'  UNION ALL
SELECT 'ISQC.30', 'Svolgimento dell''incarico'  UNION ALL
SELECT 'ISQC.30.1', 'Direzione'  UNION ALL
SELECT 'ISQC.30.11', 'Supervisione'  UNION ALL
SELECT 'ISQC.30.21', 'Riesame del lavoro da parte dei membri più esperti'  UNION ALL
SELECT 'ISQC.30.31', 'Riesame del lavoro svolto da parte del responsabile dell''incarico'  UNION ALL
SELECT 'ISQC.30.41', 'Consultazione'  UNION ALL
SELECT 'ISQC.30.51', 'Riesame della qualità dell''incarico'  UNION ALL
SELECT 'ISQC.40', 'Monitoraggio'  UNION ALL
SELECT 'ISQC.40.1', 'Attività di Monitoraggio'  UNION ALL
SELECT 'ISQC.90', 'Programma e tempi di lavoro'  UNION ALL
SELECT 'ISQC.90.11', 'Comprensione - rischio - pianificazione'  UNION ALL
SELECT 'ISQC.90.21', 'Controllo del Bilancio'  UNION ALL
SELECT 'ISQC.90.31', 'Conclusioni - Review - Relazione'  UNION ALL
SELECT 'ISQC.90.41', 'Altre attività'  UNION ALL
SELECT 'ISQC.90.51', 'Verifiche periodiche'  UNION ALL
SELECT 'ISQC.90.99', 'Riepilogo dei tempi di lavoro'  UNION ALL
SELECT 'ISQC.98', 'Allegati Liberi non associati a carte di lavoro'

GO

