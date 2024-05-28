CREATE PROCEDURE Inserir_Equipe @nome_time varchar(50), @apelido_time varchar (30),@data_criacao_time date
AS
INSERT INTO Equipe(nome,apelido,dataCriacao)
VALUES (@nome_time,@apelido_time,@data_criacao_time)
;
CREATE OR ALTER PROC ReiniciarCampeonato
AS
	delete JogoMaisGolsTime;
	delete Jogo;
	 UPDATE Equipe
	 SET Pontuacao = 0, TotalGolsMarcados = 0, TotalGolsSofridos = 0;
	
;
CREATE PROCEDURE Inserir_Jogo @id_time_casa int, @id_time_visitante int, @gols_time_casa int, @gols_time_visitante int
AS
INSERT INTO Jogo(timeCasa,timeVisitante,golsCasa,golsVis)
VALUES
(@id_time_casa,@id_time_visitante,@gols_time_casa,@gols_time_visitante)
;
--Time com mais gols marcados
CREATE or Alter Procedure MostrarTimeMaisGolsMarcados
as
	SELECT TOP(1) nome,totalGolsMarcados from Equipe
	order by totalGolsMarcados desc, totalGolsSofridos ASC
	
	RETURN;

CREATE or Alter Procedure MostrarTimeMaisGolsSofridos
as

	SELECT TOP(1) nome,totalGolsSofridos from Equipe
	order by totalGolsSofridos desc, totalGolsMarcados ASC
	
	RETURN;
	
CREATE or ALTER PROCEDURE MostrarJogoMaisGols
AS
	SELECT TOP (1) j.codigo, ec.nome as TimeCasa,ev.nome as TimeVisitante,golsCasa,golsVis,totalGols from Jogo j
	INNER JOIN Equipe AS ec ON (j.timeCasa = ec.id)
	INNER JOIN Equipe AS ev ON(j.timeVisitante = ev.id)
	order by totalGols desc
	RETURN;
CREATE OR ALTER PROCEDURE InserirJogoMaisGolsTime @id_time int, @codigo_jogo int, @quantidade_gols int
AS
	IF NOT EXISTS (SELECT idTime from JogoMaisGols where idTime = @id_time)
	BEGIN
	INSERT INTO JogoMaisGols
	Values (@id_time,@codigo_jogo,@quantidade_gols)
	END
	;
CREATE OR ALTER PROCEDURE MostrarMaiorNumGolsPartida
AS
	SELECT E.NOME AS NOME_TIME,jmg.quantidadeGols FROM JogoMaisGols as jmg
	INNER JOIN Equipe as e ON (e.id = jmg.idTime)
	RETURN;
CREATE OR ALTER PROCEDURE MostrarCampeao
as
	SELECT TOP(1) nome,  pontuacao, totalGolsMarcados, totalGolsSofridos FROM Equipe
	order by pontuacao desc, totalGolsMarcados desc, totalGolsSofridos asc
	RETURN;
CREATE OR ALTER PROCEDURE MostrarRanking
as
	SELECT nome,  pontuacao, totalGolsMarcados, totalGolsSofridos FROM Equipe
	order by pontuacao desc, totalGolsMarcados desc, totalGolsSofridos asc
	RETURN;

CREATE OR ALTER PROCEDURE IniciarCampeonato
AS
-- Excluir tabelas para iniciar um novo campeonato --
DELETE  JogoMaisGols;
DELETE  Jogo;
DELETE Equipe;
DROP TABLE jogoMaisGols;
DROP TABLE Jogo;
DROP TABLE Equipe;
-- CRIAR TABELAS --
CREATE TABLE Equipe
(
	id INT identity(1,1) primary key,
	nome varchar(50) not null,
	apelido varchar(30),
	dataCriacao DATE not null,
	pontuacao int,
	totalGolsMarcados int,
	totalGolsSofridos int,
	constraint uk_nome unique (nome)
);
CREATE TABLE Jogo
(
	codigo int identity(1,1) unique,
	timeCasa int,
	timeVisitante int,
	golsCasa int not null,
	golsVis int not null,
	totalGols int,
	constraint pk_jogo primary key (timeCasa,TimeVisitante),
	constraint fk_time_casa foreign key (timeCasa) References Equipe(id),
	constraint fk_time_visitante foreign key (timeVisitante) References Equipe(id)
);
CREATE TABLE JogoMaisGols
(
	idTime int,
	codigoJogo int,
	quantidadeGols int,
	constraint pk_jogo_mais_gols_time primary key (idTime),
	constraint fk_idTime foreign key (idTime) references Equipe(id),
	constraint fk_codigo_jogo foreign key (codigoJogo) references Jogo(codigo)
);
-- Criar TRIGGERS -- 
EXEC sp_executesql N'
        CREATE OR ALTER TRIGGER Definir_Status_Inicial
        ON Equipe
        AFTER INSERT
        AS
        BEGIN
            DECLARE @id INT;
            
            SELECT @id = id FROM INSERTED;
            
            UPDATE Equipe 
            SET pontuacao = 0, totalGolsMarcados = 0, totalGolsSofridos = 0
            WHERE id = @id;
        END;
    ';
