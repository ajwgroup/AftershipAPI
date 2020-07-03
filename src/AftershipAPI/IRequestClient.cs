namespace AftershipAPI
{
    public interface IRequestClient
    {
        string RunRequest(string method, string body, string url);
    }
}