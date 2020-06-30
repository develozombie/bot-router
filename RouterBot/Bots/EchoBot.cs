// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.9.2

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RouterBot.Model;
using Activity = Microsoft.Bot.Schema.Activity;

namespace RouterBot.Bots
{
    public class EchoBot : ActivityHandler
    {

        private static readonly string[] _menuActividades =
        {
            "tarjetas",
            "agente",
            "cuentas"
        };
        private BotState _conversationState;
        private BotState _userState;
        private readonly IConfiguration Configuration;
        public EchoBot(ConversationState conversationState, UserState userState, IConfiguration configuration)
        {
            Configuration = configuration;
            _conversationState = conversationState;
            _userState = userState;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            /**
             * Inicialización de variables.
             * replyText: Referencia a la respuesta construida luego de recibir un intent del Menu
             * repy: Referencia a la actividad de mostrar el Menu
             * userStateAccessors y conversacion: Referencia a las variables de almacenamiento de datos de sesión en memoria, esto 
             * debe ser reemplazado por almacenamiento persistente en producción.
             */
            string replyText = "";
            Activity reply;
            var respuesta = turnContext.Activity.Text;
            var userStateAccessors = _userState.CreateProperty<Conversacion>(nameof(Conversacion));
            var conversacion = await userStateAccessors.GetAsync(turnContext, () => new Conversacion());
            if (!string.IsNullOrEmpty(respuesta))
            {
                if (!string.IsNullOrEmpty(conversacion.Eleccion))
                {
                    conversacion.Cambio = false;
                    switch (conversacion.Eleccion)
                    {
                        case "tarjetas":
                            if (_menuActividades.Contains(respuesta.ToLower()))
                            {
                                conversacion.Eleccion = respuesta;
                                conversacion.Cambio = true;
                                await ConsultarBotAsync(turnContext, Configuration[$"Llaves:{respuesta.ToLower()}"]);
                                await turnContext.SendActivityAsync($"Perfecto, ahora estás comunicado con el área de {respuesta}, cómo puedo ayudarte?");
                            }
                            if (respuesta.ToLower().Equals("menu"))
                            {
                                reply = MostrarMenuInicial();
                                await turnContext.SendActivityAsync(reply, cancellationToken);
                            }
                            if(!conversacion.Cambio && !respuesta.ToLower().Equals("menu"))
                                replyText = ConsultarBotAsync(turnContext, Configuration["Llaves:Tarjetas"]).Result;                       
                            break;
                        case "agente":
                            if (_menuActividades.Contains(respuesta.ToLower()))
                            {
                                conversacion.Eleccion = respuesta;
                                conversacion.Cambio = true;
                                await turnContext.SendActivityAsync($"Perfecto, ahora estás comunicado con el área de {respuesta}, cómo puedo ayudarte?");
                            }
                            if (respuesta.ToLower().Equals("menu"))
                            {
                                reply = MostrarMenuInicial();
                                await turnContext.SendActivityAsync(reply, cancellationToken);
                            }
                            if (!conversacion.Cambio && !respuesta.ToLower().Equals("menu"))
                                await turnContext.SendActivityAsync("Acá funcionará el Handoff del Bot");
                            break;
                        case "cuentas":
                            if (_menuActividades.Contains(respuesta.ToLower()))
                            {
                                conversacion.Eleccion = respuesta;
                                conversacion.Cambio = true;
                                await ConsultarBotAsync(turnContext, Configuration[$"Llaves:{respuesta.ToLower()}"]);
                                await turnContext.SendActivityAsync($"Perfecto, ahora estás comunicado con el área de {respuesta}, cómo puedo ayudarte?");
                            }
                            if (respuesta.ToLower().Equals("menu"))
                            {
                                reply = MostrarMenuInicial();
                                await turnContext.SendActivityAsync(reply, cancellationToken);
                            }
                            if (!conversacion.Cambio && !respuesta.ToLower().Equals("menu"))
                                replyText = ConsultarBotAsync(turnContext, Configuration["Llaves:Cuentas"]).Result;
                            break;
                    }
                }
                else
                {
                    switch (respuesta)
                    {
                        case "menu":
                            reply = MostrarMenuInicial();
                            await turnContext.SendActivityAsync(reply, cancellationToken);
                            break;
                        case "tarjetas":
                            conversacion.Eleccion = respuesta;
                            await turnContext.SendActivityAsync($"Perfecto, ahora estás comunicado con el área de {respuesta}, cómo puedo ayudarte?");
                            break;
                        case "agente":
                            conversacion.Eleccion = respuesta;
                            await turnContext.SendActivityAsync($"Acá funcionará el Handoff del Bot");
                            break;
                        case "cuentas":
                            conversacion.Eleccion = respuesta;
                            await turnContext.SendActivityAsync($"Perfecto, ahora estás comunicado con el área de {respuesta}, cómo puedo ayudarte?");
                            break;
                        default:
                            await turnContext.SendActivityAsync($"Disculpame {turnContext.Activity.From.Name}, no te he entendido");
                            reply = MostrarMenuInicial();
                            await turnContext.SendActivityAsync(reply, cancellationToken);
                            break;
                    }
                }
            }
            if(!string.IsNullOrEmpty(replyText))
                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    Activity reply = MostrarMenuInicial();
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }

