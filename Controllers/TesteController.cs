using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using MySql.Data.MySqlClient;
using futebol.Objetos;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;

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
                WHERE j.salario = SECT max(x.salario) FROM sys.jogadores x";

            MySqlCommand command = new MySqlCommand(query, connection);

            connection.Open();

            MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                var jogadorMaiorSalario = new MaiorSalario();
                // Adicionar apenas as informações que retornarão da API

                connection.Close();
                return Ok(jogadorMaiorSalario);
            }
            else
            {
                connection.Close();
                return BadRequest("Jogador não encontrado");
            }
        }
    }
}
