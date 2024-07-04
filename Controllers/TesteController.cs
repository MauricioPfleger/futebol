using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using MySql.Data.MySqlClient;
using futebol.Objetos;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using Org.BouncyCastle.Crypto.Digests;
using Mysqlx.Crud;
using System.Collections.Generic;
using System.Text;

namespace futebol.Controllers
{
    [ApiController] // É uma tag que diz para o visual studio que essa classe é uma API
    [Route("[controller]")] // O nome da nossa Controller ou API vai ser o nome da classe meno a palavra Controller
    public class TesteController : ControllerBase // Qualquer Controller sempre vai herdar de uma ControllerBase
    {
        [HttpGet("olamundo")] // Consulta e o nome do endpoint entre parenteses
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)] // Tipo de reposta produzido, podendo ser definido OK ou Erro
        public IActionResult ConsultarTutor() // Método que irá realizar o que queremos
        {
            return Ok($@"Olá Mundo!!!"); // Retorna alguma informação com sucesso, lembrando que Ok é sucesso.
        }

        [HttpGet("clube")]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequest), (int)HttpStatusCode.BadRequest)]
        public IActionResult ConsultaClube([FromQuery] string? nomeClube, [FromQuery] int? idClube)
        {
            string connectionString = "Server=localhost;Port=3306;Database=sys;Uid=root;Pwd=admin;";

            MySqlConnection connection = new MySqlConnection(connectionString);

            string query = String.Empty;

            if (String.IsNullOrEmpty(nomeClube))
            {
                query = $"SELECT id, nome, trofeus, patrimonio FROM sys.clubes WHERE id = '{idClube}'";
            }
            else
            {
                query = $"SELECT id, nome, trofeus, patrimonio FROM sys.clubes WHERE nome = '{nomeClube}'";
            }

            MySqlCommand command = new MySqlCommand(query, connection);

            connection.Open();

            MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                var clube = new Clube();
                clube.Id = reader.GetInt32("id"); ;
                clube.Nome = reader.GetString("nome"); ;
                clube.Trofeus = reader.GetInt32("trofeus");
                if (!reader.IsDBNull("patrimonio"))
                    clube.Patrimonio = reader.GetDouble("patrimonio");

                connection.Close();
                return Ok(clube);
            }
            else
            {
                connection.Close();
                return BadRequest("Clube não encontrado");
            }
        }

        [HttpGet("jogadores")]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequest), (int)HttpStatusCode.BadRequest)]
        public IActionResult ConsultaJogador([FromQuery] string? nomeJogador, [FromQuery] int? idJogador)
        {
            string connectionString = "Server=localhost;Port=3306;Database=sys;Uid=root;Pwd=admin;";

            MySqlConnection connection = new MySqlConnection(connectionString);

            string query = String.Empty;

            if (String.IsNullOrEmpty(nomeJogador))
            {
                query = $@"SELECT j.id, j.nome, j.numero, j.salario, c.nome nome_clube FROM sys.jogadores j
                    JOIN sys.clubes c on c.id = j.idclube
                    WHERE j.id = {idJogador}";
            }
            else
            {
                query = $@"SELECT j.id, j.nome, j.numero, j.salario, c.nome nome_clube FROM sys.jogadores j
                    JOIN sys.clubes c on c.id = j.idclube 
                    WHERE j.nome = '{nomeJogador}'";
            }

            MySqlCommand command = new MySqlCommand(query, connection);

            connection.Open();

            MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                var jogador = new Jogador();
                jogador.Id = reader.GetInt32("id"); ;
                jogador.Nome = reader.GetString("nome");
                jogador.Numero = reader.GetInt32("numero");
                jogador.Salario = reader.GetDecimal("salario");
                jogador.NomeClube = reader.GetString("nome_clube");
                connection.Close();
                return Ok(jogador);
            }
            else
            {
                connection.Close();
                return BadRequest("Jogador não encontrado");
            }
        }

        [HttpGet("maior-salario")]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequest), (int)HttpStatusCode.BadRequest)]
        public IActionResult MaiorSalario()
        {
            string connectionString = "Server=localhost;Port=3306;Database=sys;Uid=root;Pwd=admin;";

            MySqlConnection connection = new MySqlConnection(connectionString);

            string query = $@"SELECT j.nome, j.numero FROM sys.jogadores j
                WHERE j.salario = (SELECT max(x.salario) FROM sys.jogadores x)";

            MySqlCommand command = new MySqlCommand(query, connection);

            connection.Open();

            MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                var jogadorMaiorSalario = new MaiorSalario();
                jogadorMaiorSalario.Nome = reader.GetString("nome");
                jogadorMaiorSalario.Numero = reader.GetInt32("numero");

                connection.Close();
                return Ok(jogadorMaiorSalario);
            }
            else
            {
                connection.Close();
                return BadRequest("Jogador não encontrado");
            }
        }

        [HttpPost("clube")]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequest), (int)HttpStatusCode.BadRequest)]
        public IActionResult InserirClube([FromBody] ClubeRequest clubeRequest)
        {
            if (String.IsNullOrEmpty(clubeRequest.Nome))
                return BadRequest("É necessário informar o nome do clube.");

            if (clubeRequest.Trofeus == null)
                return BadRequest("É necessário informar a quantidade de troféus.");

            string connectionString = "Server=localhost;Port=3306;Database=sys;Uid=root;Pwd=admin;";

            MySqlConnection connection = new MySqlConnection(connectionString);

            string query = $@"INSERT INTO sys.clubes (nome, trofeus, patrimonio) VALUES (@nome, @troufeus, @patrimonio)";

            MySqlCommand command = new MySqlCommand(query, connection); 
            command.Parameters.AddWithValue("@nome", clubeRequest.Nome);
            command.Parameters.AddWithValue("@troufeus", clubeRequest.Trofeus);
            command.Parameters.AddWithValue("@patrimonio", clubeRequest.Patrimonio.ToString().Replace(',', '.'));               

            connection.Open();

            var linhasAfetas = command.ExecuteNonQuery();

            if (linhasAfetas > 0)
            {
                connection.Close();
                return Ok("Clube cadastrado com sucesso.");
            }
            else
            {
                connection.Close();
                return BadRequest("Clube não foi cadastrado");
            }
        }

        [HttpPut("clube/{idClube}")]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequest), (int)HttpStatusCode.BadRequest)]
        public IActionResult AlterarClube([FromRoute] int idClube, [FromQuery] string? nomeClube, int? quantidadeTrofeus, decimal? patrimonio)
        {
            string connectionString = "Server=localhost;Port=3306;Database=sys;Uid=root;Pwd=admin;";

            MySqlConnection connection = new MySqlConnection(connectionString);

            StringBuilder query = new StringBuilder();
            query.Append("UPDATE CLUBES SET ");

            bool existeParametroInformado = false;

            if (!String.IsNullOrEmpty(nomeClube))
            {
                query.Append("NOME = @nomeClube");
                existeParametroInformado = true;
            }

            if (quantidadeTrofeus != null)
            {
                if (existeParametroInformado)
                {
                    query.Append(",");
                }
                query.Append("TROFEUS = @quantidadeTrofeus");
                existeParametroInformado = true;
            }

            if (patrimonio != null)
            {
                if (existeParametroInformado)
                {
                    query.Append(",");
                }
                query.Append("PATRIMONIO = @patrimonio");
                existeParametroInformado = true;
            }

            if (!existeParametroInformado)
            {
                return BadRequest("Nenhum parâmetro foi informado");
            }
            
            query.Append(" WHERE ID = @idClube");

            MySqlCommand command = new MySqlCommand(query.ToString(), connection);
            command.Parameters.AddWithValue("@quantidadeTrofeus", quantidadeTrofeus);
            command.Parameters.AddWithValue("@idClube", idClube);
            command.Parameters.AddWithValue("@patrimonio", patrimonio);
            command.Parameters.AddWithValue("@nomeClube", nomeClube);

            connection.Open();

            var linhasAfetas = command.ExecuteNonQuery();

            if (linhasAfetas > 0)
            {
                connection.Close();
                return Ok("Atualização ocorreu com sucesso.");
            }
            else
            {
                connection.Close();
                return BadRequest("Não foi atualizado nenhum clube.");
            }
        }

        [HttpDelete("clube/{idClube}")]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequest), (int)HttpStatusCode.BadRequest)]
        public IActionResult ExcluirClubr([FromRoute] int idClube)
        {
            string connectionString = "Server=localhost;Port=3306;Database=sys;Uid=root;Pwd=admin;";

            MySqlConnection connection = new MySqlConnection(connectionString);

            string query = "DELETE FROM sys.clubes WHERE id = @idClube";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@idClube", idClube);

            connection.Open();

            var linhasAfetas = command.ExecuteNonQuery();

            if (linhasAfetas > 0)
            {
                connection.Close();
                return Ok("Exclusão ocorreu com sucesso.");
            }
            else
            {
                connection.Close();
                return BadRequest("Não foi excluído nenhum clube.");
            }
        }

        // ------------------- EXERCICIO -----------------------------------------------------

        /*
         * 1 - É o tipo do parâmetro de entrada, ou a classe que a gente espera receber como informação  
         * 2 - Tabela que iremos inserir a informação
         * 3 - Os campos que iremos preencher na tabela
         * 4 - Os valores que iremos preencher os campos
         * 5 - Campo a ser substituido na query
         * 6 - Valor a ser substituido na query
         * 7 - Campo a ser substituido na query
         * 8 - Valor a ser substituido na query
         * 9 - Campo a ser substituido na query
         * 10 - Valor a ser substituido na query
         * 11 - Campo a ser substituido na query
         * 12 - Valor a ser substituido na query
         */

        [HttpPost("jogador")]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequest), (int)HttpStatusCode.BadRequest)]
        public IActionResult InserirJogador([FromBody] /*?1*/ jogadorRequest)
        {
            if (String.IsNullOrEmpty(jogadorRequest.Nome))
                return BadRequest("É necessário informar o nome do jogador.");

            string connectionString = "Server=localhost;Port=3306;Database=sys;Uid=root;Pwd=admin;";

            MySqlConnection connection = new MySqlConnection(connectionString);

            string query = $@"INSERT INTO /*?2*/ (/*?3*/) VALUES (/*?4*/)";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("/*?5*/", jogadorRequest./*?6*/);
            command.Parameters.AddWithValue("/*?7*/", jogadorRequest./*?8*/);
            command.Parameters.AddWithValue("@idclube", jogadorRequest.idClube);
            command.Parameters.AddWithValue("/*?11*/", jogadorRequest./*?12*/.ToString().Replace(',', '.'));

            connection.Open();

            var linhasAfetas = command.ExecuteNonQuery();

            if (linhasAfetas > 0)
            {
                connection.Close();
                return Ok("Jogador cadastrado com sucesso.");
            }
            else
            {
                connection.Close();
                return BadRequest("Jogador não foi cadastrado");
            }
        }
    }
}
