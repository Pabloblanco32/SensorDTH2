#include <Adafruit_Sensor.h>
#include <DHT.h>
#include <DHT_U.h>

// Configuración del pin y tipo de sensor DHT
#define DHTPIN 2      // Pin digital donde está conectado el sensor
#define DHTTYPE DHT11 // Cambia a DHT22 si usas ese modelo

// Inicialización del sensor DHT
DHT dht(DHTPIN, DHTTYPE);

void setup() {
  Serial.begin(9600); // Inicializamos la comunicación serial
  dht.begin();        // Inicializamos el sensor DHT
  Serial.println("Sensor DHT listo para enviar datos.");
}

void loop() {
  // Leer la humedad relativa
  float humedad = dht.readHumidity();
  // Leer la temperatura en grados Celsius
  float temperatura = dht.readTemperature();

  // Verificar si las lecturas son válidas
  if (isnan(humedad) || isnan(temperatura)) {
    Serial.println("Error al leer los datos del sensor.");
    delay(2000); // Esperar 2 segundos antes de intentar nuevamente
    return;
  }

  // Enviar los datos en el formato esperado por el programa C#
  Serial.print(humedad);       // Enviar humedad
  Serial.print("\t");          // Separador de tabulador
  Serial.println(temperatura); // Enviar temperatura y nueva línea

  delay(2000); // Esperar 2 segundos entre lecturas
}
