

ALTER TABLE UTENTIXCLIENTE add 
 UXC_REV_AUTO bit default 0
GO

update UTENTIXCLIENTE set UXC_REV_AUTO = 1 where uxc_ute_id in(select ute_id from utenti where ute_ruo_id = 6)
GO
update UTENTIXCLIENTE set UXC_REV_AUTO = 0 where UXC_REV_AUTO is null
GO

ALTER 
 PROCEDURE [dbo].[SP_UpsertUser]
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
