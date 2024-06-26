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

namespace futebol.Controllers
{
    [ApiController] // � uma tag que diz para o visual studio que essa classe � uma API
    [Route("[controller]")] // O nome da nossa Controller ou API vai ser o nome da classe meno a palavra Controller
    public class TesteController : ControllerBase // Qualquer Controller sempre vai herdar de uma ControllerBase
    {
        [HttpGet("olamundo")] // Consulta e o nome do endpoint entre parenteses
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)] // Tipo de reposta produzido, podendo ser definido OK ou Erro
        public IActionResult ConsultarTutor() // M�todo que ir� realizar o que queremos
        {
            return Ok($@"Ol� Mundo!!!"); // Retorna alguma informa��o com sucesso, lembrando que Ok � sucesso.
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
                return BadRequest("Clube n�o encontrado");
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
                return BadRequest("Jogador n�o encontrado");
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
                return BadRequest("Jogador n�o encontrado");
            }
        }

        [HttpPost("clube")]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequest), (int)HttpStatusCode.BadRequest)]
        public IActionResult InserirClube([FromBody] ClubeRequest clubeRequest)
        {
            if (String.IsNullOrEmpty(clubeRequest.Nome))
                return BadRequest("� necess�rio informar o nome do clube.");

            if (clubeRequest.Trofeus == null)
                return BadRequest("� necess�rio informar a quantidade de trof�us.");

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
                return BadRequest("Clube n�o foi cadastrado");
            }
        }

        [HttpPut("clube/{idClube}")]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequest), (int)HttpStatusCode.BadRequest)]
        public IActionResult AlterarClube([FromRoute] int idClube, [FromQuery] int quantidadeTrofeus, colocar aqui um parametro novo referente ao patrimonio do clube)
        {
            string connectionString = "Server=localhost;Port=3306;Database=sys;Uid=root;Pwd=admin;";

            MySqlConnection connection = new MySqlConnection(connectionString);

            string query = "UPDATE CLUBES SET TROFEUS = @quantidadeTrofeus, aqui deve ser feito a atribui��o do patrimonio WHERE ID = @idClube";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@quantidadeTrofeus", quantidadeTrofeus);
            command.Parameters.AddWithValue("@idClube", idClube);
            aqui deve conter mais um parametro

            connection.Open();

            var linhasAfetas = command.ExecuteNonQuery();

            if (linhasAfetas > 0)
            {
                connection.Close();
                return Ok("Atualiza��o ocorreu com sucesso.");
            }
            else
            {
                connection.Close();
                return BadRequest("N�o foi atualizado nenhum clube.");
            }
        }
    }
}
