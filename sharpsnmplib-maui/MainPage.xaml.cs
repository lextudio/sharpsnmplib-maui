using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using System.Net;

namespace sharpsnmplib_maui;

public partial class MainPage : ContentPage
{
    const int timeOut = 5000; // 5 seconds.

    public MainPage()
	{
		InitializeComponent();
	}

    public async void Button_Clicked(object sender, EventArgs args)
    {
        try
        {
            btnTest.IsEnabled = false;            
            txtResult.Text = string.Empty;
            txtResult.IsVisible = true;
            if (!IPAddress.TryParse(txtAddress.Text, out IPAddress address))
            {
                txtResult.Text = "Please provide a valid IP address.";
                return;
            }

            if (!int.TryParse(txtPort.Text, out int port) || port <= 0 || port > 65535)
            {
                txtResult.Text = "Please provide a valid port number.";
                return;
            }

            aiLoading.IsVisible = true;
            aiLoading.IsRunning = true;

            var source = new CancellationTokenSource();
            source.CancelAfter(timeOut);

            var result = await Messenger.GetAsync(
                VersionCode.V1,
                new IPEndPoint(address, port),
                new OctetString(txtCommunity.Text),
                new List<Variable> { new Variable(new ObjectIdentifier("1.3.6.1.2.1.1.1.0")) },
                source.Token);

            if (result.Count != 1)
            {
                txtResult.Text = "Invalid response data.";
                return;
            }

            txtResult.Text = result[0].Data.ToString();
        }
        catch (OperationCanceledException)
        {
            txtResult.Text = "SNMP GET operation timed out.";
        }
        catch (Exception ex)
        {
            txtResult.Text = ex.Message;
        }
        finally
        {
            aiLoading.IsVisible = false;
            aiLoading.IsRunning = false;
            btnTest.IsEnabled = true;
        }
    }
}

