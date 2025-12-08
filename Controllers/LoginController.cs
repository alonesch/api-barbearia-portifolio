//using BarbeariaPortifolio.API.Servicos.Interfaces;
//using BarbeariaPortifolio.API.Exceptions;
//using Microsoft.AspNetCore.Mvc;

//namespace BarbeariaPortifolio.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class LoginController : ControllerBase
//    {
//        private readonly IAuthServico _auth;

//        public LoginController(IAuthServico auth)
//        {
//            _auth = auth;
//        }

//        public class LoginRequest
//        {
//            public string Usuario { get; set; } = string.Empty; // email OU username
//            public string Senha { get; set; } = string.Empty;
//        }

//        [HttpPost]
//        public async Task<IActionResult> Login([FromBody] LoginRequest request)
//        {
//            if (request == null ||
//                string.IsNullOrWhiteSpace(request.Usuario) ||
//                string.IsNullOrWhiteSpace(request.Senha))
//            {
//                throw new AppException("Credenciais inválidas.", 400);
//            }

//            var (accessToken, refreshToken, usuario) =
//                await _auth.LoginAsync(request.Usuario, request.Senha);

//            var barbeiroId = await _auth.BuscarBarbeiroId(usuario.Id);

//            return Ok(new
//            {
//                mensagem = "Login realizado com sucesso.",
//                dados = new
//                {
//                    token = accessToken,
//                    refreshToken = refreshToken,
//                    usuario = new
//                    {
//                        usuario.Id,
//                        usuario.NomeUsuario,
//                        usuario.NomeCompleto,
//                        usuario.Cargo,
//                        barbeiroId = barbeiroId ?? null
//                    }
//                }
//            });
//        }
//    }
//}
