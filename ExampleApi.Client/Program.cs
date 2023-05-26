// See https://aka.ms/new-console-template for more information
using ExampleApi.Shared;
using Newtonsoft.Json;
using System.Text;


RegisterUser();
Console.ReadKey();
async void RegisterUser()
{
    HttpClient client = new HttpClient();
    var model = new RegisterViewModel
    {
        Email = "Test@ab.com",
        Password = "123qweR!",
        ConfirmPassword = "123qweR!",
    };
    var jsonData = JsonConvert.SerializeObject(model);
    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
    var response = await client.PostAsync("https://localhost:7282/api/auth/register", content);
    var responseBody = await response.Content.ReadAsStringAsync();
    var responseObject = JsonConvert.DeserializeObject<UserManagerResponse>(responseBody);
    if(responseObject == null)
    {
        Console.WriteLine("Response is null!");
    }
    else if (responseObject.IsSuccess)
    {
        Console.WriteLine("Your account has been created successfully!");
    }
    else
    {
        Console.WriteLine(responseObject.Errors.FirstOrDefault());
    }
}
