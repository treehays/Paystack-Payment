using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace testing.Models;
public class Test : ITest
{
    private readonly IConfiguration _configuration;

    public Test(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<PaystackResponse> MakePayment(double costOfProduct, string productNumber, string customerEmail)
    {

        //var senderName = _configuration.GetSection("Paystack")["Name"];
        var key = _configuration.GetSection("Paystack")["APIKey"];



        //"{\"status\":true,\"message\":\"Authorization URL created\",\"data\":{\"authorization_url\":\"https://checkout.paystack.com/6p9umo0wo05464x\",\"access_code\":\"6p9umo0wo05464x\",\"reference\":\"p7ijg33j7d\"}}"

        costOfProduct = 20000 * 100;//amount is in Kobo
        productNumber = Guid.NewGuid().ToString().Replace('-', 'y');
        customerEmail = "treehays90@gmail.com";

        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var endPoint = "https://api.paystack.co/transaction/initialize";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
        var content = new StringContent(JsonSerializer.Serialize(new
        {
            amount = costOfProduct,
            email = customerEmail,
            reference = productNumber,
            currency = "NGN",

        }), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(endPoint, content);
        //this is the respons"{\"status\":true,\"message\":\"Authorization URL created\",\"data\":{\"authorization_url\":\"https://checkout.paystack.com/3bazel0svhfum2e\",\"access_code\":\"3bazel0svhfum2e\",\"reference\":\"123456789023344\"}}"
        var resString = await response.Content.ReadAsStringAsync();
        var responseObj = JsonSerializer.Deserialize<PaystackResponse>(resString);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return responseObj;
        }

        return null;
    }



    public async Task<VerifyBank> VerifyByAccountNumber(string acNumber, string bankCode, decimal amount)
    {
        var key = _configuration.GetSection("Paystack")["APIKey"];
        acNumber = "0159192507";
        bankCode = "058";
        amount = 2000;

        var getHttpClient = new HttpClient();
        getHttpClient.DefaultRequestHeaders.Accept.Clear();
        getHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var baseUri = getHttpClient.BaseAddress = new Uri($"https://api.paystack.co/bank/resolve?account_number={acNumber}&bank_code={bankCode}");

        getHttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", key);
        var response = await getHttpClient.GetAsync(baseUri);
        var responseString = await response.Content.ReadAsStringAsync();
        //"{\"status\":true,\"message\":\"Account number resolved\",\"data\":{\"account_number\":\"0159192507\",\"account_name\":\"ABDULSALAM AHMAD AYOOLA\",\"bank_id\":9}}"
        var responseObject = JsonSerializer.Deserialize<VerifyBank>(responseString);

        if (response.StatusCode == HttpStatusCode.OK)
        {

            return responseObject;

        }

        return responseObject;
    }




    public async Task<GenerateRecipientDTO> GenerateRecipients(VerifyBank verifyBank)
    {
        var key = _configuration.GetSection("Paystack")["APIKey"];
        var getHttpClient = new HttpClient();
        getHttpClient.DefaultRequestHeaders.Accept.Clear();
        getHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var baseUri = getHttpClient.BaseAddress = new Uri($"https://api.paystack.co/transferrecipient");
        getHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
        var response = await getHttpClient.PostAsJsonAsync(baseUri, new
        {
            type = "nuban",
            name = "ABDULSALAM AHMAD AYOOLA",
            account_number = "0159192507",
            bank_code = "058",
            currency = "NGN",
        });
        //"{\"status\":true,\"message\":\"Transfer recipient created successfully\",\"data\":{\"active\":true,\"createdAt\":\"2020-10-22T11:50:38.000Z\",\"currency\":\"NGN\",\"description\":\"\",\"domain\":\"live\",\"email\":null,\"id\":9698733,\"integration\":519010,\"metadata\":null,\"name\":\"ABDULSALAM AHMAD AYOOLA\",\"recipient_code\":\"RCP_r4x2zmwjhza5m8t\",\"type\":\"nuban\",\"updatedAt\":\"2023-04-20T02:34:11.000Z\",\"is_deleted\":false,\"isDeleted\":false,\"details\":{\"authorization_code\":null,\"account_number\":\"0159192507\",\"account_name\":\"ABDULSALAM AHMAD AYOOLA\",\"bank_code\":\"058\",\"bank_name\":\"Guaranty Trust Bank\"}}}"
        var responseString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<GenerateRecipientDTO>(responseString);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (!responseObject.status)
            {
                return new GenerateRecipientDTO()
                {
                    status = false,
                    message = responseObject.message
                };
            }
            return new GenerateRecipientDTO()
            {
                status = true,
                message = "Recipient Generated",
                data = responseObject.data
            };
        }

        return new GenerateRecipientDTO()
        {
            status = false,
            message = responseObject.message
        };
    }





