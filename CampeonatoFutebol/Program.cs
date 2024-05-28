using CampeonatoFutebol;
using System.Data;
using Microsoft.Data.SqlClient;
using System.ComponentModel;

SqlConnection ConectarBD()
{
    Conexao_Banco conn = new Conexao_Banco();
    SqlConnection conexaosql = new SqlConnection(conn.Caminho());
    try
    {
      conexaosql.Open();
    } catch(SqlException e)
    {
        Console.WriteLine(e.ToString());
    } catch(Exception e)
    {
        e.ToString();
    }
    return conexaosql;
}
# region Verificações
int VerificarQTDEquipe()
{
    var conexaosql = ConectarBD();
    SqlCommand cmd = new SqlCommand("Select Count(*) From Equipe", conexaosql);
    cmd.CommandType = CommandType.Text;
    int count = (int)cmd.ExecuteScalar();
    return count;
}
bool VerificarExisteTime(int id)
{
    var conexaosql = ConectarBD();
    SqlCommand cmd = new SqlCommand($"Select Count (*) From Equipe Where id = {id}", conexaosql);
    cmd.CommandType = CommandType.Text;
    int count = (int)cmd.ExecuteScalar();
    if (count == 1)
    {
        return true;
    }
    else
    {
        return false;
    }
}
bool VerificarExisteJogo(int timecasa, int timevisitante)
{
    var conexaosql = ConectarBD();
    SqlCommand cmd = new SqlCommand($"Select Count (*) From Jogo WHERE timeCasa = {timecasa} AND timeVisitante = {timevisitante}", conexaosql);
    cmd.CommandType = CommandType.Text;
    int count = (int)cmd.ExecuteScalar();
    if (count == 1)
    {
        return true;
    }
    else
    {
        return false;
    }
}
bool VerificarFinalCampeonato()
{
    
        
        if (VerificarQtdJogo() == (VerificarQTDEquipe() * VerificarQTDEquipe() - VerificarQTDEquipe())) 
        {
            return true;
        }
        else
        {
            return false;
        }
    
}
int VerificarQtdJogo()
{
    var conexaosql = ConectarBD();
    SqlCommand cmd = new SqlCommand("Select Count(*) From Jogo", conexaosql);
    cmd.CommandType = CommandType.Text;
    int count = (int)cmd.ExecuteScalar();
    return count;
}
#endregion
#region TryCatchVariaveis
int TryCatchInt()
{
    var value = 0;
    do
    {
        try
        {
            value = int.Parse(Console.ReadLine());
            break;
        }
        catch (FormatException)
        {
            Console.WriteLine("Digite um valor inteiro valido");
        }catch(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    } while (true);
    return value;
}
#endregion
#region Inserções
void InserirJogo()
{
    var conexaosql = ConectarBD();
    if (VerificarQtdJogo() == (VerificarQTDEquipe() * VerificarQTDEquipe() - VerificarQTDEquipe()))
    {
        Console.WriteLine("Todas as possiveis combinações de jogos ja estão inseridas");
    }
    else
    {

        try
        {
            SqlCommand cmd = new SqlCommand("[dbo].[Inserir_Jogo]", conexaosql);
            cmd.CommandType = CommandType.StoredProcedure;
            Console.WriteLine(">>>>> Time da casa <<<<<");
            EquipePorID(0);
            Console.WriteLine("Digite o ID do time da casa: ");
            var time_casa = TryCatchInt();
            while (!VerificarExisteTime(time_casa))
            {
                Console.WriteLine("Time não encontrado na base da dados");
                Console.WriteLine("Digite o ID do time da casa: ");
                time_casa = int.Parse(Console.ReadLine());
            }
            Console.WriteLine("\n\n");
            Console.WriteLine(">>>>> Time visitante <<<<<");
            EquipePorID(time_casa);
            Console.WriteLine("Digite o ID do time visitante: ");
            var time_visitante = TryCatchInt();
            while (!VerificarExisteTime(time_visitante))
            {
                Console.WriteLine("Time não encontrado na base da dados");
                Console.WriteLine("Digite o ID do time visitante: ");
                time_visitante = int.Parse(Console.ReadLine());
            }
            while (time_casa == time_visitante)
            {
                Console.WriteLine("Não é possivel digiar duas vezes o mesmo time por favor escolha outro time");
                EquipePorID(time_casa);
                Console.WriteLine("Digite o ID do time visitante: ");
                time_visitante = int.Parse(Console.ReadLine());
                while (!VerificarExisteTime(time_visitante))
                {
                    Console.WriteLine("Time não encontrado na base da dados");
                    Console.WriteLine("Digite o ID do time visitante: ");
                    time_visitante = int.Parse(Console.ReadLine());
                }
            }
            if (VerificarExisteJogo(time_casa, time_visitante))
            {
                Console.WriteLine("Jogo ja existe na base de dados");
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@id_time_casa", SqlDbType.Int)).Value = time_casa;
                cmd.Parameters.Add(new SqlParameter("@id_time_visitante", SqlDbType.Int)).Value = time_visitante;
                Console.WriteLine("Gols time da casa: ");
                cmd.Parameters.Add(new SqlParameter("@gols_time_casa", SqlDbType.Int)).Value = TryCatchInt();
                Console.WriteLine("Gols time visitante");
                cmd.Parameters.Add(new SqlParameter("@gols_time_visitante", SqlDbType.Int)).Value = TryCatchInt();
                var returnValue = cmd.ExecuteReader();
                Console.WriteLine("Jogo inserido");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            conexaosql.Close();
        }
    }
}
void InserirEquipe()
{
    var conexaosql = ConectarBD();
    if (VerificarQTDEquipe() < 5)
    {
    try
    {
        SqlCommand cmd = new SqlCommand("[dbo].[Inserir_Equipe]", conexaosql);
        cmd.CommandType = CommandType.StoredProcedure;
        Console.WriteLine("Nome do time: ");
        cmd.Parameters.Add(new SqlParameter("@nome_time", SqlDbType.VarChar)).Value = Console.ReadLine();
        Console.WriteLine("Apelido time: ");
        cmd.Parameters.Add(new SqlParameter("@apelido_time", SqlDbType.VarChar)).Value = Console.ReadLine();
        Console.WriteLine("Data Criação: ");
        cmd.Parameters.Add(new SqlParameter("@data_criacao_time", SqlDbType.Date)).Value = Console.ReadLine();
        var returnValue = cmd.ExecuteReader();
        Console.WriteLine("Equipe inserida");
    }
    catch (Exception e)
    {
        Console.WriteLine("Erro ?");
    }
    finally
    {
        conexaosql.Close();
    }
    }
    else
    {
        Console.WriteLine("Não é possivel inserir mais times pois ja possui a quantidade maxima(5)");
    }
}
#endregion
#region selects
void MostrarTimeMaisGolsSofridos()
{
    var conexaosql = ConectarBD();
    SqlCommand cmd = new SqlCommand("[dbo].[MostrarTimeMaisGolsSofridos]", conexaosql);
    cmd.CommandType = CommandType.StoredProcedure;
    Console.WriteLine(">>>>> Time com mais gols sofridos <<<<<");
    using (SqlDataReader reader = cmd.ExecuteReader())
        while (reader.Read())
        {
            string nome = reader["nome"].ToString();
            int totGolsMarcados = int.Parse(reader["totalGolsSofridos"].ToString());

            Console.WriteLine($"Nome: {nome}\nTotal de gols sofridos: {totGolsMarcados}");
        }
    conexaosql.Close();
}
void MostrarJogos()
{
    if(VerificarQtdJogo() == 0)
    {
        Console.WriteLine("Nenhum jogo cadastrado até o momento");
    }
    else
    {
        var conexaosql = ConectarBD();
        try
        {
            var count = 1;
            SqlCommand cmd = new SqlCommand("Select codigo,timeCasa,timeVisitante,golsCasa,golsVis,totalGols From Jogo", conexaosql);
            using (SqlDataReader reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    Console.WriteLine($">>>>>Jogo {count}<<<<<");
                    int id = int.Parse(reader["codigo"].ToString());
                    int timeCasa = int.Parse(reader["timeCasa"].ToString());
                    int timeVisitante = int.Parse(reader["timeVisitante"].ToString());
                    int golsCasa = int.Parse(reader["golsCasa"].ToString());
                    int golsVisitante = int.Parse(reader["golsVis"].ToString());
                    Console.WriteLine($"codigo jogo: {id}\nTime da casa {timeCasa}\nTime visitante: {timeVisitante}\nQuantidade gols time da casa: {golsCasa}\nQuantidade gols time visitante: {golsVisitante}");
                    Console.WriteLine("=================================");
                    count++;
                }
        }catch(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            conexaosql.Close();
        }
    }
}
void MostrarJogoMaisGols()
{
    var conexaosql = ConectarBD();
    SqlCommand cmd = new SqlCommand("[dbo].[MostrarJogoMaisGols]", conexaosql);
    cmd.CommandType = CommandType.StoredProcedure;
    Console.WriteLine(">>>>> Jogo com mais gols <<<<<");
    using (SqlDataReader reader = cmd.ExecuteReader())
        while (reader.Read())
        {
            int codigo = int.Parse(reader["codigo"].ToString());
            string nomeTC = reader["TimeCasa"].ToString();
            string nomeTV = reader["TimeVisitante"].ToString();
            int golsCasa = int.Parse(reader["golsCasa"].ToString());
            int golsVis = int.Parse(reader["golsVis"].ToString());
            int totGols = int.Parse(reader["totalGols"].ToString());

            Console.WriteLine($"Codigo: {codigo}\nTime Casa: {nomeTC}\nTime Visitante: {nomeTV}\nGols time da casa: {golsCasa}\nGols time visitante {golsVis}\nTotal de gols: {totGols} ");
        }
    conexaosql.Close();
}
void MostrarMaisGolsJogo()
{
    var conexaosql = ConectarBD();
    SqlCommand cmd = new SqlCommand("[dbo].[MostrarMaiorNumGolsPartida]", conexaosql);
    cmd.CommandType = CommandType.StoredProcedure;
    using (SqlDataReader reader = cmd.ExecuteReader())
    while (reader.Read())
        {
            string nomeTime = reader["NOME_TIME"].ToString();
            int qtdGols = int.Parse(reader["quantidadeGols"].ToString());
            Console.WriteLine($"Time: {nomeTime}\nMaior numero de gols em uma partida: {qtdGols}");
        }
}
void MostrarCampeao()
{
    var conexaosql = ConectarBD();
    SqlCommand cmd = new SqlCommand("[dbo].[MostrarCampeao]", conexaosql);
    cmd.CommandType = CommandType.StoredProcedure;
    Console.WriteLine(">>>>> CAMPEAO <<<<<");
    using (SqlDataReader reader = cmd.ExecuteReader())
    while (reader.Read())
        {
            string nome = reader["nome"].ToString();
            int pontuacao = int.Parse(reader["pontuacao"].ToString());
            int totGolsMarcados = int.Parse(reader["totalGolsMarcados"].ToString());
            int totGolsSofridos = int.Parse(reader["totalGolsSofridos"].ToString());
            Console.WriteLine($"Nome: {nome}\nPontuação: {pontuacao}\nTotal de gols marcados: {totGolsMarcados}\nTotal de gols sofridos: {totGolsSofridos}");
        }
    conexaosql.Close();
}
void MostrarTimeMaisGolsMarcados()
{
    var conexaosql = ConectarBD();
    SqlCommand cmd = new SqlCommand("[dbo].[MostrarTimeMaisGolsMarcados]", conexaosql);
    cmd.CommandType = CommandType.StoredProcedure;
    Console.WriteLine(">>>>> Time com mais gols <<<<<");
    using (SqlDataReader reader = cmd.ExecuteReader())
        while (reader.Read())
        {
            string nome = reader["nome"].ToString();
            int totGolsMarcados = int.Parse(reader["totalGolsMarcados"].ToString());
           
            Console.WriteLine($"Nome: {nome}\nTotal de gols marcados: {totGolsMarcados}");
        }
    conexaosql.Close();
}
void MostrarRanking()
{
    var conexaosql = ConectarBD();
    SqlCommand cmd = new SqlCommand("[dbo].[MostrarRanking]", conexaosql);
    cmd.CommandType = CommandType.StoredProcedure;
    Console.WriteLine(">>>>> RANKING <<<<<");
    using (SqlDataReader reader = cmd.ExecuteReader())
        while (reader.Read())
        {
            Console.WriteLine("===============================");
            string nome = reader["nome"].ToString();
            int pontuacao = int.Parse(reader["pontuacao"].ToString());
            int totGolsMarcados = int.Parse(reader["totalGolsMarcados"].ToString());
            int totGolsSofridos = int.Parse(reader["totalGolsSofridos"].ToString());
            Console.WriteLine($"Nome: {nome}\nPontuação: {pontuacao}\nTotal de gols marcados: {totGolsMarcados}\nTotal de gols sofridos: {totGolsSofridos}");
            Console.WriteLine("===============================");
        }
    conexaosql.Close();
}
void MostrarEquipe()
{
    var conexaosql = ConectarBD();
    var count = 1;
    SqlCommand cmd = new SqlCommand("Select id,nome,apelido,dataCriacao,pontuacao,totalGolsMarcados,totalGolsSofridos From Equipe", conexaosql);
    using (SqlDataReader reader = cmd.ExecuteReader())
        while (reader.Read())
        {
            Console.WriteLine($">>>>>Equipe {count}<<<<<");
            int id = int.Parse(reader["id"].ToString());
            string nome = reader["nome"].ToString();
            string apelido = reader["apelido"].ToString();
            string data =reader["dataCriacao"].ToString();
            int pontuacao = int.Parse(reader["pontuacao"].ToString());
            int totGolsMarcados = int.Parse(reader["totalGolsMarcados"].ToString());
            int totGolsSofridos = int.Parse(reader["totalGolsSofridos"].ToString());
            Console.WriteLine($"Id time: {id}\nNome: {nome}\nApelido: {apelido}\nData Criacao: {data.Substring(0,10)}\nPontuação: {pontuacao}\nTotal de gols marcados: {totGolsMarcados}\nTotal de gols sofridos: {totGolsSofridos}");
            Console.WriteLine("=================================");
            count++;
        }
    conexaosql.Close();
}
#endregion
void ReiniciarCampeonato()
{
    var conexaosql = ConectarBD();
    try
    {
        SqlCommand cmd = new SqlCommand("[dbo].[IniciarCampeonato]", conexaosql);
        cmd.CommandType = CommandType.StoredProcedure;
        var returnValue = cmd.ExecuteNonQuery();
        Console.WriteLine("Campeonato Iniciado!");
        Console.ReadKey();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
    }finally
    {
        conexaosql.Close();
    }
}
void ReiniciarMesmoTime()
{
    var conexaosql = ConectarBD();
    try
    {
        SqlCommand cmd = new SqlCommand("[dbo].[ReiniciarCampeonato]", conexaosql);
        cmd.CommandType = CommandType.StoredProcedure;
        var returnValue = cmd.ExecuteNonQuery();
        Console.WriteLine("Pontuações e jogos zerados!");
        Console.ReadKey();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
    }
    finally
    {
        conexaosql.Close();
    }
}
void EquipePorID(int i)
{
        var conexaosql = ConectarBD();
        SqlCommand cmd = new SqlCommand($"Select id,nome From Equipe where id != {i} order by id asc", conexaosql);
        using (SqlDataReader reader = cmd.ExecuteReader())
            while (reader.Read())
            {
               
                int id = int.Parse(reader["id"].ToString());
                string nome = reader["nome"].ToString();
                Console.WriteLine($"ID: {id}\nNome: {nome}");
                Console.WriteLine("=================================");
            
            }
        conexaosql.Close();
}
void Menu()
{
    do 
    {
       while(VerificarQTDEquipe() < 3)
        {
            Console.WriteLine($"Insira o {VerificarQTDEquipe()}° time");
            InserirEquipe();
            Console.Clear();
        }
    Console.WriteLine(">>>>> CAMPEONATO FUTEBOL <<<<<");
    Console.WriteLine("[1] - Inserir Equipe");
    Console.WriteLine("[2] - Consultar Equipes");
    Console.WriteLine("[3] - Inserir Jogo");
    Console.WriteLine("[4] - Mostrar maior numero de gols que cada time fez no jogo");
    Console.WriteLine("[5] - Mostrar Ranking");
    Console.WriteLine("[6] - Mostrar Campeao");
    Console.WriteLine("[7] - Mostrar Jogos");
    Console.WriteLine("[8] - Mostrar time com maior saldo de gols");
    Console.WriteLine("[9] - Mostrar time com mais gols sofridos");
    Console.WriteLine("[10] - Mostrar jogo com maior quantidade de gols");
    Console.WriteLine("[11] - Reiniciar Campeonato");
    Console.WriteLine("[12] - Reiniciar com os mesmo times");
    Console.WriteLine("[0] - Sair");
    Console.WriteLine("Opção >>>> ");
    var op = TryCatchInt();
        switch (op)
        {
            case 0:
                Environment.Exit(0);
                break;
            case 1:
                InserirEquipe();
                break;
            case 2:
                MostrarEquipe();
                break;
            case 3:
                InserirJogo();
                break;
            case 4:
                MostrarMaisGolsJogo();
                break;
            case 5:
                MostrarRanking();
                break;
            case 6:
                MostrarCampeao();
                break;
            case 7:
                MostrarJogos();
                break;
            case 8:
                MostrarTimeMaisGolsMarcados();
                break;
            case 9:
                MostrarTimeMaisGolsSofridos();
                break;
            case 10:
                MostrarJogoMaisGols();
                break;
            case 11:
                ReiniciarCampeonato();
                break;
            case 12:
                ReiniciarMesmoTime();
                break;
        }
        Console.ReadKey();
        Console.Clear();
    } while(true);
}

Menu();