-- ============================================================================================================= --
EXEC sp_executesql N'CREATE or alter TRIGGER Atribuir_Pontuacao
ON Jogo
AFTER INSERT 
AS
	DECLARE
	@id_time_casa int,
	@id_time_visitante int,
	@codigo int,
	@gols_time_casa int,
	@gols_time_visitante int ;
BEGIN
	SELECT @codigo = codigo, @gols_time_casa = golsCasa, @gols_time_visitante = golsVis, @id_time_casa = timeCasa , @id_time_visitante = timeVisitante
	FROM INSERTED
	UPDATE Jogo
	SET totalGols = @gols_time_casa + @gols_time_visitante
	WHERE codigo = @codigo
	-- Atribuir pontuacao e gols time casa e visitante
		if (@gols_time_casa > @gols_time_visitante)
			BEGIN
			UPDATE Equipe
			SET pontuacao += 3, totalGolsMarcados += @gols_time_casa, totalGolsSofridos += @gols_time_visitante
			WHERE id = @id_time_casa
			UPDATE Equipe
			SET totalGolsMarcados += @gols_time_visitante, totalGolsSofridos += @gols_time_casa
			WHERE id = @id_time_visitante
			END
		else if(@gols_time_visitante > @gols_time_casa) 
			BEGIN
				UPDATE Equipe
				SET pontuacao += 5, totalGolsMarcados += @gols_time_visitante, totalGolsSofridos += @gols_time_casa
				WHERE id = @id_time_visitante
				UPDATE Equipe
				SET totalGolsMarcados += @gols_time_casa, totalGolsSofridos += @gols_time_visitante
				WHERE id = @id_time_casa
				END
		else 
			BEGIN
				UPDATE Equipe
				SET pontuacao += 1, totalGolsMarcados += @gols_time_visitante, totalGolsSofridos += @gols_time_casa
				WHERE id = @id_time_visitante
				UPDATE Equipe
				SET pontuacao += 1, totalGolsMarcados += @gols_time_casa, totalGolsSofridos += @gols_time_visitante
				WHERE id = @id_time_casa
			END
END;';
-- ============================================================================================================= --
EXEC sp_executesql N'CREATE OR ALTER TRIGGER Verificar_Jogo_Mais_Gols
ON Jogo
AFTER INSERT
AS
	DECLARE
	@id_time_casa int,
	@id_time_visitante int,
	@gols_time_casa int,
	@gols_time_visitante int,
	@codigo_jogo int,
	@golsJogoMaisGols int
BEGIN 
	SELECT @codigo_jogo = codigo, @id_time_casa = timeCasa, @id_time_visitante = timeVisitante,@gols_time_casa = golsCasa,@gols_time_visitante = golsVis
	FROM INSERTED
	-- Inserir se não existir na tabela JogoMaisGols
	EXEC InserirjogoMaisGolsTime @id_time_casa,@codigo_jogo,@gols_time_casa;
	-- Inserir se não existir na tabela JogoMaisGols de Visitante
	EXEC InserirjogoMaisGolsTime @id_time_visitante,@codigo_jogo,@gols_time_visitante;
    --========================================================================================================================
	--Time da casa
	SELECT @golsJogoMaisGols = quantidadeGols from JogoMaisGols
	WHERE idTime = @id_time_casa
	IF(@golsJogoMaisGols < @gols_time_casa)
		BEGIN
			UPDATE JogoMaisGols
			SET codigoJogo = @codigo_jogo ,quantidadeGols = @golsJogoMaisGols
			WHERE idTime = @id_time_casa
		END
    --========================================================================================================================
--Time visitante
	SELECT @golsJogoMaisGols = quantidadeGols from JogoMaisGols
	WHERE idTime = @id_time_visitante
	IF(@golsJogoMaisGols < @gols_time_visitante)
		BEGIN
			UPDATE JogoMaisGols
			SET codigoJogo = @codigo_jogo ,quantidadeGols = @golsJogoMaisGols
			WHERE idTime = @id_time_visitante
		END
END;';

