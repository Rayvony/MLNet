1. crear la base de datos con el uso del script: ScriptCreacionDatabase.sql
2. descomprimir el archivo image-classifier-assets.zip y copiar la carpeta assets dentro del proyecto MLNetMVC.Web
3. descomprimir el archivo inception5h.zip, copiar el contenido y pegar reemplazando los archivos en el destino,dentro del proyecto  MLNetMVC.Web, en la carpeta assets->inception
4. buildear la aplicacion previamente para realizar la correcta instalacion y referencia de los paquetes NuGet
5. crear el archivo "appsettings.json" en el proyecto web de la solucion con la siguiente configuracion:

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "MLNetProyectoDbContextConnection": "Server=TU-SERVIDOR;Database=MLNetProyecto;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
