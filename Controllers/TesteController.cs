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
    }
}
