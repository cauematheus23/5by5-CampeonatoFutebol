CREATE TRIGGER [Definir_Status_Inicial]
ON Equipe
AFTER INSERT
AS
	DECLARE 
	@id int
BEGIN

	SELECT @id = id from INSERTED
	UPDATE Equipe 
	SET pontuacao = 0, totalGolsMarcados = 0, totalGolsSofridos = 0
	WHERE id = @id
END;

CREATE or alter TRIGGER [Atribuir_Pontuacao]
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
END;

CREATE OR ALTER TRIGGER [Verificar_Jogo_Mais_Gols]
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
			SET codigoJogo = @codigo_jogo ,quantidadeGols = @gols_time_casa
			WHERE idTime = @id_time_casa
		END
    --========================================================================================================================
--Time visitante
	SELECT @golsJogoMaisGols = quantidadeGols from JogoMaisGols
	WHERE idTime = @id_time_visitante
	IF(@golsJogoMaisGols < @gols_time_visitante)
		BEGIN
			UPDATE JogoMaisGols
			SET codigoJogo = @codigo_jogo ,quantidadeGols = @gols_time_visitante
			WHERE idTime = @id_time_visitante
		END
END;