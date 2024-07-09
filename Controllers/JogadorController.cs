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
    public class JogadorController : ControllerBase // Qualquer Controller sempre vai herdar de uma ControllerBase
    {
        [HttpGet("Informacoes")]
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
         * 
         * Campos do banco de dados: nome, numero, idclube, salario
         */

        [HttpPost("Informacoes")]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequest), (int)HttpStatusCode.BadRequest)]
        public IActionResult InserirJogador([FromBody] JogadorRequest jogadorRequest)
        {
            if (String.IsNullOrEmpty(jogadorRequest.Nome))
                return BadRequest("É necessário informar o nome do jogador.");

            string connectionString = "Server=localhost;Port=3306;Database=sys;Uid=root;Pwd=admin;";

            MySqlConnection connection = new MySqlConnection(connectionString);

            string query = $@"INSERT INTO sys.jogadores (nome, numero, idclube, salario) VALUES (@nome, @numero, @idclube, @salario)";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@nome", jogadorRequest.Nome);
            command.Parameters.AddWithValue("@numero", jogadorRequest.Numero);
            command.Parameters.AddWithValue("@idclube", jogadorRequest.idClube);
            command.Parameters.AddWithValue("@salario", jogadorRequest.Salario.ToString().Replace(',', '.'));

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