    public async Task<MakeATransfer> SendMoney(string recip, decimal amount)
    {
        var key = _configuration.GetSection("Paystack")["APIKey"];
        recip = "RCP_r4x2zmwjhza5m8t";
        amount = 23344;

        var getHttpClient = new HttpClient();
        getHttpClient.DefaultRequestHeaders.Accept.Clear();
        getHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var baseUri = $"https://api.paystack.co/transfer";
        getHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
        var response = await getHttpClient.PostAsJsonAsync(baseUri, new
        {
            recipient = recip,
            amount = amount * 100,
            currency = "NGN",
            source = "balance"
        });
        var responseString = await response.Content.ReadAsStringAsync();
        //
        var responseObject = JsonSerializer.Deserialize<MakeATransfer>(responseString);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            if (!responseObject.status)
            {
                return new MakeATransfer()
                {
                    status = false,
                    message = responseObject.message
                };
            }
            return new MakeATransfer()
            {
                status = true,
                message = responseObject.message,
                data = responseObject.data
            };
        }
        return new MakeATransfer()
        {
            status = false,
            message = "Pls retry payment is not successfull"
        };
    }



}




public class PaymentRequestModel
{
    public int ApartmentId { get; set; }
    public int CustomerId { get; set; }
    public decimal AmountPaid { get; set; }
}
public class PaystackResponse
{
    public bool status { get; set; }
    public string message { get; set; }
    public PaystackData data { get; set; }
}
public class PaystackData
{
    public string authorization_url { get; set; }
    public string access_code { get; set; }
    public string reference { get; set; }
}


public class SendMoneyDto
{
    public string AccountNumber { get; set; }
    public string BankCode { get; set; }
    public decimal Amount { get; set; }
}


public class VerifyBank
{
    public bool status { get; set; }
    public string message { get; set; }
    public VerifyBankData data { get; set; }

}


public class VerifyBankData
{
    public string account_number { get; set; }
    public string account_name { get; set; }
    public string bank_code { get; set; }
    public int bank_id { get; set; }
    public string recipient_code { get; set; }


    public string reference { get; set; }
    public int integration { get; set; }
    public string domain { get; set; }
    public string amount { get; set; }
    public string currency { get; set; }
    public string source { get; set; }
    public string reason { get; set; }
    public int recipient { get; set; }
    public string status { get; set; }
    public string transfer_code { get; set; }
    public int id { get; set; }
    public string createdAt { get; set; }
    public string updatedAt { get; set; }

}

public class GenerateRecipientDTO
{
    public bool status { get; set; }
    public string message { get; set; }

    public GenerateRecipientData data { get; set; }
}
public class GenerateRecipientData
{
    public bool active { get; set; }
    public DateTime createdAt { get; set; }
    public string currency { get; set; }
    public string domain { get; set; }
    public int id { get; set; }
    public int integration { get; set; }
    public string name { get; set; }
    public string reference { get; set; }
    public string reason { get; set; }
    public string recipient_code { get; set; }
    public string type { get; set; }
    public VerifyBankData details { get; set; }
}

public class MakeATransfer
{
    public bool status { get; set; }
    public string message { get; set; }
    public MakeATransferData data { get; set; }
}

public class MakeATransferData
{
    public string reference { get; set; }
    public int integration { get; set; }
    public string domain { get; set; }
    public string amount { get; set; }
    public string currency { get; set; }
    public string source { get; set; }
    public string reason { get; set; }
    public int recipient { get; set; }
    public string status { get; set; }
    public string transfer_code { get; set; }
    public int id { get; set; }
    public string createdAt { get; set; }
    public string updatedAt { get; set; }
}



public class BaseResponse
{
    public string Message { get; set; }
    public bool Status { get; set; }
}
