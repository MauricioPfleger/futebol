using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using MySql.Data.MySqlClient;
using futebol.Objetos;
using System.Data;
using System.Diagnostics.Eventing.Reader;

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
        public IActionResult ConsultaClube([FromQuery] string nomeClube, [FromQuery] int valor) 
        {
            string connectionString = "Server=localhost;Port=3306;Database=sys;Uid=root;Pwd=admin;";

            MySqlConnection connection = new MySqlConnection(connectionString);

            string query = String.Empty;

            if (String.IsNullOrEmpty(nomeClube)) {
                // a query deve buscar pelo id
            }
            else {
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
            else {
                connection.Close();
                return BadRequest("Clube não encontrado");
            }
        }
    }
}
