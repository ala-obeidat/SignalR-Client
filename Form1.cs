using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;

namespace SiganR.Client
{
    public partial class Form1 : Form
    {
        private HubConnection connection;
        public Form1()
        {
            InitializeComponent();
            btnSend.Enabled = false;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            await OpenConnection();
        }
        private void EstablishConnection()
        {
            if (string.IsNullOrEmpty(txtUrl.Text))
            {
                ShowAlert("Please Fill URL to connect");
                return;
            }
            if (connection == null)
            {
                connection = new HubConnectionBuilder()
                    .WithUrl(txtUrl.Text).WithAutomaticReconnect()
                    .Build();

                connection.Closed += async (error) =>
                {
                    AppendResult("Connection Closed");
                    AppendResult(error);
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await OpenConnection();
                };
            }
        }
        private void ShowAlert(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private async Task OpenConnection()
        {
            try
            {
                EstablishConnection();
                await connection.StartAsync();
                AppendResult("Connection started");
                btnSend.Enabled = true;
                btnConnect.Enabled = false;
            }
            catch (Exception ex)
            {
                AppendResult(ex);
            }
        }
        private void AppendResult(Exception ex)
        {

            txtResult.Text += $"\nError: {ex.Message}";
        }
        private void AppendResult(string message)
        {

            txtResult.Text += $"\n{message}";
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (connection.State != HubConnectionState.Connected)
                {
                    AppendResult("Reconnecting to Hup");
                    await OpenConnection();
                }
                if (string.IsNullOrEmpty(txtMethod.Text))
                {
                    ShowAlert("Please Fill Method to send");
                    return;
                }
                if (string.IsNullOrEmpty(txtPayload.Text) || !string.IsNullOrEmpty(txtArgument.Text))
                {
                    object result = null;
                    if (string.IsNullOrEmpty(txtArgument.Text))
                    {
                        result = await connection.InvokeAsync<object>(txtMethod.Text);
                    }
                    else
                    {
                        if (chbInteger.Checked)
                        {
                            result = await SendMethod(txtArgument.Text.Split(',').Select(x=>Convert.ToInt32(x)).ToArray());
                        }
                        else
                        {
                            result = await SendMethod(txtArgument.Text.Split(','));
                        }
                        
                    }
                    if (result != null)
                    {
                        AppendResult($"Response: {result.ToString()}");
                    }
                }
                else
                {
                    var model = JsonSerializer.Deserialize<object>(txtPayload.Text);
                    await connection.InvokeAsync(txtMethod.Text, model);
                }
                AppendResult($"{txtMethod.Text} Sended successfully");
            }
            catch (Exception ex)
            {
                AppendResult(ex);
            }
        }

        private async Task<object> SendMethod(string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    return await connection.InvokeAsync<object>(txtMethod.Text, args[0]);
                case 2:
                    return await connection.InvokeAsync<object>(txtMethod.Text, args[0], args[1]);
                case 3:
                    return await connection.InvokeAsync<object>(txtMethod.Text, args[0], args[1], args[2]);
                case 4:
                    return await connection.InvokeAsync<object>(txtMethod.Text, args[0], args[1], args[2], args[3]);
                case 5:
                    return await connection.InvokeAsync<object>(txtMethod.Text, args[0], args[1], args[2], args[3], args[4]);
                case 6:
                    return await connection.InvokeAsync<object>(txtMethod.Text, args[0], args[1], args[2], args[3], args[4], args[5]);
            }

            return null;
        }
        private async Task<object> SendMethod(int[] args)
        {
            switch (args.Length)
            {
                case 1:
                    return await connection.InvokeAsync<object>(txtMethod.Text, args[0]);
                case 2:
                    return await connection.InvokeAsync<object>(txtMethod.Text, args[0], args[1]);
                case 3:
                    return await connection.InvokeAsync<object>(txtMethod.Text, args[0], args[1], args[2]);
                case 4:
                    return await connection.InvokeAsync<object>(txtMethod.Text, args[0], args[1], args[2], args[3]);
                case 5:
                    return await connection.InvokeAsync<object>(txtMethod.Text, args[0], args[1], args[2], args[3], args[4]);
                case 6:
                    return await connection.InvokeAsync<object>(txtMethod.Text, args[0], args[1], args[2], args[3], args[4], args[5]);
            }

            return null;
        }
    }
}
