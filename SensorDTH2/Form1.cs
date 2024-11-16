using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace SensorDTH2
{
    public partial class Form1 : Form
    {
        SerialPort serialPort;
        bool isConnected = false;
        public Form1()
        {
            InitializeComponent();
            LoadAvailablePorts();
        }

        private void LoadAvailablePorts()
        {
            comboBoxPorts.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            comboBoxPorts.Items.AddRange(ports);
            if (ports.Length > 0)
                comboBoxPorts.SelectedIndex = 0;
            else
                MessageBox.Show("No se encontraron puertos disponibles.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (!isConnected)
                ConnectToSerialPort();
            else
                DisconnectFromSerialPort();
        }

        private void ConnectToSerialPort()
        {
            try
            {
                serialPort = new SerialPort(comboBoxPorts.SelectedItem.ToString(), 9600);
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
                isConnected = true;
                buttonConnect.Text = "Desconectar";
                labelStatus.Text = "Conectado";
                labelStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisconnectFromSerialPort()
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                    serialPort.Dispose();
                }
                isConnected = false;
                buttonConnect.Text = "Conectar";
                labelStatus.Text = "Desconectado";
                labelStatus.ForeColor = System.Drawing.Color.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desconectar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Leer datos del puerto serial
                string data = serialPort.ReadLine();

                // Asegurarse de procesarlo en el hilo de la interfaz gráfica
                this.Invoke(new Action(() =>
                {
                    // Dividir los datos recibidos por el tabulador
                    string[] values = data.Split('\t');

                    // Validar que los datos estén completos (dos valores)
                    if (values.Length == 2 &&
                        float.TryParse(values[0], out float humedad) &&
                        float.TryParse(values[1], out float temperatura))
                    {
                        // Actualizar etiquetas en la interfaz gráfica
                        labelTemperature.Text = $"Temperatura: {temperatura} °C";
                        labelHumidity.Text = $"Humedad: {humedad} %";

                        // Agregar los datos al ListBox
                        string logEntry = $"Temp: {temperatura} °C | Hum: {humedad} %";
                        listBoxData.Items.Add(logEntry);

                        // Opcional: Desplazarse automáticamente al final de la lista
                        listBoxData.TopIndex = listBoxData.Items.Count - 1;
                    }
                    else
                    {
                        // Mostrar un mensaje en caso de error en los datos
                        listBoxData.Items.Add("Datos inválidos recibidos.");
                    }
                }));
            }
            catch (Exception ex)
            {
                // Manejo de errores durante la recepción de datos
                this.Invoke(new Action(() =>
                {
                    listBoxData.Items.Add($"Error: {ex.Message}");
                }));
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            LoadAvailablePorts();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            DisconnectFromSerialPort();
            Application.Exit();
        }
    }
}
