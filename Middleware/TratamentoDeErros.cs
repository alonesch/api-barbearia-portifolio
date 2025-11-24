using System.Net;
using System.Text.Json;
using BarbeariaPortifolio.API.Exceptions;


namespace BarbeariaPortifolio.API.Middleware
{
    public class TratamentoDeErros
    {
        private readonly RequestDelegate _proximo;

        public TratamentoDeErros(RequestDelegate proximo)
        {
            _proximo = proximo;
        }

        public async Task InvokeAsync(HttpContext contexto)
        {
            try
            {
                {
                    await _proximo(contexto);
                }
            }
            catch (AppException erro)
            {
                if(!contexto.Response.HasStarted)
                {
                    contexto.Response.StatusCode = erro.StatusCode;
                    contexto.Response.ContentType = "application/json";
                
                    var retorno = new { mensagem = erro.Message };
                    await contexto.Response.WriteAsync(JsonSerializer.Serialize(retorno));
                }

            }
            catch (Exception)
            {
                if(!contexto.Response.HasStarted)
                {

                    contexto.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    contexto.Response.ContentType = "application/json";

                    var retorno = new { message = "Erro interno do servidor." };
                    await contexto.Response.WriteAsync(JsonSerializer.Serialize(retorno));

                }
            }
        }
    }
}