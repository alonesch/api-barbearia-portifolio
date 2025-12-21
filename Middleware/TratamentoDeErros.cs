using System.Net;
using System.Text.Json;
using BarbeariaPortifolio.API.Exceptions;

namespace BarbeariaPortifolio.API.Middleware
{
    public class TratamentoDeErros
    {
        private readonly RequestDelegate _next;

        public TratamentoDeErros(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException erro)
            {
                Console.WriteLine(" AppException capturada:");
                Console.WriteLine(erro.ToString());

                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = erro.StatusCode;
                    context.Response.ContentType = "application/json";

                    var retorno = new { mensagem = erro.Message };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(retorno));
                }
            }
            catch (Exception erro)
            {
                Console.WriteLine("Exception NÃO TRATADA:");
                Console.WriteLine(erro.ToString());

                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var retorno = new
                    {
                        mensagem = "Erro interno do servidor.",
                        traceId = context.TraceIdentifier
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(retorno));
                }
            }
        }
    }
}
