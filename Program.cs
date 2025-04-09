namespace ChatGPTAPITest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var handler = new ChatGPTAPIHandler();

            Console.WriteLine("Prompt: ");
            string prompt = Console.ReadLine() ?? "";
            Console.WriteLine("Kérés: ");
            string request = Console.ReadLine() ?? "";

            string response = await handler.MakeRequest(prompt, request);
            Console.WriteLine(response);
        }
    }
}
