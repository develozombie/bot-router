# RouterBot

Ejemplo de Bot que routea a otros bots secundarios manteniendo persistencia de sesión en memoria

## Pre-requisitos

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 3.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## Importante
- Deberás crear un archivo appsettings.json dentro del directorio RouterBot con la siguiente estructura:
```json
{
  "MicrosoftAppId": "",
  "MicrosoftAppPassword": "",
  //Configura las llaves de DirectChannel, QnA y Luis
  "Llaves": {
    "agente": "",
    "cuentas": "",
    "tarjetas": "",
    "preguntas": ""
  }
  }

```