        public static Activity MostrarMenuInicial()
        {
            var reply = MessageFactory.Text("¿De qué deseas hablar?, siempre puedes volver a ver estas opciones escribiendo \"menu\" sin comillas");
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                },
            };
            foreach (var item in _menuActividades)
            {
                reply.SuggestedActions.Actions.Add(new CardAction()
                {
                    Title = item,
                    Value = item.ToLower(),
                    Type = ActionTypes.ImBack
                });
            }
            return reply;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.  
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        public async Task<string> ConsultarBotAsync(ITurnContext turnContext, string DirectToken)
        {
            var userStateAccessors = _userState.CreateProperty<Conversacion>(nameof(Conversacion));
            var conv = await userStateAccessors.GetAsync(turnContext, () => new Conversacion());
            string token = "", conversacion = "";
            string idconversacion, URI, HtmlResult;
            JObject textoresult;
            if(conv.Cambio)
            {
                conv.Token = conv.Conv = null;
                conv.Cambio = false;
            }
            using (WebClient wc = new WebClient())
            {
                //inicializa en almacenamiento en memoria si es que el token y el id de la conversación no ha sido generado
                if (string.IsNullOrEmpty(conv.Token) || string.IsNullOrEmpty(conv.Conv))
                {
                    //obtener token y conversacion
                    URI = "https://directline.botframework.com/v3/directline/conversations";
                    wc.Headers["Authorization"] = $"Bearer {DirectToken}";
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    HtmlResult = wc.UploadString(URI, "POST");
                    textoresult = JObject.Parse(HtmlResult);
                    token = textoresult["token"].ToString();
                    conversacion = textoresult["conversationId"].ToString();
                    conv.Token = token;
                    conv.Conv = conversacion;
                }
                else
                {
                    token = conv.Token;
                    conversacion = conv.Conv;
                }

                //enviar mensaje al bot
                var pregunta = new
                {
                    type = "message",
                    from = new
                    {
                        id = turnContext.Activity.From.Name
                    },
                    text = turnContext.Activity.Text
                };
                var dataString = Newtonsoft.Json.JsonConvert.SerializeObject(pregunta);
                URI = $"https://directline.botframework.com/v3/directline/conversations/{conversacion}/activities";
                wc.Headers["Authorization"] = $"Bearer {token}";
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                HtmlResult = wc.UploadString(URI, "POST", dataString);
                textoresult = JObject.Parse(HtmlResult);
                var arrayid = textoresult["id"].ToString().Split("|");
                idconversacion = arrayid[1].ToString();

                //recibir mensaje del bot
                URI = $"https://directline.botframework.com/v3/directline/conversations/{conversacion}/activities?watermark={idconversacion}";
                wc.Headers["Authorization"] = $"Bearer {token}";
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                HtmlResult = wc.DownloadString(URI);
                textoresult = JObject.Parse(HtmlResult);
                JObject textoArray = (JObject)(textoresult.SelectToken("activities") as JArray).First();
                var resupuesta = textoArray.Value<string>("text"); 
                return resupuesta;
            }
        }
    }
}
